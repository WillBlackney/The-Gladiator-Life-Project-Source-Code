using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.UCM;
using TMPro;
using UnityEngine.UI;
using WeAreGladiators.Perks;
using UnityEngine.EventSystems;

namespace WeAreGladiators.RewardSystems
{
    public class CharacterCombatStatCardPerkIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties + Components
        #region
        [SerializeField] private Image perkImage;
        [SerializeField] private PerkIconData perkDataRef;
        #endregion

        // Getters + Accessors
        #region
        public Image PerkImage
        {
            get { return perkImage; }
        }
        public PerkIconData PerkDataRef
        {
            get { return perkDataRef; }
        }


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

        // Misc
        #region
        public void SetMyDataReference(PerkIconData data)
        {
            perkDataRef = data;
        }
        #endregion
    }
}