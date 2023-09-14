using DG.Tweening;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Characters;
using WeAreGladiators.Libraries;

namespace WeAreGladiators.UI
{
    public class AttributeLevelUpWidget : MonoBehaviour
    {

        // Input
        #region

        public void OnAttributeButtonClicked()
        {
            // Deselect
            if (Selected)
            {
                attributeAmountText.text = currentBaseStat.ToString();
                Selected = false;
                buttonImage.color = normalColor;
            }

            // Select
            else if (!Selected)
            {
                if (myPage.GetSelectedAttributes().Count >= 3)
                {
                    return;
                }
                Selected = true;
                buttonImage.color = selectedColor;
                attributeAmountText.text = (currentBaseStat + currentRoll).ToString();
            }
            UpdateSlider();
            myPage.UpdateTotalSelectedAttributes();
        }

        #endregion
        // Properties + Components
        #region

        [Header("Properties")]
        [SerializeField] private CoreAttribute myAttribute;

        [Header("Components")]
        [SerializeField] private AttributeLevelUpPage myPage;
        [SerializeField] private TextMeshProUGUI attributeAmountText;
        [SerializeField] private TextMeshProUGUI rollText;
        [SerializeField] private TextMeshProUGUI attributeNameText;
        [SerializeField] private Image attributeImage;
        [SerializeField] private Image buttonImage;
        [SerializeField] private Slider statSlider;
        [SerializeField] private GameObject[] stars;

        [Header("Colours")]
        [SerializeField] private Color normalColor;
        [SerializeField] private Color selectedColor;

        private int currentBaseStat;
        private int currentRoll;
        private int currentStars;

        #endregion

        // Getters + Accessors
        #region

        public bool Selected { get; private set; }
        public CoreAttribute MyAttribute => myAttribute;

        #endregion

        // Logic
        #region

        public void BuildViews(HexCharacterData character)
        {
            attributeNameText.text = myAttribute.ToString();
            attributeImage.sprite = SpriteLibrary.Instance.GetAttributeSprite(myAttribute);
            AttributeRollResult roll = character.attributeRolls[0];

            if (myAttribute == CoreAttribute.Accuracy)
            {
                currentBaseStat = character.attributeSheet.accuracy.value;
                currentStars = character.attributeSheet.accuracy.stars;
                currentRoll = roll.accuracyRoll;
            }
            else if (myAttribute == CoreAttribute.Dodge)
            {
                currentBaseStat = character.attributeSheet.dodge.value;
                currentStars = character.attributeSheet.dodge.stars;
                currentRoll = roll.dodgeRoll;
            }
            else if (myAttribute == CoreAttribute.Might)
            {
                currentBaseStat = character.attributeSheet.might.value;
                currentStars = character.attributeSheet.might.stars;
                currentRoll = roll.mightRoll;
            }
            else if (myAttribute == CoreAttribute.Resolve)
            {
                currentBaseStat = character.attributeSheet.resolve.value;
                currentStars = character.attributeSheet.resolve.stars;
                currentRoll = roll.resolveRoll;
            }
            else if (myAttribute == CoreAttribute.Constitution)
            {
                currentBaseStat = character.attributeSheet.constitution.value;
                currentStars = character.attributeSheet.constitution.stars;
                currentRoll = roll.constitutionRoll;
            }
            else if (myAttribute == CoreAttribute.Wits)
            {
                currentBaseStat = character.attributeSheet.wits.value;
                currentStars = character.attributeSheet.wits.stars;
                currentRoll = roll.witsRoll;
            }

            rollText.text = "+" + currentRoll;
            attributeAmountText.text = currentBaseStat.ToString();
            for (int i = 0; i < currentStars; i++)
            {
                stars[i].SetActive(true);
            }
            UpdateSlider(0);
        }
        private void UpdateSlider(float speed = 0.5f)
        {
            float current = currentBaseStat;
            if (Selected)
            {
                current = currentBaseStat + currentRoll;
            }
            float max = 100f;
            statSlider.DOKill();
            statSlider.DOValue(current / max, speed);
        }
        public void ResetViews()
        {
            currentStars = 0;
            currentBaseStat = 0;
            currentRoll = 0;
            Selected = false;
            buttonImage.color = normalColor;
            stars.ForEach(x => x.SetActive(false));
            UpdateSlider(0f);
        }

        #endregion
    }
}
