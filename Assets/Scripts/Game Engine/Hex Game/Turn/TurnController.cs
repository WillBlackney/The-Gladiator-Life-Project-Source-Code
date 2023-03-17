﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using HexGameEngine.Characters;
using HexGameEngine.Combat;
using HexGameEngine.Utilities;
using HexGameEngine.CameraSystems;
using HexGameEngine.VisualEvents;
using HexGameEngine.UCM;
using HexGameEngine.Audio;
using HexGameEngine.Perks;

namespace HexGameEngine.TurnLogic
{
    public class TurnController : Singleton<TurnController>
    {

        // Properties + Component References
        #region
        [Header("Component References")]
        [SerializeField] private GameObject windowStartPos;
        [SerializeField] private GameObject activationPanelParent;
        [SerializeField] private GameObject panelArrow;
        [SerializeField] private GameObject activationSlotContentParent;
        [SerializeField] private GameObject activationWindowContentParent;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Turn Change Component References")]
        [SerializeField] private TextMeshProUGUI whoseTurnText;
        [SerializeField] private CanvasGroup visualParentCG;
        [SerializeField] private RectTransform blackBarImageRect;
        [SerializeField] private RectTransform middlePos;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Combat Start Screen Components")]
        [SerializeField] private GameObject shieldParent;
        [SerializeField] private Transform leftSwordRect;
        [SerializeField] private Transform rightSwordRect;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [SerializeField] private Transform leftSwordStartPos;
        [SerializeField] private Transform rightSwordStartPos;
        [SerializeField] private Transform swordEndPos;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Turn Change Properties")]
        [SerializeField] private float alphaChangeSpeed;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Variables")]
        private List<HexCharacterModel> activationOrder = new List<HexCharacterModel>();
        private List<GameObject> panelSlots = new List<GameObject>();
        private HexCharacterModel entityActivated;
        private int currentTurn;
        #endregion

        // Getters + Accessors
        #region
        public HexCharacterModel EntityActivated
        {
            get
            {
                return entityActivated;
            }
            private set
            {
                entityActivated = value;
            }
        }
        public int CurrentTurn
        {
            get { return currentTurn; }
            private set { currentTurn = value; }
        }
        public List<HexCharacterModel> ActivationOrder
        {
            get { return activationOrder; }
        }
        public void RemoveEntityFromActivationOrder(HexCharacterModel entity)
        {
            if (activationOrder.Contains(entity))
            {
                activationOrder.Remove(entity);
            }
        }
        public void AddEntityToActivationOrder(HexCharacterModel entity)
        {
            activationOrder.Add(entity);
        }
        public void DisablePanelSlotAtIndex(int index)
        {
            panelSlots[index].SetActive(false);
        }
        public void EnablePanelSlotAtIndex(int index)
        {
            panelSlots[index].SetActive(true);
        }
        #endregion

        // Setup + Initializaton
        #region 
        public void CreateActivationWindow(HexCharacterModel entity)
        {
            // Create slot
            GameObject newSlot = Instantiate(PrefabHolder.Instance.PanelSlotPrefab, activationSlotContentParent.transform);
            panelSlots.Add(newSlot);

            // Create window
            GameObject newWindow = Instantiate(PrefabHolder.Instance.ActivationWindowPrefab, activationWindowContentParent.transform);
            newWindow.transform.position = windowStartPos.transform.position;

            // Set up window + connect component references
            TurnWindow newWindowScript = newWindow.GetComponent<TurnWindow>();
            newWindowScript.myCharacter = entity;
            entity.hexCharacterView.myActivationWindow = newWindowScript;

            // Enable panel view
            newWindowScript.gameObject.SetActive(false);
            newWindowScript.gameObject.SetActive(true);

            // add character to activation order list
            AddEntityToActivationOrder(entity);

            // Build window UCM
            if(entity.characterData != null)
            {
                CharacterModeller.BuildModelFromStringReferences(newWindowScript.myUCM, entity.characterData.modelParts);
                CharacterModeller.ApplyItemSetToCharacterModelView(entity.characterData.itemSet, newWindowScript.myUCM, false);
            }
            else CharacterModeller.BuildModelFromModelClone(newWindowScript.myUCM, entity.hexCharacterView.ucm);

            // play window still anim on ucm
            newWindowScript.myUCM.SetBaseAnim();

        }
        #endregion

