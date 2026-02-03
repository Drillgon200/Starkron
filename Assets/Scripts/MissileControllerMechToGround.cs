using Unity.VisualScripting;
using UnityEngine;

public class MissileControllerMechToGround : MonoBehaviour {
	public Vector3 velocity;
	public Vector3 target;
	float age;
	public float lifetime = 10.0F;
	public float damageAmount;
	public float blastRadius;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {}

	// Update is called once per frame
	void Update() {
		float dt = Time.deltaTime;
		age += dt;
		if (age > lifetime || Physics.Raycast(new Ray(transform.position, velocity * dt), dt * velocity.magnitude)) {
			foreach (Collider toDamage in Physics.OverlapSphere(transform.position, blastRadius)) {
				IDamageable damageable = toDamage.GetComponent<IDamageable>();
				if (damageable != null) {
					damageable.TakeDamage(damageAmount, toDamage.ClosestPoint(transform.position));
				}
			}
			Destroy(gameObject);
		}
		transform.position += velocity * dt;
	}
	void FixedUpdate() {
		float dt = Time.deltaTime;
		Vector3 toTarget = Vector3.Normalize(target - transform.position);
		float turnRate = 100.0F + 600.0F * Mathf.Clamp01(age);
		transform.rotation = Quaternion.AngleAxis(dt * turnRate, Vector3.Normalize(Vector3.Cross(transform.forward, toTarget))) * transform.rotation;
		velocity += Vector3.Normalize(transform.forward * dt);
		float missileDrag = 0.9F;
		velocity *= Mathf.Exp(dt * Mathf.Log(1.0F - missileDrag));
	}
}
