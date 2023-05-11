using UnityEngine;
using TMPro;
using DG.Tweening;
using HexGameEngine.Utilities;
using UnityEngine.UI;

namespace HexGameEngine.VisualEvents
{
    public class StatusEffect : MonoBehaviour
    {
        [Header("Component References")]
        public TextMeshProUGUI statusText;
        public CanvasGroup myCg;
        public GameObject iconImageParent;
        public Image iconImage;
        public RectTransform textFitter;

        public void InitializeSetup(string statusName, Sprite sprite = null)
        {
            statusText.text = TextLogic.ReturnColoredText(statusName, TextLogic.white);
            TransformUtils.RebuildLayout(textFitter);
            iconImageParent.SetActive(false);
            if (sprite != null)
            {
                iconImageParent.SetActive(true);
                iconImage.sprite = sprite;
                float offset = iconImageParent.GetComponent<RectTransform>().rect.width * 0.6f;
                float newIconX = statusText.transform.position.x - ((statusText.GetComponent<RectTransform>().rect.width * 0.016f) * 0.5f) - offset;
                iconImageParent.transform.position = new Vector3(newIconX, iconImageParent.transform.position.y, iconImageParent.transform.position.z);
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
}