        // Turn Events
        #region
        public void OnNewCombatEventStarted()
        {
            CurrentTurn = 0;
            CombatController.Instance.SetCombatState(CombatGameState.CombatActive);

            // Apply starting perks (elf, human and orc racials, etc)
            foreach (HexCharacterModel c in activationOrder)
                HexCharacterController.Instance.ApplyCombatStartPerkEffects(c);
            // Start first turn
            StartNewTurnSequence();
        }
        private void StartNewTurnSequence()
        {
            // Tie off and block any stack visual events on all characters
            foreach (HexCharacterModel c in HexCharacterController.Instance.AllCharacters)
            {
                VisualEvent stack = c.GetLastStackEventParent();
                if (stack != null)
                {
                    Debug.Log("Closing stack event parent");
                    stack.isClosed = true;
                }
            }

            // Disable arrow
            VisualEventManager.Instance.CreateVisualEvent(() => SetPanelArrowViewState(false));

            // Move windows to start positions if combat has only just started
            if (CurrentTurn == 0)
            {
                // Hide activation windows
                foreach (HexCharacterModel character in ActivationOrder)
                {
                    character.hexCharacterView.myActivationWindow.Hide();
                }

                // Wait for character move on screen animations to finish
                VisualEventManager.Instance.InsertTimeDelayInQueue(1.5f);

                CoroutineData combatStartNotif = new CoroutineData();
                VisualEventManager.Instance.CreateVisualEvent(() => DisplayCombatStartNotification(combatStartNotif), combatStartNotif);

                // Enable activation window visibility
                VisualEventManager.Instance.CreateVisualEvent(() =>
                {
                    SetActivationWindowsParentViewState(true);

                    // Show activation windows
                    foreach (HexCharacterModel character in ActivationOrder)
                    {
                        character.hexCharacterView.myActivationWindow.Show();
                    }

                });

                // Play window move anims
                HexCharacterModel[] characters1 = activationOrder.ToArray();
                VisualEventManager.Instance.CreateVisualEvent(() => MoveAllWindowsToStartPositions(characters1), QueuePosition.Back, 0f, 0.5f);
            }

            // Increment turn count
            CurrentTurn++;

            // Resolve each entity's OnNewTurnCycleStarted events       
            foreach (HexCharacterModel entity in HexCharacterController.Instance.AllCharacters)
            {
                HexCharacterController.Instance.CharacterOnNewTurnCycleStarted(entity);
            }

            // Re-roll for initiative
            HexCharacterModel[] characters = activationOrder.ToArray();
            GenerateInitiativeRolls();
            SetActivationOrderBasedOnCurrentInitiativeRolls();

            // Reset turn delay properties
            foreach(HexCharacterModel c in activationOrder)
            {
                c.hasMadeDelayedTurn = false;
                if (c.hasDelayedPreviousTurn) c.hasDelayedPreviousTurn = false;
                if (c.hasRequestedTurnDelay)
                {
                    c.hasRequestedTurnDelay = false;
                    c.hasDelayedPreviousTurn = true;
                }
            }

            // Play roll animation sequence
            HexCharacterModel[] characters2 = activationOrder.ToArray();
            CoroutineData rollsCoroutine = new CoroutineData();
            VisualEventManager.Instance.CreateVisualEvent(() => PlayActivationRollSequence(characters2, rollsCoroutine), rollsCoroutine);

            // Move windows to new positions
            VisualEventManager.Instance.CreateVisualEvent(() => UpdateWindowPositions(), QueuePosition.Back, 0, 1);

            // Play turn change notification
            CoroutineData turnNotificationCoroutine = new CoroutineData();
            VisualEventManager.Instance.CreateVisualEvent(() => DisplayTurnChangeNotification(turnNotificationCoroutine), turnNotificationCoroutine);

            // Enable end turn + delay button visual event
            /*
            VisualEventManager.Instance.CreateVisualEvent(() => 
            { 
                EnableEndTurnButtonView();
                EnableDelayTurnButtonView();
            });
            */
            // Activate the first character in the turn cycle
            ActivateEntity(activationOrder[0]);
        }
        public void DestroyAllActivationWindows()
        {
            foreach (HexCharacterModel entity in activationOrder)
            {
                if (entity.hexCharacterView.myActivationWindow != null)
                {
                    // NOTE: maybe this should be a scheduled visual event?
                    DestroyActivationWindow(entity.hexCharacterView.myActivationWindow);
                }
            }

            if (panelSlots.Count > 1)
            {
                for (int i = panelSlots.Count - 1; i >= 0; i--)
                {
                    Destroy(panelSlots[i]);
                }
            }
            else if (panelSlots.Count == 1)
            {
                Destroy(panelSlots[0]);
            }

            activationOrder.Clear();
            panelSlots.Clear();

            // Hide activation arrow
            SetPanelArrowViewState(false);

        }
        #endregion

