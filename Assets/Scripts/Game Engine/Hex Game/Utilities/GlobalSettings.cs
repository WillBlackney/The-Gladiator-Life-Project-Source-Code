using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Characters;
using WeAreGladiators.StoryEvents;

namespace WeAreGladiators.Utilities
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
        [SerializeField] private bool showAlphaWarning = true;
        [SerializeField] private bool includeGameIntroEvent = true;

        [Title("Character Settings")]
        [SerializeField] private int startingXpBonus;
        [Space(10)]

        [Title("Combat Settings")]
        [SerializeField] private int baseHitChance;
        [Space(10)]

        [Title("Resource Settings")]
        [SerializeField] private int baseStartingGold;
        [Space(10)]

        [Title("Starting Timeline Settings")]
        [Range(1, 5)]
        [ShowIf("ShowStartingTimeSettings")]
        [SerializeField] private int startingDay = 1;
        [Range(1, 5)]
        [ShowIf("ShowStartingTimeSettings")]
        [SerializeField] private int startingChapter = 1;
        [Space(10)]

        // Single Combat Scene Properties
        [Title("Sandbox Settings")]
        [ShowIf("ShowSandBoxLevelSeed")]
        [SerializeField] private CombatMapSeedDataSO sandboxCombatMapSeed;

        [ShowIf("ShowSandBoxStoryEvent")]
        [SerializeField] private StoryEventDataSO sandboxStoryEvent;

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
        public bool ShowAlphaWarning
        {
            get { return showAlphaWarning && GameMode == GameMode.Standard; }
        }
        public int StartingDay
        {
            get { return startingDay; }
        }
        public int StartingXpBonus
        {
            get { return startingXpBonus; }
        }
        public StoryEventDataSO SandboxStoryEvent
        {
            get { return sandboxStoryEvent; }
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
        public int BaseStartingGold
        {
            get { return baseStartingGold; }
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
            else Debug.unityLogger.logEnabled = false;
        }
        public void ApplyStartingXPBonus()
        {
            foreach (HexCharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
            {
                CharacterDataController.Instance.HandleGainXP(character, StartingXpBonus, false);
            }
        }
        public void SetTestEnvironment()
        {
            gameMode = GameMode.IntegrationTesting;
            Debug.unityLogger.logEnabled = false;
        }
        #endregion

        // Odin Showifs
        #region         
        public bool ShowSandBoxStoryEvent()
        {
            return gameMode == GameMode.StoryEventSandbox;
        }
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
            return gameMode != GameMode.Standard;
        }
        public bool ShowStartingPlayerCharacters()
        {
            return gameMode != GameMode.Standard && playerCharacterSpawnSetting == PlayerCharacterSpawnSetting.Normal;
        }       
        public bool ShowPlayerAiCharacters()
        {
            return gameMode != GameMode.Standard && playerCharacterSpawnSetting == PlayerCharacterSpawnSetting.EnemyVsEnemy == true;
        }
        public bool ShowTotalRandomCharacters()
        {
            return gameMode != GameMode.Standard && playerCharacterSpawnSetting == PlayerCharacterSpawnSetting.Normal && randomizePlayerCharacters;
        }
        public bool ShowSandboxEnemyEncounter()
        {
            return gameMode == GameMode.CombatSandbox;
        }
        public bool ShowRandomizePlayerCharacters()
        {
            return gameMode != GameMode.Standard;
        }
        #endregion

    }

    public enum GameMode
    {
        Standard = 0,
        CombatSandbox = 1,
        TownSandbox = 5,
        PostCombatReward = 2,
        StoryEventSandbox = 4,
        IntegrationTesting = 6,
    }
    public enum PlayerCharacterSpawnSetting
    {
        Normal = 0,
        EnemyVsEnemy = 1,
    }
}