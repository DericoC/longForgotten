

namespace LF.LongForgotten
{
    public class Inventory : InventoryBehaviour
    {
        #region FIELDS
        
        
        
        
        private WeaponBehaviour[] weapons;
        
        
        
        
        private WeaponBehaviour equipped;
        
        
        
        private int equippedIndex = -1;

        #endregion
        
        #region METHODS
        
        public override void Init(int equippedAtStart = 0)
        {
            
            weapons = GetComponentsInChildren<WeaponBehaviour>(true);
            
            
            foreach (WeaponBehaviour weapon in weapons)
                weapon.gameObject.SetActive(false);

            
            Equip(equippedAtStart);
        }

        public override WeaponBehaviour Equip(int index)
        {
            
            if (weapons == null)
                return equipped;
            
            
            if (index > weapons.Length - 1)
                return equipped;

            
            if (equippedIndex == index)
                return equipped;
            
            
            if (equipped != null)
                equipped.gameObject.SetActive(false);

            
            equippedIndex = index;
            
            equipped = weapons[equippedIndex];
            
            equipped.gameObject.SetActive(true);

            
            return equipped;
        }
        
        #endregion

        #region Getters

        public override int GetLastIndex()
        {
            
            int newIndex = equippedIndex - 1;
            if (newIndex < 0)
                newIndex = weapons.Length - 1;

            
            return newIndex;
        }

        public override int GetNextIndex()
        {
            
            int newIndex = equippedIndex + 1;
            if (newIndex > weapons.Length - 1)
                newIndex = 0;

            
            return newIndex;
        }

        public override WeaponBehaviour GetEquipped() => equipped;
        public override int GetEquippedIndex() => equippedIndex;

        #endregion
    }
}