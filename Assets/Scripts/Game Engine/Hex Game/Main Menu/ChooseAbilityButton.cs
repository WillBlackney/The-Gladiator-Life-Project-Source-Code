using System;
using UnityEngine;
using WeAreGladiators.Abilities;
using WeAreGladiators.MainMenu;

namespace WeAreGladiators.UI
{
    public class ChooseAbilityButton : MonoBehaviour
    {
        #region Properties + Components

        [SerializeField] private UIAbilityIcon abilityIcon;
        [SerializeField] private GameObject[] selectedStateObjects;

        #endregion

        #region Getters + Accesors

        public UIAbilityIcon AbilityIcon => abilityIcon;
        public GameObject[] SelectedStateObjects => selectedStateObjects;
        public bool Selected { get; private set; }

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
                Selected = true;
                Array.ForEach(selectedStateObjects, x => x.SetActive(true));
            }
            else
            {
                Selected = false;
                Array.ForEach(selectedStateObjects, x => x.SetActive(false));
            }
        }
        public void OnClick()
        {
            // to do handle select and deselect + check for excedding choice limit of 3
            if (Selected)
            {
                HandleChangeSelectionState(false);
            }
            else if (!Selected && MainMenuController.Instance.GetSelectedAbilities().Count < 3)
            {
                HandleChangeSelectionState(true);
            }

            MainMenuController.Instance.UpdateChosenAbilitiesText();

        }

        public void MouseEnter()
        {
            if (abilityIcon.MyDataRef != null)
            {
                //abilityIcon.OnPointerEnter(null);
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(abilityIcon.MyDataRef.keyWords);
                AbilityPopupController.Instance.OnRosterAbilityButtonMousedOver(abilityIcon);
                AbilityPopupController.Instance.OnChooseAbilityButtonMousedOver(this);
            }
        }

        public void MouseExit()
        {
            if (abilityIcon.MyDataRef != null)
            {
                //abilityIcon.OnPointerExit(null);
                KeyWordLayoutController.Instance.FadeOutMainView();
                AbilityPopupController.Instance.OnAbilityButtonMousedExit();
            }
        }

        #endregion
    }
}
