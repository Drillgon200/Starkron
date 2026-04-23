using Unity.VisualScripting;
using UnityEngine;

public class EnemyTank : MonoBehaviour, IDamageable, IEnemy {
	public Rigidbody rigidBody;
	public Animator animator;
	public float maxHealth = 20.0F;
	public float turnSpeed = 400.0F;
	public float walkSpeed = 2.0F;
	public float walkDrag = 0.99F;
	public float attackCooldown = 1.0F;
	public float attackCooldownTimer;
	public float damageAmount = 20.0F;
	public float buildingDamageAmount = 5.0F;
	float health;
	public int attackTriggerFrames;
	public GameObject target;

	public GameObject deathParts;
	public GameObject bloodVFX;

	public AudioSource hurtSFX;
	public AudioSource walkSFX;

	bool isAttacking;

	void Start() {
		if (GameManager.instance.bugCount >= GameManager.instance.bugCap) {
			// Too many bugs
			Destroy(gameObject);
			return;
		}
		health = maxHealth;
		GameManager.instance.bugCount++;

		walkSFX.Play();
	}
	void OnDestroy() {
		GameManager.instance.bugCount--;
	}

	public void TriggerAttack() {
		attackTriggerFrames = 2;
	}

	public void FinishAttack() {
		isAttacking = false;
	}

	void FixedUpdate() {
		float dt = Time.fixedDeltaTime;
		if (!isAttacking) {
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
			target = GameManager.instance.FindBugTarget(transform.position, target);
		}
		if (target != null) {
			bool closeEnoughTarget = (target.transform.position - (transform.position + transform.forward * 3.5F)).sqrMagnitude < 3.0F * 3.0F;
			if (attackTriggerFrames > 0 && closeEnoughTarget) {
				foreach (Collider collider in Physics.OverlapBox(transform.position + new Vector3(0.0F, 1.753F, 0.0F) + transform.forward * 3.5F, new Vector3(3.0F, 2.0F, 4.0F), transform.rotation)) {
					PlayerCollisionController player = collider.GetComponent<PlayerCollisionController>();
					IBugTarget bugTarget = collider.GetComponent<IBugTarget>();
					if (bugTarget != null || player != null) {
						bugTarget?.TakeDamage(buildingDamageAmount, collider.transform.position, IDamageable.DamageSource.BUG);
						if (player) {
							print(collider);
							PlayerController.instance.TakeDamage(damageAmount);
						}
						attackTriggerFrames = 0;
					}
				}
			}
			if (attackCooldownTimer <= 0.0F && closeEnoughTarget) {
				animator.SetTrigger("Attack");
				isAttacking = true;
				attackCooldownTimer = attackCooldown;
			}
		}
		rigidBody.linearVelocity *= Mathf.Exp(dt * Mathf.Log(1.0F - walkDrag));
		attackCooldownTimer -= dt;
		attackTriggerFrames = Mathf.Max(attackTriggerFrames - 1, 0);
	}

	public void TakeDamage(float amount, Vector3 pos, IDamageable.DamageSource source) {
		health -= amount;
		//hurtSFX.Play();
		if (health <= 0.0F) {
			if (source == IDamageable.DamageSource.PLAYER) {
				GameManager.instance.statBugsKilledByPlayer++;
			} else if (source == IDamageable.DamageSource.TURRET) {
				GameManager.instance.statBugsKilledByTurrets++;
			}
			GameObject fragments = Instantiate(deathParts, transform.position - transform.forward * 1.0F, transform.rotation);
			GameObject vfx = Instantiate(bloodVFX, transform.position + Vector3.up * 0.5F, Quaternion.identity);
			Destroy(gameObject);
		}
	}
}
