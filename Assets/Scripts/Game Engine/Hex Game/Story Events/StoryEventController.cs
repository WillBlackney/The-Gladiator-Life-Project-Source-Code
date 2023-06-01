using DG.Tweening;
using HexGameEngine.Audio;
using HexGameEngine.Persistency;
using HexGameEngine.Utilities;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexGameEngine.StoryEvents
{
    public class StoryEventController : Singleton<StoryEventController>
    {
        #region Components
        [Header("Core Data")]
        [SerializeField] StoryEventDataSO[] allStoryEvents;

        [Space(10)]

        [Header("Core UI Components")]
        [SerializeField] Canvas rootCanvas;
        [SerializeField] StoryEventChoiceButton[] choiceButtons;
        [SerializeField] TextMeshProUGUI eventHeaderText;
        [SerializeField] TextMeshProUGUI eventDescriptionText;
        [SerializeField] Image eventImage;

        [Space(10)]

        [Header("Movement Components")]
        [SerializeField] RectTransform movementParent;
        [SerializeField] RectTransform onScreenPosition;
        [SerializeField] RectTransform offScreenPosition;

        // Hidden fields
        private List<string> eventsAlreadyEncountered = new List<string>();

        #endregion

        #region Getters + Accessors
        public StoryEventDataSO CurrentStoryEvent { get; private set; }
        public StoryEventDataSO[] AllStoryEvents { get { return allStoryEvents; } }

        #endregion

        #region Persistency Logic
        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            if (CurrentStoryEvent != null)
                saveFile.currentStoryEvent = CurrentStoryEvent.storyEventName;

            saveFile.encounteredStoryEvents = eventsAlreadyEncountered;
        }
        public void BuildMyDataFromSaveFile(SaveGameData saveFile)
        {
            foreach (StoryEventDataSO s in AllStoryEvents)
            {
                if (s.name == saveFile.currentStoryEvent)
                {
                    CurrentStoryEvent = s;
                    break;
                }
            }

            eventsAlreadyEncountered = saveFile.encounteredStoryEvents;
        }
        #endregion

        #region Start Events
        public void StartEvent(StoryEventDataSO storyEvent)
        {
            if (storyEvent == null) storyEvent = CurrentStoryEvent;
            eventHeaderText.text = storyEvent.storyEventName;
            BuildAllViewsFromPage(storyEvent.firstPage);
            ShowUI();
        }
        #endregion

        #region UI + View Logic
        private void ShowUI()
        {
            rootCanvas.enabled = true;
        }
        private void BuildAllViewsFromPage(StoryEventPageSO page)
        {
            // Set description text
            eventDescriptionText.text = page.pageDescription;

            // TO DO: Reset scroll rect to top

            // set up buttons and their views
            BuildChoiceButtonsFromPageData(page);
            
            // Set event image
            if (page.pageSprite != null) eventImage.sprite = page.pageSprite;

        }
        private void BuildChoiceButtonsFromPageData(StoryEventPageSO page)
        {
            // Reset each button
            choiceButtons.ForEach(i => i.HideAndReset());
            // Build a button for each choice
            for (int i = 0; i < page.allChoices.Length; i++) choiceButtons[i].BuildAndShow(page.allChoices[i]);
        }
        #endregion

        #region Determine Next Event Logic
        public StoryEventDataSO DetermineAndCacheNextStoryEvent()
        {
            List<StoryEventDataSO> validEvents = GetValidStoryEvents();
            if (validEvents.Count == 0)
            {
                Debug.LogWarning("StoryEventController.DetermineAndCacheNextStoryEvent() could not find any valid story events, cancelling...");
                return null;
            }

            CurrentStoryEvent = validEvents[RandomGenerator.NumberBetween(0, validEvents.Count - 1)];
            if(eventsAlreadyEncountered.Contains(CurrentStoryEvent.storyEventName) == false)
                eventsAlreadyEncountered.Add(CurrentStoryEvent.storyEventName);

            return CurrentStoryEvent;
        }
        private List<StoryEventDataSO> GetValidStoryEvents()
        {
            List<StoryEventDataSO> listReturned = new List<StoryEventDataSO>();
            foreach (StoryEventDataSO s in AllStoryEvents)
            {
                if (IsStoryEventValid(s))
                    listReturned.Add(s);
            }

            return listReturned;
        }
        #endregion

        #region Story Event Validity Checking
        private bool IsStoryEventValid(StoryEventDataSO storyEvent)
        {
            if (eventsAlreadyEncountered.Contains(storyEvent.storyEventName) == false &&
                DoesStoryEventMeetAllRequirements(storyEvent))            
                return true;            
            else return false;
        }
        private bool DoesStoryEventMeetAllRequirements(StoryEventDataSO storyEvent)
        {
            bool ret = true;

            // TO DO: some events will require non stage related things, for example
            // at least 1 character with 30+ health, or player must have 100+ gold, etc

            foreach(StoryEventRequirement req in storyEvent.requirements)
            {
                if(!IsStoryEventRequirementMet(storyEvent, req))
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }
        private bool IsStoryEventRequirementMet(StoryEventDataSO storyEvent, StoryEventRequirement requirement)
        {
            bool ret = true;

            return ret;
        }
        #endregion

        #region Buttons + Input
        public void HandleChoiceButtonClicked(StoryEventChoiceButton button)
        {
            if (button.MyChoiceData != null)
            {
                AudioManager.Instance.PlaySoundPooled(Sound.UI_Button_Click);
                HandleChoiceEffects(button.MyChoiceData);
            }

        }

        #endregion

        #region Handle Choices
        private void HandleChoiceEffects(StoryEventChoiceSO choice)
        {
            foreach (StoryChoiceEffect s in choice.effects)
            {
                //TriggerChoiceEffect(s, selectedCharacterButton.myCharacter);
            }
        }
        #endregion


    }
}