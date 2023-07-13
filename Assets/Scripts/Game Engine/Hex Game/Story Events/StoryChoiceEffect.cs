using WeAreGladiators.Boons;
using WeAreGladiators.Characters;
using WeAreGladiators.Items;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.StoryEvents
{
    [System.Serializable]
    public class StoryChoiceEffect
    {
        // General Fields
        [Header("General Settings")]
        [LabelWidth(150)]
        public StoryChoiceEffectType effectType;
        [Space(10)]

        [Header("Target Settings")]
        [ShowIf("ShowCharacterTargetIndex")]
        [LabelWidth(150)]
        [Range(0, 9)]
        public int characterTargetIndex = 0;
        [Space(10)]

        // Load page fields
        [ShowIf("ShowPageToLoad")]
        [Header("Page Settings")]
        [LabelWidth(150)]
        public StoryEventPageSO pageToLoad;

        [ShowIf("ShowBoonTag")]
        [Header("Boon Settings")]
        [LabelWidth(150)]
        public BoonTag boonGained;

        [ShowIf("ShowRecruitFields")]
        [Header("Recruit Settings")]
        [LabelWidth(200)]
        public CharacterBackground backgroundAddedToTavern;

        [ShowIf("ShowRecruitFields")]
        [Range(0,5)]
        [LabelWidth(200)]
        public int totalCharactersAddedToTavern;

        [ShowIf("ShowItemGained")]
        [LabelWidth(150)]
        public ItemDataSO itemGained;

        [ShowIf("ShowGoldGained")]
        [LabelWidth(150)]
        public int goldGained;

        [ShowIf("ShowGoldLost")]
        [LabelWidth(150)]
        public int goldLost;

        [Header("Perk Settings")]
        [ShowIf("ShowPerkGained")]
        [LabelWidth(150)]
        public Perk perkGained;

        [ShowIf("ShowPerkGainedChance")]
        [LabelWidth(150)]
        [Range(0, 100)]
        public int gainPerkChance = 100;

        [Header("Injury Settings")]
        [ShowIf("ShowInjurySeverity")]
        [LabelWidth(150)]
        public InjurySeverity injurySeverity = InjurySeverity.None;

        [ShowIf("ShowInjuryType")]
        [LabelWidth(150)]
        public InjuryType injuryType = InjuryType.None;

        [ShowIf("ShowExperienceGained")]
        [LabelWidth(150)]
        public int experienceGained = 0;

        [ShowIf("ShowCharacterJoining")]
        [LabelWidth(150)]
        public HexCharacterTemplateSO characterJoining;

        [Header("Stress Recovery Settings")]
        [ShowIf("ShowStressRecoverySettings")]
        [LabelWidth(150)]
        [Range(0, 100)]
        public int stressRecoveryChance = 100;

        [ShowIf("ShowStressRecoverySettings")]
        [LabelWidth(150)]
        public int stressRecoveredMin = 0;

        [ShowIf("ShowStressRecoverySettings")]
        [LabelWidth(150)]
        public int stressRecoveredMax = 0;

        [ShowIf("ShowHealthLost")]
        [LabelWidth(150)]
        [Range(0.01f,1f)]
        public float healthLostPercentage = 0.5f;

        [ShowIf("ShowWageIncreasePercentage")]
        [LabelWidth(150)]
        [Range(0.1f, 1f)]
        public float wageIncreasePercentage = 0.1f;

        [ShowIf("ShowCharacterLeaveProbability")]
        [LabelWidth(150)]
        [Range(0, 100)]
        public int characterLeaveProbability = 50;

        #region Odin Show Ifs  
        public bool ShowCharacterLeaveProbability()
        {
            return effectType == StoryChoiceEffectType.CharactersLeave;
        }
        public bool ShowWageIncreasePercentage()
        {
            return effectType == StoryChoiceEffectType.IncreaseDailyWageAll;
        }
        public bool ShowHealthLost()
        {
            return effectType == StoryChoiceEffectType.LoseHealth;
        }
        public bool ShowStressRecoverySettings()
        {
            return effectType == StoryChoiceEffectType.RecoverStressAll || effectType == StoryChoiceEffectType.GainStressAll;
        }
        public bool ShowInjuryType()
        {
            return effectType == StoryChoiceEffectType.GainRandomInjury;
        }
        public bool ShowInjurySeverity()
        {
            return effectType == StoryChoiceEffectType.GainRandomInjury;
        }
        public bool ShowCharacterTargetIndex()
        {
            return effectType == StoryChoiceEffectType.CharacterKilled || 
                effectType == StoryChoiceEffectType.GainPerk ||
                effectType == StoryChoiceEffectType.GainRandomInjury ||
                effectType == StoryChoiceEffectType.GainExperience ||
                effectType == StoryChoiceEffectType.LoseHealth;
        }
        public bool ShowExperienceGained()
        {
            return effectType == StoryChoiceEffectType.GainExperience;
        }
        public bool ShowCharacterJoining()  
        {
            return effectType == StoryChoiceEffectType.CharacterJoinsRoster;
        }
        public bool ShowGoldLost()
        {
            return effectType == StoryChoiceEffectType.LoseGold;
        }
        public bool ShowPerkGained()
        {
            return effectType == StoryChoiceEffectType.GainPerk ||
                effectType == StoryChoiceEffectType.GainPerkAll;
        }
        public bool ShowPerkGainedChance()
        {
            return effectType == StoryChoiceEffectType.GainPerk ||
                effectType == StoryChoiceEffectType.GainPerkAll ||
                effectType == StoryChoiceEffectType.GainRandomInjury;
        }
        public bool ShowGoldGained()
        {
            return effectType == StoryChoiceEffectType.GainGold;
        }
        public bool ShowItemGained()
        {
            return effectType == StoryChoiceEffectType.GainItem;
        }
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

    public enum StoryChoiceEffectType
    {
        None = 0,
        LoadPage = 1,
        FinishEvent = 2,
        GainGold = 3,
        LoseGold = 12,
        LoseAllGold = 9,
        GainBoon = 4,
        GainItem = 8,
        AddRecruitsToTavern = 6,
        CharacterKilled = 7,
        GainPerk = 10,
        GainPerkAll = 11,
        CharacterJoinsRoster = 13,
        GainExperience = 14,
        GainRandomInjury = 15,
        LoseHealth = 16,
        RecoverStressAll = 17,
        GainStressAll = 18,
        IncreaseDailyWageAll = 19,
        CharactersLeave = 20,
    }
}