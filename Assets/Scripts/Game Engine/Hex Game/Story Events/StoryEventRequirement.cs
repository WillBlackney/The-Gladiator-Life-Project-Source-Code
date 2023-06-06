using HexGameEngine.Boons;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.StoryEvents
{
    [System.Serializable]
    public class StoryEventRequirement
    {
        [LabelWidth(100)]
        public StoryEventRequirementType reqType;

        [Space(10)]

        [Header("Character Requirement Settings")]
        [ShowIf("ShowRequiredCharactersInRosterCount")]
        [LabelWidth(100)]
        public int requiredCharactersInRosterCount;

        [ShowIf("ShowIncludeTheKid")]
        [LabelWidth(100)]
        public bool includeTheKid = false;

        [ShowIf("ShowGoldRequired")]
        [LabelWidth(100)]
        public int goldRequired;

        [ShowIf("ShowRequiredBoon")]
        [LabelWidth(100)]
        public BoonTag requiredBoon;

        #region Odin Showifs
        public bool ShowRequiredBoon()
        {
            return reqType == StoryEventRequirementType.HasBoon ||
                reqType == StoryEventRequirementType.DoesNotHaveBoon;
        }
        public bool ShowGoldRequired()
        {
            return reqType == StoryEventRequirementType.HasXorLessGold ||
                reqType == StoryEventRequirementType.HasXorMoreGold;
        }
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
        HasXorMoreGold = 3,
        HasXorLessGold = 4,
        DoesNotHaveBoon = 5,
        HasBoon = 6,
    }
}