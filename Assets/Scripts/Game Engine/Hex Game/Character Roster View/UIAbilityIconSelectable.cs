using UnityEngine;
using WeAreGladiators.Abilities;

namespace WeAreGladiators.UI
{
    public class UIAbilityIconSelectable : MonoBehaviour
    {
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
        #region Properties + Components

        [SerializeField] private GameObject selectedParent;
        [SerializeField] private GameObject unselectedParent;
        public UIAbilityIcon icon;

        #endregion

        #region Getters + Accessors

        public GameObject SelectedParent => selectedParent;
        public GameObject UnselectedParent => unselectedParent;

        #endregion
    }
}
