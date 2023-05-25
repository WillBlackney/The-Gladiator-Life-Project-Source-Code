using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Characters;
using HexGameEngine.UCM;
using HexGameEngine.UI;
using HexGameEngine.Player;
using HexGameEngine.Perks;

namespace HexGameEngine.TownFeatures
{
    public class HospitalDropSlot : MonoBehaviour
    {
        // Components + Properties
        #region
        [Header("Core Components")]
        [SerializeField] TownActivity featureType;
        [SerializeField] GameObject portraitVisualParent;
        [SerializeField] UniversalCharacterModel portraitModel;
        [SerializeField] GameObject cancelButtonParent;

        // Non inspector fields
        private static HospitalDropSlot slotMousedOver;
        private HexCharacterData myCharacterData;
        #endregion

        // Getters + Accessors
        #region
        public static int GetFeatureGoldCost(TownActivity feature)
        {
            // to do: probably should find a better place for this function
            // the costs of features should probably be determined by GlobalSettings
            if (feature == TownActivity.BedRest) return 100;
            else if (feature == TownActivity.Therapy) return 125;
            else if (feature == TownActivity.Surgery) return 150;
            else return 0;
        }
        public static HospitalDropSlot SlotMousedOver
        {
            get { return slotMousedOver; }
            private set { slotMousedOver = value; }
        }
        public bool Available
        {
            get { return myCharacterData == null; }
        }
        public HexCharacterData MyCharacterData
        {
            get { return myCharacterData; }
        }
        public TownActivity FeatureType
        {
            get { return featureType; }
        }
        #endregion

        // Input 
        #region
        void Update()
        {
            if (SlotMousedOver == this && Input.GetKeyDown(KeyCode.Mouse1))
                OnRightClick();
        }
        public void OnRightClick()
        {
            HandleCancel();
        }
        public void MouseEnter()
        {
            SlotMousedOver = this;
        }
        public void MouseExit()
        {
            SlotMousedOver = null;
        }
        public void OnCancelButtonClicked()
        {
            HandleCancel();
        }
        #endregion

        // Logic 
        #region
       
        private void HandleCancel()
        {
            if (myCharacterData != null)
            {
                var character = MyCharacterData;
                MyCharacterData.currentTownActivity = TownActivity.None;
                myCharacterData = null;
                PlayerDataController.Instance.ModifyPlayerGold(GetFeatureGoldCost(featureType));
                BuildViews();
                CharacterScrollPanelController.Instance.GetCharacterPanel(character).UpdateActivityIndicator();
            }
        }
        public void BuildViews()
        {
            if(myCharacterData != null)
            {
                portraitVisualParent.SetActive(true);
                CharacterModeller.BuildModelFromStringReferencesAsMugshot(portraitModel, myCharacterData.modelParts);
                cancelButtonParent.SetActive(true);
            }
            else
            {
                portraitVisualParent.SetActive(false);
                cancelButtonParent.SetActive(false);
            }
        }
        public void OnCharacterDragDropSuccess(HexCharacterData character)
        {
            myCharacterData = character;
            MyCharacterData.currentTownActivity = featureType;
            BuildViews();
        }
        public void ClearAndReset()
        {
            if(myCharacterData != null) MyCharacterData.currentTownActivity = TownActivity.None;
            myCharacterData = null;
            portraitVisualParent.SetActive(false);
            cancelButtonParent.SetActive(false);
        }
        #endregion
    }

    
}