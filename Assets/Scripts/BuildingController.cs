using UnityEngine;

public class BuildingController : MonoBehaviour, IBugTarget {

	public int gameManagerRegisteredIdx;
	private int buildingHealth;

	void Start() {
		gameManagerRegisteredIdx = GameManager.instance.RegisterBuilding(this);
		buildingHealth = 4;
	}
	void OnDestroy() {
		GameManager.instance.RemoveBuilding(gameManagerRegisteredIdx);
	}
	public void TakeDamage(float amount, Vector3 pos, IDamageable.DamageSource source) {
		buildingHealth -= (int)Mathf.Ceil(amount);
		
		if (source == IDamageable.DamageSource.BUG) {
			GameManager.instance.OnBugAttackCity();
		}

		if (buildingHealth <= 0) {
            if (source == IDamageable.DamageSource.PLAYER) {
                GameManager.instance.statBuildingsDestroyedByPlayer++;
            }
            GameManager.instance.statBuildingsDestroyed++;
			Destroy(gameObject);
		} else {
			print("Not destroyed " + amount);
		}
	}
}
