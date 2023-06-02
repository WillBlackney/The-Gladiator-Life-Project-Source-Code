using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using HexGameEngine.Utilities;

namespace HexGameEngine.StoryEvents
{
    [CreateAssetMenu(fileName = "New StoryEventChoiceSO", menuName = "StoryEventChoice", order = 52)]
    public class StoryEventChoiceSO : ScriptableObject
    {
        [Header("Descriptions")]
        public List<CustomString> choiceTextOnButton;
        public StoryChoiceEffectSet[] effectSets;
        public StoryChoiceRequirement[] requirements;        
    }

    public enum ChoiceEffectTarget
    {
        None = 0,
        SelectedCharacter = 1,
        AllCharacters = 2,
    }
    public enum HealType
    {
        None = 0,
        HealFlatAmount = 1,
        HealPercentage = 2,
        HealMaximum = 3,
    }
    public enum ItemRewardType
    {
        RandomItem = 0,
        SpecificItem = 1,
    }

    public enum StoryChoiceReqType
    {
        None = 0,
        AtleastXHealthFlat = 1,
        AtleastXHealthPercent = 2,
        AttributeLevel = 3,
        GoldAmount = 4,
        TalentLevel = 5,
        Race = 6,
    }
    public enum StoryChoiceEffectType
    {
        None = 0,
        LoadPage = 1,
        FinishEvent = 2,
        ModifyGold = 3,
        GainBoon = 4,
        AddRecruitsToTavern = 6,
    }
}