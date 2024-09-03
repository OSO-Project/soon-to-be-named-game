using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    public static Score Instance;

    public int globalPlayerScore = 0; // Total score across the game
    public int roomScore = 0;
    public int upgradePoints = 0;
    public bool isEndlessMode = true;
    public int pointsPerUpgrade = 1000;

    //public int[] cleanThresholds = { 1000, 2000, 3000 }; // Dirty, Clean, Spick & Span

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
        GameEventManager.Instance.OnHoldToClean += AddScore;
    }

    public void AddScore(int points)
    {
        roomScore += points;
        globalPlayerScore += roomScore;
        UIManager.Instance.Score.text = globalPlayerScore.ToString();
    }

    public void SubtractScore(int points)
    {
        roomScore -= points;
        globalPlayerScore += roomScore;
        UIManager.Instance.Score.text = globalPlayerScore.ToString();
    }

    public void ConvertToUpgradePoints()
    {
        upgradePoints = globalPlayerScore / pointsPerUpgrade;
        globalPlayerScore = 0;
        //UIManager.Instance.UpdateUpgradePoints(upgradePoints);
    }

    // FIX: Calculate stars should be different for different levels.
    // Each level can have different star thresholds. For example instead of 1500 2500 4000, 
    // it could be 1000 2000 3000 for level 1, 2000 3000 4000 for level 2, etc.

    /*public int CalculateStars(int score)
    {
        if (score >= starThresholds[2])
            return 3;
        else if (score >= starThresholds[1])
            return 2;
        else if (score >= starThresholds[0])
            return 1;
        else
            return 0;
    }*/
    /*public bool CanLeaveRoom()
    {
        return roomScore >= cleanThresholds[1]; // Can leave after reaching the second threshold
    }*/

    public void ResetForNextRoom()
    {
        roomScore = 0;
        //UIManager.Instance.UpdateScore(playerScore);
        //UIManager.Instance.UpdateCleanlinessLevel(currentThresholdIndex);
    }

    /*public void EndLevel(GameData data)
    {
        if (isEndlessMode)
        {
            ConvertToUpgrade();
            data.AddUpgradePoints(upgradePoints);
            Debug.Log($"Score: {playerScore}, upg: {upgradePoints}");
        }
        else
        {
            *//*int stars = CalculateStars(playerScore);
            // Display total points and awarded stars
            Debug.Log($"Story Mode - Level {currentLevel + 1}: Total Points: {playerScore}, Stars Awarded: {stars}");

            // Handle level progression based on stars
            if (stars > 0)
            {
                // Proceed to next level or unlock content
                Debug.Log("Level Complete! Proceeding to next level...");
                currentLevel++;
                // Load next level or unlock new content
            }
            else
            {
                // Player failed to earn at least 1 star, require replay
                Debug.Log("Level Failed. Please try again!");
            }*//*
            playerScore = 0;
        }

        // Reset score for next level
        // IN EN
      
        

    }*/
}
