using UnityEngine;

public enum WeaponType
{
    Melee,
    Primary,
    Secondary
}

public class WeaponManager : MonoBehaviour
{
    [Header ("Weapon Capacity")]
    public GameObject[] weaponSlots = new GameObject[3]; // 0: Melee, 1: Primary, 2: Secondary

    [Header ("current Weapon")]
    [SerializeField] private int currentWeaponIndex = 0; // 0: Melee, 1: Primary, 2: Secondary

    public GameObject GetActiveWeapon(int index)
    {
        return weaponSlots[index];
    }

    public void ActivateWeapon(int index)
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] != null)
            {
                weaponSlots[i].SetActive(i == index);
            }
        }
        currentWeaponIndex = index;
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
}
