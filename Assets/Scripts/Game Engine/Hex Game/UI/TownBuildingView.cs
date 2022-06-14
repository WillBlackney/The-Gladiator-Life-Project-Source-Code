using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using HexGameEngine.CameraSystems;

namespace HexGameEngine.TownFeatures
{
    public class TownBuildingView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
        [SerializeField] RectTransform cameraZoomToPoint;

        private static bool blockMouseActions = false;
        private static bool rightClicked = false;

        void Update()
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                rightClicked = true;
            }
            else rightClicked = false;
        }



        public void OnPointerClick(PointerEventData eventData)
        {
            if (blockMouseActions) return;
            blockMouseActions = true;

            if (rightClicked)
            {
                Debug.Log("Move camera away");
                var c = CameraController.Instance.MainCamera;
                c.DOOrthoSize(5, 0.75f);
                Sequence seq = DOTween.Sequence();               
                seq.Append(c.transform.DOMove(new Vector3(0, 0, -15), 0.75f));                
                seq.OnComplete(() => { blockMouseActions = false; });
                return;
            }

            foreach (Image i in buildingImages)
                i.color = normalColor;

            outlineCg.DOKill();
            popUpRect.DOKill();
            popUpCg.DOKill();

            outlineCg.DOFade(0, 0.1f);
            popUpRect.DOMove(startPos.position, 0.25f);
            popUpCg.DOFade(0f, 0.15f);

            var cam = CameraController.Instance.MainCamera;
            cam.DOOrthoSize(2.5f, 0.75f);
            Sequence s = DOTween.Sequence();
            s.Append(cam.transform.DOMove(new Vector3(cameraZoomToPoint.position.x, cameraZoomToPoint.position.y, -15), 0.75f));           
            s.OnComplete(() => { blockMouseActions = false; });
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (blockMouseActions) return; 

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
            if (blockMouseActions) return;

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