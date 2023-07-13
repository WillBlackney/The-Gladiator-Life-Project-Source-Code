using WeAreGladiators.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.Items
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

        public int GetSellPrice()
        {
            int ret = 0;
            // TO DO: any effects that modify sell price should be calculated here
            if (itemData != null)
                ret = (int)(itemData.baseGoldValue * 0.15f);
            else if (abilityData != null) ret = 10;

            return ret;
        }
    }
}