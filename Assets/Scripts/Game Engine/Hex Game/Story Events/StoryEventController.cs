using DG.Tweening;
using HexGameEngine.Audio;
using HexGameEngine.Boons;
using HexGameEngine.Characters;
using HexGameEngine.Items;
using HexGameEngine.JourneyLogic;
using HexGameEngine.Libraries;
using HexGameEngine.Perks;
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
using UnityEngine.TextCore.Text;
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
        [SerializeField] GameObject visualParent;
        [SerializeField] CanvasGroup rootCg;
        [SerializeField] Image blackUnderlay;
        [SerializeField] TextMeshProUGUI eventHeaderText;
        [SerializeField] TextMeshProUGUI eventDescriptionText;
        [SerializeField] Image eventImage;
        [SerializeField] ScrollRect mainContentScrollView;
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
        private HexCharacterData choiceCharacterTarget;

        private const string CHARACTER_1_NAME_KEY = "{CHARACTER_1_NAME}";
        private const string CHARACTER_1_SUB_NAME_KEY = "{CHARACTER_1_SUB_NAME}";
        private const string CHARACTER_2_NAME_KEY = "{CHARACTER_2_NAME}";
        private const string CHARACTER_2_SUB_NAME_KEY = "{CHARACTER_2_SUB_NAME}";

        private const string CHOICE_CHARACTER_NAME_KEY = "{CHOICE_CHARACTER_NAME}";
        private const string CHOICE_CHARACTER_SUB_NAME_KEY = "{CHOICE_CHARACTER_SUB_NAME}";
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
            else saveFile.currentStoryEvent = "";

            saveFile.encounteredStoryEvents = eventsAlreadyEncountered;
        }
        public void BuildMyDataFromSaveFile(SaveGameData saveFile)
        {
            CurrentStoryEvent = null;
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
            visualParent.SetActive(true);

            // Flush old data
            choiceCharacterTarget = null;
            currentResultItems.Clear();

            // Get new targets (if event requires it)
            DetermineAndCacheCharacterTargetsOnEventStart(CurrentStoryEvent);
           
            // Trigger on event start effects
            CurrentStoryEvent.onStartEffects.ForEach(i => ResolveChoiceEffect(i));

            // Build UI for first page
            eventHeaderText.text = CurrentStoryEvent.storyEventName;
            BuildAllViewsFromPage(CurrentStoryEvent.firstPage);
            ShowUI();

            // Play event start fanfare SFX
            AudioManager.Instance.PlaySound(Sound.Effects_Story_Event_Start);
        }
        #endregion

        #region UI + View Logic
        private void ShowUI()
        {
            // Make UI clickable
            rootCg.interactable = true;
            visualParent.SetActive(true);            

            // Reset tweens
            movementParent.DOKill();
            blackUnderlay.DOFade(0.01f, 0);
            blackUnderlay.DOKill();
            movementParent.DOMove(offScreenPosition.position, 0f);

            // Move on screen
            blackUnderlay.DOFade(0.5f, 0.75f).OnComplete(() =>
            {
                movementParent.DOMove(onScreenPosition.position, 1f).SetEase(Ease.OutBack);
                TransformUtils.RebuildLayouts(layoutsRebuilt);
            });
            
        }
        private void HideUI(float speed = 1f, Action onComplete = null)
        {
            // Disable interactions
            rootCg.interactable = false;

            // Kill any running tweens
            movementParent.DOKill();
            blackUnderlay.DOKill();

            // Fade out and move panel off screen north
            blackUnderlay.DOFade(0f, speed * 0.66f);
            movementParent.DOMove(offScreenPosition.position, speed).SetEase(Ease.InBack).OnComplete(() =>
            {
                visualParent.SetActive(false);
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
            TransformUtils.RebuildLayouts(layoutsRebuilt);
            AutoSetButtonFitting();

            // Reset scroll rect to top
            mainContentScrollView.verticalNormalizedPosition = 1;
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
                // Check each choice is valid within the current context
                if (DoesChoiceMeetAllRequirements(page.allChoices[i]))
                {
                    unfittedChoiceButtons[i].BuildAndShow(page.allChoices[i]);
                    fittedChoiceButtons[i].BuildAndShow(page.allChoices[i]);
                }               
            }
        }
        private bool DoesChoiceMeetAllRequirements(StoryEventChoiceSO choice)
        {
            bool ret = true;

            foreach(StoryChoiceRequirement req in choice.requirements)
            {
                bool pass = IsStoryChoiceRequirementMet(req);
                if (!pass)
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }
        private bool IsStoryChoiceRequirementMet(StoryChoiceRequirement req)
        {
            bool ret = true;
            if(req.requirementType == StoryChoiceReqType.CharacterWithBackground)
            {
                bool foundMatch = false;
                foreach(HexCharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
                {
                    if(character.background.backgroundType == req.requiredBackground)
                    {
                        foundMatch = true;
                        choiceCharacterTarget = character;
                        break;
                    }
                }
                ret = foundMatch;
            }
            return ret;
        }
        private void BuildResultItemsSection()
        {
            // Reset all tabs
            resultItemRows.ForEach(i => i.Hide());

            // Build a tab row for each result element
            for (int i = 0; i < currentResultItems.Count; i++)
            {
                // If not enough UI rows for each element, create new ones
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
            // Build default fitted button state first to accurately check if
            // content exceeds the fitted view setting bounds
            fittedButtonsParent.gameObject.SetActive(true);
            TransformUtils.RebuildLayouts(layoutsRebuilt);

            // Determine current boundary of fitted button content
            float scrollBounds = 700f;
            float contentHeight = content.rect.height;

            // Content exceeds the page size => use fitted buttons
            if (contentHeight > scrollBounds)
            {
                unfittedButtonsParent.gameObject.SetActive(false);
                fittedButtonsParent.gameObject.SetActive(true);
            }

            // Content does not exceed the page size => use unfitted buttons
            else
            {
                unfittedButtonsParent.gameObject.SetActive(true);
                fittedButtonsParent.gameObject.SetActive(false);
            }

            // Rebuild all content fitters again for good measure
            TransformUtils.RebuildLayout(content);
        }
        public string GetDynamicValueString(string original)
        {
            // Function is used to inject dynamic values (like character names) into 
            // page descriptions and choice button text fields

            string ret = original;
            if(characterTargets.Count >= 1)
            {
                ret = ret.Replace(CHARACTER_1_NAME_KEY, characterTargets[0].myName);
                ret = ret.Replace(CHARACTER_1_SUB_NAME_KEY, characterTargets[0].mySubName);
            }
            if (characterTargets.Count >= 2)
            {
                ret = ret.Replace(CHARACTER_2_NAME_KEY, characterTargets[1].myName);
                ret = ret.Replace(CHARACTER_2_SUB_NAME_KEY, characterTargets[1].mySubName);
            }
            if (choiceCharacterTarget != null)
            {
                ret = ret.Replace(CHOICE_CHARACTER_NAME_KEY, choiceCharacterTarget.myName);
                ret = ret.Replace(CHOICE_CHARACTER_SUB_NAME_KEY, choiceCharacterTarget.mySubName);
            }
            return ret;
        }

        #endregion

        #region Determine Next Event Logic
        public StoryEventDataSO DetermineAndCacheNextStoryEvent()
        {
            // If sandbox mode, use the global settings predetermined event
            if (GlobalSettings.Instance.GameMode == GameMode.StoryEventSandbox)
            {
                CurrentStoryEvent = GlobalSettings.Instance.SandboxStoryEvent;
                return CurrentStoryEvent;
            }

            // Cancel if no valid story events for the current state of the game
            List<StoryEventDataSO> validEvents = GetValidStoryEvents();
            if (validEvents.Count == 0)
            {
                Debug.LogWarning("StoryEventController.DetermineAndCacheNextStoryEvent() could not find any valid story events, cancelling...");
                return null;
            }

            // Choose a valid event randomly if multiple events are valid.
            CurrentStoryEvent = validEvents[RandomGenerator.NumberBetween(0, validEvents.Count - 1)];

            // Add chosen event to list of events all experienced (so that it isn't show twice)
            if (eventsAlreadyEncountered.Contains(CurrentStoryEvent.storyEventName) == false)
                eventsAlreadyEncountered.Add(CurrentStoryEvent.storyEventName);

            return CurrentStoryEvent;
        }
        private List<StoryEventDataSO> GetValidStoryEvents(bool shuffled = true)
        {
            List<StoryEventDataSO> listReturned = new List<StoryEventDataSO>();
            foreach (StoryEventDataSO s in AllStoryEvents)
            {
                if (IsStoryEventValid(s))
                    listReturned.Add(s);
            }
            if (shuffled) return listReturned.ShuffledCopy();
            else return listReturned;
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
                ret = characterCount > requirement.requiredCharactersInRosterCount;
            }
            else if (requirement.reqType == StoryEventRequirementType.XorLessCharactersInRoster)
            {
                int characterCount = CharacterDataController.Instance.AllPlayerCharacters.Count;
                if (requirement.includeTheKid == false && TheKidIsAlive()) characterCount = characterCount - 1;
                ret = characterCount < requirement.requiredCharactersInRosterCount;
            }
            else if (requirement.reqType == StoryEventRequirementType.HasXorLessGold)
            {
                ret = PlayerDataController.Instance.CurrentGold <= requirement.goldRequired;
            }
            else if (requirement.reqType == StoryEventRequirementType.HasXorMoreGold)
            {
                ret = PlayerDataController.Instance.CurrentGold >= requirement.goldRequired;
            }
            else if (requirement.reqType == StoryEventRequirementType.HasBoon)
            {
                ret = BoonController.Instance.DoesPlayerHaveBoon(requirement.requiredBoon);
            }
            else if (requirement.reqType == StoryEventRequirementType.DoesNotHaveBoon)
            {
                ret = !BoonController.Instance.DoesPlayerHaveBoon(requirement.requiredBoon);
            }
            return ret;
        }
        private bool TheKidIsAlive()
        {
            // TO DO: This function belongs somewhere else, like CharacterDataController, GameController or RunController
            bool ret = false;

            foreach (HexCharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
            {
                if (character.background.backgroundType == CharacterBackground.TheKid)
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
                AudioManager.Instance.PlaySound(Sound.UI_Button_Click);
                HandleAllChoiceEffects(button.MyChoiceData);
            }

        }

        #endregion

        #region Handle Choices
        private void HandleAllChoiceEffects(StoryEventChoiceSO choice)
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
                    CurrentStoryEvent = null;
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
                    (effect.goldGained.ToString(), TextLogic.blueNumber) + " " + TextLogic.ReturnColoredText("Gold", TextLogic.neutralYellow) + ".", ResultRowIcon.GoldCoins);
                currentResultItems.Add(newResultItem);
            }
            else if (effect.effectType == StoryChoiceEffectType.LoseGold)
            {
                PlayerDataController.Instance.ModifyPlayerGold(-effect.goldLost);
                StoryEventResultItem newResultItem = new StoryEventResultItem("Lost " + TextLogic.ReturnColoredText
                    (effect.goldLost.ToString(), TextLogic.blueNumber) + " " + TextLogic.ReturnColoredText("Gold", TextLogic.neutralYellow) + ".", ResultRowIcon.GoldCoins);
                currentResultItems.Add(newResultItem);
            }
            else if (effect.effectType == StoryChoiceEffectType.LoseAllGold)
            {
                int goldLost = PlayerDataController.Instance.CurrentGold;
                PlayerDataController.Instance.ModifyPlayerGold(-PlayerDataController.Instance.CurrentGold);
                StoryEventResultItem newResultItem = new StoryEventResultItem("Lost " + TextLogic.ReturnColoredText
                    (goldLost.ToString(), TextLogic.blueNumber) + " " + TextLogic.ReturnColoredText("Gold", TextLogic.neutralYellow) + ".", ResultRowIcon.GoldCoins);
                currentResultItems.Add(newResultItem);
            }
            else if(effect.effectType == StoryChoiceEffectType.GainBoon)
            {
                BoonData boonGained = new BoonData(BoonController.Instance.GetBoonDataByTag(effect.boonGained));
                BoonController.Instance.HandleGainBoon(boonGained);
                StoryEventResultItem newResultItem = new StoryEventResultItem("Gained boon: " + TextLogic.ReturnColoredText(boonGained.boonDisplayName, TextLogic.neutralYellow) + ".", ResultRowIcon.FramedSprite, boonGained.BoonSprite);
                currentResultItems.Add(newResultItem);
            }
            else if (effect.effectType == StoryChoiceEffectType.GainItem)
            {
                var ic = ItemController.Instance;
                ItemData item = ic.GenerateNewItemWithRandomEffects(ic.GetItemDataByName(effect.itemGained.itemName));
                InventoryController.Instance.AddItemToInventory(item);

                StoryEventResultItem newResultItem = new StoryEventResultItem("Gained item: " + TextLogic.ReturnColoredText(item.itemName, TextLogic.neutralYellow) +".", ResultRowIcon.FramedSprite, item.ItemSprite);
                currentResultItems.Add(newResultItem);
            }
            else if (effect.effectType == StoryChoiceEffectType.AddRecruitsToTavern)
            {
                BackgroundData bgData = CharacterDataController.Instance.GetBackgroundData(effect.backgroundAddedToTavern);
                for(int i = 0; i < effect.totalCharactersAddedToTavern; i++)
                {
                    HexCharacterData newCharacter = CharacterDataController.Instance.GenerateRecruitCharacter(bgData);
                    TownController.Instance.HandleAddNewRecruitToTavernFromStoryEvent(newCharacter);
                    string message = newCharacter.myName + " " + newCharacter.mySubName + " added to the tavern.";
                    StoryEventResultItem newResultItem = new StoryEventResultItem(message, ResultRowIcon.FramedSprite, bgData.backgroundSprite);
                    currentResultItems.Add(newResultItem);
                }
            }
            else if(effect.effectType == StoryChoiceEffectType.CharacterKilled)
            {
                // todo: determine target correctly
                HexCharacterData target = characterTargets[effect.characterTargetIndex];
                CharacterDataController.Instance.RemoveCharacterFromRoster(target);
                string message = target.myName + " " + target.mySubName + " has died.";
                StoryEventResultItem newResultItem = new StoryEventResultItem(message, ResultRowIcon.Skull);
                currentResultItems.Add(newResultItem);
            }
            else if (effect.effectType == StoryChoiceEffectType.GainExperience)
            {
                HexCharacterData target = characterTargets[effect.characterTargetIndex];
                int xpGainedActual = CharacterDataController.Instance.HandleGainXP(target, effect.experienceGained, true);
                StoryEventResultItem newResultItem = new StoryEventResultItem(
                    target.myName + " " + target.mySubName + " gained " + 
                    TextLogic.ReturnColoredText(xpGainedActual.ToString(), TextLogic.blueNumber) + " " + TextLogic.ReturnColoredText("Experience", TextLogic.neutralYellow) + ".", ResultRowIcon.Star);
                currentResultItems.Add(newResultItem);
            }
            else if (effect.effectType == StoryChoiceEffectType.GainPerk)
            {
                int roll = RandomGenerator.NumberBetween(1, 100);
                if (roll > effect.gainPerkChance) return;

                int stacks = 1;
                HexCharacterData target = characterTargets[effect.characterTargetIndex];
                PerkIconData perkData = PerkController.Instance.GetPerkIconDataByTag(effect.perkGained);
                if (perkData.isInjury) stacks = RandomGenerator.NumberBetween(perkData.minInjuryDuration, perkData.maxInjuryDuration);                
                PerkController.Instance.ModifyPerkOnCharacterData(target.passiveManager, perkData.perkTag, stacks);

                StoryEventResultItem newResultItem = new StoryEventResultItem(
                    target.myName + " " + target.mySubName + " gained passive: " + TextLogic.ReturnColoredText(perkData.passiveName, TextLogic.neutralYellow) + ".", ResultRowIcon.FramedSprite, perkData.passiveSprite);
                currentResultItems.Add(newResultItem);
            }
            else if (effect.effectType == StoryChoiceEffectType.GainRandomInjury)
            {
                int roll = RandomGenerator.NumberBetween(1, 100);
                if (roll > effect.gainPerkChance) return;

                HexCharacterData target = characterTargets[effect.characterTargetIndex];
                PerkIconData injuryData = PerkController.Instance.GetRandomValidInjury(target.passiveManager, effect.injurySeverity, effect.injuryType);

                int injuryStacks = RandomGenerator.NumberBetween(injuryData.minInjuryDuration, injuryData.maxInjuryDuration);
                PerkController.Instance.ModifyPerkOnCharacterData(target.passiveManager, injuryData.perkTag, injuryStacks);

                // Check 'What Doesn't Kill Me Perk': gain permanent stats
                if (PerkController.Instance.DoesCharacterHavePerk(target.passiveManager, Perk.WhatDoesntKillMe))                
                    PerkController.Instance.ModifyPerkOnCharacterData(target.passiveManager, Perk.WhatDoesntKillMe, 1);
                
                StoryEventResultItem newResultItem = new StoryEventResultItem(
                    target.myName + " " + target.mySubName + " gained injury: " + TextLogic.ReturnColoredText(injuryData.passiveName, TextLogic.neutralYellow) + ".", ResultRowIcon.FramedSprite, injuryData.passiveSprite);
                currentResultItems.Add(newResultItem);
            }
            else if (effect.effectType == StoryChoiceEffectType.LoseHealth)
            {
                HexCharacterData target = characterTargets[effect.characterTargetIndex];
                float healthLostFlatFloat = target.currentHealth * effect.healthLostPercentage;
                int healthLostFlatInt = (int) healthLostFlatFloat;
                CharacterDataController.Instance.SetCharacterHealth(target, target.currentHealth - healthLostFlatInt);
                StoryEventResultItem newResultItem = new StoryEventResultItem(
                    target.myName + " " + target.mySubName + " lost " + TextLogic.ReturnColoredText(healthLostFlatInt.ToString(), TextLogic.blueNumber)
                    + " " + TextLogic.ReturnColoredText("Health", TextLogic.neutralYellow) + ".", ResultRowIcon.UnframedSprite, SpriteLibrary.Instance.GetAttributeSprite(CoreAttribute.Constitution));
                currentResultItems.Add(newResultItem);
            }
            else if (effect.effectType == StoryChoiceEffectType.IncreaseDailyWageAll)
            {
                List<HexCharacterData> characters = new List<HexCharacterData>();
                characters.AddRange(CharacterDataController.Instance.AllPlayerCharacters);

                for (int i = 0; i < characters.Count; i++)
                {
                    int currentWage = characters[i].dailyWage;
                    int newWage = (int)(characters[i].dailyWage * (effect.wageIncreasePercentage + 1f));
                    characters[i].dailyWage = newWage;
                    StoryEventResultItem newResultItem = new StoryEventResultItem(
                    characters[i].myName + " " + characters[i].mySubName + " daily wage increased from " + TextLogic.ReturnColoredText(currentWage.ToString(), TextLogic.blueNumber)
                    + " to " + TextLogic.ReturnColoredText(newWage.ToString(), TextLogic.blueNumber) + ".", ResultRowIcon.GoldCoins);
                    currentResultItems.Add(newResultItem);
                }
            }
            else if (effect.effectType == StoryChoiceEffectType.CharactersLeave)
            {
                // Get targets
                List<HexCharacterData> prospects = new List<HexCharacterData>();
                prospects.AddRange(CharacterDataController.Instance.AllPlayerCharacters);
                prospects.Shuffle();
                int charactersEffectedActualCount = 0;

                for (int i = 0; i < prospects.Count; i++)
                {
                    int roll = RandomGenerator.NumberBetween(1, 100);
                    if (roll > effect.characterLeaveProbability) continue;
                    CharacterDataController.Instance.RemoveCharacterFromRoster(prospects[i]);

                    StoryEventResultItem newResultItem = new StoryEventResultItem(
                    prospects[i].myName + " " + prospects[i].mySubName + " left the company.", ResultRowIcon.Skull);
                    currentResultItems.Add(newResultItem);
                    charactersEffectedActualCount += 1;
                }

                // Make sure atleast 1 character was affected
                if (charactersEffectedActualCount == 0 && prospects.Count > 0)
                {
                    CharacterDataController.Instance.RemoveCharacterFromRoster(prospects[0]);
                    StoryEventResultItem newResultItem = new StoryEventResultItem(
                    prospects[0].myName + " " + prospects[0].mySubName + " left the company.", ResultRowIcon.Skull);
                    currentResultItems.Add(newResultItem);
                }
            }
            else if (effect.effectType == StoryChoiceEffectType.GainPerkAll)
            {
                // Get targets
                List<HexCharacterData> prospects = new List<HexCharacterData>();
                prospects.AddRange(CharacterDataController.Instance.AllPlayerCharacters); 
                prospects.Shuffle();

                int charactersEffectedActualCount = 0;
                int stacks = 1;
                PerkIconData perkData = PerkController.Instance.GetPerkIconDataByTag(effect.perkGained);
                if (perkData.isInjury) stacks = RandomGenerator.NumberBetween(perkData.minInjuryDuration, perkData.maxInjuryDuration);

                for (int i = 0; i < prospects.Count; i++)
                {
                    int roll = RandomGenerator.NumberBetween(1, 100);
                    if (roll > effect.gainPerkChance) continue;
                    PerkController.Instance.ModifyPerkOnCharacterData(prospects[i].passiveManager, perkData.perkTag, stacks);
                    charactersEffectedActualCount++;

                    StoryEventResultItem newResultItem = new StoryEventResultItem(
                    prospects[i].myName + " " + prospects[i].mySubName + " gained passive: " + TextLogic.ReturnColoredText(perkData.passiveName, TextLogic.neutralYellow), ResultRowIcon.FramedSprite, perkData.passiveSprite);
                    currentResultItems.Add(newResultItem);
                }

                // Make sure atleast 1 character was affected
                if(charactersEffectedActualCount == 0 && prospects.Count > 0)
                {
                    PerkController.Instance.ModifyPerkOnCharacterData(prospects[0].passiveManager, perkData.perkTag, 1);
                    StoryEventResultItem newResultItem = new StoryEventResultItem(
                    prospects[0].myName + " " + prospects[0].mySubName + " gained passive: " + TextLogic.ReturnColoredText(perkData.passiveName, TextLogic.neutralYellow), ResultRowIcon.FramedSprite, perkData.passiveSprite);
                    currentResultItems.Add(newResultItem);
                }
            }
            else if (effect.effectType == StoryChoiceEffectType.RecoverStressAll)
            {
                // Get targets
                List<HexCharacterData> prospects = new List<HexCharacterData>();
                prospects.AddRange(CharacterDataController.Instance.AllPlayerCharacters);

                for (int i = 0; i < prospects.Count; i++)
                {
                    int roll = RandomGenerator.NumberBetween(1, 100);
                    if (roll > effect.stressRecoveryChance) continue;

                    int stressRecovered = RandomGenerator.NumberBetween(effect.stressRecoveredMin, effect.stressRecoveredMax);
                    CharacterDataController.Instance.SetCharacterStress(prospects[i], prospects[i].currentStress - stressRecovered);

                    StoryEventResultItem newResultItem = new StoryEventResultItem(
                    prospects[i].myName + " " + prospects[i].mySubName + " reduced " + TextLogic.ReturnColoredText("Stress", TextLogic.neutralYellow) + " by " + 
                    TextLogic.ReturnColoredText(stressRecovered.ToString(), TextLogic.blueNumber), ResultRowIcon.UnframedSprite, SpriteLibrary.Instance.GetStressStateSprite(StressState.Confident));
                    currentResultItems.Add(newResultItem);
                }
            }
            else if (effect.effectType == StoryChoiceEffectType.GainStressAll)
            {
                // Get targets
                List<HexCharacterData> prospects = new List<HexCharacterData>();
                prospects.AddRange(CharacterDataController.Instance.AllPlayerCharacters);

                for (int i = 0; i < prospects.Count; i++)
                {
                    int roll = RandomGenerator.NumberBetween(1, 100);
                    if (roll > effect.stressRecoveryChance) continue;

                    int stressGained = RandomGenerator.NumberBetween(effect.stressRecoveredMin, effect.stressRecoveredMax);
                    CharacterDataController.Instance.SetCharacterStress(prospects[i], prospects[i].currentStress + stressGained);

                    StoryEventResultItem newResultItem = new StoryEventResultItem(
                    prospects[i].myName + " " + prospects[i].mySubName + " increased " + TextLogic.ReturnColoredText("Stress", TextLogic.neutralYellow) + " by " +
                    TextLogic.ReturnColoredText(stressGained.ToString(), TextLogic.blueNumber), ResultRowIcon.UnframedSprite, SpriteLibrary.Instance.GetStressStateSprite(StressState.Nervous));
                    currentResultItems.Add(newResultItem);
                }
            }
            else if(effect.effectType == StoryChoiceEffectType.CharacterJoinsRoster)
            {
                HexCharacterData character = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(effect.characterJoining);
                BackgroundData bgData = CharacterDataController.Instance.GetBackgroundData(character.background.backgroundType);
                if(bgData != null)
                {
                    int startingLevelBoosts = RandomGenerator.NumberBetween(bgData.lowerLevelLimit, bgData.upperLevelLimit) - character.currentLevel;
                    for (int i = 0; i < startingLevelBoosts; i++)
                        CharacterDataController.Instance.HandleLevelUp(character);
                }
                
                CharacterDataController.Instance.AddCharacterToRoster(character);                
                string message = character.myName + " " + character.mySubName + " joined the company.";
                StoryEventResultItem newResultItem = new StoryEventResultItem(message, ResultRowIcon.Star);
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

            for(int i = 0; i < validProspects.Count; i++)
            {
                Debug.Log("TryFindSuitableCharacterTarget() propective target " + (i + 1).ToString() + ": " + validProspects[i].myName + " " + validProspects[i].mySubName);
            }

            return ret;
        }
        private void DetermineAndCacheCharacterTargetsOnEventStart(StoryEventDataSO storyEvent)
        {
            choiceCharacterTarget = null;
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