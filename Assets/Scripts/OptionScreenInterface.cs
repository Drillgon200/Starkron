using UnityEngine;
using UnityEngine.UI;

public class OptionScreenInterface : MonoBehaviour {

	public Slider verticalSensitivitySlider;
	public Slider horizontalSensitivitySlider;
	public Slider musicSlider;

	public AudioSource music;

	void Start() {
		verticalSensitivitySlider.value = GameSettings.instance.sensitivityScale.y;
		horizontalSensitivitySlider.value = GameSettings.instance.sensitivityScale.x;
		musicSlider.value = GameSettings.instance.musicVolume;
		music.volume = GameSettings.instance.musicVolume * 0.2F;
	}

	public void VerticalSensitivityChanged(float val) {
		GameSettings.instance.sensitivityScale.y = val;
	}
	public void HorizontalSensitivityChanged(float val) {
		GameSettings.instance.sensitivityScale.x = val;
	}
	public void MusicVolumeChanged(float val) {
		GameSettings.instance.musicVolume = val;
		music.volume = val * 0.2F;
	}
}
