using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using WeAreGladiators.Abilities;
using WeAreGladiators.Audio;

namespace WeAreGladiators.Items
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
        public int baseGoldValue;
        [VerticalGroup("General Info/Stats")]
        [LabelWidth(100)]
        public bool canSpawnInShop;
        [VerticalGroup("General Info/Stats")]
        [LabelWidth(100)]
        public bool canBeCombatContractReward;
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
        [BoxGroup("Weapon Info")]
        [LabelWidth(100)]
        [ShowIf("ShowWeaponField")]
        public WeaponAttackAnimationType weaponAttackAnimationType;
        [BoxGroup("Weapon Info")]
        [LabelWidth(100)]
        [ShowIf("ShowWeaponField")]
        public bool canDecapitate = false;

        [BoxGroup("Weapon Info")]
        [LabelWidth(125)]
        [ShowIf("ShowWeaponDamageFields")]
        [Header("Weapon Damage Settings")]
        [Range(0.1f, 3f)]
        [Tooltip("How much damage the weapon deals to health. A value of 1 means 100% damage to health, so no modification")]
        public float healthDamage = 1f;

        [BoxGroup("Weapon Info")]
        [LabelWidth(125)]
        [ShowIf("ShowWeaponDamageFields")]
        [Range(0.1f, 3f)]
        [Tooltip("How much damage the weapon deals to armour. A value of 1 means 100% damage to armour, so no modification")]
        public float armourDamage = 1f;

        [BoxGroup("Weapon Info")]
        [LabelWidth(125)]
        [ShowIf("ShowWeaponDamageFields")]
        [Range(0f, 1f)]
        [Tooltip("How much of the weapons damage ignore armour")]
        public float armourPenetration = 0.25f;

        [BoxGroup("Armour Info", true, true)]
        [LabelWidth(100)]
        [Range(0, 400)]
        [ShowIf("ShowArmourFields")]
        public int minArmourRoll;             

        [BoxGroup("Armour Info")]
        [LabelWidth(100)]
        [Range(0, 400)]
        [ShowIf("ShowArmourFields")]
        public int maxArmourRoll;

        [BoxGroup("Ability Info", true, true)]
        [LabelWidth(100)]
        public AbilityDataSO[] grantedAbilities;

        [BoxGroup("Effects Info", true, true)]
        [LabelWidth(100)]
        public ItemEffectSet[] itemEffectSets;

        [BoxGroup("Audio Info", true, true)]
        [LabelWidth(100)]
        public Sound equipSFX = Sound.None;

        [BoxGroup("Audio Info")]
        [ShowIf("ShowAudioWeaponFields")]
        [LabelWidth(100)]
        public Sound swingSFX = Sound.None;

        [BoxGroup("Audio Info")]
        [ShowIf("ShowAudioWeaponFields")]
        [LabelWidth(100)]
        public Sound hitSFX = Sound.None;

     
        #endregion

        #region Odin Showifs
        public bool ShowAudioWeaponFields()
        {
            return itemType == ItemType.Weapon;
        }
        public bool ShowWeaponField()
        {
            return itemType == ItemType.Weapon;
        }
        public bool ShowWeaponDamageFields()
        {
            return itemType == ItemType.Weapon &&
                weaponClass != WeaponClass.ThrowingNet &&
                weaponClass != WeaponClass.Shield &&
                weaponClass != WeaponClass.None &&
                weaponClass != WeaponClass.Holdable;
        }
        public bool ShowArmourFields()
        {
            return itemType == ItemType.Head || itemType == ItemType.Body;
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
            return (itemType == ItemType.Weapon && weaponClass == WeaponClass.Shield) || itemType == ItemType.Head || itemType == ItemType.Body;
        }
        #endregion
    }

    public enum WeaponAttackAnimationType
    {
        Overhead = 0,
        Thrust = 1,
    }
}
