

using UnityEngine;

namespace LF.LongForgotten
{
    
    
    
    public abstract class MovementBehaviour : MonoBehaviour
    {
        #region UNITY

        
        
        
        protected virtual void Awake(){}

        
        
        
        protected virtual void Start(){}

        
        
        
        protected virtual void Update(){}

        
        
        
        protected virtual void FixedUpdate(){}

        
        
        
        protected virtual void LateUpdate(){}

        #endregion

        #region GETTERS

        
        
        
        public abstract float GetLastJumpTime();
        
        
        
        
        
        public abstract float GetMultiplierForward();
        
        
        
        
        public abstract float GetMultiplierSideways();
        
        
        
        
        public abstract float GetMultiplierBackwards();

        
        
        
        public abstract Vector3 GetVelocity();
        
        
        
        public abstract bool IsGrounded();
        
        
        
        public abstract bool WasGrounded();
        
        
        
        
        public abstract bool IsJumping();

        
        
        
        public abstract bool CanCrouch(bool newCrouching);
        
        
        
        public abstract bool IsCrouching();

        #endregion

        #region METHODS

        
        
        
        public abstract void Jump();
        
        
        
        public abstract void Crouch(bool crouching);

        
        
        
        public abstract void TryCrouch(bool value);
        
        
        
        
        
        public abstract void TryToggleCrouch();

        #endregion
    }
}