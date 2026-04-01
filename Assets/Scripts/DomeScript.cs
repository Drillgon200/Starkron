using UnityEngine;

public class DomeScript : MonoBehaviour, IDamageable {
	public void TakeDamage(float amount, Vector3 pos, IDamageable.DamageSource source) {
		//throw new System.NotImplementedException();
		//damage -1 health
	}

	void OnCollisionEnter(Collision other) {
		if (!(other.gameObject.CompareTag("PlayerTag")
			|| other.gameObject.CompareTag("DomeTag") )) {

			GameObject otherObj = other.gameObject;
			Rigidbody cubeRigidbody = this.GetComponent<Rigidbody>();
			cubeRigidbody.isKinematic = false;
		}
	}
}
