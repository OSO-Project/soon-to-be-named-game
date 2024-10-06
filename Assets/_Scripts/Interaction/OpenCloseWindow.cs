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
    [SerializeField] private GameObject windHitBox;
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

        // InputManager.Instance.OpenCloseAction.performed += OnPressInteract;


    }

    private void OnDestroy()
    {
        // InputManager.Instance.OpenCloseAction.performed -= OnPressInteract;
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
                if (isOpen)
                {
                    GameEventManager.Instance.OpenWindow();
                }
            }
        }
    }

    public void OnAnimationComplete()
    {
        isAnimating = false; // Reset the animating flag
    }
    public bool GetIfUp()
    {
        return state.Equals("Up");
    }

    public float GetOpenness()
    {
        if (isOpen)
        {
            if (state.Equals("Up")) return 0.5f;
            else return 1f;
        }
        return 0f;
    }
    public GameObject GetWindHitbox()
    {
        return windHitBox;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var area = windHitBox.GetComponent<Collider>();
        if (isOpen)
        {
            Gizmos.DrawWireCube(area.bounds.center, area.bounds.size);
        }
    }
}
