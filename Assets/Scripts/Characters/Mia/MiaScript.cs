using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinemachine;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MiaScript : MonoBehaviour
{
    [SerializeField] float playerSpeed = 2.0f;
    [SerializeField] float jumpHeight = 1.0f;
    [SerializeField] float mouseSensitivity;
    [SerializeField] int playerCurrency = 200;
    [SerializeField] LayerMask groundMask;
    private bool[] keys;
    private float horizontalMove;
    private float verticalMove;
    private Animator animator;
    private CinemachineRecomposer composer;
    private Vector3 move;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private bool crouch;
    private float gravityValue = -9.81f;
    private bool isPressingDoorOpen = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        crouch = false;
        composer = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CinemachineRecomposer>();
        loadKeys();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isPressingDoorOpen = true;
            maintainDoorInteraction(1f);
        }

        groundedPlayer = groundCheck();
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            animator.SetBool("Jump", false);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouch = !crouch;
            animator.SetBool("Crouched", crouch);
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
        
        if (Input.GetKey(KeyCode.LeftShift))
        {
            move = new Vector3(Input.GetAxis("Horizontal") * 2f, 0, Input.GetAxis("Vertical") * 2f);
        }

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

        if (move.z < 0)
        {
            animator.SetFloat("Speed", -move.magnitude);
        }
        else
        {
            animator.SetFloat("Speed", move.magnitude);
        }
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

    private async void maintainDoorInteraction(float delaySeconds)
    {
        await Task.Delay((int)(delaySeconds * 1000));
        isPressingDoorOpen = false;
    }

    public void doorInteraction(DoorControl door)
    {
        if (door != null)
        {
            if (isPressingDoorOpen)
            {
                isPressingDoorOpen = false;
                if (door.IsOpen)
                {
                    door.closeDoor();
                }
                else
                {
                    if (door.HasKey)
                    {
                        if (countKeys() >= 1)
                        {
                            door.destroyDoor(keys);
                        }
                        else
                        {
                            door.lockedDoor();
                        }
                    }
                    else
                    {
                        door.openDoor();
                    }
                }
            }
        }
    }

    void loadKeys()
    {
        keys = new bool[5];
        keys[0] = false;
        keys[1] = false;
        keys[2] = false;
        keys[3] = false;
        keys[4] = false;
    }

    int countKeys()
    {
        int count = 0;
        foreach (bool key in keys)
        {
            if (key)
            {
                count++;
            }
        }
        return count;
    }
}