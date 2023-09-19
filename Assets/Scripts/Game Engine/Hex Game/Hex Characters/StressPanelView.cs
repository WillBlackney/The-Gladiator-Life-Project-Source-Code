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
            stressStateText.text = stressState.ToString();
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
