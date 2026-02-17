using UnityEngine;

public class EnemyGround : MonoBehaviour, IDamageable, IEnemy {
	public Rigidbody rigidBody;
	public float maxHealth = 20.0F;
	public float turnSpeed = 400.0F;
	public float walkSpeed = 2.0F;
	public float walkDrag = 0.99F;
	float health;
	void Start() {
		health = maxHealth;
		GameManager.instance.bugCount++;
	}
	void OnDestroy() {
		GameManager.instance.bugCount--;
	}

	void FixedUpdate() {
		Vector2 direction = GameManager.instance.GetDirectionToCity(new Vector2(transform.position.x, transform.position.z));
		// Not sure why unity doesn't have a cross product function for 2D vectors built in
		// cross tells us which way to turn, dot slows the turn rate down as it approaches the target. We don't need to be too precise with this.
		float cross = direction.y * transform.forward.x - direction.x * transform.forward.z;
		float dot = Vector3.Dot(transform.forward, new Vector3(direction.x, 0.0F, direction.y));
		rigidBody.MoveRotation(Quaternion.AngleAxis((1.0F - Mathf.Clamp01(dot)) * -Mathf.Sign(cross) * turnSpeed * Time.deltaTime, new Vector3(0.0F, 1.0F, 0.0F)) * rigidBody.rotation);
		if (Vector3.Dot(rigidBody.linearVelocity, transform.forward) < walkSpeed) {
			rigidBody.AddForce(transform.forward * 100.0F, ForceMode.Acceleration);
		}
		rigidBody.linearVelocity *= Mathf.Exp(Time.fixedDeltaTime * Mathf.Log(1.0F - walkDrag)); ;
	}


	public void TakeDamage(float amount, Vector3 pos) {
		health -= amount;
		if (health <= 0.0F) {
			Destroy(gameObject);
		}
	}
}
