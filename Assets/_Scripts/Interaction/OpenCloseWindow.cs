using UnityEngine;
using UnityEngine.InputSystem;

public class OpenCloseWindow : MonoBehaviour, Interactable
{
    private bool isUnlocked;
    private string state;
    public bool isOpen;
    private bool isAnimating = false;
    private Animator windowAnimator;
    private OpenCloseWindow currentTarget = null;
    private MoveWindowHandle windowHandleStatus = null;
    private void Start()
    {

        windowAnimator = GetComponent<Animator>();
        if (windowAnimator == null)
        {
            Debug.LogError("Animator component missing from drawer object.");
        }
        
        windowHandleStatus = transform.Find("HandleHinge").GetComponent<MoveWindowHandle>();
        if (windowHandleStatus == null)
        {
            Debug.LogError("No MoveWindowHandle script attached to the handle.");
        }
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
        isUnlocked = windowHandleStatus.isUnlocked;
        state = windowHandleStatus.state;

        if (currentTarget == this && !isAnimating)
        {
            if (!isUnlocked)
            {
                return;
            }
            else
            {
                if (windowAnimator == null)
                {
                    return;
                }

                if (!isOpen)
                {
                    if (state == "Side")
                    {
                        windowAnimator.SetTrigger("OpenHorizontal");
                    }
                    else if (state == "Up")
                    {
                        windowAnimator.SetTrigger("OpenVertical");
                    }
                    GameEventManager.Instance.OpenWindow(state.Equals("Up"));
                }
                else
                {
                    if (state == "Side")
                    {
                        windowAnimator.SetTrigger("CloseHorizontal");
                    }
                    else if (state == "Up")
                    {
                        windowAnimator.SetTrigger("CloseVertical");
                    }
                }
                isAnimating = true;
                isOpen = !isOpen;
            }
        }
    }

    public void OnAnimationComplete()
    {
        isAnimating = false; // Reset the animating flag
    }
}
