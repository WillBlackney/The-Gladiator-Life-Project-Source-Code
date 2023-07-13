using WeAreGladiators.Perks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WeAreGladiators.Characters;

namespace WeAreGladiators.UI
{

    public class UIRaceIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties + Components
        #region
        [SerializeField] private Image raceImage;
        private RaceDataSO raceData;
        #endregion

        // Getters + Accessors
        #region
        public Image RaceImage
        {
            get { return raceImage; }
        }
        public RaceDataSO RaceData
        {
            get { return raceData; }
        }


        #endregion

        // Logic
        #region      
        public void BuildFromRacialData(RaceDataSO p)
        {
            HideAndReset();
            raceData = p;
            gameObject.SetActive(true);
            raceImage.sprite = p.racialSprite;
        }

        public void HideAndReset()
        {
            gameObject.SetActive(false);
            raceData = null;
        }
        #endregion

        // Input
        #region
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (raceData != null)
                MainModalController.Instance.BuildAndShowModal(raceData);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            MainModalController.Instance.HideModal();
        }
        #endregion


    }
}
