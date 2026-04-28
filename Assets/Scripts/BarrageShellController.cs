using UnityEngine;

public class BarrageShellController : MonoBehaviour {

	public GameObject explosionVFXPrefab;
	public GameObject trail;

	public Vector3 velocity;
	public float damageAmount = 500.0F;
	public float explosionRadius = 10.0F;
	float age;
	bool exploding;

	public void LaunchTowardPoint(Vector3 target, float launchSpeed) {
		// This should be a helper function somewhere, but I don't feel like making a file for that right now
		Vector3 toTarget = target - transform.position;
		Vector3 direction = Vector3.Normalize(toTarget);
		float horizontalDist = new Vector2(toTarget.x, toTarget.z).magnitude;
		float g = 9.81F;
		float launchSpeedSq = launchSpeed * launchSpeed;
		// https://en.wikipedia.org/wiki/Projectile_motion
		float discrim = launchSpeedSq * launchSpeedSq - g * (g * horizontalDist * horizontalDist + 2.0F * toTarget.y * launchSpeedSq);
		if (discrim >= 0.0F) {
			float theta = Mathf.Atan((launchSpeedSq - Mathf.Sqrt(discrim)) / (g * horizontalDist));
			Vector2 horizontalNorm = Vector2.Normalize(new Vector2(toTarget.x, toTarget.z));
			Vector3 dir = new Vector3(horizontalNorm.x * Mathf.Cos(theta), Mathf.Sin(theta), horizontalNorm.y * Mathf.Cos(theta));
			velocity = dir * launchSpeed;
		} else {
			velocity = direction * launchSpeed;
		}
	}

	void Explode() {
		if (exploding) {
			return;
		}
		exploding = true;
		foreach (Collider collider in Physics.OverlapBox(transform.position, new Vector3(explosionRadius, explosionRadius, explosionRadius))) {
			IDamageable damageable = collider.GetComponent<IDamageable>();
			if (damageable != null) {
				Vector3 damagePoint = collider.ClosestPoint(transform.position);
				if (Vector3.Distance(damagePoint, transform.position) < explosionRadius) {
					damageable.TakeDamage(damageAmount, damagePoint, IDamageable.DamageSource.PLAYER);
				}
			}
			collider.attachedRigidbody?.AddExplosionForce(100.0F, transform.position, explosionRadius);
		}
		GameObject vfx = Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
		Destroy(vfx, 3.0F);
		trail.transform.SetParent(null);
		Destroy(gameObject);
	}

	void Update() {
		float dt = Time.deltaTime;
		Vector3 step = (velocity + Physics.gravity * 0.5F * dt) * dt;
		if (Physics.Raycast(transform.position, step, step.magnitude)) {
			Explode();
		} else if (age > 10.0F) {
			Explode();
		}
		age += dt;
		transform.position += step;
		velocity += Physics.gravity * dt;
	}
}
