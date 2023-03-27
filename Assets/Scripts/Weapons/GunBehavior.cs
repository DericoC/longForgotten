using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class GunBehavior : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GunData gunData;
    [SerializeField] private Transform muzzle;

    float timeSinceLastShot;

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        Debug.DrawRay(muzzle.position, muzzle.forward * gunData.maxDistance, Color.green);
    }

    // Check if player ain't reloading and some time has pass since last shot
    //                              (equivalent to the firerate of the gun)

    // Example: 600/60 = 10rps so, 1s/10rps == 0.1s rounds per second (rps)
    private bool CanShoot() => !gunData.reloading && timeSinceLastShot > 1f /
                               (gunData.fireRate / 60f);

    public void Shoot()
    {
        if (gunData.currentAmmo > 0)
        {
            if (CanShoot())
            {
                if (Physics.Raycast(muzzle.position, muzzle.forward, out
                    RaycastHit hitInfo, gunData.maxDistance))
                {
                    IDamagable damagable = hitInfo.transform.GetComponent<IDamagable>();
                    damagable?.TakeDamage(gunData.damage);
                }
                gunData.currentAmmo--;
                timeSinceLastShot = 0;
            }
        }
    }

    private void OnGunShot()
    {
        throw new NotImplementedException();
    }

    public void StartReloading()
    {
        if (!gunData.reloading)
        {
            StartCoroutine(Reload());
        }
    }

    public IEnumerator Reload()
    {
        gunData.reloading = true;

        yield return new WaitForSeconds(gunData.reloadTime);

        gunData.currentAmmo = gunData.magSize;

        gunData.reloading = false;
    }

    // Getters / Setters
    public GunData GunData { get => gunData; set => gunData = value; }
}
