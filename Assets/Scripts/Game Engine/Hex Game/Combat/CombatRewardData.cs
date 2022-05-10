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
            else if (difficulty == CombatDifficulty.Boss)
                BuildAsBossReward(this);
            // to do: boss reward
        }

        private void BuildAsBasicReward(CombatRewardData crd)
        {
            crd.goldAmount = RandomGenerator.NumberBetween(125, 150);
            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetRandomLootableItemByRarity(Rarity.Rare));

        }
        private void BuildAsEliteReward(CombatRewardData crd)
        {
            crd.goldAmount = RandomGenerator.NumberBetween(225, 250);
            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetRandomLootableItemByRarity(Rarity.Epic));
        }
        private void BuildAsBossReward(CombatRewardData crd)
        {
            // to do: update this when we create legendary items + define boss reward paramters
            crd.goldAmount = RandomGenerator.NumberBetween(225, 250);
            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetRandomLootableItemByRarity(Rarity.Epic));

        }


    }
}