        // Logic + Calculations
        #region
        private int CalculateInitiativeRoll(HexCharacterModel entity)
        {
            int roll = StatCalculator.GetTotalInitiative(entity);
            if (roll < 0) roll = 0;
            return roll + RandomGenerator.NumberBetween(1, 3);
        }
        private void GenerateInitiativeRolls()
        {
            foreach (HexCharacterModel entity in activationOrder)
            {
                entity.currentInitiativeRoll = CalculateInitiativeRoll(entity);
            }
        }
        private void SetActivationOrderBasedOnCurrentInitiativeRolls()
        {
            // Re arrange the activation order list based on the entity rolls
            List<HexCharacterModel> sortedList = activationOrder.OrderBy(entity => entity.currentInitiativeRoll).ToList();
            List<HexCharacterModel> inattentiveCharacters = new List<HexCharacterModel>();
            sortedList.Reverse();
            foreach(HexCharacterModel c in sortedList)
            {
                if(PerkController.Instance.DoesCharacterHavePerk(c.pManager, Perk.Inattentive))
                {
                    inattentiveCharacters.Add(c);
                }
            }
            foreach (HexCharacterModel c in inattentiveCharacters)
                sortedList.Remove(c);
            foreach (HexCharacterModel c in inattentiveCharacters)
                sortedList.Add(c);

            activationOrder = sortedList;
        }
        public void HandleMoveCharacterToEndOfTurnOrder(HexCharacterModel character)
        {
            activationOrder.Remove(character);
            activationOrder.Add(character);
            VisualEventManager.Instance.CreateVisualEvent(() => UpdateWindowPositions(), QueuePosition.Back);
        }
        #endregion

        // Player Input + UI interactions
        #region
        public void OnEndTurnButtonClicked()
        {
            Debug.Log("TurnController.OnEndTurnButtonClicked() called...");

            // wait until all visual events have completed
            // prevent function if game over sequence triggered
            if (CombatUIController.Instance.endTurnButton.interactable == true &&
                CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                 EntityActivated.controller == Controller.Player &&
                 EntityActivated.activationPhase == ActivationPhase.ActivationPhase)
            {
                CombatUIController.Instance.SetEndTurnButtonInteractions(false);

                // Mouse click SFX
                AudioManager.Instance.PlaySoundPooled(Sound.GUI_Button_Clicked);

                // Trigger character on activation end sequence and events
                HexCharacterController.Instance.CharacterOnTurnEnd(EntityActivated);
            }
        }
        public void OnDelayTurnButtonClicked()
        {
            Debug.Log("TurnController.OnDelayTurnButtonClicked() called...");
            if (CombatUIController.Instance.delayTurnButton.interactable == true &&
                EntityActivated != activationOrder[activationOrder.Count - 1])
            {
                CombatUIController.Instance.SetEndDelayTurnButtonInteractions(false);

                // prevent function if game over sequence triggered
                if (CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                     EntityActivated.controller == Controller.Player &&
                     EntityActivated.activationPhase == ActivationPhase.ActivationPhase &&
                     EntityActivated.hasRequestedTurnDelay == false)
                {
                    EntityActivated.hasRequestedTurnDelay = true;

                    // Mouse click SFX
                    AudioManager.Instance.PlaySoundPooled(Sound.GUI_Button_Clicked);

                    // Move this character to the end of the turn order.
                    HandleMoveCharacterToEndOfTurnOrder(EntityActivated);

                    // Trigger character on activation end sequence and events
                    HexCharacterController.Instance.CharacterOnTurnEnd(EntityActivated, true);
                }
            }
        }
        private void SetActivationWindowsParentViewState(bool onOrOff)
        {
            Debug.Log("ActivationManager.SetActivationWindowsParentViewState() called...");
            activationPanelParent.SetActive(onOrOff);
        }
        #endregion

