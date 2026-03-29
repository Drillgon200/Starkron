using UnityEngine;

public class deletPicture : MonoBehaviour
{

    //private GameObject tempObject;
    public int time;

    void OnTriggerEnter(Collider other)
    {
        //tempObject = other.gameObject;
        if (other.gameObject.CompareTag("PictureTag"))
        {
            other.gameObject.GetComponent<UnityEngine.UI.Image>().enabled = false;
            //StartCoroutine(Reveal());
        }

        if (other.gameObject.CompareTag("StopCreditsTag"))
        {

            CreditScroll.staticStop = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        other.gameObject.GetComponent<UnityEngine.UI.Image>().enabled = true;
    }

    //private IEnumerator Reveal()
    //{
    //    yield return new WaitForSeconds(10);
    //    tempObject.gameObject.SetActive(true);

    //    //SceneManager.LoadScene(1);
    //}
}
