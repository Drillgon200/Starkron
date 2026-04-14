using UnityEngine;

public class WormSegmentController : MonoBehaviour, IDamageable {
	public WormBossController controller;
	public void TakeDamage(float amount, Vector3 pos, IDamageable.DamageSource source) {
		controller.TakeDamage(amount, pos, source);
	}
}
