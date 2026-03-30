using UnityEngine;

public class DomeScript : MonoBehaviour
{
    void OnCollisionEnter(Collision other) {
        if (!(other.gameObject.CompareTag("PlayerTag")
            || other.gameObject.CompareTag("DomeTag") )) {
        GameObject otherObj = other.gameObject;
        Rigidbody cubeRigidbody = this.GetComponent<Rigidbody>();
        cubeRigidbody.isKinematic = false;
        }
    }
}
