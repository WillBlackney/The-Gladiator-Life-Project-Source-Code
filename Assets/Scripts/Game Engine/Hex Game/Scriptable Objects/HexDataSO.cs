using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.HexTiles
{
    [CreateAssetMenu(fileName = "New Hex Data", menuName = "Hex Data", order = 52)]
    public class HexDataSO : ScriptableObject
    {
        [Header("Properties")]
        public string tileName;
        public List<CustomString> description;
        public ModalDotRowBuildData[] effectDescriptions;
        public int moveCostModifier;
        public bool allowElevation;
    }
}
