using UnityEngine;

public class OrbitalAbilityThrowableController : MonoBehaviour {

	public GameObject orbitalLaserStrikePrefab;

	public Vector3 abilityOriginPoint;

	public Vector3 velocity;
	float age;

	void Start() {
		
	}

	public void LaunchTowardPoint(Vector3 target, float launchSpeed) {
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

	void Update() {
		float dt = Time.deltaTime;
		Vector3 step = (velocity + Physics.gravity * 0.5F * dt) * dt;
		if (Physics.Raycast(transform.position, step, step.magnitude)) {
			GameObject laser = Instantiate(orbitalLaserStrikePrefab, Vector3.zero, Quaternion.identity);
			laser.GetComponent<OrbitalLaserStrikeController>().targetPos = transform.position;
			LineRenderer line = laser.GetComponent<LineRenderer>();
			line.SetPosition(0, abilityOriginPoint);
			Destroy(gameObject);
		} else if (age > 10.0F) {
			Destroy(gameObject);
		}
		age += dt;
		transform.position += step;
		velocity += Physics.gravity * dt;
	}
}
