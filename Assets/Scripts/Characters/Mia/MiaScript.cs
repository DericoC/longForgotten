using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

public class MiaScript : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private int playerCurrency = 200;
    [SerializeField] private LayerMask groundMask;

    // START - Inventory Control
    private List<GameObject> inventory = new List<GameObject>();
    public GameObject activeInventoryItem = null;
    private int activeIsPrimary = 0; //0:Hands 1:Primary 2:Secondary
    private RuntimeAnimatorController rifleAnimator;
    private RuntimeAnimatorController noItemAnimator;
    private bool emptyInventory = true;
    // END - Inventory Control

    private bool[] keys;
    private float horizontalMove;
    private float verticalMove;
    private Animator animator;
    private CinemachineRecomposer composer;
    private Vector3 move;
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool dCheck = false;
    private bool groundedPlayer;
    private bool crouch;
    private float gravityValue = -9.81f;
    private bool isPressingDoorOpen = false;
    private PlayerShoot playerShoot;

    void Start()
    {
        playerShoot = GetComponent<PlayerShoot>();
        rifleAnimator = Resources.Load<RuntimeAnimatorController>("Mia/w Rifle/MiaAnimatorWithRifle");
        noItemAnimator = Resources.Load<RuntimeAnimatorController>("Mia/MiaAnimator");
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        crouch = false;
        composer = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<CinemachineRecomposer>();
        checkForInventory();
        playerShoot.gunChange(activeInventoryItem);
        emptyInventory = inventory.Count == 0;
        loadKeys();
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            animator.SetBool("IsAiming", true);
        } else
        {
            animator.SetBool("IsAiming", false);
        }

        if (!emptyInventory)
        {
            changeWeapon();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isPressingDoorOpen = true;
            maintainDoorInteraction(1f);
        }

        groundedPlayer = groundCheck(); d();
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
        return Physics.Raycast(transform.position, Vector3.down, 0.1f, groundMask);
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
        for (int i = 0; i < keys.Length; i++)
        {
            if (keys[i])
            {
                count++;
            }
        }
        return count;
    }

    void d()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            dCheck = !dCheck;
            animator.SetBool("D", dCheck);
        }
    }

    void checkForInventory()
    {
        bool hasRifle = false;
        GameObject rightHandInventory = GameObject.Find("inventory");

        if (rightHandInventory.transform.childCount > 0)
        {
            for (int i = 0; i < rightHandInventory.transform.childCount; i++) {
                inventory.Add(rightHandInventory.transform.GetChild(i).gameObject);
                if (rightHandInventory.transform.GetChild(i).gameObject.activeInHierarchy && rightHandInventory.transform.GetChild(i).gameObject.tag == "Rifle")
                {
                    hasRifle = true;
                    activeInventoryItem = rightHandInventory.transform.GetChild(i).gameObject;
                }
            }
        }

        animator.runtimeAnimatorController = hasRifle ? rifleAnimator : noItemAnimator;
    }

    void changeWeapon()
    {
        int keyPressed = 0;
        if (Input.GetKeyDown(KeyCode.Alpha1)) //Change to Primary
        {
            if (inventory.Count >= 1 && inventory[0] != null)
            {
                if (activeIsPrimary == 1)
                {
                    activeInventoryToNull();
                }
                else
                {
                    activeInventoryItem = inventory[0];
                    activeIsPrimary = 1;
                    playerShoot.HasGun = true;
                }
            } else
            {
                activeInventoryToNull();
            }
            keyPressed = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) // Change to Secondary
        {
            keyPressed = 2;
            if (inventory.Count >= 2 && inventory[1] != null)
            {
                if (activeIsPrimary == 2)
                {
                    activeInventoryToNull();
                } else
                {
                    activeInventoryItem = inventory[1];
                    activeIsPrimary = 2;
                    playerShoot.HasGun = true;
                }
            } else
            {
                activeInventoryToNull();
            }
        }

        // Change Inventory Status
        if (keyPressed != 0)
        {
            if (activeInventoryItem == null)
            {
                inventoryOverride(false); // Hide weapons
                animator.runtimeAnimatorController = noItemAnimator;
            }
            else
            {
                if (keyPressed == 1 && activeInventoryItem.tag == "Rifle")
                {
                    animator.runtimeAnimatorController = rifleAnimator;
                    toggleInvPrimary(true);
                } else if (keyPressed == 1 && activeInventoryItem.tag == "Pistol")
                {
                    //animator.runtimeAnimatorController = pistolAnimator;
                    toggleInvPrimary(true);
                } else if (keyPressed == 2 && activeInventoryItem.tag == "Rifle")
                {
                    animator.runtimeAnimatorController = rifleAnimator;
                    toggleInvPrimary(false);
                } else if (keyPressed == 2 && activeInventoryItem.tag == "Pistol")
                {
                    //animator.runtimeAnimatorController = pistolAnimator;
                    toggleInvPrimary(false);
                }
            }
        }
    }

    void toggleInvPrimary(bool on)
    {
        if (inventory.Count >= 2 && inventory[1] != null)
        {
            inventory[1].SetActive(!on);
        }
        if (inventory[0] != null)
        {
            inventory[0].SetActive(on);
        }
    }

    void inventoryOverride(bool o)
    {
        if (inventory.Count >= 2 && inventory[1] != null)
        {
            inventory[1].SetActive(o);
        }
        if (inventory[0] != null)
        {
            inventory[0].SetActive(o);
        }
    }

    void activeInventoryToNull()
    {
        activeInventoryItem = null;
        activeIsPrimary = 0;
        playerShoot.HasGun = false;
    }

    // Getters / Setters
    public bool EmptyInventory { get => emptyInventory; set => emptyInventory = value; }
}