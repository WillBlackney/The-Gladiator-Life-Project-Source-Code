using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WeAreGladiators.Boons;
using WeAreGladiators.Characters;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Perks;
using WeAreGladiators.Persistency;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;
using WeAreGladiators.VisualEvents;

namespace WeAreGladiators.JourneyLogic
{
    public class RunController : Singleton<RunController>
    {
        #region Properties + Component
        [Header("Run Timer Components")]
        [Tooltip("When disabled, the game will not track or display the play time duration of the player's runs")]
        [SerializeField] private bool disableRunTiming;
        [SerializeField] private TextMeshProUGUI runTimerText;
        [SerializeField] private GameObject runTimerVisualParent;
        [Space(10)]

        [Header("Combat Encounter Data")]
        [SerializeField] private EnemyEncounterSet[] allCombatEncounterSets;
    
        private float runTimer;
        private bool updateTimer;
        #endregion

        #region Getters + Accessors
        public int CurrentGold { get; private set; }
        public CombatContractData CurrentCombatContractData { get; private set; }
        public List<CharacterWithSpawnData> CurrentDeployedCharacters { get; private set; } = new List<CharacterWithSpawnData>();
        public SaveCheckPoint SaveCheckPoint { get; private set; }
        public int CurrentDay { get; private set; }
        public int CurrentChapter { get; private set; }
        public SerializedCombatMapData CurrentCombatMapData { get; private set; }
        #endregion

        #region Initialization, Save + Load Logic
        private void Start()
        {
            runTimerVisualParent.SetActive(true);
            if (disableRunTiming)
            {
                runTimerVisualParent.SetActive(false);
            }
        }
        public void BuildMyDataFromSaveFile(SaveGameData saveData)
        {
            CurrentDay = saveData.currentDay;
            CurrentChapter = saveData.currentChapter;
            CurrentCombatContractData = saveData.currentCombatContractData;
            CurrentDeployedCharacters = saveData.playerCombatCharacters;
            CurrentCombatMapData = saveData.currentCombatMapData;
            SetCheckPoint(saveData.saveCheckPoint);
            runTimer = saveData.runTimer;
            SetPlayerGold(saveData.currentGold);

            StartTimer();
            UpdateDayAndChapterTopbarText();
        }
        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            saveFile.currentDay = CurrentDay;
            saveFile.currentChapter = CurrentChapter;
            saveFile.currentCombatMapData = CurrentCombatMapData;
            saveFile.currentCombatContractData = CurrentCombatContractData;
            saveFile.saveCheckPoint = SaveCheckPoint;
            saveFile.playerCombatCharacters = CurrentDeployedCharacters;
            saveFile.runTimer = runTimer;
            saveFile.currentGold = CurrentGold;
        }
        public void SetGameStartValues()
        {
            CurrentDay = 1;
            CurrentChapter = 1;
            runTimer = 0;
            if (GlobalSettings.Instance.GameMode != GameMode.Standard)
            {
                CurrentDay = GlobalSettings.Instance.StartingDay;
                CurrentChapter = GlobalSettings.Instance.StartingChapter;
            }
            SetCheckPoint(SaveCheckPoint.GameIntroEvent);
            StartTimer();
            CurrentCombatContractData = null;

            ModifyPlayerGold(GlobalSettings.Instance.BaseStartingGold);
        }
        #endregion

        #region Core Game Cycle Events
        public bool OnNewDayStart()
        {
            bool newChapterStarted = false;
            if (CurrentDay == 5)
            {
                newChapterStarted = true;
                CurrentDay = 1;
                CurrentChapter++;
                OnNewChapterStart();
            }
            else
            {
                CurrentDay++;
            }

            // Update day + act GUI
            UpdateDayAndChapterTopbarText();

            // Boon expiries
            BoonController.Instance.TickDownBoonsOnNewDayStart();

            // Pay daily wages
            CharacterDataController.Instance.HandlePayDailyWagesOnNewDayStart();

            // Reduce all injury duration by 1 day
            PerkController.Instance.HandleTickDownInjuriesOnNewDayStart();

            // Generate new daily recruits
            TownController.Instance.GenerateDailyRecruits(RandomGenerator.NumberBetween(5, 7));

            // Generate new combat contracts
            TownController.Instance.GenerateDailyCombatContracts();

            // Refresh library books
            TownController.Instance.GenerateDailyAbilityTomes();

            // Refresh armoury items
            TownController.Instance.GenerateDailyArmouryItems();

            return newChapterStarted;

        }
        public void OnNewChapterStart()
        {
            // TO DO: generate reputation reward choices
        }

        #endregion

        #region Modify Core Journey Properties
        public void SetCheckPoint(SaveCheckPoint type)
        {
            SaveCheckPoint = type;
        }
        private void UpdateDayAndChapterTopbarText()
        {
            TopBarController.Instance.CurrentDaytext.text = "Day: " + CurrentDay + " of 5";
        }
        #endregion

