using UnityEngine;
using WeAreGladiators.Characters;
using WeAreGladiators.Player;
using WeAreGladiators.UCM;
using WeAreGladiators.UI;

namespace WeAreGladiators.TownFeatures
{
    public class HospitalDropSlot : MonoBehaviour
    {
        // Components + Properties
        #region

        [Header("Core Components")]
        [SerializeField]
        private TownActivity featureType;
        [SerializeField] private GameObject portraitVisualParent;
        [SerializeField] private UniversalCharacterModel portraitModel;
        [SerializeField] private GameObject cancelButtonParent;

        // Non inspector fields

        #endregion

        // Getters + Accessors
        #region

        public static int GetFeatureGoldCost(TownActivity feature)
        {
            // to do: probably should find a better place for this function
            // the costs of features should probably be determined by GlobalSettings
            if (feature == TownActivity.BedRest)
            {
                return 75;
            }
            if (feature == TownActivity.Therapy)
            {
                return 50;
            }
            if (feature == TownActivity.Surgery)
            {
                return 100;
            }
            return 0;
        }
        public static HospitalDropSlot SlotMousedOver { get; private set; }
        public bool Available => MyCharacterData == null;
        public HexCharacterData MyCharacterData { get; private set; }
        public TownActivity FeatureType => featureType;

        #endregion

        // Input 
        #region

        private void Update()
        {
            if (SlotMousedOver == this && Input.GetKeyDown(KeyCode.Mouse1))
            {
                OnRightClick();
            }
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
            if (MyCharacterData != null)
            {
                HexCharacterData character = MyCharacterData;
                MyCharacterData.currentTownActivity = TownActivity.None;
                MyCharacterData = null;
                PlayerDataController.Instance.ModifyPlayerGold(GetFeatureGoldCost(featureType));
                BuildViews();
            }
        }
        public void BuildViews()
        {
            if (MyCharacterData != null)
            {
                portraitVisualParent.SetActive(true);
                CharacterModeller.BuildModelFromStringReferencesAsMugshot(portraitModel, MyCharacterData.modelParts);
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
            MyCharacterData = character;
            MyCharacterData.currentTownActivity = featureType;
            BuildViews();
        }
        public void ClearAndReset()
        {
            if (MyCharacterData != null)
            {
                MyCharacterData.currentTownActivity = TownActivity.None;
            }
            MyCharacterData = null;
            portraitVisualParent.SetActive(false);
            cancelButtonParent.SetActive(false);
        }

        #endregion
    }

}
