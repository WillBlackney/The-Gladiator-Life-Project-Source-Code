using HexGameEngine.Abilities;
using HexGameEngine.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Characters
{
    [System.Serializable]
    public class RecruitLoadoutData
    {
        [Header("Loadout Settings")]
        public TalentSchool[] possibleStartingTalents;
        public List<AbilityDataSO> possibleAbilities = new List<AbilityDataSO>();
        public SerializedItemSet[] possibleWeaponLoadouts;
        public SerializedItemSet[] possibleArmourLoadouts;
      
    }
}