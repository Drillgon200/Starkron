using UnityEngine;

public class TurretRailgunController : MonoBehaviour, IBugTarget {
	public GameObject barrel;
	public Collider target;
	public GameObject railgunProjectilePrefab;
	public float range = 50.0F;
	public float fireRate;
	public float damage = 10.0F;
	public float maxHealth;
	public float rotationRateDeg = 200.0F;
	float health;
	float fireCooldown;
	public int gameManagerRegisteredIdx;
	void Start() {
		health = maxHealth;
		gameManagerRegisteredIdx = GameManager.instance.RegisterTurret(this);
	}
	void OnDestroy() {
		GameManager.instance.RemoveTurret(gameManagerRegisteredIdx);
	}

	void FixedUpdate() {
		float dt = Time.fixedDeltaTime;
		if (target) {
			Vector3 targetDirection = Vector3.Normalize(target.bounds.center - barrel.transform.position);
			Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
			barrel.transform.rotation = Quaternion.RotateTowards(barrel.transform.rotation, targetRotation, dt * rotationRateDeg);
			Vector3 fireDirection = barrel.transform.forward;
			if (fireCooldown <= 0 && Vector3.Dot(fireDirection, targetDirection) > 0.99F) {
				Vector3 fireFrom = barrel.transform.localToWorldMatrix * new Vector4(0.0F, 25.0F, 75.0F, 1.0F);
				RaycastHit bulletHit;
				bool bulletRayHit = Physics.Raycast(new Ray(fireFrom, fireDirection), out bulletHit);
				Vector3 fireTo = bulletRayHit ? bulletHit.point : fireFrom + fireDirection * 1000.0F;
				GameObject bulletVFX = Instantiate(railgunProjectilePrefab, new Vector3(0.0F, 0.0F, 0.0F), Quaternion.identity);
				LineRenderer bulletRender = bulletVFX.GetComponent<LineRenderer>();
				bulletRender.SetPosition(0, fireFrom);
				bulletRender.SetPosition(1, fireTo);
				if (bulletRayHit) {
					bulletHit.transform.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage, fireTo);
				}
				fireCooldown = 1.0F / fireRate;
			}
		}
		fireCooldown -= dt;
	}
	public void TakeDamage(float amount, Vector3 pos) {
		health -= amount;
		if (health <= 0.0F) {
			Destroy(gameObject);
		}
	}
}