        // Entity / Activation related
        #region   
        private void ActivateEntity(HexCharacterModel entity)
        {
            Debug.Log("Activating entity: " + entity.myName);
            EntityActivated = entity;
            HexCharacterModel cachedEntityRef = entity;
            entity.hasMadeTurn = true;

            // Move arrow to point at activated enemy
            VisualEventManager.Instance.CreateVisualEvent(() => MoveActivationArrowTowardsEntityWindow(cachedEntityRef), QueuePosition.Back);

            // Start character activation
            HexCharacterController.Instance.CharacterOnTurnStart(entity);

        }
        public void ActivateNextEntity()
        {
            Debug.Log("ActivationManager.ActivateNextEntity() called...");

            // Setup
            HexCharacterModel nextEntityToActivate = null;

            // dont activate next entity if either all defenders or all enemies are dead
            if (CombatController.Instance.CurrentCombatState != CombatGameState.CombatActive)
            {
                Debug.Log("ActivationManager.ActivateNextEntity() detected that an end combat event has been triggered, " +
                    "cancelling next entity activation...");
                return;
            }

            // Start a new turn if all characters have activated
            if (AllEntitiesHaveActivatedThisTurn())
            {
                StartNewTurnSequence();
            }
            else
            {
                foreach (HexCharacterModel entity in activationOrder)
                {
                    // check if the character is alive, and not yet activated this turn cycle
                    if (entity.livingState == LivingState.Alive &&
                        ((entity.hasMadeTurn == false && entity.wasSummonedThisTurn == false) || (entity.hasRequestedTurnDelay == true && entity.hasMadeDelayedTurn == false)))
                    {
                        nextEntityToActivate = entity;
                        break;
                    }
                }

                if (nextEntityToActivate != null)
                {
                    // Update all window slot positions + activation pointer arrow
                    VisualEventManager.Instance.CreateVisualEvent(() => UpdateWindowPositions());
                    VisualEventManager.Instance.CreateVisualEvent(() => MoveActivationArrowTowardsEntityWindow(nextEntityToActivate));

                    // Activate!
                    ActivateEntity(nextEntityToActivate);
                }
                else
                {
                    StartNewTurnSequence();
                }

            }
        }
        private bool AllEntitiesHaveActivatedThisTurn()
        {
            Debug.Log("ActivationManager.AllEntitiesHaveActivatedThisTurn() called...");
            bool boolReturned = true;
            foreach (HexCharacterModel entity in activationOrder)
            {
                if (entity.hasMadeTurn == false || (entity.hasMadeTurn == true && entity.hasRequestedTurnDelay == true && entity.hasMadeDelayedTurn == false))
                {
                    boolReturned = false;
                    break;
                }
            }
            return boolReturned;
        }
        #endregion


        // Visual Events
        #region

