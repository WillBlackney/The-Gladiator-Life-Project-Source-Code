using System;
using TMPro;
using UnityEngine;

namespace WeAreGladiators.GameIntroEvent
{
    public class GameIntroButton : MonoBehaviour
    {
        [SerializeField] private GameObject visualParent;
        [SerializeField] private TextMeshProUGUI buttonText;

        private bool dynamicCallback;
        private Action onClickCallback;

        public GameIntroChoiceData ChoiceData { get; private set; }

        public void HideAndReset()
        {
            visualParent.SetActive(false);
            onClickCallback = null;
            ChoiceData = null;
        }
        public void Build(GameIntroChoiceData choiceData)
        {
            dynamicCallback = false;
            visualParent.SetActive(true);
            buttonText.text = choiceData.buttonText;
            this.ChoiceData = choiceData;
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
