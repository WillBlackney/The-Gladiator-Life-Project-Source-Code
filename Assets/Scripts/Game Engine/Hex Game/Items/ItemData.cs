using HexGameEngine.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Items
{
    public class ItemData
    {
        #region Properties
        // Core Properties
        public string itemName;
        private Sprite itemSprite;
        public string itemDescription;
        public ItemType itemType;
        public bool lootable;
        public bool includeInLibrary;
        public Rarity rarity;

        // Weapon Properties
        public WeaponClass weaponClass;
        public HandRequirement handRequirement;
        public WeaponSlot allowedSlot;
        public InjuryType[] injuryTypesCaused;

        // Armour Properties
        public int armourAmount;
        public int minArmourRoll;
        public int maxArmourRoll;
        public ItemArmourClass armourClass;

        public List<AbilityData> grantedAbilities = new List<AbilityData>();
        public List<ItemEffect> itemEffects = new List<ItemEffect>();
        public ItemEffectSet[] itemEffectSets;
        #endregion

        #region Getters + Accessors
        public Sprite ItemSprite
        {
            get
            {
                if (itemSprite == null)
                {
                    itemSprite = GetMySprite();
                    return itemSprite;
                }
                else
                {
                    return itemSprite;
                }
            }
        }
        private Sprite GetMySprite()
        {
            Sprite s = null;

            foreach (ItemDataSO i in ItemController.Instance.AllItemScriptableObjects)
            {
                if (i.itemName == itemName)
                {
                    s = i.itemSprite;
                    break;
                }
            }

            if (s == null)
                Debug.LogWarning("ItemData.GetMySprite() could not sprite for item " + itemName + ", returning null...");


            return s;
        }
        #endregion

        #region Misc
        public bool IsMeleeWeapon
        {
            get
            {
                return
                weaponClass != WeaponClass.Holdable &&
                weaponClass != WeaponClass.ThrowingNet &&
                weaponClass != WeaponClass.None &&
                weaponClass != WeaponClass.Shield &&
                weaponClass != WeaponClass.Bow;
            }
        }
        public bool IsRangedWeapon
        {
            get
            {
                return
                weaponClass == WeaponClass.Bow ||
                weaponClass == WeaponClass.Staff;
            }
        }
        #endregion
    }
}