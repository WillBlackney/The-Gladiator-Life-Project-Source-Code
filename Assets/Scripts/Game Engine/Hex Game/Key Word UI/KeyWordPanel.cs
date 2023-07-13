using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WeAreGladiators.UI
{
    public class KeyWordPanel : MonoBehaviour
    {
        [Header("Text Components")]
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;

        [Header("Framed Image Components")]
        public Image framedImage;
        public GameObject framedImageParent;

        [Header("Unframed Image Components")]
        public Image unframedImage;
        public GameObject unframedImageParent;

        [Header("Transform + Layout Components")]
        public RectTransform rootLayoutRect;

    }
}