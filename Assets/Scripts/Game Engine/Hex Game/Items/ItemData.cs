using WeAreGladiators.Abilities;
using WeAreGladiators.Audio;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.Items
{
    public class ItemData
    {
        #region Properties
        // Core Properties
        public string itemName;
        private Sprite itemSprite;
        public string itemDescription;
        public ItemType itemType;
        public bool canSpawnInShop;
        public bool canBeCombatContractReward;
        public bool includeInLibrary;
        public Rarity rarity;
        public int baseGoldValue;

        // Weapon Properties
        public WeaponClass weaponClass;
        public HandRequirement handRequirement;
        public WeaponSlot allowedSlot;
        public InjuryType[] injuryTypesCaused;
        public float healthDamage = 1f;
        public float armourDamage = 1f;
        public float armourPenetration = 0.25f;
        public WeaponAttackAnimationType weaponAttackAnimationType;

        // Armour Properties
        public int fatiguePenalty;
        public int armourAmount;
        public int minArmourRoll;
        public int maxArmourRoll;

        public List<AbilityData> grantedAbilities = new List<AbilityData>();
        public List<ItemEffect> itemEffects = new List<ItemEffect>();
        public ItemEffectSet[] itemEffectSets;

        public Sound swingSFX = Sound.None;
        public Sound hitSFX = Sound.None; 
        public Sound equipSFX = Sound.None;
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
                weaponClass != WeaponClass.None &&
                weaponClass != WeaponClass.Holdable &&
                weaponClass != WeaponClass.ThrowingNet &&                
                weaponClass != WeaponClass.Shield &&
                weaponClass != WeaponClass.Bow &&
                weaponClass != WeaponClass.Crossbow;
            }
        }
        public bool IsRangedWeapon
        {
            get
            {
                return
                weaponClass == WeaponClass.Bow ||
                weaponClass == WeaponClass.Crossbow;
            }
        }
        #endregion
    }
}