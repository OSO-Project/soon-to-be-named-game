using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private InputManager _inputManager;

    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 20f;
    [SerializeField] private float jumpStrength = 260f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float distanceToGround = 0.8f;
    private float _currentSpeed;

    private bool _grounded;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _inputManager = GetComponent<InputManager>();
    }

    private void FixedUpdate()
    {
        Move();
        CheckGround();
        if (_inputManager.Jump && _grounded)
        {
            Jump();
        }
    }

    private void Move()
    {
        if (!_inputManager) return;

        Vector2 input = _inputManager.Move;
        Vector3 move = new Vector3(input.x, 0, input.y).normalized;
        move = transform.TransformDirection(move);

        if (move.magnitude >= 0.1f)
        {
            _currentSpeed = _inputManager.Run ? runSpeed : walkSpeed;

            Debug.Log("Speed: " + _currentSpeed);

            Vector3 velocity = move * _currentSpeed;
            velocity.y = _rb.velocity.y; // Preserve the existing y velocity (e.g., gravity)

            _rb.velocity = velocity;
        }
        else
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0); // Maintain vertical velocity
        }
    }

    private void Jump()
    {
        _rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
    }

    private void CheckGround()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * distanceToGround, Color.red);
        _grounded = Physics.Raycast(transform.position, Vector3.down, distanceToGround, groundMask);

    }

    public float GetCurrentSpeed()
    {
        Vector3 horizontalVelocity = _rb.velocity;
        horizontalVelocity.y = 0; // Ignore vertical velocity for speed calculation
        return horizontalVelocity.magnitude;
        //return _currentSpeed;
    }
}
