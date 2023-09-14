using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
