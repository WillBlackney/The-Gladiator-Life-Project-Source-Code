using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace HexGameEngine.StoryEvents
{
    [CreateAssetMenu(fileName = "New StoryEventDataSO", menuName = "StoryEventData", order = 52)]
    public class StoryEventDataSO : ScriptableObject
    {
        [BoxGroup("General Data", true, true)]
        [LabelWidth(100)]
        public string storyEventName;

        [BoxGroup("General Data")]
        [LabelWidth(100)]
        public bool excludeFromGame = false;

        [BoxGroup("General Data")]
        [LabelWidth(100)]
        public StoryEventPageSO firstPage;

        [BoxGroup("General Data")]
        [LabelWidth(100)]
        public StoryChoiceEffect[] onStartEffects;

        [BoxGroup("Requirements", true, true)]
        [LabelWidth(100)]
        public StoryEventRequirement[] requirements;

    }
}