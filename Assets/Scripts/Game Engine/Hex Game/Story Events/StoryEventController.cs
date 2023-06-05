using DG.Tweening;
using HexGameEngine.Audio;
using HexGameEngine.Boons;
using HexGameEngine.Characters;
using HexGameEngine.Items;
using HexGameEngine.JourneyLogic;
using HexGameEngine.Libraries;
using HexGameEngine.Persistency;
using HexGameEngine.Player;
using HexGameEngine.TownFeatures;
using HexGameEngine.UI;
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
        private List<HexCharacterData> characterTargets = new List<HexCharacterData>();

        private const string CHARACTER_1_NAME_KEY = "{CHARACTER_1_NAME}";
        private const string CHARACTER_1_SUB_NAME_KEY = "{CHARACTER_1_SUB_NAME}";

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
            DetermineAndCacheCharacterTargetsOnEventStart(CurrentStoryEvent);
            eventHeaderText.text = CurrentStoryEvent.storyEventName;
            CurrentStoryEvent.onStartEffects.ForEach(i => ResolveChoiceEffect(i));
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
            // Trigger effects on load page
            page.onPageLoadEffects.ForEach(i => ResolveChoiceEffect(i));

            // Set description text
            eventDescriptionText.text = GetDynamicValueString(page.pageDescription);

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
            for (int i = 0; i < currentResultItems.Count; i++)
            {
                if (i >= resultItemRows.Count)
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
            if (contentHeight > scrollBounds)
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
        public string GetDynamicValueString(string original)
        {
            string ret = original;
            if(characterTargets.Count > 1)
            {
                ret = original.Replace(CHARACTER_1_NAME_KEY, characterTargets[0].myName);
                ret = ret.Replace(CHARACTER_1_SUB_NAME_KEY, characterTargets[0].myClassName);
            }           
            return ret;
        }

        #endregion

        #region Determine Next Event Logic
        public StoryEventDataSO DetermineAndCacheNextStoryEvent()
        {
            if (GlobalSettings.Instance.GameMode == GameMode.StoryEventSandbox)
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
            if (eventsAlreadyEncountered.Contains(CurrentStoryEvent.storyEventName) == false)
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

        #region Story Event Requirements + Validity Checking
        private bool IsStoryEventValid(StoryEventDataSO storyEvent)
        {
            if (eventsAlreadyEncountered.Contains(storyEvent.storyEventName) == false &&
                !storyEvent.excludeFromGame &&
                DoesStoryEventMeetBaseRequirements(storyEvent) &&
                DoesStoryEventMeetCharacterTargetRequirements(storyEvent))
                return true;
            else return false;
        }
        private bool DoesStoryEventMeetBaseRequirements(StoryEventDataSO storyEvent)
        {
            bool ret = true;

            foreach (StoryEventRequirement req in storyEvent.requirements)
            {
                if (!IsStoryEventRequirementMet(storyEvent, req))
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

            if (requirement.reqType == StoryEventRequirementType.XorMoreCharactersInRoster)
            {
                int characterCount = CharacterDataController.Instance.AllPlayerCharacters.Count;
                if (requirement.includeTheKid == false && TheKidIsAlive()) characterCount = characterCount - 1;
                ret = characterCount < requirement.requiredCharactersInRosterCount;
            }
            else if (requirement.reqType == StoryEventRequirementType.XorLessCharactersInRoster)
            {
                int characterCount = CharacterDataController.Instance.AllPlayerCharacters.Count;
                if (requirement.includeTheKid == false && TheKidIsAlive()) characterCount = characterCount - 1;
                ret = characterCount > requirement.requiredCharactersInRosterCount;
            }
            else if (requirement.reqType == StoryEventRequirementType.HasXorLessGold)
            {
                ret = PlayerDataController.Instance.CurrentGold <= requirement.goldRequired;
            }
            else if (requirement.reqType == StoryEventRequirementType.HasXorMoreGold)
            {
                ret = PlayerDataController.Instance.CurrentGold >= requirement.goldRequired;
            }
            return ret;
        }
        private bool TheKidIsAlive()
        {
            bool ret = false;

            foreach (HexCharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
            {
                if (character.background.backgroundType == CharacterBackground.Companion)
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }
        private bool DoesStoryEventMeetCharacterTargetRequirements(StoryEventDataSO storyEvent)
        {
            bool ret = true;
            List<HexCharacterData> chosenCharacterProspects = new List<HexCharacterData>();

            foreach (StoryEventCharacterTarget req in storyEvent.characterRequirements)
            {
                HexCharacterData character = TryFindSuitableCharacterTarget(req, chosenCharacterProspects);
                if (character == null)
                {
                    ret = false;
                    break;
                }
                else chosenCharacterProspects.Add(character);
            }

            return ret;
        }
        private bool DoesCharacterMeetStoryEventCharacterTargetRequirement(HexCharacterData character, StoryEventCharacterTargetRequirement req)
        {
            bool ret = false;
            if (req.reqType == StoryEventCharacterTargetRequirementType.HasBackground)
            {
                foreach(CharacterBackground cbg in req.requiredBackgrounds)
                {
                    if(character.background.backgroundType == cbg)
                    {
                        ret = true;
                        break;
                    }
                }
            }
            else if (req.reqType == StoryEventCharacterTargetRequirementType.DoesNotHaveBackground)
            {
                bool pass = true;
                foreach (CharacterBackground cbg in req.requiredBackgrounds)
                {
                    if (character.background.backgroundType == cbg)
                    {
                        pass = false;
                        break;
                    }
                }
                ret = pass;
            }

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
            // Determine set
            StoryChoiceEffectSet set = null;
            if (choice.effectSets.Length == 1) set = choice.effectSets[0];
            else
            {
                int roll = RandomGenerator.NumberBetween(1, 100);
                foreach(StoryChoiceEffectSet s in choice.effectSets)
                {
                    if(roll >= s.lowerProbability && roll <= s.upperProbability)
                    {
                        set = s;
                        break;
                    }
                }
            }

            set.effects.ForEach(i => ResolveChoiceEffect(i));

            // UI Updates
            CharacterScrollPanelController.Instance.RebuildViews();
        }
        private void ResolveChoiceEffect(StoryChoiceEffect effect)
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
                BuildAllViewsFromPage(effect.pageToLoad);
            }
            else if (effect.effectType == StoryChoiceEffectType.GainGold)
            {
                PlayerDataController.Instance.ModifyPlayerGold(effect.goldGained);
                StoryEventResultItem newResultItem = new StoryEventResultItem("Gained " + TextLogic.ReturnColoredText
                    (effect.goldGained.ToString(), TextLogic.blueNumber) +" gold.", ResultRowIcon.GoldCoins);
                currentResultItems.Add(newResultItem);
            }
            else if (effect.effectType == StoryChoiceEffectType.LoseAllGold)
            {
                int goldLost = PlayerDataController.Instance.CurrentGold;
                PlayerDataController.Instance.ModifyPlayerGold(-PlayerDataController.Instance.CurrentGold);
                StoryEventResultItem newResultItem = new StoryEventResultItem("Lost " + TextLogic.ReturnColoredText
                    (goldLost.ToString(), TextLogic.blueNumber) + " gold.", ResultRowIcon.GoldCoins);
                currentResultItems.Add(newResultItem);
            }
            else if(effect.effectType == StoryChoiceEffectType.GainBoon)
            {
                BoonData boonGained = new BoonData(BoonController.Instance.GetBoonDataByTag(effect.boonGained));
                BoonController.Instance.HandleGainBoon(boonGained);
                StoryEventResultItem newResultItem = new StoryEventResultItem("Gained boon: " + boonGained.boonDisplayName, ResultRowIcon.FramedSprite, boonGained.BoonSprite);
                currentResultItems.Add(newResultItem);
            }
            else if (effect.effectType == StoryChoiceEffectType.GainItem)
            {
                var ic = ItemController.Instance;
                ItemData item = ic.GenerateNewItemWithRandomEffects(ic.GetItemDataByName(effect.itemGained.itemName));
                InventoryController.Instance.AddItemToInventory(item);

                StoryEventResultItem newResultItem = new StoryEventResultItem("Gained item: " + item.itemName, ResultRowIcon.FramedSprite, item.ItemSprite);
                currentResultItems.Add(newResultItem);
            }
            else if (effect.effectType == StoryChoiceEffectType.AddRecruitsToTavern)
            {
                BackgroundData bgData = CharacterDataController.Instance.GetBackgroundData(effect.backgroundAddedToTavern);
                for(int i = 0; i < effect.totalCharactersAddedToTavern; i++)
                {
                    HexCharacterData newCharacter = CharacterDataController.Instance.GenerateRecruitCharacter(bgData);
                    TownController.Instance.HandleAddNewRecruitToTavernFromStoryEvent(newCharacter);
                    string message = newCharacter.myName + " " + newCharacter.myClassName + " added to the tavern.";
                    StoryEventResultItem newResultItem = new StoryEventResultItem(message, ResultRowIcon.FramedSprite, bgData.backgroundSprite);
                    currentResultItems.Add(newResultItem);
                }
            }
            else if(effect.effectType == StoryChoiceEffectType.CharacterKilled)
            {
                // todo: determine target correctly
                HexCharacterData target = characterTargets[0];

                CharacterDataController.Instance.RemoveCharacterFromRoster(target);

                string message = target.myName + " " + target.myClassName + " died.";
                StoryEventResultItem newResultItem = new StoryEventResultItem(message, ResultRowIcon.Skull);
                currentResultItems.Add(newResultItem);

            }
        }
        #endregion

        #region Character Targeting Logic
        private HexCharacterData TryFindSuitableCharacterTarget(StoryEventCharacterTarget reqSet, List<HexCharacterData> excludedCharacters = null)
        {
            HexCharacterData ret = null;
            List<HexCharacterData> allProspects = new List<HexCharacterData>();
            List<HexCharacterData> validProspects = new List<HexCharacterData>();

            // Get all player characters not explicitly excluded
            foreach (HexCharacterData c in CharacterDataController.Instance.AllPlayerCharacters)
                if (excludedCharacters.Contains(c) == false) allProspects.Add(c);

            foreach (HexCharacterData prospect in allProspects)
            {
                bool passed = true;
                foreach (StoryEventCharacterTargetRequirement req in reqSet.requirements)
                {
                    if (!DoesCharacterMeetStoryEventCharacterTargetRequirement(prospect, req))
                    {
                        passed = false;
                        break;
                    }
                }

                if (passed) validProspects.Add(prospect);
            }

            // If multiple valid characters, choose one randomly
            if (validProspects.Count > 1) ret = validProspects[RandomGenerator.NumberBetween(0, validProspects.Count - 1)];            
            else if (validProspects.Count == 1) ret = validProspects[0];

            return ret;
        }
        private void DetermineAndCacheCharacterTargetsOnEventStart(StoryEventDataSO storyEvent)
        {
            characterTargets.Clear();
            int requiredCharacters = storyEvent.characterRequirements.Length;

            // Fuck it. I'm not in the mood to write a well engineered solution. I'm brute forcing this shit and moving onto the next feature.            
            for(int i = 0; i < 500; i++)
            {
                characterTargets.Clear();
                foreach (StoryEventCharacterTarget c in storyEvent.characterRequirements)
                {
                    HexCharacterData target = TryFindSuitableCharacterTarget(c, characterTargets);
                    if (target != null) characterTargets.Add(target);
                }
                if (characterTargets.Count == requiredCharacters) break;
            }


        }
        #endregion


    }
}