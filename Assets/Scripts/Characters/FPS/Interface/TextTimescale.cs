//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;

namespace LF.LongForgotten.Interface
{
    /// <summary>
    /// Component that changes a text to match the current time scale.
    /// </summary>
    public class TextTimescale : ElementText
    {
        #region METHODS

        protected override void Tick()
        {
            //Change text to match the time scale!
            textMesh.text = "Timescale : " + Time.timeScale;
        }        

        #endregion
    }
}