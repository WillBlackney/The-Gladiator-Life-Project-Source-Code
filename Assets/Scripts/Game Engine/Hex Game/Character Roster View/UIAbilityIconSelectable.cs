using UnityEngine;
using WeAreGladiators.Abilities;

namespace WeAreGladiators.UI
{
    public class UIAbilityIconSelectable : MonoBehaviour
    {
        #region Properties + Components

        [SerializeField] private GameObject selectStateParent;
        [SerializeField] private GameObject selectedParent;
        [SerializeField] private GameObject unselectedParent;
        public UIAbilityIcon icon;

        #endregion

        #region Getters + Accessors
        public GameObject SelectedParent => selectedParent;
        public GameObject UnselectedParent => unselectedParent;

        #endregion

        #region Logic
        public void OnClick()
        {
            CharacterRosterViewController.Instance.OnSelectableAbilityButtonClicked(this);
        }
        public void Hide()
        {
            gameObject.SetActive(false);
            selectStateParent.SetActive(false);
        }
        public void Build(AbilityData data, bool selected, bool showSelectionState)
        {
            selectStateParent.SetActive(false);
            gameObject.SetActive(true);
            icon.BuildFromAbilityData(data);
            if (showSelectionState)
            {
                selectStateParent.SetActive(true);
                SetSelectedViewState(selected);
            }
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
        #endregion
    }
}
