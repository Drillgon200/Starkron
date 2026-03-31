using UnityEngine;

public class SwordPickUpScript : MonoBehaviour {
	public PlayerController playerController;

    public void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("PlayerTag")) {
            playerController.swordEnabled = true;
            Destroy(this.gameObject);
        }
    }
}
