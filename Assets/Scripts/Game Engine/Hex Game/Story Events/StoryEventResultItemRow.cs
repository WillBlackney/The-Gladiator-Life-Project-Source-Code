using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexGameEngine.StoryEvents
{
    public class StoryEventResultItemRow : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] GameObject visualParent;
        [SerializeField] TextMeshProUGUI messageText;
        [SerializeField] Image unframedIconImage;

        [Space(10)]
        [SerializeField] Image framedIconImage;
        [SerializeField] GameObject framedIconImageParent;
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

            if(data.iconType == ResultRowIcon.FramedSprite && data.iconSprite != null)
            {
                framedIconImageParent.SetActive(true);
                framedIconImage.sprite = data.iconSprite;
            }
            else if (data.iconType == ResultRowIcon.UnframedSprite && data.iconSprite != null)
            {
                unframedIconImage.gameObject.SetActive(true);
                unframedIconImage.sprite = data.iconSprite;
            }
            

        }
    }

    public enum ResultRowIcon
    {
        None = 0,
        FramedSprite = 1,
        UnframedSprite = 2,
        Skull = 3,
        RedPlus = 4,
        UserIcon = 5,
        Star = 6,

        // XP, character leaving, attribute boosts, healing, stress modified, character died


    }
}