// Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;

namespace LF.LongForgotten
{
    /// <summary>
    /// FeelPreset. Holds all the Feel objects needed to create an overall feel for the game.
    /// </summary>
    [CreateAssetMenu(fileName = "SO_Feel_Preset", menuName = "Infima Games/Low Poly Shooter Pack/Feel Preset", order = 0)]
    public class FeelPreset : ScriptableObject
    {
        #region FIELDS SERIALIZED
        
        [Title(label: "Camera Feel")]
        
        [Tooltip("Camera Feel. Holds the values relating to how the camera feels when playing.")]
        [SerializeField, InLineEditor]
        private Feel cameraFeel;

        [Title(label: "Item Feel")]
        
        [Tooltip("Item Feel. Holds the values relating to how the items feels when playing.")]
        [SerializeField, InLineEditor]
        private Feel itemFeel;
        
        #endregion
        
        #region FUNCTIONS

        /// <summary>
        /// GetFeel. Returns the correct feel based on parameters.
        /// </summary>
        public Feel GetFeel(MotionType motionType)
        {
            //Switch.
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
            return motionType switch
            {
                //MotionType.Camera.
                MotionType.Camera => cameraFeel,
                //MotionType.Item.
                MotionType.Item => itemFeel,
            };
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
        }
        
        #endregion
    }
}