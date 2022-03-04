using HexGameEngine.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Items
{
    public class InventoryItem
    {
        public AbilityData abilityData;
        public ItemData itemData;

        public InventoryItem(AbilityData abiltity)
        {
            abilityData = abiltity;
            itemData = null;
        }
        public InventoryItem(ItemData item)
        {
            abilityData = null;
            itemData = item;
        }
    }
}