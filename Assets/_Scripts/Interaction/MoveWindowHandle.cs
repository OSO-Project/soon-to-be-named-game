using UnityEngine;
using UnityEngine.InputSystem;

public class MoveWindowHandle : MonoBehaviour, Interactable
{
    public bool isUnlocked = false;
    public string state = "Down";
    private bool isOpen;
    private bool isAnimating = false;
    private Animator handleAnimator;
    private MoveWindowHandle currentTarget = null;
    private OpenCloseWindow windowStatus = null;
    private void Start()
    {
        handleAnimator = GetComponent<Animator>();
        if (handleAnimator == null)
        {
            Debug.LogError("Animator component missing from drawer object.");
        }
        windowStatus = transform.parent.GetComponent<OpenCloseWindow>();
    }

    private void OnDestroy()
    {
    }

    public void OnBeginLooking()
    {
        currentTarget = this;
        //Code to execute when you aim at the object
    }
    public void OnFinishLooking()
    {
        currentTarget = null;
        //Code to execute when you aim away from the object
    }

    public void OnPressInteract(InputAction.CallbackContext ctx)
    {
        isOpen = windowStatus.isOpen;

        if (currentTarget == this && !isAnimating)
        {
            if (handleAnimator == null)
            {
                return;
            }
            if (!isOpen)
            {
                if (state == "Down")
                {
                    handleAnimator.SetTrigger("Side");
                    isAnimating = true;
                    isUnlocked = true;
                    state = "Side";
                }
                else if (state == "Side")
                {
                    handleAnimator.SetTrigger("Up");
                    isAnimating = true;
                    isUnlocked = true;
                    state = "Up";
                }
                else if (state == "Up")
                {
                    handleAnimator.SetTrigger("Down");
                    isAnimating = true;
                    isUnlocked = false;
                    state = "Down";

                }
            }
        }
    }
    public void OnAnimationComplete()
    {
        isAnimating = false; // Reset the animating flag
    }
}
