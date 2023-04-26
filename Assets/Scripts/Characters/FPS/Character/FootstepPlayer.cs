

using UnityEngine;

namespace LF.LongForgotten
{
    
    
    
    
    
    public class FootstepPlayer : MonoBehaviour
    {
        #region FIELDS SERIALIZED
        
        [Title(label: "References")]

        [Tooltip("The character's Movement Behaviour component.")]
        [SerializeField, NotNull]
        private MovementBehaviour movementBehaviour;

        [Tooltip("The character's Animator component.")]
        [SerializeField, NotNull]
        private Animator characterAnimator;
        
        [Tooltip("The character's footstep-dedicated Audio Source component.")]
        [SerializeField, NotNull]
        private AudioSource audioSource;

        [Title(label: "Settings")]

        [Tooltip("Minimum magnitude of the movement velocity at which the audio clips will start playing.")]
        [SerializeField]
        private float minVelocityMagnitude = 1.0f;
        
        [Title(label: "Audio Clips")]
        
        [Tooltip("The audio clip that is played while walking.")]
        [SerializeField]
        private AudioClip audioClipWalking;

        [Tooltip("The audio clip that is played while running.")]
        [SerializeField]
        private AudioClip audioClipRunning;
        
        #endregion
        
        #region UNITY
        
        
        
        
        private void Awake()
        {
            
            if (audioSource != null)
            {
                
                audioSource.clip = audioClipWalking;
                audioSource.loop = true;   
            }
        }

        
        
        
        private void Update()
        {
            
            if (characterAnimator == null || movementBehaviour == null || audioSource == null)
            {
                
                Log.ReferenceError(this, gameObject);
                
                
                return;
            }
            
            
            if (movementBehaviour.IsGrounded() && movementBehaviour.GetVelocity().sqrMagnitude > minVelocityMagnitude)
            {
                
                audioSource.clip = characterAnimator.GetBool(AHashes.Running) ? audioClipRunning : audioClipWalking;
                
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
            
            else if (audioSource.isPlaying)
                audioSource.Pause();
        }
        
        #endregion
    }
}