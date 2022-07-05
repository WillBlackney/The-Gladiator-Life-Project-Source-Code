using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HexGameEngine.Combat;
using HexGameEngine.Libraries;
using HexGameEngine.Utilities;

namespace HexGameEngine.Characters
{
    public class StressPanelView : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] TextMeshProUGUI stressStateText;
        [SerializeField] Image stressStateIcon;

        [Header("Popup Components")]
        [SerializeField] Image popUpStressStateIcon;
        [SerializeField] TextMeshProUGUI popUpHeaderText;
        [SerializeField] TextMeshProUGUI popUpDescriptionText;
        #endregion


        // Input
        #region
        private void OnMouseEnter()
        {
            // fade in pop up panel after delay
        }
        private void OnMouseExit()
        {
            // fade out pop up panel
        }
        #endregion

        // Logic
        #region
        public void BuildPanelViews(HexCharacterModel character)
        {
            StressState stressState = CombatController.Instance.GetStressStateFromStressAmount(character.currentStress);
            Sprite stressSprite = SpriteLibrary.Instance.GetStressStateSprite(stressState);
            int stressMod = CombatController.Instance.GetStatMultiplierFromStressState(stressState, character);
            int[] stressRanges = CombatController.Instance.GetStressStateRanges(stressState);
            stressStateText.text = stressState.ToString();
            stressStateIcon.sprite = stressSprite;

            // Popup
            popUpStressStateIcon.sprite = stressSprite;
            popUpHeaderText.text = TextLogic.ReturnColoredText(stressState.ToString(), TextLogic.neutralYellow) + " (" +
                stressRanges[0].ToString() + " - " + stressRanges[1].ToString() + ")";

            string modSymbol = "";
            if (stressState == StressState.Confident) modSymbol = "+";

            popUpDescriptionText.text = modSymbol + stressMod.ToString() + " " + "Accuracy \n" +
                modSymbol + stressMod.ToString() + " " + "Dodge \n" +
                modSymbol + stressMod.ToString() + " " + "Resolve";

        }
        #endregion
    }
}
