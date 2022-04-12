using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardGameEngine.UCM;
using HexGameEngine.Characters;
using HexGameEngine.UCM;
using HexGameEngine.UI;
using HexGameEngine.Player;
using HexGameEngine.Perks;
using HexGameEngine.Abilities;
using UnityEngine.UI;

namespace HexGameEngine.TownFeatures
{
    public class LibraryAbilityDropSlot : MonoBehaviour
    {
        // Components + Properties
        #region
        [Header("Core Components")]
        [SerializeField] Image bookImage;

        // Non inspector fields
        private AbilityData myAbilityData;
        private static bool mousedOver = false;
        #endregion

        // Getters + Accessors
        #region
        public AbilityData MyAbilityData
        {
            get { return myAbilityData; }
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
            ClearAbility();
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
        public void BuildFromAbility(AbilityData a)
        {
            myAbilityData = a;
            bookImage.sprite = a.AbilitySprite;
            bookImage.gameObject.SetActive(true);
            TownController.Instance.EvaluateLibrarySlots();
        }
        public void ClearAbility()
        {
            myAbilityData = null;
            bookImage.gameObject.SetActive(false);
            TownController.Instance.EvaluateLibrarySlots();
        }

        #endregion

    }
}