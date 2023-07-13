using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeAreGladiators.Combat;
using WeAreGladiators.Libraries;
using WeAreGladiators.Utilities;
using WeAreGladiators.UI;

namespace WeAreGladiators.Characters
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
        [SerializeField] ModalDottedRow[] dottedRows;
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
            popUpHeaderText.text = stressState.ToString() +
               TextLogic.ReturnColoredText(" (" + stressRanges[0].ToString() + " - " + stressRanges[1].ToString() + ")", TextLogic.neutralYellow);

            // to do: italic description text

            string[] attributes = { "Accuracy", "Dodge", "Resolve" };

            for(int i = 0; i < 3; i++)
            {
                DotStyle style = DotStyle.Red;
                string modSymbol = "";
                if (stressState == StressState.Confident) 
                {
                    style = DotStyle.Green;
                    modSymbol = "+";
                }

                dottedRows[i].Build(modSymbol + stressMod.ToString() + " " + attributes[i], style);
            }
           

        }
        #endregion
    }
}
