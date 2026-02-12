using UnityEngine;

public class MinimapUI : MonoBehaviour {
	public Texture uiSquare;
	public Texture lockOnIndicator;
	public Texture minimapTexture;
	public PlayerController player;
	public float minimapWorldRadius = 20.0F;
	void OnGUI() {
		if (Event.current.type != EventType.Repaint) {
			return;
		}
		float scale = Mathf.Min(Screen.width / 1920.0F, Screen.height / 1080.0F);
		if (player.planeMissileLockOnTarget != null) {
			Vector3 pos = player.lookCam.WorldToScreenPoint(player.planeMissileLockOnTarget.transform.position, Camera.MonoOrStereoscopicEye.Mono);
			if (pos.z > 0.0F) {
				float lockOnSize = 50.0F * scale;
				Graphics.DrawTexture(new Rect(pos.x - lockOnSize * 0.5F, Screen.height - pos.y - lockOnSize * 0.5F, lockOnSize, lockOnSize), lockOnIndicator);
			}
			
		}
		float minimapOFfset = 300.0F * scale;
		float minimapSize = 300.0F * scale;
		float squareSize = 10.0F * scale;
		Graphics.DrawTexture(new Rect(Screen.width - minimapOFfset - minimapSize, minimapOFfset, minimapSize, minimapSize), minimapTexture);
		Vector3 playerPos = player.transform.position;
		foreach (Collider enemy in Physics.OverlapBox(playerPos, new Vector3(minimapWorldRadius, minimapWorldRadius, minimapWorldRadius))) {
			if (enemy.GetComponent<IEnemy>() != null) {
				Vector3 pos = enemy.gameObject.transform.position;
				if (Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(playerPos.x, playerPos.z)) <= minimapWorldRadius) {
					Vector2 minimapProjection = new Vector2(pos.x - playerPos.x, pos.z - playerPos.z);
					Vector3 right = player.transform.right;
					Vector3 forward = player.transform.forward;
					minimapProjection = new Vector2(Vector2.Dot(minimapProjection, new Vector2(right.x, right.z)), -Vector2.Dot(minimapProjection, new Vector2(forward.x, forward.z)));
					minimapProjection = (minimapProjection / minimapWorldRadius * 0.5F + new Vector2(0.5F, 0.5F)) * minimapSize;
					Graphics.DrawTexture(new Rect(Screen.width - minimapOFfset - minimapSize + minimapProjection.x - squareSize * 0.5F, minimapOFfset + minimapProjection.y - squareSize * 0.5F, squareSize, squareSize), uiSquare);
				}
			}
		}
	}
}
