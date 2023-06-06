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
        }

        private void BuildAsBasicReward(CombatRewardData crd)
        {
            int maxGoldSum = 650;
            int baseGoldReward = (int) (maxGoldSum * GetActsPassedGoldRewardModifier());
            int lowerGoldReward = (int) (baseGoldReward * 0.9f);
            int upperGoldReward = (int)(baseGoldReward * 1.1f);
            baseGoldReward = RandomGenerator.NumberBetween(lowerGoldReward, upperGoldReward);

            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetAllContractRewardableItems((int)(baseGoldReward * 0.25f), (int)(baseGoldReward * 0.85f)).ShuffledCopy()[0]);
            crd.goldAmount = baseGoldReward - crd.item.baseGoldValue;
            if (crd.goldAmount < 50) crd.goldAmount = 50;
        }
        private void BuildAsEliteReward(CombatRewardData crd)
        {
            int maxGoldSum = 1000;
            int baseGoldReward = (int)(maxGoldSum * GetActsPassedGoldRewardModifier());
            int lowerGoldReward = (int)(baseGoldReward * 0.9f);
            int upperGoldReward = (int)(baseGoldReward * 1.1f);
            baseGoldReward = RandomGenerator.NumberBetween(lowerGoldReward, upperGoldReward);

            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetAllContractRewardableItems((int) (baseGoldReward * 0.25f), (int)(baseGoldReward * 0.85f)).ShuffledCopy()[0]);
            crd.goldAmount = baseGoldReward - crd.item.baseGoldValue;
            if (crd.goldAmount < 50) crd.goldAmount = 50;
        }
        private void BuildAsBossReward(CombatRewardData crd)
        {
            int maxGoldSum = 2000;
            int baseGoldReward = (int)(maxGoldSum * GetActsPassedGoldRewardModifier());
            int lowerGoldReward = (int)(baseGoldReward * 0.9f);
            int upperGoldReward = (int)(baseGoldReward * 1.1f);
            baseGoldReward = RandomGenerator.NumberBetween(lowerGoldReward, upperGoldReward);

            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetAllContractRewardableItems((int)(baseGoldReward * 0.25f), (int)(baseGoldReward * 0.85f)).ShuffledCopy()[0]);
            crd.goldAmount = baseGoldReward - crd.item.baseGoldValue;
            if (crd.goldAmount < 50) crd.goldAmount = 50;
        }
        private float GetActsPassedGoldRewardModifier()
        {
            if (RunController.Instance.CurrentDay == 0) return 1f;
            int actsPassed = RunController.Instance.CurrentDay / 4;
            return 1f + (0.2f * actsPassed);
        }


    }
}
