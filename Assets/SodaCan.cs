using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SodaCan : MonoBehaviour
{
    /*Player can drink soda cans in different colours. Upon interacting with the can, the player gets either a positive or a negative effect. There is 70% chance for a positive and 30% chance for a negative effect.

    The colour of the can (different textures) do not change how the can works. All types of cans have the same influence.

    Possible outcomes of the soda drinking are as follows:
    Positive soda drinking outcomes (70% chance):
    Add 20 seconds to timer
    Movement speed & hold to clean boost for 20 seconds
    Highlight all dirty items for 20 seconds

    Negative soda drinking outcomes (30% chance):
    Invert key binds for 30 seconds
    Slow player movement speed and hold to clean for 20 seconds
    Dizzy screen for 30 seconds

     */

    public HighlightObject _highlight;
    public static SodaCan currentTarget = null;
    public void OnBeginLooking()
    {
        
        _highlight.SetIsHighlighted(true);
        currentTarget = this;
        UIManager.Instance.HintText.gameObject.SetActive(true);
        _highlight.Highlight();
    }

    public void OnFinishLooking()
    {
        _highlight.SetIsHighlighted(false);
        currentTarget = null;
        UIManager.Instance.HintText.gameObject.SetActive(false);
        //StopAndResetProgress();
        _highlight.Highlight();
    }
    public void DrinkSoda()
    {

        float random = Random.Range(0f, 1f);
        if (random <= 0.7f)
        {
            ApplyPositiveEffect();
        }
        else
        {
            ApplyNegativeEffect();
        }
    }


    public void ShowDrinkIcon()
    {
        //UIManager.Instance.DrinkIcon.gameObject.SetActive(true);
    }
    public void HideDrinkIcon()
    {
       // UIManager.Instance.DrinkIcon.gameObject.SetActive(false);
    }

    private void ApplyPositiveEffect()
    {
        Debug.Log("Positive effect");

        //33% chance for each effect
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0:
                GameTimer.Instance.AddTime(20);
                break;
            case 1:
              //  PlayerMovement.Instance.BoostSpeed(20);
              //  HoldToClean.Instance.BoostSpeed(20);
                break;
            case 2:
                //HIGHLIGHT ALL DIRTY ITEMS FOR 20 SECONDS
                break;
        }

    }

    private void ApplyNegativeEffect()
    {

        Debug.Log("Negative effect");

        //33% chance for each effect
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0:
                //INVERT KEY BINDS FOR 30 SECONDS
                break;
            case 1:
                //PlayerMovement.Instance.SlowSpeed(20);
               // HoldToClean.Instance.SlowSpeed(20);
                break;
            case 2:
                //DIZZY SCREEN FOR 30 SECONDS
                break;
        }

        
    }

}
