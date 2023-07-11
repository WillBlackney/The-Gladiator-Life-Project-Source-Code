using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.UCM;
using HexGameEngine.Characters;
using HexGameEngine.UI;
using HexGameEngine.Player;
using HexGameEngine.Perks;
using HexGameEngine.Audio;

namespace HexGameEngine.TownFeatures
{
    public class LibraryCharacterDropSlot : MonoBehaviour
    {
        // Components + Properties
        #region
        [Header("Core Components")]
        [SerializeField] GameObject portraitVisualParent;
        [SerializeField] UniversalCharacterModel portraitModel;

        // Non inspector fields
        private HexCharacterData myCharacterData;
        private static bool mousedOver = false;
        #endregion

        // Getters + Accessors
        #region
        public HexCharacterData MyCharacterData
        {
            get { return myCharacterData; }
        }
        public static bool MousedOver
        {
            get { return mousedOver; }
        }
        #endregion

        // Input 
        #region
        void Update()
        {
            if (mousedOver && Input.GetKeyDown(KeyCode.Mouse1))
                OnRightClick();
        }
        private void OnRightClick()
        {
            ClearCharacter();
        }
        public void MouseEnter()
        {
            mousedOver = true;
        }
        public void MouseExit()
        {
            mousedOver = false;
        }
        #endregion

        // Logic
        #region
        public void BuildFromCharacter(HexCharacterData character)
        {
            myCharacterData = character;
            portraitVisualParent.SetActive(true);
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(portraitModel, myCharacterData.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(myCharacterData.itemSet, portraitModel);
            TownController.Instance.EvaluateLibrarySlots();
            AudioManager.Instance.PlaySound(Sound.UI_Drag_Drop_End);
        }
        public void ClearCharacter()
        {
            myCharacterData = null;
            portraitVisualParent.SetActive(false);
            TownController.Instance.EvaluateLibrarySlots();
        }
        #endregion

    }
}