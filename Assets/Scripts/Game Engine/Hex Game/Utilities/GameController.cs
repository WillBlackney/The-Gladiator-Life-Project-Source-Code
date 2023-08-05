using WeAreGladiators.Abilities;
using WeAreGladiators.Audio;
using WeAreGladiators.Boons;
using WeAreGladiators.CameraSystems;
using WeAreGladiators.Characters;
using WeAreGladiators.Combat;
using WeAreGladiators.GameIntroEvent;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Items;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.LoadingScreen;
using WeAreGladiators.MainMenu;
using WeAreGladiators.Persistency;
using WeAreGladiators.RewardSystems;
using WeAreGladiators.StoryEvents;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.TurnLogic;
using WeAreGladiators.UCM;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;
using WeAreGladiators.VisualEvents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using WeAreGladiators.Scoring;
using WeAreGladiators.CombatLog;

namespace WeAreGladiators
{
    public class GameController : Singleton<GameController>
    {
        // Properties + Component References
        #region
        [SerializeField] private GameObject actNotifVisualParent;
        [SerializeField] private CanvasGroup actNotifCg;
        [SerializeField] private CanvasGroup actNotifTextParentCg;
        [SerializeField] private TextMeshProUGUI actNotifCountText;
        [SerializeField] private TextMeshProUGUI actNotifNameText;
        #endregion

        // Game State Logic 
        #region
        private GameState gameState;

        public GameState GameState
        {
            get { return gameState; }
        }
        public void SetGameState(GameState newState)
        {
            gameState = newState;
        }
        #endregion

        // Initialization + Setup
        #region
        protected override void Awake()
        {
            base.Awake();
            VisualEventManager.Initialize();
        }
        private void Start()
        {
            RunApplication();
        }
        void RunApplication()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (GlobalSettings.Instance.GameMode == GameMode.Standard)
                RunStandardGameModeSetup();

            else if (GlobalSettings.Instance.GameMode == GameMode.CombatSandbox)
                RunSandboxCombat();

            else if (GlobalSettings.Instance.GameMode == GameMode.TownSandbox)
                RunSandboxTown();

            else if (GlobalSettings.Instance.GameMode == GameMode.StoryEventSandbox)
                RunSandboxStoryEvent();

            else if (GlobalSettings.Instance.GameMode == GameMode.PostCombatReward)
                RunPostCombatRewardTestEventSetup();
        }
        #endregion

        // Specific Game Mode Setups
        #region
        private void RunStandardGameModeSetup()
        {
            SetGameState(GameState.MainMenu);
            LightController.Instance.EnableStandardGlobalLight();
            TopBarController.Instance.HideMainTopBar();
            TopBarController.Instance.HideCombatTopBar();
            BlackScreenController.Instance.DoInstantFadeOut();
            BlackScreenController.Instance.FadeInScreen(2f);
            MainMenuController.Instance.SetCustomCharacterDataDefaultState();
            MainMenuController.Instance.DoGameStartMainMenuRevealSequence();
        }
        private void RunSandboxStoryEvent()
        {
            // Set up new save file
            BuildNewSaveFileForSandboxEnvironment();

            // Set state
            SetGameState(GameState.StoryEvent);

            // Show UI
            TopBarController.Instance.ShowMainTopBar();

            // Build and prepare all session data
            PersistencyController.Instance.SetUpGameSessionDataFromSaveFile();

            // Apply global settings
            GlobalSettings.Instance.ApplyStartingXPBonus();

            // Reset + Centre camera
            CameraController.Instance.ResetMainCameraPositionAndZoom();

            // Build town views here
            TownController.Instance.ShowTownView();
            CharacterScrollPanelController.Instance.BuildAndShowPanel();

            // Fade in sound + views
            DelayUtils.DelayedCall(2f, () => AudioManager.Instance.FadeInSound(Sound.Music_Town_Theme_1, 1f));
            AudioManager.Instance.FadeInSound(Sound.Ambience_Town_1, 2f);
            BlackScreenController.Instance.FadeInScreen(1f);

            // Determine and start next story event
            StoryEventController.Instance.DetermineAndCacheNextStoryEvent();
            StoryEventController.Instance.StartNextEvent();
        }
        private void RunSandboxTown()
        {
            BuildNewSaveFileForSandboxEnvironment();

            // Set state
            SetGameState(GameState.Town);

            // Show UI
            TopBarController.Instance.ShowMainTopBar();

            // Build and prepare all session data
            PersistencyController.Instance.SetUpGameSessionDataFromSaveFile();

            // Apply global settings
            GlobalSettings.Instance.ApplyStartingXPBonus();

            // testing injures + stress + health
            /*
            HexCharacterData character = CharacterDataController.Instance.AllPlayerCharacters[0];
            CharacterDataController.Instance.SetCharacterHealth(character, character.currentHealth - 20);
            CharacterDataController.Instance.SetCharacterStress(character, 10);
            var injury = PerkController.Instance.GetRandomValidInjury(character.passiveManager, InjurySeverity.Severe, InjuryType.Blunt);
            PerkController.Instance.ModifyPerkOnCharacterData(character.passiveManager, injury.perkTag, 3);*/

            // Reset+ Centre camera
            CameraController.Instance.ResetMainCameraPositionAndZoom();

            // Build town views here
            TownController.Instance.ShowTownView();
            CharacterScrollPanelController.Instance.BuildAndShowPanel();

            DelayUtils.DelayedCall(2f, ()=> AudioManager.Instance.FadeInSound(Sound.Music_Town_Theme_1, 1f));
            AudioManager.Instance.FadeInSound(Sound.Ambience_Town_1, 2f);
            BlackScreenController.Instance.FadeInScreen(1f);

            // Game Intro event
            if (GlobalSettings.Instance.IncludeGameIntroEvent)
            {
                SetGameState(GameState.StoryEvent);
                GameIntroController.Instance.StartEvent();
            }

            InventoryController.Instance.PopulateInventoryWithMockDataItems(30);
        }
        public void RunTestEnvironmentCombat(List<HexCharacterTemplateSO> playerCharacters, EnemyEncounterSO enemyEncounter)
        {
            var charactersWithSpawnPos = BuildNewSaveFileForTestingEnvironment(playerCharacters);

            // Set state
            SetGameState(GameState.CombatActive);

            // Show UI
            TopBarController.Instance.ShowCombatTopBar();
            CombatLogController.Instance.ShowLog();

            // Build mock save file + journey data
            RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatStart);

