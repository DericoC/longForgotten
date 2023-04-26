using System;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using System.Threading.Tasks;

namespace LF.LongForgotten
{
    [RequireComponent(typeof(CharacterKinematics))]
    public sealed class Character : CharacterBehaviour
    {
        #region FIELDS SERIALIZED

        [Title(label: "References")]

        [Tooltip("The character's LowerWeapon component.")]
        [SerializeField]
        private LowerWeapon lowerWeapon;

        [Title(label: "Inventory")]

        [Tooltip("Determines the index of the weapon to equip when the game starts.")]
        [SerializeField]
        private int weaponIndexEquippedAtStart;

        [Tooltip("Inventory.")]
        [SerializeField]
        private InventoryBehaviour inventory;

        [Title(label: "Grenade")]

        [Tooltip("If true, the character's grenades will never run out.")]
        [SerializeField]
        private bool grenadesUnlimited;

        [Tooltip("Total amount of grenades at start.")]
        [SerializeField]
        private int grenadeTotal = 10;

        [Tooltip("Grenade spawn offset from the character's camera.")]
        [SerializeField]
        private float grenadeSpawnOffset = 1.0f;

        [Tooltip("Grenade Prefab. Spawned when throwing a grenade.")]
        [SerializeField]
        private GameObject grenadePrefab;

        [Title(label: "Knife")]

        [Tooltip("Knife GameObject.")]
        [SerializeField]
        private GameObject knife;

        [Title(label: "Cameras")]

        [Tooltip("Normal Camera.")]
        [SerializeField]
        private Camera cameraWorld;

        [Tooltip("Weapon-Only Camera. Depth.")]
        [SerializeField]
        private Camera cameraDepth;

        [Title(label: "Animation")]

        [Tooltip("Determines how smooth the turning animation is.")]
        [SerializeField]
        private float dampTimeTurning = 0.4f;

        [Tooltip("Determines how smooth the locomotion blendspace is.")]
        [SerializeField]
        private float dampTimeLocomotion = 0.15f;

        [Tooltip("How smoothly we play aiming transitions. Beware that this affects lots of things!")]
        [SerializeField]
        private float dampTimeAiming = 0.3f;

        [Tooltip("Interpolation speed for the running offsets.")]
        [SerializeField]
        private float runningInterpolationSpeed = 12.0f;

        [Tooltip("Determines how fast the character's weapons are aimed.")]
        [SerializeField]
        private float aimingSpeedMultiplier = 1.0f;

        [Title(label: "Animation Procedural")]

        [Tooltip("Character Animator.")]
        [SerializeField]
        private Animator characterAnimator;

        [Tooltip("Zombies Spawn Controller.")]
        [SerializeField]
        private SpawnerController controllerSpawn;

        [Title(label: "Field Of View")]

        [Tooltip("Normal world field of view.")]
        [SerializeField]
        private float fieldOfView = 100.0f;

        [Tooltip("Multiplier for the field of view while running.")]
        [SerializeField]
        private float fieldOfViewRunningMultiplier = 1.05f;

        [Tooltip("Weapon-specific field of view.")]
        [SerializeField]
        private float fieldOfViewWeapon = 55.0f;

        [Title(label: "Audio Clips")]

        [Tooltip("Melee Audio Clips.")]
        [SerializeField]
        private AudioClip[] audioClipsMelee;

        [Tooltip("Grenade Throw Audio Clips.")]
        [SerializeField]
        private AudioClip[] audioClipsGrenadeThrow;

        [Title(label: "Input Options")]

        [Tooltip("If true, the running input has to be held to be active.")]
        [SerializeField]
        private bool holdToRun = true;

        [Tooltip("If true, the aiming input has to be held to be active.")]
        [SerializeField]
        private bool holdToAim = true;

        #endregion

        #region FIELDS

        private bool[] keys;
        private bool isPressingDoorOpen = false;        private bool aiming;        private bool wasAiming;        private bool running;        private bool holstered;

        private bool changeShot = false;
        private float lastShotTime;
        private int layerOverlay;        private int layerHolster;        private int layerActions;

