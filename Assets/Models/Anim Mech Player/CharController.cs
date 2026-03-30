using System.Collections;
using UnityEditor;
using UnityEngine;

public class CharController : MonoBehaviour{	
	private Animator animator;
	public bool isPlaneMode;
	public GameObject swordMesh;
	public bool swordActive;
	public float swordTime;
	public bool playerDown;
	public bool playerRun;
	public bool isGroundTouch;
	public bool isFallingBad;
	public bool boostFlight;

	void Start() {
		animator = GetComponent<Animator>();
		swordMesh.SetActive(false);
		swordActive = false;
		playerDown = false;
		playerRun = false;
		isGroundTouch = true;
		isFallingBad = false;
		boostFlight = false;
	}

	void Update() {
		//MECH mode animations =======================================
		if (!isPlaneMode) {

		if (Input.GetKey(KeyCode.LeftShift) && playerDown == false)
		{
			playerRun = true;
		} else {
			playerRun = false;
		}

		if (Input.GetKey(KeyCode.W) && playerDown == false && playerRun == false)
		{
			animator.SetBool("isForwardMoving", true);
		} else {
			animator.SetBool("isForwardMoving", false);
		}

		if (Input.GetKey(KeyCode.W) && playerDown == false && playerRun == true)
		{
			animator.SetBool("isRun", true);
		}else {
			animator.SetBool("isRun", false);
		}

		if (Input.GetKey(KeyCode.S) && playerDown == false)
		{
			animator.SetBool("isBackwards", true);
		}else {
			animator.SetBool("isBackwards", false);
		}

		if (Input.GetKey(KeyCode.A) && playerDown == false)
		{
			animator.SetBool("isLeftMoving", true);
		} else {
			animator.SetBool("isLeftMoving", false);
		}

		if (Input.GetKey(KeyCode.D) && playerDown == false)
		{
			animator.SetBool("isRightMoving", true);
		} else {
			animator.SetBool("isRightMoving", false);
		}

		if (Input.GetKeyDown(KeyCode.F) 
			&& isPlaneMode == false 
			&& playerDown == false
			&& isGroundTouch == true) {
			swordActive = false;
		}   

		if (Input.GetKey(KeyCode.Mouse0) && playerDown == false)
		{
			animator.SetBool("isMachineGun", true);
		} else {
			animator.SetBool("isMachineGun", false);
		}

		if (Input.GetKey(KeyCode.Space) && playerDown == false)
		{
			animator.SetBool("isJump", true);
			isGroundTouch = false;
		} else {
			animator.SetBool("isJump", false);
		}

		if (Input.GetKeyDown(KeyCode.Keypad1) && playerDown == false && isGroundTouch == true)
		{
			animator.SetBool("isDamaged", true);
		} else if (Input.GetKeyDown(KeyCode.Keypad1) && playerDown == false && isGroundTouch == false) {
			animator.SetBool("isDamaged", true);
			animator.SetBool("isFallingOkay", false);
			isFallingBad = true;
		} else 
		{           
			animator.SetBool("isDamaged", false);
		}
			if (Input.GetKeyDown(KeyCode.Keypad3) && playerDown == false && isGroundTouch == true) {
				isGroundTouch = false;
			} else if (Input.GetKeyDown(KeyCode.Keypad3) && playerDown == false && isGroundTouch == false && isFallingBad == false) {
				isGroundTouch = true;
			} else if (Input.GetKeyDown(KeyCode.Keypad3) && playerDown == false && isGroundTouch == false && isFallingBad == true) {
				animator.SetBool("isFallingOkay", true);
				isGroundTouch = true;
				playerDown = true;
				isFallingBad = false;
			}

			if (Input.GetKey(KeyCode.Keypad4) && playerDown == true) {
				animator.SetBool("isRecover", true);
				playerDown = false;
			} else {
				animator.SetBool("isRecover", false);
			}
		}

		//FLIGHT mode animations ================================
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

		if (Input.GetKeyDown(KeyCode.Q) && playerDown == false) {
			if (isPlaneMode == false) {
				swordMesh.SetActive(false);
				animator.SetBool("isTrans", true);
				isPlaneMode = true;
				animator.SetBool("isMachineGun", false);

			} else if (isPlaneMode == true) {
				isGroundTouch = false;
				swordMesh.SetActive(false);
				animator.SetBool("isTrans", false);
				animator.SetBool("isFallingOkay", true);
				animator.SetBool("isMachineGunPlane", false);
				isPlaneMode = false;
			}
		}
	}

	public void SetGrounded(bool grounded) {
		animator.SetBool("isGrounded", grounded);
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
