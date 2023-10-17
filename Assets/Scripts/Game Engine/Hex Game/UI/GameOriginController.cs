using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.MainMenu;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.GameOrigin
{
    public class GameOriginController : Singleton<GameOriginController>
    {
        #region Components + Variables

        [Header("Core Components")]
        [SerializeField] private GameObject visualParent;
        [SerializeField] private CanvasGroup contentCg;
        [SerializeField] private Image blackUnderlay;

        [Header("Movement Components")]
        [SerializeField] private RectTransform onScreenPosition;
        [SerializeField] private RectTransform offScreenPosition;
        [SerializeField] private RectTransform movementParent;
        [SerializeField] private TextMeshProUGUI goldModText;

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
            goldModText.text = "Start with " +
                TextLogic.ReturnColoredText(GlobalSettings.Instance.BaseStartingGold.ToString(), TextLogic.blueNumber) + " " +
                TextLogic.ReturnColoredText("Gold", TextLogic.neutralYellow) + ".";

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
