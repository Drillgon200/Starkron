using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScreenInterface : MonoBehaviour {
	public GameObject pauseOverlay;
	public GameObject winScreen;
	public GameObject loseScreen;

	void Start() {
		pauseOverlay.SetActive(false);
		winScreen.SetActive(false);
		loseScreen.SetActive(false);
	}

	public void PauseToggle() {
		// change "P' back to ESCAPE on shipping
		if (Input.GetKeyDown(KeyCode.P) && !GameManager.instance.gameOver) {
			Time.timeScale = pauseOverlay.activeSelf ? 1.0F : 0.0F;
			PlayerController.instance.SetMouseCapture(pauseOverlay.activeSelf);
			pauseOverlay.SetActive(!pauseOverlay.activeSelf);
		}
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
		SceneManager.LoadScene("Scenes/Demo1"); // change to correct scene level
	}

	public void ExitLevel() {
		// returns to main menu
		SceneManager.LoadScene("Scenes/MainMenu");
	}

}
