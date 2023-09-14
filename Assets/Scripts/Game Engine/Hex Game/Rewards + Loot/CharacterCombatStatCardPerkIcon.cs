using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeAreGladiators.Perks;

namespace WeAreGladiators.RewardSystems
{
    public class CharacterCombatStatCardPerkIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        // Misc
        #region

        public void SetMyDataReference(PerkIconData data)
        {
            perkDataRef = data;
        }

        #endregion
        // Properties + Components
        #region

        [SerializeField] private Image perkImage;
        [SerializeField] private PerkIconData perkDataRef;

        #endregion

        // Getters + Accessors
        #region

        public Image PerkImage => perkImage;
        public PerkIconData PerkDataRef => perkDataRef;

        #endregion

        // Input
        #region

        public void OnPointerEnter(PointerEventData eventData)
        {

        }

        public void OnPointerExit(PointerEventData eventData)
        {

        }

        #endregion
    }
}
