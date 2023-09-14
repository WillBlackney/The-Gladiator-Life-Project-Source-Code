using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators.UI
{
    public class CharacterRosterPageButton : MonoBehaviour
    {
        [SerializeField] private Color normalColor;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Image buttonBgImage;
        [SerializeField] private Transform scalingParent;
        [SerializeField] private LevelUpButton levelUpButton;

        public static CharacterRosterPageButton selectedButton { get; private set; }

        public void SetSelectedViewState(float speed)
        {
            if (selectedButton != null)
            {
                selectedButton.SetUnselectedViewState(speed);
            }

            selectedButton = this;
            scalingParent.DOKill();
            buttonBgImage.DOKill();

            scalingParent.DOScale(1.1f, speed);
            buttonBgImage.DOColor(selectedColor, speed);
        }
        public void SetUnselectedViewState(float speed)
        {
            scalingParent.DOKill();
            buttonBgImage.DOKill();

            scalingParent.DOScale(1f, speed);
            buttonBgImage.DOColor(normalColor, speed);
        }
        public void ShowLevelUpIcon(bool onOrOff)
        {
            if (onOrOff)
            {
                levelUpButton.ShowAndAnimate();
            }
            else
            {
                levelUpButton.Hide();
            }
        }
    }
}
