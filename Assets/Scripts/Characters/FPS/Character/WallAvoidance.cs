

using UnityEngine;

namespace LF.LongForgotten
{
    
    
    
    public class WallAvoidance : MonoBehaviour
    {
        #region PROPERTIES
        
        
        
        
        public bool HasWall => hasWall;

        #endregion
        
        #region FIELDS SERIALIZED
        
        [Title(label: "References")]
        
        [Tooltip("The Transform of the character's camera.")]
        [SerializeField, NotNull]
        private Transform playerCamera;
        
        [Title(label: "Settings")]
        
        [Tooltip("The maximum distance to check for walls.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float distance = 1.0f;
        
        [Tooltip("The radius of the sphere check.")]
        [Range(0.0f, 2.0f)]
        [SerializeField]
        private float radius = 0.5f;

        [Tooltip("The layers to count as wall layers.")]
        [SerializeField]
        private LayerMask layerMask;
        
        #endregion

        #region FIELDS
        
        
        
        
        private bool hasWall;
        
        #endregion
        
        #region METHODS
        
        
        
        
        private void Update()
        {
            
            if (playerCamera == null)
            {
                
                Log.ReferenceError(this, gameObject);

                
                return;
            }
            
            
            var ray = new Ray(playerCamera.position, playerCamera.forward);
            
            hasWall = Physics.SphereCast(ray, radius, distance, layerMask);
        }
        
        #endregion
    }
}