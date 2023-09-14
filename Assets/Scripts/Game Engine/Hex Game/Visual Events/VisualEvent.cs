using System;
using System.Collections.Generic;
using WeAreGladiators.Characters;

namespace WeAreGladiators.VisualEvents
{
    public class VisualEvent
    {
        public TaskTracker cData;
        public float endDelay;
        public Action eventFunction;

        public VisualEventType eventType;
        public bool isClosed;

        public bool isPlaying;

        public HexCharacterModel myCharacter;
        public float startDelay;

        public VisualEvent(Action _eventFunction, float _startDelay, float _endDelay, VisualEventType _eventType)
        {
            eventFunction = _eventFunction;
            startDelay = _startDelay;
            endDelay = _endDelay;
            eventType = _eventType;
        }
        public List<VisualEvent> EventStack { get; } = new List<VisualEvent>();
        public void AddEventToStack(VisualEvent newEvent)
        {
            EventStack.Add(newEvent);
        }
        public void RemoveEventFromStack(VisualEvent visualEvent)
        {
            if (EventStack.Contains(visualEvent))
            {
                EventStack.Remove(visualEvent);
            }
        }
        public VisualEvent SetStartDelay(float delay)
        {
            startDelay = delay;
            return this;
        }
        public VisualEvent SetEndDelay(float delay)
        {
            endDelay = delay;
            return this;
        }
        public VisualEvent SetCoroutineData(TaskTracker cData)
        {
            this.cData = cData;
            return this;
        }
    }
    public enum VisualEventType
    {
        Single = 1,
        StackParent = 2
    }

}