        private int round;
        private MovementBehaviour movementBehaviour;
        private WeaponBehaviour equippedWeapon;        private WeaponAttachmentManagerBehaviour weaponAttachmentManager;
        private ScopeBehaviour equippedWeaponScope;        private MagazineBehaviour equippedWeaponMagazine;
        private bool reloading;
        private bool inspecting;        private bool throwingGrenade;
        private bool meleeing;
        private bool holstering;
        private float aimingAlpha;
        private float crouchingAlpha;        private float runningAlpha;
        private Vector2 axisLook;
        private Vector2 axisMovement;
        private bool bolting;
        private int grenadeCount;
        private bool holdingButtonAim;        private bool holdingButtonRun;        private bool holdingButtonFire;
        private bool tutorialTextVisible;
        private bool cursorLocked;        private int shotsFired;

        #endregion

        #region UNITY
        protected override void Awake()
        {
            #region Lock Cursor


            cursorLocked = true;

            UpdateCursorState();

            #endregion


            movementBehaviour = GetComponent<MovementBehaviour>();


            inventory.Init(weaponIndexEquippedAtStart);


            RefreshWeaponSetup();
        }        protected override void Start()
        {

            grenadeCount = grenadeTotal;


            if (knife != null)
                knife.SetActive(false);


            layerHolster = characterAnimator.GetLayerIndex("Layer Holster");

            layerActions = characterAnimator.GetLayerIndex("Layer Actions");

            layerOverlay = characterAnimator.GetLayerIndex("Layer Overlay");
        }
        protected override void Update()
        {

            aiming = holdingButtonAim && CanAim();

            running = holdingButtonRun && CanRun();

            controllerSpawn = FindObjectOfType<SpawnerController>();


            switch (aiming)
            {

                case true when !wasAiming:
                    equippedWeaponScope.OnAim();
                    break;

                case false when wasAiming:
                    equippedWeaponScope.OnAimStop();
                    break;
            }


            if (holdingButtonFire)
            {

                if (CanPlayAnimationFire() && equippedWeapon.HasAmmunition() && equippedWeapon.IsAutomatic())
                {

                    if (Time.time - lastShotTime > 60.0f / equippedWeapon.GetRateOfFire())
                        Fire();
                }
                else
                {


                    shotsFired = 0;
                }
            }


            UpdateAnimator();
            DoorController();

            round = controllerSpawn.CurrentRound;
            if (round == 5)
            {
                if (changeShot == false)
                {
                    StartCoroutine(nameof(Equip), 1);
            changeShot = true;
        }
    }


    aimingAlpha = characterAnimator.GetFloat(AHashes.AimingAlpha);


            crouchingAlpha = Mathf.Lerp(crouchingAlpha, movementBehaviour.IsCrouching() ? 1.0f : 0.0f, Time.deltaTime * 12.0f);

            runningAlpha = Mathf.Lerp(runningAlpha, running ? 1.0f : 0.0f, Time.deltaTime * runningInterpolationSpeed);


            float runningFieldOfView = Mathf.Lerp(1.0f, fieldOfViewRunningMultiplier, runningAlpha);


            cameraWorld.fieldOfView = Mathf.Lerp(fieldOfView, fieldOfView * equippedWeapon.GetFieldOfViewMultiplierAim(), aimingAlpha) * runningFieldOfView;

            cameraDepth.fieldOfView = Mathf.Lerp(fieldOfViewWeapon, fieldOfViewWeapon * equippedWeapon.GetFieldOfViewMultiplierAimWeapon(), aimingAlpha);


            wasAiming = aiming;
        }

        #endregion

        #region GETTERS
        public override int GetShotsFired() => shotsFired;
        public override bool IsLowered()
        {

            if (lowerWeapon == null)
                return false;


            return lowerWeapon.IsLowered();
        }
        public override Camera GetCameraWorld() => cameraWorld;
        public override Camera GetCameraDepth() => cameraDepth;
        public override InventoryBehaviour GetInventory() => inventory;
        public override int GetGrenadesCurrent() => grenadeCount;        public override int GetGrenadesTotal() => grenadeTotal;

        public override bool IsRunning() => running;        public override bool IsHolstered() => holstered;
        public override bool IsCrouching() => movementBehaviour.IsCrouching();
        public override bool IsReloading() => reloading;
        public override bool IsThrowingGrenade() => throwingGrenade;

