using HexGameEngine.UI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.Characters
{
    [CreateAssetMenu(fileName = "New BackgroundDataSO", menuName = "Background Data")]
    public class BackgroundDataSO : ScriptableObject
    {
        [HorizontalGroup("Core Data", 100)]
        [HideLabel]
        [PreviewField(100)]
        public Sprite backgroundSprite;

        [VerticalGroup("Core Data/Stats")]
        [LabelWidth(100)]
        public CharacterBackground backgroundType;

        [VerticalGroup("Core Data/Stats")]
        [LabelWidth(100)]
        [TextArea]
        public string description;

        [VerticalGroup("Core Data/Stats")]
        [LabelWidth(100)]
        [Range(1,100)]
        public int spawnChance = 25;

        [BoxGroup("Level Data", true, true)]
        [LabelWidth(150)]
        [Range(1,6)]
        public int lowerLevelLimit;
        [BoxGroup("Level Data")]
        [LabelWidth(150)]
        [Range(1, 6)]
        public int upperLevelLimit;

        [BoxGroup("Cost Data", true, true)]
        [LabelWidth(100)]
        public int dailyWageMin;
        [BoxGroup("Cost Data")]
        [LabelWidth(100)]
        public int dailyWageMax;
        [BoxGroup("Cost Data")]
        [LabelWidth(100)]
        public int baseRecruitCost;

        [BoxGroup("Stat Ranges", true, true)]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int mightLower;
        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int mightUpper;
        [Space(10)]

        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int constitutionLower;
        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int constitutionUpper;
        [Space(10)]

        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int accuracyLower;
        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int accuracyUpper;
        [Space(10)]

        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int dodgeLower;
        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int dodgeUpper;
        [Space(10)]       

        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int fatigueLower;
        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int fatigueUpper;
        [Space(10)]

        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int resolveLower;
        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int resolveUpper;
        [Space(10)]

        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int witsLower;
        [BoxGroup("Stat Ranges")]
        [LabelWidth(125)]
        [Range(-30, 30)]
        public int witsUpper;
        [Space(10)]     


        [BoxGroup("Recruit Generation Settings", true, true)]
        [LabelWidth(100)]
        public CharacterRace[] validRaces;
        [BoxGroup("Recruit Generation Settings")]
        [LabelWidth(100)]
        public RecruitLoadoutData[] loadoutBuckets;

        [BoxGroup("Misc Data", true, true)]
        [LabelWidth(100)]
        public ModalDotRowBuildData[] passiveEffectDescriptions;

    }
    public  class BackgroundData
    {
        #region Properties
        public Sprite backgroundSprite;
        public CharacterBackground backgroundType;
        public string description;
        public int spawnChance;
        public int dailyWageMin;
        public int dailyWageMax;
        public int baseRecruitCost;

        public int mightLower;
        public int mightUpper;
        public int accuracyLower;
        public int accuracyUpper;
        public int dodgeLower;
        public int dodgeUpper;
        public int constitutionLower;
        public int constitutionUpper;   
        public int fatigueLower;
        public int fatigueUpper;
        public int resolveLower;
        public int resolveUpper;
        public int witsLower;
        public int witsUpper;

        public int lowerLevelLimit;
        public int upperLevelLimit;

        public List<RecruitLoadoutData> loadoutBuckets = new List<RecruitLoadoutData>();
        public List<CharacterRace> validRaces = new List<CharacterRace>();
        public List<ModalDotRowBuildData> passiveEffectDescriptions = new List<ModalDotRowBuildData>();

        #endregion

        #region Getters + Accessors
        public Sprite BackgroundSprite
        {
            get
            {
                if (backgroundSprite == null)
                {
                    backgroundSprite = GetMySprite();
                    return backgroundSprite;
                }
                else
                {
                    return backgroundSprite;
                }
            }
        }
        private Sprite GetMySprite()
        {
            Sprite s = null;

            foreach (BackgroundData i in CharacterDataController.Instance.AllCharacterBackgrounds)
            {
                if (i.backgroundType == backgroundType)
                {
                    s = i.backgroundSprite;
                    break;
                }
            }

            if (s == null)
                Debug.LogWarning("BackgroundData.GetMySprite() could not sprite for background " + backgroundType + ", returning null...");


            return s;
        }
        #endregion

        #region Logic
        public BackgroundData(BackgroundDataSO data)
        {
            backgroundSprite = data.backgroundSprite;
            backgroundType = data.backgroundType;
            description = data.description;
            spawnChance = data.spawnChance;
            dailyWageMin = data.dailyWageMin;
            dailyWageMax = data.dailyWageMax;
            baseRecruitCost = data.baseRecruitCost;
            lowerLevelLimit = data.lowerLevelLimit;
            upperLevelLimit = data.upperLevelLimit;

            mightLower = data.mightLower;
            mightUpper = data.mightUpper;

            constitutionLower = data.constitutionLower;
            constitutionUpper = data.constitutionUpper;

            fatigueLower = data.fatigueLower;
            fatigueUpper = data.fatigueUpper;

            accuracyLower = data.accuracyLower;
            accuracyUpper = data.accuracyUpper;

            dodgeLower = data.dodgeLower;
            dodgeUpper = data.dodgeUpper;

            resolveLower = data.resolveLower;
            resolveUpper = data.resolveUpper;

            witsLower = data.witsLower;
            witsUpper = data.witsUpper;

            foreach (RecruitLoadoutData ld in data.loadoutBuckets)
                loadoutBuckets.Add(ld);

            foreach (CharacterRace race in data.validRaces)
                validRaces.Add(race);

            foreach(ModalDotRowBuildData d in data.passiveEffectDescriptions)
                passiveEffectDescriptions.Add(d);

        }
        #endregion

    }
    public enum CharacterBackground
    {
        None = 0,
        Gladiator = 1,
        Farmer = 2,
        Lumberjack = 3,
        Slave = 4,
        Scholar = 5,
        Zealot = 6,
        Thief = 7,
        Outlaw = 8,
        Doctor = 9,
        Labourer = 10,
        Unknown = 11,
        RetiredWarrior = 12,
        TournamentKnight = 13,
        Mercenary = 14,
        Assassin = 15,
        Poacher = 16,
        Inquisitor = 17,


    }
}