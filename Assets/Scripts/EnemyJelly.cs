using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyJelly : MonoBehaviour, IDamageable, IFlyingEnemy {
	public Rigidbody rigidBody;
	public GameObject acidSpitPrefab;
	public float maxHealth = 20.0F;
	float health;

	public float pulseCooldown = 2.0F;
	float pulseCooldownTimer;
	float pulseTimer = 0.0F;

	public float attackCooldown = 5.0F;
	float attackTimer;

	GameObject target;
	float idealHeightAboveTarget;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		health = maxHealth;
		idealHeightAboveTarget = Random.Range(20.0F, 40.0F);
		target = PlayerController.instance.gameObject;
	}

	void FixedUpdate() {
		float dt = Time.fixedDeltaTime;
		float targetHeight = idealHeightAboveTarget;
		RaycastHit hit;
		if (Physics.Raycast(transform.position + Vector3.down * 2.0F, Vector3.down, out hit)) {
			targetHeight = Mathf.Max(targetHeight, hit.point.y + idealHeightAboveTarget);
		}
		Vector2 xzPos = new Vector3(transform.position.x, transform.position.z);
		GameObject buildingToAttack = GameManager.instance.RandomBuilding();
		GameObject turretToAttack = GameManager.instance.RandomTurret();
		float distCurrentTarget = target == null ? float.PositiveInfinity : Vector2.Distance(xzPos, new Vector2(target.transform.position.x, target.transform.position.z)) - 40.0F;
		float distPlayer = Vector2.Distance(xzPos, new Vector2(PlayerController.instance.transform.position.x, PlayerController.instance.transform.position.z)) - 30.0F;
		float distBuilding = buildingToAttack == null ? float.PositiveInfinity : Vector2.Distance(xzPos, new Vector2(buildingToAttack.transform.position.x, buildingToAttack.transform.position.z));
		float distTurret = turretToAttack == null ? float.PositiveInfinity : Vector2.Distance(xzPos, new Vector2(turretToAttack.transform.position.x, turretToAttack.transform.position.z)) - 20.0F;
		float bestDist = distCurrentTarget;
		if (distPlayer < bestDist) {
			target = PlayerController.instance.gameObject;
			bestDist = distPlayer;
		}
		if (distBuilding < bestDist) {
			target = buildingToAttack;
			bestDist = distBuilding;
		}
		if (distTurret < bestDist) {
			target = turretToAttack;
		}
		if (target) {
			targetHeight = Mathf.Max(targetHeight, target.transform.position.y + idealHeightAboveTarget);
			Vector2 targetXZPos = new Vector3(target.transform.position.x, target.transform.position.z);
			float dist = Vector2.Distance(xzPos, targetXZPos);
			if (dist >= 20.0F) {
				Vector2 toTarget = Vector2.Normalize(targetXZPos - xzPos);
				rigidBody.AddForce(new Vector3(toTarget.x, 0.0F, toTarget.y) * 15.0F * Mathf.Min(1.0F, (dist - 20.0F) * 0.1F), ForceMode.Acceleration);
			}
			if (attackCooldown <= 0.0F) {
				attackTimer = 2.0F;
				attackCooldown = 4.0F;
			}
			if (attackTimer > 0.0F && dist < 30.0F) {
				Vector3 toPosition = target.transform.position - transform.position;
				Vector3 direction = Vector3.Normalize(toPosition);
				float horizontalDist = new Vector2(toPosition.x, toPosition.z).magnitude;
				float g = 9.81F;
				float launchSpeed = 20.0F;
				float launchSpeedSq = launchSpeed * launchSpeed;
				// https://en.wikipedia.org/wiki/Projectile_motion
				// Atan and sin/cos could be optimized out using trig identities and all, I'm too tired for it right now
				float discrim = launchSpeedSq * launchSpeedSq - g * (g * horizontalDist * horizontalDist + 2.0F * toPosition.y * launchSpeedSq);
				if (discrim >= 0.0F) {
					float theta = Mathf.Atan((launchSpeedSq - Mathf.Sqrt(discrim)) / (g * horizontalDist));
					Vector2 horizontalNorm = Vector2.Normalize(new Vector2(toPosition.x, toPosition.z));
					Vector3 dir = new Vector3(horizontalNorm.x * Mathf.Cos(theta), Mathf.Sin(theta), horizontalNorm.y * Mathf.Cos(theta));
					GameObject acid = Instantiate(acidSpitPrefab, transform.position + direction * 2.0F, Random.rotation);
					acid.GetComponent<Rigidbody>().AddForce(dir * 20.0F + Random.onUnitSphere, ForceMode.VelocityChange);
				}
			}
		}
		if (pulseCooldownTimer <= 0.0F && transform.position.y < targetHeight - 2.0F) {
			pulseCooldownTimer = pulseCooldown;
			pulseTimer = 1.0F;
		}
		if (pulseTimer >= 0) {
			rigidBody.AddForce(Vector3.up * 50.0F * Mathf.Min(1.0F, pulseTimer), ForceMode.Acceleration);
		}
		pulseCooldownTimer -= dt;
		pulseTimer -= dt;
		attackTimer -= dt;
		attackCooldown -= dt;

		float yDrag = 0.99F;
		float xzDrag = 0.95F;
		float yDecay = Mathf.Exp(dt * Mathf.Log(1.0F - yDrag));
		float xzDecay = Mathf.Exp(dt * Mathf.Log(1.0F - xzDrag));
		rigidBody.AddForce(Vector3.Scale(rigidBody.linearVelocity, new Vector3(xzDecay, yDecay, xzDecay)) - rigidBody.linearVelocity, ForceMode.VelocityChange);
	}

	public void TakeDamage(float amount, Vector3 pos, IDamageable.DamageSource source) {
		health -= amount;
		if (health <= 0.0F) {
			Destroy(gameObject);
		}
	}
}
