using UnityEngine;
using System.Collections;

public class SpeedLinesScript : MonoBehaviour {
	public CharAnimController charController;
	public PlayerController playerController;
	public float delayTime;

	void Update() {
		GetComponent<UnityEngine.UI.Image>().enabled = playerController.IsBoosting() || playerController.IsSprinting() && playerController.GetMoveDirection().y > 0.5F;
	}
}
