using WeAreGladiators.StoryEvents;
using WeAreGladiators;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Characters;

namespace WeAreGladiators.StoryEvents
{
    [System.Serializable]
    public class StoryChoiceRequirement
    {
        [Header("Core Data")]
        public StoryChoiceReqType requirementType;

        // Health 
        [ShowIf("ShowRequiredBackground")]
        [Header("Health Data")]
        public CharacterBackground requiredBackground;

        // Odin Showifs
        #region
        public bool ShowRequiredBackground()
        {
            return requirementType == StoryChoiceReqType.CharacterWithBackground;
        }
        #endregion
    }
}