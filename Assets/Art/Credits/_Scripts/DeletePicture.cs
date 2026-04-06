using UnityEngine;

public class DeletePicture : MonoBehaviour {
	public int time;

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("PictureTag")) {
			other.gameObject.GetComponent<UnityEngine.UI.Image>().enabled = false;
		}

		if (other.gameObject.CompareTag("StopCreditsTag")) {
			CreditScroll.staticStop = true;
		}
	}

	void OnTriggerExit(Collider other) {
		other.gameObject.GetComponent<UnityEngine.UI.Image>().enabled = true;
	}
}
