using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.TownFeatures
{
    public class TownPopulationController : Singleton<TownPopulationController>
    {
        // In future when there are more towns than breedmarsh, this should not be set in the inspector: set programmitcally depending on current zone
        [Header("Misc")]
        [SerializeField] private TownCharacterSeed currentTownHive;

        [Header("Components")]
        [SerializeField] private TownCharacterView townCharacterPrefab;


    }
}