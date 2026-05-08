using UnityEngine;

public class OrbitalLaserStrikeController : MonoBehaviour {
	float age;
	public float maxAge = 10.0F;
	public float laserTrackingSpeed = 5.0F;
	public float trackingRadius = 20.0F;
	public float damageRadius = 10.0F;

	public GameObject target;
	public Vector3 targetPos;
	public Vector3 currentPos;
	public GameObject vfx;
	public LineRenderer lightning;
	public GameObject slag;
	LineRenderer lineRenderer;

	void Start() {
		currentPos = targetPos;
		lineRenderer = GetComponent<LineRenderer>();
	}

	void FixedUpdate() {
		int groundEnemyLayer = 1 << 7;
		Collider[] potentialTargets = Physics.OverlapSphere(currentPos, trackingRadius, groundEnemyLayer);
		Collider bestPotentialTarget = null;
		foreach (Collider c in potentialTargets) {
			float distSq = (c.transform.position - currentPos).sqrMagnitude;
			if (distSq < damageRadius * damageRadius) {
				c.GetComponent<IDamageable>().TakeDamage(10.0F, c.bounds.center, IDamageable.DamageSource.TURRET);
			}
			if (!bestPotentialTarget || distSq < (bestPotentialTarget.transform.position - currentPos).sqrMagnitude) {
				bestPotentialTarget = c;
			}
		}
		if (potentialTargets.Length > 0) {
			target = bestPotentialTarget.gameObject;
		} else if (!target) {
			EnemyGround groundBug = GameManager.instance.GetRandomGroundBug();
			target = groundBug ? groundBug.gameObject : null;
		}
		if (target) {
			targetPos = target.transform.position;
		}
		int numLightningPoints = lightning.positionCount;
		Vector3 startPos = lineRenderer.GetPosition(0);
		float dist = Vector3.Distance(startPos, currentPos);
		float lightningSize = 2.0F;
		Quaternion basis = Quaternion.LookRotation(currentPos - startPos);
		for (int i = 0; i < numLightningPoints; i++) {
			Vector3 localPos = new Vector3(Random.Range(-lightningSize, lightningSize), Random.Range(-lightningSize, lightningSize), (float)i / (numLightningPoints - 1) * dist);
			lightning.SetPosition(i, basis * localPos + startPos);
		}
		slag.transform.rotation = basis * Quaternion.AngleAxis(180.0F, Vector3.right);
	}

	void Update() {
		float dt = Time.deltaTime;

		currentPos = Vector3.MoveTowards(currentPos, targetPos, dt * laserTrackingSpeed);
		vfx.transform.position = currentPos;
		lineRenderer.SetPosition(1, currentPos);
		lineRenderer.widthMultiplier = 0.5F * (Mathf.Sin(age * 100.0F) * 0.5F + 0.5F) + 1.0F;

		if (age > maxAge) {
			Destroy(gameObject);
		}
		age += dt;
	}
}
