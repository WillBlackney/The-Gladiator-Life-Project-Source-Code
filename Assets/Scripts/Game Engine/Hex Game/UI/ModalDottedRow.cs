using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HexGameEngine.Utilities;
using UnityEngine.UI;
using TMPro;

namespace HexGameEngine.UI
{
    public class ModalDottedRow : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI mainText;
        [SerializeField] Image dotImage;
        [SerializeField] RectTransform[] fitters;

        [Header("Colours")]
        [SerializeField] Color neutral;
        [SerializeField] Color red;
        [SerializeField] Color green;

        private void RebuildFitters()
        {
            for(int i = 0; i < fitters.Length; i++)            
                LayoutRebuilder.ForceRebuildLayoutImmediate(fitters[i]);            
        }
        public void Build(ModalDotRowBuildData data)
        {
            gameObject.SetActive(true);
            mainText.text = TextLogic.ConvertCustomStringListToString(data.message);
            if (data.dotStyle == DotStyle.Neutral) dotImage.color = neutral;
            else if (data.dotStyle == DotStyle.Red) dotImage.color = red;
            else if (data.dotStyle == DotStyle.Green) dotImage.color = green;
            RebuildFitters();
        }
        public void Build(string message, DotStyle style)
        {
            gameObject.SetActive(true);
            mainText.text = message;
            if (style == DotStyle.Neutral) dotImage.color = neutral;
            else if (style == DotStyle.Red) dotImage.color = red;
            else if (style == DotStyle.Green) dotImage.color = green;
            RebuildFitters();
        }
    }
}