        public override bool IsMeleeing() => meleeing;
        public override bool IsAiming() => aiming;        public override bool IsCursorLocked() => cursorLocked;
        public override bool IsTutorialTextVisible() => tutorialTextVisible;
        public override Vector2 GetInputMovement() => axisMovement;        public override Vector2 GetInputLook() => axisLook;
        public override AudioClip[] GetAudioClipsGrenadeThrow() => audioClipsGrenadeThrow;        public override AudioClip[] GetAudioClipsMelee() => audioClipsMelee;
        public override bool IsInspecting() => inspecting;        public override bool IsHoldingButtonFire() => holdingButtonFire;

        #endregion

        #region METHODS
        private void UpdateAnimator()
        {
            #region Reload Stop


            const string boolNameReloading = "Reloading";
            if (characterAnimator.GetBool(boolNameReloading))
            {

                if (equippedWeapon.GetAmmunitionTotal() - equippedWeapon.GetAmmunitionCurrent() < 1)
                {

                    characterAnimator.SetBool(boolNameReloading, false);

                    equippedWeapon.GetAnimator().SetBool(boolNameReloading, false);
                }
            }

            #endregion


            float leaningValue = Mathf.Clamp01(axisMovement.y);
            characterAnimator.SetFloat(AHashes.LeaningForward, leaningValue, 0.5f, Time.deltaTime);


            float movementValue = Mathf.Clamp01(Mathf.Abs(axisMovement.x) + Mathf.Abs(axisMovement.y));
            characterAnimator.SetFloat(AHashes.Movement, movementValue, dampTimeLocomotion, Time.deltaTime);


            characterAnimator.SetFloat(AHashes.AimingSpeedMultiplier, aimingSpeedMultiplier);


            characterAnimator.SetFloat(AHashes.Turning, Mathf.Abs(axisLook.x), dampTimeTurning, Time.deltaTime);


            characterAnimator.SetFloat(AHashes.Horizontal, axisMovement.x, dampTimeLocomotion, Time.deltaTime);

            characterAnimator.SetFloat(AHashes.Vertical, axisMovement.y, dampTimeLocomotion, Time.deltaTime);


            characterAnimator.SetFloat(AHashes.AimingAlpha, Convert.ToSingle(aiming), dampTimeAiming, Time.deltaTime);


            const string playRateLocomotionBool = "Play Rate Locomotion";
            characterAnimator.SetFloat(playRateLocomotionBool, movementBehaviour.IsGrounded() ? 1.0f : 0.0f, 0.2f, Time.deltaTime);

            #region Movement Play Rates


            characterAnimator.SetFloat(AHashes.PlayRateLocomotionForward, movementBehaviour.GetMultiplierForward(), 0.2f, Time.deltaTime);

            characterAnimator.SetFloat(AHashes.PlayRateLocomotionSideways, movementBehaviour.GetMultiplierSideways(), 0.2f, Time.deltaTime);

            characterAnimator.SetFloat(AHashes.PlayRateLocomotionBackwards, movementBehaviour.GetMultiplierBackwards(), 0.2f, Time.deltaTime);

            #endregion


            characterAnimator.SetBool(AHashes.Aim, aiming);

            characterAnimator.SetBool(AHashes.Running, running);

            characterAnimator.SetBool(AHashes.Crouching, movementBehaviour.IsCrouching());
        }        private void Inspect()
        {

            inspecting = true;

            characterAnimator.CrossFade("Inspect", 0.0f, layerActions, 0);
        }        private void Fire()
        {


            shotsFired++;


            lastShotTime = Time.time;

            equippedWeapon.Fire(aiming ? equippedWeaponScope.GetMultiplierSpread() : 1.0f);


            const string stateName = "Fire";
            characterAnimator.CrossFade(stateName, 0.05f, layerOverlay, 0);


            if (equippedWeapon.IsBoltAction() && equippedWeapon.HasAmmunition())
                UpdateBolt(true);


            if (!equippedWeapon.HasAmmunition() && equippedWeapon.GetAutomaticallyReloadOnEmpty())
                StartCoroutine(nameof(TryReloadAutomatic));
        }
        private void PlayReloadAnimation()
        {
            #region Animation


            string stateName = equippedWeapon.HasCycledReload() ? "Reload Open" :
                (equippedWeapon.HasAmmunition() ? "Reload" : "Reload Empty");


            characterAnimator.Play(stateName, layerActions, 0.0f);

            #endregion


            characterAnimator.SetBool(AHashes.Reloading, reloading = true);


            equippedWeapon.Reload();
        }        private IEnumerator TryReloadAutomatic()
        {

            yield return new WaitForSeconds(equippedWeapon.GetAutomaticallyReloadOnEmptyDelay());


            PlayReloadAnimation();
        }
        private IEnumerator Equip(int index = 0)
        {

            if (!holstered)
            {

                SetHolstered(holstering = true);

                yield return new WaitUntil(() => holstering == false);
            }

            SetHolstered(false);

            characterAnimator.Play("Unholster", layerHolster, 0);


            inventory.Equip(index);

            RefreshWeaponSetup();
        }        private void RefreshWeaponSetup()
        {

            if ((equippedWeapon = inventory.GetEquipped()) == null)
                return;


            characterAnimator.runtimeAnimatorController = equippedWeapon.GetAnimatorController();


            weaponAttachmentManager = equippedWeapon.GetAttachmentManager();
            if (weaponAttachmentManager == null)
                return;


            equippedWeaponScope = weaponAttachmentManager.GetEquippedScope();

            equippedWeaponMagazine = weaponAttachmentManager.GetEquippedMagazine();
        }

