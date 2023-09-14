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
                BuildAsThreeSkullReward(this);
            }
        }

        private void BuildAsOneSkullReward(CombatRewardData crd, int deploymentLimit)
        {
            int maxGoldSum = 750;
            int deploymentBonus = (deploymentLimit - 3) * 150;
            if (deploymentBonus > 0)
            {
                maxGoldSum += deploymentBonus;
            }

            int baseGoldReward = (int) (maxGoldSum * GetActsPassedGoldRewardModifier());
            int lowerGoldReward = (int) (baseGoldReward * 0.95f);
            int upperGoldReward = (int) (baseGoldReward * 1.05f);
            baseGoldReward = RandomGenerator.NumberBetween(lowerGoldReward, upperGoldReward);

            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetAllContractRewardableItems(375, 525).ShuffledCopy()[0]);
            crd.goldAmount = baseGoldReward - crd.item.baseGoldValue;
            if (crd.goldAmount < 50)
            {
                crd.goldAmount = 50;
            }
        }
        private void BuildAsTwoSkullReward(CombatRewardData crd, int deploymentLimit)
        {
            int maxGoldSum = 1250;
            int deploymentBonus = (deploymentLimit - 3) * 150;
            if (deploymentBonus > 0)
            {
                maxGoldSum += deploymentBonus;
            }

            int baseGoldReward = (int) (maxGoldSum * GetActsPassedGoldRewardModifier());
            int lowerGoldReward = (int) (baseGoldReward * 0.95f);
            int upperGoldReward = (int) (baseGoldReward * 1.05f);
            baseGoldReward = RandomGenerator.NumberBetween(lowerGoldReward, upperGoldReward);

            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetAllContractRewardableItems(550, 1025).ShuffledCopy()[0]);
            crd.goldAmount = baseGoldReward - crd.item.baseGoldValue;
            if (crd.goldAmount < 50)
            {
                crd.goldAmount = 50;
            }
        }
        private void BuildAsThreeSkullReward(CombatRewardData crd)
        {
            int maxGoldSum = 2500;
            int baseGoldReward = (int) (maxGoldSum * GetActsPassedGoldRewardModifier());
            //int lowerGoldReward = (int)(baseGoldReward * 0.9f);
            // int upperGoldReward = (int)(baseGoldReward * 1.1f);
            //baseGoldReward = RandomGenerator.NumberBetween(lowerGoldReward, upperGoldReward);

            crd.abilityAwarded = AbilityController.Instance.GetRandomAbilityTomeAbility();
            crd.item = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetAllContractRewardableItems((int) (baseGoldReward * 0.25f), (int) (baseGoldReward * 0.85f)).ShuffledCopy()[0]);
            crd.goldAmount = baseGoldReward - crd.item.baseGoldValue;
            if (crd.goldAmount < 50)
            {
                crd.goldAmount = 50;
            }
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
