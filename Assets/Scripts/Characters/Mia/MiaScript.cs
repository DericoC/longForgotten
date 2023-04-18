using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MiaScript : MonoBehaviour
{
    [Header("Reference to GameObjects")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private WeaponManager weaponManager;
    private CinemachineRecomposer composer;
    private CharacterController playerController;
    private LogicController logicController;
    private Animator playerAnimator;

    [Header("Movement")]
    [SerializeField] private float crouchSpeed = 2.0f;
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float runSpeed = 6.0f;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpHeight = 1.0f;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private bool isCrouching;

    [Header("Keycodes")]
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode reloadKey = KeyCode.R;

    [Header("Mouse configuration")]
    [SerializeField] private float mouseSensitivity;

    [Header("inGame items / vars")]
    [SerializeField] private int playerCurrency = 200;
    private bool[] keys;
    private bool isPressingDoorOpen = false;
    private bool dCheck = false;

    [Header("Inventory Controll")]
    public GameObject[] weapons; // Inventory holding weapons (melee, primary, secondary)
    public int currentWeapon = 0; // 0: Melee, 1: Primary, 2: Secondary
    public GameObject activeInventoryItem = null;

    [Header ("Camera")]
    private Camera mainCamera;

    [Header("External forces")]
    public float gravity = -9.81f;
    private Vector3 fallVelocity;

    [Header ("Ground check")]
    private bool isGrounded;
    public float groundDistance = 0.5f;

    [Header ("Verification states")]
    [SerializeField] private MovementState mState;
    [SerializeField] private WeaponState wState;

    public enum MovementState
    {
        walking,
        running,
        airbone,
        crouching,
        idle
    }

    public enum WeaponState
    {
        melee,
        weapon,
        aiming,
        shooting,
        reload
    }

    void Start()
    {
        // GameObjects
        playerController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
        weaponManager = GetComponentInChildren<WeaponManager>();
        weaponManager.ActivateWeapon(currentWeapon);
        logicController = GameObject.FindWithTag("Logic").GetComponent<LogicController>();

        // Camera
        mainCamera = Camera.main;
        composer = mainCamera.GetComponentInChildren<CinemachineRecomposer>();

        // Inventory
        LoadKeys();

        // Gameplay
        Cursor.lockState = CursorLockMode.Locked;
        isCrouching = false;

        // Animator
        UpdateAnimatorController();
    }

    void FixedUpdate()
    {
        if (!logicController.Pause)
        {
            PlayerMovement();
            SpeedControl();
            GroundControll();
            PlayerJump();
            SwitchWeapons();
            HandleWeaponAnimations();
        }
    }

    void Update()
    {

        if (!logicController.Pause)
        {
            StateHandler();
            MouseControll();
            PlayerCrouch();
            DoorController();
        }
    }

    #region Movement
    void PlayerMovement()
    {
        // Movement
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // To walk in the direction looking at
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f,
                                            Input.GetAxis("Vertical")).normalized;

        playerController.Move(transform.TransformDirection(moveDirection
                            * moveSpeed * Time.deltaTime));
    }

    void StateHandler()
    {
        // Mode - Idle
        if (horizontalInput == 0 && verticalInput == 0 && !Input.GetKey(runKey))
        {
            mState = MovementState.idle;
            moveSpeed = walkSpeed;
            playerAnimator.SetBool("Run", false);
            playerAnimator.ResetTrigger("Jump");

            playerAnimator.SetFloat("XSpeed", 0);
            playerAnimator.SetFloat("YSpeed", 0);

            if (!isGrounded && Input.GetKey(jumpKey))
            {
                mState = MovementState.airbone;
                playerAnimator.SetBool("Jump", true);

            }
        }
        // Mode - running
        else if (isGrounded && Input.GetKey(runKey))
        {
            mState = MovementState.running;
            moveSpeed = runSpeed;
            playerAnimator.ResetTrigger("Jump");

            playerAnimator.SetBool("Run", true);

            playerAnimator.SetFloat("XSpeed", moveDirection.x);
            playerAnimator.SetFloat("YSpeed", moveDirection.z);
        }
        // Mode - walking
        else if (isGrounded && (horizontalInput != 0 || verticalInput != 0))
        {
            mState = MovementState.walking;
            moveSpeed = walkSpeed;
            playerAnimator.SetBool("Run", false);
            playerAnimator.ResetTrigger("Jump");

            playerAnimator.SetFloat("XSpeed", moveDirection.x);
            playerAnimator.SetFloat("YSpeed", moveDirection.z);

        }
        // Mode - air
        else if (!isGrounded)
        {
            mState = MovementState.airbone;
            playerAnimator.SetTrigger("Jump");
        }
        // Mode - crouching
        else if (!isCrouching)
        {
            mState = MovementState.crouching;
            moveSpeed = crouchSpeed;
            playerAnimator.ResetTrigger("Jump");

            playerAnimator.SetBool("Crouch", true);

            playerAnimator.SetFloat("XSpeed", moveDirection.x);
            playerAnimator.SetFloat("YSpeed", moveDirection.z);
        }
    }

    void PlayerJump()
    {
        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            fallVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (Input.GetKeyDown(jumpKey) && isGrounded && Input.GetKeyDown(runKey))
        {
            fallVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            playerAnimator.SetFloat("YSpeed", 0); // Esto deberia contrarrestar
                                                  // el movimiento que hace el anim
        }
    }

    void PlayerCrouch()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = !isCrouching;
        }
    }

    // For gravity and after jump
    private void SpeedControl()
    {
        fallVelocity.y += gravity * Time.deltaTime;
        playerController.Move(fallVelocity * Time.deltaTime);
    }

    void MouseControll()
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

    void GroundControll()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && !wasGrounded && fallVelocity.y < 0)
        {
            fallVelocity.y = -2f;
        }
    }
    #endregion Movement

    #region Doors
    private async void MaintainDoorInteraction(float delaySeconds)
    {
        await Task.Delay((int)(delaySeconds * 1000));
        isPressingDoorOpen = false;
    }

    public void DoorInteraction(DoorControl door)
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
                        if (CountKeys() >= 1)
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

    void LoadKeys()
    {
        keys = new bool[5];
        keys[0] = false;
        keys[1] = false;
        keys[2] = false;
        keys[3] = false;
        keys[4] = false;
    }

    int CountKeys()
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

    void DoorController()
    {
        // Doors
        if (Input.GetKeyDown(KeyCode.E))
        {
            isPressingDoorOpen = true;
            MaintainDoorInteraction(1f);
        }
    }

    void D()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            dCheck = !dCheck;
            playerAnimator.SetBool("D", dCheck);
        }
    }
    #endregion Doors

    #region Weapons
    private void SwitchWeapons()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentWeapon != 0)
        {
            SetCurrentWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && currentWeapon != 1)
        {
            SetCurrentWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && currentWeapon != 2)
        {
            SetCurrentWeapon(2);
        }
    }

    private void SetCurrentWeapon(int weaponIndex)
    {
        if (weaponManager.GetActiveWeapon(weaponIndex) != null)
        {
            currentWeapon = weaponIndex;
            weaponManager.ActivateWeapon(currentWeapon);
            UpdateAnimatorController();
        }
    }

    private void UpdateAnimatorController()
    {
        playerAnimator.SetLayerWeight(1, currentWeapon == 0 ? 0 : 1);
    }

    private void HandleWeaponAnimations()
    {
        // Melee or no weapon
        if (currentWeapon == 0)
        {
            wState = WeaponState.melee;
            if (Input.GetMouseButtonDown(0))
            {
                playerAnimator.SetTrigger("Punch");
            }
        }
        // Has a firearm or weapon
        else
        {
            // Shooting
            wState = WeaponState.weapon;
            if (Input.GetMouseButtonDown(0))
            {
                wState = WeaponState.shooting;
                playerAnimator.SetTrigger("Shooting");
            }
            // Aiming
            else if (Input.GetMouseButtonDown(1))
            {
                wState = WeaponState.aiming;
                playerAnimator.SetBool("isAiming", true);

                if (Input.GetMouseButtonDown(0))
                {
                    wState = WeaponState.shooting;
                    playerAnimator.SetTrigger("Shooting");
                }
            }
            // Reload
            else if (Input.GetKey(reloadKey))
            {
                wState = WeaponState.reload;
                playerAnimator.SetTrigger("Reload");
            }
            // Running
            else if (isGrounded && Input.GetKey(runKey))
            {
                playerAnimator.SetBool("Run", true);
            }
        }
    }
    #endregion Weapons
}
