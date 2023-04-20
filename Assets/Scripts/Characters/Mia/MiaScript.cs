using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static MiaScript;

public class MiaScript : MonoBehaviour
{
    [Header("Reference to GameObjects")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;
    private WeaponManager weaponManager;
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
    private Vector3 moveDirection;
    private float horizontalInput;
    private float verticalInput;
    private bool isCrouching;

    [Header("Keycodes")]
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode aimKey = KeyCode.Mouse1;
    public KeyCode shootKey = KeyCode.Mouse0;

    [Header("Mouse configuration")]
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

    [Header("inGame items / vars")]
    [SerializeField] private int playerCurrency = 200;
    private bool[] keys;
    private bool isPressingDoorOpen = false;
    private bool dCheck = false;

    [Header("Inventory Controll")]
    public int currentWeapon = 0; // 0: Melee, 1: Primary, 2: Secondary
    public float speedMultiplier = 1.0f;

    [Header ("Camera")]
    private Camera mainCamera;

    [Header("External forces")]
    public float gravity = -9.81f;
    private Vector3 fallVelocity;

    [Header ("Ground check")]
    private bool isGrounded;
    public float groundDistance = 0.5f;

    [Header ("Verification states")]
    [SerializeField] private MovementState movementState;
    [SerializeField] private WeaponState weaponState;

    [Header("Inverse Kinematics / Aiming")]
    [SerializeField] private float aimingDistance = 10f;
    private float aimingWeight;
    private Vector2 screenCenterPoint;

    private int handsLayerIndex;

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
        CacheComponents();
        LoadKeys();
        Cursor.lockState = CursorLockMode.Locked;
        UpdateAnimatorController();
    }

    void FixedUpdate()
    {
        if (!logicController.Pause)
        {
            PlayerMovement();
            SpeedControl();
            GroundControl();
        }
    }

    void Update()
    {
        if (!logicController.Pause)
        {
            HandleInput();
        }
        ShootPoint();
    }

    private void CacheComponents()
    {

        playerController = GetComponent<CharacterController>();
        playerAnimator = GetComponent<Animator>();
        weaponManager = GetComponentInChildren<WeaponManager>();
        logicController = GameObject.FindWithTag("Logic").GetComponent<LogicController>();

        mainCamera = Camera.main;
        composer = mainCamera.GetComponentInChildren<CinemachineRecomposer>();
    }

    private void HandleInput()
    {
        StateHandler();
        MouseControl();
        PlayerJump();
        DoorController();
        SwitchWeapons();
        HandleWeaponAnimations();
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

    private void StateHandler()
    {
        // Mode - Idle
        if (horizontalInput == 0 && verticalInput == 0 && !Input.GetKey(runKey))
        {
            SetMovementState(MovementState.idle, false, 0);
        }
        // Mode - running
        else if (isGrounded && Input.GetKey(runKey))
        {
            SetMovementState(MovementState.running, true, runSpeed);
        }
        // Mode - walking
        else if (isGrounded && (horizontalInput != 0 || verticalInput != 0))
        {
            SetMovementState(MovementState.walking, false, walkSpeed);
        }
        // Mode - crouching
        else if (isGrounded && Input.GetKey(crouchKey))
        {
            SetMovementState(MovementState.crouching, false, crouchSpeed);
        }
        // Mode - airbone
        else if (!isGrounded) // CAMBIAR
        {
            SetMovementState(MovementState.airbone, true, 0);
        }
    }

    private void SetMovementState(MovementState state, bool run, float speed)
    {
        movementState = state;
        moveSpeed = speed;
        playerAnimator.SetBool("Run", run);
        if (state == MovementState.crouching)
        {
            playerAnimator.SetBool("Crouch", true);
        }
        else
        {
            playerAnimator.SetBool("Crouch", false);
        }
        playerAnimator.ResetTrigger("Jump");
        playerAnimator.SetFloat("XSpeed", moveDirection.x);
        playerAnimator.SetFloat("YSpeed", moveDirection.z);
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

    // For gravity and after jump
    private void SpeedControl()
    {
        fallVelocity.y += gravity * Time.deltaTime;
        playerController.Move(fallVelocity * Time.deltaTime);
    }

    void GroundControl()
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
                        door.destroyKeylessDoor();
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

    #region Mouse

    void MouseControl()
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

    public void ShootPoint()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
        }
    }

