using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField] KeyCode reloadKey = KeyCode.R;

    [Header("States")] [Tooltip("Shows the current weapon state")] 
    [SerializeField] WeaponState state;

    private GunBehavior gunBehavior;
    private bool hasGun = false;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        state = WeaponState.idle;

        if (hasGun)
        {
            if (Input.GetMouseButton(0))
            {
                state = WeaponState.shooting;
                gunBehavior.Shoot();
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
        } else
        {
            if (Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("Punch");
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

    // Getters / Setters
    public bool HasGun { get => hasGun; set => hasGun = value; }
}
