using UnityEngine;

public class DummyDamageable : MonoBehaviour, IDamageable {
	public float health = 50.0F;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {}

	// Update is called once per frame
	void Update() {}

	public void TakeDamage(float amount, Vector3 pos) {
		health -= amount;
		if (health <= 0.0F) {
			Destroy(gameObject);
		}
	}
}
