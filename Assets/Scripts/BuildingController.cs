using UnityEngine;

public class BuildingController : MonoBehaviour, IDamageable {
	void Start() {
		GameManager.instance.buildingCount++;
	}
	void OnDestroy() {
		GameManager.instance.buildingCount--;
	}

	// Update is called once per frame
	void Update() {
	}
	public void TakeDamage(float amount, Vector3 pos) {
		Destroy(gameObject);
	}
}
