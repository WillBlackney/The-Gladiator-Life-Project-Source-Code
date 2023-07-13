using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using WeAreGladiators.Utilities;
using UnityEngine.UI;
using Sirenix.OdinInspector;

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

    [System.Serializable]
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
        Green = 3,
    }

}