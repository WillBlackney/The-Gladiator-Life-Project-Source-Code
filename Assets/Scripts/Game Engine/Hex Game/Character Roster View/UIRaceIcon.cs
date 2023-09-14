using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeAreGladiators.Characters;

namespace WeAreGladiators.UI
{

    public class UIRaceIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties + Components
        #region

        [SerializeField] private Image raceImage;
        [SerializeField] [HideInInspector] private RaceDataSO raceData;

        #endregion

        // Getters + Accessors
        #region

        public Image RaceImage => raceImage;
        public RaceDataSO RaceData => raceData;

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

        private void HideAndReset()
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
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(raceData.racialPassiveKeyWords.ToList());
                MainModalController.Instance.BuildAndShowModal(raceData);
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (raceData != null)
            {
                MainModalController.Instance.HideModal();
                KeyWordLayoutController.Instance.FadeOutMainView();
            }
        }

        #endregion
    }
}
