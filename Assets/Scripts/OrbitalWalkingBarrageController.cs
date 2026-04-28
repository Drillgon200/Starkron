using UnityEngine;

public class OrbitalWalkingBarrageController : MonoBehaviour {
	public Vector3 gunOrigin;
	public Vector3 targetOrigin;
	public Vector3 direction;
	public GameObject barrageShellPrefab;
	public float shotSpacing = 8.0F;
	public float inaccuracy = 4.0F;
	public int maxShots = 10;
	float shotDelay = 1.0F;
	float shotCountdownTimer = 1.0F;
	int shotCount;
	void FixedUpdate() {
		float dt = Time.fixedDeltaTime;
		shotCountdownTimer -= dt;
		if (shotCountdownTimer <= 0.0F) {
			Vector3 nextTarget = targetOrigin + direction * shotCount * shotSpacing + new Vector3(Random.Range(-inaccuracy, inaccuracy), 0.0F, Random.Range(-inaccuracy, inaccuracy));
			BarrageShellController shell = Instantiate(barrageShellPrefab, gunOrigin, Quaternion.identity).GetComponent<BarrageShellController>();
			shell.LaunchTowardPoint(nextTarget, 600.0F);
			shotCountdownTimer = shotDelay;
			shotCount++;
		}
		if (shotCount >= maxShots) {
			Destroy(gameObject);
		}
	}
}