            // Enable world view
            LightController.Instance.EnableDayTimeGlobalLight();
            LevelController.Instance.EnableDayTimeArenaScenery();
            LevelController.Instance.ShowAllNodeViews();
            LevelController.Instance.SetLevelNodeDayOrNightViewState(true);

            // Generate enemy wave + enemies data
            CombatContractData sandboxContractData = TownController.Instance.GenerateSandboxContractData(enemyEncounter);
            RunController.Instance.SetCurrentContractData(sandboxContractData);

            // Generate combat map data
            List<LevelNode> spawnPositions = LevelController.Instance.GetCharacterStartPositions(
                RunController.Instance.CurrentCombatContractData.enemyEncounterData.enemiesInEncounter,
                charactersWithSpawnPos);

            // Randomize level node elevation and obstructions
            RunController.Instance.SetCurrentCombatMapData(LevelController.Instance.GenerateLevelNodes(spawnPositions));

            // Save data to persistency
            PersistencyController.Instance.AutoUpdateSaveFile();

            // Setup player characters
            HexCharacterController.Instance.CreateAllPlayerCombatCharacters(charactersWithSpawnPos);

            // Animate Crowd
            LevelController.Instance.StopAllCrowdMembers(true);

            // Combat Music
            AudioManager.Instance.AutoPlayBasicCombatMusic(1f);
            AudioManager.Instance.FadeInSound(Sound.Ambience_Crowd_1, 1f);

            // Spawn enemies in world
            HexCharacterController.Instance.SpawnEnemyEncounter(sandboxContractData.enemyEncounterData);

            // Place characters off screen
            HexCharacterController.Instance.MoveAllCharactersToOffScreenPosition();

