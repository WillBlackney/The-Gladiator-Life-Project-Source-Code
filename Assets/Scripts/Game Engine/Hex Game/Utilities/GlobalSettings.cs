using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using HexGameEngine.HexTiles;
using HexGameEngine.Characters;

namespace HexGameEngine.Utilities
{
    public class GlobalSettings : Singleton<GlobalSettings>
    {
        // Properties
        #region
        [Header("General Settings")]
        [LabelWidth(200)]
        [SerializeField] private GameMode gameMode;
        [SerializeField] private bool enableDebugLogs;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [SerializeField] private bool preventAudioProfiles = true;

        [Title("Character Settings")]
        [SerializeField] private int startingXpBonus;

        [Title("Combat Settings")]
        [SerializeField] private int baseHitChance;
        [SerializeField] private int startingDeploymentLimit = 3;

        [Title("Resource Settings")]
        [SerializeField] private int baseStartingGold;
        [SerializeField] private int campActivityPointRegen;

        [Title("Starting Timeline Settings")]
        [Range(1,5)]
        [ShowIf("ShowStartingTimeSettings")]
        [SerializeField] private int startingDay = 1;
        [Range(1, 5)]
        [ShowIf("ShowStartingTimeSettings")]
        [SerializeField] private int startingChapter = 1;
        
        // Single Combat Scene Properties
        [Title("Combat Sandbox Settings")]      
        [ShowIf("ShowSandBoxLevelSeed")]
        [SerializeField] private HexMapSeedDataSO sandboxLevelSeed;

        [ShowIf("ShowEnemyVsEnemyMode")]
        [SerializeField] private bool enemyVsEnemyMode = false;

        [ShowIf("ShowRandomizeCharacters")]
        [SerializeField] private bool randomizePlayerCharacters = false;

        [ShowIf("ShowPlayerAiCharacters")]
        [SerializeField] private EnemyEncounterSO playerAiCharacters;

        [ShowIf("ShowChosenCharacterTemplates")]
        [SerializeField] private HexCharacterTemplateSO[] chosenCharacterTemplates;

        [ShowIf("ShowPossibleRandomCharacters")]
        [SerializeField] private HexCharacterTemplateSO[] possibleRandomCharacters;

        [ShowIf("ShowTotalCharacters")]
        [SerializeField] private int totalCharacters;

        [ShowIf("ShowSandboxEnemyEncounter")]
        [SerializeField] private EnemyEncounterSO[] sandboxEnemyEncounters;

       
        #endregion

        // Getters + Accessors
        #region
        public GameMode GameMode
        {
            get { return gameMode; }
        }
        public int StartingDay
        {
            get { return startingDay; }
        }
        public int StartingXpBonus
        {
            get { return startingXpBonus; }
        }
        public int StartingChapter
        {
            get { return startingChapter; }
        }
        public HexMapSeedDataSO SandboxLevelSeed
        {
            get { return sandboxLevelSeed; }
        }      
        public EnemyEncounterSO[] SandboxEnemyEncounters
        {
            get { return sandboxEnemyEncounters; }
        }
        public EnemyEncounterSO PlayerAiCharacters
        {
            get { return playerAiCharacters; }
        }
        public bool RandomizePlayerCharacters
        {
            get { return randomizePlayerCharacters; }
        }
        public int TotalCharacters
        {
            get { return totalCharacters; }
        }
        public bool EnemyVsEnemyMode
        {
            get { return enemyVsEnemyMode; }
        }
        public int CampActivityPointRegen
        {
            get { return campActivityPointRegen; }
        }
        public int BaseStartingGold
        {
            get { return baseStartingGold; }
        }
        public int StartingDeploymentLimit
        {
            get { return startingDeploymentLimit; }
        }
        public HexCharacterTemplateSO[] ChosenCharacterTemplates
        {
            get { return chosenCharacterTemplates; }
        }
        public HexCharacterTemplateSO[] PossibleRandomCharacters
        {
            get { return possibleRandomCharacters; }
        }
        public int BaseHitChance
        {
            get { return baseHitChance; }
        }

        #endregion

        // Logic
        #region
        protected override void Awake()
        {
            base.Awake();
            if (enableDebugLogs)
                Debug.unityLogger.logEnabled = true;
            else if (!enableDebugLogs)
                Debug.unityLogger.logEnabled = false;
        }
        public void ApplyStartingXPBonus()
        {
            foreach(HexCharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
            {
                CharacterDataController.Instance.HandleGainXP(character, StartingXpBonus);
            }
        }
        #endregion

        // Odin Showifs
        #region
        public bool ShowStartingTimeSettings()
        {
            return gameMode != GameMode.Standard;
        }
        public bool ShowSandBoxLevelSeed()
        {
            return gameMode == GameMode.CombatSandbox;
        }       
        public bool ShowRandomizeCharacters()
        {
            return (gameMode == GameMode.CombatSandbox && !enemyVsEnemyMode) || gameMode == GameMode.TownSandbox;
        }
        public bool ShowChosenCharacterTemplates()
        {
            return ((gameMode == GameMode.CombatSandbox && !enemyVsEnemyMode)|| gameMode == GameMode.TownSandbox) && randomizePlayerCharacters == false;
        }
        public bool ShowEnemyVsEnemyMode()
        {
            return gameMode == GameMode.CombatSandbox;
        }
        public bool ShowPlayerAiCharacters()
        {
            return gameMode == GameMode.CombatSandbox && enemyVsEnemyMode == true;
        }
        public bool ShowTotalCharacters()
        {
            return gameMode == GameMode.CombatSandbox && randomizePlayerCharacters == true && enemyVsEnemyMode == false;
        }
        public bool ShowPossibleRandomCharacters()
        {
            return gameMode == GameMode.CombatSandbox && randomizePlayerCharacters == true && enemyVsEnemyMode == false;
        }
        public bool ShowSandboxEnemyEncounter()
        {
            return gameMode == GameMode.CombatSandbox;
        }
        #endregion

    }

    public enum GameMode
    {
        Standard = 0,
        CombatSandbox = 1,
        TownSandbox = 5,
        PostCombatReward = 2,
    }
}