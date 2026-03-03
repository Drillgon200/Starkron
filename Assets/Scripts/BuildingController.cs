using UnityEngine;

public class BuildingController : MonoBehaviour, IBugTarget {
	public int gameManagerRegisteredIdx;
	void Start() {
		gameManagerRegisteredIdx = GameManager.instance.RegisterBuilding(this);
	}
	void OnDestroy() {
		GameManager.instance.RemoveBuilding(gameManagerRegisteredIdx);
	}
	public void TakeDamage(float amount, Vector3 pos, IDamageable.DamageSource source) {
		if (source == IDamageable.DamageSource.PLAYER) {
			GameManager.instance.statBuildingsDestroyedByPlayer++;
		}
		Destroy(gameObject);
	}
}
