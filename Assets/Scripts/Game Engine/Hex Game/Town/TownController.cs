using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Persistency;
using HexGameEngine.Characters;
using Sirenix.OdinInspector;
using HexGameEngine.UI;
using TMPro;
using UnityEngine.UI;
using CardGameEngine.UCM;
using HexGameEngine.UCM;
using System;
using HexGameEngine.Abilities;
using HexGameEngine.Libraries;
using HexGameEngine.Player;
using HexGameEngine.JourneyLogic;

namespace HexGameEngine.TownFeatures
{

    public class TownController : Singleton<TownController>
    {
        // Properties + Components
        #region
        [Title("Town Page Components")]
        [SerializeField] GameObject mainVisualParent;
        [Space(20)]

        [Title("Recruit Page Core Components")]
        [SerializeField] GameObject recruitPageVisualParent;
        [SerializeField] RecruitableCharacterTab[] allRecruitTabs;
        [Space(20)]

        [Header("Recruit Page Right Panel Components")]
        [SerializeField] private GameObject[] recruitRightPanelRows;
        [SerializeField] private TextMeshProUGUI recruitRightPanelNameText;
        [SerializeField] private UniversalCharacterModel recruitRightPanelPortaitModel;
        [Space(20)]
        [SerializeField] private TextMeshProUGUI recruitRightPanelRacialText;
        [SerializeField] private Image recruitRightPanelRacialImage;
        [SerializeField] private TextMeshProUGUI recruitRightPanelCostText;
        [SerializeField] private TextMeshProUGUI recruitRightPanelUpkeepText;
        [Space(20)]
        [SerializeField] private UIPerkIcon[] recruitPerkIcons;
        [SerializeField] private UITalentIcon[] recruitTalentIcons;
        [SerializeField] private UIAbilityIcon[] recruitAbilityIcons;
        [Space(20)]
        [SerializeField] private TextMeshProUGUI recruitRightPanelStrengthText;
        [SerializeField] private TextMeshProUGUI recruitRightPanelIntelligenceText;
        [SerializeField] private TextMeshProUGUI recruitRightPanelAccuracyText;
        [SerializeField] private TextMeshProUGUI recruitRightPanelDodgeText;
        [SerializeField] private TextMeshProUGUI recruitRightPanelConstitutionText;
        [SerializeField] private TextMeshProUGUI recruitRightPanelResolveText;
        [SerializeField] private TextMeshProUGUI recruitRightPanelWitsText;

        [Title("Choose Combat Page Components")]
        [SerializeField] private GameObject chooseCombatPageMainVisualParent;
        [SerializeField] private CombatContractCard[] allContractCards;
        [SerializeField] private Image goToDeploymentButton;
        [SerializeField] private Sprite readyButtonSprite;
        [SerializeField] private Sprite notReadyButtonSprite;

        [Title("Deployment Page Components")]
        [SerializeField] private GameObject deploymentPageMainVisualParent;
        [SerializeField] private TextMeshProUGUI charactersDeployedText;
        [SerializeField] private DeploymentNodeView[] allDeploymentNodes;
        [SerializeField] private GameObject deploymentWarningPopupVisualParent; 
        [SerializeField] private TextMeshProUGUI deploymentWarningPopupText;
        [SerializeField] private GameObject deploymentWarningPopupContinueButton;

        // Non-inspector properties
        private List<HexCharacterData> currentRecruits = new List<HexCharacterData>();
        private RecruitableCharacterTab selectedRecruitTab;
        private List<CombatContractData> currentDailyCombatContracts = new List<CombatContractData>();
        #endregion

        // Getters + Accessors
        #region
       
        #endregion

        // Save + Load Logic
        #region
        public void BuildMyDataFromSaveFile(SaveGameData saveFile)
        {
            currentRecruits.Clear();
            foreach (HexCharacterData c in saveFile.townRecruits)
                currentRecruits.Add(c);

            currentDailyCombatContracts.Clear();
            currentDailyCombatContracts.AddRange(saveFile.currentDailyCombatContracts);
        }
        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            saveFile.townRecruits.Clear();
            foreach (HexCharacterData c in currentRecruits)
                saveFile.townRecruits.Add(c);

