using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WeAreGladiators.Items
{
    public class ItemSet
    {
        public ItemData bodyArmour;
        public ItemData headArmour;
        public ItemData mainHandItem;
        public ItemData offHandItem;
        public ItemData trinket;

        public bool IsDualWieldingMeleeWeapons()
        {
            return mainHandItem != null &&
                mainHandItem.IsMeleeWeapon &&
                mainHandItem.handRequirement == HandRequirement.OneHanded &&
                offHandItem != null &&
                offHandItem.IsMeleeWeapon &&
                offHandItem.handRequirement == HandRequirement.OneHanded;
        }
        public bool IsWieldingTwoHandMeleeWeapon()
        {
            return mainHandItem != null &&
                mainHandItem.IsMeleeWeapon &&
                mainHandItem.handRequirement == HandRequirement.TwoHanded;
        }
        public bool IsWieldingBowOrCrossbow()
        {
            return mainHandItem != null &&
                (mainHandItem.weaponClass == WeaponClass.Bow || mainHandItem.weaponClass == WeaponClass.Crossbow);
        }
        public bool IsWieldingOneHandMeleeWeaponWithEmptyOffhand()
        {
            return mainHandItem != null &&
                mainHandItem.IsMeleeWeapon &&
                mainHandItem.handRequirement == HandRequirement.OneHanded &&
                offHandItem == null;
        }
    }

    [Serializable]
    public class SerializedItemSet
    {
        [Header("Weapons")]
        [LabelWidth(100)]
        public ItemDataSO mainHandItem;
        [LabelWidth(100)]
        public ItemDataSO offHandItem;

        [Header("Armour")]
        [LabelWidth(100)]
        public ItemDataSO headArmour;
        [LabelWidth(100)]
        public ItemDataSO chestArmour;

        [Header("Extra")]
        [LabelWidth(100)]
        public ItemDataSO trinket;
    }

    [Serializable]
    public class RecruitWeaponLoadout
    {
        public ItemDataSO[] mainHandProspects;
        public ItemDataSO[] offhandProspects;
    }
    [Serializable]
    public class RecruitArmourLoadout
    {
        public ItemDataSO[] headProspects;
        public ItemDataSO[] bodyProspects;
    }
}
