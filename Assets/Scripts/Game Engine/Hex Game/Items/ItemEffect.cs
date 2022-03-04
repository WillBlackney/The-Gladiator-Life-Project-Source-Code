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

        [Header("On Hit Effect Data")]
        [Range(0, 100)]
        [LabelWidth(200)]
        [ShowIf("ShowOnHitEffectFields")]
        public int effectChance;
        [LabelWidth(200)]
        [ShowIf("ShowOnHitEffectFields")]
        public PerkPairingData perkApplied;

        // to do: on hit effects
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
            return effectType == ItemEffectType.GainPerk;
        }
    }

    public enum ItemEffectType
    {
        None = 0,
        ModifyAttribute = 1,
        GainPerk = 2,
        OnHitEffect = 3,
    }

    
}