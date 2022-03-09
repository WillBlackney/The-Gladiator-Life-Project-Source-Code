using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CardGameEngine.UCM;
using HexGameEngine.Characters;
using HexGameEngine.UCM;
using HexGameEngine.Libraries;

namespace HexGameEngine.TownFeatures
{
    public class RecruitableCharacterTab : MonoBehaviour
    {
        // Components
        #region
        [Header("Core Components")]
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] UniversalCharacterModel portaitModel;

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

        // Logic
        #region
        public void BuildFromCharacterData(HexCharacterData data)
        {
            myCharacterData = data;

            // Texts
            nameText.text = data.myName;
            racialText.text = data.race.ToString();
            racialImage.sprite = SpriteLibrary.Instance.GetRacialSpriteFromEnum(data.race);

            // Build model
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(portaitModel, data.modelParts);
            portaitModel.SetBaseAnim();

            // TO DO: upkeep and recruit costs
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void ResetAndHide()
        {
            myCharacterData = null;
            gameObject.SetActive(false);
        }
        #endregion
    }
}