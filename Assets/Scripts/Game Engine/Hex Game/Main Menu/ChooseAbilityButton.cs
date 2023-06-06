using HexGameEngine.Abilities;
using HexGameEngine.MainMenu;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HexGameEngine.UI
{
    public class ChooseAbilityButton : MonoBehaviour
    {
        #region Properties + Components
        [SerializeField] UIAbilityIcon abilityIcon;
        [SerializeField] GameObject[] selectedStateObjects;
        private bool selected = false;
        #endregion

        #region Getters + Accesors
        public UIAbilityIcon AbilityIcon
        {
            get { return abilityIcon; }
        }
        public GameObject[] SelectedStateObjects
        {
            get { return selectedStateObjects; }
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
                Array.ForEach(selectedStateObjects, x => x.SetActive(true));
            }
            else
            {
                selected = false;
                Array.ForEach(selectedStateObjects, x => x.SetActive(false));
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

        public void MouseEnter()
        {
            if (abilityIcon.MyDataRef != null)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(abilityIcon.MyDataRef.keyWords);
                AbilityPopupController.Instance.OnRosterAbilityButtonMousedOver(abilityIcon);
            }
        }

        public void MouseExit()
        {
            if (abilityIcon.MyDataRef != null)
            {
                KeyWordLayoutController.Instance.FadeOutMainView();
                AbilityPopupController.Instance.OnAbilityButtonMousedExit();
            }
        }
        #endregion
    }
}