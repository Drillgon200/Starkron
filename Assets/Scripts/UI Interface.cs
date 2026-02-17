using UnityEngine;
using UnityEngine.SceneManagement;


public class UIInterface : MonoBehaviour
{
    public GameObject escapeOverlay;

    void Start()
    {
        escapeOverlay.SetActive(false);
    }


    void Update()
    {
        EscapePrompt();
    }

    public void EscapePrompt()
    {
        if (Input.GetKeyDown(KeyCode.P) //change from P to ESCAPE on ship
                                        //Input.GetKeyDown(KeyCode.Escape)
            )
        {
            if (escapeOverlay.activeSelf == false)
            {
                //overlay prompt to quit game
                escapeOverlay.SetActive(true);
            }
            else if (escapeOverlay.activeSelf == true)
            {
                escapeOverlay.SetActive(false);
            }
        }
    }

    public void OnClickExit()
    {
        Application.Quit();

        print("game is quit");

    }

    public void ReloadCurrentLevel()
    {
        print("not yet set"); // reffrence to current level and load that level number
    }

    public void MainMenuLoad()
    {
        SceneManager.LoadScene(0);//change to correct scene level

        print("to main menu");
    }

}
