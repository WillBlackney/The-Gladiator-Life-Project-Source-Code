using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGameEngine
{
    public class LootResultModel
    {
        public int goldReward = 0;
        public List<List<CardData>> allCharacterCardChoices = new List<List<CardData>>();
        public ItemData itemReward;
    }
}