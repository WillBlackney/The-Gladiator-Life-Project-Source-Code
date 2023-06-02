using HexGameEngine.Boons;
using HexGameEngine.Characters;
using HexGameEngine.Items;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HexGameEngine.StoryEvents
{
    [System.Serializable]
    public class StoryChoiceEffect
    {
        // General Fields
        [Header("General Settings")]
        [LabelWidth(100)]
        public StoryChoiceEffectType effectType;

        // Load page fields
        [ShowIf("ShowPageToLoad")]
        [Header("Page Settings")]
        [LabelWidth(100)]
        public StoryEventPageSO pageToLoad;

        [ShowIf("ShowBoonTag")]
        [Header("Boon Settings")]
        [LabelWidth(100)]
        public BoonTag boonGained;

        [ShowIf("ShowRecruitFields")]
        [Header("Recruit Settings")]
        [LabelWidth(200)]
        public CharacterBackground backgroundAddedToTavern;

        [ShowIf("ShowRecruitFields")]
        [Range(0,5)]
        [LabelWidth(200)]
        public int totalCharactersAddedToTavern;

        #region Odin Show Ifs  
        public bool ShowPageToLoad()
        {
            return effectType == StoryChoiceEffectType.LoadPage;
        }
        public bool ShowBoonTag()
        {
            return effectType == StoryChoiceEffectType.GainBoon;
        }
        public bool ShowRecruitFields()
        {
            return effectType == StoryChoiceEffectType.AddRecruitsToTavern;
        }
        #endregion

    }

    [System.Serializable]
    public class StoryChoiceEffectSet
    {
        [Range(0,100)]
        public int lowerProbability = 0;
        [Range(0, 100)]
        public int upperProbability = 100;
        public StoryChoiceEffect[] effects;
    }
}