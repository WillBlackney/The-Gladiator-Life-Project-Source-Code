﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HexGameEngine.Characters;
using HexGameEngine.Utilities;
using HexGameEngine.Cards;
using HexGameEngine.Perks;
using HexGameEngine.Items;

namespace HexGameEngine.UI
{
    public class UIController : Singleton<UIController>
    {
        // Properties + Components
        #region
        [SerializeField] private CardViewModel perkTalentInfoCard;
        [SerializeField] private GameObject perkTalentInfoCardMovementParent;
        [SerializeField] private GameObject perkTalentInfoCardVisualParent;
        [SerializeField] private Transform perkTalentCardRosterPosition;
        [SerializeField] private Transform perkTalentCardDraftCharacterPosition;
        private ShowCharacterWorldUiState characterWorldUiState = ShowCharacterWorldUiState.Always;
        #endregion

        // Getters + Accesors
        #region
        public ShowCharacterWorldUiState CharacterWorldUiState
        {
            get { return characterWorldUiState; }
        }
        #endregion

        // Input + Key Presses
        #region
        public void OnAltKeyPressed()
        {
            if(characterWorldUiState == ShowCharacterWorldUiState.Always)
            {
                characterWorldUiState = ShowCharacterWorldUiState.OnMouseOver;

                // hide all character world UI's
                foreach (HexCharacterModel c in HexCharacterController.Instance.AllCharacters)
                {
                    HexCharacterController.Instance.FadeOutCharacterWorldCanvas(c.hexCharacterView, null, 0.25f, 0.001f);
                }
            }
            else if(characterWorldUiState == ShowCharacterWorldUiState.OnMouseOver)
            {
                characterWorldUiState = ShowCharacterWorldUiState.Always;

                // show all character UI's
                foreach(HexCharacterModel c in HexCharacterController.Instance.AllCharacters)
                {
                    HexCharacterController.Instance.FadeInCharacterWorldCanvas(c.hexCharacterView, null, 0.25f);
                }
            }
        }
        #endregion

        // Perk + Talent Info Card Logic
        #region
      
        private void ShowPerkTalentInfoCard(PopupPositon position)
        {
            if (position == PopupPositon.CharacterRoster) perkTalentInfoCardMovementParent.transform.position = perkTalentCardRosterPosition.position;
            else if (position == PopupPositon.DraftCharacterSheet) perkTalentInfoCardMovementParent.transform.position = perkTalentCardDraftCharacterPosition.position;
            else return;
            perkTalentInfoCard.gameObject.SetActive(true);
            perkTalentInfoCardVisualParent.SetActive(true);
            perkTalentInfoCard.DOKill();
            perkTalentInfoCard.cg.alpha = 0;
            perkTalentInfoCard.cg.DOFade(1f, 0.25f);
        }
        public void HidePerkTalentInfoCard()
        {
            perkTalentInfoCardVisualParent.SetActive(false);
            perkTalentInfoCard.gameObject.SetActive(false);
            perkTalentInfoCard.DOKill();
            perkTalentInfoCard.cg.alpha = 0;
        }
        #endregion

        // Perk + Talent Button Logic
        #region        
        public void OnTalentButtonMouseOver(UITalentIcon b)
        {
            //CardController.Instance.BuildCardViewModelFromTalentData(b.MyTalentData.talentSchool, perkTalentInfoCard);
           // KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(b.MyTalentData.keyWords);
           // ShowPerkTalentInfoCard(b.PopupPositon);
        }
        public void OnTalentButtonMouseExit()
        {
           // KeyWordLayoutController.Instance.FadeOutMainView();
            //HidePerkTalentInfoCard();
        }
        #endregion
    }
    public enum ShowCharacterWorldUiState
    {
        OnMouseOver = 0,
        Always = 1,
    }
}