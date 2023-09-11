using WeAreGladiators.Characters;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WeAreGladiators.StoryEvents
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

        [Header("Perk Requirements")]
        [ShowIf("ShowPerk")]
        public Perk perk;
        #endregion

        #region Odin Showifs
        public bool ShowRequiredBackgrounds()
        {
            return reqType == StoryEventCharacterTargetRequirementType.DoesNotHaveBackground ||
                reqType == StoryEventCharacterTargetRequirementType.HasBackground;
        }
        public bool ShowPerk()
        {
            return reqType == StoryEventCharacterTargetRequirementType.DoesNotHavePerk;
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
        DoesNotHavePerk = 5,

    }
    
}