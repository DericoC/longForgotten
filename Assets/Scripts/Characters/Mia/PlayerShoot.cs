using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField] KeyCode reloadKey = KeyCode.R;

    [Header("States")]
    [Tooltip("Shows the current weapon state")]
    [SerializeField] WeaponState state;

    private GameObject crossHair;
    private CinemachineRecomposer cameraRecomposer;
    private GunBehavior gunBehavior;
    private bool hasGun = false;
    private bool isAiming = false;
    private Animator animator;
    private bool smallExec = false;
    private bool bigExec = false;
    private LogicController logicController;

    void Start()
    {
        logicController = GameObject.FindWithTag("Logic").GetComponent<LogicController>();
        animator = GetComponent<Animator>();
        crossHair = GameObject.FindWithTag("Cross");
        cameraRecomposer = GameObject.FindWithTag("Virtual Camera").GetComponent<CinemachineRecomposer>();
    }

    void Update()
    {
        if (!logicController.Pause)
        {
            state = WeaponState.idle;

            if (isAiming)
            {
                bigExec = false;
                if (!smallExec)
                {
                    StartCoroutine(smallCrossHair());
                }
            }
            else
            {
                smallExec = false;
                if (!bigExec)
                {
                    StartCoroutine(bigCrossHair());
                }
            }

            if (hasGun)
            {
                if (Input.GetMouseButton(0))
                {
                    if (isAiming)
                    {
                        state = WeaponState.shooting;
                        gunBehavior.Shoot();
                    }
                }

                if (Input.GetKey(reloadKey))
                {
                    state = WeaponState.reloading;
                    gunBehavior.StartReloading();
                }

                if (gunBehavior.GunData.reloading)
                {
                    state = WeaponState.reloading;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    animator.SetTrigger("Punch");
                }
            }
        }
    }

    public void gunChange(GameObject current)
    {
        if (current != null)
        {
            gunBehavior = current.GetComponent<GunBehavior>();
        }
    }

    public enum WeaponState
    {
        idle,
        shooting,
        reloading
    }

    public IEnumerator smallCrossHair()
    {
        smallExec = true;
        yield return new WaitForSeconds(0.25f);
        cameraRecomposer.m_ZoomScale = 1.15f;
        yield return new WaitForSeconds(0.5f);
        crossHair.GetComponent<RectTransform>().sizeDelta = new Vector2(15, 15);
    }

    public IEnumerator bigCrossHair()
    {
        bigExec = true;
        yield return new WaitForSeconds(0.15f);
        cameraRecomposer.m_ZoomScale = 1.25f;
        yield return new WaitForSeconds(0.3f);
        crossHair.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
    }

    // Getters / Setters
    public bool HasGun { get => hasGun; set => hasGun = value; }
    public bool IsAiming { get => isAiming; set => isAiming = value; }
}
