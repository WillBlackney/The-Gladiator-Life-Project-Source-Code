using HexGameEngine.Abilities;
using HexGameEngine.MainMenu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.UI
{
    public class ChooseAbilityButton : MonoBehaviour
    {
        #region Properties + Components
        [SerializeField] UIAbilityIcon abilityIcon;
        [SerializeField] GameObject selectedVisualParent;
        private bool selected = false;
        #endregion

        #region Getters + Accesors
        public UIAbilityIcon AbilityIcon
        {
            get { return abilityIcon; }
        }
        public GameObject SelectedVisualParent
        {
            get { return selectedVisualParent; }
        }
        public bool Selected
        {
            get { return selected; }
        }
        #endregion

        #region Logic
        public void ResetAndHide()
        {
            gameObject.SetActive(false);
            HandleChangeSelectionState(false);
        }
        public void BuildAndShow(AbilityData data)
        {
            gameObject.SetActive(true);
            abilityIcon.BuildFromAbilityData(data);
        }
        public void HandleChangeSelectionState(bool selection)
        {
            if (selection)
            {
                selected = true;
                selectedVisualParent.SetActive(true);
            }
            else
            {
                selected = false;
                selectedVisualParent.SetActive(false);
            }
        }
        public void OnClick()
        {
            // to do handle select and deselect + check for excedding choice limit of 3
            if (selected)            
                HandleChangeSelectionState(false);
            else if(!selected && MainMenuController.Instance.GetSelectedAbilities().Count < 3)            
                HandleChangeSelectionState(true);

            MainMenuController.Instance.UpdateChosenAbilitiesText();


        }
        #endregion
    }
}