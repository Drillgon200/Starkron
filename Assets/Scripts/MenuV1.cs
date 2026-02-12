using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuV1 : MonoBehaviour
{

    public GameObject mainMenuScene;
    public GameObject OptionsScene;
    public GameObject levelSelectScene;
    public GameObject exitOverlayGraphic;

    private void Start()
    {
        mainMenuScene.SetActive(true);
        OptionsScene.SetActive(false);
        levelSelectScene.SetActive(false);
        exitOverlayGraphic.SetActive(false);
    }

    private void Update()
    {
        ExitPrompt();

    }

    public void ExitPrompt()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (exitOverlayGraphic.activeSelf == false && mainMenuScene.activeSelf == false)
            {
                //overlay prompt to quit game
                exitOverlayGraphic.SetActive(true);
            }
            else if (exitOverlayGraphic.activeSelf == true)
            {
                exitOverlayGraphic.SetActive(false);
            }
        }
    }

    public void OptionsSelect()
    {
        mainMenuScene.SetActive(false);
        OptionsScene.SetActive(true);
        levelSelectScene.SetActive(false);
    }

    public void LevelSelect()
    {
        mainMenuScene.SetActive(false);
        OptionsScene.SetActive(false);
        levelSelectScene.SetActive(true);
    }

    public void MainRootSelect()
    {
        mainMenuScene.SetActive(true);
        OptionsScene.SetActive(false);
        levelSelectScene.SetActive(false);
    }

    public void OnClickExit()
    {
        Application.Quit();

        print("game is quit");

    }

    public void LoadLevelOne()
    {
        SceneManager.LoadScene(1);//change to correct scene level

        print("Load level 1");
    }




}