            // Move characters towards start nodes
            VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.MoveAllCharactersToStartingNodes(null));

        }
        private void RunSandboxCombat()
        {
            var charactersWithSpawnPos = BuildNewSaveFileForSandboxEnvironment();

            // Set state
            SetGameState(GameState.CombatActive); 

            // Show UI
            TopBarController.Instance.ShowCombatTopBar();
            CombatLogController.Instance.ShowLog();

            // Build mock save file + journey data
            RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatStart);

            // Apply global settings
            GlobalSettings.Instance.ApplyStartingXPBonus();

            // Enable world view
            LightController.Instance.EnableDayTimeGlobalLight();
            LevelController.Instance.EnableDayTimeArenaScenery();
            LevelController.Instance.ShowAllNodeViews();
            LevelController.Instance.SetLevelNodeDayOrNightViewState(true);

            // Generate enemy wave + enemies data
            CombatContractData sandboxContractData = TownController.Instance.GenerateSandboxContractData();
            RunController.Instance.SetCurrentContractData(sandboxContractData);

            // Generate combat map data
            List<LevelNode> spawnPositions = LevelController.Instance.GetCharacterStartPositions(
                RunController.Instance.CurrentCombatContractData.enemyEncounterData.enemiesInEncounter,
                charactersWithSpawnPos);

            // Randomize level node elevation and obstructions
            RunController.Instance.SetCurrentCombatMapData(LevelController.Instance.GenerateLevelNodes(spawnPositions));           

            // Save data to persistency
            PersistencyController.Instance.AutoUpdateSaveFile();

            // Setup player characters
            if (GlobalSettings.Instance.PlayerCharacterSpawnSetting == PlayerCharacterSpawnSetting.EnemyVsEnemy)
            {
                HexCharacterController.Instance.SpawnEnemyEncounterAsPlayerTeam
                    (RunController.Instance.GenerateEnemyEncounterFromTemplate(GlobalSettings.Instance.PlayerAiCharacters));
            }
            else
            {
                HexCharacterController.Instance.CreateAllPlayerCombatCharacters(charactersWithSpawnPos);
            }

            // Animate Crowd
            LevelController.Instance.StopAllCrowdMembers(true);

            // Combat Music
            AudioManager.Instance.AutoPlayBasicCombatMusic(1f);
            AudioManager.Instance.FadeInSound(Sound.Ambience_Crowd_1, 1f);
           
            // Spawn enemies in world
            HexCharacterController.Instance.SpawnEnemyEncounter(sandboxContractData.enemyEncounterData);            

            // Place characters off screen
            HexCharacterController.Instance.MoveAllCharactersToOffScreenPosition();

            // Move characters towards start nodes
            TaskTracker cData = new TaskTracker();
            VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.MoveAllCharactersToStartingNodes(cData));

            // Start a new combat event
            TurnController.Instance.OnNewCombatEventStarted();

            // Create mock scoring data
            PlayerScoreTracker scoreData = ScoreController.Instance.CurrentScoreData;
            ScoreController.Instance.GenerateMockScoreData();

            ItemController.Instance.GetAllShopSpawnableItems(Rarity.Epic, ItemType.Weapon).ForEach(i => 
                InventoryController.Instance.AddItemToInventory(i));
        }
        private void RunPostCombatRewardTestEventSetup()
        {
            // Set up new save file
            BuildNewSaveFileForSandboxEnvironment();

            // Build mock save file + journey data
            RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatStart);

            // Set state
            SetGameState(GameState.CombatActive);

            // Show UI
            TopBarController.Instance.ShowCombatTopBar();
            CombatLogController.Instance.ShowLog();

            // Randomize level node elevation and obstructions
            LevelController.Instance.GenerateLevelNodes();

            // Apply global settings
            GlobalSettings.Instance.ApplyStartingXPBonus();

            // Enable world view
            LightController.Instance.EnableDayTimeGlobalLight();
            LevelController.Instance.EnableDayTimeArenaScenery();
            LevelController.Instance.ShowAllNodeViews();
            LevelController.Instance.SetLevelNodeDayOrNightViewState(true);

            // Setup player characters
            HexCharacterController.Instance.CreateAllPlayerCombatCharacters(CharacterDataController.Instance.AllPlayerCharacters);

            // Generate enemy wave + enemies data + save to run controller
            CombatContractData sandboxContractData = TownController.Instance.GenerateSandboxContractData();
            RunController.Instance.SetCurrentContractData(sandboxContractData);

            // Start combat end process
            if(RunController.Instance.CurrentCombatContractData.enemyEncounterData.difficulty == CombatDifficulty.Boss) HandleGameOverBossCombatVictory();
            else StartCombatVictorySequence();
        }      
        #endregion

        // Post Combat Logic
        #region
        public void StartCombatVictorySequence()
        {
            StartCoroutine(StartCombatVictorySequenceCoroutine());
        }
        private IEnumerator StartCombatVictorySequenceCoroutine()
        {
            Debug.Log("GameController.StartCombatVictorySequenceCoroutine() called");

            // Set state
            SetGameState(GameState.CombatRewardPhase);
            EnemyInfoModalController.Instance.HideModal();

            // Determine characters to reward XP to
            List<HexCharacterModel> charactersRewarded = new List<HexCharacterModel>();
            foreach (HexCharacterModel character in HexCharacterController.Instance.AllPlayerCharacters)
            {
                if (character.livingState == LivingState.Alive && character.currentHealth > 0)
                    charactersRewarded.Add(character);
            }
            foreach (HexCharacterModel character in HexCharacterController.Instance.Graveyard)
            {
                if (character.controller == Controller.Player)
                    charactersRewarded.Add(character);
            }

            // Reward XP, build and show combat stats screen
            List<CharacterCombatStatData> combatStats = CombatRewardController.Instance.GenerateCombatStatResultsForCharacters(charactersRewarded, true);
            CombatRewardController.Instance.CacheStatResult(combatStats);
            CombatRewardController.Instance.ApplyXpGainFromStatResultsToCharacters(combatStats);

            // Gain loot
            CombatRewardController.Instance.HandleGainRewardsOfContract(RunController.Instance.CurrentCombatContractData);

            // Determine combat results (for scoring)
            CombatController.Instance.UpdateScoreDataPostCombat();

            // Save game
            RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatEnd);
            PersistencyController.Instance.AutoUpdateSaveFile();

            // Wait until v queue count = 0
            yield return new WaitUntil(() => VisualEventManager.EventQueue.Count == 0);

            AudioManager.Instance.FadeOutAllCombatMusic(0.25f);

            // Tear down summoned characters
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllSummonedPlayerCharacters)
            {
                // Smokey vanish effect
                VisualEffectManager.Instance.CreateExpendEffect(model.hexCharacterView.WorldPosition, 15, 0.2f, false);

                // Fade out character model
                CharacterModeller.FadeOutCharacterModel(model.hexCharacterView.ucm, 0.5f);

                // Fade out UI elements
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null, 0.5f);
            }

            // Disable any player character gui's if they're still active
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllPlayerCharacters)
            {
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
            }           

            // Hide level nodes
            LevelController.Instance.HideAllNodeViews();

            // Destroy Activation windows
            TurnController.Instance.DestroyAllActivationWindows();

            // Hide combat UI + Popup Modals
            CombatLogController.Instance.HideLog();
            CombatUIController.Instance.HideViewsOnTurnEnd();
            LevelController.Instance.HideTileInfoPopup();
            AbilityController.Instance.HideHitChancePopup();
            AbilityPopupController.Instance.HidePanel();
            MoveActionController.Instance.HidePathCostPopup();
            EnemyInfoModalController.Instance.HideModal();

            // Crowd combat end applause SFX and animations
            AudioManager.Instance.PlaySound(Sound.Crowd_Big_Cheer_1);
            LevelController.Instance.AnimateCrowdOnCombatVictory();
            CameraController.Instance.DoCameraZoom(5, 6f, 1f);            

            yield return new WaitForSeconds(2f);
            DelayUtils.DelayedCall(2f, ()=> AudioManager.Instance.FadeOutSound(Sound.Crowd_Big_Cheer_1, 3f));

            // Show combat screen
            CombatRewardController.Instance.BuildAndShowPostCombatScreen(combatStats, RunController.Instance.CurrentCombatContractData, true);

           

        }
        public void StartCombatDefeatSequence()
        {
            StartCoroutine(StartCombatDefeatSequenceCoroutine());
        }
        private IEnumerator StartCombatDefeatSequenceCoroutine()
        {
            Debug.Log("GameController.StartCombatDefeatSequence() called");

            // Set state
            SetGameState(GameState.CombatRewardPhase);
            EnemyInfoModalController.Instance.HideModal();

            // Determine characters to reward XP to   
            List<HexCharacterModel> charactersRewarded = new List<HexCharacterModel>();            
            foreach (HexCharacterModel character in HexCharacterController.Instance.Graveyard)
            {
                if (character.controller == Controller.Player)
                    charactersRewarded.Add(character);
            }

            // Reward XP, build and show combat stats screen
            List<CharacterCombatStatData> combatStats = CombatRewardController.Instance.GenerateCombatStatResultsForCharacters(charactersRewarded, true);
            CombatRewardController.Instance.CacheStatResult(combatStats);
            CombatRewardController.Instance.ApplyXpGainFromStatResultsToCharacters(combatStats);

            // Determine combat results (for scoring)
            CombatController.Instance.UpdateScoreDataPostCombat(false);

            // Save game
            RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatEnd);
            PersistencyController.Instance.AutoUpdateSaveFile();

            // Wait until v queue count = 0
            yield return new WaitUntil(() => VisualEventManager.EventQueue.Count == 0);

            AudioManager.Instance.FadeOutAllCombatMusic(0.25f);

            // Disable any enemy character gui's if they're still active
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllEnemies)
            {
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
            }

            // Hide level nodes
            LevelController.Instance.HideAllNodeViews();

            // Destroy Activation windows
            TurnController.Instance.DestroyAllActivationWindows();

            // Hide combat UI + Popup Modals
            CombatLogController.Instance.HideLog();
            CombatUIController.Instance.HideViewsOnTurnEnd();
            LevelController.Instance.HideTileInfoPopup();
            AbilityController.Instance.HideHitChancePopup();
            AbilityPopupController.Instance.HidePanel();
            MoveActionController.Instance.HidePathCostPopup();
            EnemyInfoModalController.Instance.HideModal();

            // Crowd combat end applause SFX and animations
            AudioManager.Instance.PlaySound(Sound.Crowd_Big_Cheer_1); // to do: should be a boo'ing sound
            LevelController.Instance.AnimateCrowdOnCombatVictory(); // to do: defeat animations
            CameraController.Instance.DoCameraZoom(5, 6f, 1f);

            yield return new WaitForSeconds(2f);
            DelayUtils.DelayedCall(2f, () => AudioManager.Instance.FadeOutSound(Sound.Crowd_Big_Cheer_1, 3f));

            // Show combat screen
            CombatRewardController.Instance.BuildAndShowPostCombatScreen(combatStats, RunController.Instance.CurrentCombatContractData, false);

        }
        public void HandleGameOverBossCombatDefeat()
        {
            StartCoroutine(HandleGameOverBossCombatDefeatCoroutine());
        }
        private IEnumerator HandleGameOverBossCombatDefeatCoroutine()
        {
            Debug.Log("GameController.StartCombatDefeatSequence() called");

            // Set state
            SetGameState(GameState.CombatRewardPhase);
            EnemyInfoModalController.Instance.HideModal();            

            // Wait until v queue count = 0
            yield return new WaitUntil(() => VisualEventManager.EventQueue.Count == 0);

            // Calculate score and delete save file
            CombatController.Instance.UpdateScoreDataPostCombat(false);
            PlayerScoreTracker scoreSet = ScoreController.Instance.CurrentScoreData;
            PersistencyController.Instance.DeleteSaveFileOnDisk();

            AudioManager.Instance.FadeOutAllCombatMusic(0.25f);

            // Disable any enemy character gui's if they're still active
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllEnemies)            
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);            

            // Hide level nodes
            LevelController.Instance.HideAllNodeViews();

            // Destroy Activation windows
            TurnController.Instance.DestroyAllActivationWindows();

            // Hide combat UI + Popup Modals
            TopBarController.Instance.HideCombatTopBar();
            CombatLogController.Instance.HideLog();
            CombatUIController.Instance.HideViewsOnTurnEnd();
            LevelController.Instance.HideTileInfoPopup();
            AbilityController.Instance.HideHitChancePopup();
            AbilityPopupController.Instance.HidePanel();
            MoveActionController.Instance.HidePathCostPopup();
            EnemyInfoModalController.Instance.HideModal();

            // Crowd combat end applause SFX and animations
            AudioManager.Instance.PlaySound(Sound.Crowd_Big_Cheer_1); // to do: should be a boo'ing sound
            LevelController.Instance.AnimateCrowdOnCombatVictory(); // to do: defeat animations
            CameraController.Instance.DoCameraZoom(5, 6f, 1f);

            yield return new WaitForSeconds(2f);
            DelayUtils.DelayedCall(2f, () => AudioManager.Instance.FadeOutSound(Sound.Crowd_Big_Cheer_1, 3f));

            // Show defeat + score screen
            ScoreController.Instance.CalculateAndShowScoreScreen(scoreSet, true);

        }
        public void HandleGameOverBossCombatVictory()
        {
            StartCoroutine(HandleGameOverBossCombatVictoryCoroutine());
        }
        private IEnumerator HandleGameOverBossCombatVictoryCoroutine()
        {
            Debug.Log("GameController.StartCombatVictorySequenceCoroutine() called");

            // Set state
            SetGameState(GameState.CombatRewardPhase);
            EnemyInfoModalController.Instance.HideModal();

            // Determine characters to reward XP to
            List<HexCharacterModel> charactersRewarded = new List<HexCharacterModel>();
            foreach (HexCharacterModel character in HexCharacterController.Instance.AllPlayerCharacters)
            {
                if (character.livingState == LivingState.Alive && character.currentHealth > 0)
                    charactersRewarded.Add(character);
            }
            foreach (HexCharacterModel character in HexCharacterController.Instance.Graveyard)
            {
                if (character.controller == Controller.Player)
                    charactersRewarded.Add(character);
            }

            // Reward XP, build and show combat stats screen
            List<CharacterCombatStatData> combatStats = CombatRewardController.Instance.GenerateCombatStatResultsForCharacters(charactersRewarded, true);
            CombatRewardController.Instance.CacheStatResult(combatStats);
            CombatRewardController.Instance.ApplyXpGainFromStatResultsToCharacters(combatStats);

            // Gain loot
            CombatRewardController.Instance.HandleGainRewardsOfContract(RunController.Instance.CurrentCombatContractData);            

            // Wait until v queue count = 0
            yield return new WaitUntil(() => VisualEventManager.EventQueue.Count == 0);

            // Calculate score and delete save file
            CombatController.Instance.UpdateScoreDataPostCombat();
            PlayerScoreTracker scoreSet = ScoreController.Instance.CurrentScoreData;
            PersistencyController.Instance.DeleteSaveFileOnDisk();

            AudioManager.Instance.FadeOutAllCombatMusic(0.25f);

            // Tear down summoned characters
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllSummonedPlayerCharacters)
            {
                // Smokey vanish effect
                VisualEffectManager.Instance.CreateExpendEffect(model.hexCharacterView.WorldPosition, 15, 0.2f, false);

                // Fade out character model
                CharacterModeller.FadeOutCharacterModel(model.hexCharacterView.ucm, 0.5f);

                // Fade out UI elements
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null, 0.5f);
            }

            // Disable any player character gui's if they're still active
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllPlayerCharacters)
            {
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
            }

            // Hide level nodes
            LevelController.Instance.HideAllNodeViews();

            // Destroy Activation windows
            TurnController.Instance.DestroyAllActivationWindows();

            // Hide combat UI + Popup Modals
            CombatLogController.Instance.HideLog();
            TopBarController.Instance.HideCombatTopBar();
            CombatUIController.Instance.HideViewsOnTurnEnd();
            LevelController.Instance.HideTileInfoPopup();
            AbilityController.Instance.HideHitChancePopup();
            AbilityPopupController.Instance.HidePanel();
            MoveActionController.Instance.HidePathCostPopup();
            EnemyInfoModalController.Instance.HideModal();

            // Crowd combat end applause SFX and animations
            AudioManager.Instance.PlaySound(Sound.Crowd_Big_Cheer_1);
            LevelController.Instance.AnimateCrowdOnCombatVictory();
            CameraController.Instance.DoCameraZoom(5, 6f, 1f);

            yield return new WaitForSeconds(2f);
            DelayUtils.DelayedCall(2f, () => AudioManager.Instance.FadeOutSound(Sound.Crowd_Big_Cheer_1, 3f));

            // Show defeat + score screen
            ScoreController.Instance.CalculateAndShowScoreScreen(scoreSet);

        }
        public void HandlePostCombatToTownTransistion()
        {
            AudioManager.Instance.FadeOutAllAmbience(1f);
            LoadingScreenController.Instance.ShowLoadingScreen(1.5f, 1f, null, () =>
            {
                // Prepare town + new day start data + save game
                RunController.Instance.OnNewDayStart();
                SetGameState(GameState.Town);
                RunController.Instance.SetCheckPoint(SaveCheckPoint.Town);

                // Determine and start next story event
                StoryEventDataSO nextEvent = StoryEventController.Instance.DetermineAndCacheNextStoryEvent();
                if (nextEvent != null && RunController.Instance.CurrentDay != 1)
                {
                    SetGameState(GameState.StoryEvent);
                    RunController.Instance.SetCheckPoint(SaveCheckPoint.StoryEvent);
                }

                // Save changes
                PersistencyController.Instance.AutoUpdateSaveFile();

                // Tear down combat views
                LevelController.Instance.StopAllCrowdMembers();
                HexCharacterController.Instance.HandleTearDownCombatScene();
                LevelController.Instance.HandleTearDownAllCombatViews();
                LightController.Instance.EnableStandardGlobalLight();
                CombatRewardController.Instance.HidePostCombatRewardScreen();

                // Show town UI
                TownController.Instance.ShowTownView();
                CharacterScrollPanelController.Instance.BuildAndShowPanel();
                TopBarController.Instance.ShowMainTopBar();

                // Reset + Centre camera
                CameraController.Instance.ResetMainCameraPositionAndZoom();

                // Fade back in views + sound
                AudioManager.Instance.FadeInSound(Sound.Ambience_Town_1, 2f);
                DelayUtils.DelayedCall(1f, () =>
                {
                    AudioManager.Instance.FadeInSound(Sound.Music_Town_Theme_1, 1f);

                    // Show next story event
                    if (nextEvent != null && RunController.Instance.CurrentDay != 1)
                        StoryEventController.Instance.StartNextEvent();
                });
            });
        }
        #endregion

        // Main menu to game transisitions
        #region
        public void HandleStartNewGameFromMainMenuEvent()
        {
            AudioManager.Instance.PlaySound(Sound.Events_New_Game_Started);
            AudioManager.Instance.StopMainMenuMusic(2f);

            LoadingScreenController.Instance.ShowLoadingScreen(1.5f, 1f,
            () =>
            {
                // Set state
                SetGameState(GameState.StoryEvent);

                // Enable GUI
                TopBarController.Instance.ShowMainTopBar();

                // Set up new save file 
                PersistencyController.Instance.BuildNewSaveFileOnNewGameStarted();

                // Build and prepare all session data
                PersistencyController.Instance.SetUpGameSessionDataFromSaveFile();

                // Reset+ Centre camera
                CameraController.Instance.ResetMainCameraPositionAndZoom();

                // Hide Main Menu
                MainMenuController.Instance.HideChooseCharacterScreen();

                // Build town views 
                TownController.Instance.ShowTownView();
                CharacterScrollPanelController.Instance.BuildAndShowPanel();
            },
            () =>
            {
                // Start music, fade in
                AudioManager.Instance.FadeInSound(Sound.Ambience_Town_1, 1f);
                AudioManager.Instance.FadeInSound(Sound.Music_Town_Theme_1, 1f);
                GameIntroController.Instance.StartEvent();

            });
        }
        public void HandleQuitToMainMenuFromInGame()
        {
            Debug.Log("HandleQuitToMainMenuFromInGameCoroutine");

            // Hide menus + GUI + misc annoying stuff
            MainMenuController.Instance.HideInGameMenuView();
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);

            // Fade out battle music + ambience
            AudioManager.Instance.FadeOutAllCombatMusic(1f);
            AudioManager.Instance.FadeOutSound(Sound.Music_Town_Theme_1, 1f);
            AudioManager.Instance.FadeOutAllAmbience(1f);
            VisualEvent handle = VisualEventManager.HandleEventQueueTearDown();
            Func<bool> awaitCondition = null;
            if (handle != null && handle.cData != null) awaitCondition = () => handle.cData.Complete() == true;

            LoadingScreenController.Instance.ShowLoadingScreen(1.5f, 1f, () =>
            {
                // Save game if in town
                if (GameState == GameState.Town)
                    PersistencyController.Instance.AutoUpdateSaveFile();

                // Set menu state
                SetGameState(GameState.MainMenu);

                // Pause run timer
                RunController.Instance.PauseTimer();

                // Hide top bars
                TopBarController.Instance.HideMainTopBar();
                TopBarController.Instance.HideCombatTopBar();

                // Brute force stop all game music
                AudioManager.Instance.ForceStopAllCombatMusic();

                // Destroy combat + town scenes
                LevelController.Instance.StopAllCrowdMembers();
                HexCharacterController.Instance.HandleTearDownCombatScene();
                TurnController.Instance.DestroyAllActivationWindows();

                // Hide combat UI
                CombatLogController.Instance.HideLog();
                CombatUIController.Instance.HideViewsOnTurnEnd();
                LevelController.Instance.HandleTearDownAllCombatViews();
                LightController.Instance.EnableStandardGlobalLight();
                LevelController.Instance.DisableArenaView();
                ScoreController.Instance.HideScoreScreen(0f);

                // Hide UI + town views
                TownController.Instance.TearDownOnExitToMainMenu();
                CharacterScrollPanelController.Instance.HideMainView();
                InventoryController.Instance.HideInventoryView();
                CharacterRosterViewController.Instance.HideCharacterRosterScreen();
                GameIntroController.Instance.HideAllViews();
                CombatRewardController.Instance.HidePostCombatRewardScreen();
                BoonController.Instance.HideBoonIconsPanel();
            },
            () =>
            {
                // Fade in menu music
                AudioManager.Instance.PlayMainMenuMusic();

                // Show menu screen
                MainMenuController.Instance.ShowFrontScreen();
                MainMenuController.Instance.RenderMenuButtons();
            },
            awaitCondition);
        }
        public void HandleLoadSavedGameFromMainMenuEvent()
        {
            // Fade menu music
            AudioManager.Instance.StopMainMenuMusic();

            LoadingScreenController.Instance.ShowLoadingScreen(1.5f, 1f, null, () =>
            {
                // Build and prepare all session data
                PersistencyController.Instance.SetUpGameSessionDataFromSaveFile();

                // Reset Camera
                CameraController.Instance.ResetMainCameraPositionAndZoom();

                // Hide Main Menu
                MainMenuController.Instance.HideFrontScreen();

                // Build town views 
                if (RunController.Instance.SaveCheckPoint == SaveCheckPoint.Town)
                {
                    // Set state
                    SetGameState(GameState.Town);

                    // Enable GUI
                    TopBarController.Instance.ShowMainTopBar();
                    TownController.Instance.ShowTownView();
                    CharacterScrollPanelController.Instance.BuildAndShowPanel();

                    // Assign hospital slots
                    TownController.Instance.HandleAssignCharactersToHospitalSlotsOnGameLoad();

                    // Start music, fade in
                    DelayUtils.DelayedCall(1f, () => AudioManager.Instance.FadeInSound(Sound.Music_Town_Theme_1, 1f));
                    AudioManager.Instance.FadeInSound(Sound.Ambience_Town_1, 1f);
                }
                else if (RunController.Instance.SaveCheckPoint == SaveCheckPoint.GameIntroEvent)
                {
                    // Set state
                    SetGameState(GameState.StoryEvent);

                    // Enable GUI
                    TopBarController.Instance.ShowMainTopBar();
                    TownController.Instance.ShowTownView();
                    CharacterScrollPanelController.Instance.BuildAndShowPanel();

                    // Start music, fade in
                    AudioManager.Instance.FadeInSound(Sound.Ambience_Town_1, 1f);
                    AudioManager.Instance.FadeInSound(Sound.Music_Town_Theme_1, 1f);
                    GameIntroController.Instance.StartEvent();
                }
                else if (RunController.Instance.SaveCheckPoint == SaveCheckPoint.StoryEvent)
                {
                    // Set state
                    SetGameState(GameState.StoryEvent);

                    // Enable GUI
                    TopBarController.Instance.ShowMainTopBar();
                    TownController.Instance.ShowTownView();
                    CharacterScrollPanelController.Instance.BuildAndShowPanel();

                    // Start music, fade in
                    AudioManager.Instance.FadeInSound(Sound.Ambience_Town_1, 1f);
                    DelayUtils.DelayedCall(1f, () =>
                    {
                        AudioManager.Instance.FadeInSound(Sound.Music_Town_Theme_1, 1f);
                        StoryEventController.Instance.StartNextEvent();
                    });

                }
                else if (RunController.Instance.SaveCheckPoint == SaveCheckPoint.CombatStart)
                {
                    TopBarController.Instance.ShowCombatTopBar();
                    CombatLogController.Instance.ShowLog();
                    SetGameState(GameState.CombatActive);
                    LevelController.Instance.GenerateLevelNodes(RunController.Instance.CurrentCombatMapData);

                    // Set up combat level views + lighting
                    LightController.Instance.EnableDayTimeGlobalLight();
                    LevelController.Instance.EnableDayTimeArenaScenery();
                    LevelController.Instance.ShowAllNodeViews();
                    LevelController.Instance.SetLevelNodeDayOrNightViewState(true);

                    // Animate Crowd
                    LevelController.Instance.StopAllCrowdMembers(true);

                    // Combat Music
                    AudioManager.Instance.AutoPlayBasicCombatMusic(1f);
                    AudioManager.Instance.FadeInSound(Sound.Ambience_Crowd_1, 1f);

                    // Setup player characters
                    HexCharacterController.Instance.CreateAllPlayerCombatCharacters(RunController.Instance.CurrentDeployedCharacters);

                    // Setup enemy characters
                    HexCharacterController.Instance.SpawnEnemyEncounter(RunController.Instance.CurrentCombatContractData.enemyEncounterData);

                    // Place characters off screen
                    HexCharacterController.Instance.MoveAllCharactersToOffScreenPosition();

                    // Move characters towards start nodes
                    TaskTracker cData = new TaskTracker();
                    VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.MoveAllCharactersToStartingNodes(cData));

                    // Start a new combat event
                    TurnController.Instance.OnNewCombatEventStarted();
                }
                else if (RunController.Instance.SaveCheckPoint == SaveCheckPoint.CombatEnd)
                {
                    SetGameState(GameState.CombatRewardPhase);
                    TopBarController.Instance.ShowCombatTopBar();
                    CombatLogController.Instance.HideLog();

                    // Set up combat level views + lighting
                    LightController.Instance.EnableDayTimeGlobalLight();
                    LevelController.Instance.EnableDayTimeArenaScenery();
                    LevelController.Instance.ShowAllNodeViews();
                    LevelController.Instance.SetLevelNodeDayOrNightViewState(true);

                    // Show combat screen
                    CombatRewardController.Instance.BuildAndShowPostCombatScreen(CombatRewardController.Instance.CurrentStatResults, RunController.Instance.CurrentCombatContractData, true);
                }
            });
        }       
        public void OnGameOverScreenMainMenuButtonClicked()
        {
            HandleQuitToMainMenuFromInGame();
        }
        #endregion

        // Load Encounters Logic (General)
        #region
        public void HandleLoadIntoCombatFromDeploymentScreen()
        {
            AudioManager.Instance.PlaySound(Sound.Effects_End_Deployment);
            AudioManager.Instance.FadeOutAllAmbience(1f);
            AudioManager.Instance.FadeOutSound(Sound.Music_Town_Theme_1, 1f);
            LoadingScreenController.Instance.ShowLoadingScreen(1.5f, 1f, null, () =>
            {

                // Animate Crowd
                LevelController.Instance.StopAllCrowdMembers(true);

                // Combat Music
                AudioManager.Instance.AutoPlayBasicCombatMusic(1f);
                AudioManager.Instance.FadeInSound(Sound.Ambience_Crowd_1, 1f);

                // Hide town views
                BlackScreenController.Instance.FadeInScreen(1f);
                TownController.Instance.HideTownView();
                TownController.Instance.HideDeploymentPage();
                CharacterScrollPanelController.Instance.HideMainView();
                TopBarController.Instance.ShowCombatTopBar();
                CombatLogController.Instance.ShowLog();
                BoonController.Instance.HideBoonIconsPanel();
                SetGameState(GameState.CombatActive);

                // Setup combat data for persistency
                RunController.Instance.SetCurrentContractData(CombatContractCard.SelectectedCombatCard.MyContractData);
                RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatStart);
                RunController.Instance.SetPlayerDeployedCharacters(TownController.Instance.GetDeployedCharacters());

                // Generate combat map data
                List<LevelNode> spawnPositions = LevelController.Instance.GetCharacterStartPositions(
                    RunController.Instance.CurrentCombatContractData.enemyEncounterData.enemiesInEncounter,
                    RunController.Instance.CurrentDeployedCharacters);

                // Generate combat map 
                SerializedCombatMapData combatMapData = LevelController.Instance.GenerateLevelNodes(spawnPositions);               
                RunController.Instance.SetCurrentCombatMapData(combatMapData);

                // Save game data
                PersistencyController.Instance.AutoUpdateSaveFile();

                // Setup level + lighting
                LightController.Instance.EnableDayTimeGlobalLight();
                LevelController.Instance.EnableDayTimeArenaScenery();
                LevelController.Instance.ShowAllNodeViews();
                LevelController.Instance.SetLevelNodeDayOrNightViewState(true);

                // Setup player characters      
                HexCharacterController.Instance.CreateAllPlayerCombatCharacters(RunController.Instance.CurrentDeployedCharacters);

                // Setup enemy characters
                HexCharacterController.Instance.SpawnEnemyEncounter(RunController.Instance.CurrentCombatContractData.enemyEncounterData);

                // Place characters off screen
                HexCharacterController.Instance.MoveAllCharactersToOffScreenPosition();

                // Move characters towards start nodes
                TaskTracker cData = new TaskTracker();
                VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.MoveAllCharactersToStartingNodes(cData));

                // Start a new combat event
                TurnController.Instance.OnNewCombatEventStarted();
            });
        }       
        #endregion              

        // Misc Logic
        #region
        private List<CharacterWithSpawnData> CreateSandboxCharacterDataFiles()
        {
            List<CharacterWithSpawnData> characters = new List<CharacterWithSpawnData>();

            // Random player characters
            if (GlobalSettings.Instance.RandomizePlayerCharacters)
            {
                List<PlayerGroup> possibleRandoms = new List<PlayerGroup>();
                possibleRandoms = GlobalSettings.Instance.StartingPlayerCharacters.ToList();
                possibleRandoms.Shuffle();

                for(int i = 0; i < GlobalSettings.Instance.TotalRandomCharacters && i < possibleRandoms.Count; i++)
                {
                    characters.Add(possibleRandoms[i].GenerateCharacterWithSpawnData());
                }
            }
            else
            {
                foreach(PlayerGroup pg in GlobalSettings.Instance.StartingPlayerCharacters)
                {
                    characters.Add(pg.GenerateCharacterWithSpawnData());
                }
            }

            return characters;
        }      
        private List<CharacterWithSpawnData> BuildNewSaveFileForSandboxEnvironment()
        {
            List<CharacterWithSpawnData> charactersWithSpawnPos = CreateSandboxCharacterDataFiles();
            PersistencyController.Instance.BuildNewSaveFileOnNewGameStarted(charactersWithSpawnPos[0].characterData);
            List<HexCharacterData> characters = new List<HexCharacterData>();
            charactersWithSpawnPos.ForEach(x => characters.Add(x.characterData));
            for (int i = 1; i < characters.Count; i++) CharacterDataController.Instance.AddCharacterToRoster(characters[i]);
            PersistencyController.Instance.AutoUpdateSaveFile();
            return charactersWithSpawnPos;
        }
        private List<CharacterWithSpawnData> BuildNewSaveFileForTestingEnvironment(List<HexCharacterTemplateSO> playerCharacters)
        {
            List<CharacterWithSpawnData> charactersWithSpawnPos = new List<CharacterWithSpawnData>();

            int ySpawn = 1;
            int xSpawn = -2;

            playerCharacters.ForEach(i => 
            {
                var character = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(i);
                charactersWithSpawnPos.Add(new CharacterWithSpawnData(character, new Vector2(xSpawn, ySpawn)));
                
                // Deployed 3 characters? start deploying one rank back.
                if(ySpawn == -1)
                {
                    ySpawn = 1;
                    xSpawn = -3;
                }
                else ySpawn -= 1;
            });

            PersistencyController.Instance.BuildNewSaveFileOnNewGameStarted(charactersWithSpawnPos[0].characterData);
            List<HexCharacterData> characters = new List<HexCharacterData>();
            charactersWithSpawnPos.ForEach(x => characters.Add(x.characterData));
            for (int i = 1; i < characters.Count; i++) CharacterDataController.Instance.AddCharacterToRoster(characters[i]);
            PersistencyController.Instance.AutoUpdateSaveFile();
            return charactersWithSpawnPos;
        }

        #endregion

    }

    public enum GameState
    {
        MainMenu = 0,
        CombatActive = 1,
        CombatRewardPhase = 2,
        StoryEvent = 3,
        Town = 4
    }
}
