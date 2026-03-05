using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour {
	public static GameManager instance;

	public UIScreenInterface uiScreen;

	public int statBugsKilledByPlayer;
	public int statBugsKilledByTurrets;
	public int statHivesKilledByPlayer;
	public int statHivesKilledByTurrets;
	public int statBuildingsDestroyed;
	public int statBuildingsTotal;
	public int statBuildingsDestroyedByPlayer;

	public int bugCount;
	public int hiveCount;
	public float spawnRateMultiplier = 1.0F;
	public double gameTime;
	public bool gameOver;

	public int bugCap;

	// So we can efficiently render all of them in one call as well as find targets (there will be a lot of these, we need this to be fast)
	public Material groundBugMaterial;
	public Material groundBugOutlineMaterial;
	const int groundBugsToTargetAndAttackPerTick = 50;
	int currentGroundBugTargetIdx = 0;
	List<EnemyGround> allGroundBugs = new();
	GraphicsBuffer groundEnemyLocalToWorlds;
	GraphicsBuffer groundEnemyAnimTimes;

	// So we can efficiently target buildings
	List<BuildingController> allBuildings = new();
	List<Vector3> buildingPositions = new();

	// For amortized bug targeting for turrets
	List<TurretRailgunController> turrets = new();
	Collider[] overlapTestArray = new Collider[128];
	const int turretsToTargetPerTick = 5;
	int currentTurretTargetIdx = 0;

	const float GRID_SIZE = 400.0F;
	const int GRID_RESOLUTION = 400;

	public Vector3 CITY_CENTER = new Vector3(2.014788F, 0.0F, 2.673001F);

	[System.Serializable]
	public struct Wave {
		[SerializeField]
		public List<HiveController> hives;
		[SerializeField]
		public float spawnRateMin;
		[SerializeField]
		public float spawnRateMax;
		[SerializeField]
		public int groupMemberCount;
		[SerializeField]
		public float groupDelay;
		[SerializeField]
		public float hiveHealth;
	}
	public List<Wave> waves = new();
	public int currentWave = 0;
	float nextWaveCountdownTimer = 0;
	bool needsNextWave;

	byte[] directions = new byte[GRID_RESOLUTION * GRID_RESOLUTION];

	Vector2[] directionByteToDirection = new Vector2[8]{
		Vector2.Normalize(new Vector2(-1, -1)),
		Vector2.Normalize(new Vector2(0, -1)),
		Vector2.Normalize(new Vector2(1, -1)),
		Vector2.Normalize(new Vector2(-1, 0)),
		Vector2.Normalize(new Vector2(1, 0)),
		Vector2.Normalize(new Vector2(-1, 1)),
		Vector2.Normalize(new Vector2(0, 1)),
		Vector2.Normalize(new Vector2(1, 1))
	};

	public Vector2 GetDirectionToCity(Vector2 pos) {
		int x = Mathf.RoundToInt((pos.x / GRID_SIZE + 0.5F) * GRID_RESOLUTION);
		int y = Mathf.RoundToInt((pos.y / GRID_SIZE + 0.5F) * GRID_RESOLUTION);
		if (x < 0 || y < 0 || x >= GRID_RESOLUTION || y >= GRID_RESOLUTION) {
			return Vector2.zero;
		}
		byte rotation = directions[y * GRID_RESOLUTION + x];
		return directionByteToDirection[rotation];
	}
	void Awake() {
		instance = this;
	}

	void OnDestroy() {
		groundEnemyLocalToWorlds?.Dispose();
		groundEnemyLocalToWorlds = null;
		groundEnemyAnimTimes?.Dispose();
		groundEnemyAnimTimes = null;
	}
	
	void IncrementWave() {
		foreach (HiveController controller in waves[currentWave].hives) {
			controller.gameObject.SetActive(false);
		}
		hiveCount = 0;
		currentWave++;
		uiScreen.ShowNextWaveIndicator();
		needsNextWave = true;
		nextWaveCountdownTimer = 4.0F;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		foreach (HiveController controller in FindObjectsByType<HiveController>(FindObjectsSortMode.None)) {
			controller.gameObject.SetActive(false);
		}
		foreach (HiveController controller in waves[currentWave].hives) {
			controller.gameObject.SetActive(true);
			hiveCount++;
			controller.WaveInit(waves[currentWave]);
		}

		groundEnemyLocalToWorlds = new GraphicsBuffer(GraphicsBuffer.Target.Structured, bugCap, 16 * sizeof(float));
		groundEnemyAnimTimes = new GraphicsBuffer(GraphicsBuffer.Target.Structured, bugCap, 4 * sizeof(float));

		float gridCellHalfSize = 0.5F * GRID_SIZE / GRID_RESOLUTION;
		int[] costs = new int[GRID_RESOLUTION * GRID_RESOLUTION];
		for (int y = 0; y < GRID_RESOLUTION; y++) {
			float cellY = ((float)y / GRID_RESOLUTION - 0.5F) * GRID_SIZE;
			for (int x = 0; x < GRID_RESOLUTION; x++) {
				float cellX = ((float)x / GRID_RESOLUTION - 0.5F) * GRID_SIZE;
				bool hasCollider = false;
				foreach (Collider collider in Physics.OverlapBox(new Vector3(cellX + gridCellHalfSize, 0.0F, cellY + gridCellHalfSize), new Vector3(gridCellHalfSize, 2.0F, gridCellHalfSize))) {
					if (collider.GetComponent<BugCollision>()) {
						hasCollider = true;
						break;
					}
				}
				costs[y * GRID_RESOLUTION + x] = hasCollider ? 1000000 : 1;
			}
		}
		int[] distances = new int[GRID_RESOLUTION * GRID_RESOLUTION];
		for (int i = 0; i < distances.Length; i++) {
			distances[i] = int.MaxValue;
		}
		// Will replace with something better once we get the actual city mechanics in
		Vector2 cityPos = new Vector2(CITY_CENTER.x, CITY_CENTER.z);
		int targetX = Mathf.RoundToInt((cityPos.x / GRID_SIZE + 0.5F) * GRID_RESOLUTION);
		int targetY = Mathf.RoundToInt((cityPos.y / GRID_SIZE + 0.5F) * GRID_RESOLUTION);
		costs[targetY * GRID_RESOLUTION + targetX] = 0;
		distances[targetY * GRID_RESOLUTION + targetX] = 0;
		Queue<Vector2Int> queue = new Queue<Vector2Int>(GRID_RESOLUTION * GRID_RESOLUTION);
		queue.Enqueue(new Vector2Int(targetX, targetY));
		Vector2Int[] neighbors = new Vector2Int[8]{
			new Vector2Int(-1, -1),
			new Vector2Int(0, -1),
			new Vector2Int(1, -1),
			new Vector2Int(-1, 0),
			new Vector2Int(1, 0),
			new Vector2Int(-1, 1),
			new Vector2Int(0, 1),
			new Vector2Int(1, 1)
		};
		// Approximate sqrt(2) distance metric for diagonals
		int[] directionCosts = new int[8] { 7, 5, 7, 5, 5, 7, 5, 7 };
		while (queue.Count != 0) {
			Vector2Int cellPos = queue.Dequeue();
			int currentDistance = distances[cellPos.y * GRID_RESOLUTION + cellPos.x];
			for (int directionIdx = 0; directionIdx < 8; directionIdx++) {
				Vector2Int neighborPos = cellPos - neighbors[directionIdx];
				if (neighborPos.x >= 0 && neighborPos.x < GRID_RESOLUTION && neighborPos.y >= 0 && neighborPos.y < GRID_RESOLUTION) {
					int neighborDistance = distances[neighborPos.y * GRID_RESOLUTION + neighborPos.x];
					int newNeighborDistance = unchecked(currentDistance + costs[neighborPos.y * GRID_RESOLUTION + neighborPos.x] + directionCosts[directionIdx]);
					newNeighborDistance = newNeighborDistance < 0 ? int.MaxValue : newNeighborDistance;
					if (newNeighborDistance < neighborDistance) {
						distances[neighborPos.y * GRID_RESOLUTION + neighborPos.x] = newNeighborDistance;
						directions[neighborPos.y * GRID_RESOLUTION + neighborPos.x] = (byte)directionIdx;
						queue.Enqueue(neighborPos);
					}
				}
			}
		}
	}
	void DrawDebugPathingVisualization() {
		for (int y = 0; y < GRID_RESOLUTION; y++) {
			for (int x = 0; x < GRID_RESOLUTION; x++) {
				byte rotation = directions[y * GRID_RESOLUTION + x];
				Vector2 dir = directionByteToDirection[rotation];
				Vector3 startPos = new Vector3(((float)x / GRID_RESOLUTION - 0.5F) * GRID_SIZE, 2.0F, ((float)y / GRID_RESOLUTION - 0.5F) * GRID_SIZE);
				Debug.DrawRay(startPos, new Vector3(0.0F, 1.0F, 0.0F), Color.red);
				Debug.DrawRay(startPos, new Vector3(dir.x, 0.0F, dir.y) * 0.4F);
			}
		}
	}
	public int randomGlobal = 1337;
	void BurnCycles(int amount) {
		for (int i = 0; i < amount; i++) {
			randomGlobal = (randomGlobal >> 2 ^ randomGlobal << 6) * 11;
		}
	}

	public string get_stats_string() {
		return
			"Bugs killed by you: " + statBugsKilledByPlayer + "\n" +
			"Bugs killed by turrets: " + statBugsKilledByTurrets + "\n" +
			"Hives killed by you: " + statHivesKilledByPlayer + "\n" +
			"Hives killed by turrets: " + statHivesKilledByTurrets + "\n" +
			"Buildings destroyed: " + statBuildingsDestroyed + "/" + statBuildingsTotal + "\n" +
			"Buildings destroyed by you: " + statBuildingsDestroyedByPlayer;
	}

	public int RegisterGroundBug(EnemyGround enemy) {
		int id = allGroundBugs.Count;
		allGroundBugs.Add(enemy);
		return id;
	}
	public void RemoveGroundBug(int id) {
		if (id < allGroundBugs.Count) {
			allGroundBugs[id] = allGroundBugs.Last();
			allGroundBugs[id].gameManagerRegisteredIdx = id;
			allGroundBugs.RemoveAt(allGroundBugs.Count - 1);
		}
	}
	public int RegisterTurret(TurretRailgunController turret) {
		int id = turrets.Count;
		turrets.Add(turret);
		return id;
	}
	public void RemoveTurret(int id) {
		if (id < allGroundBugs.Count) {
			turrets[id] = turrets.Last();
			turrets[id].gameManagerRegisteredIdx = id;
			turrets.RemoveAt(turrets.Count - 1);
		}
	}
	public int RegisterBuilding(BuildingController building) {
		int id = allBuildings.Count;
		allBuildings.Add(building);
		buildingPositions.Add(building.transform.position);
		statBuildingsTotal++;
		return id;
	}
	public void RemoveBuilding(int id) {
		if (id < allBuildings.Count) {
			allBuildings[id] = allBuildings.Last();
			allBuildings[id].gameManagerRegisteredIdx = id;
			allBuildings.RemoveAt(allBuildings.Count - 1);
			buildingPositions[id] = buildingPositions.Last();
			buildingPositions.RemoveAt(buildingPositions.Count - 1);
		}
	}

	// Update is called once per frame
	void Update() {
		//DrawDebugPathingVisualization();
		//BurnCycles(10000000);
		int bugsToDraw = Mathf.Min(allGroundBugs.Count, bugCap);
		if (bugsToDraw > 0) {
			Unity.Collections.NativeArray<float> matrices = new(allGroundBugs.Count * 16, Unity.Collections.Allocator.Temp);
			Unity.Collections.NativeArray<float> animTimes = new(allGroundBugs.Count * 4, Unity.Collections.Allocator.Temp);
			float animFrames = 41 - 15;
			float animLength = animFrames / 24.0F;
			for (int i = 0; i < bugsToDraw; i++) {
				EnemyGround bug = allGroundBugs[i];
				Matrix4x4 l2w = bug.transform.localToWorldMatrix;
				matrices[i * 16 + 0] = l2w.m00;
				matrices[i * 16 + 1] = l2w.m10;
				matrices[i * 16 + 2] = l2w.m20;
				matrices[i * 16 + 3] = l2w.m30;
				matrices[i * 16 + 4] = l2w.m01;
				matrices[i * 16 + 5] = l2w.m11;
				matrices[i * 16 + 6] = l2w.m21;
				matrices[i * 16 + 7] = l2w.m31;
				matrices[i * 16 + 8] = l2w.m02;
				matrices[i * 16 + 9] = l2w.m12;
				matrices[i * 16 + 10] = l2w.m22;
				matrices[i * 16 + 11] = l2w.m32;
				matrices[i * 16 + 12] = l2w.m03;
				matrices[i * 16 + 13] = l2w.m13;
				matrices[i * 16 + 14] = l2w.m23;
				matrices[i * 16 + 15] = l2w.m33;
				animTimes[i * 4 + 0] = Mathf.Repeat(bug.lastAnimTime, bug.lastAnimLength) * 24.0F + bug.lastAnimOffset;
				animTimes[i * 4 + 1] = Mathf.Repeat(bug.animTime, bug.animLength) * 24.0F + bug.animOffset;
				animTimes[i * 4 + 2] = bug.animBlendFactor;
			}
			groundEnemyLocalToWorlds.SetData(matrices);
			groundEnemyAnimTimes.SetData(animTimes);
			RenderParams renderParams = new RenderParams(groundBugMaterial);
			renderParams.worldBounds = new Bounds(Vector3.zero, new Vector3(50000.0F, 50000.0F, 50000.0F));
			renderParams.matProps = new MaterialPropertyBlock();
			renderParams.matProps.SetBuffer("_LocalToWorld", groundEnemyLocalToWorlds);
			renderParams.matProps.SetBuffer("_AnimTime", groundEnemyAnimTimes);
			renderParams.shadowCastingMode = ShadowCastingMode.On;
			int groundBugVertexCount = 1968;
			Graphics.RenderPrimitives(renderParams, MeshTopology.Triangles, groundBugVertexCount, bugsToDraw);
		}
	}

	public void DrawOutlines(CommandBuffer cmdBuf) {
		int bugsToDraw = Mathf.Min(allGroundBugs.Count, bugCap);
		if (bugsToDraw > 0) {
			cmdBuf.SetGlobalBuffer("_LocalToWorld", groundEnemyLocalToWorlds);
			cmdBuf.SetGlobalBuffer("_AnimTime", groundEnemyAnimTimes);
			Vector3 camPos = PlayerController.instance.lookCam.transform.position;
			Vector3 camForward = PlayerController.instance.lookForward;
			cmdBuf.SetGlobalVector("_CamPos", camPos);
			cmdBuf.SetGlobalVector("_CamForward", camForward);
			int groundBugVertexCount = 1968;
			cmdBuf.DrawProcedural(Matrix4x4.identity, groundBugOutlineMaterial, 0, MeshTopology.Triangles, groundBugVertexCount, bugsToDraw);
		}
	}
	void GroundBugsTarget() {
		if (allGroundBugs.Count == 0) {
			return;
		}
		currentGroundBugTargetIdx %= allGroundBugs.Count;
		for (int i = 0; i < groundBugsToTargetAndAttackPerTick; i++) {
			if (++currentGroundBugTargetIdx == allGroundBugs.Count) {
				currentGroundBugTargetIdx = 0;
			}
			EnemyGround bug = allGroundBugs[currentGroundBugTargetIdx];
			if (bug.attackCooldownTimer <= 0.0F) {
				// Target
				GameObject bestTarget = PlayerController.instance.gameObject;
				float bestDistance = (bestTarget.transform.position - bug.transform.position).sqrMagnitude;
				int bestBuildingIdx = -1;
				for (int j = 0; j < buildingPositions.Count; j++) {
					float newDist = (buildingPositions[j] - bug.transform.position).sqrMagnitude;
					if (newDist < bestDistance) {
						bestBuildingIdx = j;
						bestDistance = newDist;
					}
				}
				if (bestBuildingIdx != -1) {
					bestTarget = allBuildings[bestBuildingIdx].gameObject;
				}
				foreach (TurretRailgunController turret in turrets) {
					float newDist = (turret.transform.position - bug.transform.position).sqrMagnitude;
					if (newDist < bestDistance) {
						bestTarget = turret.gameObject;
						bestDistance = newDist;
					}
				}
				if (bestDistance < 20.0F * 20.0F) {
					bug.target = bestTarget;
				}
				if (bug.target && ((bug.target.transform.position - bug.transform.position).sqrMagnitude > 20.0F * 20.0F || bug.target.transform.position.y > transform.position.y + 5.0F)) {
					bug.target = null;
				}
			}
		}
	}
	void TurretsTarget() {
		if (turrets.Count == 0) {
			return;
		}
		currentTurretTargetIdx %= turrets.Count;
		for (int i = 0; i < Mathf.Min(turretsToTargetPerTick, turrets.Count); i++) {
			if (++currentTurretTargetIdx == turrets.Count) {
				currentTurretTargetIdx = 0;
			}
			TurretRailgunController turret = turrets[currentTurretTargetIdx];
			Vector3 turretPos = turret.transform.position;
			int overlapCount = Physics.OverlapSphereNonAlloc(turretPos, turret.range, overlapTestArray);
			float bestDist = float.PositiveInfinity;
			Collider bestTarget = null;
			for (int j = 0; j < overlapCount; j++) {
				Collider collider = overlapTestArray[j];
				float sqrDist = (collider.transform.position - turretPos).sqrMagnitude;
				if (sqrDist < bestDist && collider.GetComponent<EnemyGround>()) {
					bestTarget = collider;
					bestDist = sqrDist;
				}
			}
			if (bestTarget != null) {
				turret.target = bestTarget;
			}
		}
	}
	void FixedUpdate() {
		GroundBugsTarget();
		TurretsTarget();
		if (needsNextWave && nextWaveCountdownTimer <= 0.0F) {
			foreach (HiveController controller in waves[currentWave].hives) {
				controller.gameObject.SetActive(true);
				hiveCount++;
				controller.WaveInit(waves[currentWave]);
			}
			needsNextWave = false;
		}
		nextWaveCountdownTimer -= Time.fixedDeltaTime;
		gameTime += Time.fixedDeltaTime;
		if (allBuildings.Count <= 0 || PlayerController.instance.IsDead()) {
			gameOver = true;
			PlayerController.instance.actionsDisabled = true;
			uiScreen.ShowLoseOverlay();
		} else if (hiveCount <= 0 && currentWave < waves.Count - 1 && !needsNextWave) {
			if (currentWave == 0) {
				uiScreen.EnqueueAlert(uiScreen.messageShop, 6.0F);
			}
			IncrementWave();
		} else if (bugCount <= 0 && hiveCount <= 0 && currentWave >= waves.Count - 1 && !needsNextWave) {
			gameOver = true;
			PlayerController.instance.actionsDisabled = true;
			uiScreen.ShowWinOverlay();
		}
	}
}
