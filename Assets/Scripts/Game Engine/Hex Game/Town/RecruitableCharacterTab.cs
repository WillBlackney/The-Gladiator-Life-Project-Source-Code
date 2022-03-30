using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CardGameEngine.UCM;
using HexGameEngine.Characters;
using HexGameEngine.UCM;
using HexGameEngine.Libraries;
using HexGameEngine.Player;
using HexGameEngine.Utilities;

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

        [Space(20)]

        [Header("Race Section Components")]
        [SerializeField] TextMeshProUGUI racialText;
        [SerializeField] Image racialImage;

        [Space(20)]

        [Header("Cost Section Components")]
        [SerializeField] TextMeshProUGUI recruitCostText;
        [SerializeField] TextMeshProUGUI upkeepCostText;

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
            nameText.text = data.myName;
            racialText.text = data.race.ToString();
            racialImage.sprite = SpriteLibrary.Instance.GetRacialSprite(data.race);

            // Build model
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(portaitModel, data.modelParts);
            portaitModel.SetBaseAnim();

            // Wage and recruit costs
            string col = "<color=#FFFFFF>";
            if (PlayerDataController.Instance.CurrentGold < data.recruitCost) col = TextLogic.lightRed;
            upkeepCostText.text = data.dailyWage.ToString();
            recruitCostText.text = TextLogic.ReturnColoredText(data.recruitCost.ToString(),col);
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