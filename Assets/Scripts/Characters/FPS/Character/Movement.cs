

using System.Collections;
using UnityEngine;

namespace LF.LongForgotten
{
    
    
    
    
    public class Movement : MovementBehaviour
    {
        #region FIELDS SERIALIZED
        
        [Title(label: "Acceleration")]
        
        [Tooltip("How fast the character's speed increases.")]
        [SerializeField]
        private float acceleration = 9.0f;

        [Tooltip("Acceleration value used when the character is in the air. This means either jumping, or falling.")]
        [SerializeField]
        private float accelerationInAir = 3.0f;

        [Tooltip("How fast the character's speed decreases.")]
        [SerializeField]
        private float deceleration = 11.0f;
        
        [Title(label: "Speeds")]

        [Tooltip("The speed of the player while walking.")]
        [SerializeField]
        private float speedWalking = 4.0f;
        
        [Tooltip("How fast the player moves while aiming.")]
        [SerializeField]
        private float speedAiming = 3.2f;
        
        [Tooltip("How fast the player moves while aiming.")]
        [SerializeField]
        private float speedCrouching = 3.5f;

        [Tooltip("How fast the player moves while running."), SerializeField]
        private float speedRunning = 6.8f;
        
        [Title(label: "Walking Multipliers")]
        
        [Tooltip("Value to multiply the walking speed by when the character is moving forward."), SerializeField]
        [Range(0.0f, 1.0f)]
        private float walkingMultiplierForward = 1.0f;

