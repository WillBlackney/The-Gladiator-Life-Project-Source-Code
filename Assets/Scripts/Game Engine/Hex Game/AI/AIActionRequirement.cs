using HexGameEngine.Perks;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.AI
{
    [System.Serializable]
    public class AIActionRequirement
    {
        [LabelWidth(100)]
        public AIActionRequirementType requirementType;

        [ShowIf("ShowRange")]
        [LabelWidth(100)]
        public int range;

        [ShowIf("ShowPerkPairing")]
        public PerkPairingData perkPairing;

        [ShowIf("ShowHealthPercentage")]
        [Range(0, 100)]
        [LabelWidth(100)]
        public int healthPercentage;

        [ShowIf("ShowEnemiesInMeleeRange")]
        [LabelWidth(100)]
        public int enemiesInMeleeRange = 1;

        [ShowIf("ShowEnergyReq")]
        [LabelWidth(100)]
        public int energyReq;

        [ShowIf("ShowFatigueReq")]
        [LabelWidth(100)]
        public int fatigueReq;

        [ShowIf("ShowStressReq")]
        [LabelWidth(100)]
        public int stressReq;

        [ShowIf("ShowTurnReq")]
        [LabelWidth(100)]
        public int turnReq;

        [ShowIf("ShowAlliesAlive")]
        [LabelWidth(100)]
        public int alliesAlive;

        [ShowIf("ShowAbilityName")]
        [LabelWidth(100)]
        public string abilityName;

        [ShowIf("ShowArmourReq")]
        [LabelWidth(100)]
        public int armourReq;

        public bool ShowArmourReq()
        {
            return requirementType == AIActionRequirementType.TargetHasMoreArmorThanX ||
                requirementType == AIActionRequirementType.TargetHasLessArmorThanX;
        }
        public bool ShowAbilityName()
        {
            return requirementType == AIActionRequirementType.AbilityIsOffCooldown;
        }
        public bool ShowTurnReq()
        {
            return requirementType == AIActionRequirementType.IsLessThanTurnX || requirementType == AIActionRequirementType.IsMoreThanTurnX;
        }
        public bool ShowEnergyReq()
        {
            if (requirementType == AIActionRequirementType.HasMoreEnergyThanX || requirementType == AIActionRequirementType.HasLessEnergyThanX)
            {
                return true;
            }
            else return false;
        }
        public bool ShowStressReq()
        {
            return requirementType == AIActionRequirementType.TargetHasLessStressThanX ||
                requirementType == AIActionRequirementType.TargetHasMoreStressThanX;
        }
        public bool ShowFatigueReq()
        {
            if (requirementType == AIActionRequirementType.HasMoreFatigueThanX)
            {
                return true;
            }
            else return false;
        }
        public bool ShowEnemiesInMeleeRange()
        {
            if (requirementType == AIActionRequirementType.SelfEngagedInMelee ||
                requirementType == AIActionRequirementType.TargetEngagedInMelee)
            {
                return true;
            }
            else return false;
        }
        public bool ShowRange()
        {
            if (requirementType == AIActionRequirementType.TargetIsWithinRange)
            {
                return true;
            }
            else return false;
        }
        public bool ShowHealthPercentage()
        {
            if (requirementType == AIActionRequirementType.TargetHasLessHealthThanX ||
                requirementType == AIActionRequirementType.SelfHasLessHealthThanX)
            {
                return true;
            }
            else return false;
        }
        public bool ShowPerkPairing()
        {
            if (requirementType == AIActionRequirementType.HasLessThanPerkStacksSelf ||
                requirementType == AIActionRequirementType.HasMoreThanPerkStacksSelf ||
                requirementType == AIActionRequirementType.TargetHasLessPerkStacks ||
                requirementType == AIActionRequirementType.TargetHasMorePerkStacks)
            {
                return true;
            }
            else return false;
        }
        public bool ShowAlliesAlive()
        {
            return requirementType == AIActionRequirementType.LessThanAlliesAlive || 
                requirementType == AIActionRequirementType.MoreThanAlliesAlive;
        }
    }

    public enum AIActionRequirementType
    {
        None = 0,
        TargetIsWithinRange = 1,
        SelfEngagedInMelee = 2,
        NotEngagedInMelee = 8,
        HasMoreEnergyThanX = 10,
        HasLessEnergyThanX = 18,
        HasLessThanPerkStacksSelf = 3,
        HasMoreThanPerkStacksSelf = 4,
        TargetHasMorePerkStacks = 5,
        TargetHasLessPerkStacks = 6,
        TargetHasLessHealthThanX = 7,
        SelfHasLessHealthThanX = 9,
        MoreThanAlliesAlive = 11,
        LessThanAlliesAlive = 12,
        HasRangedAdvantage = 13,
        DoesNotHaveRangedAdvantage = 14,
        AbilityIsOffCooldown = 15,
        TargetNotEngagedInMelee = 16,
        TargetEngagedInMelee = 19,
        TargetIsAdjacentToAlly = 17,
        TargetPositionedForKnockBackStun = 20,
        TargetIsElevated = 21,
        TargetIsNotElevated = 22,
        HasMoreFatigueThanX = 23,
        TargetHasLessStressThanX = 24,
        TargetHasMoreStressThanX = 25,
        IsLessThanTurnX = 26,
        IsMoreThanTurnX = 27,
        TargetHasMoreArmorThanX = 28,
        TargetHasLessArmorThanX = 29,
        TargetIsStunnable = 30,
        TargetHasShield = 31,
        TargetDoesNotHaveShield = 32,
    }
}