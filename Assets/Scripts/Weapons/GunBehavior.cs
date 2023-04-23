using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class GunBehavior : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GunData gunData;
    [SerializeField] private Transform muzzle;
    public float impactForce = 30f;
    public GameObject GenericImpactEffect;
    public GameObject BloodImpactEffect;
    public GameObject muzzleFlash;
    public float impactDuration = 0.2f;

    float timeSinceLastShot;

    private void Start()
    {
        gunData.currentAmmo = gunData.magSize;
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        Debug.DrawRay(muzzle.position, muzzle.forward * gunData.maxDistance, Color.green);

        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
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
                RaycastHit hit;
                if (Physics.Raycast(muzzle.position, muzzle.forward, out hit, gunData.maxDistance))
                {
                    if (hit.rigidbody != null)
                    {
                        hit.rigidbody.AddForce(-hit.normal * gunData.impactForce);
                    }

                    switch (hit.transform.tag)
                    {
                        default:
                            GameObject impact = Instantiate(GenericImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                            Destroy(impact, impactDuration);
                            Debug.Log("Dong!");
                            break;
                            case "Zombie":
                            GameObject impact2 = Instantiate(BloodImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));

                            IDamagable damagable = hit.transform.GetComponent<IDamagable>();
                            damagable?.TakeDamage(gunData.damage);

                            Destroy(impact2, impactDuration);
                            Debug.Log("Ding!");
                            break;
                    }
                    gunData.currentAmmo--;
                    timeSinceLastShot = 0;
                }
            }
            else return;
        }
        else
        {
            StartReloading();
        }
    }

    public void StartReloading()
    {
        if (!gunData.reloading && !gunData.currentAmmo.Equals(gunData.magSize))
        {
            StartCoroutine(Reload());
        }
    }

    public IEnumerator Reload()
    {
        gunData.reloading = true;

        // Show reloading message
        //GameObject canvas = GameObject.Find("AmmoHUD");
        //Text reloadingText = canvas.transform.Find("ReloadText").GetComponent<Text>();
        //reloadingText.text = "Reloading...";
        //reloadingText.enabled = true;

        yield return new WaitForSeconds(gunData.reloadTime);

        // Hide reloading message
        //reloadingText.enabled = false;

        gunData.currentAmmo = gunData.magSize;

        gunData.reloading = false;
    }

    /*
    void UpdateAmmoCounter()
    {
        GameObject canvas = GameObject.Find("AmmoHUD");
        Text ammoText = canvas.transform.Find("CurrentAmmo").GetComponent<Text>();

        string bulletSymbols = "";
        for (int i = 0; i < currentAmmo; i++)
        {
            bulletSymbols += "I";
        }

        ammoText.text = bulletSymbols;
    }
     * 
     */



    // Getters / Setters
    public GunData GunData { get => gunData; set => gunData = value; }
}
