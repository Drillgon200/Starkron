using UnityEngine;

public class RocketAnimScript : MonoBehaviour {
	public PlayerController playerController;

	public GameObject RocketA;
	public GameObject RocketB;
	public GameObject RocketC;
	public GameObject RocketD;

	public float spaceTimer;

	void Start() {
		RocketB.SetActive(false);
		RocketC.SetActive(false);
		RocketD.SetActive(false);
	}

	void Update() {
		if (playerController.rocketSalvoCount == 1) {
			RocketA.SetActive(true);
		} else if (playerController.rocketSalvoCount == 2) {
			RocketB.SetActive(true);
		} else if (playerController.rocketSalvoCount == 3) {
			RocketC.SetActive(true);
		} else if (playerController.rocketSalvoCount == 4) {
			RocketD.SetActive(true);
		}
	}
}
