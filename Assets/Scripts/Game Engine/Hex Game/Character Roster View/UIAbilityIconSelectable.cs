using WeAreGladiators.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WeAreGladiators.UI
{
    public class UIAbilityIconSelectable : MonoBehaviour
    {
        #region Properties + Components

        [SerializeField] GameObject selectedParent;
        [SerializeField] GameObject unselectedParent;
        public UIAbilityIcon icon;

        #endregion

        #region Getters + Accessors
        public GameObject SelectedParent
        {
            get { return selectedParent; }
        }
        public GameObject UnselectedParent
        {
            get { return unselectedParent; }
        }
        #endregion
        public void OnClick()
        {
            CharacterRosterViewController.Instance.OnSelectableAbilityButtonClicked(this);
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Build(AbilityData data, bool selected)
        {
            gameObject.SetActive(true);
            icon.BuildFromAbilityData(data);
            SetSelectedViewState(selected);
        }
        public void SetSelectedViewState(bool selected)
        {
            if (selected)
            {
                selectedParent.SetActive(true);
                unselectedParent.SetActive(false);
            }
            else
            {
                selectedParent.SetActive(false);
                unselectedParent.SetActive(true);
            }
        }
    }
}