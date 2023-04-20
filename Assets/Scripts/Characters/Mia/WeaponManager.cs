using UnityEngine;

public enum WeaponType
{
    Melee,
    Primary,
    Secondary
}

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Capacity")]
    public GameObject[] weaponPrefabs = new GameObject[3]; // 0: Melee, 1: Primary, 2: Secondary
    private GameObject[] weaponInstances = new GameObject[3];

    [Header("Current Weapon")]
    public GameObject currentWeaponParent;
    [SerializeField] private int currentWeaponIndex = 0; // 0: Melee, 1: Primary, 2: Secondary

    public GameObject GetActiveWeapon(int index)
    {
        return weaponInstances[index];
    }

    public void ActivateWeapon(int index)
    {
        for (int i = 0; i < weaponInstances.Length; i++)
        {
            if (weaponInstances[i] != null)
            {
                weaponInstances[i].SetActive(i == index);
            }
        }
        currentWeaponIndex = index;
        UpdateCurrentWeapon();
    }


    private void UpdateCurrentWeapon()
    {
        // Destroy the previous weapon if any
        if (currentWeaponParent.transform.childCount > 0)
        {
            Destroy(currentWeaponParent.transform.GetChild(0).gameObject);
        }

        if (weaponPrefabs[currentWeaponIndex] != null)
        {
            // Instantiate the new weapon
            GameObject newWeapon = Instantiate(weaponPrefabs[currentWeaponIndex], currentWeaponParent.transform);
            newWeapon.transform.localPosition = Vector3.zero;
            newWeapon.transform.localRotation = Quaternion.identity;

            // Update the weaponInstances array
            weaponInstances[currentWeaponIndex] = newWeapon;
        }
    }


    public string GetCurrentWeaponName()
    {
        if (weaponPrefabs[currentWeaponIndex] != null)
        {
            return weaponPrefabs[currentWeaponIndex].name.Replace("(Clone)", "").Trim();

        }
        return "No Weapon";
    }

    public WeaponType GetActiveWeaponType()
    {
        switch (currentWeaponIndex)
        {
            case 0:
                return WeaponType.Melee;
            case 1:
                return WeaponType.Primary;
            case 2:
                return WeaponType.Secondary;
            default:
                return WeaponType.Melee; // fallback to Melee if currentWeapon is out of bounds
        }
    }

    public float GetCurrentWeaponReloadTime()
    {
        if (weaponPrefabs[currentWeaponIndex] != null)
        {
            GunBehavior gunBehavior = weaponPrefabs[currentWeaponIndex].GetComponent<GunBehavior>();
            if (gunBehavior != null)
            {
                Debug.Log($"Weapon: {currentWeaponIndex}, Reload Time: {gunBehavior.GunData.reloadTime}");
                return gunBehavior.GunData.reloadTime;
            }
        }
        return 0;
    }
}