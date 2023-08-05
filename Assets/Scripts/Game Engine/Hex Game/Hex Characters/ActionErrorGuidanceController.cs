using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;
using DG.Tweening;
using TMPro;
using WeAreGladiators.TurnLogic;
using WeAreGladiators.Characters;

namespace WeAreGladiators.UI
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

        public void ShowErrorMessage(HexCharacterModel user, string message, float showSpeed = 0.15f)
        {
            if (!ShouldShowErrorMessage(user)) return;

            ErrorMessageEventHandle handle = new ErrorMessageEventHandle();
            currentHandle = handle;
            rootCanvas.enabled = true;
            movementParent.DOKill();
            messageText.DOKill();
            messageText.color = textStartColor;
            cg.DOKill();
            cg.alpha = 0;
            movementParent.position = offScreenPosition.position;


            // Initial animation
            messageText.text = message;
            messageText.DOColor(textEndColor, 0.15f).SetLoops(6, LoopType.Yoyo);
            movementParent.DOMove(onScreenPosition.position, showSpeed).SetEase(showEase);
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
        private bool ShouldShowErrorMessage(HexCharacterModel user)
        {
            return user != null &&
                user == TurnController.Instance.EntityActivated &&
                user.controller == Controller.Player &&
                 user.activationPhase == ActivationPhase.ActivationPhase;
        }
    }

    class ErrorMessageEventHandle
    {

    }

    

}