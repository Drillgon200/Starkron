using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class WormBossController : MonoBehaviour, IDamageable {
	public SplineContainer pathObject;
	Spline path;
	public int middleSegmentCount = 10;
	public float segmentSpacing = 1.0F;
	public float maxHealth = 200.0F;
	public float speed = 10.0F;
	public float scale = 1.0F;
	public GameObject segmentPrefab;
	public Mesh headMesh;
	public Mesh tailMesh;
	WormSegmentController[] wormSegments;

	float health;
	float currentOffset;
	bool complete;

	void Start() {
		path = pathObject.Spline;
		segmentSpacing *= scale;
		if (segmentSpacing * (middleSegmentCount + 1) > path.GetLength()) {
			throw new System.Exception("Path length must be longer");
		}
		wormSegments = new WormSegmentController[middleSegmentCount + 2];
		wormSegments[0] = Instantiate(segmentPrefab, Vector3.zero, Quaternion.identity).GetComponent<WormSegmentController>();
		wormSegments[0].GetComponent<MeshFilter>().mesh = tailMesh;
		for (int i = 1; i < middleSegmentCount + 1; i++) {
			wormSegments[i] = Instantiate(segmentPrefab, Vector3.zero, Quaternion.identity).GetComponent<WormSegmentController>();
		}
		wormSegments[middleSegmentCount + 1] = Instantiate(segmentPrefab, Vector3.zero, Quaternion.identity).GetComponent<WormSegmentController>();
		wormSegments[middleSegmentCount + 1].GetComponent<MeshFilter>().mesh = headMesh;

		foreach (WormSegmentController segment in wormSegments) {
			segment.controller = this;
			segment.transform.localScale = new Vector3(scale, scale, scale);
		}

		health = maxHealth;
	}

	void Update() {
		if (complete) {
			return;
		}
		float dt = Time.deltaTime;
		currentOffset += dt * speed;
		float pathLength = path.GetLength();
		for (int i = 0; i < wormSegments.Length; i++) {
			float segmentOffset = currentOffset + i * segmentSpacing;
			// Calculating t like this is not technically correct, since splines aren't parameterized in distance, but it should look good enough in non antagonistic cases
			float t = Mathf.Clamp01(segmentOffset / pathLength);
			float3 pos, tan, up;
			path.Evaluate(t, out pos, out tan, out up);
			wormSegments[i].transform.position = (Vector3)pos + pathObject.transform.position;
			Quaternion basis = Quaternion.LookRotation(tan, up);
			if (i == wormSegments.Length - 1) {
				// Stupid hack because the test model is oriented wrong
				basis = basis * Quaternion.AngleAxis(-90.0F, Vector3.up);
			}
			wormSegments[i].transform.rotation = basis;
		}
		if ((wormSegments.Length - 1) * segmentSpacing + currentOffset >= pathLength) {
			GameManager.instance.wormKilledCity = true;
			complete = true;
		}
	}
	public void TakeDamage(float amount, Vector3 pos, IDamageable.DamageSource source) {
		health -= amount;
		if (health <= 0.0F) {
			foreach (WormSegmentController segment in wormSegments) {
				Destroy(segment.gameObject);
			}
			Destroy(gameObject);
		}
	}
}
