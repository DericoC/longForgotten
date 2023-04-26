

using UnityEngine;

namespace LF.LongForgotten
{
    
    
    
    public abstract class CharacterBehaviour : MonoBehaviour
    {
        #region UNITY

        protected virtual void Awake(){}
        protected virtual void Start(){}
        protected virtual void Update(){}
        protected virtual void LateUpdate(){}

        #endregion
        
        #region GETTERS
        
        public abstract int GetShotsFired();
        public abstract bool IsLowered();
        public abstract Camera GetCameraWorld();
        public abstract Camera GetCameraDepth();
        
        
        
        
        public abstract InventoryBehaviour GetInventory();

        
        
        
        public abstract int GetGrenadesCurrent();
        
        
        
        public abstract int GetGrenadesTotal();

        
        
        
        public abstract bool IsRunning();
        
        
        
        public abstract bool IsHolstered();

        
        
        
        public abstract bool IsCrouching();
        
        
        
        public abstract bool IsReloading();

        
        
        
        public abstract bool IsThrowingGrenade();
        
        
        
        public abstract bool IsMeleeing();
        
        
        
        
        public abstract bool IsAiming();
        
        
        
        public abstract bool IsCursorLocked();

        
        
        
        public abstract bool IsTutorialTextVisible();

        
        
        
        public abstract Vector2 GetInputMovement();
        
        
        
        public abstract Vector2 GetInputLook();

        
        
        
        public abstract AudioClip[] GetAudioClipsGrenadeThrow();
        
        
        
        public abstract AudioClip[] GetAudioClipsMelee();
        
        
        
        
        public abstract bool IsInspecting();
        
        
        
        
        public abstract bool IsHoldingButtonFire();
        
        #endregion

        #region ANIMATION

        
        
        
        public abstract void EjectCasing();
        
        
        
        public abstract void FillAmmunition(int amount);

        
        
        
        public abstract void Grenade();
        
        
        
        public abstract void SetActiveMagazine(int active);
        
        
        
        
        public abstract void AnimationEndedBolt();
        
        
        
        public abstract void AnimationEndedReload();

        
        
        
        public abstract void AnimationEndedGrenadeThrow();
        
        
        
        public abstract void AnimationEndedMelee();

        
        
        
        public abstract void AnimationEndedInspect();
        
        
        
        public abstract void AnimationEndedHolster();

        
        
        
        public abstract void SetSlideBack(int back);

        public abstract void SetActiveKnife(int active);

        #endregion
    }
}