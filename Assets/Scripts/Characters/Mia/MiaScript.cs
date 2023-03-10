using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cinemachine;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MiaScript : MonoBehaviour
{
    [SerializeField] float playerSpeed = 2.0f;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] float mouseSensitivity;
    [SerializeField] LayerMask groundMask;
    private float horizontalMove;
    private float verticalMove;
    private Animator animator;
    private CinemachineRecomposer composer;
    private Vector3 move;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float gravityValue = -9.81f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        composer = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CinemachineRecomposer>();
    }

    void Update()
    {
        groundedPlayer = groundCheck();
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            animator.SetBool("Jump", false);
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        controller.Move(transform.TransformDirection(move * playerSpeed * Time.deltaTime));

        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);

            if (move.magnitude >= 0.5)
            {
                animator.SetBool("Jump", true);
            } else
            {
                animator.SetTrigger("StillJump");
            }
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
        animator.SetFloat("Speed", move.magnitude);

        mouseControl();
    }

    void mouseControl()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;
        transform.Rotate(Vector3.up * mouseX);
        float mouseY = -(Input.GetAxis("Mouse Y") * mouseSensitivity * Time.fixedDeltaTime);
        composer.m_Tilt += mouseY;

        if (composer.m_Tilt >= 40)
        {
            composer.m_Tilt = 40;
        }
        else if (composer.m_Tilt <= -40)
        {
            composer.m_Tilt = -40;
        }
    }

    bool groundCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 0.1f, groundMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}