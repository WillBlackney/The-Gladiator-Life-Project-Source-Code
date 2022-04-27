using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using HexGameEngine.Abilities;

namespace HexGameEngine.Items
{
    [CreateAssetMenu(fileName = "New Item Data", menuName = "Item Data", order = 52)]
    public class ItemDataSO: ScriptableObject
    {
        #region Properties
        [HorizontalGroup("General Info", 75)]
        [HideLabel]
        [PreviewField(75)]
        public Sprite itemSprite;

        [VerticalGroup("General Info/Stats")]
        [LabelWidth(100)]
        public string itemName;
        [VerticalGroup("General Info/Stats")]
        [LabelWidth(100)]
        [TextArea]
        public string itemDescription;
        [VerticalGroup("General Info/Stats")]
        [LabelWidth(100)]
        public ItemType itemType;
        [VerticalGroup("General Info/Stats")]
        [LabelWidth(100)]
        public Rarity rarity;
        [VerticalGroup("General Info/Stats")]
        [LabelWidth(100)]
        public bool lootable;
        [VerticalGroup("General Info/Stats")]
        [LabelWidth(100)]
        public bool includeInLibrary = true;

        [BoxGroup("Weapon Info", true, true)]
        [LabelWidth(100)]
        [ShowIf("ShowWeaponField")]
        public WeaponClass weaponClass;
        [BoxGroup("Weapon Info")]
        [LabelWidth(100)]
        [ShowIf("ShowWeaponField")]
        public HandRequirement handRequirement;
        [BoxGroup("Weapon Info")]
        [LabelWidth(100)]
        [ShowIf("ShowWeaponField")]
        public WeaponSlot allowedSlot;
        [BoxGroup("Weapon Info")]
        [LabelWidth(100)]
        [ShowIf("ShowWeaponField")]
        public InjuryType[] injuryTypesCaused;

        [BoxGroup("Armour Info", true, true)]
        [LabelWidth(100)]
        [Range(0, 50)]
        [ShowIf("ShowArmourFields")]
        public int minArmourRoll;

        [BoxGroup("Armour Info")]
        [LabelWidth(100)]
        [Range(0, 50)]
        [ShowIf("ShowArmourFields")]
        public int maxArmourRoll;

        [BoxGroup("Ability Info", true, true)]
        [LabelWidth(100)]
        public AbilityDataSO[] grantedAbilities;

        [BoxGroup("Effects Info", true, true)]
        [LabelWidth(100)]
        public ItemEffectSet[] itemEffectSets;
        #endregion

        #region Odin Showifs
        public bool ShowWeaponField()
        {
            return itemType == ItemType.Weapon;
        }
        public bool ShowArmourFields()
        {
            return itemType == ItemType.Head || itemType == ItemType.Chest;
        }
        public bool ShowWeaponDamageField()
        {
            if(itemType == ItemType.Weapon &&
              (weaponClass != WeaponClass.Holdable && weaponClass != WeaponClass.Shield && weaponClass != WeaponClass.None)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool ShowArmourValue()
        {
            return (itemType == ItemType.Weapon && weaponClass == WeaponClass.Shield) || itemType == ItemType.Head || itemType == ItemType.Chest;
        }
        #endregion
    }
}
