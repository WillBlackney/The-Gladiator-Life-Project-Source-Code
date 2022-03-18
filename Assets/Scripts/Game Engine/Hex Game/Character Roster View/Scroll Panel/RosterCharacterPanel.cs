using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CardGameEngine.UCM;
using HexGameEngine.Characters;
using HexGameEngine.UCM;

namespace HexGameEngine.UI
{
    public class RosterCharacterPanel : MonoBehaviour
    {
        // Components
        #region
        [Header("Core Components")]
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] UniversalCharacterModel portaitModel;

        [Header("Health Bar Components")]
        [SerializeField] Slider healthBar;
        [SerializeField] TextMeshProUGUI  healthText;

        [Header("Stress Bar Components")]
        [SerializeField] Slider stressBar;
        [SerializeField] TextMeshProUGUI stressText;


        // Non-inspector properties
        private HexCharacterData myCharacterData;
        #endregion

        // Getters + Accessors
        #region
        public HexCharacterData MyCharacterData
        {
            get { return myCharacterData; }
        }
        #endregion

        // Logic
        #region
        public void BuildFromCharacterData(HexCharacterData data)
        {
            myCharacterData = data;

            // Texts
            nameText.text = data.myName;
            healthText.text = data.currentHealth.ToString();
            stressText.text = data.currentStress.ToString();

            // Build model
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(portaitModel, data.modelParts);
            portaitModel.SetBaseAnim();

            // Build bars
            healthBar.value = (float)((float) data.currentHealth / (float) StatCalculator.GetTotalMaxHealth(data));
            stressBar.value = (float)((float) data.currentStress / 100f);

            // TO DO: Injuries
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
        public void OnClickAndDragStart()
        {
            Debug.Log("OnClickAndDragStart()");
            PortraitDragController.Instance.OnRosterCharacterPanelDragStart(this);
        }
        #endregion
    }
}