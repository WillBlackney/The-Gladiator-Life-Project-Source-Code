using System;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Abilities;
using WeAreGladiators.Items;

namespace WeAreGladiators.Characters
{
    [Serializable]
    public class RecruitLoadoutData
    {
        [Header("Loadout Settings")]
        public TalentSchool[] possibleStartingTalents;
        public List<AbilityDataSO> possibleAbilities = new List<AbilityDataSO>();
        public RecruitWeaponLoadout[] possibleWeaponLoadouts;
        public RecruitArmourLoadout[] possibleArmourLoadouts;
    }
}
