using UnityEngine;

public class DomeScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision other) {

        if (!(other.gameObject.CompareTag("PlayerTag")
            || other.gameObject.CompareTag("DomeTag")
            ))
            //(other.gameObject.CompareTag("MachineGunBulletTag") || 
            //other.gameObject.CompareTag("GroundBugTag") ||
            //other.gameObject.CompareTag("missileTag")) 

            {

            print("break");

        GameObject otherObj = other.gameObject;

        Rigidbody cubeRigidbody = this.GetComponent<Rigidbody>();
        cubeRigidbody.isKinematic = false;

        }

        

    }

}
