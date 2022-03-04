using HexGameEngine.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Items
{
    public class ItemData
    {    
        public string itemName;
        public string itemDescription;
        public ItemType itemType;
        public bool lootable;
        public bool includeInLibrary;

        public WeaponClass weaponClass;
        public HandRequirement handRequirement;
        public WeaponSlot allowedSlot;
        public InjuryType[] injuryTypesCaused;
        public Rarity rarity;

        public int armourValue;

        public List<AbilityData> grantedAbilities = new List<AbilityData>();
        public List<ItemEffect> itemEffects = new List<ItemEffect>();
        public ItemEffectSet[] itemEffectSets;

        private Sprite itemSprite;
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

        public bool IsMeleeWeapon
        {
            get
            {
                return
                weaponClass != WeaponClass.Holdable &&
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
    }
}