    #endregion Mouse

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
        if (weaponManager.weaponPrefabs[weaponIndex] != null)
        {
            currentWeapon = weaponIndex;
            weaponManager.ActivateWeapon(currentWeapon);
            UpdateAnimatorController();
        }
    }

    private void UpdateAnimatorController()
    {
        // Set active weapon object
        GameObject activeWeapon = weaponManager.GetActiveWeapon(currentWeapon);
        if (activeWeapon != null)
        {
            activeWeapon.SetActive(true);
        }

        if (weaponManager != null)
        {
            // Determine if the player is aiming
            bool isAiming = weaponManager.GetActiveWeaponType() != WeaponType.Melee;

            // Update animator layer weight for AimLayer and HandsLayer
            playerAnimator.SetLayerWeight(1, isAiming ? 1f : 0f); // AimLayer

            // Update animator layer weight for HandsLayer based on the current weapon state
            if (weaponState == WeaponState.shooting || weaponState == WeaponState.aiming || weaponState == WeaponState.reload)
            {
                playerAnimator.SetLayerWeight(2, 1f); // HandsLayer
            }
            else
            {
                playerAnimator.SetLayerWeight(2, 0f); // HandsLayer
            }
        }
    }

    private void HandleWeaponAnimations()
    {
        // Melee or no weapon
        if (currentWeapon == 0)
        {
            weaponState = WeaponState.melee;
            if (Input.GetMouseButtonDown(0))
            {
                playerAnimator.SetTrigger("Punch");
            }
        }
        // Has a firearm or weapon
        else
        {
            weaponState = WeaponState.weapon;
            // Shooting
            if (Input.GetMouseButton(0))
            {
                weaponState = WeaponState.shooting;
                playerAnimator.SetBool("Shooting", true);
            }
            else
            {
                playerAnimator.SetBool("Shooting", false);
            }

            // Aiming
            if (Input.GetMouseButton(1))
            {
                weaponState = WeaponState.aiming;
                playerAnimator.SetBool("isAiming", true);
            }
            else
            {
                playerAnimator.SetBool("isAiming", false);
            }

            // Reload
            if (Input.GetKey(reloadKey) && weaponState != WeaponState.reload)
            {
                weaponState = WeaponState.reload;
                playerAnimator.SetTrigger("Reload");
                playerAnimator.SetFloat("ReloadSpeed", speedMultiplier);
                StartCoroutine(ReloadCoroutine(weaponManager.GetCurrentWeaponReloadTime()));
            }

            // Running
            else if (isGrounded && Input.GetKey(runKey))
            {
                playerAnimator.SetBool("Run", true);
            }
        }
    }

    IEnumerator ReloadCoroutine(float reloadTime)
    {
        // Cache AnimatorClipInfo for the reload animation
        AnimatorClipInfo[] clipInfo = playerAnimator.GetCurrentAnimatorClipInfo(0);
        float animationDuration = 0f;
        for (int i = 0; i < clipInfo.Length; i++)
        {
            if (clipInfo[i].clip.name == "Reload") // Replace "Reload" with the name of your reload animation clip
            {
                animationDuration = clipInfo[i].clip.length;
                break;
            }
        }

        // Calculate the speed multiplier based on the duration of the animation clip and the currentWeaponReloadTime
        speedMultiplier = animationDuration / reloadTime;

        yield return new WaitForSeconds(reloadTime);
        playerAnimator.ResetTrigger("Reload");
        weaponState = WeaponState.weapon;

        // Reset the speed of the reload animation
        speedMultiplier = 1.0f;
    }
    #endregion Weapons
}