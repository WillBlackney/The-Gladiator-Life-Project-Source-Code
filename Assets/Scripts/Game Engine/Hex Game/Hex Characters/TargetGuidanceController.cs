using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class TargetGuidanceController : Singleton<TargetGuidanceController>
    {
        #region Components

        [Header("Settings")]
        [SerializeField]
        private Ease onScreenEase = Ease.Linear;
        [SerializeField] private Ease offScreenEase = Ease.Linear;

        [Space(10)]
        [Header("Core Components")]
        [SerializeField]
        private Canvas rootCanvas;
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Image iconImage;

        [Space(10)]
        [Header("Movement Components")]
        [SerializeField]
        private RectTransform movementParent;
        [SerializeField] private RectTransform onScreenPosition;
        [SerializeField] private RectTransform offScreenPosition;

        [Header("Icon Sprites")]
        [SerializeField]
        private Sprite allyIcon;
        [SerializeField] private Sprite enemyIcon;
        [SerializeField] private Sprite characterIcon;
        [SerializeField] private Sprite directionIcon;
        [SerializeField] private Sprite tileIcon;
        [SerializeField] private Sprite destinationIcon;

        #endregion

        #region Logic

        public void BuildAndShow(GuidanceInstruction guidance, float showSpeed = 0.35f)
        {
            if (guidance == GuidanceInstruction.None)
            {
                return;
            }
            MoveOnScreen(showSpeed);
            BuildMessage(guidance);
        }
        public void Hide(float hideSpeed = 0.5f)
        {
            MoveOffScreen(hideSpeed);
        }

        private void MoveOnScreen(float speed)
        {
            movementParent.DOKill();
            cg.DOKill();
            iconImage.transform.DOKill();
            iconImage.transform.DOScale(1, 0);
            rootCanvas.enabled = true;
            movementParent.DOMove(onScreenPosition.position, speed).SetEase(onScreenEase);
            iconImage.transform.DOScale(1.25f, 0.25f).SetLoops(-1, LoopType.Yoyo);
            cg.DOFade(1f, speed * 0.75f);
        }
        private void MoveOffScreen(float speed)
        {
            movementParent.DOKill();
            cg.DOKill();
            movementParent.DOMove(offScreenPosition.position, speed * 0.75f).SetEase(offScreenEase);
            cg.DOFade(0f, speed).OnComplete(() =>
            {
                rootCanvas.enabled = false;
                iconImage.transform.DOKill();
                iconImage.transform.DOScale(1, 0);
            });
        }
        private void BuildMessage(GuidanceInstruction guidance)
        {
            if (guidance == GuidanceInstruction.SelectAnEnemy)
            {
                iconImage.sprite = enemyIcon;
                messageText.text = "Select an enemy";
            }
            else if (guidance == GuidanceInstruction.SelectAnAlly)
            {
                iconImage.sprite = allyIcon;
                messageText.text = "Select an ally";
            }
            else if (guidance == GuidanceInstruction.SelectSelfOrAnAlly)
            {
                iconImage.sprite = allyIcon;
                messageText.text = "Select ally or self";
            }
            else if (guidance == GuidanceInstruction.SelectATile)
            {
                iconImage.sprite = tileIcon;
                messageText.text = "Select a tile";
            }
            else if (guidance == GuidanceInstruction.SelectADirection)
            {
                iconImage.sprite = directionIcon;
                messageText.text = "Select a direction";
            }
            else if (guidance == GuidanceInstruction.SelectDestination)
            {
                iconImage.sprite = destinationIcon;
                messageText.text = "Select a destination";
            }
            else if (guidance == GuidanceInstruction.SelectACharacter)
            {
                iconImage.sprite = characterIcon;
                messageText.text = "Select a character";
            }
        }

        #endregion
    }
    public enum GuidanceInstruction
    {
        None = 0,
        SelectAnEnemy = 1,
        SelectAnAlly = 2,
        SelectSelfOrAnAlly = 3,
        SelectACharacter = 4,
        SelectATile = 5,
        SelectADirection = 6,
        SelectDestination = 7
    }
}
