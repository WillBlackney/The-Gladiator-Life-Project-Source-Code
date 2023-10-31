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

        [ShowIf("ShowMovementType")]
        [LabelWidth(150)]
        public MovementType movementType;

        [ShowIf("ShowGetRangeFromAbility")]
        [LabelWidth(150)]
        public bool getRangeFromAbility;
        [ShowIf("ShowRange")]
        [LabelWidth(100)]
        public int range;

        [ShowIf("ShowMaxMoveDistance")]
        [LabelWidth(100)]
        public int maxMoveDistance;

        [Header("Ability Properties")]
        [ShowIf("ShowAbilityName")]
        [LabelWidth(100)]
        public string abilityName;

        [ShowIf("ShowTeleportAbilityName")]
        [LabelWidth(100)]
        public string teleportAbilityName;

        public bool ShowTargettingPriority()
        {
            return actionType != AIActionType.DelayTurn && actionType != AIActionType.MoveToNearbyElevation;
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

        public bool ShowMaxMoveDistance()
        {
            return actionType == AIActionType.MoveToNearbyElevation;
        }

        public bool ShowGetRangeFromAbility()
        {
            if (actionType == AIActionType.MoveWithinRangeOfTarget)
            {
                return true;
            }
            return false;
        }

        public bool ShowMovementType()
        {
            return actionType == AIActionType.MoveWithinRangeOfTarget ||
                actionType == AIActionType.MoveToElevationOrGrassCloserToTarget ||
               actionType == AIActionType.MoveToEngageInMelee;
        }

        public bool ShowTeleportAbilityName()
        {
            return (actionType == AIActionType.MoveWithinRangeOfTarget ||
                actionType == AIActionType.MoveToElevationOrGrassCloserToTarget ||
               actionType == AIActionType.MoveToEngageInMelee) && movementType == MovementType.Teleport;
        }
    }

    public enum MovementType
    {
        Move = 0,
        Teleport = 1,
    }

    public enum AIActionType
    {
        None = 0,
        UseAbilityCharacterTarget = 1,
        UseCharacterTargettedSummonAbility = 5,
        MoveToEngageInMelee = 2,
        MoveWithinRangeOfTarget = 3,
        MoveToElevationOrGrassCloserToTarget = 6,
        MoveToNearbyElevation = 9,
        DelayTurn = 7,
        ChargeAndStun = 8,
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
