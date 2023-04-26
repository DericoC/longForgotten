

using UnityEngine;
using UnityEngine.InputSystem;

namespace LF.LongForgotten
{
    
    
    
    public class LeaningInput : MonoBehaviour
    {
        #region FIELDS SERIALIZED
        
        [Title(label: "References")]
        
        [Tooltip("The character's CharacterBehaviour component.")]
        [SerializeField, NotNull]
        private CharacterBehaviour characterBehaviour;
        
        [Tooltip("The character's Animator component.")]
        [SerializeField, NotNull]
        private Animator characterAnimator;

        #endregion
        
        #region FIELDS
        
        
        
        
        private float leaningInput;
        
        
        
        private bool isLeaning;

        #endregion
        
        #region METHODS
        
        
        
        
        private void Update()
        {
            
            isLeaning = (leaningInput != 0.0f);
            
            
            characterAnimator.SetFloat(AHashes.LeaningInput, leaningInput);
            
            characterAnimator.SetBool(AHashes.Leaning, isLeaning);
        }

        
        
        
        public void Lean(InputAction.CallbackContext context)
        {
            
            if (!characterBehaviour.IsCursorLocked())
            {
                
                leaningInput = 0.0f;
                
                
                return;
            }

            
            leaningInput = context.ReadValue<float>();
        }
        
        #endregion
    }
}