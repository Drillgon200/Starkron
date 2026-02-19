using UnityEngine;
using UnityEngine.SceneManagement;

public class UISceenFace : MonoBehaviour
{
    public GameObject pauseOverlay;
    public GameObject winScreen;
    public GameObject loseScreen;

    private void Start()
    {
        pauseOverlay.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

    }

    private void Update()
    {
        ExitPrompt();


        //MouseActive();


    }

    private void MouseActive()
    {
        //mouse needed for the UI menu buttons
        if (pauseOverlay.activeSelf == true || winScreen.activeSelf == true || loseScreen.activeSelf == true)
        {
            Cursor.visible = true;
        }
    }

    public void ExitPrompt()
    {
        if (Input.GetKeyDown(KeyCode.P)) // change "P' back to ESCAPE on shipping
        //if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseOverlay.activeSelf == false)
            {
                //overlay prompt to quit game
                pauseOverlay.SetActive(true);
            }
            else if (pauseOverlay.activeSelf == true)
            {
                pauseOverlay.SetActive(false);
            }
        }
    }

    public void OnClickQuit()
    {
        Application.Quit();

        print("game is quit");

    }

    public void ReloadLevelCurrent()
    {
        //get reffrence to current level
        //load that reffence

        SceneManager.LoadScene(1);//change to correct scene level
        print("REALOAD level");
    }

    public void ExitLevel()
    {
        //returns to main menu
        SceneManager.LoadScene(0);//change to correct scene level

    }

}
