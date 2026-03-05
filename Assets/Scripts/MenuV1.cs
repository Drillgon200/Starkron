using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuV1 : MonoBehaviour {

	public GameObject mainMenuScene;
	public GameObject OptionsScene;
	public GameObject levelSelectScene;
	public GameObject exitOverlayGraphic;

	int currentResolutionIdx;

	void Start() {
		mainMenuScene.SetActive(true);
		OptionsScene.SetActive(false);
		levelSelectScene.SetActive(false);
		exitOverlayGraphic.SetActive(false);
	}

	void Update() {
		ExitPrompt();
	}
	public void ExitPrompt() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (exitOverlayGraphic.activeSelf == false && mainMenuScene.activeSelf == false) {
				//overlay prompt to quit game
				exitOverlayGraphic.SetActive(true);
			}
			else if (exitOverlayGraphic.activeSelf == true) {
				exitOverlayGraphic.SetActive(false);
			}
		}
	}

	public void ResolutionToggle() {
		//for (int i = 0; i < Screen.resolutions.Length; i++) {
		//	if (Screen.resolutions[currentResolutionIdx].width != Screen.width || Screen.resolutions[currentResolutionIdx].height != Screen.height) {
		//		break;
		//	}
		//	currentResolutionIdx = (currentResolutionIdx + 1) % Screen.resolutions.Length;
		//}
		//Screen.SetResolution(Screen.resolutions[currentResolutionIdx].width, Screen.resolutions[currentResolutionIdx].height, Screen.fullScreen);
		//OptionsScene.transform.Find("ResolutionButton").Find("Text").GetComponent<TMP_Text>().text = Screen.width + "x" + Screen.height;
	}

	public void FullscreenToggle() {
		Screen.SetResolution(Screen.width, Screen.height, !Screen.fullScreen);
	}

	public void VsyncToggle() {
		if (QualitySettings.vSyncCount == 0) {
			QualitySettings.vSyncCount = 1;
			OptionsScene.transform.Find("VsyncButton").Find("Text").GetComponent<TMP_Text>().text = "V-SYNC: ON";
		} else {
			QualitySettings.vSyncCount = 0;
			OptionsScene.transform.Find("VsyncButton").Find("Text").GetComponent<TMP_Text>().text = "V-SYNC: OFF";
		}
	}

	public void OptionsSelect() {
		mainMenuScene.SetActive(false);
		OptionsScene.SetActive(true);
		levelSelectScene.SetActive(false);
	}

	public void LevelSelect() {
		mainMenuScene.SetActive(false);
		OptionsScene.SetActive(false);
		levelSelectScene.SetActive(true);
	}

	public void MainRootSelect() {
		mainMenuScene.SetActive(true);
		OptionsScene.SetActive(false);
		levelSelectScene.SetActive(false);
	}

	public void OnClickExit() {
		Application.Quit();
		print("game is quit");
	}

	public void LoadLevelOne() {
		SceneManager.LoadScene("Scenes/Demo1");
	}
}
