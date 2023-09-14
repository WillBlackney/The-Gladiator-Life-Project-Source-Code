using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace WeAreGladiators.HexTiles
{
    [CreateAssetMenu(fileName = "New Hex Map Seed", menuName = "Hex Map Seed", order = 52)]
    public class HexMapSeedDataSO : ScriptableObject
    {
        [Header("Shape + Size")]
        public HexMapShape shape;

        [ShowIf("ShowRadius")]
        public int mapRadius;

        [ShowIf("ShowWidth")]
        public int width;

        [ShowIf("ShowHeight")]
        public int height;

        [Header("Elevation + Obstacles")]
        [Range(0, 100)]
        public int elevationPercentage;
        public HexMapTilingConfig[] tilingConfigs;

        [Range(0, 100)]
        public int obstructionPercentage;
        public bool allowObstaclesOnElevation = true;

        // Odin Show If's
        #region

        public bool ShowRadius()
        {
            return shape == HexMapShape.Hexagonal;
        }
        public bool ShowHeight()
        {
            return shape == HexMapShape.Rectangular;
        }
        public bool ShowWidth()
        {
            return shape == HexMapShape.Rectangular;
        }

        #endregion
    }

    [Serializable]
    public class HexMapTilingConfig
    {
        public HexDataSO hexData;
        [Range(1, 100)]
        public int spawnChance;
    }
}
