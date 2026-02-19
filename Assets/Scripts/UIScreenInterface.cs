using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScreenInterface : MonoBehaviour {
	public GameObject pauseOverlay;
	public GameObject winScreen;
	public GameObject loseScreen;
	public GameObject messageWindowDemo1;
	public GameObject messageWindowTwoDemo1;
	public AudioSource alert;

	void Start() {
		pauseOverlay.SetActive(false);
		winScreen.SetActive(false);
		loseScreen.SetActive(false);
		messageWindowDemo1.SetActive(false);
		messageWindowTwoDemo1.SetActive(false);
		//temporary pop up messages for the DEMO1
		Invoke("InstructionsPopup", 3);
	}

	public void InstructionsPopup() {
		alert.Play();
		messageWindowDemo1.SetActive(true);
		Invoke("InstructionsPopupHide", 10);
	}

	public void InstructionsPopupHide() {
		messageWindowDemo1.SetActive(false);
		Invoke("PressQ", 3);
	}

	public void PressQ() {
		alert.Play();
		messageWindowTwoDemo1.SetActive(true);
		Invoke("PressQHide", 7);
	}

	public void PressQHide() {
		messageWindowTwoDemo1.SetActive(false);
	}

	public void PauseToggle() {
		//escape not working to pause game
		Time.timeScale = pauseOverlay.activeSelf ? 1.0F : 0.0F;
		PlayerController.instance.SetMouseCapture(pauseOverlay.activeSelf);
		pauseOverlay.SetActive(!pauseOverlay.activeSelf);
	}

	public void ShowWinOverlay() {
		PlayerController.instance.SetMouseCapture(false);
		winScreen.SetActive(true);
		// Just in case
		loseScreen.SetActive(false);
		pauseOverlay.SetActive(false);
	}

	public void ShowLoseOverlay() {
		PlayerController.instance.SetMouseCapture(false);
		loseScreen.SetActive(true);
		// Just in case
		winScreen.SetActive(false);
		pauseOverlay.SetActive(false);
	}

	public void OnClickQuit() {
		Application.Quit();
		print("game is quit");
	}

	public void ReloadLevelCurrent() {
		// get reference to current level
		// load that reference
		Time.timeScale = 1.0F;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name); // change to correct scene level
	}

	public void ExitLevel() {
		// returns to main menu
		Time.timeScale = 1.0F;
		SceneManager.LoadScene("Scenes/MainMenu");
	}

}
