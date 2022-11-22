using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using HexGameEngine.Characters;
using UnityEngine.UI;
using HexGameEngine.Perks;

namespace HexGameEngine.UI
{
    public class PerkTalentLevelUpPage : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Core Components")]       
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private PerkTalentLevelUpCard[] allLevelUpCards;

        [Header("Confirm Button Components")]
        [SerializeField] private Image confirmButtonImage;
        [SerializeField] private Sprite readyImage;
        [SerializeField] private Sprite notReadyImage;

        // Non inspector fields
        private HexCharacterData currentCharacter;
        #endregion

        // Getters + Accessors
        #region
        public PerkTalentLevelUpCard[] AllLevelUpCards
        {
            get { return allLevelUpCards; }
        }
        #endregion

        // Input
        #region
        public void OnConfirmButtonClicked()
        {
            if (!HasMadeSelection()) return;
            HandleSelectionConfirmed();

        }
        public void OnCancelButtonClicked()
        {
            currentCharacter = null;
            mainVisualParent.SetActive(false);
        }
        #endregion

        // Logic
        #region       
        public void ShowAndBuildForTalentReward(HexCharacterData character)
        {
            currentCharacter = character;
            mainVisualParent.SetActive(true);
            confirmButtonImage.sprite = notReadyImage;
            headerText.text = "Choose a new talent!";

            for (int i = 0; i < 3; i++)
            {
                allLevelUpCards[i].Reset();
                allLevelUpCards[i].BuildFromTalentData(character.talentRolls[0].talentChoices[i]);
            }

            UpdateConfirmButtonState();

        }
        private bool HasMadeSelection()
        {
            bool ret = false;
            foreach(PerkTalentLevelUpCard card in allLevelUpCards)
            {
                if (card.Selected)
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }
        private PerkTalentLevelUpCard GetSelectedCard()
        {
            PerkTalentLevelUpCard ret = null;
            foreach (PerkTalentLevelUpCard card in allLevelUpCards)
            {
                if (card.Selected)
                {
                    ret = card;
                    break;
                }
            }
            return ret;
        }
        private void HandleSelectionConfirmed()
        {
            PerkTalentLevelUpCard card = GetSelectedCard();

            // Perk
            if(card.PerkData != null)
            {
                PerkController.Instance.ModifyPerkOnCharacterData(currentCharacter.passiveManager, card.PerkData.perkTag, 1);
                currentCharacter.perkPoints -= 1;                
            }

            // Talent
            else if (card.TalentData != null)
            {
                CharacterDataController.Instance.HandleLearnNewTalent(currentCharacter, card.TalentData.talentSchool);
                currentCharacter.talentRolls.RemoveAt(0);
            }

            mainVisualParent.SetActive(false);
            // Rebuild character roster views
            CharacterRosterViewController.Instance.HandleRedrawRosterOnCharacterUpdated();
            CharacterScrollPanelController.Instance.RebuildViews();
        }
        public void UpdateConfirmButtonState()
        {
            if (HasMadeSelection())           
                confirmButtonImage.sprite = readyImage;            
            else confirmButtonImage.sprite = notReadyImage;
        }
        #endregion

    }
}