using HexGameEngine.Perks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HexGameEngine.Items
{
    [System.Serializable]
    public class ItemEffect 
    {
        #region Properties
        [Header("General Properties")]
        [LabelWidth(200)]
        public ItemEffectType effectType;

        [Header("Attribute Properties")]
        [ShowIf("ShowAttributeModified")]
        [LabelWidth(200)]
        public ItemCoreAttribute attributeModified;
        [ShowIf("ShowAttributeModified")]
        [LabelWidth(200)]
        public int modAmount;

        [Header("Perk Properties")]
        [ShowIf("ShowPerkGained")]
        [LabelWidth(200)]
        public ActivePerk perkGained;

        [ShowIf("ShowGainPerkChance")]
        [LabelWidth(200)]
        [Range(1,100)]
        public int gainPerkChance = 50;

        [Header("On Hit Effect Data")]
        [Range(0, 100)]
        [LabelWidth(200)]
        [ShowIf("ShowOnHitEffectFields")]
        public int effectChance;
        [LabelWidth(200)]
        [ShowIf("ShowOnHitEffectFields")]
        public PerkPairingData perkApplied;

        // Innate effects
        [Header("Innate Weapon Effects")]
        [ShowIf("ShowInnateEffectType")]
        [LabelWidth(200)]
        public InnateItemEffectType innateItemEffectType;

        [ShowIf("ShowInnateAccuracyMod")]
        [LabelWidth(200)]
        [Range(-100, 100)]
        public int innateAccuracyMod;

        [ShowIf("ShowInnateAccuracyAgainstAdjacentMod")]
        [LabelWidth(200)]
        [Range(-100, 100)]
        public int innateAccuracyAgainstAdjacentMod;

        [ShowIf("ShowInnateAccuracyAgainstAdjacentMod")]
        [LabelWidth(200)]
        [Range(0, 2)]
        public int innateWeaponRangeBonus = 0;

        [ShowIf("ShowInnateBackstabPenetrationBonus")]
        [LabelWidth(200)]
        [Range(0, 100)]
        public int innateBackstabPenetrationBonus = 0;

        [ShowIf("ShowInnatePerkGainedOnUse")]
        [LabelWidth(200)]
        public ActivePerk innatePerkGainedOnUse;

        #endregion

        #region Constructors
        public ItemEffect() { }
        public ItemEffect(ItemCoreAttribute attribute, int mod)
        {
            effectType = ItemEffectType.ModifyAttribute;
            attributeModified = attribute;
            modAmount = mod;
        }
        #endregion

        #region Odin Show ifs
        public bool ShowOnHitEffectFields()
        {
            return effectType == ItemEffectType.OnHitEffect;
        }
        public bool ShowAttributeModified()
        {
            return effectType == ItemEffectType.ModifyAttribute;
        }
        public bool ShowPerkGained()
        {
            return effectType == ItemEffectType.GainPerk ||
                 effectType == ItemEffectType.GainPerkTurnStart || 
                 effectType == ItemEffectType.GainPerkCombatStart;
        }
        public bool ShowGainPerkChance()
        {
            return effectType == ItemEffectType.GainPerkTurnStart;
        }
        public bool ShowInnateEffectType()
        {
            return effectType == ItemEffectType.InnateWeaponEffect;
        }
        public bool ShowInnateAccuracyMod()
        {
            return effectType == ItemEffectType.InnateWeaponEffect && innateItemEffectType == InnateItemEffectType.InnateAccuracyModifier;
        }
        public bool ShowInnateAccuracyAgainstAdjacentMod()
        {
            return effectType == ItemEffectType.InnateWeaponEffect && innateItemEffectType == InnateItemEffectType.InnateAccuracyAgainstAdjacentModifier;
        }
        public bool ShowInnatePerkGainedOnUse()
        {
            return effectType == ItemEffectType.InnateWeaponEffect && innateItemEffectType == InnateItemEffectType.InnatePerkGainedOnUse;
        }
        public bool ShowInnateBackstabPenetrationBonus()
        {
            return effectType == ItemEffectType.InnateWeaponEffect && innateItemEffectType == InnateItemEffectType.PenetrationBonusOnBackstab;
        }
        public bool ShowInnateWeaponRangeBonus()
        {
            return effectType == ItemEffectType.InnateWeaponEffect && innateItemEffectType == InnateItemEffectType.BonusMeleeRange;
        }
        #endregion
    }

    public enum ItemEffectType
    {
        None = 0,
        ModifyAttribute = 1,
        GainPerk = 2,
        GainPerkCombatStart = 4,
        GainPerkTurnStart = 5,
        OnHitEffect = 3,
        InnateWeaponEffect = 6,
    }

    public enum InnateItemEffectType
    {
        None = 0,
        InnateAccuracyModifier = 1,
        InnateAccuracyAgainstAdjacentModifier = 2,
        InnatePerkGainedOnUse = 3,
        BonusMeleeRange = 4,
        PenetrationBonusOnBackstab = 5,
}

    
}