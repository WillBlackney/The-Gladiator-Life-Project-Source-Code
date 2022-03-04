using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Characters;
using HexGameEngine.Abilities;
using HexGameEngine.Perks;
using DG.Tweening;
using System;
using Sirenix.OdinInspector;
using HexGameEngine.Cards;
using HexGameEngine.Persistency;
using HexGameEngine.JourneyLogic;
using HexGameEngine.UCM;
using HexGameEngine.Combat;
using HexGameEngine.DungeonMap;

namespace HexGameEngine.RewardSystems
{
    public class CombatRewardController : Singleton<CombatRewardController>
    {
        // Properties + Components
        #region
        [Header("Stat Screen Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private GameObject characterStatPageVisualParent;
        [SerializeField] private GameObject lootPageVisualParent;
        [SerializeField] private CharacterCombatStatCard[] allCharacterStatCards;
        [SerializeField] private GameObject[] statCardRows;

        private List<CharacterCombatStatData> currentStatResults = new List<CharacterCombatStatData>();

        private WindowState currentWindowViewing = WindowState.CharactersPage;
        private enum WindowState
        {
            CharactersPage = 0,
            LootPage = 1,
        }
        #endregion

        // Getters + Accesors
        #region
        public List<CharacterCombatStatData> CurrentStatResults
        {
            get { return currentStatResults; }
        }
        #endregion

        // Persistency Controller
        #region
        public void SaveMyDataToSaveFile(SaveGameData saveData)
        {
            saveData.currentCombatStatResult = currentStatResults;
        }
        public void BuildMyDataFromSaveFile(SaveGameData saveData)
        {
            currentStatResults = saveData.currentCombatStatResult;
        }
        public void CacheStatResult(List<CharacterCombatStatData> result)
        {
            currentStatResults = result;
        }
        #endregion

        // Xp Reward Logic
        #region
        public List<CharacterCombatStatData> GenerateCombatStatResultsForCharacters(List<HexCharacterModel> characters)
        {
            List<CharacterCombatStatData> dataRet = new List<CharacterCombatStatData>();

            // Calculate xp gain          
            int baseXp = RunController.Instance.CurrentCombatEncounterData.baseXpReward;
            int killXpSlice = 0;
            var encounterData = RunController.Instance.CurrentCombatEncounterData;

            // Prevent divide by zero error
            if (encounterData.TotalEnemyXP != 0 &&
                encounterData.TotalEnemies != 0)
                killXpSlice = encounterData.TotalEnemyXP / encounterData.TotalEnemies;

            foreach (HexCharacterModel character in characters)
            {
                CharacterCombatStatData result = GenerateCharacterCombatStatResult(character);
                dataRet.Add(result);

                // apply xp gain + level up
                int xpGained = baseXp + (killXpSlice * character.totalKills);
                int previousLevel = character.characterData.currentLevel;
                if(character.characterData.currentXP + xpGained >= character.characterData.currentMaxXP)
                {
                    result.didLevelUp = true;
                }
               // CharacterDataController.Instance.HandleGainXP(character.characterData, xpGained);
               // if (previousLevel < character.characterData.currentLevel)
                //    result.didLevelUp = true;
                result.xpGained = xpGained;
            }

            return dataRet;
        }
        public void ApplyXpGainFromStatResultsToCharacters(List<CharacterCombatStatData> results)
        {
            foreach(CharacterCombatStatData result in results)
            {
                CharacterDataController.Instance.HandleGainXP(result.characterData, result.xpGained);
            }
        }
        private CharacterCombatStatData GenerateCharacterCombatStatResult(HexCharacterModel character)
        {
            CharacterCombatStatData result = new CharacterCombatStatData();

            result.hexCharacter = character;
            result.characterData = character.characterData;
            result.totalKills = character.totalKills;
            result.healthLost = character.healthLostThisCombat;
            result.stressGained = character.stressGainedThisCombat;
            result.injuriesGained.AddRange(character.injuriesGainedThisCombat);
            // to do: need some extra logic for tracking PERMANENT injuires gained from combat

            return result;
        }
        #endregion

        // Loot Generation 
        #region
        public LootSet GenerateNewLootResult()
        {
            LootSet newLoot = new LootSet();

            // Generate gold reward
            newLoot.goldReward = 0;

            EnemyEncounterData combatData = RunController.Instance.CurrentCombatEncounterData;

            // Determine gold reward
            if (combatData.difficulty == CombatDifficulty.Basic)
            {
                newLoot.goldReward = RandomGenerator.NumberBetween(10, 20);
                //    (GlobalSettings.Instance.basicEnemyGoldRewardLowerLimit, GlobalSettings.Instance.basicEnemyGoldRewardUpperLimit);
            }
            else if (combatData.difficulty == CombatDifficulty.Basic)
            {
                newLoot.goldReward = RandomGenerator.NumberBetween(25, 35);
                //    (GlobalSettings.Instance.eliteEnemyGoldRewardLowerLimit, GlobalSettings.Instance.eliteEnemyGoldRewardUpperLimit);
            }


            /*
            else if (JourneyManager.Instance.CurrentEncounter == EncounterType.EliteEnemy)
            {
                newLoot.goldReward = RandomGenerator.NumberBetween
                    (GlobalSettings.Instance.eliteEnemyGoldRewardLowerLimit, GlobalSettings.Instance.eliteEnemyGoldRewardUpperLimit);
            }
            else if (JourneyManager.Instance.CurrentEncounter == EncounterType.BossEnemy)
            {
                newLoot.goldReward = RandomGenerator.NumberBetween
                    (GlobalSettings.Instance.bossEnemyGoldRewardLowerLimit, GlobalSettings.Instance.bossEnemyGoldRewardUpperLimit);
            }

            // Generate character card choices
            for (int i = 0; i < CharacterDataController.Instance.AllPlayerCharacters.Count; i++)
            {
                newLoot.allCharacterCardChoices.Add(new List<CardData>());
                newLoot.allCharacterCardChoices[i] = GenerateCharacterCardLootChoices(CharacterDataController.Instance.AllPlayerCharacters[i]);
            }

            // Forced loot item
            if (JourneyManager.Instance.CurrentEnemyWave.itemReward != null)
                newLoot.itemReward = ItemController.Instance.GetItemDataByName(JourneyManager.Instance.CurrentEnemyWave.itemReward.itemName);

            // Roll for a trinket reward normally
            else
            {
                bool shouldGetTrinket = false;
                int trinketRoll = RandomGenerator.NumberBetween(1, 100);

                if (JourneyManager.Instance.CurrentEncounter == EncounterType.BasicEnemy &&
                 trinketRoll <= GlobalSettings.Instance.basicTrinketProbability)
                    shouldGetTrinket = true;

                if (JourneyManager.Instance.CurrentEncounter == EncounterType.EliteEnemy &&
                  trinketRoll <= GlobalSettings.Instance.eliteTrinketProbability)
                    shouldGetTrinket = true;

                // Rolled successfully for trinket?
                if (shouldGetTrinket)
                    newLoot.itemReward = GetRandomTrinketLootReward();
            }
            */
            return newLoot;
        }
        #endregion

        // Build + Show Screen views
        #region
        public void HidePostCombatRewardScreen()
        {
            mainVisualParent.SetActive(false);
        }
        public void BuildAndShowPostCombatScreen(List<CharacterCombatStatData> data)
        {
            mainVisualParent.SetActive(true);
            characterStatPageVisualParent.SetActive(true);
            currentWindowViewing = WindowState.CharactersPage;
            lootPageVisualParent.SetActive(false);
            BuildCharacterStatCardPage(data);
        }
        private void BuildCharacterStatCardPage(List<CharacterCombatStatData> data)
        {           
            // Reset views
            foreach (CharacterCombatStatCard c in allCharacterStatCards)            
                c.gameObject.SetActive(false);            
            foreach (GameObject g in statCardRows)
                g.SetActive(false);

            // Build a card for each character
            for(int i = 0; i < data.Count; i++)
            {
                BuildStatCardFromStatData(allCharacterStatCards[i], data[i]);
            }

            // enable rows
            int rowsToShow = 1 + (data.Count / 3);
            for (int i = 0; i < rowsToShow; i++)
                statCardRows[i].SetActive(true);
        }
        private void BuildStatCardFromStatData(CharacterCombatStatCard card, CharacterCombatStatData data)
        {
            HexCharacterData character = data.characterData;

            // Reset
            foreach(CharacterCombatStatCardPerkIcon p in card.InjuryIcons)
            {
                p.gameObject.SetActive(false);
            }

            // show card
            card.gameObject.SetActive(true);

            // ucm 
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(card.Ucm, character.modelParts);

            // text fields
            card.NameText.text = character.myName;
            card.XpText.text = data.xpGained.ToString();
            card.HealthLostText.text = data.healthLost.ToString();
            card.StressGainedText.text = data.stressGained.ToString();
            card.KillsText.text = data.totalKills.ToString();

            // level up indicator
            if (data.didLevelUp) card.LevelUpParent.SetActive(true);
            else card.LevelUpParent.SetActive(false);            

            // build injury icons
            for(int i = 0; i < data.injuriesGained.Count; i++)
            {
                if (i == 4) break;

                PerkIconData perkData = PerkController.Instance.GetPerkIconDataByTag(data.injuriesGained[i]);
                card.InjuryIcons[i].PerkImage.sprite = perkData.passiveSprite;
                card.InjuryIcons[i].SetMyDataReference(perkData);
                card.InjuryIcons[i].gameObject.SetActive(true);
            }

        }
        #endregion

        // Input + Buttons Logic
        #region
        public void OnStatsButtonClicked()
        {
            if(currentWindowViewing != WindowState.CharactersPage)
            {
                currentWindowViewing = WindowState.CharactersPage;
                characterStatPageVisualParent.SetActive(true);
                lootPageVisualParent.SetActive(false);
            }
        }
        public void OnLootButtonClicked()
        {
            if (currentWindowViewing != WindowState.LootPage)
            {
                currentWindowViewing = WindowState.LootPage;
                characterStatPageVisualParent.SetActive(false);
                lootPageVisualParent.SetActive(true);
            }
        }
        public void OnContinueButtonClicked()
        {
            if (currentWindowViewing == WindowState.CharactersPage)
                OnLootButtonClicked();
            else
            {
                // show map + unlock
                MapPlayerTracker.Instance.UnlockMap();
                MapView.Instance.OnWorldMapButtonClicked();
            }
        }
     
        #endregion

    }

    public class CharacterCombatStatData
    {
        public HexCharacterModel hexCharacter;
        public HexCharacterData characterData; 
        public int xpGained;
        public bool didLevelUp;
        public int totalKills;
        public int healthLost;
        public int stressGained;
        public List<Perk> injuriesGained = new List<Perk>();
    }
   
}