using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using DG.Tweening;
using HexGameEngine.Persistency;
using HexGameEngine.Characters;
using HexGameEngine.UCM;
using HexGameEngine.UI;
using System;
using TMPro;
using CardGameEngine.UCM;
using HexGameEngine.Abilities;

namespace HexGameEngine.TownFeatures
{
    public class OLDTownController : Singleton<OLDTownController>
    {
        // Properties
        #region
        [Header("Shared Screen Components")]
        [SerializeField] private GameObject masterVisualParent;
        [SerializeField] private CanvasGroup masterVisualParentCg;

        [Header("Front Screen Components")]
        [SerializeField] private GameObject frontScreenVisualParent;
        [SerializeField] private CanvasGroup frontScreenCg;

        [Header("Recruit Screen Components")]
        [SerializeField] private GameObject recruitScreenVisualParent;
        [SerializeField] private CanvasGroup recruitScreenCg;
        [SerializeField] private GameObject[] recruitScreenRightPanelRows;
        [SerializeField] private OLDCharacterRecruitTab[] allCharacterRecruitTabs;
        [SerializeField] private UIPerkIcon[] recruitPerkIcons;
        [SerializeField] private UITalentIcon[] recruitTalentIcons;
        [SerializeField] private UIAbilityIcon[] recruitAbilityIcons;
        [SerializeField] private TextMeshProUGUI recruitRightPanelNameText;
        [SerializeField] private TextMeshProUGUI recruitRightPanelClassNameText;
        [SerializeField] private UniversalCharacterModel recruitRightPanelUcm;
        private OLDCharacterRecruitTab selectedRecruitTab;

        [Header("Core Attribute Text Components")]
        [SerializeField] private TextMeshProUGUI strengthText;
        [SerializeField] private TextMeshProUGUI intelligenceText;
        [SerializeField] private TextMeshProUGUI constitutionText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private TextMeshProUGUI dodgeText;
        [SerializeField] private TextMeshProUGUI resolveText;


