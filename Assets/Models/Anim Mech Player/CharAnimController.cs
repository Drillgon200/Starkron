using System.Collections;
using UnityEditor;
using UnityEngine;

public class CharAnimController : MonoBehaviour{	
	private Animator animator;
	public bool isPlaneMode;
	public GameObject swordMesh;
	public bool swordActive;
	public float swordTime;
	public bool playerDown;
	public bool isGroundTouch;
	public bool isFallingBad;
	public bool boostFlight;

	void Start() {
		animator = GetComponent<Animator>();
		swordMesh.SetActive(false);
		swordActive = false;
		playerDown = false;
		isGroundTouch = true;
		isFallingBad = false;
		boostFlight = false;
	}

	void Update() {
		// MECH mode animations =======================================
		if (!isPlaneMode) {
			bool playerRun = PlayerController.instance.IsSprinting();
			Vector2 moveVector = PlayerController.instance.GetMoveDirection();
			animator.SetBool("isRun", moveVector.sqrMagnitude > 0.0F && playerRun);
			animator.SetBool("isForwardMoving", moveVector.y > 0.0F && !playerRun);
			animator.SetBool("isBackwards", moveVector.y < 0.0F);
			animator.SetBool("isLeftMoving", moveVector.x < 0.0F);
			animator.SetBool("isRightMoving", moveVector.x > 0.0F);

			if (Input.GetKeyDown(KeyCode.F) 
				&& isPlaneMode == false 
				&& playerDown == false
				&& isGroundTouch == true) {
				swordActive = false;
			}   

			if (Input.GetKey(KeyCode.Mouse0) && playerDown == false) {
				animator.SetBool("isMachineGun", true);
			} else {
				animator.SetBool("isMachineGun", false);
			}

			if (Input.GetKey(KeyCode.Space) && playerDown == false)	{
				animator.SetBool("isJump", true);
				isGroundTouch = false;
			} else {
				animator.SetBool("isJump", false);
			}

			if (Input.GetKeyDown(KeyCode.Keypad1) && playerDown == false && isGroundTouch == true) {
				animator.SetBool("isDamaged", true);
			} else if (Input.GetKeyDown(KeyCode.Keypad1) && playerDown == false && isGroundTouch == false) {
				animator.SetBool("isDamaged", true);
				animator.SetBool("isFallingOkay", false);
				isFallingBad = true;
			} else {           
				animator.SetBool("isDamaged", false);
			}
			if (Input.GetKeyDown(KeyCode.Keypad3) && playerDown == false && isGroundTouch == true) {
				isGroundTouch = false;
			} else if (Input.GetKeyDown(KeyCode.Keypad3) && playerDown == false && isGroundTouch == false && isFallingBad == false) {
				isGroundTouch = true;
			} else if (Input.GetKeyDown(KeyCode.Keypad3) && playerDown == false && isGroundTouch == false &&
				isFallingBad == true) {
				animator.SetBool("isFallingOkay", true);
				isGroundTouch = true;
				playerDown = true;
				isFallingBad = false;
			}

			animator.SetBool("isRecover", false);
		}

		// FLIGHT mode animations ================================

		if (isPlaneMode) {
			if (Input.GetKey(KeyCode.LeftShift)) {
				boostFlight = true;
			} else {
				boostFlight = false;
			}

			if (boostFlight) {
				animator.SetBool("isFlightBoost", true);
			} else {
				animator.SetBool("isFlightBoost", false);
			}

			if (Input.GetKey(KeyCode.Space)) {
				animator.SetBool("isBrakeBoth", true);
				StartCoroutine(CoroutineTimer());
			} else {
				animator.SetBool("isBrakeBoth", false);
				animator.SetBool("isFlightStop", false);
			}

			if (Input.GetKey(KeyCode.Keypad1)) {
				animator.SetBool("isDamaged", true);
			} else {
				animator.SetBool("isDamaged", false);
			}

			if (Input.GetKey(KeyCode.Mouse0) && playerDown == false) {
				animator.SetBool("isMachineGunPlane", true);			   
			} else {
				animator.SetBool("isMachineGunPlane", false);
			}
		}

	}

	public void SetIsPlaneMode(bool planeMode) {
		if (planeMode) {
			swordMesh.SetActive(false);
			animator.SetBool("isTrans", true);
			animator.SetBool("isMachineGun", false);
		} else {
			isGroundTouch = false;
			swordMesh.SetActive(false);
			animator.SetBool("isTrans", false);
			animator.SetBool("isFallingOkay", true);
			animator.SetBool("isMachineGunPlane", false);
		}
		isPlaneMode = planeMode;
	}

	public void SetGrounded(bool grounded) {
		animator.SetBool("isGrounded", grounded);
	}

	public void FallRecover() {
		animator.SetBool("isRecover", true);
	}

	public void ActivateSword() {
		swordMesh.SetActive(true);
		StartCoroutine(CoroutineTimer());
	}

	IEnumerator CoroutineTimer() {
		if (isPlaneMode == false) {
			if (swordActive == false) {
				swordMesh.SetActive(true);
				animator.SetBool("isSwordAttack", true);
				yield return new WaitForSeconds(swordTime);
				animator.SetBool("isSwordAttack", false);
				swordActive = false;
				yield return new WaitForSeconds(swordTime + 3);
				swordMesh.SetActive(false);
			}
		}

		if (isPlaneMode == true) {
			if (Input.GetKey(KeyCode.Space)) {
				yield return new WaitForSeconds(2);
				animator.SetBool("isFlightStop", true);
			}
		}
	}
}
