using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class ModalDottedRow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mainText;
        [SerializeField] private Image dotImage;
        [SerializeField] private RectTransform[] fitters;

        [Header("Colours")]
        [SerializeField]
        private Color neutral;
        [SerializeField] private Color red;
        [SerializeField] private Color green;

        public void Build(ModalDotRowBuildData data)
        {
            gameObject.SetActive(true);
            mainText.text = TextLogic.ConvertCustomStringListToString(data.message);
            if (data.dotStyle == DotStyle.Neutral)
            {
                dotImage.color = neutral;
            }
            else if (data.dotStyle == DotStyle.Red)
            {
                dotImage.color = red;
            }
            else if (data.dotStyle == DotStyle.Green)
            {
                dotImage.color = green;
            }
            TransformUtils.RebuildLayouts(fitters);
        }
        public void Build(string message, DotStyle style)
        {
            gameObject.SetActive(true);
            mainText.text = message;
            if (style == DotStyle.Neutral)
            {
                dotImage.color = neutral;
            }
            else if (style == DotStyle.Red)
            {
                dotImage.color = red;
            }
            else if (style == DotStyle.Green)
            {
                dotImage.color = green;
            }
            TransformUtils.RebuildLayouts(fitters);
        }
    }
}
