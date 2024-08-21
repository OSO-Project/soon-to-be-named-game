using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int playerScore = 0;
    public int upgradePoints = 0;
    public bool isEndlessMode = false;
    public int pointsPerUpgrade;
    public int currentLevel = 0; // scriptable obj?

    public int[] starThresholds = { 1500, 2500, 4000 };

    private void Start()
    {
        GameEventManager.Instance.OnAddScore += AddScore;
        GameEventManager.Instance.OnLevelEnd += EndLevel;
    }

    public void AddScore(int points)
    {
        playerScore += points;
        UIManager.Instance.Score.text = playerScore.ToString();
    }

    public void SubtractScore(int points)
    {
        playerScore -= points;
        UIManager.Instance.Score.text = playerScore.ToString();
    }

    public void ConvertToUpgrade()
    {
        if (isEndlessMode)
        {
            upgradePoints = playerScore / pointsPerUpgrade;
        }
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

    public void EndLevel(GameData data)
    {
        if (isEndlessMode)
        {
            ConvertToUpgrade();
            data.AddUpgradePoints(upgradePoints);
            Debug.Log($"Score: {playerScore}, upg: {upgradePoints}");
        }
        else
        {
            /*int stars = CalculateStars(playerScore);
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
            }*/
            playerScore = 0;
        }

        // Reset score for next level
        // IN EN
      
        

    }
}
