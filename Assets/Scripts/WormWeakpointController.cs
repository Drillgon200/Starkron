using UnityEngine;

public class WormWeakpointController : MonoBehaviour, IDamageable {
	public WormSegmentController controller;
	public float health = 10.0F;
	public void TakeDamage(float amount, Vector3 pos, IDamageable.DamageSource source) {
		health -= amount;
		if (health <= 0.0F) {
			controller.controller.WeakpointDestroyed();
			Destroy(gameObject);
		}
	}
}
