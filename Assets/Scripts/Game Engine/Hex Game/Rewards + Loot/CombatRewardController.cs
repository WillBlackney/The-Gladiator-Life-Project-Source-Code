using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.Items;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Perks;
using WeAreGladiators.Persistency;
using WeAreGladiators.Player;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.UCM;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.RewardSystems
{
    public class CombatRewardController : Singleton<CombatRewardController>
    {
        #region Properties + Components

        [Header("Core Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private RectTransform contentRect;
        [SerializeField] private RectTransform contentOffScreenRect;
        [SerializeField] private RectTransform contentOnScreenRect;
        [SerializeField] private UnityEngine.UI.Image blackUnderlay;
        [SerializeField] private ItemGridScrollView lootGrid;
        [SerializeField] private ScrollRect characterStatsScrollView;
        [SerializeField] private Scrollbar characterStatsScrollBar;

        [Header("Page UI Components")]
        [SerializeField] private List<CharacterCombatStatCard> allCharacterStatCards;
        [SerializeField] private GameObject characterStatCardPrefab;
        [SerializeField] private Transform characterStatCardParent;

        #endregion

        #region Getters + Accesors

        public List<CharacterCombatStatData> CurrentStatResults { get; private set; } = new List<CharacterCombatStatData>();

        #endregion

        #region Items Loot Reward Logic

        public void HandleGainRewardsOfContract(CombatContractData contract)
        {
            RunController.Instance.ModifyPlayerGold(contract.combatRewardData.goldAmount);
            InventoryController.Instance.AddItemToInventory(contract.combatRewardData.item);
            InventoryController.Instance.AddItemToInventory(contract.combatRewardData.abilityAwarded);
        }

        #endregion 

        #region Movement + Anims

        private void MoveContentOnScreen()
        {
            blackUnderlay.DOKill();
            blackUnderlay.DOFade(0, 0);
            blackUnderlay.DOFade(0.5f, 0.5f);

            contentRect.DOKill();
            contentRect.position = contentOffScreenRect.position;
            contentRect.DOMove(contentOnScreenRect.position, 1.5f).SetEase(Ease.OutBack);
        }

        #endregion

        #region Persistency Controller

        public void SaveMyDataToSaveFile(SaveGameData saveData)
        {
            saveData.currentCombatStatResult = CurrentStatResults;
        }
        public void BuildMyDataFromSaveFile(SaveGameData saveData)
        {
            CurrentStatResults = saveData.currentCombatStatResult;
        }
        public void CacheStatResult(List<CharacterCombatStatData> result)
        {
            CurrentStatResults = result;
        }

        #endregion

        #region XP Reward Logic

        public List<CharacterCombatStatData> GenerateCombatStatResultsForCharacters(List<HexCharacterModel> characters, bool victory)
        {
            List<CharacterCombatStatData> dataRet = new List<CharacterCombatStatData>();

            // Calculate xp gain          
            int baseXp = RunController.Instance.CurrentCombatContractData.enemyEncounterData.baseXpReward / characters.Count;
            int killXpSlice = 0;
            EnemyEncounterData encounterData = RunController.Instance.CurrentCombatContractData.enemyEncounterData;
            int totalKillingBlowXp = 0;

            // Prevent divide by zero error
            if (encounterData.TotalEnemyXP != 0 &&
                encounterData.TotalEnemies != 0)
            {
                killXpSlice = encounterData.TotalEnemyXP / encounterData.TotalEnemies;
            }

            // Determine if any enemies were killed indirectly by player (from burning, poisoned, etc)
            // Split their xp value evenly over all player characters
            foreach (HexCharacterModel character in characters)
            {
                totalKillingBlowXp += character.charactersKilledThisTurn * killXpSlice;
            }

            // Is there experience from an enemy death that has gone unrewarded?
            if (totalKillingBlowXp < encounterData.TotalEnemyXP)
            {
                baseXp += (encounterData.TotalEnemyXP - totalKillingBlowXp) / characters.Count;
            }

            // Start xp reward process
            foreach (HexCharacterModel character in characters)
            {
                CharacterCombatStatData result = GenerateCharacterCombatStatResult(character);
                dataRet.Add(result);

                // Dead characters dont get XP
                if (character.characterData.currentHealth <= 0 || !victory)
                {
                    continue;
                }

                // Apply xp gain + level up
                int xpGained = baseXp + killXpSlice * character.totalKills;
                if (character.characterData.currentXP + xpGained * StatCalculator.GetCharacterXpGainRate(character.characterData) >= character.characterData.currentMaxXP)
                {
                    result.didLevelUp = true;
                }
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
            result.stressGained = character.moraleStatesLoweredThisCombat;
            result.armourLost = character.armourLostThisCombat;
            result.damageDealt = character.damageDealtThisCombat;
            result.injuriesGained.AddRange(character.injuriesGainedThisCombat);
            result.permanentInjuriesGained.AddRange(character.permanentInjuriesGainedThisCombat);
            if (character.characterData.currentHealth <= 0)
            {
                result.died = true;
            }

            return result;
        }

        #endregion

        #region Build + Show Screen views

        public void HidePostCombatRewardScreen()
        {
            mainVisualParent.SetActive(false);
        }
        public void BuildAndShowPostCombatScreen(List<CharacterCombatStatData> data, CombatContractData contractData, bool victory)
        {
            mainVisualParent.SetActive(true);
            BuildCharacterStatCardPage(data);
            lootGrid.ResetSlots();

            if (victory)
            {
                AudioManager.Instance.PlaySound(Sound.Music_Victory_Fanfare);
                headerText.text = "VICTORY!";
                lootGrid.BuildForLootRewards(contractData);
            }
            else
            {
                AudioManager.Instance.PlaySound(Sound.Music_Defeat_Fanfare);
                headerText.text = "DEFEAT!";
                lootGrid.ResetSlots();
            }

            MoveContentOnScreen();
            ResetScrollView();
        }

        private void BuildCharacterStatCardPage(List<CharacterCombatStatData> data)
        {
            // Reset views
            foreach (CharacterCombatStatCard c in allCharacterStatCards)
            {
                c.gameObject.SetActive(false);
            }

            // Build a card for each character
            for (int i = 0; i < data.Count; i++)
            {
                if (allCharacterStatCards.Count <= i)
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
            {
                p.HideAndReset();
            }
            card.DeathIndicatorParent.SetActive(false);
            card.Ucm.gameObject.SetActive(true);
            card.KnockDownIndicatorParent.SetActive(false);
            card.PortraitDeathIcon.SetActive(false);
            card.LevelUpParent.SetActive(false);

            // show card
            card.gameObject.SetActive(true);

            // ucm 
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(card.Ucm, character.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, card.Ucm);

            // Knock down views setup
            if (data.permanentInjuriesGained.Count > 0)
            {
                card.KnockDownIndicatorParent.SetActive(true);
            }

            // Death views setup
            else if (data.died)
            {
                card.Ucm.gameObject.SetActive(false);
                card.PortraitDeathIcon.SetActive(true);
                card.DeathIndicatorParent.SetActive(true);
            }

            // text fields
            card.NameText.text = TextLogic.ReturnColoredText(character.myName, TextLogic.orangeHeaderText) + " " + TextLogic.ReturnColoredText(character.mySubName, TextLogic.brownBodyText);
            card.CurrentLevelText.text = character.currentLevel.ToString();
            card.XpText.text = data.xpGained.ToString();
            card.HealthLostText.text = Mathf.Abs(data.healthLost).ToString();
            card.ArmourLostText.text = Mathf.Abs(data.armourLost).ToString();
            card.DamageDealtText.text = data.damageDealt.ToString();
            card.KillsText.text = data.totalKills.ToString();

            // level up indicator
            if (data.didLevelUp)
            {
                card.ShowLevelUpIndicator();
            }
            else
            {
                card.LevelUpParent.SetActive(false);
            }

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
        private void ResetScrollView()
        {
            characterStatsScrollView.verticalNormalizedPosition = 1;
            characterStatsScrollBar.value = 1;
        }

        #endregion

        #region Input + Buttons Logic



        public void OnContinueButtonClicked()
        {
            GameController.Instance.HandlePostCombatToTownTransistion();
        }

        #endregion
    }

    public class CharacterCombatStatData
    {
        public int armourLost;
        public HexCharacterData characterData;
        public int damageDealt;
        public bool didLevelUp;
        public bool died;
        public int healthLost;
        public HexCharacterModel hexCharacter;
        public List<Perk> injuriesGained = new List<Perk>();
        public List<Perk> permanentInjuriesGained = new List<Perk>();
        public int stressGained;
        public int totalKills;
        public int xpGained;
    }

}
