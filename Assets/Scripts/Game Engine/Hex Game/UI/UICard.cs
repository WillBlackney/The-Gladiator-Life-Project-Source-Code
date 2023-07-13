using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators.UI
{
    public class UICard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image cardMainImage;

        public void BuildCard(string headerText, string descriptionText, Sprite cardSprite)
        {
            this.headerText.text = headerText;
            this.descriptionText.text = descriptionText;
            cardMainImage.sprite = cardSprite;
        }
    }
}