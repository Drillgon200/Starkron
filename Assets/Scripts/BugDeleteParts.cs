using UnityEngine;

public class BugDeleteParts : MonoBehaviour {

    // Update is called once per frame

    private void Update() {
        Invoke("deleteMat", 3);
    }


    private void deleteMat() {
        Destroy(this.gameObject);
    }

    //OnCollisionEnter(Collision collision) {
    //    Destroy(this.gameObject);
    //}
}
