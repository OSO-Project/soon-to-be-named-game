using UnityEngine;
using UnityEngine.InputSystem;

public class MoveWindowHandle : MonoBehaviour, Interactable
{
    public bool isUnlocked = false;
    public string state = "Down";
    private bool isOpen;
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
        isOpen = windowStatus.isOpen;

        if (currentTarget == this)
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
                    isUnlocked = true;
                    state = "Side";
                }
                else if (state == "Side")
                {
                    handleAnimator.SetTrigger("Up");
                    isUnlocked = true;
                    state = "Up";
                }
                else if (state == "Up")
                {
                    handleAnimator.SetTrigger("Down");
                    isUnlocked = false;
                    state = "Down";

                }
            }
        }
    }
}
