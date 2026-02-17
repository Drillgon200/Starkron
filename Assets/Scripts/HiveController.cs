using UnityEngine;

public class HiveController : MonoBehaviour, IDamageable {
	public GameObject groundBugPrefab;
	public float spawnDelayMin = 5.0F;
	public float spawnDelayMax = 20.0F;
	public float spawnRange = 5.0F;
	public float health = 200.0F;
	float spawnDelay;
	void Start() {
		GameManager.instance.hiveCount++;
	}
	void OnDestroy() {
		GameManager.instance.hiveCount--;
	}
	// Update is called once per frame
	void FixedUpdate() {
		if (spawnDelay < 0.0F) {
			Vector3 spawnPos = new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange), transform.position.y + 0.25F, transform.position.z + Random.Range(-spawnRange, spawnRange));
			if (Physics.OverlapBox(spawnPos, new Vector3(1.0F, 0.2F, 1.0F)).Length == 0) {
				GameObject groundBug = Instantiate(groundBugPrefab, spawnPos, Quaternion.identity);
			}
			spawnDelay = Random.Range(spawnDelayMin, spawnDelayMax);
		}
		spawnDelay -= Time.fixedDeltaTime;
	}
	public void TakeDamage(float amount, Vector3 pos) {
		health -= amount;
		if (health <= 0.0F) {
			Destroy(gameObject);
		}
	}
}
