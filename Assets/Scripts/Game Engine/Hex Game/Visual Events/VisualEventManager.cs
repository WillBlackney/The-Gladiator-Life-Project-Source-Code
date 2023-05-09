using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HexGameEngine.Utilities;
using System.Threading.Tasks;
using PlasticGui.WorkspaceWindow.BranchExplorer;

namespace HexGameEngine.VisualEvents
{
    public class VisualEventManager : Singleton<VisualEventManager>
    {
        // Properties
        #region
        private List<VisualEvent> eventQueue = new List<VisualEvent>();
        [SerializeField] private float startDelayExtra;
        [SerializeField] private float endDelayExtra;
        private bool paused;
        private bool currentEventPlaying;
        public List<VisualEvent> EventQueue
        {
            get { return eventQueue; }
            private set { eventQueue = value; }
        }
        #endregion

        // Misc Logic
        #region
        private void Update()
        {
            PlayNextEventFromQueue();
        }
        #endregion

        // Trigger Visual Events
        #region
        private async void PlayEventFromQueue(VisualEvent ve)
        {
            // Mark as active
            ve.isPlaying = true;
            currentEventPlaying = true;

            // Start Delay    
            if (ve.startDelay > 0) await Task.Delay((int)((ve.startDelay + startDelayExtra) * 1000));

            // Execute function 
            if (ve.eventFunction != null) ve.eventFunction.Invoke();            

            // Wait until execution finished 
            if (ve.cData != null) while (ve.cData.Complete() == false) await Task.Delay(10);            

            // End delay
            if (ve.endDelay > 0) await Task.Delay((int)((ve.endDelay + endDelayExtra) * 1000));            

            // Remove from queue
            RemoveEventFromQueue(ve);
            currentEventPlaying = false;

            // Start next event
            PlayNextEventFromQueue();

        }
        private void PlayNextEventFromQueue()
        {
            if (EventQueue.Count > 0)
            {
                // Pop the first event from the top
                if (EventQueue.Count > 0 &&
                    EventQueue[0].isPlaying == false &&
                    paused == false &&
                    currentEventPlaying == false &&
                    EventQueue[0].eventType == VisualEventType.Single)
                {
                    PlayEventFromQueue(EventQueue[0]);
                }

                else if (EventQueue.Count > 0 &&
                    EventQueue[0].isPlaying == false &&
                    EventQueue[0].eventType == VisualEventType.StackParent)
                {
                    List<VisualEvent> allConsecutiveStackEvents = new List<VisualEvent>();

                    for (int i = 0; i < EventQueue.Count; i++)
                    {
                        if (EventQueue[i].eventType == VisualEventType.StackParent)
                            allConsecutiveStackEvents.Add(EventQueue[i]);
                        else break;
                    }

                    foreach (VisualEvent ve in allConsecutiveStackEvents)
                    {
                        StartEventStackPlaying(ve);
                    }
                }

            }

        }    
        private void StartEventStackPlaying(VisualEvent parentEvent)
        {
            parentEvent.isPlaying = true;
            if (parentEvent.EventStack.Count > 0) PlayEventFromStackEvent(parentEvent.EventStack[0], parentEvent);
            else HandleEmptyStackEventAddedToQueue(parentEvent);            
        }
        private async void HandleEmptyStackEventAddedToQueue(VisualEvent parentEvent)
        {
            Debug.Log("HandleEmptyStackEventAddedToQueue() called");
            //yield return null;
            await Task.Delay((int)(Time.deltaTime * 2000f));

            if (parentEvent.EventStack.Count > 0)
            {
                Debug.Log("HandleEmptyStackEventAddedToQueue() stack was empty, but an event was added during the yield delay: playing stack event normally...");
                //StartCoroutine(PlayEventFromStackEvent(parentEvent.EventStack[0], parentEvent));
                PlayEventFromStackEvent(parentEvent.EventStack[0], parentEvent);
            }

            else
            {
                Debug.Log("HandleEmptyStackEventAddedToQueue() empty stack event was added to queue, and was still empty after yield delay, removing stack event from queue...");
                RemoveEventFromQueue(parentEvent);
            }
        }
        private async void PlayEventFromStackEvent(VisualEvent childEvent, VisualEvent parentEvent)
        {
            // Start Delay    
            if (childEvent.startDelay > 0) await Task.Delay((int)((childEvent.startDelay + startDelayExtra) * 1000));            

            // Execute function 
            if (childEvent.eventFunction != null) childEvent.eventFunction.Invoke();            

            // Wait until execution finished 
            if (childEvent.cData != null) 
                while (childEvent.cData.Complete() == false) await Task.Delay((int)(Time.deltaTime * 1000));            

            // End delay
            if (childEvent.endDelay > 0) await Task.Delay((int)((childEvent.endDelay + endDelayExtra) * 1000));            

            // Remove from stack's list of events
            RemoveEventFromStackEvent(childEvent, parentEvent);

            // Start next event in list
            if (parentEvent.EventStack.Count > 0) PlayEventFromStackEvent(parentEvent.EventStack[0], parentEvent);
            else if (parentEvent.EventStack.Count == 0)
            {
                parentEvent.myCharacter.HandlePopStackEvent(parentEvent);
                RemoveEventFromQueue(parentEvent);
            }
        }
        #endregion

