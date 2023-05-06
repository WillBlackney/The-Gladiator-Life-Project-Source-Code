using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HexGameEngine.Characters;

namespace HexGameEngine.VisualEvents
{
    public class VisualEvent
    {
        public CoroutineData cData;
        public Action eventFunction;

        public bool isPlaying;
        public bool isClosed;
        public VisualEventType eventType;
        public float startDelay;
        public float endDelay;

        public HexCharacterModel myCharacter;

        private List<VisualEvent> eventStack = new List<VisualEvent>();
        public List<VisualEvent> EventStack
        {
            get { return eventStack; }
        }

        public VisualEvent(Action _eventFunction, CoroutineData _cData, float _startDelay, float _endDelay, VisualEventType _eventType)
        {
            eventFunction = _eventFunction;
            cData = _cData;
            startDelay = _startDelay;
            endDelay = _endDelay;
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
    }
    public enum VisualEventType
    {
        Single = 1,
        StackParent = 2,
    }


}