        private void FireEmpty()
        {

            lastShotTime = Time.time;

            characterAnimator.CrossFade("Fire Empty", 0.05f, layerOverlay, 0);
        }        private void UpdateCursorState()
        {

            Cursor.visible = !cursorLocked;

            Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
        }
        private void PlayGrenadeThrow()
        {

            throwingGrenade = true;


            characterAnimator.CrossFade("Grenade Throw", 0.15f,
                characterAnimator.GetLayerIndex("Layer Actions Arm Left"), 0.0f);


            characterAnimator.CrossFade("Grenade Throw", 0.05f,
                characterAnimator.GetLayerIndex("Layer Actions Arm Right"), 0.0f);
        }        private void PlayMelee()
        {

            meleeing = true;


            characterAnimator.CrossFade("Knife Attack", 0.05f,
                characterAnimator.GetLayerIndex("Layer Actions Arm Left"), 0.0f);


            characterAnimator.CrossFade("Knife Attack", 0.05f,
                characterAnimator.GetLayerIndex("Layer Actions Arm Right"), 0.0f);
        }
        private void UpdateBolt(bool value)
        {

            characterAnimator.SetBool(AHashes.Bolt, bolting = value);
        }        private void SetHolstered(bool value = true)
        {

            holstered = value;


            const string boolName = "Holstered";
            characterAnimator.SetBool(boolName, holstered);
        }

        #region ACTION CHECKS
        private bool CanPlayAnimationFire()
        {

            if (holstered || holstering)
                return false;


            if (meleeing || throwingGrenade)
                return false;


            if (reloading || bolting)
                return false;


            if (inspecting)
                return false;


            return true;
        }
        private bool CanPlayAnimationReload()
        {

            if (reloading)
                return false;


            if (meleeing)
                return false;


            if (bolting)
                return false;


            if (throwingGrenade)
                return false;


            if (inspecting)
                return false;


            if (!equippedWeapon.CanReloadWhenFull() && equippedWeapon.IsFull())
                return false;


            return true;
        }
        private bool CanPlayAnimationGrenadeThrow()
        {

            if (holstered || holstering)
                return false;


            if (meleeing || throwingGrenade)
                return false;


            if (reloading || bolting)
                return false;


            if (inspecting)
                return false;


            if (!grenadesUnlimited && grenadeCount == 0)
                return false;


            return true;
        }
        private bool CanPlayAnimationMelee()
        {

            if (holstered || holstering)
                return false;


            if (meleeing || throwingGrenade)
                return false;


            if (reloading || bolting)
                return false;


            if (inspecting)
                return false;


            return true;
        }

        private bool CanPlayAnimationHolster()
        {

            if (meleeing || throwingGrenade)
                return false;


            if (reloading || bolting)
                return false;


            if (inspecting)
                return false;


            return true;
        }

