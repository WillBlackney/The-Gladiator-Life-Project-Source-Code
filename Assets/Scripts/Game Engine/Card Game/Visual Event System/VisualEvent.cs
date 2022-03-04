using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CardGameEngine
{
    public class VisualEvent
    {
        public CoroutineData cData;
        public Action eventFunction;

        public bool isPlaying;
        public EventDetail eventDetail;
        public VisualEventType eventType;
        public float startDelay;
        public float endDelay;

        public HexGameEngine.Characters.HexCharacterModel myCharacter;

        private List<VisualEvent> eventStack = new List<VisualEvent>();
        public List<VisualEvent> EventStack
        {
            get { return eventStack; }
        }

        public VisualEvent(Action _eventFunction, CoroutineData _cData, float _startDelay, float _endDelay, EventDetail _eventDetail, VisualEventType _eventType)
        {
            eventFunction = _eventFunction;
            cData = _cData;
            startDelay = _startDelay;
            endDelay = _endDelay;
            eventDetail = _eventDetail;
            eventType = _eventType;
        }
        public void AddEventToStack(VisualEvent newEvent)
        {
            eventStack.Add(newEvent);
        }
        public void RemoveEventFromStack(VisualEvent visualEvent)
        {
            if (eventStack.Contains(visualEvent))
                eventStack.Remove(visualEvent);
        }
    }
    public enum VisualEventType
    {
        Single = 1,
        StackParent = 2,
    }


}