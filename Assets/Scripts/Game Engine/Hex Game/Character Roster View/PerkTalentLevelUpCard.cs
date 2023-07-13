using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WeAreGladiators.Characters;
using WeAreGladiators.Libraries;
using WeAreGladiators.Perks;
using DG.Tweening;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class PerkTalentLevelUpCard : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] private GameObject scalingParent;
        [SerializeField] private PerkTalentLevelUpPage myPage;
        [Header("Card Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image artImage;

        [SerializeField] float startScale = 1f, endScale = 1.1f;

        // non inspector fields
        private PerkIconData perkData;
        private TalentPairing talentData;
        private bool selected = false;
        #endregion

        // Getters + Accessors
        #region
        public bool Selected
        {
            get { return selected; }
        }
        public PerkIconData PerkData
        {
            get { return perkData; }
        }
        public TalentPairing TalentData
        {
            get { return talentData; }
        }
        #endregion

        // Input 
        #region
        public void Click()
        {
            if (selected) HandleDeselect();
            else if (!selected) HandleSelect();           
        }
       
        #endregion

        // Logic
        #region
        private void HandleDeselect()
        {
            selected = false;
            scalingParent.transform.DOKill();
            scalingParent.transform.DOScale(startScale, 0.25f);
            myPage.UpdateConfirmButtonState();
        }
        private void HandleSelect()
        {
            selected = true;
            scalingParent.transform.DOKill();
            scalingParent.transform.DOScale(endScale, 0.25f);            

            // If another card is already selected, deselect it
            foreach (PerkTalentLevelUpCard card in myPage.AllLevelUpCards)
            {
                if (card.Selected && card != this)
                {
                    card.selected = false;
                    card.scalingParent.transform.DOKill();
                    card.scalingParent.transform.DOScale(startScale, 0.25f);
                    break;
                }
            }

            myPage.UpdateConfirmButtonState();
        }
        public void Reset()
        {
            gameObject.SetActive(false);
            scalingParent.transform.DOKill();
            scalingParent.transform.DOScale(startScale, 0f);
            perkData = null;
            talentData = null;
            HandleDeselect();
        }
        public void BuildFromPerkData(PerkIconData data)
        {
            gameObject.SetActive(true);
            perkData = data;
            nameText.text = data.passiveName;
            descriptionText.text = TextLogic.ConvertCustomStringListToString(data.passiveDescription);
            Sprite s = data.passiveSprite;
            if (s == null) s = PerkController.Instance.GetPassiveSpriteByName(data.passiveName);
            artImage.sprite = s;


        }
        public void BuildFromTalentData(TalentPairing tp)
        {
            gameObject.SetActive(true);
            talentData = tp;
            nameText.text = tp.talentSchool.ToString(); //+ " " + tp.level.ToString();
            TalentDataSO data = CharacterDataController.Instance.GetTalentDataFromTalentEnum(tp.talentSchool);
            descriptionText.text = TextLogic.ConvertCustomStringListToString(data.talentDescription);
            artImage.sprite = data.talentSprite;

        }
        #endregion
    }
}