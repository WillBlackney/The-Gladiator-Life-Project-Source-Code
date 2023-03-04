using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HexGameEngine.GameIntroEvent
{
    public class GameIntroButton : MonoBehaviour
    {
        [SerializeField] GameObject visualParent;
        [SerializeField] TextMeshProUGUI buttonText;
        private Action onClickCallback;


        public void Hide()
        {
            visualParent.SetActive(false);
        }
        public void BuildAndShow(string text, Action onClick)
        {
            visualParent.SetActive(true);
            buttonText.text = text;
            onClickCallback = onClick;
        }

        public void OnClick()
        {
            onClickCallback?.Invoke();
        }
    }
}