using WeAreGladiators.Abilities;
using WeAreGladiators.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.Characters
{
    [System.Serializable]
    public class RecruitLoadoutData
    {
        [Header("Loadout Settings")]
        public TalentSchool[] possibleStartingTalents;
        public List<AbilityDataSO> possibleAbilities = new List<AbilityDataSO>();
        public RecruitWeaponLoadout[] possibleWeaponLoadouts;
        public RecruitArmourLoadout[] possibleArmourLoadouts;
      
    }
}