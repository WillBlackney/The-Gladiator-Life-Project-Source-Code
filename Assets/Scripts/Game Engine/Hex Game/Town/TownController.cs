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

namespace HexGameEngine.TownFeatures
{

    public class TownController : Singleton<TownController>
    {
        // Properties + Components
        #region
        [Title("Town Page Components")]
        [SerializeField] GameObject mainVisualParent;
        [Space(20)]

        [Title("Recruit Page Components")]
        [SerializeField] GameObject recruitPageVisualParent;
        [SerializeField] RecruitableCharacterTab[] allRecruitTabs;
        [Space(20)]

        [Header("Right Panel Components")]
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

        // Non-inspector properties
        private List<HexCharacterData> currentRecruits = new List<HexCharacterData>();
        private RecruitableCharacterTab selectedRecruitTab;
        #endregion

        // Save + Load Logic
        #region
        public void BuildMyDataFromSaveFile(SaveGameData saveFile)
        {
            currentRecruits.Clear();
            foreach (HexCharacterData c in saveFile.townRecruits)
                currentRecruits.Add(c);
        }
        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            saveFile.townRecruits.Clear();
            foreach (HexCharacterData c in currentRecruits)
                saveFile.townRecruits.Add(c);
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
                recruitTalentIcons[i].BuildFromTalentData(character.talentPairings[i]);            

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
        #endregion

    }
}