using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 20f;
    [SerializeField] private float jumpStrength = 100f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float distanceToGround = 0.8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float additionalFallGravity = 20f;

    private bool _grounded;

    public bool CanMove;

    private void Start()
    {
        CanMove = true;
        _rb = GetComponent<Rigidbody>();
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
        if (InputManager.Instance.Jump && _grounded)
        {
            Jump();
        }
    }

    private void Move()
    {
        if (!InputManager.Instance) return;
        if (!CanMove)
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        }
        else
        {
            Vector2 input = InputManager.Instance.Move;
            Vector3 targetVelocity = new Vector3(input.x, 0, input.y).normalized;
            targetVelocity = transform.TransformDirection(targetVelocity) * (InputManager.Instance.Run ? runSpeed : walkSpeed);

            Vector3 velocity = _rb.velocity;
            Vector3 velocityChange = (targetVelocity - velocity);
            velocityChange.y = 0;

            if (targetVelocity.magnitude > 0)
            {
                _rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
            }
            else
            {
                _rb.AddForce(velocityChange * deceleration, ForceMode.Acceleration);
            }
        }
    }

    private void Jump()
    {
        if (_grounded)
        {
            Vector3 jumpForce = Vector3.up * jumpStrength;
            _rb.AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    private void ApplyAdditionalGravity()
    {
        if (!_grounded)
        {
            _rb.AddForce(Vector3.down * additionalFallGravity, ForceMode.Acceleration);
        }
    }

    private void CheckGround()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * distanceToGround, Color.red);
        _grounded = Physics.Raycast(transform.position, Vector3.down, distanceToGround, groundMask);
    }

    public float GetCurrentSpeed()
    {
        Vector3 horizontalVelocity = _rb.velocity;
        horizontalVelocity.y = 0;
        return horizontalVelocity.magnitude;
    }
}
