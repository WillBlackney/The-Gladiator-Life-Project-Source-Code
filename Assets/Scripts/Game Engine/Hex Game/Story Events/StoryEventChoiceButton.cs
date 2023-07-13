using WeAreGladiators.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WeAreGladiators.StoryEvents
{
    public class StoryEventChoiceButton : MonoBehaviour
    {
        #region Components + Variables
        [Header("Components")]
        [SerializeField] GameObject visualParent;
        [SerializeField] TextMeshProUGUI buttonText;

        private StoryEventChoiceSO myChoiceData;
        #endregion

        #region Getters + Accessors
        public StoryEventChoiceSO MyChoiceData { get { return myChoiceData; } }
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
            myChoiceData = choiceData;
        }
        public void OnClick()
        {
            StoryEventController.Instance.HandleChoiceButtonClicked(this);
        }
        #endregion
    }
}