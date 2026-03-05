using UnityEditor.Recorder;
using UnityEngine;

public class HiveController : MonoBehaviour, IDamageable {
	public GameObject groundBugPrefab;
	public float spawnRange = 5.0F;
	public AudioClip hiveDamageB;
	float health;
	float spawnDelay;
	int bugsSpawnedInCurrentGroup;
	GameManager.Wave wave;
	// Update is called once per frame
	void FixedUpdate() {
		if (GameManager.instance.bugCount < GameManager.instance.bugCap) {
			if (spawnDelay < 0.0F) {
				spawnDelay = 0.0F;
				if (++bugsSpawnedInCurrentGroup >= wave.groupMemberCount) {
					spawnDelay += wave.groupDelay;
					bugsSpawnedInCurrentGroup = 0;
				}

				Vector3 spawnPos = new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange), transform.position.y + 0.25F, transform.position.z + Random.Range(-spawnRange, spawnRange));
				RaycastHit hit;
				if (Physics.Raycast(spawnPos + Vector3.up * 10.0F, Vector3.down, out hit, 20.0F)) {
					spawnPos = hit.point + Vector3.up * 0.25F;
					GameObject groundBug = Instantiate(groundBugPrefab, spawnPos, Quaternion.identity);
					spawnDelay += 1.0F / (GameManager.instance.spawnRateMultiplier * Random.Range(wave.spawnRateMin, wave.spawnRateMax));
				}
			}
		}
		spawnDelay -= Time.fixedDeltaTime;
	}
	public void WaveInit(GameManager.Wave wave) {
		this.wave = wave;
		spawnDelay = Random.Range(2.0F, 5.0F);
		bugsSpawnedInCurrentGroup = 0;
		health = wave.hiveHealth;
	}
	public void TakeDamage(float amount, Vector3 pos, IDamageable.DamageSource source) {
		health -= amount;
		if (health <= 0.0F) {
			if (source == IDamageable.DamageSource.PLAYER) {
				GameManager.instance.statHivesKilledByPlayer++;
			} else if (source == IDamageable.DamageSource.BUG) {
				GameManager.instance.statHivesKilledByTurrets++;
			}
			GameManager.instance.hiveCount--;
			SoundFXManager.instance.PlaySoundFXClip(hiveDamageB, transform, 0.8F);
			gameObject.SetActive(false);
		}
	}
}
