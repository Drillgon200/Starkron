using UnityEngine;
using System.Collections;

public class speedLinesScript : MonoBehaviour {
	public CharController charController;
	public PlayerController playerController;
	public float delayTime;

	void Update() {
		if (Input.GetKey(KeyCode.LeftShift) && charController.playerDown == false) {
			if (charController.isPlaneMode == false && Input.GetKey(KeyCode.W)) {
				GetComponent<UnityEngine.UI.Image>().enabled = true;
			} else if (charController.isPlaneMode == true) {
				GetComponent<UnityEngine.UI.Image>().enabled = true;
			} else {
				GetComponent<UnityEngine.UI.Image>().enabled = false;
			}
		} else {
			GetComponent<UnityEngine.UI.Image>().enabled = false;
		}
	}
}
