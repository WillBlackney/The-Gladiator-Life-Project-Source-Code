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
        [SerializeField] Image iconImage;

        public void Hide()
        {
            visualParent.SetActive(false);
        }
        public void BuildAndShow(StoryEventResultItem data)
        {
            visualParent.SetActive(true);
            messageText.text = data.message;
            // to do: set icon sprite
        }
    }
}