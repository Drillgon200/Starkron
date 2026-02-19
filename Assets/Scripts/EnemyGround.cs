using UnityEngine;

public class EnemyGround : MonoBehaviour, IDamageable, IEnemy {
	public Rigidbody rigidBody;
	public Animator animator;
	public float maxHealth = 20.0F;
	public float turnSpeed = 400.0F;
	public float walkSpeed = 2.0F;
	public float walkDrag = 0.99F;
	public float attackCooldown = 1.0F;
	float attackCooldownTimer;
	public float damageAmount = 20.0F;
	float health;
	int attackTriggerFrames;
	GameObject target;
	void Start() {
		health = maxHealth;
		GameManager.instance.bugCount++;
	}
	void OnDestroy() {
		GameManager.instance.bugCount--;
	}

	void OnTriggerStay(Collider collider) {
		PlayerCollisionController player = collider.gameObject.GetComponent<PlayerCollisionController>();
		BuildingController building = collider.gameObject.GetComponent<BuildingController>();
		if (attackTriggerFrames > 0 && (player || building)) {
			building?.TakeDamage(damageAmount, collider.ClosestPoint(transform.position));
			if (player) {
				PlayerController.instance.TakeDamage(damageAmount);
			}
			attackTriggerFrames = 0;
		}
		if (attackCooldownTimer <= 0.0F && (player || building)) {
			animator.SetTrigger("Attack");
			attackCooldownTimer = attackCooldown;
		}
	}

	// Should be called after update loop
	public void OnAttackAnimationEvent() {
		attackTriggerFrames = 2;
	}

	void FixedUpdate() {
		float dt = Time.fixedDeltaTime;
		if (attackCooldownTimer <= 0.0F) {
			{ // Select target
				if (Random.Range(0, 25) == 0) {
					GameObject bestTarget = PlayerController.instance.gameObject;
					float bestDistance = Vector3.Distance(bestTarget.transform.position, transform.position);
					foreach (Collider collider in Physics.OverlapSphere(transform.position, 10.0F)) {
						float newDist = Vector3.Distance(collider.transform.position, transform.position);
						if (collider.GetComponent<BuildingController>() && newDist < bestDistance) {
							bestTarget = collider.gameObject;
							bestDistance = newDist;
						}
					}
					if (bestDistance < 20.0F) {
						target = bestTarget;
					}
				}
				if (target && (Vector3.Distance(target.transform.position, transform.position) > 20.0F || target.transform.position.y > transform.position.y + 5.0F)) {
					target = null;
				}
			}

			Vector2 direction =
				target ? Vector2.Normalize(new Vector2(target.transform.position.x - transform.position.x, target.transform.position.z - transform.position.z)) :
				GameManager.instance.GetDirectionToCity(new Vector2(transform.position.x, transform.position.z));
			// Not sure why unity doesn't have a cross product function for 2D vectors built in
			// cross tells us which way to turn, dot slows the turn rate down as it approaches the target. We don't need to be too precise with this.
			float cross = direction.y * transform.forward.x - direction.x * transform.forward.z;
			float dot = Vector3.Dot(transform.forward, new Vector3(direction.x, 0.0F, direction.y));
			rigidBody.MoveRotation(Quaternion.AngleAxis((1.0F - Mathf.Clamp01(dot)) * -Mathf.Sign(cross) * turnSpeed * dt, new Vector3(0.0F, 1.0F, 0.0F)) * rigidBody.rotation);
			if (Vector3.Dot(rigidBody.linearVelocity, transform.forward) < walkSpeed) {
				rigidBody.AddForce(transform.forward * 100.0F, ForceMode.Acceleration);
			}
		}
		rigidBody.linearVelocity *= Mathf.Exp(dt * Mathf.Log(1.0F - walkDrag));
		attackCooldownTimer -= dt;
		attackTriggerFrames = Mathf.Max(attackTriggerFrames - 1, 0);
	}


	public void TakeDamage(float amount, Vector3 pos) {
		health -= amount;
		if (health <= 0.0F) {
			Destroy(gameObject);
		}
	}
}
