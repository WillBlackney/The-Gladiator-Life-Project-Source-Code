using UnityEngine;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.MainMenu;

namespace WeAreGladiators.UI
{
    public class ChooseTalentButton : MonoBehaviour
    {
        #region Properties + Components

        [SerializeField] private UITalentIcon talentIcon;
        [SerializeField] private GameObject unselectedVisualParent;
        [SerializeField] private TalentPairing talentPairing;

        #endregion

        #region Getters + Accessors

        public UITalentIcon TalentIcon => talentIcon;
        public GameObject SelectedVisualParent => unselectedVisualParent;
        public bool Selected { get; private set; }
        public TalentPairing TalentPairing => talentPairing;

        #endregion

        #region Logic

        public void ResetAndHide()
        {
            gameObject.SetActive(false);
            HandleChangeSelectionState(false);
        }
        public void BuildAndShow()
        {
            gameObject.SetActive(true);
            talentIcon.BuildFromTalentPairing(talentPairing);
        }
        public void HandleChangeSelectionState(bool selection)
        {
            if (selection)
            {
                Selected = true;
                unselectedVisualParent.SetActive(false);
            }
            else
            {
                Selected = false;
                unselectedVisualParent.SetActive(true);
            }

            AudioManager.Instance.PlaySound(Sound.UI_Button_Click);
        }
        public void OnClick()
        {
            // to do handle select and deselect + check for excedding choice limit of 3
            if (Selected)
            {
                HandleChangeSelectionState(false);
            }
            else if (!Selected && MainMenuController.Instance.GetSelectedTalents().Count < 2)
            {
                HandleChangeSelectionState(true);
            }

            MainMenuController.Instance.UpdateChosenTalentsText();

        }

        #endregion
    }
}
