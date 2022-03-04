using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.AI
{
    [System.Serializable]
    public class AIAction
    {
        [Header("Core Properties")]
        [LabelWidth(100)]
        public AIActionType actionType;
        [LabelWidth(100)]
        public TargettingPriority targettingPriority;

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

        public bool ShowAbilityName()
        {
            if (actionType == AIActionType.UseAbilityCharacterTarget ||
                actionType == AIActionType.UseCharacterTargettedSummonAbility ||
                (actionType == AIActionType.MoveIntoRangeOfTarget && getRangeFromAbility))
            {
                return true;
            }
            else return false;
        }

        public bool ShowRange()
        {
            if (actionType == AIActionType.MoveIntoRangeOfTarget && !getRangeFromAbility) return true;
            else return false;
        }

        public bool ShowGetRangeFromAbility()
        {
            if (actionType == AIActionType.MoveIntoRangeOfTarget) return true;
            else return false;
        }
    }

    public enum AIActionType
    {
        None = 0,
        UseAbilityCharacterTarget = 1,
        UseCharacterTargettedSummonAbility = 5,
        MoveToEngageInMelee = 2,
        MoveIntoRangeOfTarget = 3,
    }

    public enum TargettingPriority
    {
        None = 0,
        ClosestUnfriendlyTarget = 1,
        RandomAlly = 3,
        RandomAllyOrSelf = 4,
        Self = 2,
    }
}