        // Modify Queue
        #region
        private void RemoveEventFromStackEvent(VisualEvent childEvent, VisualEvent parentEvent)
        {
            if (parentEvent.EventStack.Contains(childEvent)) parentEvent.RemoveEventFromStack(childEvent);
        }
        private void RemoveEventFromQueue(VisualEvent ve)
        {
            if (EventQueue.Contains(ve)) EventQueue.Remove(ve);
        }
        private void AddEventToBackOfQueue(VisualEvent ve)
        {
            EventQueue.Add(ve);
        }
        public void ClearEventQueue()
        {
            EventQueue.Clear();
        }
        public VisualEvent HandleEventQueueTearDown()
        {
            // this function is used to make sure no null ref errors
            // occur when we transistion from game scene to menu scene.
            // errors can occur if we destroy the scene while coroutines
            // are still actively running and operating on scene objects.
            // this function allows the game to safely clear the event
            // queue, wait for the current event to finish, then tear down
            // the game scene.

            // TO DO: just realized a problem: not all corutines running from
            // visual events have a coroutine data tracker. They start their coroutine 
            // and get removed from the queue straight away. If an event like this is playing
            // when we tear down the scene, the logic wont know to wait for it to finish
            // since it isnt being tracked, and we will probably get errors!! This could sort
            // of be fixed by waiting for a second or two when we fade the screen out, since most
            // coroutines that arent tracked dont last longer then 2 seconds.

            VisualEvent handleReturned = null;
            PauseQueue();
            if (EventQueue.Count > 0 && currentEventPlaying == true) handleReturned = EventQueue[0];
            ClearEventQueue();
            EnableQueue();
            return handleReturned;
        }
        #endregion

        // Create Events
        #region
        public VisualEvent CreateVisualEvent(Action eventFunction, VisualEvent parentEvent = null)
        {
            VisualEvent vEvent = new VisualEvent(eventFunction, 0, 0, VisualEventType.Single);
            if (parentEvent != null && !parentEvent.isClosed) parentEvent.AddEventToStack(vEvent);
            else AddEventToBackOfQueue(vEvent);
            return vEvent;
        }
        public VisualEvent CreateStackParentVisualEvent(Characters.HexCharacterModel character)
        {
            // NOTE: This method requires on argument of 'CoroutineData'.
            // this function is only for visual events that have their sequence
            // triggered over time by way of coroutine.
            // if a visual event has no coroutine and resolves instantly when played from the queue,
            // it should be called using the overload function below this function

            VisualEvent vEvent = new VisualEvent(null, 0f, 0f, VisualEventType.StackParent);
            vEvent.myCharacter = character;
            character.eventStacks.Add(vEvent);
            AddEventToBackOfQueue(vEvent);             
            return vEvent;
        }
        #endregion

        // Custom Events
        #region
        public VisualEvent InsertTimeDelayInQueue(float delayDuration,VisualEvent parentEvent = null)
        {
            VisualEvent vEvent = new VisualEvent(null, 0, delayDuration, VisualEventType.Single);
            if (parentEvent != null && !parentEvent.isClosed) parentEvent.AddEventToStack(vEvent);           
            else AddEventToBackOfQueue(vEvent);      
            return vEvent;
        }
        #endregion


        // Disable + Enable Queue 
        #region
        public void EnableQueue()
        {
            paused = false;
        }
        public void PauseQueue()
        {
            paused = true;
        }
        #endregion
    }

}