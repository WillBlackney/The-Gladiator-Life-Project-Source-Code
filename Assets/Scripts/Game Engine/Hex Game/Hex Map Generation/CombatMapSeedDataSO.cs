using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HexGameEngine.HexTiles
{
    [CreateAssetMenu(fileName = "New Combat Map Seed", menuName = "Combat Map Seed", order = 52)]
    public class CombatMapSeedDataSO : ScriptableObject
    {
        [Header("Elevation Settings")]
        [Range(0, 100)]
        public int elevationPercentage;
        [Space(10)]
        [Header("Tile Type Settings")]
        public HexMapTilingConfig[] tilingConfigs;

        [Header("Obstruction Settings")]
        [Range(0, 100)]
        public int obstructionPercentage;
        public bool allowObstaclesOnElevation = true;

    }

    
    [System.Serializable]
    public class CombatMapTilingConfig
    {
        public HexDataSO hexData;
        [Range(1, 100)]
        public int lowerProbability;
        [Range(1, 100)]
        public int upperProbability;
    }
    
}
