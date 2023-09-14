using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class AlphaWarningController : Singleton<AlphaWarningController>
    {
        [Header("Components")]
        [SerializeField]
        private GameObject visualParent;
        [SerializeField] private Image blackUnderlayImage;
        [SerializeField] private Button confirmButton;

        [Space(10)]
        [Header("Content Movement")]
        [SerializeField]
        private RectTransform movementParent;
        [SerializeField] private RectTransform onScreenPosition;
        [SerializeField] private RectTransform offScreenPosition;

        private Action onConfirm;

        public bool HasShownWarningThisSession { get; private set; }

        public void ShowWarningPage(Action onConfirmClicked = null)
        {
            HasShownWarningThisSession = true;
            onConfirm = onConfirmClicked;
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);

            // Reset
            confirmButton.interactable = false;
            visualParent.SetActive(true);
            blackUnderlayImage.DOFade(0, 0);
            movementParent.position = offScreenPosition.position;
            visualParent.SetActive(true);

            // Show
            blackUnderlayImage.DOFade(0.5f, 0.75f);
            movementParent.DOMove(onScreenPosition.position, 1f).SetEase(Ease.OutBack)
                .OnComplete(() => confirmButton.interactable = true);
        }

        private void OnConfirmButtonClicked()
        {
            confirmButton.interactable = false;
            blackUnderlayImage.DOKill();
            movementParent.DOKill();
            blackUnderlayImage.DOFade(0f, 1f);
            movementParent.DOMove(offScreenPosition.position, 1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                if (onConfirm != null)
                {
                    onConfirm.Invoke();
                }
                visualParent.SetActive(false);
            });

        }
    }
}
