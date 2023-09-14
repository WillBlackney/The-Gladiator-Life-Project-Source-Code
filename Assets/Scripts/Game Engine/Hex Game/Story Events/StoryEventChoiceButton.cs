using TMPro;
using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.StoryEvents
{
    public class StoryEventChoiceButton : MonoBehaviour
    {

        #region Getters + Accessors

        public StoryEventChoiceSO MyChoiceData { get; private set; }

        #endregion
        #region Components + Variables

        [Header("Components")]
        [SerializeField]
        private GameObject visualParent;
        [SerializeField] private TextMeshProUGUI buttonText;

        #endregion

        #region Logic

        public void HideAndReset()
        {
            visualParent.SetActive(false);
        }
        public void BuildAndShow(StoryEventChoiceSO choiceData)
        {
            visualParent.SetActive(true);
            buttonText.text = StoryEventController.Instance.GetDynamicValueString(TextLogic.ConvertCustomStringListToString(choiceData.choiceTextOnButton));
            MyChoiceData = choiceData;
        }
        public void OnClick()
        {
            StoryEventController.Instance.HandleChoiceButtonClicked(this);
        }

        #endregion
    }
}
