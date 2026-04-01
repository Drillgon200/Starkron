using UnityEngine;

public class SwordPickUpScript : MonoBehaviour {
	
	public void OnCollisionEnter(Collision other) {
		if (other.gameObject.CompareTag("PlayerTag")) {
			PlayerController.instance.swordEnabled = true;
			Destroy(this.gameObject);
		}
	}
}
