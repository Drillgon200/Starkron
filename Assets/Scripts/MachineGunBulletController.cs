using UnityEngine;

public class MachineGunBulletController : MonoBehaviour {
	public float lifetime = 0.1F;
	public float width = 1.0F;
	float age;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {}

	// Update is called once per frame
	void Update() {
		if (age > lifetime) {
			Destroy(gameObject);
		}
		age += Time.deltaTime;
		GetComponent<LineRenderer>().widthMultiplier = (1.0F - age / lifetime) * width;
	}
}
