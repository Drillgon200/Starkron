using UnityEngine;

public class TurretAAController : MonoBehaviour, IBugTarget {
	public GameObject barrel;
	public Collider target;
	public GameObject missilePlanePrefab;
	public float range = 500.0F;
	public float fireRate;
	public float damage = 10.0F;
	public float maxHealth;
	public float rotationRateDeg = 20.0F;
	float health;
	public float iFrames;
	float iFrameCooldown;
	float fireCooldown;
	public int gameManagerRegisteredIdx;

	void Start() {
		health = maxHealth;
		gameManagerRegisteredIdx = GameManager.instance.RegisterAATurret(this);
	}
	void OnDestroy() {
		GameManager.instance.RemoveAATurret(gameManagerRegisteredIdx);
	}

	void FixedUpdate() {
		float dt = Time.fixedDeltaTime;
		int flyingEnemyLayer = 8;
		foreach (Collider c in Physics.OverlapSphere(transform.position, range, 1 << flyingEnemyLayer)){
			if (c.GetComponent<IFlyingEnemy>() != null) {
				if (!target || (c.transform.position - transform.position).sqrMagnitude < (target.transform.position - transform.position).sqrMagnitude) {
					target = c;
				}
			}
		}
		if (target) {
			Vector3 targetDirection = Vector3.Normalize(target.bounds.center - barrel.transform.position);
			Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
			barrel.transform.rotation = Quaternion.RotateTowards(barrel.transform.rotation, targetRotation, dt * rotationRateDeg);
			Vector3 fireDirection = barrel.transform.forward;
			if (fireCooldown <= 0 && Vector3.Dot(fireDirection, targetDirection) > 0.99F) {
				Vector3 fireFrom = barrel.transform.localToWorldMatrix * new Vector4(0.0F, 25.0F, 75.0F, 1.0F);
				GameObject bulletVFX = Instantiate(missilePlanePrefab, fireFrom, transform.rotation);
				MissileControllerPlane missileController = bulletVFX.GetComponent<MissileControllerPlane>();
				missileController.velocity = fireDirection * 100.0F;
				missileController.speed = 2.0F;
				missileController.target = target.gameObject;
				missileController.damageAmount = damage;

				fireCooldown = 1.0F / fireRate;
			}
		}
		fireCooldown -= dt;
		iFrameCooldown -= dt;
	}
	public void TakeDamage(float amount, Vector3 pos, IDamageable.DamageSource source) {
		if (iFrameCooldown > 0.0F || source == IDamageable.DamageSource.TURRET) {
			return;
		}
		iFrameCooldown = iFrames;
		if (amount > 100.0F) {
			health = 0.0F;
		} else {
			health -= 1.0F;
		}
		if (health <= 0.0F) {
			Destroy(gameObject);
		}
	}
}
