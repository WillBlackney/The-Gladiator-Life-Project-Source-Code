using HexGameEngine.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HexGameEngine.StoryEvents
{
    public class StoryEventChoiceButton : MonoBehaviour
    {
        [SerializeField] GameObject visualParent;
        [SerializeField] TextMeshProUGUI buttonText;
        private StoryEventChoiceSO myChoiceData;
        public StoryEventChoiceSO MyChoiceData { get { return myChoiceData; } }

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
    }
}