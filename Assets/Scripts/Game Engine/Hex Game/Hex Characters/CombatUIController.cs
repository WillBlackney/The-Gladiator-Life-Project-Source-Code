using CardGameEngine.UCM;
using DG.Tweening;
using HexGameEngine.Abilities;
using HexGameEngine.Perks;
using HexGameEngine.UCM;
using HexGameEngine.Utilities;
using HexGameEngine.VisualEvents;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private TextMeshProUGUI currentArmourText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Health Bar UI References")]
        [SerializeField] private Slider healthBarUI;
        [SerializeField] private TextMeshProUGUI currentHealthText;
        [SerializeField] private TextMeshProUGUI maxHealthText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Stress Bar UI References")]
        [SerializeField] private Slider stressBarUI;
        [SerializeField] private TextMeshProUGUI stressText;
        [SerializeField] private TextMeshProUGUI maxStressText;
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

            float speed = 0.75f;

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

            // Health 
            UpdateHealthComponents(character.currentHealth, StatCalculator.GetTotalMaxHealth(character));

            // Stress Components
            UpdateStressComponents(character.currentStress, character);

            // Armour
            currentArmourText.text = character.currentArmour.ToString();

            // End + Delay Turn Buttons
            BuildTurnButtons(character);

            // Fade in views
            CharacterModeller.FadeOutCharacterModel(uiPotraitUCM, 0);
            CharacterModeller.FadeInCharacterModel(uiPotraitUCM, speed);
            mainCanvasCg.DOKill();
            mainCanvasCg.alpha = 0.001f;
            mainCanvasCg.DOFade(1f, speed);

            // Animate on screen
            MovePanelsOnScreen(speed);
        }
        public void HideViewsOnTurnEnd(CoroutineData cData = null, float speed = 1.5f)
        {
            SetInteractability(false);
            mainCanvasCg.DOKill();
           // mainCanvasCg.interactable = false;
           // mainCanvasCg.blocksRaycasts = false;

            middlePanelTransform.localPosition = middlePanelOnScreenPos.localPosition;
            rightPanelTransform.localPosition = rightPanelOnScreenPos.localPosition;
            leftPanelTransform.localPosition = leftPanelOnScreenPos.localPosition;

            MovePanelsOffScreen(speed);
            
            // Fade out views
            CharacterModeller.FadeOutCharacterModel(uiPotraitUCM, speed);            
            Sequence s = DOTween.Sequence();
            s.Append(mainCanvasCg.DOFade(0f, speed));
            s.OnComplete(() =>
            {
                mainVisualParent.SetActive(false);
                if (cData != null) cData.MarkAsCompleted();
            });
        }
        private void MovePanelsOnScreen(float moveSpeed = 0.5f)
        {
            // Stop all animations + reset
            middlePanelTransform.DOKill();
            rightPanelTransform.DOKill();
            leftPanelTransform.DOKill();

            middlePanelTransform.DOMove(middlePanelOnScreenPos.position, moveSpeed);
            rightPanelTransform.DOMove(rightPanelOnScreenPos.position, moveSpeed);
            leftPanelTransform.DOMove(leftPanelOnScreenPos.position, moveSpeed);
        }
        private void MovePanelsOffScreen(float moveSpeed = 0.5f)
        {
            // Stop all animations + reset
            middlePanelTransform.DOKill();
            rightPanelTransform.DOKill();
            leftPanelTransform.DOKill();

            middlePanelTransform.DOMove(middlePanelOffScreenPos.position, moveSpeed);
            rightPanelTransform.DOMove(rightPanelOffScreenPos.position, moveSpeed);
            leftPanelTransform.DOMove(leftPanelOffScreenPos.position, moveSpeed);
        }
        public void SetInteractability(bool onOrOff)
        {
            mainCanvasCg.interactable = onOrOff;
            mainCanvasCg.blocksRaycasts = onOrOff;
        }
        #endregion

        #region Update Health / Stress Sliders
        public void UpdateHealthComponents(int health, int maxHealth)
        {
            // Convert health int values to floats
            float currentHealthFloat = health;
            float currentMaxHealthFloat = maxHealth;
            float healthBarFloat = currentHealthFloat / currentMaxHealthFloat;

            // Modify UI elements
            healthBarUI.value = healthBarFloat;
            currentHealthText.text = health.ToString();
            maxHealthText.text = maxHealth.ToString();
        }
        public void UpdateStressComponents(int stress, HexCharacterModel character)
        {
            float currentStressFloat = stress;
            float currentMaxHealthFloat = 100f;
            float stressBarFloat = currentStressFloat / currentMaxHealthFloat;

            // Modify UI elements
            stressBarUI.value = stressBarFloat;
            stressText.text = stress.ToString();
            maxStressText.text = "100";

            stressPanel.BuildPanelViews(character);
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