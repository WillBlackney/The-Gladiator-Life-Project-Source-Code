using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using WeAreGladiators.MainMenu;

namespace WeAreGladiators.UI
{
    public class CustomCharacterAttributeRow : MonoBehaviour
    {
        #region Properties + Components
        [Header("Data")]
        [SerializeField] CoreAttribute attribute;
        [SerializeField] Color normalStatTextColor;
        [SerializeField] Color boostedStatTextColor;
        [Space(10)]
        [Header("Components")]
        [SerializeField] TextMeshProUGUI amountText;
        [SerializeField] GameObject plusButtonParent;
        [SerializeField] GameObject minusButtonParent;
        #endregion

        #region Getters + Accessors
        public CoreAttribute Attribute
        {
            get { return attribute; }
        }
        public Color NormalStatTextColor
        {
            get { return normalStatTextColor; }
        }
        public Color BoostedStatTextColor
        {
            get { return boostedStatTextColor; }
        }
        public TextMeshProUGUI AmountText
        {
            get { return amountText; }
        }
        public GameObject PlusButtonParent
        {
            get { return plusButtonParent; }
        }
        public GameObject MinusButtonParent
        {
            get { return minusButtonParent; }
        }
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