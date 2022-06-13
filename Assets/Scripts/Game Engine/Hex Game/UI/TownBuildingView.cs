using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace HexGameEngine.TownFeatures
{
    public class TownBuildingView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Color normalColor;
        [SerializeField] Color mouseOverColor;
        [SerializeField] Image[] buildingImages;
        [SerializeField] RectTransform popUpRect;
        [SerializeField] RectTransform startPos;
        [SerializeField] RectTransform endPos;
        [Space(10)]
        [SerializeField] CanvasGroup popUpCg;       
        [SerializeField] CanvasGroup outlineCg;

        public void OnPointerEnter(PointerEventData eventData)
        {
            
            foreach(Image i in buildingImages)            
                i.color = mouseOverColor;

            
            outlineCg.DOKill();
            popUpRect.DOKill();
            popUpCg.DOKill();

            outlineCg.DOFade(1, 0.1f);
            popUpRect.DOMove(endPos.position, 0.25f);
            popUpCg.DOFade(1f, 0.15f);
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            foreach (Image i in buildingImages)
                i.color = normalColor;

            outlineCg.DOKill();
            popUpRect.DOKill();
            popUpCg.DOKill();

            outlineCg.DOFade(0, 0.1f);
            popUpRect.DOMove(startPos.position, 0.25f);
            popUpCg.DOFade(0f, 0.15f);
        }
    }
}