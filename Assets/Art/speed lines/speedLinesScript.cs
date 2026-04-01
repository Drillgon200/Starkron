using UnityEngine;
using System.Collections;

public class SpeedLinesScript : MonoBehaviour {
	public CharAnimController charController;
	public PlayerController playerController;
	public float delayTime;

	void Update() {
		if (Input.GetKey(KeyCode.LeftShift) && !charController.playerDown) {
			if (!charController.isPlaneMode && Input.GetKey(KeyCode.W)) {
				GetComponent<UnityEngine.UI.Image>().enabled = true;
			} else if (charController.isPlaneMode) {
				GetComponent<UnityEngine.UI.Image>().enabled = true;
			} else {
				GetComponent<UnityEngine.UI.Image>().enabled = false;
			}
		} else {
			GetComponent<UnityEngine.UI.Image>().enabled = false;
		}
	}
}
