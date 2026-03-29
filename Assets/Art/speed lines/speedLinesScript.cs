using UnityEngine;
using System.Collections;
using UnityEditor.Recorder;


public class speedLinesScript : MonoBehaviour
{
    public CharController charController;
    public float delayTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKey(KeyCode.LeftShift)&& charController.playerDown == false) {
           
                if (charController.isPlaneMode == false && Input.GetKey(KeyCode.W)) {

                    GetComponent<UnityEngine.UI.Image>().enabled = true;

                }
                else if (charController.isPlaneMode == true) {

                    GetComponent<UnityEngine.UI.Image>().enabled = true;
                }
                else {

                    GetComponent<UnityEngine.UI.Image>().enabled = false;
                }

                //StartCoroutine(SpeedLineDelay()); 
        }
        else {

            GetComponent<UnityEngine.UI.Image>().enabled = false;
        }





        //if (charController.isPlaneMode == false) {

        //    if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && charController.playerDown == false) {
        //        charController.playerRun = true;

        //        StartCoroutine(SpeedLineDelay());

        //    }

        //}
        //else {

        //    GetComponent<UnityEngine.UI.Image>().enabled = false;
        //}

        //if (charController.isPlaneMode == true) {

        //    if (Input.GetKey(KeyCode.LeftShift) && charController.playerDown == false) {

        //        charController.playerRun = true;

        //        StartCoroutine(SpeedLineDelay());

        //    }
        //}
        //else {

        //    GetComponent<UnityEngine.UI.Image>().enabled = false;
        //}
    }

    //IEnumerator SpeedLineDelay() {

    //    yield return new WaitForSeconds(delayTime);

    //    if ((Input.GetKey(KeyCode.LeftShift)) && charController.playerDown == false) {


    //        if (charController.isPlaneMode == false && Input.GetKey(KeyCode.W)) {

    //            GetComponent<UnityEngine.UI.Image>().enabled = true;

    //        }
    //        else if (charController.isPlaneMode == true) {

    //            GetComponent<UnityEngine.UI.Image>().enabled = true;
    //        }
    //    }      
    //}

}
