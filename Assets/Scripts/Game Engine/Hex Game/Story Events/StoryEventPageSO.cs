using Sirenix.OdinInspector;
using UnityEngine;

namespace WeAreGladiators.StoryEvents
{
    [CreateAssetMenu(fileName = "New StoryEventPageSO", menuName = "StoryEventPage", order = 52)]
    public class StoryEventPageSO : ScriptableObject
    {
        [Header("Display Settings")]
        [PreviewField(75)]
        public Sprite pageSprite;
        [TextArea(20, 20)]
        public string pageDescription;

        [Space(10)]

        [Header("Choices + Effects")]
        public StoryChoiceEffect[] onPageLoadEffects;
        public StoryEventChoiceSO[] allChoices;       
        

    }
}