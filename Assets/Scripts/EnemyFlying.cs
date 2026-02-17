using UnityEngine;

public class EnemyFlying : MonoBehaviour, IFlyingEnemy, IDamageable {
	public float health = 10.0F;
	public float speed = 50.0F;
	public float diveSpeed = 100.0F;
	public float drag = 0.9F;
	public float explosionRadius = 5.0F;
	public float explosionDamage = 100.0F;
	public Vector3 originalPosition;
	public Vector3 targetPosition;
	Vector3 velocity;
	Vector3 diveBombDirection;
	bool isDiveBombing;
	bool foundDiveBombTarget;
	float diveBombTime;
	float diveBombTargetFindTime;
	bool exploding;
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		targetPosition = originalPosition = transform.position;
	}

	// Update is called once per frame
	void Update() {
		float dt = Time.deltaTime;
		transform.position += velocity * dt;
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
					damageable.TakeDamage(explosionDamage, damagePoint);
				}
			}
		}
		if (Vector3.Distance(PlayerController.instance.transform.position, transform.position) < explosionRadius) {
			PlayerController.instance.TakeDamage(explosionDamage);
		}
		Destroy(gameObject);
	}
	void OnTriggerEnter(Collider collider) {
		if (velocity.magnitude > 5.0F || isDiveBombing && Vector3.Distance(targetPosition, transform.position) < explosionRadius) {
			Explode();
		}
	}
	void FixedUpdate() {
		float dt = Time.fixedDeltaTime;
		if (isDiveBombing) {
			diveBombTime += dt;
			velocity += (foundDiveBombTarget ? diveBombDirection : (targetPosition - transform.position).normalized) * diveSpeed * dt;
			if (Vector3.Distance(targetPosition, transform.position) < 10.0F) {
				foundDiveBombTarget = true;
				diveBombTargetFindTime = diveBombTime;
			}
			if (foundDiveBombTarget && diveBombTime - diveBombTargetFindTime > 2.0F || diveBombTime > 10.0F) {
				isDiveBombing = false;
			}
		} else {
			if (Random.Range(0, 100) == 0) {
				Vector3 playerPos = PlayerController.instance.transform.position;
				if (transform.position.y > playerPos.y + 10.0F) {
					targetPosition = playerPos;
					diveBombDirection = (targetPosition - transform.position).normalized;
					isDiveBombing = true;
					diveBombTime = 0.0F;
				}
			}
			if (Vector3.Distance(targetPosition, transform.position) < 10.0F) {
				targetPosition = originalPosition + new Vector3(Random.Range(-100.0F, -100.0F), Random.Range(-20.0F, 20.0F), Random.Range(-100.0F, 100.0F));
			}
			velocity += (targetPosition - transform.position).normalized * speed * dt;
		}
		velocity *= Mathf.Exp(dt * Mathf.Log(1.0F - drag));
	}

	public void TakeDamage(float amount, Vector3 pos) {
		health -= amount;
		if (health <= 0.0F) {
			Explode();
		}
	}
}
