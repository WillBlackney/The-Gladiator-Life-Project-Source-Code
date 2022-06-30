using HexGameEngine.Perks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HexGameEngine.Characters;

namespace HexGameEngine.UI
{

    public class UIBackgroundIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties + Components
        #region
        [SerializeField] private Image backgroundImage;
        private BackgroundData backgroundData;
        #endregion

        // Getters + Accessors
        #region
        public Image BackgroundImage
        {
            get { return backgroundImage; }
        }
        public BackgroundData BackgroundData
        {
            get { return backgroundData; }
        }


        #endregion

        // Logic
        #region      
        public void BuildFromBackgroundData(BackgroundData p)
        {
            HideAndReset();
            backgroundData = p;
            gameObject.SetActive(true);
            backgroundImage.sprite = p.BackgroundSprite;
        }

        public void HideAndReset()
        {
            gameObject.SetActive(false);
            backgroundData = null;
        }
        #endregion

        // Input
        #region
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (backgroundData != null)
                MainModalController.Instance.BuildAndShowModal(backgroundData);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            MainModalController.Instance.HideModal();
        }
        #endregion


    }
}
