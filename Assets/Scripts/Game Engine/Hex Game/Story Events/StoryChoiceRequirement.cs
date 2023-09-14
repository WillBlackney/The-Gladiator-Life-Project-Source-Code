using System;
using Sirenix.OdinInspector;
using UnityEngine;
using WeAreGladiators.Characters;

namespace WeAreGladiators.StoryEvents
{
    [Serializable]
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
