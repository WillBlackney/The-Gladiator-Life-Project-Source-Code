using DG.Tweening;
using TMPro;
using UnityEngine;
using WeAreGladiators.Characters;
using WeAreGladiators.TurnLogic;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class ActionErrorGuidanceController : Singleton<ActionErrorGuidanceController>
    {

        public void ShowErrorMessage(HexCharacterModel user, string message, float showSpeed = 0.15f)
        {
            if (user != null && !ShouldShowErrorMessage(user))
            {
                return;
            }

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
                if (handle != currentHandle)
                {
                    return;
                }
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
        #region Components

        [Header("Settings")]
        [SerializeField]
        private Ease showEase = Ease.Linear;
        [SerializeField] private Ease hideEase = Ease.Linear;

        [Space(10)]
        [Header("Core Components")]
        [SerializeField]
        private Canvas rootCanvas;
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private TextMeshProUGUI messageText;

        [Header("Movement Components")]
        [SerializeField]
        private RectTransform movementParent;
        [SerializeField] private RectTransform onScreenPosition;
        [SerializeField] private RectTransform offScreenPosition;

        [SerializeField] private Color textStartColor;
        [SerializeField] private Color textEndColor;

        private ErrorMessageEventHandle currentHandle;

        #endregion
    }

    class ErrorMessageEventHandle
    {
    }

}
