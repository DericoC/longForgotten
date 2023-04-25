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
    private Vector2 screenCenterPoint;

    float timeSinceLastShot;

    private void Start()
    {
        //MAX AMMO
        gunData.currentAmmo = gunData.magSize;
        //Center of the screen (Donde esta el crosshair)
        screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        Debug.DrawRay(muzzle.position, muzzle.forward * gunData.maxDistance, Color.green);

        //ESTO MANEJA EL DISPARO POR AHORA
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
        //Si el arma tiene balas ->
        if (gunData.currentAmmo > 0)
        {
            //Si se puede disparar ->
            if (CanShoot())
            {
                //Disparamos un rayo desde el 'Muzzle' hacia la direccion del 'screenCenterPoint' (HAY QUE CAMBIAR ESTO)
                RaycastHit hit;
                if (Physics.Raycast(muzzle.position, screenCenterPoint, out hit, gunData.maxDistance))
                {
                    //Esto es para que los objetos que se impacten y tengan rigidbody se muevan 
                    if (hit.rigidbody != null)
                    {
                        //Se usa una variable del gunData para que el impacto sea mas o menos fuerte dependiendo del arma
                        hit.rigidbody.AddForce(-hit.normal * gunData.impactForce);
                    }

                    //Instanciamos el efecto de impacto en el punto de impacto
                    //Usamos un switch para que el efecto de impacto sea diferente dependiendo del tag del objeto impactado
                    switch (hit.transform.tag)
                    {
                        default:
                            //Iniciamos el objeto y lo destruimos despues de un tiempo (impactDuration) este tiene impacto de pared
                            GameObject impact = Instantiate(GenericImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                            Destroy(impact, impactDuration);
                            //El dong es para saber que se disparo a la pared
                            Debug.Log("Dong!");
                            break;
                        case "Zombie":
                            //Iniciamos el objeto y lo destruimos despues de un tiempo (impactDuration) este tiene impacto de sangre
                            GameObject impact2 = Instantiate(BloodImpactEffect, hit.point, Quaternion.LookRotation(hit.normal));

                            //Realizamos dano al zombie
                            IDamagable damagable = hit.transform.GetComponent<IDamagable>();
                            damagable?.TakeDamage(gunData.damage);

                            Destroy(impact2, impactDuration);
                            //El ding es para saber que se dispara a un zombie
                            Debug.Log("Ding!");
                            break;
                    }
                    //Reducimos ammo y reiniciamos el timer.
                    gunData.currentAmmo--;
                    timeSinceLastShot = 0;
                }
            }
            else return;
        }
        //Este else es para que si no hay balas se recargue automaticamente.
        else
        {
            StartReloading();
        }
    }

    public void StartReloading()
    {
        //Se recarga si se hace preciona la tecla de recargar y si no se esta recargando y si no se tiene el maximo de balas
        if (!gunData.reloading && !gunData.currentAmmo.Equals(gunData.magSize))
        {
            StartCoroutine(Reload());
        }
    }

    public IEnumerator Reload()
    {
        gunData.reloading = true;

        //Show reloading message
        //GameObject canvas = GameObject.Find("AmmoHUD");
        //Text reloadingText = canvas.transform.Find("ReloadText").GetComponent<Text>();
        //reloadingText.text = "Reloading...";
        //reloadingText.enabled = true;

        yield return new WaitForSeconds(gunData.reloadTime);

        //Hide reloading message
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
