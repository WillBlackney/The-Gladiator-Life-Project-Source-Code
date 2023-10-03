using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators.UI
{
    public class CharacterRosterPageButton : MonoBehaviour
    {
        [SerializeField] private GameObject selected;
        [SerializeField] private LevelUpButton levelUpButton;

        public static CharacterRosterPageButton selectedButton { get; private set; }

        public void SetSelectedViewState()
        {
            if (selectedButton != null)
            {
                selectedButton.SetUnselectedViewState();
            }

            selectedButton = this;
            selected.SetActive(true);
        }
        public void SetUnselectedViewState()
        {
            selected.SetActive(false);
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
