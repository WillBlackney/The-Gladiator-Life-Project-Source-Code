using WeAreGladiators.UI;
using WeAreGladiators.Utilities;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.HexTiles
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
        public ModalDotRowBuildData[] effectDescriptions;
        public int moveCostModifier;
        public bool allowElevation;

       
    }
}