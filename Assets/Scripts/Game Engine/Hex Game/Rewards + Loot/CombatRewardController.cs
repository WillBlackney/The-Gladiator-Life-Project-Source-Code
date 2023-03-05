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
using HexGameEngine.TownFeatures;
using HexGameEngine.Player;
using HexGameEngine.Items;
using TMPro;
using HexGameEngine.UI;
using UnityEngine.UI;
namespace HexGameEngine.RewardSystems
{
    public class CombatRewardController : Singleton<CombatRewardController>
    {
        // Properties + Components
        #region
        [Header("Stat Screen Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private GameObject characterStatPageVisualParent;
        [SerializeField] private GameObject lootPageVisualParent;
        [SerializeField] private List<CharacterCombatStatCard> allCharacterStatCards;
        [SerializeField] private GameObject characterStatCardPrefab;
        [SerializeField] private Transform characterStatCardParent;
        [SerializeField] private List<CombatLootIcon> lootIcons;

        [Header("Button Components")]
        [SerializeField] Sprite selectedButtonSprite;
        [SerializeField] Sprite unselectedButtonSprite;
        [SerializeField] Image statsHeaderButtonImage;
        [SerializeField] Image lootHeaderButtonImage;

        [Header("Game Over Screen Components")]
        [SerializeField] private GameObject gameOverVisualParent;

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
        public List<CharacterCombatStatData> GenerateCombatStatResultsForCharacters(List<HexCharacterModel> characters, bool victory)
        {
            List<CharacterCombatStatData> dataRet = new List<CharacterCombatStatData>();

            // Calculate xp gain          
            int baseXp = RunController.Instance.CurrentCombatContractData.enemyEncounterData.baseXpReward / characters.Count;
            int killXpSlice = 0;
            var encounterData = RunController.Instance.CurrentCombatContractData.enemyEncounterData;
            int totalKillingBlowXp = 0;

            // Prevent divide by zero error
            if (encounterData.TotalEnemyXP != 0 &&
                encounterData.TotalEnemies != 0)
                killXpSlice = encounterData.TotalEnemyXP / encounterData.TotalEnemies;

            // Determine if any enemies were killed indirectly by player (from burning, poisoned, etc)
            // Split their xp value evenly over all player characters
            foreach (HexCharacterModel character in characters)
            {
                totalKillingBlowXp += character.charactersKilledThisTurn * killXpSlice;
            }

            // Is there experience from an enemy death that has gone unrewarded?
            if(totalKillingBlowXp < encounterData.TotalEnemyXP)
            {
                baseXp += (encounterData.TotalEnemyXP - totalKillingBlowXp) / characters.Count;
            }

            // Start xp reward process
            foreach (HexCharacterModel character in characters)
            {
                CharacterCombatStatData result = GenerateCharacterCombatStatResult(character);
                dataRet.Add(result);

                // Dead characters dont get XP
                if (character.characterData.currentHealth <= 0 || !victory) continue;

                // Apply xp gain + level up
                int xpGained = baseXp + (killXpSlice * character.totalKills);
                if (character.characterData.currentXP + xpGained >= character.characterData.currentMaxXP)                
                    result.didLevelUp = true;                
                result.xpGained = xpGained;
            }

            return dataRet;
        }
        public void ApplyXpGainFromStatResultsToCharacters(List<CharacterCombatStatData> results)
        {
            foreach (CharacterCombatStatData result in results)
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
            result.permanentInjuriesGained.AddRange(character.permanentInjuriesGainedThisCombat);
            if (character.characterData.currentHealth <= 0) result.died = true;

            return result;
        }
        #endregion

        // Items Loot Reward Logic
        #region
        public void HandleGainRewardsOfContract(CombatContractData contract)
        {
            PlayerDataController.Instance.ModifyPlayerGold(contract.combatRewardData.goldAmount);
            InventoryController.Instance.AddItemToInventory(InventoryController.Instance.CreateInventoryItemFromItemData(contract.combatRewardData.item));
            InventoryController.Instance.AddItemToInventory(InventoryController.Instance.CreateInventoryItemAbilityData(contract.combatRewardData.abilityAwarded));
        }
        #endregion

        // Build + Show Screen views
        #region
        public void HidePostCombatRewardScreen()
        {
            mainVisualParent.SetActive(false);
        }
        public void BuildAndShowPostCombatScreen(List<CharacterCombatStatData> data, CombatContractData contractData, bool victory)
        {
            mainVisualParent.SetActive(true);            
            characterStatPageVisualParent.SetActive(true);
            currentWindowViewing = WindowState.CharactersPage;
            lootPageVisualParent.SetActive(false);
            statsHeaderButtonImage.sprite = selectedButtonSprite;
            lootHeaderButtonImage.sprite = unselectedButtonSprite;
            BuildCharacterStatCardPage(data);
            if (victory)
            {
                headerText.text = "VICTORY!";
                BuildLootPageFromContractLootData(contractData);
            }
            else
            {
                // Hide Loot Icons
                for (int i = 0; i < lootIcons.Count; i++)
                    lootIcons[i].Reset();

                headerText.text = "DEFEAT!";
            }

        }
        private void BuildCharacterStatCardPage(List<CharacterCombatStatData> data)
        {
            // Reset views
            foreach (CharacterCombatStatCard c in allCharacterStatCards)
                c.gameObject.SetActive(false);

            // Build a card for each character
            for (int i = 0; i < data.Count; i++)
            {
                if(allCharacterStatCards.Count <= i)
                {
                    CharacterCombatStatCard newCard = Instantiate(characterStatCardPrefab, characterStatCardParent).GetComponent<CharacterCombatStatCard>();
                    allCharacterStatCards.Add(newCard);
                }
                BuildStatCardFromStatData(allCharacterStatCards[i], data[i]);
            }
        }
        private void BuildStatCardFromStatData(CharacterCombatStatCard card, CharacterCombatStatData data)
        {
            HexCharacterData character = data.characterData;

            // Reset
            foreach (UIPerkIcon p in card.InjuryIcons)
                p.HideAndReset();
            card.DeathIndicatorParent.SetActive(false);
            card.KnockDownIndicatorParent.SetActive(false);
            card.PortraitDeathIcon.SetActive(false);

            // show card
            card.gameObject.SetActive(true);

            // ucm 
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(card.Ucm, character.modelParts);            

            // Knock down views setup
            if (data.permanentInjuriesGained.Count > 0)            
                card.KnockDownIndicatorParent.SetActive(true);

            // Death views setup
            else if (data.died)
            {
                card.PortraitDeathIcon.SetActive(true);
                card.DeathIndicatorParent.SetActive(true);
            }

            // text fields
            card.NameText.text = character.myName; 
            card.SubNameText.text = character.myClassName;
            card.CurrentLevelText.text = character.currentLevel.ToString();
            card.XpText.text = data.xpGained.ToString();
            card.HealthLostText.text = Mathf.Abs(data.healthLost).ToString();
            card.StressGainedText.text = data.stressGained.ToString();
            card.KillsText.text = data.totalKills.ToString();

            // level up indicator
            if (data.didLevelUp) card.LevelUpParent.SetActive(true);
            else card.LevelUpParent.SetActive(false);

            // build injury icons
            List<Perk> injuriesShown = new List<Perk>();
            injuriesShown.AddRange(data.injuriesGained);
            injuriesShown.AddRange(data.permanentInjuriesGained);
            for (int i = 0; i < injuriesShown.Count && i < 4; i++)
            {
                ActivePerk ap = PerkController.Instance.GetActivePerkOnCharacter(character.passiveManager, injuriesShown[i]);
                card.InjuryIcons[i].BuildFromActivePerk(ap);
            }

        }
        private void BuildLootPageFromContractLootData(CombatContractData contract)
        {
            for(int i = 0; i < lootIcons.Count;i++)            
                lootIcons[i].Reset();

            int iconIndex = 0;
            if(contract.combatRewardData.item != null)
            {
                lootIcons[iconIndex].BuildAsItemReward(contract.combatRewardData.item);
                iconIndex++;
            }
            if (contract.combatRewardData.abilityAwarded != null)
            {
                lootIcons[iconIndex].BuildAsAbilityReward(contract.combatRewardData.abilityAwarded);
                iconIndex++;
            }
            if (contract.combatRewardData.goldAmount > 0)
            {
                lootIcons[iconIndex].BuildAsGoldReward(contract.combatRewardData.goldAmount);
                iconIndex++;
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

                // set button states
                statsHeaderButtonImage.sprite = selectedButtonSprite;
                lootHeaderButtonImage.sprite = unselectedButtonSprite;
            }
        }
        public void OnLootButtonClicked()
        {
            if (currentWindowViewing != WindowState.LootPage)
            {
                currentWindowViewing = WindowState.LootPage;
                characterStatPageVisualParent.SetActive(false);
                lootPageVisualParent.SetActive(true);

                // set button states
                statsHeaderButtonImage.sprite = unselectedButtonSprite;
                lootHeaderButtonImage.sprite = selectedButtonSprite;
            }
        }
        public void OnContinueButtonClicked()
        {
            if (currentWindowViewing == WindowState.CharactersPage) OnLootButtonClicked();
            else GameController.Instance.HandlePostCombatToTownTransistion();            
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
        public List<Perk> permanentInjuriesGained = new List<Perk>();
        public bool died = false;
    }
   
}