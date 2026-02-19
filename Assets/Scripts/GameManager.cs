using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager instance;

	public UIScreenInterface uiScreen;

	public int buildingCount;
	public int bugCount;
	public int hiveCount;
	public double gameTime;
	public bool gameOver;

	public int bugCap;

	const float GRID_SIZE = 300.0F;
	const int GRID_RESOLUTION = 300;

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

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
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
		Vector2 cityPos = new Vector2(-0.1755F, -0.58833F);
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
	// Update is called once per frame
	void Update() {
	}
	void FixedUpdate() {
		gameTime += Time.fixedDeltaTime;
		if (buildingCount <= 0 || PlayerController.instance.IsDead()) {
			print("Game over, you lose");
			gameOver = true;
			uiScreen.ShowLoseOverlay();
		} else if (bugCount <= 0 && hiveCount <= 0) {
			gameOver = true;
			uiScreen.ShowWinOverlay();
		}
	}
}
