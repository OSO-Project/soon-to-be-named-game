using UnityEngine;
using UnityEngine.InputSystem;

public class OpenCloseWindow : MonoBehaviour, Interactable
{
    private bool isUnlocked;
    private string state;
    public bool isOpen;
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

        InputManager.Instance.OpenCloseAction.performed += OnPressInteract;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OpenCloseAction.performed -= OnPressInteract;
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

        if (currentTarget == this)
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
                    GameEventManager.Instance.OpenWindow();
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
                isOpen = !isOpen;
            }
        }
    }
}
