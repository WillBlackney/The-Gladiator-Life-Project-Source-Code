using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardGameEngine.UCM;
using HexGameEngine.Characters;
using HexGameEngine.UCM;
using HexGameEngine.UI;

namespace HexGameEngine.TownFeatures
{
    public class HospitalDropSlot : MonoBehaviour
    {
        // Components + Properties
        #region
        [Header("Core Components")]
        [SerializeField] HospitalFeature featureType;
        [SerializeField] GameObject portraitVisualParent;
        [SerializeField] UniversalCharacterModel portraitModel;

        // Non inspector fields
        private static HospitalDropSlot slotMousedOver;
        private HexCharacterData myCharacterData;
        #endregion

        // Getters + Accessors
        #region
        public static int GetFeatureGoldCost(HospitalFeature feature)
        {
            // to do: probably should find a better place for this function
            // the costs of features should probably be determined by GlobalSettings

            return 50;
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
        public HospitalFeature FeatureType
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
            Debug.Log("OnRightClick");
            if (myCharacterData != null)
            {
                myCharacterData = null;
                portraitVisualParent.SetActive(false);
                // todo: update character roster panel view (remove unavailble indicator)

                // refund gold (maybe make this all a function called "HandleRemoveCharacterFromSlot()"
            }
        }
        public void MouseEnter()
        {
            Debug.Log("MouseEnter");
            SlotMousedOver = this;
        }
        public void MouseExit()
        {
            Debug.Log("MouseExit");
            SlotMousedOver = null;
        }
        #endregion

        // Logic 
        #region
        public void BuildViews()
        {
            if(myCharacterData != null)
            {
                portraitVisualParent.SetActive(true);
                CharacterModeller.BuildModelFromStringReferencesAsMugshot(portraitModel, myCharacterData.modelParts);
            }
            else
            {
                portraitVisualParent.SetActive(false);
            }
        }
        #endregion
    }

    public enum HospitalFeature
    {
        None = 0,
        BedRest = 1,
        Surgery = 2,
        Therapy = 3,
    }
}