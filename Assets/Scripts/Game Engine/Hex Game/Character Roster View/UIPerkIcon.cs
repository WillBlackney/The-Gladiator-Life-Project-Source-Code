using HexGameEngine.Perks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HexGameEngine.UI
{

    public class UIPerkIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

        // Logic
        #region
        public void SetMyDataReference(PerkIconData data)
        {
            perkDataRef = data;
        }
        public void BuildFromPerkData(ActivePerk p)
        {
            //PerkIconData data = PerkController.Instance.GetPerkIconDataByTag(p.perkTag);
            PerkIconData data = p.Data;
            SetMyDataReference(data);
            gameObject.SetActive(true);
            PerkImage.sprite = data.passiveSprite;
        }
        public void HideAndReset()
        {
            gameObject.SetActive(false);
            perkDataRef = null;
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

        
    }
}