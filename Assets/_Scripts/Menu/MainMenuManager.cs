using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    private GameObject optionsMenuPanel;
    private GameObject mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        //find object called OptionsMenuPanel and set it to false
        mainMenu = GameObject.Find("MainMenu");
        optionsMenuPanel = GameObject.Find("OptionsMenuPanel");
        optionsMenuPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LastMerge 24.07.2024");
    }

    public void OpenSettings()
    {
        mainMenu.SetActive(false);
        optionsMenuPanel.SetActive(true);

    }

    public void CloseSettings()
    {
        mainMenu.SetActive(true);
        optionsMenuPanel.SetActive(false);

    }

    public void OpenCredits()
    {

    }

    public void CloseCredits()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
