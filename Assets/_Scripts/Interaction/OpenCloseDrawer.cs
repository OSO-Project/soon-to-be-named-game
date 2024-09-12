using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OpenCloseDrawer : MonoBehaviour, Interactable
{
    //This is a template for scripts for interactable objects
    //Remember to always set the tag of the object with this script to Interactable, either through engine
    //Or better yet, do it through code with f.e putting "gameObject.tag="Interactable"" in Start() or Awake()
    //Remember to also include the Interactable interface, as shown above.
    //The interaction range is global between interactable objects, and you can set it in the
    //InteractionManager.cs script or the object with that script. If needed, i can change this.
    
    private bool isOpen = false;
    private Animator animator;
    private static OpenCloseDrawer currentTarget = null;
    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component missing from drawer object.");
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
        if(currentTarget == this)
        {
            if (animator == null)
            {
                Debug.Log("animator is null");
                return;
            }

            if (isOpen)
            {
                animator.SetTrigger("close");
            }
            else
            {
                animator.SetTrigger("open");
            }

            isOpen = !isOpen;
        }
    }
}