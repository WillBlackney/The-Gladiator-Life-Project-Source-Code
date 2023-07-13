using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.StoryEvents
{
    public class StoryEventResultItem
    {
        public string message;
        public ResultRowIcon iconType;
        public Sprite iconSprite;

        public StoryEventResultItem(string message, ResultRowIcon iconType, Sprite iconSprite = null)
        {
            this.message = message;
            this.iconType = iconType;
            this.iconSprite = iconSprite;
        }
    }

}