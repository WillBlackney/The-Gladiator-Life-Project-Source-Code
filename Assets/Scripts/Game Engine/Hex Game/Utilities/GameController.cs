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

            else if (GlobalSettings.Instance.GameMode == GameMode.CampSiteSandbox)
                RunCampSiteSandbox();

            else if (GlobalSettings.Instance.GameMode == GameMode.CharacterDraftSandbox)
                RunCharacterDraftTestEventSetup();
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

            // TO DO: Build town views here
            TownController.Instance.ShowTownView();
            CharacterScrollPanelController.Instance.BuildAndShowPanel();

            AudioManager.Instance.FadeInSound(Sound.Ambience_Outdoor_Spooky, 1f);
            BlackScreenController.Instance.FadeInScreen(1f);
        }
        private void RunSandboxCombat()
        {
            // Set state
            SetGameState(GameState.CombatActive); 

            // Show UI
            TopBarController.Instance.ShowTopBar();

            // Build mock save file + journey data
            RunController.Instance.SetGameStartValues();
            RunController.Instance.SetCurrentEncounterType(EncounterType.BasicEnemy);
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

            // Generate enemy wave + enemies data + save to run controller           
            EnemyEncounterData enemyEncounter = RunController.Instance.GenerateEnemyEncounterFromTemplate(GlobalSettings.Instance.SandboxEnemyEncounters.GetRandomElement());
            RunController.Instance.SetCurrentEnemyEncounter(enemyEncounter);

            // Spawn enemies in world
            HexCharacterController.Instance.SpawnEnemyEncounter(enemyEncounter);

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
            SetGameState(GameState.CombatRewardPhase);

            // Set up day + time
            //RunController.Instance.SetGameStartValues();

            // Show UI
            TopBarController.Instance.ShowTopBar();

            // Create + setup hex level
           // LevelController.Instance.SetupNewHexMap(GlobalSettings.Instance.SandboxLevelSeed);

            // Reset Camera + Lighting
            LightController.Instance.EnableDungeonGlobalLight();
            //CameraController.Instance.DoCameraMove(LevelController.Instance.CurrentHexMap.WorldCentre, 0);

            // Build character roster from mock data
            List<HexCharacterData> characters = CreateSandboxCharacterDataFiles();
            CharacterDataController.Instance.BuildCharacterRoster(characters);

            // Setup player characters
            HexCharacterController.Instance.CreateAllPlayerCombatCharacters(CharacterDataController.Instance.AllPlayerCharacters);

            StartCombatVictorySequence();
        }
        private void RunCampSiteSandbox()
        {
            // Set state
            SetGameState(GameState.NonCombatEvent);

            // Show UI
            TopBarController.Instance.ShowTopBar();

            // Reset Camera + Lighting           
            CameraController.Instance.ResetMainCameraPositionAndZoom();

            // Reset Camera + Lighting
            LightController.Instance.EnableStandardGlobalLight();

            // Build character roster + mock data
            List<HexCharacterData> characters = CreateSandboxCharacterDataFiles();
            CharacterDataController.Instance.BuildCharacterRoster(characters);

            // Build mock map data
            MapManager.Instance.SetCurrentMap(MapManager.Instance.GenerateNewMap());
            MapPlayerTracker.Instance.LockMap();

            // Load new camp event
            HandleLoadCampSiteEvent();
        }
        private void RunCharacterDraftTestEventSetup()
        {
            // Set state
            SetGameState(GameState.NonCombatEvent);

            // Build mock map data
            MapManager.Instance.SetCurrentMap(MapManager.Instance.GenerateNewMap());
            MapPlayerTracker.Instance.LockMap();

            // Show UI
            TopBarController.Instance.ShowTopBar();

            // Reset Camera + Lighting           
            CameraController.Instance.ResetMainCameraPositionAndZoom();

            // Build character roster from mock data
            List<HexCharacterData> characters = new List<HexCharacterData>();
            characters.Add(CreateSandboxCharacterDataFiles()[0]);
            CharacterDataController.Instance.BuildCharacterRoster(characters);

            HandleLoadCharacterDraftEvent();
        }
        #endregion

        // Handle Post Combat Stuff
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

            // Loading in from main menu? or combat just finished?
            if(RunController.Instance.SaveCheckPoint != SaveCheckPoint.CombatEnd)
            {
                // Determine characters to reward XP to
                List<HexCharacterModel> charactersRewarded = new List<HexCharacterModel>();
                foreach (HexCharacterModel character in HexCharacterController.Instance.AllDefenders)
                {
                    if (character.livingState == LivingState.Alive && character.currentHealth > 0)
                    {
                        charactersRewarded.Add(character);
                    }
                }

                // Reward XP, build and show combat stats screen
                List<CharacterCombatStatData> combatStats = CombatRewardController.Instance.GenerateCombatStatResultsForCharacters(charactersRewarded);
                CombatRewardController.Instance.CacheStatResult(combatStats);
                CombatRewardController.Instance.ApplyXpGainFromStatResultsToCharacters(combatStats);
                CombatRewardController.Instance.BuildAndShowPostCombatScreen(combatStats);

                // Generate loot data


                // Cache loot data + combat stat data to persistency, save data and set save checkpoint.
                RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatEnd);
                PersistencyController.Instance.AutoUpdateSaveFile();
            }
            else
            {

            }
           

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

            // Hide world map + roster
            MapView.Instance.HideMainMapView();
            CharacterRosterViewController.Instance.HideCharacterRosterScreen();

            // Hide Loot screen elements
            CombatRewardController.Instance.HidePostCombatRewardScreen();

            // Hide Draft event elements
            DraftEventController.Instance.HideMainViewParent();
            LevelController.Instance.DisableGraveyardScenery();

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
            HandleLoadEncounter(RunController.Instance.CurrentEncounterType);
        }
        #endregion

        // Load Encounters Logic (General)
        #region
        public void HandleLoadIntoCombatFromDeploymentScreen()
        {

        }



        public void HandleLoadEncounter(EncounterType encounter)
        {
            Debug.LogWarning("EventSequenceController.HandleLoadEncounter() loading: " + encounter.ToString());

            if ((encounter == EncounterType.BasicEnemy ||
                encounter == EncounterType.EliteEnemy ||
                 encounter == EncounterType.MysteryCombat ||
                encounter == EncounterType.BossEnemy) &&
                RunController.Instance.SaveCheckPoint == SaveCheckPoint.CombatStart
                )
            {
                
                HandleLoadCombatEncounter(RunController.Instance.CurrentCombatEncounterData);
            }

            else if ((encounter == EncounterType.BasicEnemy ||
                 encounter == EncounterType.MysteryCombat ||
                encounter == EncounterType.EliteEnemy) &&
                RunController.Instance.SaveCheckPoint == SaveCheckPoint.CombatEnd
                )
            {
                BlackScreenController.Instance.FadeInScreen(1f);
               // LevelManager.Instance.EnableDungeonScenery();
                //CharacterEntityController.Instance.CreateAllPlayerCombatCharacters();
                //StartCombatVictorySequence(encounter);
            }

            /*
            else if (RunController.Instance.SaveCheckPoint == SaveCheckPoint.DraftEvent)
            {
                HandleLoadCharacterDraftEvent();
            }*/

            /*
            else if (JourneyManager.Instance.CheckPointType == SaveCheckPoint.CampSite)
            {
                HandleLoadCampSiteEvent();
            }
            else if (JourneyManager.Instance.CheckPointType == SaveCheckPoint.Shop)
            {
                HandleLoadShopEvent();
            }
            else if (JourneyManager.Instance.CheckPointType == SaveCheckPoint.Shrine)
            {
                HandleLoadShrineEvent();
            }
            else if (JourneyManager.Instance.CheckPointType == SaveCheckPoint.MysteryEventStart)
            {
                HandleLoadMysteryEvent();
            }
            */
        }
        public void HandleLoadNextEncounter(MapNode mapNode)
        {
            StartCoroutine(HandleLoadNextEncounterCoroutine(mapNode));
        }
        private IEnumerator HandleLoadNextEncounterCoroutine(MapNode mapNode)
        {
            // Hide world map view
            MapView.Instance.HideMainMapView();

            // Cache previous encounter data 
            EncounterType nextEncounterType = mapNode.Node.NodeType;
            EncounterType previousEncounter = RunController.Instance.CurrentEncounterType;
            EnemyEncounterData previousEnemyWave = RunController.Instance.CurrentCombatEncounterData;

            // If mystery node, roll for story event, combat or shop
            if (nextEncounterType == EncounterType.Story)
            {
                int roll = RandomGenerator.NumberBetween(1, 100);
                if (roll >= 81 && roll <= 90)
                {
                    nextEncounterType = EncounterType.BasicEnemy;
                }
                else if (roll >= 91 && roll <= 95)
                {
                    nextEncounterType = EncounterType.EliteEnemy;
                }
                else if (roll >= 96 && roll <= 100)
                {
                    nextEncounterType = EncounterType.Shop;
                }

                Debug.Log("Mystery event roll result: " + nextEncounterType.ToString());

                /* UNCOMMENT AFTER REIMPLEMENTING STORY EVENTS
                 * 
                // Force roll as combat event if no available story events
                if (nextEncounterType == EncounterType.Story &&
                   StoryEventController.Instance.GetValidStoryEvents().Count == 0)
                {
                    nextEncounterType = EncounterType.BasicEnemy;
                    Debug.Log("No valid story events at this time, next encounter will be a basic enemy combat...");
                }
                */
            }

            // Increment world position + set next encounter
            //ScoreManager.Instance.IncrementRoomsCleared();
            RunController.Instance.SetCurrentEncounterType(nextEncounterType);

            // Destroy all characters and activation windows if the previous encounter was a combat event
            if (previousEncounter == EncounterType.BasicEnemy ||
                previousEncounter == EncounterType.EliteEnemy ||
                previousEncounter == EncounterType.MysteryCombat)
            {
                // Mark wave as seen
                RunController.Instance.AddEnemyWaveToAlreadyEncounteredList(previousEnemyWave);

                // Update scoring
                // if (previousEncounter == EncounterType.BasicEnemy) ScoreManager.Instance.IncrementBasicsCleared();
                // else if (previousEncounter == EncounterType.EliteEnemy) ScoreManager.Instance.IncrementMinibossesCleared();

                // Fade out visual event
                BlackScreenController.Instance.FadeOutScreen(1.5f);

                // Hide Loot screen elements
                CombatRewardController.Instance.HidePostCombatRewardScreen();

                // Move characters off screen
                HexCharacterController.Instance.MoveCharactersToOffScreenRight(HexCharacterController.Instance.AllDefenders, null);
                AudioManager.Instance.FadeOutSound(Sound.Environment_Camp_Fire, 3f);

                // Zoom and move camera & Fade foot steps
                yield return new WaitForSeconds(0.5f);
                AudioManager.Instance.FadeOutSound(Sound.Character_Footsteps, 2.5f);
                CameraController.Instance.DoCameraMove(3, 0, 3f);
                CameraController.Instance.DoCameraZoom(5, 3, 3f);

                // Wait for visual events
                yield return new WaitForSeconds(4f);

                // Tear down remaining combat views
                HexCharacterController.Instance.HandleTearDownCombatScene();
                LevelController.Instance.HandleTearDownCombatViews();
                yield return new WaitForSeconds(1.5f);

                

            }

            // TO DO IN FUTURE: Boss victory means loading the next act, so we need
            // different loading/continuation logic here. We also need to end the game,
            // show score, etc, if the boss is last boss (act 3).
            else if (previousEncounter == EncounterType.BossEnemy)
            {

            }

            // Do draft event end sequence + tear down
            else if (previousEncounter == EncounterType.DraftEvent)
            {
                CoroutineData kbcSequence = new CoroutineData();
                HandlDraftEventContinueSequence(kbcSequence);
                yield return new WaitUntil(() => kbcSequence.CoroutineCompleted());
                LevelController.Instance.DisableGraveyardScenery();
            }

            // Do camp site end sequence + tear down
            else if (previousEncounter == EncounterType.CampSite)
            {
                // Fade out Screen
                BlackScreenController.Instance.FadeOutScreen(2f);
                yield return new WaitForSeconds(2f);

                // Close vamp site views
                CampSiteController.Instance.HideMainView();

                
            }

            // Mystery Event teardown
            else if (previousEncounter == EncounterType.Story)
            {
                /*
                // Fade out Screen
                BlackScreenController.Instance.FadeOutScreen(1f);

                // Wait for visual events
                yield return new WaitForSeconds(1f);

                // Close views and clear data
                StoryEventController.Instance.ClearCurrentStoryEvent();
                StoryEventController.Instance.HideMainScreen();
                */
            }

            // Do shop end sequence + tear down
            else if (previousEncounter == EncounterType.Shop)
            {
                /*
                // Shop keeper farewell
                ShopController.Instance.DoMerchantFarewell();

                // disable shop keeper clickability + GUI
                ShopController.Instance.SetShopKeeperInteractionState(false);
                ShopController.Instance.SetContinueButtonInteractionState(false);
                ShopController.Instance.HideContinueButton();

                // Clear shop content result data
                ShopController.Instance.ClearShopContentDataSet();

                // Move characters off screen
                ShopController.Instance.MoveCharactersToOffScreenRight();

                // Zoom and move camera
                yield return new WaitForSeconds(0.5f);
                CameraManager.Instance.DoCameraMove(3, 0, 3f);
                CameraManager.Instance.DoCameraZoom(5, 3, 3f);

                // Fade out Screen
                BlackScreenController.Instance.FadeOutScreen(3f);

                // Wait for visual events
                yield return new WaitForSeconds(4f);

                ShopController.Instance.DisableCharacterViewParent();
                LevelManager.Instance.DisableShopScenery();
                */
            }

            // Do shrine end sequence + tear down
            else if (previousEncounter == EncounterType.TreasureRoom)
            {
                /*
                // Hide shrine GUI
                ShrineController.Instance.HideContinueButton();

                // Move characters off screen
                ShrineController.Instance.MoveCharactersToOffScreenRight();

                // Zoom and move camera
                yield return new WaitForSeconds(0.5f);
                CameraManager.Instance.DoCameraMove(3, 0, 3f);
                CameraManager.Instance.DoCameraZoom(5, 3, 3f);

                // Fade out Screen
                BlackScreenController.Instance.FadeOutScreen(3f);

                // Wait for visual events
                yield return new WaitForSeconds(4f);

                ShrineController.Instance.DisableAllViews();
                LevelManager.Instance.DisableShrineScenery();
                */
            }


            // ::: LOAD NEXT EVENT START :::

            // Reset Camera
            CameraController.Instance.ResetMainCameraPositionAndZoom();

            // If next event is a combat, get + set enemy wave before saving to disk
            if (RunController.Instance.CurrentEncounterType == EncounterType.BasicEnemy ||
                RunController.Instance.CurrentEncounterType == EncounterType.EliteEnemy ||
                RunController.Instance.CurrentEncounterType == EncounterType.BossEnemy)
            {
                if (RunController.Instance.CurrentEncounterType == EncounterType.BasicEnemy)
                {
                    RunController.Instance.SetCurrentEnemyEncounter
                        (RunController.Instance.GenerateEnemyEncounterFromTemplate
                        (RunController.Instance.GetRandomCombatData(RunController.Instance.CurrentChapter, CombatDifficulty.Basic)));
                }
                
                else if (RunController.Instance.CurrentEncounterType == EncounterType.EliteEnemy)
                {
                    RunController.Instance.SetCurrentEnemyEncounter
                        (RunController.Instance.GenerateEnemyEncounterFromTemplate
                        (RunController.Instance.GetRandomCombatData(RunController.Instance.CurrentChapter, CombatDifficulty.Elite)));
                }


                // Set check point
                RunController.Instance.SetCheckPoint(SaveCheckPoint.CombatStart);

                // Auto save
                PersistencyController.Instance.AutoUpdateSaveFile();

                // Start Load combat
                HandleLoadCombatEncounter(RunController.Instance.CurrentCombatEncounterData);
            }

            // Shop event
            else if (RunController.Instance.CurrentEncounterType == EncounterType.Shop)
            {
                /*
                // Generate new shop contents
                if (ShopController.Instance.CurrentShopContentResultData == null)
                {
                    ShopController.Instance.SetAndCacheNewShopContentDataSet();
                }

                // Set check point
                JourneyManager.Instance.SetCheckPoint(SaveCheckPoint.Shop);

                // Auto save
                PersistencyManager.Instance.AutoUpdateSaveFile();

                HandleLoadShopEvent();
                */
            }

            // Shrine event
            else if (RunController.Instance.CurrentEncounterType == EncounterType.TreasureRoom)
            {
                /*
                // Generate new shop contents
                if (ShrineController.Instance.CurrentShrineStates == null)
                {
                    ShrineController.Instance.SetAndCacheNewShrineContentDataSet();
                }

                // Set check point
                JourneyManager.Instance.SetCheckPoint(SaveCheckPoint.Shrine);

                // Auto save
                PersistencyManager.Instance.AutoUpdateSaveFile();

                HandleLoadShrineEvent();
                */
            }

            // Draft
            else if (RunController.Instance.CurrentEncounterType == EncounterType.DraftEvent)
            {
                // Set check point
                //RunController.Instance.SetCheckPoint(SaveCheckPoint.DraftEvent);

                // Auto save
                PersistencyController.Instance.AutoUpdateSaveFile();

                HandleLoadCharacterDraftEvent();
            }

            // Camp site
            else if (RunController.Instance.CurrentEncounterType == EncounterType.CampSite)
            {
                // Set check point
               // RunController.Instance.SetCheckPoint(SaveCheckPoint.CampSite);

                // Auto save
                PersistencyController.Instance.AutoUpdateSaveFile();

                HandleLoadCampSiteEvent();
            }

            // Story Event 
            else if (RunController.Instance.CurrentEncounterType == EncounterType.Story)
            {
                /*
                // Generate and cache mystery event, if dont have one saved
                if (StoryEventController.Instance.CurrentStoryEvent == null)
                {
                    StoryEventController.Instance.GenerateAndCacheNextStoryEventRandomly();
                }

                // Set check point
                RunController.Instance.SetCheckPoint(SaveCheckPoint.MysteryEventStart);

                // Auto save
                PersistencyController.Instance.AutoUpdateSaveFile();

                HandleLoadMysteryEvent();
                */
            }
        }
        private void HandleLoadCombatEncounter(EnemyEncounterData enemyWave)
        {
            // Set state
            SetGameState(GameState.CombatActive);
            MapPlayerTracker.Instance.LockMap();

            // Enable ambience if not playing
            if (!AudioManager.Instance.IsSoundPlaying(Sound.Ambience_Crypt))            
                AudioManager.Instance.FadeInSound(Sound.Ambience_Crypt, 1f);

            // Enable world view
            LightController.Instance.EnableDungeonGlobalLight();
            LevelController.Instance.EnableNightTimeArenaScenery();
            LevelController.Instance.ShowAllNodeViews();

            // Spawn player characters
            HexCharacterController.Instance.CreateAllPlayerCombatCharacters(CharacterDataController.Instance.AllPlayerCharacters);

            // Spawn enemies in world
            HexCharacterController.Instance.SpawnEnemyEncounter(enemyWave);           

            // Camera Zoom out effect
            CameraController.Instance.DoCameraZoom(4, 5, 1);

            // Play battle music
            AudioManager.Instance.AutoPlayBasicCombatMusic(1f);

            // Fade In
            BlackScreenController.Instance.FadeInScreen(1f);

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
                    characters.Add(CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(c));
                }
            }
            else
            {
                foreach (HexCharacterTemplateSO dataSO in GlobalSettings.Instance.ChosenCharacterTemplates)
                {
                    characters.Add(CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(dataSO));
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
