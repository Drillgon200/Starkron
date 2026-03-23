using System.Collections;
using UnityEngine;

public class CharController : MonoBehaviour
{

    //public float speed;
    //public float rotationSpeed;
    //public float jumpSpeed;
    //public float jumpButtonGracePeriod;

    private Animator animator;
    //private CharacterController characterController;
    //private float ySpeed;
    //private float originalStepOffset;
    //private float? lastGroundedTime;
    //private float? jumpButtonPressedTime;

    private bool isPlaneMode;
    public GameObject swordMesh;
    private bool swordActive;
    public float swordTime;
    private bool playerDown;
    private bool playerRun;
    private bool isGroundTouch;
    private bool isFallingBad;
    private bool boostFlight;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        swordMesh.SetActive(false);
        swordActive = false;
        playerDown = false;
        playerRun = false;
        isGroundTouch = true;
        isFallingBad = false;
        boostFlight = false;

        //characterController = GetComponent<CharacterController>();
        //originalStepOffset = characterController.stepOffset;
    }

    // Update is called once per frame
    void Update()
    {
        //MECH mode animations =======================================
        if (isPlaneMode == false) {

        if (Input.GetKey(KeyCode.LeftShift) && playerDown == false)
        {
            print("RUN");
            playerRun = true;
        }
        else
        {
            playerRun = false;
        }

        if (Input.GetKey(KeyCode.W) && playerDown == false && playerRun == false)
        {
            print("forward");
            animator.SetBool("isForwardMoving", true);
        }
        else
        {
            animator.SetBool("isForwardMoving", false);
        }

        if (Input.GetKey(KeyCode.W) && playerDown == false && playerRun == true)
        {
            print("run fowrward");
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }

        if (Input.GetKey(KeyCode.S) && playerDown == false)
        {
            print("BACKWARD");
            animator.SetBool("isBackwards", true);
        }
        else
        {
            animator.SetBool("isBackwards", false);
        }

        if (Input.GetKey(KeyCode.A) && playerDown == false)
        {
            print("straf LEFT");
            animator.SetBool("isLeftMoving", true);
        }
        else
        {
            animator.SetBool("isLeftMoving", false);
        }

        if (Input.GetKey(KeyCode.D) && playerDown == false)
        {
            print("straf RIGHT");
            animator.SetBool("isRightMoving", true);
        }
        else
        {
            animator.SetBool("isRightMoving", false);
        }

        if (Input.GetKeyDown(KeyCode.F) 
            && isPlaneMode == false 
            && playerDown == false
            && isGroundTouch == true)
        {
            //SWORD
            swordActive = false;
            StartCoroutine(CoroutineTimer());
        }   

        if (Input.GetKey(KeyCode.Mouse0) && playerDown == false)
        {
            print("machine gun");
            animator.SetBool("isMachineGun", true);
        }
        else
        {
            animator.SetBool("isMachineGun", false);
        }

        if (Input.GetKey(KeyCode.Space) && playerDown == false)
        {
            print("player jump good"); //jump test
            animator.SetBool("isJump", true);
            animator.SetBool("isGrounded", false);
            isGroundTouch = false;
        }
        else
        {
            animator.SetBool("isJump", false);
        }

        if (Input.GetKeyDown(KeyCode.Keypad1) && playerDown == false && isGroundTouch == true)
        {
            print("Damage on ground");
            animator.SetBool("isDamaged", true);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1) && playerDown == false && isGroundTouch == false) {
            print("Damage bad falling");
            animator.SetBool("isDamaged", true);
            animator.SetBool("isFallingOkay", false);
            isFallingBad = true;
            //playerDown = true;
        }
        else 
        {           
            animator.SetBool("isDamaged", false);
        }

            if (Input.GetKeyDown(KeyCode.Keypad3) && playerDown == false && isGroundTouch == true) {
                print("Toggle: NOT touching ground");
                animator.SetBool("isGrounded", false);
                isGroundTouch = false;


            }
            else if (Input.GetKeyDown(KeyCode.Keypad3) && playerDown == false && isGroundTouch == false && isFallingBad == false) {
                print("Toggle: YES touching ground");
                animator.SetBool("isGrounded", true);
                isGroundTouch = true;
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3) && playerDown == false && isGroundTouch == false && isFallingBad == true) {
                print("Toggle: YES HURT touching ground");
                animator.SetBool("isGrounded", true);
                animator.SetBool("isFallingOkay", true);

                isGroundTouch = true;
                playerDown = true;
                isFallingBad = false;
            }

            if (Input.GetKey(KeyCode.Keypad4) && playerDown == true) {
                print("get up recovery");
                animator.SetBool("isRecover", true);
                playerDown = false;
            }
            else {
                animator.SetBool("isRecover", false);
            }

        }

        //FLIGHT mode animations ================================
        if (isPlaneMode == true) {

            if (Input.GetKey(KeyCode.LeftShift)) {
                boostFlight = true;
            }
            else {             
                boostFlight = false;
            }

            if (boostFlight == true) {
                print("flight boost");
                animator.SetBool("isFlightBoost", true);
            }
            else {
                animator.SetBool("isFlightBoost", false);
            }

            if (Input.GetKey(KeyCode.Space)) {
                print("airbrakes BOTH");
                animator.SetBool("isBrakeBoth", true);

                StartCoroutine(CoroutineTimer());
            }
            else {
                animator.SetBool("isBrakeBoth", false);
                animator.SetBool("isFlightStop", false);
            }

            if (Input.GetKey(KeyCode.Keypad1)) {
                print("flight damage");
                animator.SetBool("isDamaged", true);
            }
            else {
                animator.SetBool("isDamaged", false);
            }

            if (Input.GetKey(KeyCode.Mouse0) && playerDown == false) {
                print("machine gun");
                animator.SetBool("isMachineGun", true);
            }
            else {
                animator.SetBool("isMachineGun", false);
            }

        }

        if (Input.GetKeyDown(KeyCode.Q) && playerDown == false) {
            if (isPlaneMode == false) {
                swordMesh.SetActive(false);
                print("Transform > PLANE MODE");
                animator.SetBool("isTrans", true);
                animator.SetBool("isGrounded", false);
                isPlaneMode = true;

            }
            else if (isPlaneMode == true) {
                isGroundTouch = false;
                swordMesh.SetActive(false);
                print("Transform > ROBOT MODE");
                animator.SetBool("isTrans", false);
                animator.SetBool("isFallingOkay", true);
                
                isPlaneMode = false;
            }
        }

        //============================================

        //if (Input.GetKey(KeyCode.Keypad4) && playerDown == false) {
        //    print("HURT player fall");
        //    animator.SetBool("isFallingOkay", false);

        //    playerDown = true;
        //}

        //if (Input.GetKey(KeyCode.Keypad4) && playerDown == false)
        //{
        //    print("HURT player fall");// hurt test
        //    animator.SetBool("isJump", true);
        //    animator.SetBool("isFallingOkay", false);

        //    playerDown = true;
        //}
        //else
        //{
        //    animator.SetBool("isJump", false);
        //}

    }



    IEnumerator CoroutineTimer()
    {

        if (isPlaneMode == false) {

            if (swordActive == false) {
                print("Sword");
                swordMesh.SetActive(true);
                //animator.SetBool("isSwordAttack", false);
                animator.SetBool("isSwordAttack", true);

                yield return new WaitForSeconds(swordTime);

                print("Sword Off");
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



    //    float horizontalInput = Input.GetAxis("Horizontal");
    //    float verticalInput = Input.GetAxis("Vertical");

    //    Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
    //    float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
    //    movementDirection.Normalize();

    //    ySpeed += Physics.gravity.y * Time.deltaTime;

    //    if (characterController.isGrounded) {
    //        lastGroundedTime = Time.time;
    //    }

    //    if (Input.GetButtonDown("Jump")) {
    //        jumpButtonPressedTime = Time.time;
    //    }

    //    if (Time.time - lastGroundedTime <= jumpButtonGracePeriod) {
    //        characterController.stepOffset = originalStepOffset;
    //        ySpeed = -0.5f;

    //        if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod) {
    //            ySpeed = jumpSpeed;
    //            jumpButtonPressedTime = null;
    //            lastGroundedTime = null;
    //        }
    //    }
    //    else {
    //        characterController.stepOffset = 0;
    //    }

    //    Vector3 velocity = movementDirection * magnitude;
    //    velocity.y = ySpeed;

    //    characterController.Move(velocity * Time.deltaTime);

    //    if (movementDirection != Vector3.zero) {
    //        animator.SetBool("IsMoving", true);
    //        Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

    //        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    //    }
    //    else {
    //        animator.SetBool("IsMoving", false);
    //    }
    //}
}
