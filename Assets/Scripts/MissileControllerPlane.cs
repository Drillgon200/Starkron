using UnityEngine;

public class MissileControllerPlane : MonoBehaviour {
	public float speed = 2.0F;
	public float maxAge = 10.0F;
	float age;
	public float damageAmount = 10.0F;
	public float explosionRadius = 10.0F;
	public Vector3 velocity;
	public GameObject target;
	Vector3 lastTargetPos;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
	}

	// Update is called once per frame
	void Update() {
		transform.position += velocity * Time.deltaTime;
	}

	void explode() {
		foreach (Collider collider in Physics.OverlapBox(transform.position, new Vector3(explosionRadius, explosionRadius, explosionRadius))) {
			IDamageable damageable = collider.GetComponent<IDamageable>();
			if (damageable != null) {
				Vector3 damagePoint = collider.ClosestPoint(transform.position);
				if (Vector3.Distance(damagePoint, transform.position) < explosionRadius) {
					damageable.TakeDamage(damageAmount, damagePoint);
				}
			}
		}
		Destroy(gameObject);
	}

	void FixedUpdate() {
		float dt = Time.deltaTime;
		age += dt;
		if (age > maxAge) {
			explode();
			return;
		}
		if (target == null) {
			// Unguided
			if (Physics.Raycast(new Ray(transform.position, velocity * dt), dt * velocity.magnitude)) {
				explode();
			}
		} else {
			Vector3 targetPos = target.transform.position;
			if (Vector3.Distance(targetPos, transform.position) < explosionRadius * 0.5F) {
				explode();
				return;
			}
			if (lastTargetPos == Vector3.zero) {
				lastTargetPos = targetPos;
			}
			Vector3 targetVelocity = targetPos - lastTargetPos;
			//TODO replace with real math to calculate the right place to aim for
			float timeToReachTarget = Vector3.Distance(transform.position, targetPos) / velocity.magnitude;
			Vector3 predictedTarget = targetPos + targetVelocity * timeToReachTarget;

			Vector3 toTarget = Vector3.Normalize(predictedTarget - transform.position);
			float turnRate = 600.0F;
			transform.rotation = Quaternion.AngleAxis(dt * turnRate, Vector3.Normalize(Vector3.Cross(transform.forward, toTarget))) * transform.rotation;
			velocity += Vector3.Normalize(transform.forward * dt) * speed;
			float missileDrag = 0.9F;
			velocity *= Mathf.Exp(dt * Mathf.Log(1.0F - missileDrag));

			lastTargetPos = targetPos;
		}
	}
}
