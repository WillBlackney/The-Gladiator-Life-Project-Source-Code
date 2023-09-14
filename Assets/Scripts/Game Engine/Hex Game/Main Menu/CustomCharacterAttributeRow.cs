using TMPro;
using UnityEngine;
using WeAreGladiators.MainMenu;

namespace WeAreGladiators.UI
{
    public class CustomCharacterAttributeRow : MonoBehaviour
    {
        #region Properties + Components

        [Header("Data")]
        [SerializeField]
        private CoreAttribute attribute;
        [SerializeField] private Color normalStatTextColor;
        [SerializeField] private Color boostedStatTextColor;
        [Space(10)]
        [Header("Components")]
        [SerializeField]
        private TextMeshProUGUI amountText;
        [SerializeField] private GameObject plusButtonParent;
        [SerializeField] private GameObject minusButtonParent;

        #endregion

        #region Getters + Accessors

        public CoreAttribute Attribute => attribute;
        public Color NormalStatTextColor => normalStatTextColor;
        public Color BoostedStatTextColor => boostedStatTextColor;
        public TextMeshProUGUI AmountText => amountText;
        public GameObject PlusButtonParent => plusButtonParent;
        public GameObject MinusButtonParent => minusButtonParent;

        #endregion

        #region Logic

        public void OnIncreaseAttributeButtonClicked()
        {
            MainMenuController.Instance.OnIncreaseAttributeButtonClicked(this);
        }
        public void OnDecreaseAttributeButtonClicked()
        {
            MainMenuController.Instance.OnDecreaseAttributeButtonClicked(this);
        }

        #endregion
    }
}
