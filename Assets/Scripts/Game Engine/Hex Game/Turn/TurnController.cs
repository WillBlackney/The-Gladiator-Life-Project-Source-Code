using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using WeAreGladiators.Characters;
using WeAreGladiators.Combat;
using WeAreGladiators.Utilities;
using WeAreGladiators.CameraSystems;
using WeAreGladiators.VisualEvents;
using WeAreGladiators.UCM;
using WeAreGladiators.Audio;
using WeAreGladiators.Perks;
using UnityEngine.TextCore.Text;

namespace WeAreGladiators.TurnLogic
{
    public class TurnController : Singleton<TurnController>
    {

        #region Properties + Component References
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

        #region Getters + Accessors
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
            if (activationOrder.Contains(entity)) activationOrder.Remove(entity);
            
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
        public int GetCharacterTurnsUntilTheirTurn(HexCharacterModel character)
        {
            if (!activationOrder.Contains(character)) return 0;
            else return activationOrder.IndexOf(character) - activationOrder.IndexOf(entityActivated);            
        }
        public HexCharacterModel LastToActivate
        {
            get
            {
                return ActivationOrder[ActivationOrder.Count - 1];
            }
        }
        #endregion
        
        #region Setup + Initializaton
        public void CreateTurnWindow(HexCharacterModel entity)
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
                CharacterModeller.ApplyItemSetToCharacterModelView(entity.characterData.itemSet, newWindowScript.myUCM);
            }
            else CharacterModeller.BuildModelFromModelClone(newWindowScript.myUCM, entity.hexCharacterView.ucm);

