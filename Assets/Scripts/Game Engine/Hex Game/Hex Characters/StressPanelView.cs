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
            MoraleState moraleState = character.currentMoraleState;
            if (character.currentMoraleState == MoraleState.None) moraleState = MoraleState.Steady;
            Sprite stressSprite = SpriteLibrary.Instance.GetMoraleStateSprite(moraleState);
            stressStateText.text = moraleState.ToString();
            stressStateIcon.sprite = stressSprite;
        }

        #endregion
        // Properties + Components
        #region

        [Header("Core Components")]
        [SerializeField] private TextMeshProUGUI stressStateText;
        [SerializeField] private Image stressStateIcon;

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
