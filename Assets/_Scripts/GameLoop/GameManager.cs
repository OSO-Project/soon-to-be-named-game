using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isEndlessMode = false;
    private bool isLevelActive = true;
    public EndlessModeManager emm;

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
    }

    private void Start()
    {
        GameEventManager.Instance.OnLevelEnd += EndLevel;

        isLevelActive = true;
        Score.Instance.ResetForNextRoom();
        GameTimer.Instance.RestartTimer();

        emm = GameObject.Find("EndlessModeManager").GetComponent<EndlessModeManager>();
    }

    public bool AttemptToLeaveRoom()
    {
        Debug.Log(emm.cleanliness >= 0.6f);
        return emm.cleanliness >= 0.6f;
    }

    public void ProceedToNextRoom()
    {
        // Logic to transition to the next room
        Score.Instance.ResetForNextRoom(); // Reset score for the new room
        //GameTimer.Instance.RestartTimer(); // Reset timer for the new room

        Debug.Log("next room");
    }

    public void EndLevel()
    {
        isLevelActive = false;
        if (isEndlessMode)
        {
            Score.Instance.ConvertToUpgradePoints();
        }
        else
        {
            // Show level complete screen and handle story mode logic
            //UIManager.Instance.ShowLevelComplete();
            // Load next room or handle story mode progression logic here
        }
    }
}
