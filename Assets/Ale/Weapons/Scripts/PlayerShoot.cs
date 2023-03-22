using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public static Action shootInput;
    public static Action reloadInput;

    [Header("Keys")]
    [SerializeField] KeyCode reloadKey;

    [Header("States")] [Tooltip("Shows the current weapon state")] 
    [SerializeField] WeaponState state;

    void Update()
    {
        state = WeaponState.idle;

        if (Input.GetMouseButton(0))
        {
            state = WeaponState.shooting;
            // Will Invoke onclick, if not, it'll catch it
            shootInput?.Invoke();
        }

        if (Input.GetKeyDown(reloadKey))
        {
            state = WeaponState.reloading;
            // Will Invoke onclick, if not, it'll catch it
            reloadInput?.Invoke();
        }
    }

    public enum WeaponState
    {
        idle,
        shooting,
        reloading
    }
}