        private bool CanChangeWeapon()
        {

            if (holstering)
                return false;


            if (meleeing || throwingGrenade)
                return false;


            if (reloading || bolting)
                return false;


            if (inspecting)
                return false;


            return true;
        }
        private bool CanPlayAnimationInspect()
        {

            if (holstered || holstering)
                return false;


            if (meleeing || throwingGrenade)
                return false;


            if (reloading || bolting)
                return false;


            if (inspecting)
                return false;


            return true;
        }

        private bool CanAim()
        {

            if (holstered || inspecting)
                return false;


            if (meleeing || throwingGrenade)
                return false;


            if ((!equippedWeapon.CanReloadAimed() && reloading) || holstering)
                return false;


            return true;
        }

        private bool CanRun()
        {

            if (inspecting || bolting)
                return false;


            if (movementBehaviour.IsCrouching())
                return false;


            if (meleeing || throwingGrenade)
                return false;


            if (reloading || aiming)
                return false;


            if (holdingButtonFire && equippedWeapon.HasAmmunition())
                return false;


            if (axisMovement.y <= 0 || Math.Abs(Mathf.Abs(axisMovement.x) - 1) < 0.01f)
                return false;


            return true;
        }

        #endregion

        #region INPUT
        public void OnTryFire(InputAction.CallbackContext context)
        {

            if (!cursorLocked)
                return;


            switch (context)
            {

                case { phase: InputActionPhase.Started }:

                    holdingButtonFire = true;


                    shotsFired = 0;
                    break;

                case { phase: InputActionPhase.Performed }:

                    if (!CanPlayAnimationFire())
                        break;


                    if (equippedWeapon.HasAmmunition())
                    {

                        if (equippedWeapon.IsAutomatic())
                        {


                            shotsFired = 0;


                            break;
                        }


                        if (Time.time - lastShotTime > 60.0f / equippedWeapon.GetRateOfFire())
                            Fire();
                    }

                    else
                        FireEmpty();
                    break;

                case { phase: InputActionPhase.Canceled }:

                    holdingButtonFire = false;


                    shotsFired = 0;
                    break;
            }
        }        public void OnTryPlayReload(InputAction.CallbackContext context)
        {

            if (!cursorLocked)
                return;


            if (!CanPlayAnimationReload())
                return;


            switch (context)
            {

                case { phase: InputActionPhase.Performed }:

                    PlayReloadAnimation();
                    break;
            }
        }
        public void OnTryInspect(InputAction.CallbackContext context)
        {

            if (!cursorLocked)
                return;


            if (!CanPlayAnimationInspect())
                return;


            switch (context)
            {

                case { phase: InputActionPhase.Performed }:

                    Inspect();
                    break;
            }
        }        public void OnTryAiming(InputAction.CallbackContext context)
        {

            if (!cursorLocked)
                return;


            switch (context.phase)
            {

                case InputActionPhase.Started:

                    if (holdToAim)
                        holdingButtonAim = true;
                    break;

                case InputActionPhase.Performed:

                    if (!holdToAim)
                        holdingButtonAim = !holdingButtonAim;
                    break;

                case InputActionPhase.Canceled:

                    if (holdToAim)
                        holdingButtonAim = false;
                    break;
            }
        }
        public void OnTryHolster(InputAction.CallbackContext context)
        {

            if (!cursorLocked)
                return;


            if (!CanPlayAnimationHolster())
                return;


            switch (context.phase)
            {

                case InputActionPhase.Started:

                    if (holstered)
                    {

                        SetHolstered(false);

                        holstering = true;
                    }
                    break;

                case InputActionPhase.Performed:

                    SetHolstered(!holstered);

                    holstering = true;
                    break;
            }
        }        public void OnTryThrowGrenade(InputAction.CallbackContext context)
        {

            if (!cursorLocked)
                return;


            switch (context.phase)
            {

                case InputActionPhase.Performed:

                    if (CanPlayAnimationGrenadeThrow())
                        PlayGrenadeThrow();
                    break;
            }
        }
        public void OnTryMelee(InputAction.CallbackContext context)
        {

            if (!cursorLocked)
                return;


            switch (context.phase)
            {

                case InputActionPhase.Performed:

                    if (CanPlayAnimationMelee())
                        PlayMelee();
                    break;
            }
        }        public void OnTryRun(InputAction.CallbackContext context)
        {

            if (!cursorLocked)
                return;


            switch (context.phase)
            {

                case InputActionPhase.Performed:

                    if (!holdToRun)
                        holdingButtonRun = !holdingButtonRun;
                    break;

                case InputActionPhase.Started:

                    if (holdToRun)
                        holdingButtonRun = true;
                    break;

                case InputActionPhase.Canceled:

                    if (holdToRun)
                        holdingButtonRun = false;
                    break;
            }
        }
        public void OnTryJump(InputAction.CallbackContext context)
        {

            if (!cursorLocked)
                return;


            switch (context.phase)
            {

                case InputActionPhase.Performed:

                    movementBehaviour.Jump();
                    break;
            }
        }        public void OnTryInventoryNext(InputAction.CallbackContext context)
        {

            if (!cursorLocked)
                return;


            if (inventory == null)
                return;


            if (round >= 5)
            {
                switch (context)
                {

                    case { phase: InputActionPhase.Performed }:


                        float scrollValue = context.valueType.IsEquivalentTo(typeof(Vector2)) ? Mathf.Sign(context.ReadValue<Vector2>().y) : 1.0f;


                        int indexNext = scrollValue > 0 ? inventory.GetNextIndex() : inventory.GetLastIndex();

                        int indexCurrent = inventory.GetEquippedIndex();


                        if (CanChangeWeapon() && (indexCurrent != indexNext))
                            StartCoroutine(nameof(Equip), indexNext);
                        break;
                }
            }
        }

