using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScreenInterface : MonoBehaviour {
	public GameObject pauseOverlay;
	public GameObject winScreen;
	public GameObject loseScreen;
	public GameObject messageWindowDemo1;
	public GameObject messageWindowTwoDemo1;
	public GameObject messageShop;
	public GameObject messagePressE;
	public GameObject shopOverlay;
	public GameObject waveInfo;
	public AudioSource alert;
	public AudioClip denySound;
	bool guiActive;

	public GameObject turretPrefab;
	public GameObject turretHologramPrefab;
	int turretCost = 1;

	struct QueuedAlertMessage {
		public GameObject toDisplay;
		public float time;

		public QueuedAlertMessage(GameObject toDisplay, float time) {
			this.toDisplay = toDisplay;
			this.time = time;
		}
	}

	Queue<QueuedAlertMessage> alertQueue = new();
	GameObject activeAlert;
	float queueTime = 1.0F;
	public float timeBetweenAlerts = 1.0F;

	void Start() {
		pauseOverlay.SetActive(false);
		winScreen.SetActive(false);
		loseScreen.SetActive(false);
		messageWindowDemo1.SetActive(false);
		messageWindowTwoDemo1.SetActive(false);
		shopOverlay.SetActive(false);
		//temporary pop up messages for the DEMO1
		EnqueueAlert(messageWindowDemo1, 4.0F);
		EnqueueAlert(messageWindowTwoDemo1, 4.0F);
	}

	void FixedUpdate() {
		queueTime -= Time.fixedDeltaTime;
		if (queueTime <= 0.0F) {
			if (activeAlert) {
				activeAlert.SetActive(false);
				activeAlert = null;
				queueTime = timeBetweenAlerts;
			} else if (alertQueue.Count > 0) {
				QueuedAlertMessage msg = alertQueue.Dequeue();
				activeAlert = msg.toDisplay;
				queueTime = msg.time;
				activeAlert.SetActive(true);
				alert.Play();
			}
		}
	}

	public void EnqueueAlert(GameObject alert, float time) {
		alertQueue.Enqueue(new QueuedAlertMessage(alert, time));
	}
	public void SetInteractIndicator(bool enabled) {
		messagePressE.SetActive(enabled);
	}

	public void OpenShop() {
		shopOverlay.SetActive(true);
		shopOverlay.transform.Find("Item current").Find("Price Text").GetComponent<TMP_Text>().text = "$" + turretCost;
		PlayerController.instance.SetMouseCapture(false);
		PlayerController.instance.actionsDisabled = true;
		guiActive = true;
	}
	public void CloseShop() {
		shopOverlay.SetActive(false);
		PlayerController.instance.SetMouseCapture(true);
		PlayerController.instance.actionsDisabled = false;
		guiActive = false;
	}
	public void TryBuyTurret() {
		PlayerController player = PlayerController.instance;
		if (player.GetBankTotal() >= turretCost && !player.isPlacingObject) {
			player.SetBankTotal(player.GetBankTotal() - turretCost);
			turretCost++;
			shopOverlay.transform.Find("Item current").Find("Price Text").GetComponent<TMP_Text>().text = "$" + turretCost;
			player.isPlacingObject = true;
			player.canPlaceObject = false;
			player.placementPrefab = turretPrefab;
			player.placementHologram = Instantiate(turretHologramPrefab, player.transform.position, Quaternion.identity);
			CloseShop();
		} else {
			SoundFXManager.instance.PlaySoundFXClip(denySound, player.transform, 1.0F);
		}
	}

	public void ShowNextWaveIndicator() {
		waveInfo.SetActive(true);
		waveInfo.transform.Find("Wave").GetComponent<TMP_Text>().text = "Wave complete!";
		Invoke("ShowNextWaveNumber", 2);
	}
	void ShowNextWaveNumber() {
		waveInfo.transform.Find("Wave").GetComponent<TMP_Text>().text = "Wave " + (GameManager.instance.currentWave + 1);
		Invoke("CloseWaveInfo", 2);
	}
	void CloseWaveInfo() {
		waveInfo.SetActive(false);
	}

	public void PauseToggle() {
		if (!guiActive) {
			Time.timeScale = pauseOverlay.activeSelf ? 1.0F : 0.0F;
			PlayerController.instance.SetMouseCapture(pauseOverlay.activeSelf);
			pauseOverlay.SetActive(!pauseOverlay.activeSelf);
		}
	}

	public void ShowWinOverlay() {
		PlayerController.instance.SetMouseCapture(false);
		winScreen.SetActive(true);
		winScreen.transform.Find("Stats").GetComponent<TMP_Text>().text = GameManager.instance.get_stats_string();
		// Just in case
		loseScreen.SetActive(false);
		pauseOverlay.SetActive(false);
		shopOverlay.SetActive(false);
		guiActive = true;
	}

	public void ShowLoseOverlay() {
		PlayerController.instance.SetMouseCapture(false);
		loseScreen.SetActive(true);
		loseScreen.transform.Find("Stats").GetComponent<TMP_Text>().text = GameManager.instance.get_stats_string();
		// Just in case
		winScreen.SetActive(false);
		pauseOverlay.SetActive(false);
		shopOverlay.SetActive(false);
		guiActive = true;
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