        // Number roll sequence visual events
        #region
        private void PlayActivationRollSequence(HexCharacterModel[] characters, CoroutineData cData)
        {
            StartCoroutine(PlayActivationRollSequenceCoroutine(characters, cData));
        }
        private IEnumerator PlayActivationRollSequenceCoroutine(HexCharacterModel[] characters, CoroutineData cData)
        {
            // Disable arrow to prevtn blocking numbers
            //panelArrow.SetActive(false);
            SetPanelArrowViewState(false);

            // start number rolling sfx
            AudioManager.Instance.PlaySound(Sound.GUI_Rolling_Bells);

            foreach (HexCharacterModel entity in characters)
            {
                // start animating their roll number text
                StartCoroutine(PlayRandomNumberAnim(entity.hexCharacterView.myActivationWindow));
            }

            yield return new WaitForSeconds(1);

            foreach (HexCharacterModel entity in characters)
            {
                // cache window
                TurnWindow window = entity.hexCharacterView.myActivationWindow;

                // stop the number anim
                window.animateNumberText = false;

                // set the number text as their initiative roll
                window.rollText.text = entity.currentInitiativeRoll.ToString();

                // chime ping SFX
                AudioManager.Instance.PlaySoundPooled(Sound.GUI_Chime_1);

                // do breath effect on window
                float currentScale = window.rollText.transform.localScale.x;
                float endScale = currentScale * 1.5f;
                float animSpeed = 0.25f;
                window.rollText.transform.DOScale(endScale, animSpeed).SetEase(Ease.OutQuint);
                yield return new WaitForSeconds(animSpeed);
                window.rollText.transform.DOScale(currentScale, animSpeed).SetEase(Ease.OutQuint);

                // brief yield before animating next window
                yield return new WaitForSeconds(0.1f);
            }

            // stop rolling sfx
            AudioManager.Instance.StopSound(Sound.GUI_Rolling_Bells);

            // brief yield
            yield return new WaitForSeconds(1f);

            // Disable roll number text components
            foreach (HexCharacterModel entity in characters)
            {
                entity.hexCharacterView.myActivationWindow.rollText.enabled = false;
            }

            // Resolve
            if (cData != null)
            {
                cData.MarkAsCompleted();
            }

        }
        private IEnumerator PlayRandomNumberAnim(TurnWindow window)
        {
            Debug.Log("PlayRandomNumberAnim() called....");
            int numberDisplayed = 0;
            window.animateNumberText = true;
            window.rollText.enabled = true;

            while (window.animateNumberText == true)
            {
                //Debug.Log("Animating roll number text....");
                numberDisplayed++;
                if (numberDisplayed > 9)
                {
                    numberDisplayed = 0;
                }
                window.rollText.text = numberDisplayed.ToString();

                yield return new WaitForEndOfFrame();
            }
        }
        #endregion

        // Destroy activation window visual events
        #region
        private void FadeOutAndDestroyActivationWindow(TurnWindow window, CoroutineData cData)
        {
            StartCoroutine(FadeOutAndDestroyActivationWindowCoroutine(window, cData));
        }
        private IEnumerator FadeOutAndDestroyActivationWindowCoroutine(TurnWindow window, CoroutineData cData)
        {
            while (window.myCanvasGroup.alpha > 0)
            {
                window.myCanvasGroup.alpha -= 0.05f;
                if (window.myCanvasGroup.alpha == 0)
                {
                    // Make sure the slot is found and destroyed if it exists still
                    GameObject slotDestroyed = panelSlots[panelSlots.Count - 1];
                    if (activationOrder.Contains(window.myCharacter))
                    {
                        RemoveEntityFromActivationOrder(window.myCharacter);
                    }

                    // Remove slot from list and destroy
                    panelSlots.Remove(slotDestroyed);
                    Destroy(slotDestroyed);
                }
                yield return new WaitForEndOfFrame();
            }

            // Destroy window GO
            DestroyActivationWindow(window);

            // Resolve
            if (cData != null)
            {
                cData.MarkAsCompleted();
            }

        }
        private void DestroyActivationWindow(TurnWindow window)
        {
            Destroy(window.gameObject);
        }
        public void OnCharacterKilledVisualEvent(TurnWindow window, HexCharacterModel currentlyActivated, CoroutineData cData)
        {
            // Need to cache the currently activated entity in a new variable called 'currentlyActivated'.
            // this makes sure the arrow points to the window of the character that is VISUALLY activated,
            // but not activated in the logic side.
            StartCoroutine(OnCharacterKilledVisualEventCoroutine(window, currentlyActivated, cData));
        }
        private IEnumerator OnCharacterKilledVisualEventCoroutine(TurnWindow window, HexCharacterModel currentlyActivated, CoroutineData cData)
        {
            FadeOutAndDestroyActivationWindow(window, null);
            yield return new WaitForSeconds(0.5f);

            UpdateWindowPositions();

            // If the entity that just died wasn't killed during its activation, do this
            if (activationOrder.Contains(currentlyActivated))
            {
                MoveActivationArrowTowardsEntityWindow(activationOrder[activationOrder.IndexOf(currentlyActivated)]);
            }
            else if (activationOrder.Contains(EntityActivated))
            {
                MoveActivationArrowTowardsEntityWindow(activationOrder[activationOrder.IndexOf(EntityActivated)]);
            }

            // Resolve
            if (cData != null)
            {
                cData.MarkAsCompleted();
            }

        }
        #endregion

