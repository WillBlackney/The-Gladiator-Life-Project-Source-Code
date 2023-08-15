using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using WeAreGladiators.CameraSystems;
using UnityEngine.Events;
using WeAreGladiators.UI;
using WeAreGladiators.Audio;

namespace WeAreGladiators.TownFeatures
{
    public class TownBuildingView : MonoBehaviour
    {
        [Header("World Building Components")]
        [SerializeField] Color normalColor;
        [SerializeField] Color mouseOverColor;
        [SerializeField] SpriteRenderer[] buildingSprites;
        [SerializeField] RectTransform popUpRect;
        [SerializeField] RectTransform startPos;
        [SerializeField] RectTransform endPos;
        [Space(10)]
        [SerializeField] CanvasGroup popUpCg;
        [SerializeField] GameObject outlineSprite;
        [SerializeField] RectTransform cameraZoomToPoint;
        [SerializeField] Sound entranceSound;

        private static bool blockMouseActions = false;

        [Header("Page Components")]
        [SerializeField] UnityEvent pageBuildFunction;
        [SerializeField] GameObject pageVisualParent;
        [SerializeField] RectTransform pageMovementParent;
        [SerializeField] CanvasGroup pageCg;
        [SerializeField] RectTransform pageStartPos;
        [SerializeField] RectTransform pageEndPos;

        public RectTransform PageMovementParent
        {
            get { return pageMovementParent; }
        }
        public GameObject PageVisualParent
        {
            get { return pageVisualParent; }
        }
        public RectTransform PageStartPos
        {
            get { return pageStartPos; }
        }
        public CanvasGroup PageCg
        {
            get { return pageCg; }
        }

        private IEnumerator LeaveCoroutine()
        {
            if (blockMouseActions) yield break;
            blockMouseActions = true;

            // Move canvas to start pos + setup
            pageCg.DOKill();
            pageMovementParent.DOKill();

            // Leave SFX
            AudioManager.Instance.PlaySound(Sound.UI_Door_Open);

            // Fade out screen
            pageCg.DOFade(0f, 1f);

            // Move page offscreen
            pageMovementParent.DOMove(pageStartPos.position, 0.65f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(0.65f);

            // Stop feature ambience SFX
            AudioManager.Instance.FadeOutSound(entranceSound, 0.75f);

            // Move and zoom out camera
            var c = CameraController.Instance.MainCamera;
            c.DOOrthoSize(5, 0.65f);
            c.transform.DOMove(new Vector3(0, 0, -15), 0.66f).OnComplete(() =>
            {
                blockMouseActions = false;
                pageVisualParent.SetActive(false);
            });
        }
        public void OnLeaveFeatureButtonClicked()
        {
            StartCoroutine(LeaveCoroutine());
        }     
        public void SnapToArenaViewSettings()
        {
            // Move canvas to start pos + setup
            pageCg.DOKill();
            pageMovementParent.DOKill();
            pageCg.alpha = 1f;
            pageVisualParent.SetActive(true);
            pageBuildFunction.Invoke();
            pageMovementParent.position = pageEndPos.position;
            CameraController.Instance.MainCamera.DOOrthoSize(2.5f, 0f);
            CameraController.Instance.MainCamera.transform.position = new Vector3(cameraZoomToPoint.position.x, cameraZoomToPoint.position.y, -15);
        }
        public void CloseAndResetAllUiViews()
        {
            blockMouseActions = false;
            pageCg.DOKill();
            pageMovementParent.DOKill();
            pageCg.DOFade(0f, 0f);
            pageMovementParent.DOMove(pageStartPos.position, 0.35f);
            pageVisualParent.SetActive(false);
        }


        #region Input Events

        public void MouseEnter()
        {
            if (blockMouseActions || TownController.Instance.AnyFeaturePageIsActive) return;
            CursorController.Instance.SetCursor(CursorType.Enter_Door);

            foreach (SpriteRenderer i in buildingSprites)
                i.color = mouseOverColor;

            popUpRect.DOKill();
            popUpCg.DOKill();

            outlineSprite.SetActive(true);
            popUpRect.DOMove(endPos.position, 0.25f);
            popUpCg.DOFade(1f, 0.15f);
        }
        public void MouseExit()
        {
            if (blockMouseActions || TownController.Instance.AnyFeaturePageIsActive) return;
            CursorController.Instance.SetCursor(CursorType.NormalPointer);

            foreach (SpriteRenderer i in buildingSprites)
                i.color = normalColor;

            popUpRect.DOKill();
            popUpCg.DOKill();

            outlineSprite.SetActive(false);
            popUpRect.DOMove(startPos.position, 0.25f);
            popUpCg.DOFade(0f, 0.15f);
        }
        public void MouseClick()
        {
            StartCoroutine(ClickCoroutine());
        }
        private IEnumerator ClickCoroutine()
        {
            yield return null;
            if (blockMouseActions || TownController.Instance.AnyFeaturePageIsActive) yield break;
            blockMouseActions = true;
            CursorController.Instance.SetCursor(CursorType.NormalPointer);
            foreach (SpriteRenderer i in buildingSprites)
                i.color = normalColor;

            popUpRect.DOKill();
            popUpCg.DOKill();

            outlineSprite.SetActive(false);
            popUpRect.DOMove(startPos.position, 0.25f);
            popUpCg.DOFade(0f, 0.15f);

            // Move canvas to start pos + setup
            pageCg.DOKill();
            pageMovementParent.DOKill();
            pageCg.alpha = 0.001f;
            pageVisualParent.SetActive(true);
            pageBuildFunction.Invoke();
            pageMovementParent.position = pageStartPos.position;

            // Play entrance sounds
            AudioManager.Instance.PlaySound(Sound.UI_Door_Open);

            // Fade in screen + SFX
            pageCg.DOFade(1f, 0.5f);

            // Move + zoom camera towards building
            var cam = CameraController.Instance.MainCamera;
            cam.DOOrthoSize(2.5f, 0.65f).SetEase(Ease.OutCubic);
            cam.transform.DOMove(new Vector3(cameraZoomToPoint.position.x, cameraZoomToPoint.position.y, -15), 0.5f).SetEase(Ease.OutCubic);
            yield return new WaitForSeconds(0.66f);

            // Feature specific entrance SFX
            AudioManager.Instance.PlaySound(entranceSound);

            // Move page to centre
            pageMovementParent.DOMove(pageEndPos.position, 0.65f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => blockMouseActions = false);
        }
        #endregion
    }
}