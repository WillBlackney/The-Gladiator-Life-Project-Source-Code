using HexGameEngine.Characters;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.StoryEvents
{
    [System.Serializable]
    public class StoryEventCharacterTargetRequirement
    {
        [Header("Core Requirements")]
        public StoryEventCharacterTargetRequirementType reqType;
        [Space(10)]

        [Header("Background Requirements")]
        [ShowIf("ShowRequiredBackgrounds")]
        public CharacterBackground[] requiredBackgrounds;

        #region Odin Showifs
        public bool ShowRequiredBackgrounds()
        {
            return reqType == StoryEventCharacterTargetRequirementType.DoesNotHaveBackground ||
                reqType == StoryEventCharacterTargetRequirementType.HasBackground;
        }


        #endregion
    }

    public enum StoryEventCharacterTargetRequirementType
    {
        None = 0,
        HasBackground = 1,
        DoesNotHaveBackground = 2,
        XorMoreHealth = 3,
        XorLessHealth = 4,

    }
    [System.Serializable]
    public class StoryEventCharacterTarget
    {
        public StoryEventCharacterTargetRequirement[] requirements;
    }
}