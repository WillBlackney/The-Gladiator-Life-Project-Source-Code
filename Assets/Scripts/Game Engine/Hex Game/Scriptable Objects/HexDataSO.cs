using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.HexTiles
{
    [CreateAssetMenu(fileName = "New Hex Data", menuName = "Hex Data", order = 52)]
    public class HexDataSO : ScriptableObject
    {
        [Header("Asset References")]
        [PreviewField(75)]
        public Material tileMaterial;

        [Header("Properties")]
        public string tileName;
        [TextArea]
        public string tileDescription;
        public int moveCostModifier;
        public bool allowElevation;

       
    }
}