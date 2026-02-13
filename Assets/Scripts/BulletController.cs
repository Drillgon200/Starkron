using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletController : MonoBehaviour {

	public Vector3 velocity;
	public float damageAmount;
	public float maxAge = 5.0F;
	float age;
	LineRenderer lineRenderer;
	Vector3 originalPosition;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		lineRenderer = GetComponent<LineRenderer>();
		originalPosition = transform.position;
	}

	// Update is called once per frame
	void Update() {
		float dt = Time.deltaTime;
		age += dt;
		if (age > maxAge) {
			Destroy(gameObject);
			return;
		}
		RaycastHit hit;
		if (age > 0.05F && Physics.Raycast(new Ray(transform.position, velocity * dt), out hit, dt * velocity.magnitude)) {
			hit.collider.GetComponent<IDamageable>()?.TakeDamage(damageAmount, hit.point);
			Destroy(gameObject);
		}
		transform.position += dt * (velocity + Physics.gravity * dt * 0.5F);
		velocity += Physics.gravity * dt;
		lineRenderer.SetPosition(0, transform.position - Vector3.Normalize(velocity) * Mathf.Min(10.0F, Vector3.Distance(transform.position, originalPosition)));
		lineRenderer.SetPosition(1, transform.position);
	}
}
