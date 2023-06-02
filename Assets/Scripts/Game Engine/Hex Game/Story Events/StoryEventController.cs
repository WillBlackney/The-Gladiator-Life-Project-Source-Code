using DG.Tweening;
using HexGameEngine.Audio;
using HexGameEngine.Boons;
using HexGameEngine.JourneyLogic;
using HexGameEngine.Persistency;
using HexGameEngine.Utilities;
using Sirenix.Utilities;
using System;
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
        [SerializeField] CanvasGroup rootCg;
        [SerializeField] Image blackUnderlay;       
        [SerializeField] TextMeshProUGUI eventHeaderText;
        [SerializeField] TextMeshProUGUI eventDescriptionText;
        [SerializeField] Image eventImage;
        [SerializeField] Scrollbar verticalContentScrollbar;
        [SerializeField] StoryEventChoiceButton[] fittedChoiceButtons;
        [SerializeField] StoryEventChoiceButton[] unfittedChoiceButtons;
        [SerializeField] RectTransform[] layoutsRebuilt;

        [Space(10)]

        [Header("Result Item Components")]
        [SerializeField] Transform resultItemsParent;
        [SerializeField] GameObject resultItemRowPrefab;
        [SerializeField] List<StoryEventResultItemRow> resultItemRows;

        [Space(10)]

        [Header("Movement Components")]
        [SerializeField] RectTransform movementParent;
        [SerializeField] RectTransform onScreenPosition;
        [SerializeField] RectTransform offScreenPosition;

        [Header("Button Auto Fitting")]
        [SerializeField] RectTransform content;
        [SerializeField] Transform unfittedButtonsParent;
        [SerializeField] Transform fittedButtonsParent;

        // Hidden fields
        private List<string> eventsAlreadyEncountered = new List<string>();
        private List<StoryEventResultItem> currentResultItems = new List<StoryEventResultItem>();

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
        public void StartNextEvent()
        {
            currentResultItems.Clear();
            eventHeaderText.text = CurrentStoryEvent.storyEventName;
            CurrentStoryEvent.onStartEffects.ForEach(i => TriggerChoiceEffect(i));
            BuildAllViewsFromPage(CurrentStoryEvent.firstPage);
            ShowUI();
            AudioManager.Instance.PlaySoundPooled(Sound.Effects_Story_Event_Start);
        }
        #endregion

        #region UI + View Logic
        private void ShowUI()
        {
            rootCg.interactable = true;
            rootCanvas.enabled = true;
            movementParent.DOKill();
            blackUnderlay.DOKill();
            blackUnderlay.DOFade(0.5f, 0f);
            movementParent.DOMove(onScreenPosition.position, 0f);
            TransformUtils.RebuildLayouts(layoutsRebuilt);
        }
        private void HideUI(float speed = 0.75f, Action onComplete = null)
        {
            rootCg.interactable = false;
            movementParent.DOKill();
            blackUnderlay.DOKill();
            blackUnderlay.DOFade(0f, speed * 0.66f);
            movementParent.DOMove(offScreenPosition.position, speed).SetEase(Ease.InBack).OnComplete(() =>
            {
                rootCanvas.enabled = false;
                if (onComplete != null) onComplete.Invoke();
            });
        }
        private void BuildAllViewsFromPage(StoryEventPageSO page)
        {
            // Set description text
            eventDescriptionText.text = page.pageDescription;

            // Set up buttons and their views
            BuildChoiceButtonsFromPageData(page);

            // Build current result items section
            BuildResultItemsSection();

            // Set event image
            if (page.pageSprite != null) eventImage.sprite = page.pageSprite;

            // Flush result items from previous page
            currentResultItems.Clear();

            // Rebuild layouts
            TransformUtils.RebuildLayouts(layoutsRebuilt);
            AutoSetButtonFitting();

            // Reset scroll rect to top
            verticalContentScrollbar.value = 1;

        }
        private void BuildChoiceButtonsFromPageData(StoryEventPageSO page)
        {
            // Reset each button
            fittedChoiceButtons.ForEach(i => i.HideAndReset());
            unfittedChoiceButtons.ForEach(i => i.HideAndReset());

            fittedButtonsParent.gameObject.SetActive(true);

            // Build a button for each choice
            for (int i = 0; i < page.allChoices.Length; i++)
            {
                unfittedChoiceButtons[i].BuildAndShow(page.allChoices[i]);
                fittedChoiceButtons[i].BuildAndShow(page.allChoices[i]);
            }
        }
        private void BuildResultItemsSection()
        {
            resultItemRows.ForEach(i => i.Hide());
            for(int i = 0; i < currentResultItems.Count; i++)
            {
                if(i >= resultItemRows.Count)
                {
                    StoryEventResultItemRow newRow = Instantiate(resultItemRowPrefab, resultItemsParent).GetComponent<StoryEventResultItemRow>();
                    resultItemRows.Add(newRow);
                }

                resultItemRows[i].BuildAndShow(currentResultItems[i]);
            }
        }
        private void AutoSetButtonFitting()
        {
            fittedButtonsParent.gameObject.SetActive(true);
            TransformUtils.RebuildLayouts(layoutsRebuilt);

            float scrollBounds = 700f;
            float contentHeight = content.rect.height;
            Debug.Log("Content height: " + contentHeight.ToString());

            // Use fitted buttons
            if(contentHeight > scrollBounds)
            {
                unfittedButtonsParent.gameObject.SetActive(false);
                fittedButtonsParent.gameObject.SetActive(true);
            }

            // Use unfitted buttons
            else
            {
                unfittedButtonsParent.gameObject.SetActive(true);
                fittedButtonsParent.gameObject.SetActive(false);
            }

            TransformUtils.RebuildLayout(content);

        }
       
        #endregion

        #region Determine Next Event Logic
        public StoryEventDataSO DetermineAndCacheNextStoryEvent()
        {
            if(GlobalSettings.Instance.GameMode == GameMode.StoryEventSandbox)
            {
                CurrentStoryEvent = GlobalSettings.Instance.SandboxStoryEvent;
                return CurrentStoryEvent;
            }

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
                !storyEvent.excludeFromGame &&
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
            // to do: need to return a list of things that happened so that we can show the player
            foreach (StoryChoiceEffect s in choice.effects)
            {
                TriggerChoiceEffect(s);
            }
        }
        private void TriggerChoiceEffect(StoryChoiceEffect effect)
        {
            if (effect.effectType == StoryChoiceEffectType.FinishEvent)
            {
                HideUI(0.75f, () =>
                {
                    RunController.Instance.SetCheckPoint(SaveCheckPoint.Town);
                    GameController.Instance.SetGameState(GameState.Town);
                    PersistencyController.Instance.AutoUpdateSaveFile();
                });
            }
            else if (effect.effectType == StoryChoiceEffectType.LoadPage)
            {

            }
            else if(effect.effectType == StoryChoiceEffectType.GainBoon)
            {
                BoonData boonGained = new BoonData(BoonController.Instance.GetBoonDataByTag(effect.boonGained));
                BoonController.Instance.HandleGainBoon(boonGained);
                StoryEventResultItem newResultItem = new StoryEventResultItem("Gained boon: " + boonGained.boonDisplayName, ResultRowIcon.FramedSprite, boonGained.BoonSprite);
                currentResultItems.Add(newResultItem);
            }
        }
        #endregion


    }
}