using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators.UI
{
    public class UIAttributeSlider : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI attributeAmountText;
        [SerializeField] private GameObject[] stars;
        public void Build(int statAmount, int starCount = 0)
        {
            float statFloat = statAmount;
            slider.DOKill();
            slider.DOValue(statFloat / 100f, 0.25f);
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].gameObject.SetActive(false);
                if (i < starCount)
                {
                    stars[i].gameObject.SetActive(true);
                }
            }
            attributeAmountText.text = statAmount.ToString();
        }
    }
}
