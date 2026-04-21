using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class WormBossController : MonoBehaviour {
	public SplineContainer pathObject;
	Spline path;
	public int middleSegmentCount = 10;
	public float segmentSpacing = 1.0F;
	public float maxHealth = 200.0F;
	public float speed = 10.0F;
	public float scale = 1.0F;
	public GameObject headSegmentPrefab;
	public GameObject middleSegmentPrefab;
	public GameObject tailSegmentPrefab;
	WormSegmentController[] wormSegments;

	int weakpointCount;
	float currentOffset;
	bool complete;

	void Start() {
		path = pathObject.Spline;
		segmentSpacing *= scale;
		if (segmentSpacing * (middleSegmentCount + 1) > path.GetLength()) {
			throw new System.Exception("Path length must be longer");
		}
		wormSegments = new WormSegmentController[middleSegmentCount + 2];
		wormSegments[0] = Instantiate(tailSegmentPrefab, Vector3.zero, Quaternion.identity).GetComponent<WormSegmentController>();
		for (int i = 1; i < middleSegmentCount + 1; i++) {
			wormSegments[i] = Instantiate(middleSegmentPrefab, Vector3.zero, Quaternion.identity).GetComponent<WormSegmentController>();
		}
		wormSegments[middleSegmentCount + 1] = Instantiate(headSegmentPrefab, Vector3.zero, Quaternion.identity).GetComponent<WormSegmentController>();

		foreach (WormSegmentController segment in wormSegments) {
			segment.controller = this;
			segment.transform.localScale = new Vector3(scale * 0.0025F, scale * 0.0025F, scale * 0.0025F);
		}

		weakpointCount = middleSegmentCount * 2 + 1; // Two eyes per middle segment plus mouth
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
			wormSegments[i].transform.rotation = basis;
		}
		if ((wormSegments.Length - 1) * segmentSpacing + currentOffset >= pathLength) {
			GameManager.instance.wormKilledCity = true;
			complete = true;
		}
	}
	public void WeakpointDestroyed() {
		if (--weakpointCount == 0) {
			foreach (WormSegmentController segment in wormSegments) {
				Destroy(segment.gameObject);
			}
			GameManager.instance.wormAlive = false;
			Destroy(gameObject);
		}
	}
}
