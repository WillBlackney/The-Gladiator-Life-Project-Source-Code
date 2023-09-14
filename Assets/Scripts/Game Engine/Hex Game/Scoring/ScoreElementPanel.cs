using TMPro;
using UnityEngine;
using WeAreGladiators.UI;

namespace WeAreGladiators.Scoring
{
    public class ScoreElementPanel : MonoBehaviour
    {
        // Properties + Components
        #region

        [SerializeField] private TextMeshProUGUI nameText, valueText;
        [SerializeField] private CanvasGroup myCg;
        [HideInInspector] public string Description;

        #endregion

        #region Getters + Accessors

        public bool IsActive { get; set; } = false;
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
