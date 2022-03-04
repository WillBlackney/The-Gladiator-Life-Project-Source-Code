using HexGameEngine.Perks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HexGameEngine.UI
{

    public class UIPerkButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties + Components
        #region
        [SerializeField] private Image perkImage;
        [SerializeField] private PerkIconData perkDataRef;
        [SerializeField] private PopupPositon popupPositon;
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
        public PopupPositon PopupPositon
        {
            get { return popupPositon; }
        }


        #endregion

        // Input
        #region
        public void OnPointerEnter(PointerEventData eventData)
        {
            UIController.Instance.OnPerkButtonMouseOver(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UIController.Instance.OnPerkButtonMouseExit();
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