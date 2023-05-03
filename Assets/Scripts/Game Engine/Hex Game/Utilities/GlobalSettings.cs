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
        [SerializeField] private bool includeGameIntroEvent = true;

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
        [SerializeField] private CombatMapSeedDataSO sandboxCombatMapSeed;

        [ShowIf("ShowPlayerCharacterSpawnSetting")]
        [SerializeField] private PlayerCharacterSpawnSetting playerCharacterSpawnSetting;

        [ShowIf("ShowRandomizePlayerCharacters")]
        [SerializeField] private bool randomizePlayerCharacters;

        [ShowIf("ShowTotalRandomCharacters")]
        [SerializeField] private int totalRandomCharacters;

        [ShowIf("ShowPlayerAiCharacters")]
        [SerializeField] private EnemyEncounterSO playerAiCharacters;

        [ShowIf("ShowStartingPlayerCharacters")]
        [SerializeField] private PlayerGroup[] startingPlayerCharacters;         

        [ShowIf("ShowSandboxEnemyEncounter")]
        [SerializeField] private EnemyEncounterSO[] sandboxEnemyEncounters;

       
        #endregion

        // Getters + Accessors
        #region
        public GameMode GameMode
        {
            get { return gameMode; }
        }
        public bool PreventAudioProfiles => preventAudioProfiles;
        public bool IncludeGameIntroEvent => includeGameIntroEvent;
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
        public CombatMapSeedDataSO SandboxCombatMapSeed
        {
            get { return sandboxCombatMapSeed; }
        }      
        public EnemyEncounterSO[] SandboxEnemyEncounters
        {
            get { return sandboxEnemyEncounters; }
        }
        public EnemyEncounterSO PlayerAiCharacters
        {
            get { return playerAiCharacters; }
        }
        public PlayerCharacterSpawnSetting PlayerCharacterSpawnSetting
        {
            get { return playerCharacterSpawnSetting; }
        }
        public bool RandomizePlayerCharacters
        {
            get { return randomizePlayerCharacters; }
        }
        public int TotalRandomCharacters
        {
            get { return totalRandomCharacters; }
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
        public PlayerGroup[] StartingPlayerCharacters
        {
            get { return startingPlayerCharacters; }
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
                CharacterDataController.Instance.HandleGainXP(character, StartingXpBonus, false);
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
        public bool ShowPlayerCharacterSpawnSetting()
        {
            return gameMode == GameMode.CombatSandbox || gameMode == GameMode.TownSandbox;
        }
        public bool ShowStartingPlayerCharacters()
        {
            return (gameMode == GameMode.CombatSandbox || gameMode == GameMode.TownSandbox) && playerCharacterSpawnSetting == PlayerCharacterSpawnSetting.Normal;
        }       
        public bool ShowPlayerAiCharacters()
        {
            return gameMode == GameMode.CombatSandbox && playerCharacterSpawnSetting == PlayerCharacterSpawnSetting.EnemyVsEnemy == true;
        }
        public bool ShowTotalRandomCharacters()
        {
            return (gameMode == GameMode.CombatSandbox || gameMode == GameMode.TownSandbox) && playerCharacterSpawnSetting == PlayerCharacterSpawnSetting.Normal && randomizePlayerCharacters;
        }
        public bool ShowSandboxEnemyEncounter()
        {
            return gameMode == GameMode.CombatSandbox;
        }
        public bool ShowRandomizePlayerCharacters()
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
    public enum PlayerCharacterSpawnSetting
    {
        Normal = 0,
        EnemyVsEnemy = 1,
    }
}