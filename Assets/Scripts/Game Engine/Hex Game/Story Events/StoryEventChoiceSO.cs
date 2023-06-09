using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using HexGameEngine.Utilities;

namespace HexGameEngine.StoryEvents
{
    [CreateAssetMenu(fileName = "New StoryEventChoiceSO", menuName = "StoryEventChoice", order = 52)]
    public class StoryEventChoiceSO : ScriptableObject
    {
        #region Components + Variables
        [Header("UI Settings")]
        public List<CustomString> choiceTextOnButton;

        [Space(10)]

        [Header("Effects and Requirements")]
        public StoryChoiceEffectSet[] effectSets;
        public StoryChoiceRequirement[] requirements;
        #endregion
    }
    public enum StoryChoiceReqType
    {
        None = 0,
        CharacterWithBackground = 1,
    }   
}