using UnityEngine;

public class MinimapUI : MonoBehaviour {
	public Texture xIcon;
	public Texture oIcon;
	public Texture lockOnIndicator;
	public Texture minimapTexture;
	public Texture shieldBarTexture;
	public Material shieldBarMat;
	public PlayerController player;
	public float minimapWorldRadius = 20.0F;
	public float minimapVirtualSize = 300.0F;

	void DrawMinimapEnemies(float x, float y, float scale, int mask, Texture icon) {
		float minimapSize = minimapVirtualSize * scale;
		Vector3 playerPos = player.transform.position;
		float squareSize = 10.0F * scale;
		foreach (Collider enemy in Physics.OverlapBox(playerPos, new Vector3(minimapWorldRadius, minimapWorldRadius, minimapWorldRadius), Quaternion.identity, mask)) {
			Vector3 pos = enemy.gameObject.transform.position;
			if (Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(playerPos.x, playerPos.z)) <= minimapWorldRadius * 0.95F) {
				Vector2 minimapProjection = new Vector2(pos.x - playerPos.x, pos.z - playerPos.z);
				Vector2 forward = player.Get2DForward();
				Vector2 right = new Vector2(forward.y, -forward.x);
				minimapProjection = new Vector2(Vector2.Dot(minimapProjection, right), -Vector2.Dot(minimapProjection, forward));
				minimapProjection = (minimapProjection / minimapWorldRadius * 0.5F + new Vector2(0.5F, 0.5F)) * minimapSize;
				Graphics.DrawTexture(new Rect(Screen.width - x - minimapSize + minimapProjection.x - squareSize * 0.5F, y + minimapProjection.y - squareSize * 0.5F, squareSize, squareSize), icon);
			}
		}
	}

	void OnGUI() {
		if (Event.current.type != EventType.Repaint) {
			return;
		}

		// Lock-on indicator
		float scale = Mathf.Min(Screen.width / 1920.0F, Screen.height / 1080.0F);
		if (player.planeMissileLockOnTarget != null) {
			Vector3 pos = player.lookCam.WorldToScreenPoint(player.planeMissileLockOnTarget.transform.position, Camera.MonoOrStereoscopicEye.Mono);
			if (pos.z > 0.0F) {
				float lockOnSize = 50.0F * scale;
				Graphics.DrawTexture(new Rect(pos.x - lockOnSize * 0.5F, Screen.height - pos.y - lockOnSize * 0.5F, lockOnSize, lockOnSize), lockOnIndicator);
			}
			
		}

		// Minimap
		float minimapOffsetX = 200.0F * scale;
		float minimapOffsetY = 150.0F * scale;
		float minimapSize = minimapVirtualSize * scale;
		Graphics.DrawTexture(new Rect(Screen.width - minimapOffsetX - minimapSize, minimapOffsetY, minimapSize, minimapSize), minimapTexture, Rect.MinMaxRect(0.0F, 0.0F, 1.0F, 1.0F), 0, 0, 0, 0, Color.magenta * 0.5F);
		int enemyMask = 1 << 7;
		DrawMinimapEnemies(minimapOffsetX, minimapOffsetY, scale, enemyMask, xIcon);
		int flyingEnemyMask = 1 << 8;
		DrawMinimapEnemies(minimapOffsetX, minimapOffsetY, scale, flyingEnemyMask, oIcon);

		// Shield bar
		float shieldSizeX = 800.0F * scale;
		float shieldSizeY = shieldSizeX * 0.25F;
		float shieldOffsetX = Screen.width * 0.5F - shieldSizeX * 0.5F;
		float shieldOffsetY = -50.0F * scale;
		shieldBarMat.SetFloat("_Health", PlayerController.instance.GetHealthNormalized());
		Graphics.DrawTexture(new Rect(shieldOffsetX, shieldOffsetY, shieldSizeX, shieldSizeY), shieldBarTexture, shieldBarMat);
	}
}
