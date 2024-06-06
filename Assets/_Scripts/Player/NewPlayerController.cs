using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewPlayerController : MonoBehaviour
{
    private CharacterController _characterController;

    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 20f;
    [SerializeField] private float jumpStrength = 100f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float distanceToGround = 0.8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float additionalFallGravity = 20f;

    public bool _grounded;

    public bool CanMove;

    private void Start()
    {
        CanMove = true;
        _characterController = GetComponent<CharacterController>();
    }

    // For testing earthquake encounter
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameEventManager.Instance.HandleEncounterStart();
        }
    }

    private void FixedUpdate()
    {
        Move();
        if (!CanMove) return;
        CheckGround();

        HandleJump();
        ApplyAdditionalGravity();
    }

    private void Move()
    {
        if (!InputManager.Instance) return;
        if (!CanMove)
        {
            _characterController.Move(Vector3.zero);
        }
        else
        {
            Vector2 input = InputManager.Instance.Move;
            Vector3 targetVelocity = new Vector3(input.x, 0, input.y).normalized;
            targetVelocity = transform.TransformDirection(targetVelocity) * (InputManager.Instance.Run ? runSpeed : walkSpeed);

            Vector3 currentVelocity = _characterController.velocity;

            _characterController.Move(targetVelocity * Time.fixedDeltaTime);
        }
    }


    private void HandleJump()
    {
        if (InputManager.Instance.Jump && _grounded)
        {
            Vector3 jumpVelocity = Vector3.up * Mathf.Sqrt(2 * jumpStrength * Mathf.Abs(Physics.gravity.y));
            _characterController.Move(jumpVelocity * Time.fixedDeltaTime);
        }
    }

    private void ApplyAdditionalGravity()
    {
        if (!_grounded)
        {
            Vector3 additionalGravity = Vector3.down * additionalFallGravity;
            _characterController.Move(additionalGravity * Time.fixedDeltaTime);
        }
    }

    private void CheckGround()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * distanceToGround, Color.red);
        _grounded = Physics.Raycast(transform.position, Vector3.down, distanceToGround, groundMask);
    }

    public float GetCurrentSpeed()
    {
        Vector3 horizontalVelocity = _characterController.velocity;
        horizontalVelocity.y = 0;
        return horizontalVelocity.magnitude;
    }
}
