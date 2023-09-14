using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Abilities;
using WeAreGladiators.UI;

namespace WeAreGladiators.TownFeatures
{
    public class LibraryAbilityDropSlot : MonoBehaviour
    {
        // Components + Properties
        #region

        [Header("Core Components")]
        [SerializeField]
        private Image bookImage;

        // Non inspector fields

        #endregion

        // Getters + Accessors
        #region

        public AbilityData MyAbilityData { get; private set; }
        public static bool MousedOver { get; private set; }

        #endregion

        // Input 
        #region

        private void Update()
        {
            if (MousedOver && Input.GetKeyDown(KeyCode.Mouse1))
            {
                OnRightClick();
            }
        }
        private void OnRightClick()
        {
            ClearAbility();
        }
        public void MouseEnter()
        {
            MousedOver = true;
            if (MyAbilityData != null)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(MyAbilityData.keyWords);
                AbilityPopupController.Instance.OnLibraryAbilityDropSlotMousedOver(this);
            }
        }
        public void MouseExit()
        {
            MousedOver = false;
            if (MyAbilityData != null)
            {
                KeyWordLayoutController.Instance.FadeOutMainView();
                AbilityPopupController.Instance.OnAbilityButtonMousedExit();
            }
        }

        #endregion

        // Logic
        #region

        public void BuildFromAbility(AbilityData a)
        {
            MyAbilityData = a;
            bookImage.sprite = a.AbilitySprite;
            bookImage.gameObject.SetActive(true);
            TownController.Instance.EvaluateLibrarySlots();
        }
        public void ClearAbility()
        {
            MyAbilityData = null;
            bookImage.gameObject.SetActive(false);
            TownController.Instance.EvaluateLibrarySlots();
        }

        #endregion
    }
}
