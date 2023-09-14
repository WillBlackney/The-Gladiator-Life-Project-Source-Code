using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators.StoryEvents
{
    public class StoryEventResultItemRow : MonoBehaviour
    {
        [Header("Core Components")]
        [SerializeField]
        private GameObject visualParent;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Image unframedIconImage;

        [Space(10)]
        [Header("Framed Image Components")]
        [SerializeField]
        private Image framedIconImage;
        [SerializeField] private GameObject framedIconImageParent;

        [Space(10)]
        [Header("Sprites")]
        [SerializeField]
        private Sprite star;
        [SerializeField] private Sprite skull;
        [SerializeField] private Sprite goldCoins;
        public void Hide()
        {
            visualParent.SetActive(false);
            unframedIconImage.gameObject.SetActive(false);
            framedIconImageParent.gameObject.SetActive(false);
        }
        public void BuildAndShow(StoryEventResultItem data)
        {
            visualParent.SetActive(true);
            messageText.text = data.message;
            // to do: set icon sprite

            if (data.iconType == ResultRowIcon.FramedSprite && data.iconSprite != null)
            {
                framedIconImageParent.SetActive(true);
                framedIconImage.sprite = data.iconSprite;
            }
            else if (data.iconType == ResultRowIcon.UnframedSprite && data.iconSprite != null)
            {
                unframedIconImage.gameObject.SetActive(true);
                unframedIconImage.sprite = data.iconSprite;
            }
            else if (data.iconType == ResultRowIcon.Skull)
            {
                unframedIconImage.gameObject.SetActive(true);
                unframedIconImage.sprite = skull;
            }
            else if (data.iconType == ResultRowIcon.Star)
            {
                unframedIconImage.gameObject.SetActive(true);
                unframedIconImage.sprite = star;
            }
            else if (data.iconType == ResultRowIcon.GoldCoins)
            {
                unframedIconImage.gameObject.SetActive(true);
                unframedIconImage.sprite = goldCoins;
            }
        }
    }

    public enum ResultRowIcon
    {
        None = 0,
        FramedSprite = 1,
        UnframedSprite = 2,
        Skull = 3,
        UserIcon = 4,
        Star = 5,
        GoldCoins = 7
    }
}