            saveFile.currentDailyCombatContracts.Clear();
            saveFile.currentDailyCombatContracts.AddRange(currentDailyCombatContracts);
        }
        #endregion

        // Show + Hide Main View Logic
        #region
        public void ShowTownView()
        {
            mainVisualParent.SetActive(true);
        }
        public void HideTownView()
        {
            mainVisualParent.SetActive(false);
        }
        #endregion

        // Recruit Characters Page Logic
        #region
        public void GenerateDailyRecruits(int amount)
        {
            currentRecruits.Clear();
            for (int i = 0; i < amount; i++)
                HandleAddNewRecruitFromCharacterDeck();
        }
        private void HandleAddNewRecruitFromCharacterDeck()
        {
            if (CharacterDataController.Instance.CharacterDeck.Count == 0)
                CharacterDataController.Instance.AutoGenerateAndCacheNewCharacterDeck();
            currentRecruits.Add(CharacterDataController.Instance.CharacterDeck[0]);
            CharacterDataController.Instance.CharacterDeck.RemoveAt(0);

        }
        public void BuildAndShowRecruitPage()
        {
            recruitPageVisualParent.SetActive(true);

            // Reset recruit tabs
            foreach (RecruitableCharacterTab tab in allRecruitTabs)
                tab.ResetAndHide();

            // Build a tab for each recruit
            for (int i = 0; i < currentRecruits.Count && i < allRecruitTabs.Length; i++)
                allRecruitTabs[i].BuildFromCharacterData(currentRecruits[i]);

            // Build right panel
            if (currentRecruits.Count == 0) HideAllRightPanelRows();
            else
            {
                OnCharacterRecruitTabClicked(allRecruitTabs[0]);
            }

        }
        private void ShowAllRightPanelRows()
        {
            foreach (GameObject g in recruitRightPanelRows)
                g.SetActive(true);
        }
        private void HideAllRightPanelRows()
        {
            foreach (GameObject g in recruitRightPanelRows)
                g.SetActive(false);
        }
        public void OnCharacterRecruitTabClicked(RecruitableCharacterTab tab)
        {
            if (tab.MyCharacterData == null) return;
            if (selectedRecruitTab != null) selectedRecruitTab.SelectedParent.SetActive(false);
            selectedRecruitTab = tab;
            selectedRecruitTab.SelectedParent.SetActive(true);
            BuildRecruitPageRightPanel(tab.MyCharacterData);
            ShowAllRightPanelRows();
        }
        public void OnRecruitCharacterButtonClicked()
        {
            if (!selectedRecruitTab) return;

            // to do: check if player has enough gold and roster space before recruiting + pay gold for recruitment
            if (// enough space in roster &&
               PlayerDataController.Instance.CurrentGold < selectedRecruitTab.MyCharacterData.recruitCost) return;

            // Pay for recruit
            PlayerDataController.Instance.ModifyPlayerGold(-selectedRecruitTab.MyCharacterData.recruitCost);

            // Add character to roster
            CharacterDataController.Instance.AddCharacterToRoster(selectedRecruitTab.MyCharacterData);

            // Remove from current recruit pool
            currentRecruits.Remove(selectedRecruitTab.MyCharacterData);

            // Redraw windows
            BuildAndShowRecruitPage();

            // Rebuild character scroll roster
            CharacterScrollPanelController.Instance.BuildViews();
        }
        private void BuildRecruitPageRightPanel(HexCharacterData character)
        {
            // Reset
            Array.ForEach(recruitPerkIcons, x => x.HideAndReset());
            Array.ForEach(recruitTalentIcons, x => x.HideAndReset());
            Array.ForEach(recruitAbilityIcons, x => x.HideAndReset());

            // Character Model
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(recruitRightPanelPortaitModel, character.modelParts);
            recruitRightPanelPortaitModel.SetIdleAnim();

            // Texts
            recruitRightPanelNameText.text = character.myName;
            recruitRightPanelRacialText.text = character.race.ToString();
            string col = "<color=#FFFFFF>";
            if (PlayerDataController.Instance.CurrentGold < character.recruitCost) col = TextLogic.lightRed;
            recruitRightPanelUpkeepText.text = character.dailyWage.ToString();
            recruitRightPanelCostText.text = TextLogic.ReturnColoredText(character.recruitCost.ToString(), col);

            // Misc
            recruitRightPanelRacialImage.sprite = SpriteLibrary.Instance.GetRacialSpriteFromEnum(character.race);

            // Build stats section
            recruitRightPanelStrengthText.text = character.attributeSheet.strength.ToString();
            recruitRightPanelIntelligenceText.text = character.attributeSheet.intelligence.ToString();
            recruitRightPanelConstitutionText.text = character.attributeSheet.constitution.ToString();
            recruitRightPanelAccuracyText.text = character.attributeSheet.accuracy.ToString();
            recruitRightPanelDodgeText.text = character.attributeSheet.dodge.ToString();
            recruitRightPanelResolveText.text = character.attributeSheet.resolve.ToString();
            recruitRightPanelWitsText.text = character.attributeSheet.wits.ToString();

            // Build perk buttons
            for (int i = 0; i < character.passiveManager.perks.Count; i++)
                recruitPerkIcons[i].BuildFromPerkData(character.passiveManager.perks[i]);

            // Build talent buttons
            for (int i = 0; i < character.talentPairings.Count; i++)
                recruitTalentIcons[i].BuildFromTalentPairing(character.talentPairings[i]);

            // Build abilities section
            // Main hand weapon abilities
            int newIndexCount = 0;
            for (int i = 0; i < character.itemSet.mainHandItem.grantedAbilities.Count; i++)
            {
                // Characters dont gain special weapon ability if they have an off hand item
                if (character.itemSet.offHandItem == null || (character.itemSet.offHandItem != null && character.itemSet.mainHandItem.grantedAbilities[i].weaponAbilityType == WeaponAbilityType.Basic))
                {
                    recruitAbilityIcons[i].BuildFromAbilityData(character.itemSet.mainHandItem.grantedAbilities[i]);
                    newIndexCount++;
                }
            }

            // Off hand weapon abilities
            if (character.itemSet.offHandItem != null)
            {
                for (int i = 0; i < character.itemSet.offHandItem.grantedAbilities.Count; i++)
                {
                    recruitAbilityIcons[i + newIndexCount].BuildFromAbilityData(character.itemSet.offHandItem.grantedAbilities[i]);
                    newIndexCount++;
                }
            }

            // Build non item derived abilities
            for (int i = 0; i < character.abilityBook.allKnownAbilities.Count; i++)
                recruitAbilityIcons[i + newIndexCount].BuildFromAbilityData(character.abilityBook.allKnownAbilities[i]);

        }
        #endregion