        private List<HexCharacterData> currentRecruits = new List<HexCharacterData>();

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
        }
        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            saveFile.townRecruits.Clear();
            foreach (HexCharacterData c in currentRecruits)
                saveFile.townRecruits.Add(c);
        }
        #endregion

        // Enter Town Logic
        #region
        public void HandleEnterTown()
        {
            // build town views
            // show town views
            // set gameState as inTown
            //GameController.Instance.SetGameState(GameState.InTown);
            RevealStartingTownView();
        }
        #endregion

        // Show + Hide Main View Logic
        #region
        private void RevealStartingTownView()
        {
            // Reset
            masterVisualParentCg.DOKill();
            masterVisualParentCg.alpha = 0;
            masterVisualParent.SetActive(true);
            frontScreenCg.DOKill();
            frontScreenCg.alpha = 1;
            frontScreenVisualParent.SetActive(true);

            // to do: close all other windows just incase (black smith, surgeon, etc)

            // Fade in
            masterVisualParentCg.DOFade(1, 0.5f);
        }
        #endregion

        // On Click Logic
        #region
        public void OnLeaveTownButtonClicked()
        {
            //GameController.Instance.SetGameState(GameState.WorldMap); 
            masterVisualParentCg.DOKill();

            Sequence s = DOTween.Sequence();
            s.Append(masterVisualParentCg.DOFade(0, 0.5f));
            s.OnComplete(() => masterVisualParent.SetActive(false));
        }
        public void OnMainPageRecruitButtonClicked()
        {
            recruitScreenCg.DOKill();
            frontScreenCg.DOKill();
            recruitScreenCg.alpha = 0;
            recruitScreenVisualParent.SetActive(true);

            BuildRecruitPage();
            recruitScreenCg.DOFade(1, 0.5f);
            Sequence s = DOTween.Sequence();
            s.Append(frontScreenCg.DOFade(0, 0.5f));
            s.OnComplete(() => frontScreenVisualParent.SetActive(false));
        }
        public void OnRecruitPageBackToTownButtonClicked()
        {
            recruitScreenCg.DOKill();
            frontScreenCg.DOKill();
            frontScreenVisualParent.SetActive(true);
            frontScreenCg.alpha = 0;

            frontScreenCg.DOFade(1, 0.5f);
            Sequence s = DOTween.Sequence();
            s.Append(recruitScreenCg.DOFade(0, 0.5f));
            s.OnComplete(() => recruitScreenVisualParent.SetActive(false));
        }
        #endregion

        // Recruit Characters Logic
        #region
        public void GenerateDailyRecruits(int amount)
        {
            for (int i = 0; i < amount; i++)
                HandleAddNewRecruitFromCharacterDeck();
        }
        public void HandleAddNewRecruitFromCharacterDeck()
        {
            if(CharacterDataController.Instance.CharacterDeck.Count == 0)            
                CharacterDataController.Instance.AutoGenerateAndCacheNewCharacterDeck();            
            currentRecruits.Add(CharacterDataController.Instance.CharacterDeck[0]);
            CharacterDataController.Instance.CharacterDeck.RemoveAt(0);

        }
        private void BuildRecruitPage()
        {
            // Reset recruit tabs
            foreach (OLDCharacterRecruitTab tab in allCharacterRecruitTabs)
                tab.gameObject.SetActive(false);

            // Build a tab for each recruit
            for(int i = 0; i < currentRecruits.Count; i++)            
                BuildRecruitTabFromCharacterData(allCharacterRecruitTabs[i], currentRecruits[i]);

            // Build right panel
            if (currentRecruits.Count == 0) HideAllRightPanelRows();
            else
            {               
                OnCharacterRecruitTabClicked(allCharacterRecruitTabs[0]);
            }

            
        }
        private void BuildRecruitTabFromCharacterData(OLDCharacterRecruitTab tab, HexCharacterData data)
        {
            tab.SetMyCharacter(data);
            tab.gameObject.SetActive(true);
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(tab.Ucm, data.modelParts);            
            tab.Ucm.SetBaseAnim();

            tab.NameText.text = data.myName + ": " + TextLogic.ReturnColoredText(data.race.ToString() + " " + data.myClassName, TextLogic.neutralYellow);
        }
        private void BuildRecruitPageRightPanel(HexCharacterData character)
        {
            // Reset
            Array.ForEach(recruitPerkIcons, x => x.gameObject.SetActive(false));
            Array.ForEach(recruitTalentIcons, x => x.gameObject.SetActive(false));
            Array.ForEach(recruitAbilityIcons, x => x.gameObject.SetActive(false));

            // Character Model
            CharacterModeller.BuildModelFromStringReferences(recruitRightPanelUcm, character.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, recruitRightPanelUcm);
            recruitRightPanelUcm.SetIdleAnim();

            // Texts
            recruitRightPanelNameText.text = character.myName;
            recruitRightPanelClassNameText.text = TextLogic.ReturnColoredText(character.race.ToString() + " " + character.myClassName, TextLogic.neutralYellow);

            // Build stats section
            strengthText.text = character.attributeSheet.strength.value.ToString();
            intelligenceText.text = character.attributeSheet.intelligence.value.ToString();
            constitutionText.text = character.attributeSheet.constitution.value.ToString();
            accuracyText.text = character.attributeSheet.accuracy.value.ToString();
            dodgeText.text = character.attributeSheet.dodge.value.ToString();
            resolveText.text = character.attributeSheet.resolve.value.ToString();

            // Build perk buttons
            for (int i = 0; i < character.passiveManager.perks.Count; i++)
            {
                UIController.Instance.BuildPerkButton(character.passiveManager.perks[i], recruitPerkIcons[i]);
            }

            // Build talent buttons
            for (int i = 0; i < character.talentPairings.Count; i++)
            {
                recruitTalentIcons[i].BuildFromTalentPairing(character.talentPairings[i]);
            }

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
            {
                recruitAbilityIcons[i + newIndexCount].BuildFromAbilityData(character.abilityBook.allKnownAbilities[i]);
            }   
           
        }
        private void ShowAllRightPanelRows()
        {
            foreach (GameObject g in recruitScreenRightPanelRows)
                g.SetActive(true);
        }
        private void HideAllRightPanelRows()
        {
            foreach (GameObject g in recruitScreenRightPanelRows)
                g.SetActive(false);
        }
        public void OnCharacterRecruitTabClicked(OLDCharacterRecruitTab tab)
        {
            if (tab.MyDataRef == null) return;
            if (selectedRecruitTab != null) selectedRecruitTab.SelectedParent.SetActive(false);
            selectedRecruitTab = tab;
            selectedRecruitTab.SelectedParent.SetActive(true);
            BuildRecruitPageRightPanel(tab.MyDataRef);
            ShowAllRightPanelRows();
        }
        public void OnConfirmRecruitCharacterButtonClicked()
        {
            if (!selectedRecruitTab) return;
            
            // to do: check if player has enough gold and roster space before recruiting

            // add character to roster
            CharacterDataController.Instance.AddCharacterToRoster(selectedRecruitTab.MyDataRef);

            // remove from current recruit pool
            currentRecruits.Remove(selectedRecruitTab.MyDataRef);

            // re draw windows
            BuildRecruitPage();
        }
        #endregion

        // Events
        #region
        public void OnNewDayCycleStart()
        {
            int newRecruits = RandomGenerator.NumberBetween(2, 4);
            for(int i = 0; i < newRecruits; i++)
                HandleAddNewRecruitFromCharacterDeck();

            // to do: remove recruits that have been in available for 3 or more days
        }
        #endregion

    }
}