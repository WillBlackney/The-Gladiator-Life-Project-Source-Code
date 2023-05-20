using CardGameEngine.UCM;
using DG.Tweening;
using HexGameEngine.Abilities;
using HexGameEngine.Perks;
using HexGameEngine.TurnLogic;
using HexGameEngine.UCM;
using HexGameEngine.UI;
using HexGameEngine.Utilities;
using HexGameEngine.VisualEvents;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexGameEngine.Characters
{
    public class CombatUIController : Singleton<CombatUIController>
    {
        [Header("Core Canvas Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private CanvasGroup mainCanvasCg;     
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Positioning Components")]
        [SerializeField] private Transform middlePanelTransform;
        [SerializeField] private Transform middlePanelOnScreenPos;
        [SerializeField] private Transform middlePanelOffScreenPos;
        [PropertySpace(SpaceBefore = 10, SpaceAfter = 0)]
        [SerializeField] private Transform leftPanelTransform;
        [SerializeField] private Transform leftPanelOnScreenPos;
        [SerializeField] private Transform leftPanelOffScreenPos;
        [PropertySpace(SpaceBefore = 10, SpaceAfter = 0)]
        [SerializeField] private Transform rightPanelTransform;
        [SerializeField] private Transform rightPanelOnScreenPos;
        [SerializeField] private Transform rightPanelOffScreenPos;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Middle Section Components")]
        [SerializeField] private AbilityButton[] abilityButtons;
        [SerializeField] private EnergyPanelView energyBar;
        [SerializeField] private PerkLayoutPanel perkPanel;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Left Section Components")]
        [SerializeField] private UniversalCharacterModel uiPotraitUCM;
        [SerializeField] private TextMeshProUGUI characterNameTextUI;
        [SerializeField] private StressPanelView stressPanel;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Current Initiative Components")]
        [SerializeField] TextMeshProUGUI currentInitiativePanelText;
        [SerializeField] ModalDottedRow[] initiativeModalRows;
        [SerializeField] RectTransform[] initiativeModalLayouts;

        [Header("Health Bar UI References")]
        [SerializeField] private Slider healthBarUI;
        [SerializeField] private TextMeshProUGUI currentHealthText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Armour Bar UI References")]
        [SerializeField] private Slider armourBarUI;
        [SerializeField] private TextMeshProUGUI currentArmourText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Stress Bar UI References")]
        [SerializeField] private Slider stressBarUI;
        [SerializeField] private TextMeshProUGUI stressText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Fatigue Bar UI References")]
        [SerializeField] private Slider fatigueBarUI;
        [SerializeField] private Slider fatigueSubBarUI;
        [SerializeField] private TextMeshProUGUI fatigueText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("End + Delay Turn Button Components")]
        public Button endTurnButton;
        public Button delayTurnButton;
        public Image delayTurnButtonImage;
        public Sprite delayTurnButtonReadySprite;
        public Sprite delayTurnButtonNotReadySprite;


        #region Getters + Accessors
        public PerkLayoutPanel PerkPanel
        {
            get { return perkPanel; }
        }
        public EnergyPanelView EnergyBar
        {
            get { return energyBar; }
        }
        public TextMeshProUGUI CurrentArmourText
        {
            get { return currentArmourText; }
        }
        public AbilityButton[] AbilityButtons
        {
            get { return abilityButtons; }
        }
        public bool ViewIsActive
        {
            get 
            { 
                return mainVisualParent.activeSelf; 
            } 
        }
        #endregion

        #region Build Show, Hide and Animate Main Views
        public void BuildAndShowViewsOnTurnStart(HexCharacterModel character)
        {
            SetInteractability(true);
            mainVisualParent.SetActive(true);
                        
            middlePanelTransform.localPosition = middlePanelOffScreenPos.localPosition;
            rightPanelTransform.localPosition = rightPanelOffScreenPos.localPosition;
            leftPanelTransform.localPosition = leftPanelOffScreenPos.localPosition;

            // Ability Bar
            BuildHexCharacterAbilityBar(character);

            // Energy Bar
            energyBar.UpdateIcons(character.currentEnergy);

            // Perk Panel
            perkPanel.ResetPanel();
            perkPanel.BuildFromPerkManager(character.pManager);

            // Portrait
            characterNameTextUI.text = character.myName;
            CharacterModeller.BuildModelFromStringReferences(uiPotraitUCM, character.characterData.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, uiPotraitUCM);
            uiPotraitUCM.SetIdleAnim();

            // Health + Armour
            UpdateHealthComponents(character.currentHealth, StatCalculator.GetTotalMaxHealth(character));
            UpdateArmourComponents(character.currentArmour, character.startingArmour);

            // Stress Components
            UpdateStressComponents(character.currentStress, character);

            // Fatigue 
            UpdateFatigueComponents(character.currentFatigue, StatCalculator.GetTotalMaxFatigue(character));

            // Initiative
            UpdateCurrentInitiativeComponents(character);

            // End + Delay Turn Buttons
            BuildTurnButtons(character);

            // Fade in views
            float speed = 1f;
            CharacterModeller.FadeOutCharacterModel(uiPotraitUCM, 0);
            CharacterModeller.FadeInCharacterModel(uiPotraitUCM, speed * 0.5f);
            mainCanvasCg.DOKill();
            mainCanvasCg.alpha = 0.001f;
            mainCanvasCg.DOFade(1f, speed * 0.5f);

            // Animate on screen
            MovePanelsOnScreen(speed);
        }
        public void HideViewsOnTurnEnd(TaskTracker tracker = null, float speed = 1.25f)
        {
            SetInteractability(false);
            mainCanvasCg.DOKill();

            middlePanelTransform.localPosition = middlePanelOnScreenPos.localPosition;
            rightPanelTransform.localPosition = rightPanelOnScreenPos.localPosition;
            leftPanelTransform.localPosition = leftPanelOnScreenPos.localPosition;

            MovePanelsOffScreen(speed);
            
            // Fade out views
            CharacterModeller.FadeOutCharacterModel(uiPotraitUCM, speed);   
            DOVirtual.DelayedCall(speed * 0.5f, () =>
            {
                mainCanvasCg.DOFade(0f, speed * 0.5f).OnComplete(() =>
                {
                    mainVisualParent.SetActive(false);
                    if (tracker != null) tracker.MarkAsCompleted();
                });

            });
        }
        private void MovePanelsOnScreen(float moveSpeed = 0.5f)
        {
            // Stop all animations + reset
            middlePanelTransform.DOKill();
            rightPanelTransform.DOKill();
            leftPanelTransform.DOKill();

            middlePanelTransform.DOMove(middlePanelOnScreenPos.position, moveSpeed).SetEase(Ease.OutBack);
            rightPanelTransform.DOMove(rightPanelOnScreenPos.position, moveSpeed).SetEase(Ease.OutBack);
            leftPanelTransform.DOMove(leftPanelOnScreenPos.position, moveSpeed).SetEase(Ease.OutBack);
        }
        private void MovePanelsOffScreen(float moveSpeed = 0.5f)
        {
            // Stop all animations + reset
            middlePanelTransform.DOKill();
            rightPanelTransform.DOKill();
            leftPanelTransform.DOKill();

            middlePanelTransform.DOMove(middlePanelOffScreenPos.position, moveSpeed).SetEase(Ease.InOutSine);
            rightPanelTransform.DOMove(rightPanelOffScreenPos.position, moveSpeed).SetEase(Ease.InOutSine);
            leftPanelTransform.DOMove(leftPanelOffScreenPos.position, moveSpeed).SetEase(Ease.InOutSine);
        }
        public void SetInteractability(bool onOrOff)
        {
            mainCanvasCg.interactable = onOrOff;
            mainCanvasCg.blocksRaycasts = onOrOff;
        }
        #endregion

        #region Update Health / Stress Sliders
        private void BuildCurrentInitiativeModal(HexCharacterModel character)
        {
            int baseInitiative = StatCalculator.GetTotalInitiative(character, false);
            int fatPenalty = StatCalculator.GetFatiguePenaltyToInitiative(character);
            int delayPenalty = StatCalculator.GetTurnDelayPenaltyToInitiative(character);
            initiativeModalRows.ForEach(x => x.gameObject.SetActive(false));

            initiativeModalRows[0].Build("Base Initiative: " + TextLogic.ReturnColoredText(baseInitiative.ToString(), TextLogic.blueNumber), DotStyle.Green);
            if(fatPenalty != 0)
            {
                initiativeModalRows[1].Build("Fatigue Penalty: " + TextLogic.ReturnColoredText("-" + fatPenalty.ToString(), TextLogic.redText), DotStyle.Red);
            }
            if (delayPenalty != 0)
            {
                initiativeModalRows[2].Build("Delay Turn Penalty: " + TextLogic.ReturnColoredText("-" + delayPenalty.ToString(), TextLogic.redText), DotStyle.Red);
            }

            TransformUtils.RebuildLayouts(initiativeModalLayouts);
        }
        public void UpdateCurrentInitiativeComponents(HexCharacterModel character)
        {
            if (character != null && 
                TurnController.Instance.EntityActivated == character && 
                character.controller == Controller.Player)
            {
                BuildCurrentInitiativeModal(character);
                currentInitiativePanelText.text = StatCalculator.GetTotalInitiative(character).ToString();
            }
                
        }
        public void UpdateHealthComponents(int health, int maxHealth)
        {
            // Convert health int values to floats
            float currentHealthFloat = health;
            float currentMaxHealthFloat = maxHealth;
            float healthBarFloat = currentHealthFloat / currentMaxHealthFloat;

            // Modify UI elements
            healthBarUI.value = healthBarFloat;
            currentHealthText.text = currentHealthFloat.ToString() + "/" + currentMaxHealthFloat.ToString();
        }
        public void UpdateArmourComponents(int armour, int maxArmour)
        {
            // Convert health int values to floats
            float currentArmourFloat = armour;
            float currentMaxArmourFloat = maxArmour;
            float armourBarFloat = currentArmourFloat / currentMaxArmourFloat;

            // Modify UI elements
            armourBarUI.value = armourBarFloat;
            currentArmourText.text = currentArmourFloat.ToString() + "/" + currentMaxArmourFloat.ToString();
        }
        public void UpdateStressComponents(int stress, HexCharacterModel character)
        {
            float currentStressFloat = stress;
            float currentMaxStressFloat = 20f;
            float stressBarFloat = currentStressFloat / currentMaxStressFloat;

            // Modify UI elements
            stressBarUI.value = stressBarFloat;
            stressText.text = currentStressFloat.ToString() + "/" + currentMaxStressFloat.ToString();

            stressPanel.BuildPanelViews(character);
        }
        public void UpdateFatigueComponents(int fatigue, int maxFatigue, float speed = 0.25f)
        {
            float currentFat = fatigue;
            float maxFatFloat = maxFatigue;
            float fatBarFloat = currentFat / maxFatFloat;

            // Modify UI elements
            fatigueSubBarUI.DOKill();
            fatigueBarUI.DOKill();
            fatigueSubBarUI.DOValue(fatBarFloat, speed);
            fatigueBarUI.DOValue(fatBarFloat, speed);
            fatigueText.text = currentFat.ToString() + "/" + maxFatFloat.ToString();
        }
        public void DoFatigueCostDemo(int fatCost, int currentFat, int maxFat)
        {
            float sum = fatCost + currentFat;
            if (sum == 0 || maxFat == 0) return;
            float fatBarFloat = sum / maxFat;
            fatigueSubBarUI.DOKill();
            fatigueSubBarUI.DOValue(fatBarFloat, 0.25f);
            Debug.Log("DoFatigueCostDemo() demo value = " + fatBarFloat.ToString() + ", normal value = " + fatigueBarUI.value.ToString());
        }
        public void ResetFatigueCostPreview()
        {
            fatigueSubBarUI.DOKill();
            fatigueSubBarUI.DOValue(fatigueBarUI.value, 0.25f);
        }
        #endregion

        #region Energy Bar Logic

        #endregion

        #region Ability Bar Logic
        private void BuildHexCharacterAbilityBar(HexCharacterModel character)
        {
            ResetCharacterAbilityBar();

            for (int i = 0; i < character.abilityBook.activeAbilities.Count; i++)            
                abilityButtons[i].BuildButton(character.abilityBook.activeAbilities[i]);
        }             
        private void ResetCharacterAbilityBar()
        {
            foreach (AbilityButton b in abilityButtons)            
                b.ResetButton();            
        }       
        public AbilityButton FindAbilityButton(AbilityData ability)
        {
            if (ability == null || ability.myCharacter == null) return null;

            AbilityButton bRet = null;
            foreach (AbilityButton b in abilityButtons)
            {
                if (b.MyAbilityData == ability)
                {
                    bRet = b;
                    break;
                }
            }

            return bRet;
        }
        #endregion

        #region End + Delay Turn Buttons Logic
        private void BuildTurnButtons(HexCharacterModel character)
        {
            SetEndTurnButtonInteractions(true);
            if (character.hasRequestedTurnDelay)
            {
                SetEndDelayTurnButtonInteractions(false);
                delayTurnButtonImage.sprite = delayTurnButtonNotReadySprite;
            }
            else
            {
                SetEndDelayTurnButtonInteractions(true);
                delayTurnButtonImage.sprite = delayTurnButtonReadySprite;
            }
        }
        public void SetEndTurnButtonInteractions(bool interactable)
        {
            endTurnButton.interactable = interactable;
        }
        public void SetEndDelayTurnButtonInteractions(bool interactable)
        {
            delayTurnButton.interactable = interactable;
        }


        #endregion
    }
}