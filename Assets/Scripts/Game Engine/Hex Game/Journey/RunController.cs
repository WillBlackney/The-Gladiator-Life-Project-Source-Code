using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using HexGameEngine.Utilities;
using HexGameEngine.Characters;
using HexGameEngine.Persistency;
using HexGameEngine.UI;
using HexGameEngine.TownFeatures;
using HexGameEngine.Perks;

namespace HexGameEngine.JourneyLogic
{

    public class RunController : Singleton<RunController>
    {
        // Properties + Component Refs
        #region
        [Header("Combat Encounter Data")]
        [SerializeField] private EnemyEncounterSet[] allCombatEncounterSets;

        // Non inspector fields     
        public List<CharacterWithSpawnData> currentDeployedCharacters = new List<CharacterWithSpawnData>();
        #endregion

        // Getters + Accessors
        #region
        public CombatContractData CurrentCombatContractData { get; private set; }
        public List<CharacterWithSpawnData> CurrentDeployedCharacters 
        { 
            get { return currentDeployedCharacters; } 
            private set { currentDeployedCharacters = value; }
        }
        public SaveCheckPoint SaveCheckPoint { get; private set; }
        public int CurrentDay { get; private set; }
        public int CurrentChapter { get; private set; }
       

        #endregion

        // Initialization, Save + Load Logic
        #region
        public void BuildMyDataFromSaveFile(SaveGameData saveData)
        {
            CurrentDay = saveData.currentDay;
            CurrentChapter = saveData.currentChapter;
            CurrentCombatContractData = saveData.currentCombatContractData;
            CurrentDeployedCharacters = saveData.playerCombatCharacters;
            SetCheckPoint(saveData.saveCheckPoint);

            UpdateDayAndChapterTopbarText();
        }
        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            saveFile.currentDay = CurrentDay;
            saveFile.currentChapter = CurrentChapter;
            saveFile.currentCombatContractData = CurrentCombatContractData;
            saveFile.saveCheckPoint = SaveCheckPoint;
            saveFile.playerCombatCharacters = CurrentDeployedCharacters;
        }
        public void SetGameStartValues()
        {
            CurrentDay = 1;
            CurrentChapter = 1;
            if(GlobalSettings.Instance.GameMode != GameMode.Standard)
            {
                CurrentDay = GlobalSettings.Instance.StartingDay;
                CurrentChapter = GlobalSettings.Instance.StartingChapter;
            }
            SetCheckPoint(SaveCheckPoint.Town);
            CurrentCombatContractData = null;
        }

        #endregion

        // Core Events
        #region       
        public bool OnNewDayStart()
        {
            bool newChapterStarted = false;
            // Increment day
            if(CurrentDay == 5)
            {
                newChapterStarted = true;
                CurrentDay = 1;
                CurrentChapter++;
                OnNewChapterStart();
            }
            else CurrentDay++;

            // Update day + act GUI
            UpdateDayAndChapterTopbarText();

            // Pay daily wages
            CharacterDataController.Instance.HandlePayDailyWagesOnNewDayStart();

            // Reduce all injury duration by 1 day
            PerkController.Instance.HandleTickDownInjuriesOnNewDayStart();

            // Generate and apply effect of new random town events 

            // Generate new daily recruits
            TownController.Instance.GenerateDailyRecruits(RandomGenerator.NumberBetween(4,6));

            // Generate new combat contracts
            TownController.Instance.GenerateDailyCombatContracts();

            // Refresh library books
            TownController.Instance.GenerateDailyAbilityTomes();

            // Refresh armoury items
            TownController.Instance.GenerateDailyArmouryItems();

            // Characters in town features gain effect of hospital feature (heal, remove stress, remove all injuries, etc)
            TownController.Instance.HandleApplyHospitalFeaturesOnNewDayStart();

            // All characters heal 10% health and 5 stress
            CharacterDataController.Instance.HandlePassiveStressAndHealthRecoveryOnNewDayStart();

            return newChapterStarted;

        }
        public void OnNewChapterStart()
        {
            // TO DO: generate reputation reward choices

        }
        #endregion

        // Modify Core Journey Properties
        #region       
        public void SetCheckPoint(SaveCheckPoint type)
        {
            SaveCheckPoint = type;
        }       
        private void UpdateDayAndChapterTopbarText()
        {
            TopBarController.Instance.CurrentDaytext.text = "Day: " + CurrentDay.ToString();
            TopBarController.Instance.CurrentChapterText.text = "Chapter: " + CurrentChapter.ToString();
        }
        #endregion

        // Get + Set Enemy Waves
        #region
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
            EnemyEncounterSO ret = null;
            foreach (EnemyEncounterSet set in allCombatEncounterSets)
            {
                if (currentAct >= set.actRangeLower && 
                    currentAct <= set.actRangeUpper &&
                    set.combatDifficulty == difficulty)
                {
                    // Filter out all combats the player has already encountered during this run.
                    List<EnemyEncounterSO> validCombats = new List<EnemyEncounterSO>();
                    foreach(EnemyEncounterSO encounter in set.possibleEnemyEncounters)
                    {
                        validCombats.Add(encounter);
                    }                    

                    // No unseen combats in the set, just give one the player has already seen
                    if (validCombats.Count == 0)                        
                        ret = set.possibleEnemyEncounters[RandomGenerator.NumberBetween(0, set.possibleEnemyEncounters.Count - 1)];

                    // If only one valud choice, select it
                    else if(validCombats.Count == 1)
                        ret = validCombats[0];

                    // Choose a random unseen combat if there are many
                    else
                        ret = validCombats[RandomGenerator.NumberBetween(0, validCombats.Count - 1)];

                    break;
                }
            }

            return ret;

        }
        public List<EnemyEncounterSO> GetCombatData(int amount, int currentAct, CombatDifficulty difficulty)
        {
            List<EnemyEncounterSO> ret = new List<EnemyEncounterSO>();
            foreach (EnemyEncounterSet set in allCombatEncounterSets)
            {
                if (currentAct >= set.actRangeLower &&
                    currentAct <= set.actRangeUpper &&
                    set.combatDifficulty == difficulty)
                {
                    for (int i = 0; i < amount; i++)
                        ret.Add(set.possibleEnemyEncounters[i]);
                    break;
                }
            }
            return ret;
        }
        public EnemyEncounterData GenerateEnemyEncounterFromTemplate(EnemyEncounterSO template)
        {
            EnemyEncounterData ret = new EnemyEncounterData();

            ret.baseXpReward = template.baseXpReward;
            ret.difficulty = template.difficulty;
            ret.encounterName = template.encounterName;
            ret.enemiesInEncounter = new List<CharacterWithSpawnData>();

            // Create all enemies in wave
            foreach (EnemyGroup enemyGroup in template.enemyGroups)
            {
                // Random choose enemy data
                int randomIndex = Random.Range(0, enemyGroup.possibleEnemies.Count);
                EnemyTemplateSO data = enemyGroup.possibleEnemies[randomIndex];
                HexCharacterData enemy = CharacterDataController.Instance.GenerateEnemyDataFromEnemyTemplate(data);
                ret.enemiesInEncounter.Add(new CharacterWithSpawnData(enemy, enemyGroup.spawnPosition));
            }

            return ret;
        }
        
        #endregion

      


    }
}