        public void OnLockCursor(InputAction.CallbackContext context)
        {

            switch (context)
            {

                case { phase: InputActionPhase.Performed }:

                    cursorLocked = !cursorLocked;

                    UpdateCursorState();
                    break;
            }
        }
        public void OnMove(InputAction.CallbackContext context)
        {

            axisMovement = cursorLocked ? context.ReadValue<Vector2>() : default;
        }        public void OnLook(InputAction.CallbackContext context)
        {

            axisLook = cursorLocked ? context.ReadValue<Vector2>() : default;


            if (equippedWeapon == null)
                return;


            if (equippedWeaponScope == null)
                return;


            axisLook *= aiming ? equippedWeaponScope.GetMultiplierMouseSensitivity() : 1.0f;
        }
        public void OnUpdateTutorial(InputAction.CallbackContext context)
        {

            tutorialTextVisible = context switch
            {

                { phase: InputActionPhase.Started } => true,

                { phase: InputActionPhase.Canceled } => false,

                _ => tutorialTextVisible
            };
        }

        #endregion

        #region ANIMATION EVENTS
        public override void EjectCasing()
        {

            if (equippedWeapon != null)
                equippedWeapon.EjectCasing();
        }        public override void FillAmmunition(int amount)
        {

            if (equippedWeapon != null)
                equippedWeapon.FillAmmunition(amount);
        }        public override void Grenade()
        {

            if (grenadePrefab == null)
                return;


            if (cameraWorld == null)
                return;


            if (!grenadesUnlimited)
                grenadeCount--;


            Transform cTransform = cameraWorld.transform;

            Vector3 position = cTransform.position;
            position += cTransform.forward * grenadeSpawnOffset;

            Instantiate(grenadePrefab, position, cTransform.rotation);
        }        public override void SetActiveMagazine(int active)
        {

            equippedWeaponMagazine.gameObject.SetActive(active != 0);
        }
        public override void AnimationEndedBolt()
        {

            UpdateBolt(false);
        }        public override void AnimationEndedReload()
        {

            reloading = false;
        }
        public override void AnimationEndedGrenadeThrow()
        {

            throwingGrenade = false;
        }        public override void AnimationEndedMelee()
        {

            meleeing = false;
        }
        public override void AnimationEndedInspect()
        {

            inspecting = false;
        }        public override void AnimationEndedHolster()
        {

            holstering = false;
        }
        public override void SetSlideBack(int back)
        {

            if (equippedWeapon != null)
                equippedWeapon.SetSlideBack(back);
        }
        public override void SetActiveKnife(int active)
        {

            knife.SetActive(active != 0);
        }

        #endregion

        #endregion

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
            if (Input.GetKeyDown(KeyCode.F))
            {
                isPressingDoorOpen = true;
                MaintainDoorInteraction(1f);
            }
        }
        #endregion Doors

        // Getters / Setters
        public bool CursorLocked { get => cursorLocked; set => cursorLocked = value; }
    }
}