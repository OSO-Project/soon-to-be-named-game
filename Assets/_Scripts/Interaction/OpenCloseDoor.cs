using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenCloseDoor : MonoBehaviour, Interactable
{
    //This is a template for scripts for interactable objects
    //Remember to always set the tag of the object with this script to Interactable, either through engine
    //Or better yet, do it through code with f.e putting "gameObject.tag="Interactable"" in Start() or Awake()
    //Remember to also include the Interactable interface, as shown above.
    //The interaction range is global between interactable objects, and you can set it in the
    //InteractionManager.cs script or the object with that script. If needed, i can change this.

    [SerializeField]
    private bool isUnlocked = false;
    private bool isOpen = false;
    private bool isAnimating = false;
    private Animator animator;
    private static OpenCloseDoor currentTarget = null;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing from drawer object.");
        }
    }

    private void OnDestroy()
    {
    }

    public void OnBeginLooking()
    {
        Debug.Log("Looking");
        currentTarget = this;
        //Code to execute when you aim at the object
    }
    public void OnFinishLooking()
    {
        currentTarget = null;
        Debug.Log("NotLooking");
        //Code to execute when you aim away from the object
    }

    public void OnPressInteract(InputAction.CallbackContext ctx)
    {
        if (currentTarget == this && !isAnimating)
        {
            if (!isUnlocked)
            {
                return;
            }
            else
            {
                if(GameManager.Instance.AttemptToLeaveRoom())
                {
                    if (animator == null )
                    {
                        return;
                    }

                    if (isOpen)
                    {
                        animator.SetTrigger("Close");
                    }
                    else
                    {
                        animator.SetTrigger("Open");
                    }
                    isAnimating = true;
                    isOpen = !isOpen;
                }
            }
        }
    }

    public void CloseAndLockDoor()
    {
        isUnlocked = false;
        isOpen = false;
        animator.SetTrigger("Close");
    }
    public void OnAnimationComplete()
    {
        isAnimating = false; // Reset the animating flag
    }
}