using System.Collections;
using UnityEngine;

public class CharAnimController : MonoBehaviour{	
	private Animator animator;
	public bool isPlaneMode;
	public GameObject swordMesh;
	public bool swordActive;
	public float swordTime;

	void Start() {
		animator = GetComponent<Animator>();
		swordMesh.SetActive(false);
		swordActive = false;
	}

	void Update() {
		if (!PlayerController.instance) {
			return;
		}
		// MECH mode animations =======================================
		if (!isPlaneMode) {
			bool playerRun = PlayerController.instance.IsSprinting();
			Vector2 moveVector = PlayerController.instance.GetMoveDirection();
			animator.SetBool("isRun", moveVector.sqrMagnitude > 0.0F && playerRun);
			animator.SetBool("isForwardMoving", moveVector.y > 0.0F && !playerRun);
			animator.SetBool("isBackwards", moveVector.y < 0.0F);
			animator.SetBool("isLeftMoving", moveVector.x < 0.0F);
			animator.SetBool("isRightMoving", moveVector.x > 0.0F);
		}

		// FLIGHT mode animations ================================
		if (isPlaneMode) {
			animator.SetBool("isFlightBoost", PlayerController.instance.IsBoosting());
			bool braking = PlayerController.instance.IsBraking();
			animator.SetBool("isBrakeBoth", braking);
			animator.SetBool("isFlightStop", braking && PlayerController.instance.rigidBody.linearVelocity.sqrMagnitude < 4.0F);
		}
	}


	public void SetIsPlaneMode(bool planeMode) {
		if (planeMode) {
			swordMesh.SetActive(false);
			animator.SetBool("isTrans", true);
			animator.SetBool("isMachineGun", false);
		} else {
			swordMesh.SetActive(false);
			animator.SetBool("isTrans", false);
			animator.SetBool("isFallingOkay", true);
			animator.SetBool("isMachineGunPlane", false);
		}
		isPlaneMode = planeMode;
	}

	public void SetIsMachineGun(bool isMachineGun) {
		animator.SetBool("isMachineGun", !isPlaneMode && isMachineGun);
		animator.SetBool("isMachineGunPlane", isPlaneMode && isMachineGun);
	}

	public void SetJetpackActive(bool active) {
		animator.SetBool("isJump", active);
	}

	public void SetGrounded(bool grounded) {
		animator.SetBool("isGrounded", grounded);
		animator.SetBool("isFallingOkay", !grounded);
	}

	public void SetFallingDamaged() {
		animator.SetBool("isRecover", false);
		animator.SetBool("isFallingOkay", false);
	}

	public void TakeDamage() {
		animator.SetTrigger("isDamaged");
	}

	public void FallRecover() {
		animator.SetBool("isFallingOkay", true);
		animator.SetBool("isRecover", true);
	}

	public void ActivateSword() {
		swordMesh.SetActive(true);
		StartCoroutine(SwordActionTimer());
	}

	IEnumerator SwordActionTimer() {
		if (!swordActive && !isPlaneMode) {
			swordMesh.SetActive(true);
			animator.SetBool("isSwordAttack", true);
			yield return new WaitForSeconds(swordTime);
			animator.SetBool("isSwordAttack", false);
			swordActive = false;
			yield return new WaitForSeconds(swordTime + 3);
			swordMesh.SetActive(false);
		}
	}
}
