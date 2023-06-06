using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using HexGameEngine.Characters;
using HexGameEngine.UCM;
using HexGameEngine.Libraries;
using HexGameEngine.Player;
using HexGameEngine.Utilities;
using HexGameEngine.UI;

namespace HexGameEngine.TownFeatures
{
    public class RecruitableCharacterTab : MonoBehaviour
    {
        // Components
        #region
        [Header("Core Components")]
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] UniversalCharacterModel portaitModel;
        [SerializeField] GameObject selectedParent;

        [Space(10)]

        [Header("Race Section Components")]
        [SerializeField] TextMeshProUGUI racialText;
        [SerializeField] UIRaceIcon racialIcon;

        [Space(10)]

        [Header("Background Section Components")]
        [SerializeField] TextMeshProUGUI backgroundText;
        [SerializeField] UIBackgroundIcon backgroundIcon;

        [Space(10)]

        [Header("Cost Section Components")]
        [SerializeField] TextMeshProUGUI recruitCostText;
        [SerializeField] TextMeshProUGUI upkeepCostText;

        [Space(10)]

        [Header("Misc Components")]
        [SerializeField] TextMeshProUGUI levelText;

        // Non-inspector properties
        private HexCharacterData myCharacterData;
        #endregion

        // Getters + Accesors
        #region
        public HexCharacterData MyCharacterData
        {
            get { return myCharacterData; }
        }
        public GameObject SelectedParent
        {
            get { return selectedParent; }
        }
        #endregion

        // Logic
        #region
        public void BuildFromCharacterData(HexCharacterData data)
        {
            Show();
            myCharacterData = data;

            // Texts
            levelText.text = data.currentLevel.ToString();
            nameText.text = "<color=#BC8252>" + data.myName + "<color=#DDC6AB>    " + data.mySubName;
            if(racialText != null) racialText.text = data.race.ToString();
            racialIcon.BuildFromRacialData(CharacterDataController.Instance.GetRaceData(data.race));
            if (backgroundText != null) backgroundText.text = TextLogic.SplitByCapitals(data.background.backgroundType.ToString());
            backgroundIcon.BuildFromBackgroundData(data.background);

            // Build model
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(portaitModel, data.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(data.itemSet, portaitModel);
            portaitModel.SetBaseAnim();

            // Wage and recruit costs
            int cost = CharacterDataController.Instance.GetCharacterInitialHiringCost(data);
            string col = "<color=#DDC6AB>";
            if (PlayerDataController.Instance.CurrentGold < cost) col = TextLogic.lightRed;
            upkeepCostText.text = data.dailyWage.ToString();
            recruitCostText.text = TextLogic.ReturnColoredText(cost.ToString(),col);
        }
        private void Show()
        {
            gameObject.SetActive(true);
        }
        public void ResetAndHide()
        {
            myCharacterData = null;
            gameObject.SetActive(false);
            selectedParent.SetActive(false);
        }
        #endregion

        // Input
        #region
        public void OnClick()
        {
            TownController.Instance.OnCharacterRecruitTabClicked(this);
        }
        #endregion
    }
}