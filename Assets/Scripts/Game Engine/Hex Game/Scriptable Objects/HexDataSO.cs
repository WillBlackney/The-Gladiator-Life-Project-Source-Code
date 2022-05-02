using HexGameEngine.Utilities;
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
        public Sprite tileSprite;

        [Header("Properties")]
        public string tileName;
        public List<CustomString> description;
        public int moveCostModifier;
        public bool allowElevation;

       
    }
}