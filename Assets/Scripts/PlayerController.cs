using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	public Camera lookCam;
	public Rigidbody rigidBody;

	InputAction moveAction;
	InputAction lookAction;
	InputAction jumpAction;

	float lookYaw;
	float lookPitch;

	bool onGround;
	float hoverFuelLeft;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		PlayerInput playerInput = GetComponent<PlayerInput>();
		moveAction = InputSystem.actions.FindAction("Move");
		lookAction = InputSystem.actions.FindAction("Look");
		jumpAction = InputSystem.actions.FindAction("Jump");

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void OnEnable() {
		
	}
	void OnCollisionStay(Collision collision) {
		onGround = false;
		foreach (ContactPoint contact in collision.contacts) {
			if (contact.normal.y > 0.707F) {
				onGround = true;
				break;
			}
		}
		print(onGround);
	}
	void OnCollisionExit(Collision collision) {
		onGround = false;
	}

	// Update is called once per frame
	void Update() {
		float dt = Time.deltaTime;
		{ // Look update
			float sensitivity = 50.0F;
			Vector2 lookAmount = lookAction.ReadValue<Vector2>();
			lookYaw += lookAmount.x * sensitivity * dt;
			lookPitch = Mathf.Clamp(lookPitch - lookAmount.y * sensitivity * dt, -89.999F, 89.999F);
			lookCam.transform.eulerAngles = new Vector3(lookPitch, lookYaw, 0.0F);
			lookCam.transform.position = transform.position - lookCam.transform.forward * 10.0F + new Vector3(0.0F, 2.0F, 0.0F);
		}
	}

	void FixedUpdate() {
		float hoverSpeed = 10.0F;
		float moveSpeedGround = 50.0F;
		float moveSpeedAir = 10.0F;
		float dragGround = 0.99F;
		float dragAir = 0.1F;
		float jumpSpeed = 4.0F;
		float hoverFuelMax = 3.0F;

		float dt = Time.fixedDeltaTime;
		Vector3 velocity = new Vector3();
		{ // Movement input
			Vector2 moveAmount = moveAction.ReadValue<Vector2>();
			Vector3 forward = new Vector3(-Mathf.Sin(-lookYaw * Mathf.Deg2Rad), 0.0F, Mathf.Cos(-lookYaw * Mathf.Deg2Rad));
			Vector3 right = Vector3.Cross(Vector3.up, forward);
			float moveSpeed = onGround ? moveSpeedGround : moveSpeedAir;
			velocity += right * moveAmount.x * moveSpeed * dt;
			velocity += forward * moveAmount.y * moveSpeed * dt;
			if (jumpAction.IsPressed() && hoverFuelLeft > 0.0F) {
				velocity.y += hoverSpeed * dt;
				hoverFuelLeft -= dt;
			}
			if (onGround && jumpAction.IsPressed()) {
				velocity.y += jumpSpeed;
				hoverFuelLeft = hoverFuelMax;
			}
		}
		float drag = onGround ? dragGround : dragAir;
		Vector3 velocityXZ = new Vector3(rigidBody.linearVelocity.x, 0.0F, rigidBody.linearVelocity.z);
		Vector3 dragAdjustment = velocityXZ * Mathf.Exp(dt * Mathf.Log(1.0F - drag)) - velocityXZ;
		rigidBody.AddForce(velocity + dragAdjustment, ForceMode.VelocityChange);
		rigidBody.AddForce(Mathf.Abs(rigidBody.linearVelocity.x) < 0.1F ? -rigidBody.linearVelocity.x : 0.0F, 0.0F, Mathf.Abs(rigidBody.linearVelocity.z) < 0.1F ? -rigidBody.linearVelocity.z : 0.0F, ForceMode.VelocityChange);
	}
}
