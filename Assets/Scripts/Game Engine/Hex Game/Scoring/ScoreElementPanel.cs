using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using WeAreGladiators.UI;

namespace WeAreGladiators.Scoring
{
    public class ScoreElementPanel : MonoBehaviour
    {
        // Properties + Components
        #region
        [SerializeField] TextMeshProUGUI nameText, valueText;
        [SerializeField] CanvasGroup myCg;
        [HideInInspector] public string Description;
        #endregion

        #region Getters + Accessors
        public bool IsActive { get; set;} = false;
        public TextMeshProUGUI NameText => nameText;
        public TextMeshProUGUI ValueText => valueText;
        public CanvasGroup MyCg => myCg;
        #endregion

        #region UI Events
        public void ShowInfoPanel()
        {
            MainModalController.Instance?.BuildAndShowModal(NameText.text, Description);
        }
        public void HideInfoPanel()
        {
            MainModalController.Instance?.HideModal();
        }
        #endregion
    }
}
