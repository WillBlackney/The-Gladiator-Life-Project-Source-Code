using System;
using UnityEngine;

namespace WeAreGladiators.HexTiles
{
    [CreateAssetMenu(fileName = "New Combat Map Seed", menuName = "Combat Map Seed", order = 52)]
    public class CombatMapSeedDataSO : ScriptableObject
    {
        [Header("Elevation Settings")]
        [Range(0, 100)]
        public int elevationPercentage;
        public int maximumElevations = 5;

        [Space(10)]
        [Header("Tile Type Settings")]
        public HexDataSO defaultTile;
        public HexMapTilingConfig[] tilingConfigs;

        [Header("Obstruction Settings")]
        [Range(0, 100)]
        public int obstructionPercentage;
        public bool allowObstaclesOnElevation = true;
        public int maximumObstructions = 5;
    }

    [Serializable]
    public class CombatMapTilingConfig
    {
        public HexDataSO hexData;
        [Range(1, 100)]
        public int lowerProbability;
        [Range(1, 100)]
        public int upperProbability;
    }

}
