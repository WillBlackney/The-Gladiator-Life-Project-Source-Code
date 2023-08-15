using DG.Tweening;
using WeAreGladiators.MainMenu;
using WeAreGladiators.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators.GameOrigin
{
    public class GameOriginController : Singleton<GameOriginController>
    {
        #region Components + Variables
        [Header("Core Components")]
        [SerializeField] GameObject visualParent;
        [SerializeField] CanvasGroup contentCg;
        [SerializeField] Image blackUnderlay;

        [Header("Movement Components")]
        [SerializeField] RectTransform onScreenPosition;
        [SerializeField] RectTransform offScreenPosition;
        [SerializeField] RectTransform movementParent;

        #endregion

        #region UI Logic
        public void ShowOriginScreen()
        {
            visualParent.SetActive(true);
            contentCg.DOKill();
            contentCg.interactable = false;
            blackUnderlay.DOKill();
            blackUnderlay.DOFade(0f, 0f);
            movementParent.DOKill();
            movementParent.position = offScreenPosition.position;

            // to do: build content to default state

            blackUnderlay.DOFade(0.5f, 0.5f);
            movementParent.DOMove(onScreenPosition.position, 0.65f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                contentCg.interactable = true;
            });
        }
        private void HideOriginScreen(float speed = 0.65f)
        {
            visualParent.SetActive(true);
            contentCg.DOKill();
            contentCg.interactable = false;
            blackUnderlay.DOKill();
            movementParent.DOKill();

            // to do: build content to default state

            blackUnderlay.DOFade(0f, speed * 0.75f);
            movementParent.DOMove(offScreenPosition.position, speed).SetEase(Ease.InBack).OnComplete(() =>
            {
                visualParent.SetActive(false);
            });
        }
        #endregion

        #region Buttons Logic
        public void OnCloseOriginPageButtClicked()
        {
            HideOriginScreen();
        }
        public void OnStartGameButtonClicked()
        {
            contentCg.interactable = true;
            BlackScreenController.Instance.FadeOutAndBackIn(0.75f, 0.25f, 0.75f, () =>
            {
                HideOriginScreen(0f);
                MainMenuController.Instance.SetCustomCharacterDataDefaultState();
                MainMenuController.Instance.ShowChooseCharacterScreen();
                MainMenuController.Instance.HideFrontScreen();
            });
        }
        #endregion
    }
}