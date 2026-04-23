using UnityEngine;

public class TimedDestroy : MonoBehaviour {
	public float time;
	void Awake() {
		Destroy(gameObject, time);
	}
}
