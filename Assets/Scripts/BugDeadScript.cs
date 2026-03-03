using UnityEngine;

public class BugDeadScript : MonoBehaviour {

    public GameObject pieceHead;
    public GameObject pieceMan1;
    public GameObject pieceMan2;
    public GameObject pieceLeg1;
    public GameObject pieceLeg2;
    public GameObject pieceLeg3;
    public GameObject pieceLeg4;
    public GameObject pieceBody;

    private void Awake() {
        Invoke("TimedMesh", 0.1f);
    }

    private void TimedMesh() {
        pieceHead.GetComponent<CapsuleCollider>().enabled = false;
        pieceMan1.GetComponent<CapsuleCollider>().enabled = false;
        pieceMan2.GetComponent<CapsuleCollider>().enabled = false;
        pieceLeg1.GetComponent<CapsuleCollider>().enabled = false;
        pieceLeg2.GetComponent<CapsuleCollider>().enabled = false;
        pieceLeg3.GetComponent<CapsuleCollider>().enabled = false;
        pieceLeg4.GetComponent<CapsuleCollider>().enabled = false;
        pieceBody.GetComponent<CapsuleCollider>().enabled = false;

        Invoke("timedDestroy", 10);
    }

    private void timedDestroy() {
        Destroy(this.gameObject);
    }
}
