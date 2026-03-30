using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScroll : MonoBehaviour {
	public int speed;
	private bool start;
	public static bool staticStop;

	void Start() {
		StartCoroutine(ScrollUp());
		StartCoroutine(EndCredits());
	}

	void Update() {
		if (start) {
			GetComponent<Rigidbody2D>().linearVelocity = transform.up * speed;
		}

		if (staticStop == true) {
			speed = 0;
		}
	}

	private IEnumerator ScrollUp()	{
		yield return new WaitForSeconds(10);
		start = true;
	}

	private IEnumerator EndCredits() {
		yield return new WaitForSeconds(190);
		SceneManager.LoadScene("Scenes/CAGD_Logo");
	}
}
