using HexGameEngine.StoryEvents;
using HexGameEngine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.StoryEvents
{
    [System.Serializable]
    public class StoryChoiceRequirement
    {
        [Header("Core Data")]
        public StoryChoiceReqType requirementType;

        // Health 
        [ShowIf("ShowFlatHealth")]
        [Header("Health Data")]
        public int healthMinimum;
        [ShowIf("ShowPercentHealth")]
        [Header("Health Data")]
        [Range(1, 100)]
        public int healthPercentMinimum;

        // Gold
        [ShowIf("ShowGoldFields")]
        [Header("Gold Data")]
        public int goldMinimum;

        // Talent
        [ShowIf("ShowTalentFields")]
        [Header("Talent Data")]
        public TalentSchool talent;
        [ShowIf("ShowTalentFields")]
        [Range(1, 2)]
        public int talentLevel;

        // Attribute
        [ShowIf("ShowAttributeFields")]
        [Header("Attribute Data")]
        public CoreAttribute attribute;
        [ShowIf("ShowAttributeFields")]
        [Range(10, 30)]
        public int attributeLevel;

        // Racial
        [ShowIf("ShowRequiredRace")]
        [Header("Racial Data")]
        public CharacterRace requiredRace;

        // Odin Showifs
        #region
        public bool ShowRequiredRace()
        {
            return requirementType == StoryChoiceReqType.Race;
        }
        public bool ShowTalentFields()
        {
            return requirementType == StoryChoiceReqType.TalentLevel;
        }
        public bool ShowAttributeFields()
        {
            return requirementType == StoryChoiceReqType.AttributeLevel;
        }
        public bool ShowFlatHealth()
        {
            return requirementType == StoryChoiceReqType.AtleastXHealthFlat;
        }
        public bool ShowPercentHealth()
        {
            return requirementType == StoryChoiceReqType.AtleastXHealthPercent;
        }
        public bool ShowGoldFields()
        {
            return requirementType == StoryChoiceReqType.GoldAmount;
        }
        #endregion
    }
}