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

        [Title("Combat Settings")]
        [SerializeField] private int baseHitChance;
        [SerializeField] private int startingDeploymentLimit;

        [Title("Resource Settings")]
        [SerializeField] private int baseStartingGold;
        [SerializeField] private int campActivityPointRegen;

        // Single Combat Scene Properties
        [Title("Combat Sandbox Settings")]      
        [ShowIf("ShowSandBoxLevelSeed")]
        [SerializeField] private HexMapSeedDataSO sandboxLevelSeed;       

        [ShowIf("ShowRandomizeCharacters")]
        [SerializeField] private bool randomizePlayerCharacters = false;

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
        public HexMapSeedDataSO SandboxLevelSeed
        {
            get { return sandboxLevelSeed; }
        }      
        public EnemyEncounterSO[] SandboxEnemyEncounters
        {
            get { return sandboxEnemyEncounters; }
        }
        public bool RandomizePlayerCharacters
        {
            get { return randomizePlayerCharacters; }
        }
        public int TotalCharacters
        {
            get { return totalCharacters; }
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
        #endregion

        

        // Odin Showifs
        #region
        public bool ShowSandBoxLevelSeed()
        {
            return gameMode == GameMode.CombatSandbox;
        }       
        public bool ShowRandomizeCharacters()
        {
            return gameMode == GameMode.CombatSandbox;
        }
        public bool ShowChosenCharacterTemplates()
        {
            return gameMode == GameMode.CombatSandbox && randomizePlayerCharacters == false;
        }
        public bool ShowTotalCharacters()
        {
            return gameMode == GameMode.CombatSandbox && randomizePlayerCharacters == true;
        }
        public bool ShowPossibleRandomCharacters()
        {
            return gameMode == GameMode.CombatSandbox && randomizePlayerCharacters == true;
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