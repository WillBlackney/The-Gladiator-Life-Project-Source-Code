using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WeAreGladiators.AI
{
    [Serializable]
    public class AIAction
    {
        [Header("Core Properties")]
        [LabelWidth(100)]
        public AIActionType actionType;
        [LabelWidth(100)]
        [ShowIf("ShowTargettingPriority")]
        public TargettingPriority[] targettingPriority;

        [ShowIf("ShowGetRangeFromAbility")]
        [LabelWidth(150)]
        public bool getRangeFromAbility;
        [ShowIf("ShowRange")]
        [LabelWidth(100)]
        public int range;

        [Header("Ability Properties")]
        [ShowIf("ShowAbilityName")]
        [LabelWidth(100)]
        public string abilityName;

        public bool ShowTargettingPriority()
        {
            return actionType != AIActionType.DelayTurn;
        }

        public bool ShowAbilityName()
        {
            if (actionType == AIActionType.UseAbilityCharacterTarget ||
                actionType == AIActionType.UseCharacterTargettedSummonAbility ||
                actionType == AIActionType.MoveWithinRangeOfTarget && getRangeFromAbility)
            {
                return true;
            }
            return false;
        }

        public bool ShowRange()
        {
            if (actionType == AIActionType.MoveWithinRangeOfTarget && !getRangeFromAbility)
            {
                return true;
            }
            return false;
        }

        public bool ShowGetRangeFromAbility()
        {
            if (actionType == AIActionType.MoveWithinRangeOfTarget)
            {
                return true;
            }
            return false;
        }
    }

    public enum AIActionType
    {
        None = 0,
        UseAbilityCharacterTarget = 1,
        UseCharacterTargettedSummonAbility = 5,
        MoveToEngageInMelee = 2,
        MoveWithinRangeOfTarget = 3,
        MoveToElevationOrGrassCloserToTarget = 6,
        DelayTurn = 7
    }

    public enum TargettingPriority
    {
        None = 0,
        ClosestUnfriendlyTarget = 1,
        ClosestFriendlyTarget = 8,
        BestValidUnfriendlyTarget = 5,
        RandomValidUnfriendlyTarget = 6,
        RandomValidFriendlyTarget = 9,
        MostEndangeredValidFriendly = 7,
        RandomAlly = 3,
        RandomValidFriendlyOrSelf = 4,
        Self = 2
    }
}
