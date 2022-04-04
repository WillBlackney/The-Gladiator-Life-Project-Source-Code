using HexGameEngine.Abilities;
using HexGameEngine.Items;
using HexGameEngine.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Characters
{
    public class CombatRewardData
    {
        public int goldAmount;
        public ItemData item;
        public AbilityData abilityAwarded;

        public CombatRewardData(CombatDifficulty difficulty)
        {
            if (difficulty == CombatDifficulty.Basic)
                BuildAsBasicReward(this);
            else if (difficulty == CombatDifficulty.Elite)
                BuildAsEliteReward(this);
            // to do: boss reward
        }

        public void BuildAsBasicReward(CombatRewardData crd)
        {
            crd.goldAmount = RandomGenerator.NumberBetween(125, 150);
            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetRandomLootableItemByRarity(Rarity.Rare));

        }
        public void BuildAsEliteReward(CombatRewardData crd)
        {
            crd.goldAmount = RandomGenerator.NumberBetween(225, 250);
            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetRandomLootableItemByRarity(Rarity.Epic));
        }
        public void BuildAsBossReward()
        {

        }


    }
}
