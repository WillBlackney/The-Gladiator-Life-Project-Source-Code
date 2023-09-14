using UnityEngine;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.UCM;

namespace WeAreGladiators.TownFeatures
{
    public class LibraryCharacterDropSlot : MonoBehaviour
    {
        // Components + Properties
        #region

        [Header("Core Components")]
        [SerializeField]
        private GameObject portraitVisualParent;
        [SerializeField] private UniversalCharacterModel portraitModel;

        // Non inspector fields

        #endregion

        // Getters + Accessors
        #region

        public HexCharacterData MyCharacterData { get; private set; }
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
            ClearCharacter();
        }
        public void MouseEnter()
        {
            MousedOver = true;
        }
        public void MouseExit()
        {
            MousedOver = false;
        }

        #endregion

        // Logic
        #region

        public void BuildFromCharacter(HexCharacterData character)
        {
            MyCharacterData = character;
            portraitVisualParent.SetActive(true);
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(portraitModel, MyCharacterData.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(MyCharacterData.itemSet, portraitModel);
            TownController.Instance.EvaluateLibrarySlots();
            AudioManager.Instance.PlaySound(Sound.UI_Drag_Drop_End);
        }
        public void ClearCharacter()
        {
            MyCharacterData = null;
            portraitVisualParent.SetActive(false);
            TownController.Instance.EvaluateLibrarySlots();
        }

        #endregion
    }
}
