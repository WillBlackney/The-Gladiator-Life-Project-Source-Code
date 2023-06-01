using Sirenix.OdinInspector;
using UnityEngine;

namespace HexGameEngine.StoryEvents
{
    [CreateAssetMenu(fileName = "New StoryEventPageSO", menuName = "StoryEventPage", order = 52)]
    public class StoryEventPageSO : ScriptableObject
    {
        [TextArea(20, 20)]
        public string pageDescription;
        public StoryEventChoiceSO[] allChoices;
        [PreviewField(75)]
        public Sprite pageSprite;

    }
}