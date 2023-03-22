using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerX : MonoBehaviour
{
    [Header("GameObjects")] [Tooltip("External objects that need to be added")]
    public CharacterController playerController;
    public Transform groundCheck;

    [Header("Movement")] [Tooltip("Move Speed is managed by walk/run so don't change it")]
    [SerializeField] float moveSpeed; 
    public float walkSpeed = 3f;
    public float runSpeed = 3f;
    public float jumpHeight = 1.5f;
    [SerializeField] Vector3 moveDirection;

    [Header("Keycodes")]
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
     
    [Header("External forces")]
    public float gravity = -18f;
    private Vector3 velocity;

    [Header("Ground Check")] [Tooltip("Ground Mask is the floor layer")]
    public LayerMask groundMask;
    public float groundDistance = 0.4f;
    public bool isGrounded;

    [Header("Verification states")]
    public MovementState state;

    // Internal variables
    float horizontalInput;
    float verticalInput; 

    private void Awake()
    {
        playerController = GetComponent<CharacterController>();
    }

    void Update()
    {
        PlayerMove();
        GroundControl();
        SpeedControl();
        StateHandler();
        PlayerJump();
    }

    private void PlayerMove()
    {
        // Movement
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // To walk in the direction looking at

        Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f,
                                            Input.GetAxis("Vertical")).normalized;

        playerController.Move(transform.TransformDirection(moveDirection
                            * moveSpeed * Time.deltaTime));
    }

    private void GroundControl()
    {
        // Creates a shpere around groundCheck to check if isGrounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Just to force player to stay on the ground
        }

    }

    // For gravity and after jump
    private void SpeedControl()
    {
        velocity.y += gravity * Time.deltaTime;
        playerController.Move(velocity * Time.deltaTime);
    }

    public enum MovementState
    {
        walking,
        running,
        airbone
    }

    private void StateHandler()
    {
        // Mode - running
        if (isGrounded && Input.GetKey(runKey))
        {
            state = MovementState.running;
            moveSpeed = runSpeed;
        }
        // Mode - walking
        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        // Mode - air
        else
        {
            state = MovementState.airbone;
        }
    }

    private void PlayerJump()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
