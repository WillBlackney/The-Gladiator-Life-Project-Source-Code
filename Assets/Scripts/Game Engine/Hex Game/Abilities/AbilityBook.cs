using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Abilities
{
    public class AbilityBook
    {
        public List<AbilityData> allKnownAbilities = new List<AbilityData>();
    }
    [System.Serializable]
    public class SerializedAbilityBook
    {
        // Class is used to assign abilities to characters templates / enemy data models in the inspector
        public List<AbilityDataSO> activeAbilities = new List<AbilityDataSO>();
    }
}
