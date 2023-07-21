using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;
using DG.Tweening;
using UnityEngine.UI;

namespace WeAreGladiators.UI
{
    public class AlphaWarningController : Singleton<AlphaWarningController>
    {
        [Header("Components")]
        [SerializeField] GameObject visualParent;
        [SerializeField] Image blackUnderlayImage;
        [SerializeField] Button confirmButton;

        [Space(10)]

        [Header("Content Movement")]
        [SerializeField] RectTransform movementParent;
        [SerializeField] RectTransform onScreenPosition;
        [SerializeField] RectTransform offScreenPosition;
        

        bool hasShownWarningThisSession = false;
        Action onConfirm;

        public bool HasShownWarningThisSession => hasShownWarningThisSession;

        public void ShowWarningPage(Action onConfirmClicked = null)
        {
            hasShownWarningThisSession = true;
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
                .OnComplete(()=> confirmButton.interactable = true);
        }

        private void OnConfirmButtonClicked()
        {
            confirmButton.interactable = false;
            blackUnderlayImage.DOKill();
            movementParent.DOKill();
            blackUnderlayImage.DOFade(0f, 1f);
            movementParent.DOMove(offScreenPosition.position, 1f).SetEase(Ease.InBack).OnComplete(() => 
            { 
                if (onConfirm != null) onConfirm.Invoke();
                visualParent.SetActive(false);
            });


        }
    }
}