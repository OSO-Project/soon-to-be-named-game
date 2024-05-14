using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player data")]
    [SerializeField] public float _crouchSpeed = 2.5f;
    [SerializeField] public float _walkSpeed = 5f;
    [SerializeField] public float _runSpeed = 8f;
    [SerializeField] private float _jumpForce = 7.0f;
    [SerializeField] private float _gravityMod = 2.5f;
    [SerializeField] public float _activeMoveSpeed;
    [SerializeField] private Rigidbody rb;
    [Header("Ground Layers")]
    [SerializeField] private LayerMask _groundLayers;


    public Vector3 _moveDirection;
    private Vector3 _movement;


    [Header("Crouch Settings")]
    public float standingHeight = 2f;
    public bool isWalking;

    [SerializeField] private AudioSource audioSourceMoves;
    public GameObject audioObject;


    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;
    private void Start()
    {
    }

    private void Update()
    {
        PlayerMovement();
    }


    private void PlayerMovement()
    {
        Debug.Log(move.action.ReadValue<Vector2>().x);
        Debug.Log(move.action.ReadValue<Vector2>().y);
        _moveDirection = new Vector3(move.action.ReadValue<Vector2>().x, 0f, move.action.ReadValue<Vector2>().y);
        var horizontalVelocity = new Vector3(5, 0, 5);
        float horizontalSpeed = horizontalVelocity.magnitude;
        if (horizontalSpeed == 0)
        {
            isWalking = false;
        }

        var yVelocity = _movement.y;

        var playerTransform = transform;
        _movement = (playerTransform.forward * _moveDirection.z + playerTransform.right * _moveDirection.x).normalized * _activeMoveSpeed;

        _movement.y += yVelocity;

        _movement.y += Physics.gravity.y * Time.deltaTime * _gravityMod;


   
    }
}
