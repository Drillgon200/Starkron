using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CreditScroll : MonoBehaviour
{

    public int speed;
    private bool start;
    public static bool staticStop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ScrollUp());
        StartCoroutine(EndCredits());
    }

    // Update is called once per frame
    void Update()
    {
        if (start == true)
        {

            GetComponent<Rigidbody2D>().linearVelocity = transform.up * speed;
        }

        if (Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(1))
        {
            SceneManager.LoadScene(0);
        }

        if (staticStop == true)
        {
            speed = 0;
        }
    }

    private IEnumerator ScrollUp()
    {
        yield return new WaitForSeconds(10);
        simpleStart();
    }

    private IEnumerator EndCredits()
    {
        yield return new WaitForSeconds(190);
        SceneManager.LoadScene(0);
    }

    private void simpleStart()
    {
        start = true;
    }
}
