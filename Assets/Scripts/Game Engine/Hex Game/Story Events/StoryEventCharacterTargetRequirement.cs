using HexGameEngine.Characters;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace HexGameEngine.StoryEvents
{
    [Serializable]
    public class StoryEventCharacterTargetRequirement
    {
        #region Components + Variables
        [Header("Core Requirements")]
        public StoryEventCharacterTargetRequirementType reqType;
        [Space(10)]

        [Header("Background Requirements")]
        [ShowIf("ShowRequiredBackgrounds")]
        public CharacterBackground[] requiredBackgrounds;
        #endregion

        #region Odin Showifs
        public bool ShowRequiredBackgrounds()
        {
            return reqType == StoryEventCharacterTargetRequirementType.DoesNotHaveBackground ||
                reqType == StoryEventCharacterTargetRequirementType.HasBackground;
        }


        #endregion
    }

    [Serializable]
    public class StoryEventCharacterTarget
    {
        public StoryEventCharacterTargetRequirement[] requirements;
    }

    public enum StoryEventCharacterTargetRequirementType
    {
        None = 0,
        HasBackground = 1,
        DoesNotHaveBackground = 2,
        XorMoreHealth = 3,
        XorLessHealth = 4,

    }
    
}