using UnityEngine;

public class JellyAcidController : MonoBehaviour {
	public float damage = 2.0F;
	void OnCollisionEnter(Collision collision) {
		foreach (Collider c in Physics.OverlapSphere(transform.position, 1.5F)) {
			c.GetComponent<IDamageable>()?.TakeDamage(damage, c.ClosestPoint(transform.position), IDamageable.DamageSource.BUG);
			if (c.GetComponent<PlayerCollisionController>()) {
				PlayerController.instance.TakeDamage(damage);
			}
		}
		Destroy(gameObject);
	}
}
