using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace WeAreGladiators.GameIntroEvent
{
    public class GameIntroButton : MonoBehaviour
    {
        [SerializeField] GameObject visualParent;
        [SerializeField] TextMeshProUGUI buttonText;
        [SerializeField] GameIntroChoiceData choiceData;
        private Action onClickCallback;

        bool dynamicCallback = false;

        public GameIntroChoiceData ChoiceData => choiceData;


        public void HideAndReset()
        {
            visualParent.SetActive(false);
            onClickCallback = null;
            choiceData = null;
        }
        public void Build(GameIntroChoiceData choiceData)
        {
            dynamicCallback = false;
            visualParent.SetActive(true);
            buttonText.text = choiceData.buttonText;
            this.choiceData = choiceData;
        }
        public void BuildAndShow(string text, Action onClick)
        {
            visualParent.SetActive(true);
            buttonText.text = text;
            onClickCallback = onClick;
            dynamicCallback = true;
        }

        public void OnClick()
        {
            if (dynamicCallback)
            {
                onClickCallback?.Invoke();
            }
            else
            {
                GameIntroController.Instance.HandleChoiceButtonClicked(this);
            }
           
        }
    }
}