        // Feature Buttons On Click
        #region
        public void OnRecruitPageButtonClicked()
        {
            BuildAndShowRecruitPage();
        }
        public void OnRecruitPageLeaveButtonClicked()
        {
            recruitPageVisualParent.SetActive(false);
        }
        public void OnCombatPageBackToTownButtonClicked()
        {
            BlackScreenController.Instance.FadeOutScreen(0.5f, () =>
            {
                HideCombatContractPage();
                ShowTownView();
                BlackScreenController.Instance.FadeInScreen(0.5f);
            });
        }
        public void OnCombatPageDeploymentButtonClicked()
        {
            if (CombatContractCard.SelectectedCombatCard == null) return;
            BlackScreenController.Instance.FadeOutScreen(0.5f, () =>
            {
                HideCombatContractPage();
                BuildAndShowDeploymentPage();
                BlackScreenController.Instance.FadeInScreen(0.5f);
            });

        }
        public void OnDeploymentPageBackButtonClicked()
        {
            BlackScreenController.Instance.FadeOutScreen(0.5f, () =>
            {
                HideDeploymentPage();
                BuildAndShowCombatContractPage();
                BlackScreenController.Instance.FadeInScreen(0.5f);
            });
        }
        public void OnDeploymentPageReadyButtonClicked()
        {
            HandleReadyButtonClicked();
        }
        public void OnArenaPageButtonClicked()
        {
            BlackScreenController.Instance.FadeOutScreen(0.5f, () =>
            {
                HideTownView();
                BuildAndShowCombatContractPage();
                BlackScreenController.Instance.FadeInScreen(0.5f);
            });
        }