            // play window still anim on ucm
            newWindowScript.myUCM.SetBaseAnim();

        }
        #endregion
        
        #region Turn Events
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
            VisualEventManager.CreateVisualEvent(() => SetPanelArrowViewState(false));

            // Move windows to start positions if combat has only just started
            if (CurrentTurn == 0)
            {
                // Hide activation windows
                foreach (HexCharacterModel character in ActivationOrder)
                {
                    character.hexCharacterView.myActivationWindow.Hide();
                }

                // Wait for character move on screen animations to finish
                VisualEventManager.InsertTimeDelayInQueue(1.5f);

                TaskTracker combatStartNotif = new TaskTracker();
                VisualEventManager.CreateVisualEvent(() => DisplayCombatStartNotification(combatStartNotif)).SetCoroutineData(combatStartNotif);

                // Enable activation window visibility
                VisualEventManager.CreateVisualEvent(() =>
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
                VisualEventManager.CreateVisualEvent(() => MoveAllWindowsToStartPositions(characters1)).SetEndDelay(0.5f);
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
            TaskTracker rollsCoroutine = new TaskTracker();
            VisualEventManager.CreateVisualEvent(() => PlayActivationRollSequence(characters2, rollsCoroutine)).SetCoroutineData(rollsCoroutine);

            // Move windows to new positions
            var cachedOrder = ActivationOrder.ToList();
            VisualEventManager.CreateVisualEvent(() => UpdateWindowPositions(cachedOrder)).SetEndDelay(1);

            // Play turn change notification
            TaskTracker turnNotificationCoroutine = new TaskTracker();
            VisualEventManager.CreateVisualEvent(() => DisplayTurnChangeNotification(turnNotificationCoroutine)).SetCoroutineData(turnNotificationCoroutine);
            
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

        #region Logic + Calculations
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
            var cachedOrder = ActivationOrder.ToList();
            VisualEventManager.CreateVisualEvent(() => UpdateWindowPositions(cachedOrder));
        }
        #endregion

        #region Player Input + UI interactions
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
                AudioManager.Instance.PlaySound(Sound.UI_Heavy_Click);

                // Trigger character on activation end sequence and events
                HexCharacterController.Instance.CharacterOnTurnEnd(EntityActivated);
            }
        }
        public void OnDelayTurnButtonClicked()
        {
            Debug.Log("TurnController.OnDelayTurnButtonClicked() called...");
            if (CombatUIController.Instance.delayTurnButton.interactable == true &&
                EntityActivated != activationOrder[activationOrder.Count - 1] &&
                CombatController.Instance.CurrentCombatState == CombatGameState.CombatActive &&
                EntityActivated.controller == Controller.Player &&
                EntityActivated.activationPhase == ActivationPhase.ActivationPhase &&
                EntityActivated.hasRequestedTurnDelay == false)
            {
                CombatUIController.Instance.SetEndDelayTurnButtonInteractions(false);
                EntityActivated.hasRequestedTurnDelay = true;

                // Mouse click SFX
                AudioManager.Instance.PlaySound(Sound.UI_Heavy_Click);

                // Move this character to the end of the turn order.
                HandleMoveCharacterToEndOfTurnOrder(EntityActivated);
                var cachedOrder = ActivationOrder.ToList();
                VisualEventManager.CreateVisualEvent(() => UpdateWindowPositions(cachedOrder));

                // Trigger character on activation end sequence and events
                HexCharacterController.Instance.CharacterOnTurnEnd(EntityActivated, true);
            }
        }
        private void SetActivationWindowsParentViewState(bool onOrOff)
        {
            Debug.Log("ActivationManager.SetActivationWindowsParentViewState() called...");
            activationPanelParent.SetActive(onOrOff);
        }
        #endregion

        #region Entity / Activation related
        private void ActivateEntity(HexCharacterModel entity)
        {
            Debug.Log("Activating entity: " + entity.myName);
            EntityActivated = entity;
            HexCharacterModel cachedEntityRef = entity;
            entity.hasMadeTurn = true;

            // Move arrow to point at activated enemy
            GameObject panelSlot = panelSlots[activationOrder.IndexOf(cachedEntityRef)];
            VisualEventManager.CreateVisualEvent(() => MoveActivationArrowTowardsEntityWindow(cachedEntityRef, panelSlot));

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

                    var cachedOrder = ActivationOrder.ToList();
                    GameObject panelSlot = panelSlots[activationOrder.IndexOf(nextEntityToActivate)];
                    VisualEventManager.CreateVisualEvent(() => UpdateWindowPositions(cachedOrder));
                    VisualEventManager.CreateVisualEvent(() => MoveActivationArrowTowardsEntityWindow(nextEntityToActivate, panelSlot));

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
        private void PlayActivationRollSequence(HexCharacterModel[] characters, TaskTracker cData)
        {
            StartCoroutine(PlayActivationRollSequenceCoroutine(characters, cData));
        }
        private IEnumerator PlayActivationRollSequenceCoroutine(HexCharacterModel[] characters, TaskTracker cData)
        {
            // Disable arrow to prevtn blocking numbers
            //panelArrow.SetActive(false);
            SetPanelArrowViewState(false);

            // start number rolling sfx
            AudioManager.Instance.PlaySound(Sound.UI_Rolling_Bells);

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
                AudioManager.Instance.PlaySound(Sound.UI_Chime_1);

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
            AudioManager.Instance.StopSound(Sound.UI_Rolling_Bells);

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
        private void FadeOutAndDestroyActivationWindow(TurnWindow window)
        {
            GameObject slotDestroyed = panelSlots[panelSlots.Count - 1];
            RemoveEntityFromActivationOrder(window.myCharacter);            

            // Remove slot from list and destroy
            panelSlots.Remove(slotDestroyed);
            Destroy(slotDestroyed);

            // Destroy window GO
            DestroyActivationWindow(window);         
        }
        private void DestroyActivationWindow(TurnWindow window)
        {
            Destroy(window.gameObject);
        }
        public void OnCharacterKilledVisualEvent(TurnWindow window, HexCharacterModel currentlyActivated, List<HexCharacterModel> cachedOrder)
        {
            // Need to cache the currently activated entity in a new variable called 'currentlyActivated'.
            // this makes sure the arrow points to the window of the character that is VISUALLY activated,
            // but not activated in the logic side.
            //StartCoroutine(OnCharacterKilledVisualEventCoroutine(window, currentlyActivated, cData));

            FadeOutAndDestroyActivationWindow(window);
            UpdateWindowPositions(cachedOrder);

            // If the entity that just died wasn't killed during its activation, do this
            if (cachedOrder.Contains(currentlyActivated))
                MoveActivationArrowTowardsEntityWindow(cachedOrder[cachedOrder.IndexOf(currentlyActivated)]);

            else if (cachedOrder.Contains(EntityActivated))
                MoveActivationArrowTowardsEntityWindow(cachedOrder[cachedOrder.IndexOf(EntityActivated)]);

        }
        #endregion

        // Update window position visual events
        #region
        public void UpdateWindowPositions(List<HexCharacterModel> cachedOrder = null)
        {
            if (cachedOrder == null) cachedOrder = activationOrder;
            cachedOrder.ForEach(x => MoveWindowTowardsSlotPosition(x, cachedOrder));      
        }
        private void MoveWindowTowardsSlotPosition(HexCharacterModel character, List<HexCharacterModel> cachedActivationOrder)
        {
            // Get panel slot
            GameObject panelSlot = panelSlots[cachedActivationOrder.IndexOf(character)];

            // Cache window
            TurnWindow window = character.hexCharacterView.myActivationWindow;

            // Do we have everything needed to move?
            if (panelSlot && window)
            {
                // Move the window
                window.transform.DOKill();
                window.transform.DOMoveX(panelSlot.transform.position.x, 0.25f);
            }
        }
        private void MoveAllWindowsToStartPositions(HexCharacterModel[] characters)
        {
            for (int i = 0; i < characters.Length; i++)
                characters[i].hexCharacterView.myActivationWindow.transform.DOMoveX(panelSlots[i].transform.position.x, 0.25f);
        }
        #endregion

        // Arrow pointer visual events
        #region
        private void SetPanelArrowViewState(bool onOrOff)
        {
            panelArrow.SetActive(onOrOff);
        }
        public void MoveActivationArrowTowardsEntityWindow(HexCharacterModel character, GameObject panelSlot = null)
        {
            Debug.Log("ActivationManager.MoveActivationArrowTowardsPosition() called...");

            if (panelSlot == null && activationOrder.Contains(character)) panelSlot = panelSlots[activationOrder.IndexOf(character)];            

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
        private void DisplayTurnChangeNotification(TaskTracker cData)
        {
            StartCoroutine(DisplayTurnChangeNotificationCoroutine(cData));
        }
        private IEnumerator DisplayTurnChangeNotificationCoroutine(TaskTracker cData)
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
            if (cData != null) cData.MarkAsCompleted();            
        }
        private void DisplayCombatStartNotification(TaskTracker cData)
        {
            StartCoroutine(DisplayCombatStartNotificationCoroutine(cData));
        }
        private IEnumerator DisplayCombatStartNotificationCoroutine(TaskTracker cData)
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
        public void DisplayCustomNotification(TaskTracker cData, string customMessage)
        {
            StartCoroutine(DisplayCustomNotificationCoroutine(cData, customMessage));
        }
        private IEnumerator DisplayCustomNotificationCoroutine(TaskTracker cData, string customMessage)
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