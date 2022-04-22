using DG.Tweening;
using HexGameEngine.Audio;
using HexGameEngine.CameraSystems;
using HexGameEngine.Camping;
using HexGameEngine.Characters;
using HexGameEngine.DraftEvent;
using HexGameEngine.DungeonMap;
using HexGameEngine.HexTiles;
using HexGameEngine.Items;
using HexGameEngine.JourneyLogic;
using HexGameEngine.MainMenu;
using HexGameEngine.Persistency;
using HexGameEngine.Player;
using HexGameEngine.RewardSystems;
using HexGameEngine.TownFeatures;
using HexGameEngine.TurnLogic;
using HexGameEngine.UCM;
using HexGameEngine.UI;
using HexGameEngine.Utilities;
using HexGameEngine.VisualEvents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HexGameEngine
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
        private void Start()
        {
            RunApplication();
        }
        void RunApplication()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            if (GlobalSettings.Instance.GameMode == GameMode.Standard)
                StartCoroutine(RunStandardGameModeSetup());

            else if (GlobalSettings.Instance.GameMode == GameMode.CombatSandbox)
                RunSandboxCombat();

            else if (GlobalSettings.Instance.GameMode == GameMode.TownSandbox)
                RunSandboxTown();

            else if (GlobalSettings.Instance.GameMode == GameMode.PostCombatReward)
                RunPostCombatRewardTestEventSetup();
        }
        #endregion

        // Specific Game Mode Setups
        #region
        private IEnumerator RunStandardGameModeSetup()
        {
            gameState = GameState.MainMenu;
            LightController.Instance.EnableStandardGlobalLight();
            TopBarController.Instance.HideTopBar();
            BlackScreenController.Instance.DoInstantFadeOut();
            BlackScreenController.Instance.FadeInScreen(2f);
            MainMenuController.Instance.SetChooseCharacterBoxStartingStates();
            MainMenuController.Instance.DoGameStartMainMenuRevealSequence();

            yield return null;
        }
        private void RunSandboxTown()
        {
            // Set state
            SetGameState(GameState.NonCombatEvent);

            // Show UI
            TopBarController.Instance.ShowTopBar();

            // Set up new save file 
            PersistencyController.Instance.BuildNewSaveFileOnNewGameStarted(CreateSandboxCharacterDataFiles()[0]);

            // Build and prepare all session data
            PersistencyController.Instance.SetUpGameSessionDataFromSaveFile();

            // Reset+ Centre camera
            CameraController.Instance.ResetMainCameraPositionAndZoom();

            // Build town views here
            TownController.Instance.ShowTownView();
            CharacterScrollPanelController.Instance.BuildAndShowPanel();

            AudioManager.Instance.FadeInSound(Sound.Ambience_Outdoor_Spooky, 1f);
            BlackScreenController.Instance.FadeInScreen(1f);

            InventoryController.Instance.PopulateInventoryWithMockDataItems(20);
        }
        private void RunSandboxCombat()
        {
            // Set state
            SetGameState(GameState.CombatActive); 

            // Show UI
            TopBarController.Instance.ShowTopBar();

            // Build mock save file + journey data
            RunController.Instance.SetGameStartValues();
            RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatStart);
            // set combat with run controller

            // Build character roster + mock data
            List<HexCharacterData> characters = CreateSandboxCharacterDataFiles();
            CharacterDataController.Instance.BuildCharacterRoster(characters);

            // Enable world view
            LightController.Instance.EnableDungeonGlobalLight();
            LevelController.Instance.EnableNightTimeArenaScenery();
            LevelController.Instance.ShowAllNodeViews();
            

            // Setup player characters
            HexCharacterController.Instance.CreateAllPlayerCombatCharacters(CharacterDataController.Instance.AllPlayerCharacters);

            // Generate enemy wave + enemies data + save to run controller
            CombatContractData sandboxContractData = TownController.Instance.GenerateSandboxContractData();
            RunController.Instance.SetCurrentContractData(sandboxContractData);

            // Spawn enemies in world
            HexCharacterController.Instance.SpawnEnemyEncounter(sandboxContractData.enemyEncounterData);

            // Randomize level node elevation and obstructions
            LevelController.Instance.GenerateLevelNodes();

            // Place characters off screen
            HexCharacterController.Instance.MoveAllCharactersToOffScreenPosition();

            // Move characters towards start nodes
            CoroutineData cData = new CoroutineData();
            VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.MoveAllCharactersToStartingNodes(cData));

            // Start a new combat event
            TurnController.Instance.OnNewCombatEventStarted();
        }
        private void RunPostCombatRewardTestEventSetup()
        {
            // Set state
            SetGameState(GameState.CombatActive);

            // Show UI
            TopBarController.Instance.ShowTopBar();

            // Build mock save file + journey data
            RunController.Instance.SetGameStartValues();
            RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatStart);

            // Build character roster + mock data
            List<HexCharacterData> characters = CreateSandboxCharacterDataFiles();
            CharacterDataController.Instance.BuildCharacterRoster(characters);

            // Enable world view
            LightController.Instance.EnableDungeonGlobalLight();
            LevelController.Instance.EnableNightTimeArenaScenery();
            LevelController.Instance.ShowAllNodeViews();

            // Setup player characters
            HexCharacterController.Instance.CreateAllPlayerCombatCharacters(CharacterDataController.Instance.AllPlayerCharacters);

            // Randomize level node elevation and obstructions
            LevelController.Instance.GenerateLevelNodes();

            // Generate enemy wave + enemies data + save to run controller
            CombatContractData sandboxContractData = TownController.Instance.GenerateSandboxContractData();
            RunController.Instance.SetCurrentContractData(sandboxContractData);

            // Start a new combat event
            StartCombatVictorySequence();
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
            // Set state
            SetGameState(GameState.CombatRewardPhase);

            // wait until v queue count = 0
            yield return new WaitUntil(() => VisualEventManager.Instance.EventQueue.Count == 0);

            // Zoom camera to reset settings
            CameraController.Instance.DoPostCombatZoomAndMove(1f);

            // Tear down summoned characters
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllSummonedDefenders)
            {
                // Smokey vanish effect
                VisualEffectManager.Instance.CreateExpendEffect(model.hexCharacterView.WorldPosition, 15, 0.2f, false);

                // Fade out character model
                CharacterModeller.FadeOutCharacterModel(model.hexCharacterView.ucm, 1f);

                // Fade out UI elements
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
                HexCharacterController.Instance.FadeOutCharacterUICanvas(model.hexCharacterView, null);
                if (model.hexCharacterView.uiCanvasParent.activeSelf == true)
                {
                    HexCharacterController.Instance.FadeOutCharacterUICanvas(model.hexCharacterView, null);
                }
            }

            // Disable any player character gui's if they're still active
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllDefenders)
            {
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
                HexCharacterController.Instance.FadeOutCharacterUICanvas(model.hexCharacterView, null);
                if (model.hexCharacterView != null && model.hexCharacterView.uiCanvasParent.activeSelf == true)
                {
                    HexCharacterController.Instance.FadeOutCharacterUICanvas(model.hexCharacterView, null);
                }
            }

            // Hide level nodes
            LevelController.Instance.HideAllNodeViews();

            // Destroy Activation windows
            TurnController.Instance.DestroyAllActivationWindows();

            // Hide end turn button
            TurnController.Instance.DisableEndTurnButtonView();
            TurnController.Instance.DisableDelayTurnButtonView();

            // Determine characters to reward XP to
            List<HexCharacterModel> charactersRewarded = new List<HexCharacterModel>();
            foreach (HexCharacterModel character in HexCharacterController.Instance.AllDefenders)
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
            CombatRewardController.Instance.BuildAndShowPostCombatScreen(combatStats, RunController.Instance.CurrentCombatContractData, true);

            // Gain loot
            CombatRewardController.Instance.HandleGainRewardsOfContract(RunController.Instance.CurrentCombatContractData);

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

            // wait until v queue count = 0
            yield return new WaitUntil(() => VisualEventManager.Instance.EventQueue.Count == 0);

            // Zoom camera to reset settings
            CameraController.Instance.DoPostCombatZoomAndMove(1f);

            // Tear down summoned characters + enemies
            List<HexCharacterModel> destCharacters = new List<HexCharacterModel>();
            destCharacters.AddRange(HexCharacterController.Instance.AllSummonedDefenders);
            destCharacters.AddRange(HexCharacterController.Instance.AllEnemies);
            foreach (HexCharacterModel model in destCharacters)
            {
                // Smokey vanish effect
                VisualEffectManager.Instance.CreateExpendEffect(model.hexCharacterView.WorldPosition, 15, 0.2f, false);

                // Fade out character model
                CharacterModeller.FadeOutCharacterModel(model.hexCharacterView.ucm, 1f);

                // Fade out UI elements
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
                HexCharacterController.Instance.FadeOutCharacterUICanvas(model.hexCharacterView, null);
                if (model.hexCharacterView.uiCanvasParent.activeSelf == true)
                {
                    HexCharacterController.Instance.FadeOutCharacterUICanvas(model.hexCharacterView, null);
                }
            }

            // Disable any player character gui's if they're still active
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllDefenders)
            {
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
                HexCharacterController.Instance.FadeOutCharacterUICanvas(model.hexCharacterView, null);
                if (model.hexCharacterView != null && model.hexCharacterView.uiCanvasParent.activeSelf == true)
                {
                    HexCharacterController.Instance.FadeOutCharacterUICanvas(model.hexCharacterView, null);
                }
            }

            // Hide level nodes
            LevelController.Instance.HideAllNodeViews();

            // Destroy Activation windows
            TurnController.Instance.DestroyAllActivationWindows();

            // Hide end turn button
            TurnController.Instance.DisableEndTurnButtonView();
            TurnController.Instance.DisableDelayTurnButtonView();

            // Determine characters to reward XP to
            List<HexCharacterModel> charactersRewarded = new List<HexCharacterModel>();
            foreach (HexCharacterModel character in HexCharacterController.Instance.AllDefenders)
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
            List<CharacterCombatStatData> combatStats = CombatRewardController.Instance.GenerateCombatStatResultsForCharacters(charactersRewarded, false);
            CombatRewardController.Instance.CacheStatResult(combatStats);
            CombatRewardController.Instance.BuildAndShowPostCombatScreen(combatStats, RunController.Instance.CurrentCombatContractData, false);
        }
        public void StartGameOverSequenceFromCombat()
        {
            StartCoroutine(StartGameOverSequenceFromCombatCoroutine());
        }
        private IEnumerator StartGameOverSequenceFromCombatCoroutine()
        {
            // Delete save file
            PersistencyController.Instance.DeleteSaveFileOnDisk();

            // Set state
            SetGameState(GameState.CombatRewardPhase);

            // wait until v queue count = 0
            yield return new WaitUntil(() => VisualEventManager.Instance.EventQueue.Count == 0);

            // Zoom camera to reset settings
            CameraController.Instance.DoPostCombatZoomAndMove(1f);

            // Tear down summoned characters + enemies
            List<HexCharacterModel> destCharacters = new List<HexCharacterModel>();
            destCharacters.AddRange(HexCharacterController.Instance.AllSummonedDefenders);
            destCharacters.AddRange(HexCharacterController.Instance.AllEnemies);
            foreach (HexCharacterModel model in destCharacters)
            {
                // Smokey vanish effect
                VisualEffectManager.Instance.CreateExpendEffect(model.hexCharacterView.WorldPosition, 15, 0.2f, false);

                // Fade out character model
                CharacterModeller.FadeOutCharacterModel(model.hexCharacterView.ucm, 1f);

                // Fade out UI elements
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
                HexCharacterController.Instance.FadeOutCharacterUICanvas(model.hexCharacterView, null);
                if (model.hexCharacterView.uiCanvasParent.activeSelf == true)
                {
                    HexCharacterController.Instance.FadeOutCharacterUICanvas(model.hexCharacterView, null);
                }
            }

            // Disable any player character gui's if they're still active
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllDefenders)
            {
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
                HexCharacterController.Instance.FadeOutCharacterUICanvas(model.hexCharacterView, null);
                if (model.hexCharacterView != null && model.hexCharacterView.uiCanvasParent.activeSelf == true)
                {
                    HexCharacterController.Instance.FadeOutCharacterUICanvas(model.hexCharacterView, null);
                }
            }

            // Hide level nodes
            LevelController.Instance.HideAllNodeViews();

            // Destroy Activation windows
            TurnController.Instance.DestroyAllActivationWindows();

            // Hide end turn button
            TurnController.Instance.DisableEndTurnButtonView();
            TurnController.Instance.DisableDelayTurnButtonView();

            // Show Game over screen
            CombatRewardController.Instance.BuildAndShowGameOverScreen();

        }

        public void HandlePostCombatToTownTransistion()
        {
            StartCoroutine(HandlePostCombatToTownTransistionCoroutine());
        }
        private IEnumerator HandlePostCombatToTownTransistionCoroutine()
        {
            // Fade out
            BlackScreenController.Instance.FadeOutScreen(1f);
            yield return new WaitForSeconds(1f);

            // Tear down combat views
            HexCharacterController.Instance.HandleTearDownCombatScene();
            LevelController.Instance.HandleTearDownCombatViews();
            LightController.Instance.EnableStandardGlobalLight();
            CombatRewardController.Instance.HidePostCombatRewardScreen();

            // town new day start stuff
            RunController.Instance.OnNewDayStart();

            // Show town UI
            TownController.Instance.ShowTownView();
            CharacterScrollPanelController.Instance.BuildAndShowPanel();
            TopBarController.Instance.ShowTopBar();

            // Reset+ Centre camera
            CameraController.Instance.ResetMainCameraPositionAndZoom();

            // Save game
            PersistencyController.Instance.AutoUpdateSaveFile();

            // Fade back in views + sound
            yield return new WaitForSeconds(0.5f);
            AudioManager.Instance.FadeInSound(Sound.Ambience_Outdoor_Spooky, 1f);
            BlackScreenController.Instance.FadeInScreen(1f);
        }
        #endregion

        // Main menu to game transisitions
        #region
        public void HandleStartNewGameFromMainMenuEvent()
        {
            StartCoroutine(HandleStartNewGameFromMainMenuEventCoroutine());
        }
        public IEnumerator HandleStartNewGameFromMainMenuEventCoroutine()
        {
            // Fade out screen + audio
            AudioManager.Instance.PlaySound(Sound.Events_New_Game_Started);
            AudioManager.Instance.FadeOutSound(Sound.Music_Main_Menu_Theme_1, 2f);
            BlackScreenController.Instance.FadeOutScreen(2f);
            yield return new WaitForSeconds(2f);

            // Enable GUI
            TopBarController.Instance.ShowTopBar();

            // Set up new save file 
            PersistencyController.Instance.BuildNewSaveFileOnNewGameStarted();

            // Build and prepare all session data
            PersistencyController.Instance.SetUpGameSessionDataFromSaveFile();

            // Reset+ Centre camera
            CameraController.Instance.ResetMainCameraPositionAndZoom();

            // Hide Main Menu
            MainMenuController.Instance.HideChooseCharacterScreen();

            // Act start visual sequence
            /*
            yield return new WaitForSeconds(0.5f);
            AudioManager.Instance.FadeInSound(Sound.Ambience_Outdoor_Spooky, 1f);
            yield return new WaitForSeconds(1f);
            PlayActNotificationVisualEvent();
            BlackScreenController.Instance.FadeInScreen(0f);
            yield return new WaitForSeconds(3.5f);
            */

            // TO DO: Build town views here
            TownController.Instance.ShowTownView();
            CharacterScrollPanelController.Instance.BuildAndShowPanel();

            yield return new WaitForSeconds(0.5f);
            AudioManager.Instance.FadeInSound(Sound.Ambience_Outdoor_Spooky, 1f);
            BlackScreenController.Instance.FadeInScreen(2f);



            // Start the first encounter set up sequence
            //HandleLoadEncounter(RunController.Instance.CurrentEncounterType);
        }       
        public void HandleQuitToMainMenuFromInGame()
        {
            StartCoroutine(HandleQuitToMainMenuFromInGameCoroutine());
        }
        private IEnumerator HandleQuitToMainMenuFromInGameCoroutine()
        {
            Debug.LogWarning("HandleQuitToMainMenuFromInGameCoroutine");

            // Hide menus + GUI + misc annoying stuff
            MainMenuController.Instance.HideInGameMenuView();
            AudioManager.Instance.StopSound(Sound.Character_Footsteps);

            // Fade out battle music + ambience
            AudioManager.Instance.FadeOutAllCombatMusic(2f);
            AudioManager.Instance.FadeOutAllAmbience(2f);

            // Do black screen fade out
            BlackScreenController.Instance.FadeOutScreen(2f);

            // Wait for the current visual event to finish playing
            VisualEvent handle = VisualEventManager.Instance.HandleEventQueueTearDown();

            // Wait till its safe to tearn down event queue and scene
            yield return new WaitForSeconds(2f);

            // Hide Game GUI
            TopBarController.Instance.HideTopBar();

            // Brute force stop all game music
            AudioManager.Instance.ForceStopAllCombatMusic();

            if (handle != null && handle.cData != null)
            {
                yield return new WaitUntil(() => handle.cData.CoroutineCompleted() == true);
            }

            // Destroy game scene
            HexCharacterController.Instance.HandleTearDownCombatScene();
            LevelController.Instance.HandleTearDownCombatViews();
            LightController.Instance.EnableStandardGlobalLight();

            // Hide world map + roster
            //MapView.Instance.HideMainMapView();
            CharacterRosterViewController.Instance.HideCharacterRosterScreen();

            // Hide Draft event elements
            //DraftEventController.Instance.HideMainViewParent();
            LevelController.Instance.DisableArenaView();

            // Hide post combat views
            CombatRewardController.Instance.HideGameOverScreen();
            CombatRewardController.Instance.HidePostCombatRewardScreen();

            // Fade in menu music
            AudioManager.Instance.FadeInSound(Sound.Music_Main_Menu_Theme_1, 1f);

            // Show menu screen
            MainMenuController.Instance.ShowFrontScreen();
            MainMenuController.Instance.RenderMenuButtons();

            // Do black screen fade in
            BlackScreenController.Instance.FadeInScreen(2f);
        }
        public void HandleLoadSavedGameFromMainMenuEvent()
        {
            StartCoroutine(HandleLoadSavedGameFromMainMenuEventCoroutine());
        }
        private IEnumerator HandleLoadSavedGameFromMainMenuEventCoroutine()
        {
            // Fade menu music
            if (AudioManager.Instance.IsSoundPlaying(Sound.Music_Main_Menu_Theme_1))
            {
                AudioManager.Instance.FadeOutSound(Sound.Music_Main_Menu_Theme_1, 2f);
            }

            // Fade out menu scren
            BlackScreenController.Instance.FadeOutScreen(2f);
            yield return new WaitForSeconds(2f);

            // Build and prepare all session data
            PersistencyController.Instance.SetUpGameSessionDataFromSaveFile();

            // Enable GUI
            TopBarController.Instance.ShowTopBar();

            // Reset Camera
            CameraController.Instance.ResetMainCameraPositionAndZoom();

            // Hide Main Menu
            MainMenuController.Instance.HideFrontScreen();

            // Load the encounter the player saved at
            //HandleLoadEncounter(RunController.Instance.CurrentEncounterType);
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
            StartCoroutine(HandleLoadIntoCombatFromDeploymentScreenCoroutine());

        }
        private IEnumerator HandleLoadIntoCombatFromDeploymentScreenCoroutine()
        {
            BlackScreenController.Instance.FadeOutScreen(1f);          
            yield return new WaitForSeconds(1f);

            // Hide down town views
            BlackScreenController.Instance.FadeInScreen(1f);
            TownController.Instance.HideTownView();
            TownController.Instance.HideDeploymentPage();
            CharacterScrollPanelController.Instance.HideMainView();
            // in future: hide top bar

            SetGameState(GameState.CombatActive);
            RunController.Instance.SetCurrentContractData(CombatContractCard.SelectectedCombatCard.MyContractData);
            RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatStart);
            RunController.Instance.SetPlayerDeployedCharacters(TownController.Instance.GetDeployedCharacters());
            PersistencyController.Instance.AutoUpdateSaveFile();

            // Enable world view
            LightController.Instance.EnableDungeonGlobalLight();
            LevelController.Instance.EnableNightTimeArenaScenery();
            LevelController.Instance.ShowAllNodeViews();

            // Setup player characters
            List<HexCharacterData> spawnedPlayerCharacters = new List<HexCharacterData>();
            foreach(CharacterWithSpawnData c in RunController.Instance.CurrentDeployedCharacters)            
                spawnedPlayerCharacters.Add(c.characterData);            
            HexCharacterController.Instance.CreateAllPlayerCombatCharacters(spawnedPlayerCharacters);

            // Setup enemy characters
            HexCharacterController.Instance.SpawnEnemyEncounter(RunController.Instance.CurrentCombatContractData.enemyEncounterData);

            // Randomize level node elevation and obstructions
            LevelController.Instance.GenerateLevelNodes();

            // Place characters off screen
            HexCharacterController.Instance.MoveAllCharactersToOffScreenPosition();

            // Move characters towards start nodes
            CoroutineData cData = new CoroutineData();
            VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.MoveAllCharactersToStartingNodes(cData));

            // Start a new combat event
            TurnController.Instance.OnNewCombatEventStarted();
        }
        #endregion

        // Character Draft Event
        #region
        private void HandleLoadCharacterDraftEvent()
        {
            StartCoroutine(HandleLoadCharacterDraftEventCoroutine());
        }
        private IEnumerator HandleLoadCharacterDraftEventCoroutine()
        {
            // Disable dungeon view, if open
            //LevelController.Instance.DisableDungeonScenery();

            // Reset event 
            DraftEventController.Instance.ResetDraftEventViewsAndData();

            // Set lighting
            LightController.Instance.EnableOutdoorCryptGlobalLight();

            // Play ambience if not already playing
            if (!AudioManager.Instance.IsSoundPlaying(Sound.Ambience_Outdoor_Spooky))            
                AudioManager.Instance.FadeInSound(Sound.Ambience_Outdoor_Spooky, 1f); 

            // Build starting views           
            DraftEventController.Instance.BuildInitialViews(CharacterDataController.Instance.AllPlayerCharacters[0]);

            // Screen Reveal
            BlackScreenController.Instance.FadeInScreen(1.5f);

            // Set Camera start settings
            CameraController.Instance.DoCameraZoom(2, 2, 0f);
            CameraController.Instance.DoCameraMove(-5, -3, 0f);

            // Start moving player model + animate king
            DraftEventController.Instance.PlayKingFloatAnim();
            DraftEventController.Instance.DoPlayerModelMoveToMeetingSequence();
            yield return new WaitForSeconds(0.5f);

            // Move + Zoom out camera
            CameraController.Instance.DoCameraZoom(2, 5, 2f);
            CameraController.Instance.DoCameraMove(0, 0, 2f);
            yield return new WaitForSeconds(1.5f);

            // King greeting
            DraftEventController.Instance.DoKingGreeting();
            yield return new WaitForSeconds(1.5f);

            // Start draft choices + UI here
            DraftEventController.Instance.StartBuildAndShowDraftCharacterScreenInitialSequence();

            // Fade in choice buttons + health bar
            // KingsBlessingController.Instance.FadeInChoiceButtons();
            // KingsBlessingController.Instance.FadeHealthBar(1, 1);
        }
        public void HandlDraftEventContinueSequence(CoroutineData cData)
        {
            StartCoroutine(HandlDraftEventContinueSequenceCoroutine(cData));
        }
        private IEnumerator HandlDraftEventContinueSequenceCoroutine(CoroutineData cData)
        {
            // Disable continue button
            DraftEventController.Instance.HideContinueButton();

            // King greeting
            DraftEventController.Instance.DoKingFarewell();

            // Open door visual sequence
            DraftEventController.Instance.DoDoorOpeningSequence();
            yield return new WaitForSeconds(1.75f);

            // start camera zoom + movement here
            CameraController.Instance.DoCameraZoom(5, 2, 3.25f);
            CameraController.Instance.DoCameraMove(0, 2, 3.25f);

            // Player moves towards door visual sequence
            StartCoroutine(DraftEventController.Instance.DoPlayerCharactersMoveThroughEntranceSequence());
            yield return new WaitForSeconds(2.25f);

            // Fade out outdoor ambience
            AudioManager.Instance.FadeOutSound(Sound.Ambience_Outdoor_Spooky, 1f);

            // Black screen fade out start here
            BlackScreenController.Instance.FadeOutScreen(1f);
            yield return new WaitForSeconds(1.1f);

            DraftEventController.Instance.HideMainViewParent();

            if (cData != null)
            {
                cData.MarkAsCompleted();
            }
        }
        #endregion

        // Camping Events
        #region
        public void HandleLoadCampSiteEvent()
        {
            StartCoroutine(HandleLoadCampSiteEventCoroutine());
        }
        private IEnumerator HandleLoadCampSiteEventCoroutine()
        {
            // Screen Reveal
            BlackScreenController.Instance.FadeInScreen(1.5f);

            CampSiteController.Instance.BuildAllViewsAndPropertiesOnNewEventStart();
            yield return null;

        }
        #endregion




        // Misc Logic
        #region
        private List<HexCharacterData> CreateSandboxCharacterDataFiles()
        {
            List<HexCharacterData> characters = new List<HexCharacterData>();

            if (GlobalSettings.Instance.RandomizePlayerCharacters)
            {
                List<HexCharacterTemplateSO> randomCharacters = GenerateRandomSandboxCharacters(GlobalSettings.Instance.TotalCharacters);
                foreach (HexCharacterTemplateSO c in randomCharacters)
                {
                    HexCharacterData character = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(c);
                    CharacterDataController.Instance.HandleGainXP(character, 300);
                    /*
                    for(int i = 0; i < 2; i++)
                    {
                        character.attributeRolls.Add(AttributeRollResult.GenerateRoll(character)); // generate roll result for testing
                        character.perkRolls.Add(PerkRollResult.GenerateRoll(character));
                    }
                    */
                    characters.Add(character);
                }
            }
            else
            {
                foreach (HexCharacterTemplateSO dataSO in GlobalSettings.Instance.ChosenCharacterTemplates)
                {
                    HexCharacterData character = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(dataSO);
                    CharacterDataController.Instance.HandleGainXP(character, 300);
                    /*for (int i = 0; i < 2; i++)
                    {
                        character.attributeRolls.Add(AttributeRollResult.GenerateRoll(character)); // generate roll result for testing
                        character.perkRolls.Add(PerkRollResult.GenerateRoll(character));
                    }
                    */
                    characters.Add(character);
                }
            }


            return characters;
        }
        public List<HexCharacterTemplateSO> GenerateRandomSandboxCharacters(int characters)
        {
            List<HexCharacterTemplateSO> listReturned = new List<HexCharacterTemplateSO>();
            List<HexCharacterTemplateSO> shuffleList = new List<HexCharacterTemplateSO>();
            shuffleList.AddRange(GlobalSettings.Instance.PossibleRandomCharacters);
            shuffleList.Shuffle();

            for (int i = 0; i < characters; i++)
            {
                listReturned.Add(shuffleList[i]);
            }
            return listReturned;

        }
        #endregion     

        // Act Notification Logic
        #region
        public void PlayActNotificationVisualEvent()
        {
            StartCoroutine(PlayActNotificationVisualEventCoroutine());
        }
        private IEnumerator PlayActNotificationVisualEventCoroutine()
        {
            // Enable ambience if not playing
            if (!AudioManager.Instance.IsSoundPlaying(Sound.Ambience_Outdoor_Spooky))
            {
                AudioManager.Instance.FadeInSound(Sound.Ambience_Outdoor_Spooky, 1f);
            }

            // Set up
            ResetActNotificationViews();
            actNotifCountText.text = "Act One";
            actNotifNameText.text = "Into The Crypt...";
            actNotifVisualParent.SetActive(true);
            actNotifCg.alpha = 1;
            actNotifTextParentCg.alpha = 1;

            // Fade in act count text
            actNotifCountText.gameObject.SetActive(true);
            actNotifCountText.DOFade(1, 1);

            // Wait for player to read
            yield return new WaitForSeconds(1.5f);

            // Fade in act NAME text
            actNotifNameText.gameObject.SetActive(true);
            actNotifNameText.DOFade(1, 1);

            // Wait for player to read
            yield return new WaitForSeconds(1.5f);

            // Fade out text quickly
            actNotifTextParentCg.DOFade(0, 0.5f);
            yield return new WaitForSeconds(0.5f);

            // Fade out entire view
            actNotifCg.DOFade(0, 1f);
            yield return new WaitForSeconds(1);
            ResetActNotificationViews();
        }
        private void ResetActNotificationViews()
        {
            actNotifCountText.DOFade(0, 0);
            actNotifNameText.DOFade(0, 0);
            actNotifNameText.gameObject.SetActive(false);
            actNotifCountText.gameObject.SetActive(false);
            actNotifVisualParent.SetActive(false);

        }
        #endregion
    }

    public enum GameState
    {
        MainMenu = 0,
        CombatActive = 1,
        CombatRewardPhase = 2,
        NonCombatEvent = 3,
    }
}