        #endregion

        // Choose Combat Contract Page Logic
        #region
        public CombatContractData GenerateSandboxContractData()
        {
            CombatContractData ret = new CombatContractData();
            ret.enemyEncounterData = RunController.Instance.GenerateEnemyEncounterFromTemplate(GlobalSettings.Instance.SandboxEnemyEncounters.GetRandomElement());
            ret.combatRewardData = new CombatRewardData(ret.enemyEncounterData.difficulty);
            return ret;
        }
        public void GenerateDailyCombatContracts()
        {
            currentDailyCombatContracts.Clear();

            // On normal days, generate 2 basics and 1 elite combat
            if (RunController.Instance.CurrentDay != 5)
            {
                for (int i = 0; i < 2; i++)
                {
                    currentDailyCombatContracts.Add(GenerateRandomDailyCombatContract(RunController.Instance.CurrentChapter, CombatDifficulty.Basic));
                }
                currentDailyCombatContracts.Add(GenerateRandomDailyCombatContract(RunController.Instance.CurrentChapter, CombatDifficulty.Elite));

            }

            // on 5th day, only offer the boss fight
            else
            {
                currentDailyCombatContracts.Add(GenerateRandomDailyCombatContract(RunController.Instance.CurrentChapter, CombatDifficulty.Boss));
            }
        }
        private CombatContractData GenerateRandomDailyCombatContract(int currentAct, CombatDifficulty difficulty)
        {
            CombatContractData ret = new CombatContractData();
            ret.enemyEncounterData = RunController.Instance.GenerateEnemyEncounterFromTemplate(RunController.Instance.GetRandomCombatData(currentAct, difficulty));
            ret.combatRewardData = new CombatRewardData(difficulty);
            return ret;
        }
        private void BuildAndShowCombatContractPage()
        {
            chooseCombatPageMainVisualParent.SetActive(true);
            for (int i = 0; i < currentDailyCombatContracts.Count && i < allContractCards.Length; i++)
            {
                allContractCards[i].BuildFromContractData(currentDailyCombatContracts[i]);
            }

            CombatContractCard.HandleDeselect();
        }
        private void HideCombatContractPage()
        {
            chooseCombatPageMainVisualParent.SetActive(false);
        }
        public void SetDeploymentButtonReadyState(bool onOrOff)
        {
            if (onOrOff) goToDeploymentButton.sprite = readyButtonSprite;
            else goToDeploymentButton.sprite = notReadyButtonSprite;
        }
        #endregion

