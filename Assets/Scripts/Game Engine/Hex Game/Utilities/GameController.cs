using DG.Tweening;
using HexGameEngine.Abilities;
using HexGameEngine.Audio;
using HexGameEngine.CameraSystems;
using HexGameEngine.Camping;
using HexGameEngine.Characters;
using HexGameEngine.Combat;
using HexGameEngine.GameIntroEvent;
using HexGameEngine.HexTiles;
using HexGameEngine.Items;
using HexGameEngine.JourneyLogic;
using HexGameEngine.MainMenu;
using HexGameEngine.Persistency;
using HexGameEngine.Reputations;
using HexGameEngine.RewardSystems;
using HexGameEngine.TownFeatures;
using HexGameEngine.TurnLogic;
using HexGameEngine.UCM;
using HexGameEngine.UI;
using HexGameEngine.Utilities;
using HexGameEngine.VisualEvents;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            SetGameState(GameState.MainMenu);
            LightController.Instance.EnableStandardGlobalLight();
            TopBarController.Instance.HideMainTopBar();
            TopBarController.Instance.HideCombatTopBar();
            BlackScreenController.Instance.DoInstantFadeOut();
            BlackScreenController.Instance.FadeInScreen(2f);
            MainMenuController.Instance.SetCustomCharacterDataDefaultState();
            MainMenuController.Instance.DoGameStartMainMenuRevealSequence();

            yield return null;
        }
        private void RunSandboxTown()
        {
            // Set state
            SetGameState(GameState.Town);

            // Show UI
            TopBarController.Instance.ShowMainTopBar();

            // Set up new save file 
            PersistencyController.Instance.BuildNewSaveFileOnNewGameStarted(CreateSandboxCharacterDataFiles()[0].characterData);

            // Build and prepare all session data
            PersistencyController.Instance.SetUpGameSessionDataFromSaveFile();

            // Apply global settings
            GlobalSettings.Instance.ApplyStartingXPBonus();

            // Reset+ Centre camera
            CameraController.Instance.ResetMainCameraPositionAndZoom();

            // Build town views here
            TownController.Instance.ShowTownView();
            CharacterScrollPanelController.Instance.BuildAndShowPanel();

            AudioManager.Instance.FadeInSound(Sound.Ambience_Outdoor_Spooky, 1f);
            BlackScreenController.Instance.FadeInScreen(1f);

            // Game Intro event
            if (GlobalSettings.Instance.IncludeGameIntroEvent)
            {
                SetGameState(GameState.StoryEvent);
                GameIntroController.Instance.StartEvent();
            }

            InventoryController.Instance.PopulateInventoryWithMockDataItems(30);
        }
        private void RunSandboxCombat()
        {            
            // Set state
            SetGameState(GameState.CombatActive); 

            // Show UI
            TopBarController.Instance.ShowCombatTopBar();

            // Build mock save file + journey data
            RunController.Instance.SetGameStartValues();
            RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatStart);           

            // Build character roster + mock data
            List<CharacterWithSpawnData> charactersWithSpawnPos = CreateSandboxCharacterDataFiles();
            List<HexCharacterData> characters = new List<HexCharacterData>();
            charactersWithSpawnPos.ForEach(x => characters.Add(x.characterData));
            CharacterDataController.Instance.BuildCharacterRoster(characters);

            // Apply global settings
            GlobalSettings.Instance.ApplyStartingXPBonus();

            // Enable world view
            LightController.Instance.EnableDayTimeGlobalLight();
            LevelController.Instance.EnableDayTimeArenaScenery();
            LevelController.Instance.ShowAllNodeViews();
            LevelController.Instance.SetLevelNodeDayOrNightViewState(true);

            // Randomize level node elevation and obstructions
            RunController.Instance.SetCurrentCombatMapData(LevelController.Instance.GenerateLevelNodes());

            // Generate enemy wave + enemies data
            CombatContractData sandboxContractData = TownController.Instance.GenerateSandboxContractData();
            RunController.Instance.SetCurrentContractData(sandboxContractData);

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

            // Combat Music
            AudioManager.Instance.AutoPlayBasicCombatMusic(1f);
            AudioManager.Instance.FadeInSound(Sound.Ambience_Crowd_1, 1f);
           
            // Spawn enemies in world
            HexCharacterController.Instance.SpawnEnemyEncounter(sandboxContractData.enemyEncounterData);            

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
            TopBarController.Instance.ShowCombatTopBar();

            // Build mock save file + journey data
            RunController.Instance.SetGameStartValues();
            RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatStart);

            // Randomize level node elevation and obstructions
            LevelController.Instance.GenerateLevelNodes();

            // Build character roster + mock data
            List<CharacterWithSpawnData> charactersWithSpawnPos = CreateSandboxCharacterDataFiles();
            List<HexCharacterData> characters = new List<HexCharacterData>();
            charactersWithSpawnPos.ForEach(x => characters.Add(x.characterData));

            CharacterDataController.Instance.BuildCharacterRoster(characters);

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
            Debug.Log("GameController.StartCombatVictorySequenceCoroutine() called");

            // Set state
            SetGameState(GameState.CombatRewardPhase);

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

            // Gain loot
            CombatRewardController.Instance.HandleGainRewardsOfContract(RunController.Instance.CurrentCombatContractData);

            // Save game
            RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatEnd);
            PersistencyController.Instance.AutoUpdateSaveFile();

            // Wait until v queue count = 0
            yield return new WaitUntil(() => VisualEventManager.Instance.EventQueue.Count == 0);

            // Tear down summoned characters
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllSummonedDefenders)
            {
                // Smokey vanish effect
                VisualEffectManager.Instance.CreateExpendEffect(model.hexCharacterView.WorldPosition, 15, 0.2f, false);

                // Fade out character model
                CharacterModeller.FadeOutCharacterModel(model.hexCharacterView.ucm, 1f);

                // Fade out UI elements
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
            }

            // Disable any player character gui's if they're still active
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllDefenders)
            {
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
            }

            // Hide level nodes
            LevelController.Instance.HideAllNodeViews();

            // Destroy Activation windows
            TurnController.Instance.DestroyAllActivationWindows();

            // Hide combat UI + Popup Modals
            CombatUIController.Instance.HideViewsOnTurnEnd();
            LevelController.Instance.HideTileInfoPopup();
            AbilityController.Instance.HideHitChancePopup();
            AbilityPopupController.Instance.HidePanel();
            MoveActionController.Instance.HidePathCostPopup();
            EnemyInfoModalController.Instance.HideModal();

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

            // wait until v queue count = 0
            yield return new WaitUntil(() => VisualEventManager.Instance.EventQueue.Count == 0);

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
            }

            // Disable any player character gui's if they're still active
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllDefenders)
            {
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
            }

            // Hide level nodes
            LevelController.Instance.HideAllNodeViews();

            // Destroy Activation windows
            TurnController.Instance.DestroyAllActivationWindows();

            // Hide combat UI
            CombatUIController.Instance.HideViewsOnTurnEnd();

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
            }

            // Disable any player character gui's if they're still active
            foreach (HexCharacterModel model in HexCharacterController.Instance.AllDefenders)
            {
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(model.hexCharacterView, null);
            }

            // Hide level nodes
            LevelController.Instance.HideAllNodeViews();

            // Destroy Activation windows
            TurnController.Instance.DestroyAllActivationWindows();

            // Hide combat UI
            CombatUIController.Instance.HideViewsOnTurnEnd();

            // Show Game over screen
            //CombatRewardController.Instance.BuildAndShowGameOverScreen();

        }

        public void HandlePostCombatToTownTransistion()
        {
            StartCoroutine(HandlePostCombatToTownTransistionCoroutine());
        }
        private IEnumerator HandlePostCombatToTownTransistionCoroutine()
        {
            // Prepare town + new day start data + save game
            RunController.Instance.OnNewDayStart();
            RunController.Instance.SetCheckPoint(SaveCheckPoint.Town);
            PersistencyController.Instance.AutoUpdateSaveFile();

            // Fade out
            BlackScreenController.Instance.FadeOutScreen(1f);
            yield return new WaitForSeconds(1f);                        

            // Tear down combat views
            HexCharacterController.Instance.HandleTearDownCombatScene();
            LevelController.Instance.HandleTearDownAllCombatViews();
            LightController.Instance.EnableStandardGlobalLight();
            CombatRewardController.Instance.HidePostCombatRewardScreen();            

            // Show town UI
            TownController.Instance.ShowTownView();
            CharacterScrollPanelController.Instance.BuildAndShowPanel();
            TopBarController.Instance.ShowMainTopBar();

            // Reset+ Centre camera
            CameraController.Instance.ResetMainCameraPositionAndZoom();
                        
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

            // Set state
            SetGameState(GlobalSettings.Instance.IncludeGameIntroEvent ? GameState.StoryEvent : GameState.Town);

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

            // Start music, fade in
            yield return new WaitForSeconds(0.5f);
            //AudioManager.Instance.FadeInSound(Sound.Ambience_Outdoor_Spooky, 1f);

            BlackScreenController.Instance.FadeInScreen(2f);
            DOVirtual.DelayedCall(1.5f, () =>
            {
                if (GlobalSettings.Instance.IncludeGameIntroEvent)
                    GameIntroController.Instance.StartEvent();
            });  
                
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

            // Save game if in town
            if(GameState == GameState.Town)            
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

            if (handle != null && handle.cData != null)
            {
                yield return new WaitUntil(() => handle.cData.CoroutineCompleted() == true);
            }

            // Destroy combat + town scenes
            HexCharacterController.Instance.HandleTearDownCombatScene();
            TurnController.Instance.DestroyAllActivationWindows();

            // Hide combat UI
            CombatUIController.Instance.HideViewsOnTurnEnd();            
            LevelController.Instance.HandleTearDownAllCombatViews();
            LightController.Instance.EnableStandardGlobalLight();
            LevelController.Instance.DisableArenaView();

            // Hide UI + town views
            TownController.Instance.TearDownOnExitToMainMenu();
            CharacterScrollPanelController.Instance.HideMainView();
            InventoryController.Instance.HideInventoryView();
            CharacterRosterViewController.Instance.HideCharacterRosterScreen();
            GameIntroController.Instance.HideAllViews();
            //CombatRewardController.Instance.HideGameOverScreen();
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
                AudioManager.Instance.FadeOutSound(Sound.Music_Main_Menu_Theme_1, 1f);
            }

            // Fade out menu scren
            BlackScreenController.Instance.FadeOutScreen(1f);
            yield return new WaitForSeconds(1f);

            // Build and prepare all session data
            PersistencyController.Instance.SetUpGameSessionDataFromSaveFile();         

            // Reset Camera
            CameraController.Instance.ResetMainCameraPositionAndZoom();

            // Hide Main Menu
            MainMenuController.Instance.HideFrontScreen();

            // Build town views 
            if(RunController.Instance.SaveCheckPoint == SaveCheckPoint.Town)
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
                AudioManager.Instance.FadeInSound(Sound.Ambience_Outdoor_Spooky, 1f);
                BlackScreenController.Instance.FadeInScreen(2f);
            }
            if (RunController.Instance.SaveCheckPoint == SaveCheckPoint.GameIntroEvent)
            {
                // Set state
                SetGameState(GameState.StoryEvent);

                // Enable GUI
                TopBarController.Instance.ShowMainTopBar();
                TownController.Instance.ShowTownView();
                CharacterScrollPanelController.Instance.BuildAndShowPanel();

                // Start music, fade in
                AudioManager.Instance.FadeInSound(Sound.Ambience_Outdoor_Spooky, 1f);
                BlackScreenController.Instance.FadeInScreen(2f, ()=> GameIntroController.Instance.StartEvent());
            }
            else if (RunController.Instance.SaveCheckPoint == SaveCheckPoint.CombatStart)
            {
                TopBarController.Instance.ShowCombatTopBar();
                BlackScreenController.Instance.FadeInScreen(1f);
                SetGameState(GameState.CombatActive);
                LevelController.Instance.GenerateLevelNodes(RunController.Instance.CurrentCombatMapData);

                // Set up combat level views + lighting
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
                CoroutineData cData = new CoroutineData();
                VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.MoveAllCharactersToStartingNodes(cData));

                // Start a new combat event
                TurnController.Instance.OnNewCombatEventStarted();
            }
            else if (RunController.Instance.SaveCheckPoint == SaveCheckPoint.CombatEnd)
            {
                SetGameState(GameState.CombatRewardPhase);
                TopBarController.Instance.ShowCombatTopBar();
                BlackScreenController.Instance.FadeInScreen(1f);

                // Set up combat level views + lighting
                LightController.Instance.EnableDayTimeGlobalLight();
                LevelController.Instance.EnableDayTimeArenaScenery();
                LevelController.Instance.ShowAllNodeViews();
                LevelController.Instance.SetLevelNodeDayOrNightViewState(true);

                // Show combat screen
                CombatRewardController.Instance.BuildAndShowPostCombatScreen(CombatRewardController.Instance.CurrentStatResults, RunController.Instance.CurrentCombatContractData, true);
            }
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
            TopBarController.Instance.ShowCombatTopBar();
            SetGameState(GameState.CombatActive);          

            // Generate combat map data
            SerializedCombatMapData combatMapData =  LevelController.Instance.GenerateLevelNodes();

            // Setup combat data for persistency
            RunController.Instance.SetCurrentContractData(CombatContractCard.SelectectedCombatCard.MyContractData);
            RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatStart);
            RunController.Instance.SetPlayerDeployedCharacters(TownController.Instance.GetDeployedCharacters());
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
            CoroutineData cData = new CoroutineData();
            VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.MoveAllCharactersToStartingNodes(cData));

            // Start a new combat event
            TurnController.Instance.OnNewCombatEventStarted();
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
        StoryEvent = 3,
        Town = 4
    }
}
