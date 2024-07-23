using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIManager : MonoBehaviour
{
    public static PauseUIManager Instance;
    [SerializeField] private GameObject _gamePauseMenu;
    [SerializeField] private GameObject _playerCrosshair;
    public bool IsGamePaused { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        Unpause();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            IsGamePaused = !IsGamePaused;
        }
        if (IsGamePaused)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }

    private void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        _playerCrosshair.SetActive(false);
        _gamePauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    private void Unpause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _gamePauseMenu.SetActive(false);
        Time.timeScale = 1;
        _playerCrosshair.SetActive(true);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
