using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using DG.Tweening;
using TMPro;
using HexGameEngine.TurnLogic;

namespace HexGameEngine.UI
{
    public class ActionErrorGuidanceController : Singleton<ActionErrorGuidanceController>
    {
        #region Components
        [Header("Settings")]
        [SerializeField] Ease showEase = Ease.Linear;
        [SerializeField] Ease hideEase = Ease.Linear;

        [Space(10)]

        [Header("Core Components")]
        [SerializeField] Canvas rootCanvas;
        [SerializeField] CanvasGroup cg;
        [SerializeField] TextMeshProUGUI messageText;

        [Header("Movement Components")]
        [SerializeField] RectTransform movementParent;
        [SerializeField] RectTransform onScreenPosition;
        [SerializeField] RectTransform offScreenPosition;

        [SerializeField] Color textStartColor;
        [SerializeField] Color textEndColor;

        private ErrorMessageEventHandle currentHandle;

        #endregion

        public void ShowErrorMessage(string message, float showSpeed = 0.15f)
        {
            if (!ShouldShowErrorMessage()) return;

            ErrorMessageEventHandle handle = new ErrorMessageEventHandle();
            currentHandle = handle;
            rootCanvas.enabled = true;
            movementParent.DOKill();
            messageText.DOKill();
            messageText.color = textStartColor;
            cg.DOKill();
            cg.alpha = 0;
            movementParent.DOMove(offScreenPosition.position, 0f);


            // Initial animation
            messageText.text = message;
            messageText.DOColor(textEndColor, 0.15f).SetLoops(6, LoopType.Yoyo);
            movementParent.DOMove(onScreenPosition.position, showSpeed);
            cg.DOFade(1f, showSpeed * 0.75f);

            // Pause for a bit
            DelayUtils.DelayedCall(3f, () =>
            {
                if (handle != currentHandle) return;
                HideErrorMessage();
            });
        }
        public void HideErrorMessage(float hideSpeed = 0.15f)
        {
            movementParent.DOKill();
            cg.DOKill(); 
            messageText.DOKill();

            cg.DOFade(0, hideSpeed * 0.75f);
            movementParent.DOMove(offScreenPosition.position, hideSpeed).SetEase(hideEase).OnComplete(() =>
            {
                rootCanvas.enabled = false;
            });
        }
        public bool ShouldShowErrorMessage()
        {
            return TurnController.Instance.EntityActivated != null &&
                TurnController.Instance.EntityActivated.controller == Controller.Player;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T)) ShowErrorMessage("Not enough energy!");
        }
    }

    class ErrorMessageEventHandle
    {

    }

    

}