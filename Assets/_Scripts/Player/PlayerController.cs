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

    // For testing earthquake encounter
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEventManager.Instance.HandleEncounterStart();
        }
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

            Vector3 targetVelocity = move * _currentSpeed;
        }
        Vector3 velocity = move * _currentSpeed;
        _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z);
        /*        if (!_inputManager) return;

                Vector2 input = _inputManager.Move;
                Vector3 move = new Vector3(input.x, 0, input.y).normalized;
                move = transform.TransformDirection(move);

                if (move.magnitude >= 0.1f)
                {
                    _currentSpeed = _inputManager.Run ? runSpeed : walkSpeed;

                    Vector3 targetVelocity = move * _currentSpeed;
                    _rb.MovePosition(_rb.position + targetVelocity * Time.fixedDeltaTime);
                }*/
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
