using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SodaCan : MonoBehaviour, Interactable
{

    // public HighlightObject _highlight;
    public static SodaCan currentTarget = null;

    [Header("Positive Effects")]
    [SerializeField] private float _boostBuffMultiplier;
    [SerializeField] private float _boostHoldToCleanMultiplier;
    [SerializeField] private float _speedBoostDurationInSec;
    [SerializeField] private float _timeToAdd;
    [SerializeField] private float _highlightDurationInSec;

    [Header("Negative Effects")]
    [SerializeField] private float _slowDebuffMultiplier;
    [SerializeField] private float _slowHoldToCleanMultiplier;
    [SerializeField] private float _slowDurationInSec;
    [SerializeField] private float _dizzyDurationInSec;
    [SerializeField] private float _invertKeyBindsDurationInSec;

    public void Start()
    {
        InputManager.Instance.InteractAction.performed += OnPressInteract;
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
        //Object.Destroy(gameObject);
    }

    private void ApplyPositiveEffect()
    {
        Debug.Log("Positive effect applied.");
       
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0:
                GameTimer.Instance.AddTime(_timeToAdd);
                break;
            case 1:
                PlayerController.Instance.BoostSpeed(_speedBoostDurationInSec, _boostBuffMultiplier);
                CleanItem.currentTarget.BoostHoldToCleanSpeed(_speedBoostDurationInSec, _boostHoldToCleanMultiplier);
                break;
            //case 2:
            //    //HIGHLIGHT ALL DIRTY ITEMS FOR 20 SECONDS
            //    Debug.Log("Highlight all dirty items for 20 seconds");
            //    break;

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
                //INVERT KEY BINDS FOR DURATION
                StartCoroutine(InputManager.Instance.InvertKeybindsForDuration(_invertKeyBindsDurationInSec));
                break;
            case 1:
                //SLOW PLAYER SPEED FOR DURATION
                PlayerController.Instance.SlowSpeed(_slowDurationInSec, _slowDebuffMultiplier);
                CleanItem.currentTarget.SlowHoldToCleanSpeed(_slowDurationInSec, _slowHoldToCleanMultiplier);
                break;
            case 2:
                //APPLY POSTFX DIZZY EFFECT FOR DURATION
                StartCoroutine(PostFXManager.Instance.ApplyDizzyEffect(_dizzyDurationInSec));
                break;
        }


    }

    public void OnBeginLooking()
    {
        // _highlight.SetIsHighlighted(true);
        Debug.Log("Looking at soda can");
        currentTarget = this;
        ShowDrinkIcon();
        // _highlight.Highlight();
    }

    public void OnFinishLooking()
    {
        Debug.Log("Stopped looking at soda can");
        // _highlight.SetIsHighlighted(false);
        currentTarget = null;
        HideDrinkIcon();
        // _highlight.Highlight();
    }

    public void ShowDrinkIcon()
    {
        UIManager.Instance.interactText.gameObject.SetActive(true);

    }
    public void HideDrinkIcon()
    {
        UIManager.Instance.interactText.gameObject.SetActive(false);
    }

    public void OnPressInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log("Drinking soda can. Interact system works.");

        DrinkSoda();

    }

    private void OnDestroy()
    {
        InputManager.Instance.InteractAction.performed -= OnPressInteract;
    }


}