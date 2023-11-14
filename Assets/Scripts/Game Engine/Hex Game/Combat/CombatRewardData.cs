using System;
using System.Linq;
using WeAreGladiators.Abilities;
using WeAreGladiators.Items;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Characters
{
    public class CombatRewardData
    {
        public AbilityData abilityAwarded;
        public int goldAmount;
        public ItemData item;

        public CombatRewardData(CombatDifficulty difficulty, int deploymentLimit)
        {
            if (difficulty == CombatDifficulty.Basic)
            {
                BuildAsOneSkullReward(this, deploymentLimit);
            }
            else if (difficulty == CombatDifficulty.Elite)
            {
                BuildAsTwoSkullReward(this, deploymentLimit);
            }
            else if (difficulty == CombatDifficulty.Boss)
            {
                BuildAsThreeSkullReward(this, deploymentLimit);
            }
        }

        private void BuildAsOneSkullReward(CombatRewardData crd, int deploymentLimit)
        {
            int basicReward = 200;
            int perPersonBonus = 100 * deploymentLimit;
            int baseGoldReward = perPersonBonus + basicReward;
            baseGoldReward = RandomGenerator.NumberBetween(baseGoldReward - 50, baseGoldReward + 50);
            ItemData commonTrinket = Array.FindAll(ItemController.Instance.AllItems, item => item.itemType == ItemType.Trinket && item.rarity == Rarity.Common && item.canBeCombatContractReward).ToList().GetRandomElement();

            crd.goldAmount = baseGoldReward;
            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(commonTrinket);
        }

        private void BuildAsTwoSkullReward(CombatRewardData crd, int deploymentLimit)
        {
            int eliteReward = 500;
            int perPersonBonus = 100 * deploymentLimit;
            int baseGoldReward = perPersonBonus + eliteReward;
            baseGoldReward = RandomGenerator.NumberBetween(baseGoldReward - 50, baseGoldReward + 50);
            ItemData commonTrinket = Array.FindAll(ItemController.Instance.AllItems, item => item.itemType == ItemType.Trinket && item.rarity == Rarity.Rare && item.canBeCombatContractReward).ToList().GetRandomElement();

            crd.goldAmount = baseGoldReward;
            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(commonTrinket);
        }
        
        private void BuildAsThreeSkullReward(CombatRewardData crd, int deploymentLimit)
        {
            int bossReward = 800;
            int perPersonBonus = 100 * deploymentLimit;
            int baseGoldReward = perPersonBonus + bossReward;
            baseGoldReward = RandomGenerator.NumberBetween(baseGoldReward - 50, baseGoldReward + 50);
            ItemData commonTrinket = Array.FindAll(ItemController.Instance.AllItems, item => item.itemType == ItemType.Trinket && item.rarity == Rarity.Epic && item.canBeCombatContractReward).ToList().GetRandomElement();

            crd.goldAmount = baseGoldReward;
            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(commonTrinket);
        }
        private float GetActsPassedGoldRewardModifier()
        {
            if (RunController.Instance.CurrentDay == 0)
            {
                return 1f;
            }
            int actsPassed = RunController.Instance.CurrentDay / 4;
            return 1f + 0.2f * actsPassed;
        }
    }
}
