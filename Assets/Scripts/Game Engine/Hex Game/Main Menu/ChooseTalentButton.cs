using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.MainMenu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.UI
{
    public class ChooseTalentButton : MonoBehaviour
    {
        #region Properties + Components
        [SerializeField] UITalentIcon talentIcon;
        [SerializeField] GameObject unselectedVisualParent;
        [SerializeField] TalentPairing talentPairing;
        private bool selected = false;
        #endregion

        #region Getters + Accessors
        public UITalentIcon TalentIcon
        {
            get { return talentIcon; }
        }
        public GameObject SelectedVisualParent
        {
            get { return unselectedVisualParent; }
        }
        public bool Selected
        {
            get { return selected; }
        }
        public TalentPairing TalentPairing
        {
            get { return talentPairing; }
        }
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
                selected = true;
                unselectedVisualParent.SetActive(false);
            }
            else
            {
                selected = false;
                unselectedVisualParent.SetActive(true);
            }

            AudioManager.Instance.PlaySound(Sound.UI_Button_Click);
        }
        public void OnClick()
        {
            // to do handle select and deselect + check for excedding choice limit of 3
            if (selected)
                HandleChangeSelectionState(false);
            else if (!selected && MainMenuController.Instance.GetSelectedTalents().Count < 2)
                HandleChangeSelectionState(true);

            MainMenuController.Instance.UpdateChosenTalentsText();


        }
        #endregion
    }
}