        [Tooltip("Value to multiply the walking speed by when the character is moving sideways.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float walkingMultiplierSideways = 1.0f;

        [Tooltip("Value to multiply the walking speed by when the character is moving backwards.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float walkingMultiplierBackwards = 1.0f;
        
        [Title(label: "Air")]

        [Tooltip("How much control the player has over changes in direction while the character is in the air.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float airControl = 0.8f;

        [Tooltip("The value of the character's gravity. Basically, defines how fast the character falls.")]
        [SerializeField]
        private float gravity = 1.1f;

        [Tooltip("The value of the character's gravity while jumping.")]
        [SerializeField]
        private float jumpGravity = 1.0f;

        [Tooltip("The force of the jump.")]
        [SerializeField]
        private float jumpForce = 100.0f;

        [Tooltip("Force applied to keep the character from flying away while descending slopes.")]
        [SerializeField]
        private float stickToGroundForce = 0.03f;

        [Title(label: "Crouching")]

        [Tooltip("Setting this to false will always block the character from crouching.")]
        [SerializeField]
        private bool canCrouch = true;

        [Tooltip("If true, the character will be able to crouch/un-crouch while falling, which can lead to " +
                 "some slightly interesting results.")]
        [SerializeField, ShowIf(nameof(canCrouch), true)]
        private bool canCrouchWhileFalling = false;

        [Tooltip("If true, the character will be able to jump while crouched too!")]
        [SerializeField, ShowIf(nameof(canCrouch), true)]
        private bool canJumpWhileCrouching = true;

        [Tooltip("Height of the character while crouching.")]
        [SerializeField, ShowIf(nameof(canCrouch), true)]
        private float crouchHeight = 1.0f;
        
        [Tooltip("Mask of possible layers that can cause overlaps when trying to un-crouch. Very important!")]
        [SerializeField, ShowIf(nameof(canCrouch), true)]
        private LayerMask crouchOverlapsMask;

        [Title(label: "Rigidbody Push")]

        [Tooltip("Force applied to other rigidbodies when walking into them. This force is multiplied by the character's " +
                 "velocity, so it is never applied by itself, that's important to note.")]
        [SerializeField]
        private float rigidbodyPushForce = 1.0f;

        #endregion

        #region FIELDS

        
        
        
        private CharacterController controller;

        
        
        
        private CharacterBehaviour playerCharacter;
        
        
        
        private WeaponBehaviour equippedWeapon;

        
        
        
        private float standingHeight;

        
        
        
        private Vector3 velocity;

        
        
        
        private bool isGrounded;
        
        
        
        private bool wasGrounded;

        
        
        
        private bool jumping;
        
        
        
        private bool crouching;

        
        
        
        private float lastJumpTime;
        
        #endregion

        #region UNITY FUNCTIONS

        
        
        
        protected override void Awake()
        {
            
            playerCharacter = ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();
        }
        
        protected override void Start()
        {
            
            controller = GetComponent<CharacterController>();
            
            
            standingHeight = controller.height;
        }

        
        protected override void Update()
        {
            
            equippedWeapon = playerCharacter.GetInventory().GetEquipped();

            
            isGrounded = IsGrounded();
            
            if (isGrounded && !wasGrounded)
            {
                
                jumping = false;
                
                lastJumpTime = 0.0f;
            }
            else if (wasGrounded && !isGrounded)
                lastJumpTime = Time.time;
            
            
            MoveCharacter();
            
            wasGrounded = isGrounded;
        }
        
        
        
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            
            if (hit.moveDirection.y > 0.0f && velocity.y > 0.0f)
                velocity.y = 0.0f;

            
            Rigidbody hitRigidbody = hit.rigidbody;
            if (hitRigidbody == null)
                return;
            
            
            Vector3 force = (hit.moveDirection + Vector3.up * 0.35f) * velocity.magnitude * rigidbodyPushForce;
            hitRigidbody.AddForceAtPosition(force, hit.point);
        }
        
        #endregion

        #region METHODS

        
        
        
        private void MoveCharacter()
        {
            
            Vector2 frameInput = Vector3.ClampMagnitude(playerCharacter.GetInputMovement(), 1.0f);
            
            var desiredDirection = new Vector3(frameInput.x, 0.0f, frameInput.y);
            
            
            if(playerCharacter.IsRunning())
                desiredDirection *= speedRunning;
            else
            {
                
                if (crouching)
                    desiredDirection *= speedCrouching;
                else
                {
                    
                    if (playerCharacter.IsAiming())
                        desiredDirection *= speedAiming;
                    else
                    {
                        
                        desiredDirection *= speedWalking;
                        
                        desiredDirection.x *= walkingMultiplierSideways;
                        
                        desiredDirection.z *=
                            (frameInput.y > 0 ? walkingMultiplierForward : walkingMultiplierBackwards);
                    }
                }
            } 

            
            desiredDirection = transform.TransformDirection(desiredDirection);
            
            if (equippedWeapon != null)
                desiredDirection *= equippedWeapon.GetMultiplierMovementSpeed();
            
            
            if (isGrounded == false)
            {
                
                if (wasGrounded && !jumping)
                    velocity.y = 0.0f;
                
                
                velocity += desiredDirection * (accelerationInAir * airControl * Time.deltaTime);
                
                velocity.y -= (velocity.y >= 0 ? jumpGravity : gravity) * Time.deltaTime;
            }
            
            else if(!jumping)
            {
                
                velocity = Vector3.Lerp(velocity, new Vector3(desiredDirection.x, velocity.y, desiredDirection.z), Time.deltaTime * (desiredDirection.sqrMagnitude > 0.0f ? acceleration : deceleration));
            }

            
            Vector3 applied = velocity * Time.deltaTime;
            
            if (controller.isGrounded && !jumping)
                applied.y = -stickToGroundForce;

            
            controller.Move(applied);
        }

        
        
        
        public override bool WasGrounded() => wasGrounded;
        
        
        
        public override bool IsJumping() => jumping;

        
        
        
        public override bool CanCrouch(bool newCrouching)
        {
            
            if (canCrouch == false)
                return false;

            
            if (isGrounded == false && canCrouchWhileFalling == false)
                return false;
            
            
            if (newCrouching)
                return true;

            
            Vector3 sphereLocation = transform.position + Vector3.up * standingHeight;
            
            return (Physics.OverlapSphere(sphereLocation, controller.radius, crouchOverlapsMask).Length == 0);
        }

        
        
        
        
        public override bool IsCrouching() => crouching;

        
        
        
        public override void Jump()
        {
            
            if (crouching && !canJumpWhileCrouching)
                return;
            
            
            if (!isGrounded)
                return;

            
            jumping = true;
            
            velocity = new Vector3(velocity.x, Mathf.Sqrt(2.0f * jumpForce * jumpGravity), velocity.z);

            
            lastJumpTime = Time.time;
        }
        
        
        
        public override void Crouch(bool newCrouching)
        {
            
            crouching = newCrouching;
            
            
            controller.height = crouching ? crouchHeight : standingHeight;
            
            controller.center = controller.height / 2.0f * Vector3.up;
        }

        public override void TryCrouch(bool value)
        {
            
            if (value && CanCrouch(true))
                Crouch(true);
            
            else if(!value)
                StartCoroutine(nameof(TryUncrouch));
        }

        
        
        
        public override void TryToggleCrouch() => TryCrouch(!crouching);
        
        
        
        private IEnumerator TryUncrouch()
        {
            
            
            yield return new WaitUntil(() => CanCrouch(false));
            
            
            Crouch(false);
        }

        #endregion

        #region GETTERS

        
        
        
        public override float GetLastJumpTime() => lastJumpTime;

        
        
        
        public override float GetMultiplierForward() => walkingMultiplierForward;
        
        
        
        public override float GetMultiplierSideways() => walkingMultiplierSideways;
        
        
        
        public override float GetMultiplierBackwards() => walkingMultiplierBackwards;
        
        
        
        
        public override Vector3 GetVelocity() => controller.velocity;
        
        
        
        public override bool IsGrounded() => controller.isGrounded;

        #endregion
    }
}