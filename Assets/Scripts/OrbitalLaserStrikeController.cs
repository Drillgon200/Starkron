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

	void Start() {
		currentPos = targetPos;
	}

	void FixedUpdate() {
		int groundEnemyLayer = 1 << 7;
		Collider[] potentialTargets = Physics.OverlapSphere(currentPos, trackingRadius, groundEnemyLayer);
		Collider bestPotentialTarget = null;
		foreach (Collider c in potentialTargets) {
			float distSq = (c.transform.position - currentPos).sqrMagnitude;
			if (distSq < damageRadius * damageRadius) {
				c.GetComponent<IDamageable>().TakeDamage(1.0F, c.bounds.center, IDamageable.DamageSource.TURRET);
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
	}

	void Update() {
		float dt = Time.deltaTime;

		currentPos = Vector3.MoveTowards(currentPos, targetPos, dt * laserTrackingSpeed);
		GetComponent<LineRenderer>().SetPosition(1, currentPos);

		if (age > maxAge) {
			Destroy(gameObject);
		}
		age += dt;
	}
}
