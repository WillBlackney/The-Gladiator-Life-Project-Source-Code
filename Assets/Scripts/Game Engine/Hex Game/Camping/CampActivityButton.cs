using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using HexGameEngine.Utilities;

namespace HexGameEngine.Camping
{
    public class CampActivityButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        // Components + Variables
        #region
        [Header("Core Components")]
        [SerializeField] private Image activityImage;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Image selectedGlowOutline;
        [Header("Pop Up Components")]
        [SerializeField] private CanvasGroup infoPopupCg;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [HideInInspector] public CampActivityDataSO myActivityData;
        #endregion

        // Getters + Accessors
        #region
        public Image ActivityImage { get { return activityImage; } }
        public TextMeshProUGUI CostText { get { return costText; } }
        #endregion

        // Input Listeners
        #region
        public void OnPointerEnter(PointerEventData eventData)
        {
            infoPopupCg.DOKill();
            infoPopupCg.gameObject.SetActive(true);           
            infoPopupCg.alpha = 0;
            infoPopupCg.DOFade(1f, 0.25f);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            infoPopupCg.DOKill();
            infoPopupCg.gameObject.SetActive(false);           
            infoPopupCg.alpha = 0;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            CampSiteController.Instance.OnActivityButtonClicked(this);
        }
        #endregion

        // Misc
        #region
        public void PlayGlowAnimation()
        {
            Debug.Log("CampActivityButton.PlayGlowAnimation() called...");
            selectedGlowOutline.gameObject.SetActive(true);
            selectedGlowOutline.DOKill();
            selectedGlowOutline.DOFade(0f, 0f);
            selectedGlowOutline.DOFade(1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        public void StopGlowAnimation()
        {
            Debug.Log("CampActivityButton.StopGlowAnimation() called...");
            selectedGlowOutline.DOKill();
            selectedGlowOutline.DOFade(0f, 0f);
            selectedGlowOutline.gameObject.SetActive(false);
        }
        public void BuildPopupWindow(CampActivityDataSO activityData)
        {
            nameText.text = activityData.name;
            descriptionText.text = "";
            foreach(CustomString cs in activityData.dynamicDescription)
                descriptionText.text += TextLogic.ConvertCustomStringToString(cs);
        }
        #endregion




    }
}