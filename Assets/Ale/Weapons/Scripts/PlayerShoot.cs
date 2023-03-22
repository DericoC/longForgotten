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

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // Will Invoke onclick, if not, it'll catch it
            shootInput?.Invoke();
        }

        if (Input.GetKeyDown(reloadKey))
        {
            reloadInput?.Invoke();
        }
    }
}
