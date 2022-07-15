using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using HexGameEngine.Characters;
using HexGameEngine.Libraries;

namespace HexGameEngine.UI
{
    public class AttributeLevelUpWidget : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Properties")]
        [SerializeField] private CoreAttribute myAttribute;

        [Header("Components")]
        [SerializeField] private AttributeLevelUpPage myPage;
        [SerializeField] private TextMeshProUGUI rollText;
        [SerializeField] private TextMeshProUGUI attributeNameText;
        [SerializeField] private Image attributeImage;
        [SerializeField] private Image buttonImage;

        [Header("Colours")]
        [SerializeField] private Color normalColor;
        [SerializeField] private Color selectedColor;

        private bool selected = false;

        #endregion

        // Getters + Accessors
        #region
        public bool Selected
        {
            get { return selected; }
        }
        public CoreAttribute MyAttribute
        {
            get { return myAttribute; }
        }
        #endregion

        // Input
        #region
        public void OnAttributeButtonClicked()
        {
            // Deselect
            if (selected)
            {
                selected = false;
                buttonImage.color = normalColor;
            }

            // Select
            else if (!selected)
            {
                if (myPage.GetSelectedAttributes().Count >= 3) return;
                selected = true;
                buttonImage.color = selectedColor;
                
            }

            myPage.UpdateTotalSelectedAttributes();
        }
        #endregion

        // Logic
        #region
        public void BuildViews(HexCharacterData character)
        {
            attributeNameText.text = myAttribute.ToString();
            attributeImage.sprite = SpriteLibrary.Instance.GetAttributeSprite(myAttribute);
            AttributeRollResult roll = character.attributeRolls[0];

            if (myAttribute == CoreAttribute.Accuracy) rollText.text = "+" + roll.accuracyRoll.ToString();
            else if (myAttribute == CoreAttribute.Dodge) rollText.text = "+" + roll.dodgeRoll.ToString();
            else if (myAttribute == CoreAttribute.Might) rollText.text = "+" + roll.mightRoll.ToString();
            else if (myAttribute == CoreAttribute.Resolve) rollText.text = "+" + roll.resolveRoll.ToString();
            else if (myAttribute == CoreAttribute.Constituition) rollText.text = "+" + roll.constitutionRoll.ToString();
            else if (myAttribute == CoreAttribute.Wits) rollText.text = "+" + roll.witsRoll.ToString();

        }
        public void Reset()
        {
            selected = false;
            buttonImage.color = normalColor;
        }
        #endregion
    }
}