        // Deployment Page Logic
        #region
        private void BuildAndShowDeploymentPage()
        {
            deploymentPageMainVisualParent.SetActive(true);

            // Reset deployment nodes
            for (int i = 0; i < allDeploymentNodes.Length; i++)
                allDeploymentNodes[i].SetUnoccupiedState();

            UpdateCharactersDeployedText();

            // build enemy deployment nodes
            BuildEnemyNodes(CombatContractCard.SelectectedCombatCard.MyContractData);
        }
        private void BuildEnemyNodes(CombatContractData combatData)
        {
            foreach(CharacterWithSpawnData eg in combatData.enemyEncounterData.enemiesInEncounter)
            {
                foreach(DeploymentNodeView node in allDeploymentNodes)
                {
                    if(node.GridPosition == eg.spawnPosition)
                    {
                        node.BuildFromCharacterData(eg.characterData);
                        break;
                    }
                }
            }
        }
        public void HideDeploymentPage()
        {
            deploymentPageMainVisualParent.SetActive(false);
        }
        public void UpdateCharactersDeployedText()
        {
            charactersDeployedText.text = "Characters Deployed: " + GetDeployedCharacters().Count.ToString() + 
                " / " + PlayerDataController.Instance.DeploymentLimit.ToString();
        }
        public void HandleDropCharacterOnDeploymentNode(DeploymentNodeView node, HexCharacterData draggedCharacter)
        {
            Debug.Log("HandleDropCharacterOnDeploymentNode");
            // Prevent drag drop new character when already deployed maximum characters
            if (node.IsNodeAvailable() && GetDeployedCharacters().Count >= PlayerDataController.Instance.DeploymentLimit) return;

            // Handle dropped on empty slot
            if (node.AllowedCharacter == Allegiance.Player)
            {
                node.BuildFromCharacterData(draggedCharacter);
                UpdateCharactersDeployedText();
            }
        }
        public List<CharacterWithSpawnData> GetDeployedCharacters()
        {
            List<CharacterWithSpawnData> ret = new List<CharacterWithSpawnData>();

            foreach(DeploymentNodeView n in allDeploymentNodes)
            {
                if (n.AllowedCharacter == Allegiance.Player &&
                    n.MyCharacterData != null)
                {
                    ret.Add(new CharacterWithSpawnData(n.MyCharacterData, n.GridPosition));
                }
                   
            }

            return ret;
        }
        public bool IsCharacterDraggableFromRosterToDeploymentNode(HexCharacterData character)
        {
            List<HexCharacterData> charactersDep = new List<HexCharacterData>();
            foreach(CharacterWithSpawnData c in GetDeployedCharacters())
            {
                charactersDep.Add(c.characterData);
            }

            if (deploymentPageMainVisualParent.activeSelf &&
                charactersDep.Contains(character))
                return false;
            else return true;
            
        }
        private void HandleReadyButtonClicked()
        {
            var characters = GetDeployedCharacters();
            // Validate
            if (characters.Count == 0)
            {
                Debug.Log("HandleReadyButtonClicked() cancelling: player has not deployed any characters");
                ShowNoCharactersDeployedPopup();
                return;
            }
            else if(characters.Count > 0 && characters.Count < PlayerDataController.Instance.DeploymentLimit)
            {
                ShowLessThanMaxCharactersDeployedPopup();
            }
            else
            {
                GameController.Instance.HandleLoadIntoCombatFromDeploymentScreen();
            }
        }
        private void ShowNoCharactersDeployedPopup()
        {
            deploymentWarningPopupVisualParent.SetActive(true);
            deploymentWarningPopupContinueButton.SetActive(false);
            deploymentWarningPopupText.text = "Cannot start combat as you have not deployed any characters!";
        }
        private void ShowLessThanMaxCharactersDeployedPopup()
        {
            deploymentWarningPopupVisualParent.SetActive(true);
            deploymentWarningPopupContinueButton.SetActive(true);
            deploymentWarningPopupText.text = "You have deployed less characters than your allowed limit of " + PlayerDataController.Instance.DeploymentLimit.ToString()
                + ". Are you sure want to start this combat?";
        }
        public void OnDeploymentPopupBackButtonClicked()
        {
            deploymentWarningPopupVisualParent.SetActive(false);
        }
        public void OnDeploymentPopupContinueButtonClicked()
        {
            deploymentWarningPopupVisualParent.SetActive(false);
            GameController.Instance.HandleLoadIntoCombatFromDeploymentScreen();
        }
        #endregion

    }
}