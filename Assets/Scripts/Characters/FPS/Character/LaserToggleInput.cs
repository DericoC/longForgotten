

using UnityEngine;
using UnityEngine.InputSystem;

namespace LF.LongForgotten
{
    
    
    
    
    public class LaserToggleInput : MonoBehaviour
    {
        #region FIELDS SERIALIZED
        
        [Title(label: "References")]

        [Tooltip("The character's Animator component.")]
        [SerializeField, NotNull]
        private Animator animator;
        
        [Tooltip("The character's InventoryBehaviour component.")]
        [SerializeField, NotNull]
        private InventoryBehaviour inventoryBehaviour;

        #endregion
        
        #region FIELDS
        
        
        
        
        private LaserBehaviour laserBehaviour;

        
        
        
        private bool wasAiming;
        
        
        
        private bool wasRunning;
        
        #endregion
        
        #region METHODS

        
        
        
        private void Update()
        {
            
            if (animator == null || inventoryBehaviour == null)
            {
                
                Log.ReferenceError(this, gameObject);
                
                
                return;
            }
            
            
            WeaponBehaviour weaponBehaviour = inventoryBehaviour.GetEquipped();
            if (weaponBehaviour == null)
                return;

            
            laserBehaviour = weaponBehaviour.GetAttachmentManager().GetEquippedLaser();
            if (laserBehaviour == null)
                return;
            
            
            bool aiming = animator.GetBool(AHashes.Aim);
            
            bool running = animator.GetBool(AHashes.Running);
            
            
            if (aiming && !wasAiming)
            {
                
                if(laserBehaviour.GetTurnOffWhileAiming())
                    laserBehaviour.Hide();
            }
            
            else if (!aiming && wasAiming)
            {
                
                if(laserBehaviour.GetTurnOffWhileAiming())
                    laserBehaviour.Reapply();
            }

            
            if (running && !wasRunning)
            {
                
                if (laserBehaviour.GetTurnOffWhileRunning())
                    laserBehaviour.Hide();
            }
            
            else if (!running && wasRunning)
            {
                
                if (laserBehaviour.GetTurnOffWhileRunning())
                    laserBehaviour.Reapply();
            }

            
            wasAiming = aiming;
            
            wasRunning = running;
        }

        
        
        
        public void Input(InputAction.CallbackContext context)
        {
            
            switch (context)
            {
                
                case {phase: InputActionPhase.Performed}:
                    
                    Toggle();
                    break;
            }
        }

        
        
        
        private void Toggle()
        {
			
			if(laserBehaviour == null)
				return;
			
            
            laserBehaviour.Toggle();
        }
        
        #endregion
    }
}