        // Update window position visual events
        #region
        public void UpdateWindowPositions()
        {
            foreach (HexCharacterModel character in activationOrder)
            {
                MoveWindowTowardsSlotPositionCoroutine(character);
            }
        }
        private void MoveWindowTowardsSlotPositionCoroutine(HexCharacterModel character)
        {
            Debug.Log("ActivationWindow.MoveWindowTowardsSlotPositionCoroutine() called for character: " + character.myName);

            // Get panel slot
            GameObject panelSlot = panelSlots[activationOrder.IndexOf(character)];

            // cache window
            TurnWindow window = character.hexCharacterView.myActivationWindow;

            // do we have everything needed to move?
            if (panelSlot && window)
            {
                // move the window
                Sequence s = DOTween.Sequence();
                s.Append(window.transform.DOMoveX(panelSlot.transform.position.x, 0.3f));
            }
        }
        private void MoveAllWindowsToStartPositions(HexCharacterModel[] characters)
        {
            StartCoroutine(MoveAllWindowsToStartPositionsCoroutine(characters));
        }
        private IEnumerator MoveAllWindowsToStartPositionsCoroutine(HexCharacterModel[] characters)
        {
            yield return null;

            for (int i = 0; i < characters.Length; i++)
            {
                // move the window
                Sequence s = DOTween.Sequence();
                s.Append(characters[i].hexCharacterView.myActivationWindow.transform.DOMoveX(panelSlots[i].transform.position.x, 0.3f));
            }
        }
        #endregion

        // Arrow pointer visual events
        #region
        private void SetPanelArrowViewState(bool onOrOff)
        {
            panelArrow.SetActive(onOrOff);
        }
        public void MoveActivationArrowTowardsEntityWindow(HexCharacterModel character)
        {
            Debug.Log("ActivationManager.MoveActivationArrowTowardsPosition() called...");

            GameObject panelSlot = null;

            if (activationOrder.Contains(character))
            {
                panelSlot = panelSlots[activationOrder.IndexOf(character)];
            }

            if (panelSlot != null)
            {
                // Activate arrow view
                SetPanelArrowViewState(true);

                // move the arrow
                Sequence s = DOTween.Sequence();
                s.Append(panelArrow.transform.DOMoveX(panelSlot.transform.position.x, 0.2f));
            }
            else
            {
                Debug.LogWarning("ActivationManager.MoveActivationArrowTowardsEntityWindow()" +
                    " did not find the character " + character.myName + " in activation order, " +
                    "cancelling activation arrow position update");
            }

        }
        #endregion

