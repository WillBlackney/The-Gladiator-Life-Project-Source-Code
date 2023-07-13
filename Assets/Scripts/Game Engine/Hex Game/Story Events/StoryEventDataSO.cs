using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace WeAreGladiators.StoryEvents
{
    [CreateAssetMenu(fileName = "New StoryEventDataSO", menuName = "StoryEventData", order = 52)]
    public class StoryEventDataSO : ScriptableObject
    {
        [BoxGroup("General Data", true, true)]
        [LabelWidth(125)]
        public string storyEventName;               

        [BoxGroup("General Data")]
        [LabelWidth(125)]
        public StoryEventPageSO firstPage;

        [BoxGroup("General Data")]
        [LabelWidth(125)]
        public bool excludeFromGame = false;

        [BoxGroup("General Data")]
        [LabelWidth(100)]
        public StoryChoiceEffect[] onStartEffects;

        [BoxGroup("Requirements", true, true)]
        [LabelWidth(100)]
        public StoryEventRequirement[] requirements;

        [BoxGroup("Requirements")]
        [LabelWidth(100)]
        public StoryEventCharacterTarget[] characterRequirements;

    }
}