        #region Get + Set Enemy Waves
        public void SetCurrentCombatMapData(SerializedCombatMapData mapData)
        {
            CurrentCombatMapData = mapData;
        }
        public void SetPlayerDeployedCharacters(List<CharacterWithSpawnData> characters)
        {
            CurrentDeployedCharacters = characters;
        }
        public void SetCurrentContractData(CombatContractData wave)
        {
            CurrentCombatContractData = wave;
        }
        public EnemyEncounterSO GetRandomCombatData(int currentAct, CombatDifficulty difficulty)
        {
            Debug.Log("RunController.GetRandomCombatData() getting random combat for act " + currentAct + " and difficulty " + difficulty);
            EnemyEncounterSO ret = null;
            foreach (EnemyEncounterSet set in allCombatEncounterSets)
            {
                if (currentAct >= set.actRangeLower &&
                    currentAct <= set.actRangeUpper &&
                    set.combatDifficulty == difficulty)
                {
                    // Filter out all combats the player has already encountered during this run.
                    List<EnemyEncounterSO> validCombats = new List<EnemyEncounterSO>();
                    foreach (EnemyEncounterSO encounter in set.possibleEnemyEncounters)
                    {
                        validCombats.Add(encounter);
                    }

                    // No unseen combats in the set, just give one the player has already seen
                    if (validCombats.Count == 0)
                    {
                        ret = set.possibleEnemyEncounters[RandomGenerator.NumberBetween(0, set.possibleEnemyEncounters.Count - 1)];
                    }

                    // If only one valid choice, select it
                    else if (validCombats.Count == 1)
                    {
                        ret = validCombats[0];
                    }

                    // Choose a random unseen combat if there are many
                    else
                    {
                        ret = validCombats[RandomGenerator.NumberBetween(0, validCombats.Count - 1)];
                    }

                    break;
                }
            }

            return ret;

        }
        public List<EnemyEncounterSO> GetCombatData(int currentAct, CombatDifficulty difficulty)
        {
            List<EnemyEncounterSO> ret = new List<EnemyEncounterSO>();
            foreach (EnemyEncounterSet set in allCombatEncounterSets)
            {
                if (currentAct >= set.actRangeLower &&
                    currentAct <= set.actRangeUpper &&
                    set.combatDifficulty == difficulty)
                {
                    foreach (EnemyEncounterSO e in set.possibleEnemyEncounters)
                    {
                        ret.Add(e);
                    }
                    break;
                }
            }
            return ret;
        }
        public EnemyEncounterData GenerateEnemyEncounterFromTemplate(EnemyEncounterSO template)
        {
            EnemyEncounterData ret = new EnemyEncounterData();

            ret.difficulty = template.difficulty;
            ret.deploymentLimit = template.deploymentLimit;
            ret.enemiesInEncounter = new List<CharacterWithSpawnData>();

            // Create all enemies in wave
            foreach (EnemyGroup enemyGroup in template.enemyGroups)
            {
                // Random choose enemy data
                int randomIndex = Random.Range(0, enemyGroup.possibleEnemies.Count);
                EnemyTemplateSO data = enemyGroup.possibleEnemies[randomIndex];
                HexCharacterData enemy = CharacterDataController.Instance.GenerateCharacterDataFromEnemyTemplate(data);
                ret.enemiesInEncounter.Add(new CharacterWithSpawnData(enemy, enemyGroup.spawnPosition));
            }

            return ret;
        }
        #endregion

        #region Run Timer Logic
        private void Update()
        {
            if (!disableRunTiming && updateTimer)
            {
                UpdateTimer();
            }
        }
        private void UpdateTimer()
        {
            runTimer += Time.deltaTime;

            int seconds = (int) (runTimer % 60);
            int minutes = (int) (runTimer / 60) % 60;
            int hours = (int) (runTimer / 3600) % 24;

            string runTimerString = string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);
            runTimerText.text = runTimerString;
        }
        public void StartTimer()
        {
            if (disableRunTiming) return;
            runTimerVisualParent.SetActive(true);
            updateTimer = true;
        }
        public void PauseTimer()
        {
            if (disableRunTiming) return;
            runTimerVisualParent.SetActive(false);
            updateTimer = false;
        }
        #endregion

        #region Gold Logic
        public void ModifyPlayerGold(int goldGainedOrLost)
        {
            CurrentGold += goldGainedOrLost;
            int vEventValue = CurrentGold;
            VisualEventManager.CreateVisualEvent(() => TopBarController.Instance.UpdateGoldText(vEventValue.ToString()));
        }
        private void SetPlayerGold(int newValue)
        {
            CurrentGold = newValue;
            int vEventValue = CurrentGold;
            VisualEventManager.CreateVisualEvent(() => TopBarController.Instance.UpdateGoldText(vEventValue.ToString()));
        }

        #endregion
    }
}
