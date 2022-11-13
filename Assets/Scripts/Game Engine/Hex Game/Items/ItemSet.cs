using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HexGameEngine.Items
{
    public class ItemSet
    {
        public ItemData mainHandItem;
        public ItemData offHandItem;
        public ItemData bodyArmour;
        public ItemData headArmour;
        public ItemData trinket;
    }

    [System.Serializable]
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

    [System.Serializable]
    public class RecruitWeaponLoadout
    {
        public ItemDataSO[] mainHandProspects;
        public ItemDataSO[] offhandProspects;
    }
    [System.Serializable]
    public class RecruitArmourLoadout
    {
        public ItemDataSO[] headProspects;
        public ItemDataSO[] bodyProspects;
    }
}