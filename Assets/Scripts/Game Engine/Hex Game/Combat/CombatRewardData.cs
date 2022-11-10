using HexGameEngine.Abilities;
using HexGameEngine.Items;
using HexGameEngine.JourneyLogic;
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
            int baseGoldReward = (int) (150f * GetActsPassedGoldRewardModifier());
            int lowerGoldReward = (int) (baseGoldReward * 0.9f);
            int upperGoldReward = (int)(baseGoldReward * 1.1f);
            crd.goldAmount = RandomGenerator.NumberBetween(lowerGoldReward, upperGoldReward);
            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
        }
        private void BuildAsEliteReward(CombatRewardData crd)
        {
            int baseGoldReward = (int)(200f * GetActsPassedGoldRewardModifier());
            int lowerGoldReward = (int)(baseGoldReward * 0.9f);
            int upperGoldReward = (int)(baseGoldReward * 1.1f);
            crd.goldAmount = RandomGenerator.NumberBetween(lowerGoldReward, upperGoldReward);
            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetAllContractRewardableItems(Rarity.Rare)[0]);
        }
        private void BuildAsBossReward(CombatRewardData crd)
        {
            int baseGoldReward = (int)(250f * GetActsPassedGoldRewardModifier());
            int lowerGoldReward = (int)(baseGoldReward * 0.9f);
            int upperGoldReward = (int)(baseGoldReward * 1.1f);
            crd.goldAmount = RandomGenerator.NumberBetween(lowerGoldReward, upperGoldReward);
            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetAllContractRewardableItems(Rarity.Epic)[0]);

        }
        private float GetActsPassedGoldRewardModifier()
        {
            if (RunController.Instance.CurrentDay == 0) return 1f;
            int actsPassed = RunController.Instance.CurrentDay / 4;
            return 1f + (0.2f * actsPassed);
        }


    }
}
