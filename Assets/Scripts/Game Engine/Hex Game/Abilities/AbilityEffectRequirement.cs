using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Abilities
{
    [System.Serializable]
    public class AbilityEffectRequirement
    {
        public AbilityEffectRequirementType requirementType;

        [ShowIf("ShowPerk")]
        public Perk perk;

        public bool ShowPerk()
        {
            return requirementType == AbilityEffectRequirementType.TargetHasPerk ||
                requirementType == AbilityEffectRequirementType.CasterHasPerk ||
                requirementType == AbilityEffectRequirementType.CasterDoesNotHavePerk ||
                requirementType == AbilityEffectRequirementType.TargetDoesNotHavePerk;
        }
    }
    public enum AbilityEffectRequirementType
    {
        None = 0,
        BackStrike = 1,
        TargetHasPerk = 2,
        TargetDoesNotHavePerk = 3,
        CasterHasPerk = 4,
        CasterDoesNotHavePerk = 5,
    }
}