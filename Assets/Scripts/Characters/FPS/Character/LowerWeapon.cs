

using UnityEngine;
using UnityEngine.InputSystem;

namespace LF.LongForgotten
{
    
    
    
    
    public class LowerWeapon : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Title(label: "References")]
        
        [Tooltip("The character's Animator component.")]
        [SerializeField, NotNull]
        private Animator characterAnimator;
        
        [Tooltip("A WallAvoidance component is required so we can check if the character is facing a wall " +
                 "and lower the weapon automatically. If there's no such component assigned, this will never " +
                 "happen.")]
        [SerializeField]
        private WallAvoidance wallAvoidance;
        
        [Tooltip("The character's InventoryBehaviour component.")]
        [SerializeField, NotNull]
        private InventoryBehaviour inventoryBehaviour;

        [Tooltip("The character's CharacterBehaviour component.")]
        [SerializeField, NotNull]
        private CharacterBehaviour characterBehaviour;

        [Title(label: "Settings")]

        [Tooltip("If true, the lowered state is stopped when the character starts firing.")]
        [SerializeField]
        private bool stopWhileFiring = true;
        
        #endregion
        
        #region FIELDS
        
        
        
        
        
        private bool lowered;
        
        
        
        
        private bool loweredPressed;
        
        #endregion
        
        #region UNITY

        
        
        
        private void Update()
        {
            
            if (characterAnimator == null || characterBehaviour == null || inventoryBehaviour == null)
            {
                
                Log.ReferenceError(this, gameObject);
                
                
                return;
            }

            
            lowered = (loweredPressed || wallAvoidance != null && wallAvoidance.HasWall) && !characterBehaviour.IsAiming() && !characterBehaviour.IsRunning()
                      && !characterBehaviour.IsInspecting() && !characterBehaviour.IsHolstered();

            
            
            if (stopWhileFiring && characterBehaviour.IsHoldingButtonFire())
                lowered = false;
            
            
            var animationData = inventoryBehaviour.GetEquipped().GetComponent<ItemAnimationDataBehaviour>();
            if (animationData == null)
                lowered = false;
            else
            {
                
                if (animationData.GetLowerData() == null)
                    lowered = false;
            }
            
            
            characterAnimator.SetBool(AHashes.Lowered, lowered);
        }

        #endregion
        
        #region GETTERS
        
        
        
        
        
        
        public bool IsLowered() => lowered;
        
        #endregion
        
        #region METHODS

        
        
        
        
        public void Lower(InputAction.CallbackContext context)
        {
            
            if (!characterBehaviour.IsCursorLocked())
                return;

            
            if (characterBehaviour.IsAiming() || characterBehaviour.IsInspecting() || 
                characterBehaviour.IsRunning() || characterBehaviour.IsHolstered())
                return;
			
            
            switch (context)
            {
                
                case {phase: InputActionPhase.Performed}:
                    
                    loweredPressed = !loweredPressed;
                    break;
            }
        }
        
        #endregion
    }
}