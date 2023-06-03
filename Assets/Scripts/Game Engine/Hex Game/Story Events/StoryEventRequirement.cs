using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.StoryEvents
{
    [System.Serializable]
    public class StoryEventRequirement
    {
        public StoryEventRequirementType reqType;

        [Space(10)]

        [Header("Character Requirement Settings")]
        [ShowIf("ShowRequiredCharactersInRosterCount")]
        [LabelWidth(100)]
        public int requiredCharactersInRosterCount;

        [ShowIf("ShowIncludeTheKid")]
        [LabelWidth(100)]
        public bool includeTheKid = false;

        #region Odin Showifs
        public bool ShowRequiredCharactersInRosterCount()
        {
            return reqType == StoryEventRequirementType.XorMoreCharactersInRoster ||
                reqType == StoryEventRequirementType.XorLessCharactersInRoster;
        }
        public bool ShowIncludeTheKid()
        {
            return reqType == StoryEventRequirementType.XorMoreCharactersInRoster ||
                reqType == StoryEventRequirementType.XorLessCharactersInRoster;
        }
        #endregion
    }

    public enum StoryEventRequirementType
    {
        None = 0,
        XorMoreCharactersInRoster = 1,
        XorLessCharactersInRoster = 2,
    }
}