using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public bool CanMove;

    private void Start()
    {
        CanMove = true;
        _rb = GetComponent<Rigidbody>();
        _inputManager = GetComponent<InputManager>();
    }

    private void FixedUpdate()
    {
        Move();
        if (!CanMove) return;
        CheckGround();
        if (_inputManager.Jump && _grounded)
        {
            Jump();
        }
    }

    private void Move()
    {
        if (!_inputManager) return;
        if (!CanMove)
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
        }
        else
        {
            Vector2 input = _inputManager.Move;
            Vector3 move = new Vector3(input.x, 0, input.y).normalized;
            move = transform.TransformDirection(move);

            if (move.magnitude >= 0.1f)
            {
                _currentSpeed = _inputManager.Run ? runSpeed : walkSpeed;

                Vector3 targetVelocity = move * _currentSpeed;
                _rb.velocity = new Vector3(targetVelocity.x, _rb.velocity.y, targetVelocity.z);
            }
            Vector3 velocity = move * _currentSpeed;
            _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z);
        }
    }

    private void Jump()
    {
        if (_grounded)
        {
            //_rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);
            Vector3 jumpVelocity = _rb.velocity;
            jumpVelocity.y = Mathf.Sqrt(2f * jumpStrength * -Physics.gravity.y); // Calculate jump velocity using physics formula
            _rb.velocity = jumpVelocity;
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
        horizontalVelocity.y = 0; // Ignore vertical velocity for speed calculation
        return horizontalVelocity.magnitude;
        //return _currentSpeed;
    }
}
