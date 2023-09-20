using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.VisualEvents
{
    public class StatusEffect : MonoBehaviour
    {
        [Header("Core Components")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private CanvasGroup myCg;
        [SerializeField] private RectTransform textFitter;

        [Header("Circular Icon Components")]
        [SerializeField] private GameObject circleIconParent;
        [SerializeField] private Image circleIconFrame;
        [SerializeField] private Image circleIconImage;

        [Header("Square Icon Components")]
        [SerializeField] private GameObject squareIconParent;
        [SerializeField] private Image squareIconImage;

        [Header("Frameless Icon Components")]
        [SerializeField] private Image framelessIconImage;

        [Header("Sprites")]
        [SerializeField]
        private Sprite brownCircleFrame;
        [SerializeField] private Sprite redCircleFrame;

        public void InitializeSetup(string statusName, Sprite sprite = null, StatusFrameType frameType = StatusFrameType.NoImageOrFrame)
        {
            statusText.text = TextLogic.ReturnColoredText(statusName, TextLogic.white);
            TransformUtils.RebuildLayout(textFitter);
            circleIconParent.SetActive(false);
            squareIconParent.SetActive(false);
            framelessIconImage.gameObject.SetActive(false);

            if (sprite != null)
            {
                float textScaleLocal = 0.016f;
                if (frameType == StatusFrameType.CircularBrown || frameType == StatusFrameType.CircularRed)
                {
                    circleIconParent.SetActive(true);
                    circleIconImage.sprite = sprite;
                    circleIconFrame.sprite = brownCircleFrame;
                    if (frameType == StatusFrameType.CircularRed)
                    {
                        circleIconFrame.sprite = redCircleFrame;
                    }
                    float iconWidth = circleIconParent.GetComponent<RectTransform>().rect.width;
                    float recenteringOffset = iconWidth * 0.5f;
                    float iconOffset = iconWidth * 0.75f;
                    float newIconX = statusText.transform.position.x - statusText.GetComponent<RectTransform>().rect.width * textScaleLocal * 0.5f - iconOffset + recenteringOffset;
                    circleIconParent.transform.position = new Vector3(newIconX, circleIconParent.transform.position.y, circleIconParent.transform.position.z);
                    statusText.transform.position = new Vector3(statusText.transform.position.x + recenteringOffset, statusText.transform.position.y, statusText.transform.position.z);

                }
                else if (frameType == StatusFrameType.SquareBrown)
                {
                    squareIconParent.SetActive(true);
                    squareIconImage.sprite = sprite;
                    float iconWidth = squareIconParent.GetComponent<RectTransform>().rect.width;
                    float recenteringOffset = iconWidth * 0.5f;
                    float iconOffset = iconWidth * 0.75f;
                    float newIconX = statusText.transform.position.x - statusText.GetComponent<RectTransform>().rect.width * textScaleLocal * 0.5f - iconOffset + recenteringOffset;
                    squareIconParent.transform.position = new Vector3(newIconX, squareIconParent.transform.position.y, squareIconParent.transform.position.z);
                    statusText.transform.position = new Vector3(statusText.transform.position.x + recenteringOffset, statusText.transform.position.y, statusText.transform.position.z);

                }
                else if (frameType == StatusFrameType.ImageWithoutFrame)
                {
                    framelessIconImage.gameObject.SetActive(true);
                    framelessIconImage.sprite = sprite;
                    float iconWidth = framelessIconImage.GetComponent<RectTransform>().rect.width;
                    float recenteringOffset = iconWidth * 0.5f;
                    float iconOffset = iconWidth * 0.75f;
                    float newIconX = statusText.transform.position.x - statusText.GetComponent<RectTransform>().rect.width * textScaleLocal * 0.5f - iconOffset + recenteringOffset;
                    framelessIconImage.transform.position = new Vector3(newIconX, framelessIconImage.transform.position.y, framelessIconImage.transform.position.z);
                    statusText.transform.position = new Vector3(statusText.transform.position.x + recenteringOffset, statusText.transform.position.y, statusText.transform.position.z);

                }
            }
            PlayAnimation();
        }
        public void DestroyThis()
        {
            Destroy(gameObject);
        }
        public void PlayAnimation()
        {
            myCg.alpha = 0;
            myCg.DOFade(1, 0.5f);
            transform.DOLocalMoveY(transform.localPosition.y + 1.25f, 1.5f);

            Sequence s1 = DOTween.Sequence();
            s1.Append(transform.DOScale(new Vector2(1.1f, 1.1f), 1));
            s1.OnComplete(() =>
            {
                myCg.DOFade(0, 0.5f).OnComplete(() => DestroyThis());
            });

        }
    }

    public enum StatusFrameType
    {
        NoImageOrFrame = 0,
        ImageWithoutFrame = 4,
        CircularBrown = 1,
        CircularRed = 2,
        SquareBrown = 3
    }
}
