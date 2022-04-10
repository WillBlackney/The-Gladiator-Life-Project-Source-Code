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
        private ActivePerk activePerk;
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
        public ActivePerk ActivePerk
        {
            get { return activePerk; }
        }


        #endregion

        // Logic
        #region
        public void SetMyDataReference(PerkIconData data)
        {
            perkDataRef = data;
        }
        public void BuildFromActivePerk(ActivePerk p)
        {
            activePerk = p;
            SetMyDataReference(p.Data);
            gameObject.SetActive(true);
            PerkImage.sprite = p.Data.passiveSprite;
        }

        public void HideAndReset()
        {
            gameObject.SetActive(false);
            perkDataRef = null;
            activePerk = null;
        }
        #endregion

        // Input
        #region
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(activePerk != null)            
                MainModalController.Instance.BuildAndShowModal(activePerk);            
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            MainModalController.Instance.HideModal();            
        }
        #endregion

        
    }
}