using UnityEngine;

public class EnemyFlying : MonoBehaviour, IFlyingEnemy, IDamageable {
	public float health = 10.0F;
	public float speed = 50.0F;
	public float drag = 0.9F;
	public Vector3 originalPosition;
	public Vector3 targetPosition;
	Vector3 velocity;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		targetPosition = originalPosition = transform.position;
	}

	// Update is called once per frame
	void Update() {
		float dt = Time.deltaTime;
		transform.position += velocity * dt;
	}

	void FixedUpdate() {
		float dt = Time.deltaTime;
		if (Vector3.Distance(targetPosition, transform.position) < 10.0F) {
			targetPosition = originalPosition + new Vector3(Random.Range(-100.0F, -100.0F), Random.Range(-20.0F, 20.0F), Random.Range(-100.0F, 100.0F));
		}
		velocity += (targetPosition - transform.position).normalized * speed * dt;
		velocity *= Mathf.Exp(dt * Mathf.Log(1.0F - drag));
	}

	public void TakeDamage(float amount, Vector3 pos) {
		health -= amount;
		if (health <= 0.0F) {
			Destroy(gameObject);
		}
	}
}
