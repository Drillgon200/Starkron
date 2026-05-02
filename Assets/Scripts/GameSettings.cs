using UnityEngine;

public class GameSettings : MonoBehaviour {
	public static GameSettings instance;

	public Vector2 sensitivityScale = new Vector2(1.0F, 1.0F);
	public float musicVolume = 1.0F;

	void Awake() {
		if (instance != null) {
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
}
