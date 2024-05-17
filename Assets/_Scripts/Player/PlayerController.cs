using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private InputManager _inputManager;

    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float jumpStrength = 260f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float distanceToGround = 0.8f;

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

    /*    private void Move()
        {
            Vector3 direction = new Vector3(_inputManager.Move.x, 0, _inputManager.Move.y);
            Vector3 move = transform.right * direction.x + transform.forward * direction.z;
            _rb.velocity = new Vector3(move.x * walkSpeed, _rb.velocity.y, move.z * walkSpeed);
        }*/

    private void Move()
    {
        if (!_inputManager) return;

        Vector2 input = _inputManager.Move;
        Vector3 move = new Vector3(input.x, 0, input.y).normalized;
        move = transform.TransformDirection(move);

        if (move.magnitude >= 0.1f)
        {
            float targetSpeed = walkSpeed;
            Vector3 velocity = move * targetSpeed;
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
}
