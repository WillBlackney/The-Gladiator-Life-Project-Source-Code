using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HexGameEngine.UI
{
    public class CharacterRosterPageButton : MonoBehaviour
    {
        [SerializeField] Color normalColor;
        [SerializeField] Color selectedColor;
        [SerializeField] Image buttonBgImage;
        [SerializeField] Transform scalingParent;
        [SerializeField] LevelUpButton levelUpButton;

        public static CharacterRosterPageButton selectedButton { get; private set; }

        public void SetSelectedViewState(float speed)
        {
            if (selectedButton != null) selectedButton.SetUnselectedViewState(speed);

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
            if (onOrOff) levelUpButton.ShowAndAnimate();
            else levelUpButton.Hide();
        }
    }
}