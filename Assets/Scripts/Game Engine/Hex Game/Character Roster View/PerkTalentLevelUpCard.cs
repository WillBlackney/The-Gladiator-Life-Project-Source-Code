using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Characters;
using WeAreGladiators.Perks;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class PerkTalentLevelUpCard : MonoBehaviour
    {

        // Input 
        #region

        public void Click()
        {
            if (Selected)
            {
                HandleDeselect();
            }
            else if (!Selected)
            {
                HandleSelect();
            }
        }

        #endregion
        // Properties + Components
        #region

        [Header("Core Components")]
        [SerializeField] private GameObject scalingParent;
        [SerializeField] private PerkTalentLevelUpPage myPage;
        [Header("Card Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image artImage;

        [SerializeField] private float startScale = 1f, endScale = 1.1f;

        // non inspector fields

        #endregion

        // Getters + Accessors
        #region

        public bool Selected { get; private set; }
        public PerkIconData PerkData { get; private set; }
        public TalentPairing TalentData { get; private set; }

        #endregion

        // Logic
        #region

        private void HandleDeselect()
        {
            Selected = false;
            scalingParent.transform.DOKill();
            scalingParent.transform.DOScale(startScale, 0.25f);
            myPage.UpdateConfirmButtonState();
        }
        private void HandleSelect()
        {
            Selected = true;
            scalingParent.transform.DOKill();
            scalingParent.transform.DOScale(endScale, 0.25f);

            // If another card is already selected, deselect it
            foreach (PerkTalentLevelUpCard card in myPage.AllLevelUpCards)
            {
                if (card.Selected && card != this)
                {
                    card.Selected = false;
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
            PerkData = null;
            TalentData = null;
            HandleDeselect();
        }
        public void BuildFromPerkData(PerkIconData data)
        {
            gameObject.SetActive(true);
            PerkData = data;
            nameText.text = data.passiveName;
            descriptionText.text = TextLogic.ConvertCustomStringListToString(data.passiveDescription);
            Sprite s = data.passiveSprite;
            if (s == null)
            {
                s = PerkController.Instance.GetPassiveSpriteByName(data.passiveName);
            }
            artImage.sprite = s;

        }
        public void BuildFromTalentData(TalentPairing tp)
        {
            gameObject.SetActive(true);
            TalentData = tp;
            nameText.text = tp.talentSchool.ToString(); //+ " " + tp.level.ToString();
            TalentDataSO data = CharacterDataController.Instance.GetTalentDataFromTalentEnum(tp.talentSchool);
            descriptionText.text = TextLogic.ConvertCustomStringListToString(data.talentDescription);
            artImage.sprite = data.talentSprite;

        }

        #endregion
    }
}
