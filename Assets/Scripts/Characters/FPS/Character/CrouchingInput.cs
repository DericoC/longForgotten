

using UnityEngine;
using UnityEngine.InputSystem;

namespace LF.LongForgotten
{public class CrouchingInput : MonoBehaviour
    {
        #region FIELDS SERIALIZED

        [Title(label: "References")]

        [Tooltip("The character's CharacterBehaviour component.")]
        [SerializeField, NotNull]
        private CharacterBehaviour characterBehaviour;
        
        [Tooltip("The character's MovementBehaviour component.")]
        [SerializeField, NotNull]
        private MovementBehaviour movementBehaviour;

        [Title(label: "Settings")]

        [Tooltip("If true, the crouch button has to be held to keep crouching.")]
        [SerializeField]
        private bool holdToCrouch;
        
        #endregion

        #region FIELDS

        
        
        
        private bool holding;

        #endregion

        #region UNITY

        
        
        
        private void Update()
        {
            
            
            if(holdToCrouch)
                movementBehaviour.TryCrouch(holding);
        }

        #endregion
        
        #region INPUT

        
        
        
        
        
        public void Crouch(InputAction.CallbackContext context)
        {
            
            if (characterBehaviour == null || movementBehaviour == null)
            {
                
                Log.ReferenceError(this, this.gameObject);

                
                return;
            }
            
            
            if (!characterBehaviour.IsCursorLocked())
                return;

            
            switch (context.phase)
            {
                
                case InputActionPhase.Started:
                    holding = true;
                    break;
                
                case InputActionPhase.Performed:
                    
                    if(!holdToCrouch)
                        movementBehaviour.TryToggleCrouch();
                    break;
                
                case InputActionPhase.Canceled:
                    holding = false;
                    break;
            }
        }

        #endregion
    }
}