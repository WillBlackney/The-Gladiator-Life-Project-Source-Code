using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Combat;
using WeAreGladiators.Libraries;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Characters
{
    public class StressPanelView : MonoBehaviour
    {

        // Logic
        #region

        public void BuildPanelViews(HexCharacterModel character)
        {
            MoraleState stressState = character.currentMoraleState;
            Sprite stressSprite = SpriteLibrary.Instance.GetMoraleStateSprite(stressState);
            int stressMod = CombatController.Instance.GetStatMultiplierFromStressState(stressState, character);
            //int[] stressRanges = CombatController.Instance.GetStressStateRanges(stressState);
            stressStateText.text = stressState.ToString();
            stressStateIcon.sprite = stressSprite;

            // Popup
            popUpStressStateIcon.sprite = stressSprite;
            popUpHeaderText.text = stressState.ToString();

            // to do: italic description text

            string[] attributes =
            {
                "Accuracy", "Dodge", "Resolve"
            };

            for (int i = 0; i < 3; i++)
            {
                DotStyle style = DotStyle.Red;
                string modSymbol = "";
                if (stressState == MoraleState.Confident)
                {
                    style = DotStyle.Green;
                    modSymbol = "+";
                }

                dottedRows[i].Build(modSymbol + stressMod + " " + attributes[i], style);
            }

        }

        #endregion
        // Properties + Components
        #region

        [Header("Core Components")]
        [SerializeField]
        private TextMeshProUGUI stressStateText;
        [SerializeField] private Image stressStateIcon;

        [Header("Popup Components")]
        [SerializeField]
        private Image popUpStressStateIcon;
        [SerializeField] private TextMeshProUGUI popUpHeaderText;
        [SerializeField] private TextMeshProUGUI popUpDescriptionText;
        [SerializeField] private ModalDottedRow[] dottedRows;

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
    }
}
