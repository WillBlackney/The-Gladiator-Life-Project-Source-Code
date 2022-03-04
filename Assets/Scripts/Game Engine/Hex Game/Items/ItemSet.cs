using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Items
{
    public class ItemSet
    {
        public ItemData mainHandItem;
        public ItemData offHandItem;
        public ItemData chestArmour;
        public ItemData headArmour;
        public ItemData trinket;
    }

    [System.Serializable]
    public class SerializedItemSet
    {
        [Header("Weapons")]
        public ItemDataSO mainHandItem;
        public ItemDataSO offHandItem;

        [Header("Armour")]
        public ItemDataSO headArmour;
        public ItemDataSO chestArmour;

        [Header("Extra")]
        public ItemDataSO trinket;

    }
}