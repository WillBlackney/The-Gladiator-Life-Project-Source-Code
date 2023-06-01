using HexGameEngine.Boons;
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
        public StoryChoiceEffectType effectType;

        // Load page fields
        [ShowIf("ShowPageToLoad")]
        [Header("Page Settings")]
        public StoryEventPageSO pageToLoad;

        [ShowIf("ShowBoonTag")]
        [Header("Boon Settings")]
        public BoonTag boonGained;

        #region Odin Show Ifs  
        public bool ShowPageToLoad()
        {
            return effectType == StoryChoiceEffectType.LoadPage;
        }
        public bool ShowBoonTag()
        {
            return effectType == StoryChoiceEffectType.GainBoon;
        }
        #endregion

    }
}