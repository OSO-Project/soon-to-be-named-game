using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    public float StartTimeInSeconds = 300f; // 5 minutes in seconds
    private float _remainingTime;
    private bool _isRunning = true;

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


    void Start()
    {
        _remainingTime = StartTimeInSeconds;
        UpdateTimerText();
    }

    void Update()
    {
        if (_isRunning)
        {
            if (_remainingTime > 0)
            {
                _remainingTime -= Time.deltaTime;
                if (_remainingTime < 0)
                {
                    _remainingTime = 0;
                    _isRunning = false;
                    GameEventManager.Instance.EndLevel();
                }
                UpdateTimerText();
            }
        }
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(_remainingTime / 60);
        int seconds = Mathf.FloorToInt(_remainingTime % 60);
        UIManager.Instance.TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void PauseTimer()
    {
        _isRunning = false;
    }

    public void ResumeTimer()
    {
        _isRunning = true;
    }

    public void RestartTimer()
    {
        _remainingTime = StartTimeInSeconds;
        _isRunning = true;
        UpdateTimerText();
    }

    public void AddTime(float time)
    {
        _remainingTime += time;
        UpdateTimerText();
    }
}
