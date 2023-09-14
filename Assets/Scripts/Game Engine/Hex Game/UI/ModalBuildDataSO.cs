using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    [CreateAssetMenu(fileName = "New Modal Build Data", menuName = "Modal Build Data", order = 52)]
    public class ModalBuildDataSO : ScriptableObject
    {
        public ModalBuildPreset myPreset;
        [PreviewField(75)]
        public Sprite mainSprite;
        public bool frameSprite = true;
        public string headerName;
        public bool italicDescription = true;
        public List<CustomString> description;
        public ModalDotRowBuildData[] infoRows;
    }

    [Serializable]
    public class ModalDotRowBuildData
    {
        public DotStyle dotStyle;
        public List<CustomString> message;
    }

    public enum DotStyle
    {
        None = 0,
        Neutral = 1,
        Red = 2,
        Green = 3
    }

}