        // Turn Change Notification visual events
        #region
        private void DisplayTurnChangeNotification(CoroutineData cData)
        {
            StartCoroutine(DisplayTurnChangeNotificationCoroutine(cData));
        }
        private IEnumerator DisplayTurnChangeNotificationCoroutine(CoroutineData cData)
        {
            // Get transforms
            RectTransform mainParent = visualParentCG.gameObject.GetComponent<RectTransform>();
            RectTransform textParent = whoseTurnText.gameObject.GetComponent<RectTransform>();

            // Set starting view state values
            shieldParent.SetActive(false);
            leftSwordRect.gameObject.SetActive(false);
            rightSwordRect.gameObject.SetActive(false);
            visualParentCG.gameObject.SetActive(true);
            mainParent.position = middlePos.position;
            textParent.localScale = new Vector3(4f, 4f, 1);
            blackBarImageRect.localScale = new Vector3(1f, 0.1f, 1);
            visualParentCG.alpha = 0;
            whoseTurnText.text = "Turn " + CurrentTurn.ToString();

            AudioManager.Instance.FadeInSound(Sound.Events_New_Turn_Notification, 0.5f);
            visualParentCG.DOFade(1f, 0.5f);
            textParent.DOScale(1.5f, 0.25f);
            blackBarImageRect.DOScaleY(1, 0.25f);

            yield return new WaitForSeconds(1.5f);

            visualParentCG.DOFade(0f, 0.5f);
            blackBarImageRect.DOScaleY(0.1f, 0.5f);

            // Resolve
            if (cData != null)
            {
                cData.MarkAsCompleted();
            }
        }
        private void DisplayCombatStartNotification(CoroutineData cData)
        {
            StartCoroutine(DisplayCombatStartNotificationCoroutine(cData));
        }
        private IEnumerator DisplayCombatStartNotificationCoroutine(CoroutineData cData)
        {
            // Get transforms
            RectTransform mainParent = visualParentCG.gameObject.GetComponent<RectTransform>();
            RectTransform textParent = whoseTurnText.gameObject.GetComponent<RectTransform>();

            // Set starting view state values
            shieldParent.SetActive(true);
            visualParentCG.gameObject.SetActive(true);
            leftSwordRect.gameObject.SetActive(true);
            rightSwordRect.gameObject.SetActive(true);
            mainParent.position = middlePos.position;
            textParent.localScale = new Vector3(4f, 4f, 1);
            blackBarImageRect.localScale = new Vector3(1f, 0.1f, 1);
            visualParentCG.alpha = 0;
            whoseTurnText.text = "Combat Start!";

            // set sword starting positions + rotations
            leftSwordRect.position = leftSwordStartPos.position;
            rightSwordRect.position = rightSwordStartPos.position;
            leftSwordRect.DORotate(new Vector3(0, 0, 0), 0f);
            rightSwordRect.DORotate(new Vector3(0, 0, 0), 0f);
            yield return new WaitForSeconds(0.05f);

            // Fade in text + bg
            visualParentCG.DOFade(1f, 0.5f);
            textParent.DOScale(1.5f, 0.25f);
            blackBarImageRect.DOScaleY(1, 0.25f);

            // move swords
            leftSwordRect.DOMoveX(swordEndPos.position.x, 0.5f);
            rightSwordRect.DOMoveX(swordEndPos.position.x, 0.5f);

            // rotate swords    
            Vector3 rightEndRotation = new Vector3(0, 0, -720);
            rightSwordRect.DORotate(rightEndRotation, 0.45f);
            Vector3 leftEndRotation = new Vector3(0, 0, -720);
            leftSwordRect.DORotate(leftEndRotation, 0.45f);

            // wait for swords to reach centre
            yield return new WaitForSeconds(0.4f);

            // SFX and camera shake when swords clash
            AudioManager.Instance.PlaySound(Sound.Ability_Metallic_Ting);
            CameraController.Instance.CreateCameraShake(CameraShakeType.Small);
            yield return new WaitForSeconds(1f);

            visualParentCG.DOFade(0f, 0.5f);
            blackBarImageRect.DOScaleY(0.1f, 0.5f);
            yield return new WaitForSeconds(0.55f);

            // Resolve
            if (cData != null)
            {
                cData.MarkAsCompleted();
            }
        }
        public void DisplayCustomNotification(CoroutineData cData, string customMessage)
        {
            StartCoroutine(DisplayCustomNotificationCoroutine(cData, customMessage));
        }
        private IEnumerator DisplayCustomNotificationCoroutine(CoroutineData cData, string customMessage)
        {
            // Get transforms
            RectTransform mainParent = visualParentCG.gameObject.GetComponent<RectTransform>();
            RectTransform textParent = whoseTurnText.gameObject.GetComponent<RectTransform>();

            // Set starting view state values
            shieldParent.SetActive(false);
            leftSwordRect.gameObject.SetActive(false);
            rightSwordRect.gameObject.SetActive(false);
            visualParentCG.gameObject.SetActive(true);
            mainParent.position = middlePos.position;
            textParent.localScale = new Vector3(4f, 4f, 1);
            blackBarImageRect.localScale = new Vector3(1f, 0.1f, 1);
            visualParentCG.alpha = 0;
            whoseTurnText.text = customMessage;

            AudioManager.Instance.FadeInSound(Sound.Events_New_Turn_Notification, 0.5f);
            visualParentCG.DOFade(1f, 0.5f);
            textParent.DOScale(1.5f, 0.25f);
            blackBarImageRect.DOScaleY(1, 0.25f);

            yield return new WaitForSeconds(1.5f);

            visualParentCG.DOFade(0f, 0.5f);
            blackBarImageRect.DOScaleY(0.1f, 0.5f);

            // Resolve
            if (cData != null)
            {
                cData.MarkAsCompleted();
            }
        }
        #endregion

        #endregion
    }
}