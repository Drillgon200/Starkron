using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
	public Camera lookCam;
	public Rigidbody rigidBody;

	InputAction moveAction;
	InputAction lookAction;
	InputAction jumpAction;
	InputAction switchModeAction;
	InputAction rocketAction;
	InputAction machineGunAction;
	InputAction sprintAction;
	InputAction swordAction;

	float lookYaw;
	float lookPitch;
	bool cameraRayHit;
	Vector3 cameraRayHitPos;


	bool onGround;
	bool mechMode = true;
	float hoverFuelLeft;

	public float hoverSpeed = 10.0F;
	public float moveSpeedGround = 50.0F;
	public float moveSpeedAir = 10.0F;
	public float moveSpeedSprint = 50.0F;
	public float dragGround = 0.99F;
	public float dragSprint = 0.5F;
	public float dragAir = 0.1F;
	public float jumpSpeed = 3.0F;
	public float hoverFuelMax = 3.0F;

	public float flySpeed = 80.0F;
	public float flyDrag = 0.5F;

	public GameObject rocketPrefab;
	public float rocketFireRate = 4.0F;
	public float rocketCooldown = 4.0F;
	public int rocketSalvoCount = 4;
	public float rocketDamage = 20.0F;
	float rocketSpawnTimer = -1.0F;
	float rocketCooldownTimer;
	int rocketsLeftToFire;
	Vector3 rocketTargetPos;

	public GameObject machineGunBulletPrefab;
	public float machineGunMaxFireRate = 25.0F;
	public float machineGunFireRateWarmupRate = 10.0F;
	public float machineGunBulletDamage = 5.0F;
	float machineGunFireRate;
	float machineGunFireTimer;

	public BoxCollider swordHitbox;
	public float swordDamage = 50.0F;
	public float swordCooldown = 1.0F;
	float swordCooldownTimer;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		PlayerInput playerInput = GetComponent<PlayerInput>();
		moveAction = InputSystem.actions.FindAction("Move");
		lookAction = InputSystem.actions.FindAction("Look");
		jumpAction = InputSystem.actions.FindAction("Jump");
		switchModeAction = InputSystem.actions.FindAction("SwitchMode");
		switchModeAction.performed += (InputAction.CallbackContext ctx) => { if (ctx.performed) mechMode = !mechMode; };
		rocketAction = InputSystem.actions.FindAction("FireRockets");
		rocketAction.performed += (InputAction.CallbackContext ctx) => {
			if (ctx.performed && cameraRayHit && rocketCooldownTimer <= 0.0F) {
				rocketSpawnTimer = 0.0F;
				rocketsLeftToFire = rocketSalvoCount;
				rocketTargetPos = cameraRayHitPos;
			}
		};
		machineGunAction = InputSystem.actions.FindAction("FireMachineGun");
		sprintAction = InputSystem.actions.FindAction("Sprint");
		swordAction = InputSystem.actions.FindAction("Sword");
		swordAction.performed += (InputAction.CallbackContext ctx) => {
			if (ctx.performed && swordCooldownTimer <= 0.0F) {
				foreach (Collider toDamage in Physics.OverlapBox(swordHitbox.transform.position + swordHitbox.center, swordHitbox.size * 0.5F, swordHitbox.transform.rotation)) {
					IDamageable damageable = toDamage.GetComponent<IDamageable>();
					if (damageable != null) {
						damageable.TakeDamage(swordDamage, toDamage.ClosestPoint(transform.position));
					}
				}
				swordCooldownTimer = swordCooldown;
			}
		};

		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void OnEnable() { }

	void OnCollisionStay(Collision collision) {
		onGround = false;
		foreach (ContactPoint contact in collision.contacts) {
			if (contact.normal.y > 0.707F) {
				onGround = true;
				break;
			}
		}
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
			lookPitch = Mathf.Clamp(lookPitch - lookAmount.y * sensitivity * dt, -60.0F, 60.0F);
			transform.eulerAngles = new Vector3(0.0F, lookYaw, 0.0F);
			lookCam.transform.eulerAngles = new Vector3(lookPitch, lookYaw, 0.0F);
			Vector3 cameraStartPos = transform.position + new Vector3(0.0F, 2.0F, 0.0F);
			float cameraDistance = 10.0F;
			RaycastHit camBackHit;
			if (Physics.Raycast(cameraStartPos, -lookCam.transform.forward, out camBackHit, cameraDistance + 2.0F)) {
				cameraDistance = Mathf.Min(cameraDistance, Vector3.Distance(cameraStartPos, camBackHit.point) - 0.4F);
			}
			lookCam.transform.position = cameraStartPos - lookCam.transform.forward * cameraDistance;
		}
	}

	Vector3 random_vec3_in_cone(float coneHalfAngle) {
		float randPitch = Random.Range(0.0F, Mathf.Deg2Rad * coneHalfAngle);
		float randYaw = Random.Range(0.0F, Mathf.Deg2Rad * 360.0F);
		return new Vector3(Mathf.Cos(randYaw) * Mathf.Sin(randPitch), Mathf.Cos(randPitch), Mathf.Sin(randYaw) * Mathf.Sin(randPitch));
	}

	void FixedUpdate() {
		float dt = Time.fixedDeltaTime;
		RaycastHit lookHit;
		cameraRayHit = Physics.Raycast(new Ray(lookCam.transform.position, lookCam.transform.forward), out lookHit, float.PositiveInfinity, ~LayerMask.GetMask("Ignore Raycast"));
		if (cameraRayHit) {
			print(lookHit.collider);
		}
		cameraRayHitPos = lookHit.point;
		if (mechMode) {
			Vector3 velocity = new Vector3();
			bool sprinting = onGround && sprintAction.IsPressed();
			{ // Movement input
				Vector2 moveAmount = moveAction.ReadValue<Vector2>();
				Vector3 forward = new Vector3(-Mathf.Sin(-lookYaw * Mathf.Deg2Rad), 0.0F, Mathf.Cos(-lookYaw * Mathf.Deg2Rad));
				Vector3 right = Vector3.Cross(Vector3.up, forward);
				float moveSpeed =
					sprinting ? moveSpeedSprint :
					onGround ? moveSpeedGround :
					moveSpeedAir;
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
			float drag =
				sprinting ? dragSprint :
				onGround ? dragGround :
				dragAir;
			Vector3 velocityXZ = new Vector3(rigidBody.linearVelocity.x, 0.0F, rigidBody.linearVelocity.z);
			Vector3 dragAdjustment = velocityXZ * Mathf.Exp(dt * Mathf.Log(1.0F - drag)) - velocityXZ;
			rigidBody.useGravity = true;
			rigidBody.AddForce(velocity + dragAdjustment, ForceMode.VelocityChange);
			rigidBody.AddForce(Mathf.Abs(rigidBody.linearVelocity.x) < 0.1F ? -rigidBody.linearVelocity.x : 0.0F, 0.0F, Mathf.Abs(rigidBody.linearVelocity.z) < 0.1F ? -rigidBody.linearVelocity.z : 0.0F, ForceMode.VelocityChange);

			rocketSpawnTimer -= dt;
			if (rocketsLeftToFire > 0 && rocketSpawnTimer <= 0.0F) {
				rocketsLeftToFire--;
				rocketCooldownTimer = rocketCooldown;
				Vector3 startVelocity = random_vec3_in_cone(20.0F) * 15.0F;
				GameObject rocket = Instantiate(rocketPrefab, transform.position + new Vector3(0.0F, 1.5F, 0.0F), Quaternion.LookRotation(startVelocity));
				MissileControllerMechToGround rocketController = rocket.GetComponent<MissileControllerMechToGround>();
				rocketController.velocity = startVelocity;
				rocketController.target = rocketTargetPos;
				rocketController.damageAmount = rocketDamage;
				rocketSpawnTimer = 1.0F / rocketFireRate;
			}

			if (machineGunAction.IsPressed()) {
				machineGunFireRate = Mathf.Min(machineGunMaxFireRate, machineGunFireRate + machineGunFireRateWarmupRate * dt);
				float secondsPerBullet = 1.0F / machineGunFireRate;
				machineGunFireTimer += dt;
				while (machineGunFireTimer >= secondsPerBullet) {
					Vector3 fireFrom = transform.position + new Vector3(0.0F, 0.25F, 0.0F) + transform.forward;
					Vector3 fireTo = cameraRayHit ? cameraRayHitPos : lookCam.transform.position + lookCam.transform.forward * 1000.0F;
					Vector3 inaccuracy = random_vec3_in_cone(Mathf.Lerp(0.2F, 2.0F, machineGunFireRate / machineGunMaxFireRate));
					Vector3 fireVec = Quaternion.LookRotation(fireTo - fireFrom) * new Vector3(inaccuracy.x, inaccuracy.z, inaccuracy.y);
					RaycastHit bulletHit;
					bool bulletRayHit = Physics.Raycast(new Ray(fireFrom, fireVec), out bulletHit);
					fireTo = bulletRayHit ? bulletHit.point : fireFrom + fireVec * 1000.0F;
					GameObject bulletVFX = Instantiate(machineGunBulletPrefab, new Vector3(0.0F, 0.0F, 0.0F), Quaternion.identity);
					LineRenderer bulletRender = bulletVFX.GetComponent<LineRenderer>();
					bulletRender.SetPosition(0, fireFrom);
					bulletRender.SetPosition(1, fireTo);
					machineGunFireTimer -= secondsPerBullet;
					if (bulletRayHit) {
						IDamageable damageable = bulletHit.transform.gameObject.GetComponent<IDamageable>();
						if (damageable != null) {
							damageable.TakeDamage(machineGunBulletDamage, fireTo);
						}
					}
				}
			} else {
				// Not sure if we want this to slowly cooldown or require a full warm up every time
				//machineGunFireRate = Mathf.Max(0.0F, machineGunFireRate - machineGunFireRateWarmupRate * dt);
				machineGunFireRate = 0.0F;
				machineGunFireTimer = 0.0F;
			}
		} else { // fly mode
			Vector3 velocity = new Vector3();
			{ // Movement input
				Vector2 moveAmount = moveAction.ReadValue<Vector2>();
				velocity += lookCam.transform.forward * flySpeed * dt;
				//velocity += lookCam.transform.forward * moveAmount.y * flySpeed * dt;
				//velocity += lookCam.transform.right * moveAmount.x * flySpeed * dt;
			}
			Vector3 dragAdjustment = rigidBody.linearVelocity * Mathf.Exp(dt * Mathf.Log(1.0F - flyDrag)) - rigidBody.linearVelocity;
			rigidBody.AddForce(velocity + dragAdjustment, ForceMode.VelocityChange);
			rigidBody.useGravity = false;
		}
		rocketCooldownTimer -= dt;
		swordCooldownTimer -= dt;
	}
}
