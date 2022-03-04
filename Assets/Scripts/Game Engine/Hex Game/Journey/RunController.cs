using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using HexGameEngine.Utilities;
using HexGameEngine.Characters;
using HexGameEngine.Persistency;

namespace HexGameEngine.JourneyLogic
{

    public class RunController : Singleton<RunController>
    {
        // Properties + Component Refs
        #region
        [Header("General Properties")]
        [SerializeField] private bool allowSameEnemyWaveMultipleTimes;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Combat Encounter Data")]
        [SerializeField] private EnemyEncounterSet[] allCombatEncounterSets;


        private List<EnemyEncounterData> enemyWavesAlreadyEncountered = new List<EnemyEncounterData>();       

        #endregion

        // Getters + Accessors
        #region
        public EnemyEncounterData CurrentCombatEncounterData { get; private set; }
        public EncounterType CurrentEncounterType { get; private set; }
        public SaveCheckPoint SaveCheckPoint { get; private set; }
        public int CurrentJourneyPosition { get; private set; }
        public int CurrentAct { get; private set; }
        public bool AllowSameEnemyWaveMultipleTimes
        {
            get { return allowSameEnemyWaveMultipleTimes; }
            private set { allowSameEnemyWaveMultipleTimes = value; }
        }

        #endregion

        // Initialization, Save + Load Logic
        #region
        public void BuildMyDataFromSaveFile(SaveGameData saveData)
        {
            CurrentJourneyPosition = saveData.currentJourneyPosition;
            CurrentAct = saveData.currentAct;
            SetCurrentEncounterType(saveData.currentEncounterType);
            SetCheckPoint(saveData.saveCheckPoint);
            enemyWavesAlreadyEncountered = saveData.encounteredCombats;
            UpdateCurrentEncounterText();
            SetCurrentEnemyEncounter(saveData.currentCombatEncounterData);
        }
        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            saveFile.currentJourneyPosition = CurrentJourneyPosition;
            saveFile.currentAct = CurrentAct;
            saveFile.currentEncounterType = CurrentEncounterType;
            saveFile.currentCombatEncounterData = CurrentCombatEncounterData;
            saveFile.saveCheckPoint = SaveCheckPoint;
            saveFile.encounteredCombats = enemyWavesAlreadyEncountered;
        }
        public void SetGameStartValues()
        {
            CurrentJourneyPosition = 0;
            CurrentAct = 1;
            SetCurrentEncounterType(EncounterType.DraftEvent);
            SetCheckPoint(SaveCheckPoint.DraftEvent);
            enemyWavesAlreadyEncountered.Clear();
            CurrentCombatEncounterData = null;
        }

        #endregion

        // Modify Core Journey Properties
        #region
        public void SetCurrentEncounterType(EncounterType type)
        {
            CurrentEncounterType = type;
        }
        public void SetCheckPoint(SaveCheckPoint type)
        {
            SaveCheckPoint = type;
        }
        public void IncrementWorldMapPosition()
        {
            CurrentJourneyPosition++;
            UpdateCurrentEncounterText();
        }
        private void UpdateCurrentEncounterText()
        {
            //TopBarController.Instance.CurrentEncounterText.text = (CurrentJourneyPosition + 1).ToString();
            //TopBarController.Instance.MaxEncounterText.text = (MapSystem.MapManager.Instance.config.layers.Length + 1).ToString();
        }
        #endregion

        // Get + Set Enemy Waves
        #region
        public void SetCurrentEnemyEncounter(EnemyEncounterData wave)
        {
            CurrentCombatEncounterData = wave;
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
                        if (!HasCombatAlreadyBeenEncountered(encounter.encounterName) ||
                            (HasCombatAlreadyBeenEncountered(encounter.encounterName) && AllowSameEnemyWaveMultipleTimes))
                        {
                            validCombats.Add(encounter);
                        }
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
        public EnemyEncounterData GenerateEnemyEncounterFromTemplate(EnemyEncounterSO template)
        {
            EnemyEncounterData ret = new EnemyEncounterData();

            ret.baseXpReward = template.baseXpReward;
            ret.difficulty = template.difficulty;
            ret.encounterName = template.encounterName;
            ret.enemiesInEncounter = new List<HexCharacterData>();

            // Create all enemies in wave
            foreach (EnemyGroup enemyGroup in template.enemyGroups)
            {
                // Random choose enemy data
                int randomIndex = Random.Range(0, enemyGroup.possibleEnemies.Count);
                EnemyTemplateSO data = enemyGroup.possibleEnemies[randomIndex];
                HexCharacterData enemy = CharacterDataController.Instance.GenerateEnemyDataFromEnemyTemplate(data);
                ret.enemiesInEncounter.Add(enemy);
            }

            return ret;
        }
        public void AddEnemyWaveToAlreadyEncounteredList(EnemyEncounterData wave)
        {
            Debug.Log("JourneyManager.AddEnemyWaveToAlreadyEncounteredList() adding " + wave.encounterName + " to already encounter list");
            enemyWavesAlreadyEncountered.Add(wave);
        }
        #endregion

        // Misc
        #region
        private bool HasCombatAlreadyBeenEncountered(string encounterName)
        {
            bool ret = false;
            foreach(EnemyEncounterData e in enemyWavesAlreadyEncountered)
            {
                if(e.encounterName == encounterName)
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }
        #endregion

    }
}