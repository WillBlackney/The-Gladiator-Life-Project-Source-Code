using UnityEngine;

namespace WeAreGladiators.StoryEvents
{
    public class StoryEventResultItem
    {
        public Sprite iconSprite;
        public ResultRowIcon iconType;
        public string message;

        public StoryEventResultItem(string message, ResultRowIcon iconType, Sprite iconSprite = null)
        {
            this.message = message;
            this.iconType = iconType;
            this.iconSprite = iconSprite;
        }
    }

}
