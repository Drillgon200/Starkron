using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour {
	public static PlayerController instance;
	public Camera lookCam;
	public Rigidbody rigidBody;
	public GameObject playerModelObject;
	public MeshFilter renderMeshFilter;
	public Collider mechCollider;
	public Collider planeCollider;
	public Mesh mechMesh;
	public Mesh planeMesh;
	public UIScreenInterface uiScreen;

	// Mech actions
	InputAction moveAction;
	InputAction lookAction;
	InputAction jumpAction;
	InputAction switchModeAction;
	InputAction rocketAction;
	InputAction machineGunAction;
	InputAction sprintAction;
	InputAction swordAction;
	InputAction pauseAction;

	// Plane actions
	InputAction planeMissileAction;
	InputAction boostAction;
	InputAction airbrakeAction;
	InputAction firePlaneGunAction;

	public float cameraDistance = 10.0F;
	public float cameraRise = 2.0F;
	float lookYaw;
	float lookPitch;
	bool mouseCaptured;
	bool cameraRayHit;
	Vector3 cameraRayHitPos;
	public Vector3 lookForward;
	public Vector3 forward;
	public Vector3 right;


	bool onGround;
	float hoverFuelLeft;
	public float maxHealth = 100.0F;
	float health;
	public float healthRechargeRate = 20.0F;
	public float healthRechargeCooldown = 3.0F;
	float healthRechargeCooldownTimer = 0.0F;

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
	public float boostSpeed = 180.0F;
	public float planeManeuverSpeed = 10.0F;
	public float flyDrag = 0.5F;

	public GameObject rocketPrefab;
	public float rocketFireRate = 4.0F;
	public float rocketCooldown = 4.0F;
	public int rocketSalvoCount = 4;
	public float rocketRandomTargetRadius = 3.0F;
	public float rocketDamage = 20.0F;
	float rocketSpawnTimer = -1.0F;
	float rocketCooldownTimer;
	int rocketsLeftToFire;
	Vector3 rocketTargetPos;

	public GameObject machineGunBulletPrefab;
	public float machineGunStartFireRate = 5.0F;
	public float machineGunMaxFireRate = 25.0F;
	public float machineGunFireRateWarmupRate = 10.0F;
	public float machineGunBulletDamage = 5.0F;
	float machineGunFireRate;
	float machineGunFireTimer;

	public BoxCollider swordHitbox;
	public float swordDamage = 50.0F;
	public float swordCooldown = 1.0F;
	float swordCooldownTimer;

	public GameObject lockOnMissilePrefab;
	public float planeMissileTrackingHoldTimeCutoff = 0.25F;
	public float planeMissileLockOnRadius = 500.0F;
	public float planeMissileLockOnAngle = 10.0F;
	public float planeMissileSpeed = 2.0F;
	public float planeMissileFireCooldown = 0.5F;
	float planeMissileCooldownTimer;
	float planeMissileHoldTime;
	public GameObject planeMissileLockOnTarget = null;

	public GameObject planeBulletPrefab;
	int planeBulletFireCount;
	public float planeGunFireRate = 5.0F;
	public float planeBulletDamage = 50.0F;
	float planeBulletCooldownTimer;


	Vector3 planeTiltRotation;
	Vector3 planeTiltRotationVelocity;
	float planeBoostTurnRotation;
	public float planeTiltSpringStiffness = 0.5F;
	public float planeTiltSpringDamping = 0.9F;
	float boostTimer;

	System.Action<InputAction.CallbackContext> switchModePerformedAction;
	System.Action<InputAction.CallbackContext> rocketPerformedAction;
	System.Action<InputAction.CallbackContext> swordPerformedAction;
	System.Action<InputAction.CallbackContext> planeMissileCanceledAction;
	System.Action<InputAction.CallbackContext> pausePerformedAction;

	//ROSS changes
	public int bankTotal = 0;
	public TMP_Text bankUICounterText;

	enum TransformState {
		MECH,
		PLANE,
		MECH_TO_PLANE,
		PLANE_TO_MECH
	};
	TransformState transformState = TransformState.MECH;
	public float transformTime = 3.0F;
	float transformCooldown;

	public Vector2 Get2DForward() {
		return new Vector2(Mathf.Sin(lookYaw * Mathf.Deg2Rad), Mathf.Cos(lookYaw * Mathf.Deg2Rad));
	}
	void Awake() {
		instance = this;
	}

	public void SetMouseCapture(bool cap) {
		if (cap) {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		} else {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		mouseCaptured = cap;
	}

	public float GetHealthNormalized() {
		return Mathf.Clamp01(health / maxHealth);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		moveAction = InputSystem.actions.FindAction("Move");
		lookAction = InputSystem.actions.FindAction("Look");
		jumpAction = InputSystem.actions.FindAction("Jump");
		switchModeAction = InputSystem.actions.FindAction("SwitchMode");
		switchModeAction.performed += (switchModePerformedAction = (InputAction.CallbackContext ctx) => {
			if (GameManager.instance.gameOver) {
				return;
			}
			switch (transformState) {
			case TransformState.MECH: {
				transformState = TransformState.MECH_TO_PLANE;
			} break;
			case TransformState.PLANE: {
				transformState = TransformState.PLANE_TO_MECH;
			} break;
			}
			transformCooldown = transformTime;

		});
		rocketAction = InputSystem.actions.FindAction("FireRockets");
		rocketAction.performed += (rocketPerformedAction = (InputAction.CallbackContext ctx) => {
			if (transformState == TransformState.MECH && cameraRayHit && rocketCooldownTimer <= 0.0F) {
				rocketSpawnTimer = 0.0F;
				rocketsLeftToFire = rocketSalvoCount;
				rocketTargetPos = cameraRayHitPos;
			}
		});
		machineGunAction = InputSystem.actions.FindAction("FireMachineGun");
		sprintAction = InputSystem.actions.FindAction("Sprint");
		swordAction = InputSystem.actions.FindAction("Sword");
		swordAction.performed += (swordPerformedAction = (InputAction.CallbackContext ctx) => {
			if (swordCooldownTimer <= 0.0F && !GameManager.instance.gameOver) {
				foreach (Collider toDamage in Physics.OverlapBox(swordHitbox.transform.position + swordHitbox.center, swordHitbox.size * 0.5F, swordHitbox.transform.rotation)) {
					IDamageable damageable = toDamage.GetComponent<IDamageable>();
					if (damageable != null) {
						damageable.TakeDamage(swordDamage, toDamage.ClosestPoint(transform.position));
					}
				}
				swordCooldownTimer = swordCooldown;
			}
		});

		planeMissileAction = InputSystem.actions.FindAction("ActivatePlaneMissile");
		planeMissileAction.canceled += (planeMissileCanceledAction = (InputAction.CallbackContext ctx) => {
			if (transformState == TransformState.PLANE && planeMissileCooldownTimer <= 0.0F && !GameManager.instance.gameOver) {
				Vector3 startVelocity = lookForward * 100.0F;
				GameObject missile = Instantiate(lockOnMissilePrefab, transform.position + lookForward * 1.5F, Quaternion.LookRotation(startVelocity));
				MissileControllerPlane missileController = missile.GetComponent<MissileControllerPlane>();
				missileController.velocity = startVelocity;
				missileController.speed = planeMissileSpeed;
				missileController.target = planeMissileLockOnTarget;
				missileController.damageAmount = rocketDamage;

				planeMissileCooldownTimer = planeMissileFireCooldown;
			}
		});
		boostAction = InputSystem.actions.FindAction("Boost");
		airbrakeAction = InputSystem.actions.FindAction("Airbrake");
		firePlaneGunAction = InputSystem.actions.FindAction("FirePlaneGun");
		pauseAction = InputSystem.actions.FindAction("Pause");
		pauseAction.performed += (pausePerformedAction = (InputAction.CallbackContext ctx) => uiScreen.PauseToggle());

		SetMouseCapture(true);
		health = maxHealth;
	}

	void OnTriggerEnter(Collider other) {
		if (other.transform.tag == "CrystalTag") {
			bankTotal++;
			bankUICounterText.text = "Crystal x" + bankTotal.ToString();
			Destroy(other.gameObject);
		}
	}

	private void OnDestroy() {
		pauseAction.performed -= pausePerformedAction;
		planeMissileAction.canceled -= planeMissileCanceledAction;
		swordAction.performed -= swordPerformedAction;
		rocketAction.performed -= rocketPerformedAction;
		switchModeAction.performed -= switchModePerformedAction;
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

	float SmoothCutoff(float x, float max) {
		if (Mathf.Abs(max) < 0.000001F) {
			return Mathf.Min(x, max);
		}
		// A quick function I made in Desmos that smoothly tapers off x to an asymptote defined by max.
		// It's mostly linear in the first half of the function, then curves off towards max as x goes to infinity
		float halfwayTarget = x * 2.0F / max;
		float remapped = max - max / (halfwayTarget * halfwayTarget + 1.0F);
		return remapped * Mathf.Sign(x);
	}

	// Update is called once per frame
	void Update() {
		if (GameManager.instance.gameOver) {
			return;
		}
		float dt = Time.deltaTime;
		{ // Look update
			float sensitivity = 10.0F / 100.0F;
			Vector2 lookAmount = lookAction.ReadValue<Vector2>();
			if (!mouseCaptured) {
				lookAmount = Vector2.zero;
			}
			switch (transformState) {
			case TransformState.MECH:
			case TransformState.MECH_TO_PLANE:
			case TransformState.PLANE_TO_MECH: {
				lookYaw += lookAmount.x * sensitivity;
				lookPitch = Mathf.Clamp(lookPitch - lookAmount.y * sensitivity, -60.0F, 60.0F);
				transform.eulerAngles = new Vector3(0.0F, lookYaw, 0.0F);
				playerModelObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			}
			break;
			case TransformState.PLANE: {
				bool boost = boostAction.IsPressed();

				float maxPlaneTurnSpeed = boost ? 100.0F * dt : 400.0F * dt;
				float yawLookChange = SmoothCutoff(lookAmount.x * sensitivity, maxPlaneTurnSpeed);
				float pitchLookChange = -SmoothCutoff(lookAmount.y * sensitivity, maxPlaneTurnSpeed);
				lookYaw += yawLookChange;
				lookPitch = Mathf.Clamp(lookPitch + pitchLookChange, -60.0F, 60.0F);
				transform.eulerAngles = new Vector3(lookPitch, lookYaw, 0.0F);

				float boostRotationTarget = boost ? yawLookChange / maxPlaneTurnSpeed : 0.0F;
				float boostTurnRotationRate = 0.95F;
				planeBoostTurnRotation = (planeBoostTurnRotation - boostRotationTarget) * Mathf.Exp(dt * Mathf.Log(1.0F - boostTurnRotationRate)) + boostRotationTarget;
				playerModelObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				playerModelObject.transform.Translate(0.0F, cameraRise, 0.0F, Space.Self);
				playerModelObject.transform.Rotate(0.0F, 0.0F, -planeBoostTurnRotation * 90.0F, Space.Self);
				playerModelObject.transform.Translate(0.0F, -cameraRise, 0.0F, Space.Self);
				planeTiltRotationVelocity *= Mathf.Exp(dt * Mathf.Log(1.0F - planeTiltSpringDamping));
				planeTiltRotation += planeTiltRotationVelocity * dt;
				playerModelObject.transform.localRotation *= Quaternion.AngleAxis(planeTiltRotation.magnitude * Mathf.Rad2Deg, planeTiltRotation);
				planeTiltRotationVelocity.z -= yawLookChange * 0.05F;
				planeTiltRotationVelocity.y += yawLookChange * 0.015F;
				planeTiltRotationVelocity.x += pitchLookChange * 0.025F;
			} break;
			}
			lookCam.transform.eulerAngles = new Vector3(lookPitch, lookYaw, 0.0F);
			// Somehow lookCam.transform.forward just doesn't update if the framerate gets too low so we have to construct it manually
			// I could not find an adequate explanation of why this happens, and look input update must happen in Update so that camera look is smooth, so I'm just going to do it like this
			lookForward = new Vector3(-Mathf.Sin(-lookYaw * Mathf.Deg2Rad) * Mathf.Cos(lookPitch * Mathf.Deg2Rad), -Mathf.Sin(lookPitch * Mathf.Deg2Rad), Mathf.Cos(-lookYaw * Mathf.Deg2Rad) * Mathf.Cos(lookPitch * Mathf.Deg2Rad));
			forward = new Vector3(-Mathf.Sin(-lookYaw * Mathf.Deg2Rad), 0.0F, Mathf.Cos(-lookYaw * Mathf.Deg2Rad));
			right = Vector3.Cross(Vector3.up, forward);
			RaycastHit lookHit;
			cameraRayHit = Physics.Raycast(new Ray(lookCam.transform.position, lookForward), out lookHit, float.PositiveInfinity, ~LayerMask.GetMask("Ignore Raycast"));
			cameraRayHitPos = lookHit.point;
			Vector3 cameraStartPos = transform.position + new Vector3(0.0F, cameraRise, 0.0F);
			RaycastHit camBackHit;
			float modifiedCameraDistance = cameraDistance;
			if (Physics.Raycast(cameraStartPos, -lookForward, out camBackHit, modifiedCameraDistance + cameraRise)) {
				modifiedCameraDistance = Mathf.Min(modifiedCameraDistance, Vector3.Distance(cameraStartPos, camBackHit.point) - 0.4F);
			}
			lookCam.transform.position = cameraStartPos - lookForward * modifiedCameraDistance;
		}

		if (healthRechargeCooldownTimer <= 0.0F) {
			health = Mathf.Min(maxHealth, health + healthRechargeRate * dt);
		}
		healthRechargeCooldownTimer -= dt;
	}

	Vector3 RandomVec3InCone(float coneHalfAngle) {
		float randPitch = Random.Range(0.0F, Mathf.Deg2Rad * coneHalfAngle);
		float randYaw = Random.Range(0.0F, Mathf.Deg2Rad * 360.0F);
		return new Vector3(Mathf.Cos(randYaw) * Mathf.Sin(randPitch), Mathf.Cos(randPitch), Mathf.Sin(randYaw) * Mathf.Sin(randPitch));
	}

	void FixedUpdate() {
		if (GameManager.instance.gameOver) {
			return;
		}
		float dt = Time.fixedDeltaTime;
		switch (transformState) {
		case TransformState.MECH: {
			Vector3 velocity = new Vector3();
			bool sprinting = onGround && sprintAction.IsPressed();
			{ // Movement input
				Vector2 moveAmount = moveAction.ReadValue<Vector2>();
				
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
				Vector3 startVelocity = RandomVec3InCone(20.0F) * 15.0F;
				GameObject rocket = Instantiate(rocketPrefab, transform.position + new Vector3(0.0F, 1.5F, 0.0F), Quaternion.LookRotation(startVelocity));
				MissileControllerMechToGround rocketController = rocket.GetComponent<MissileControllerMechToGround>();
				rocketController.velocity = startVelocity;
				rocketController.target = rocketTargetPos + new Vector3(Random.Range(-rocketRandomTargetRadius, rocketRandomTargetRadius), 0.0F, Random.Range(-rocketRandomTargetRadius, rocketRandomTargetRadius));
				rocketController.damageAmount = rocketDamage;
				rocketSpawnTimer = 1.0F / rocketFireRate;
			}

			if (machineGunAction.IsPressed()) {
				machineGunFireRate = Mathf.Min(machineGunMaxFireRate, machineGunFireRate + machineGunFireRateWarmupRate * dt);
				float secondsPerBullet = 1.0F / machineGunFireRate;
				if (machineGunFireTimer >= secondsPerBullet) {
					Vector3 fireFrom = transform.position + new Vector3(0.0F, 0.25F, 0.0F) + forward;
					Vector3 fireTo = cameraRayHit ? cameraRayHitPos : lookCam.transform.position + lookForward * 1000.0F;
					Vector3 inaccuracy = RandomVec3InCone(Mathf.Lerp(0.2F, 2.0F, machineGunFireRate / machineGunMaxFireRate));
					Vector3 fireVec = Quaternion.LookRotation(fireTo - fireFrom) * new Vector3(inaccuracy.x, inaccuracy.z, inaccuracy.y);
					RaycastHit bulletHit;
					bool bulletRayHit = Physics.Raycast(new Ray(fireFrom, fireVec), out bulletHit);
					fireTo = bulletRayHit ? bulletHit.point : fireFrom + fireVec * 1000.0F;
					GameObject bulletVFX = Instantiate(machineGunBulletPrefab, new Vector3(0.0F, 0.0F, 0.0F), Quaternion.identity);
					LineRenderer bulletRender = bulletVFX.GetComponent<LineRenderer>();
					bulletRender.SetPosition(0, fireFrom);
					bulletRender.SetPosition(1, fireTo);
					machineGunFireTimer = 0.0F;
					if (bulletRayHit) {
						IDamageable damageable = bulletHit.transform.gameObject.GetComponent<IDamageable>();
						damageable?.TakeDamage(machineGunBulletDamage, fireTo);
					}
				}
			} else {
				// Not sure if we want this to slowly cooldown or require a full warm up every time
				//machineGunFireRate = Mathf.Max(0.0F, machineGunFireRate - machineGunFireRateWarmupRate * dt);
				machineGunFireRate = machineGunStartFireRate;
			}
			machineGunFireTimer += dt;
		}
		break;
		case TransformState.PLANE: {
			Vector3 velocity = new Vector3();
			{ // Movement input
				bool boost = boostAction.IsPressed();
				bool brake = airbrakeAction.IsPressed();
				if (!brake) {
					velocity += lookForward * (boost ? boostSpeed : flySpeed) * dt;
				}
				if (!boost) {
					Vector2 moveAmount = moveAction.ReadValue<Vector2>();
					velocity += lookForward * moveAmount.y * planeManeuverSpeed * dt;
					velocity += right * moveAmount.x * planeManeuverSpeed * dt;
					planeTiltRotationVelocity.x += moveAmount.y * 0.25F;
					planeTiltRotationVelocity.z -= moveAmount.x * 0.25F;
				}
			}

			if (firePlaneGunAction.IsPressed() && planeBulletCooldownTimer < 0.0F) {
				planeBulletFireCount++;
				Vector3 fireFrom = transform.localToWorldMatrix * new Vector4((planeBulletFireCount & 1) == 0 ? 1.0F : -1.0F, 0.0F, -1.0F, 1.0F);
				GameObject bullet = Instantiate(planeBulletPrefab, fireFrom, Quaternion.identity);
				BulletController bulletController = bullet.GetComponent<BulletController>();
				Vector3 fireDirection = cameraRayHit ? Vector3.Normalize(cameraRayHitPos - fireFrom) : lookForward;
				bulletController.velocity = fireDirection * 400.0F;
				bulletController.damageAmount = planeBulletDamage;
				planeBulletCooldownTimer = 1.0F / planeGunFireRate;
			}

			if (planeMissileAction.IsPressed()) {
				planeMissileHoldTime += dt;
				if (planeMissileHoldTime > planeMissileTrackingHoldTimeCutoff && planeMissileLockOnTarget == null) {
					float cosLockOnAngle = Mathf.Cos(planeMissileLockOnAngle * Mathf.Deg2Rad);
					float bestDistance = float.PositiveInfinity;
					foreach (Collider collider in Physics.OverlapBox(transform.position, new Vector3(planeMissileLockOnRadius, planeMissileLockOnRadius, planeMissileLockOnRadius))) {
						if (collider.GetComponent<IFlyingEnemy>() != null && Vector3.Dot(Vector3.Normalize(collider.transform.position - lookCam.transform.position), lookForward) > cosLockOnAngle) {
							float distanceToTarget = Vector3.Distance(transform.position, collider.transform.position);
							if (distanceToTarget < bestDistance) {
								planeMissileLockOnTarget = collider.gameObject;
								bestDistance = distanceToTarget;
							}
						}
					}
				}
			} else {
				planeMissileLockOnTarget = null;
				planeMissileHoldTime = 0.0F;
			}

			Vector3 dragAdjustment = rigidBody.linearVelocity * Mathf.Exp(dt * Mathf.Log(1.0F - flyDrag)) - rigidBody.linearVelocity;
			rigidBody.AddForce(velocity + dragAdjustment, ForceMode.VelocityChange);
			rigidBody.useGravity = false;
			planeTiltRotationVelocity -= planeTiltRotation * planeTiltSpringStiffness * dt;
		} break;
		case TransformState.MECH_TO_PLANE: {
			if (rigidBody.linearVelocity.y < 0.0F) {
				rigidBody.AddForce(Vector3.up * -rigidBody.linearVelocity.y * 0.9F, ForceMode.VelocityChange);
			}
			if (transformCooldown <= 0.0F) {
				transformState = TransformState.PLANE;
				rigidBody.useGravity = false;
				renderMeshFilter.mesh = planeMesh;
				planeCollider.enabled = true;
				mechCollider.enabled = false;
				planeTiltRotation = Vector3.zero;
				planeTiltRotationVelocity = Vector3.zero;
			}
		} break;
		case TransformState.PLANE_TO_MECH: {
			if (transformCooldown <= 0.0F) {
				transformState = TransformState.MECH;
				rigidBody.useGravity = true;
				renderMeshFilter.mesh = mechMesh;
				planeCollider.enabled = false;
				mechCollider.enabled = true;
				playerModelObject.transform.localRotation = Quaternion.identity;
			}
		} break;
		}
		rocketCooldownTimer -= dt;
		swordCooldownTimer -= dt;
		transformCooldown -= dt;
		planeMissileCooldownTimer -= dt;
		planeBulletCooldownTimer -= dt;
		onGround = false;
	}

	public void TakeDamage(float damage) {
		health -= damage;
		healthRechargeCooldownTimer = healthRechargeCooldown;
	}
	public bool IsDead() {
		return health <= 0.0F;
	}
}
