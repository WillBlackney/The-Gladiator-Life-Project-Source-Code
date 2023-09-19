using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Abilities;
using WeAreGladiators.Perks;
using WeAreGladiators.TurnLogic;
using WeAreGladiators.UCM;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;
using WeAreGladiators.VisualEvents;

namespace WeAreGladiators.Characters
{
    public class CombatUIController : Singleton<CombatUIController>
    {
        [Header("Core Canvas Components")]
        [SerializeField] private Canvas canvasRoot;
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
        [SerializeField]
        private TextMeshProUGUI currentInitiativePanelText;
        [SerializeField] private ModalDottedRow[] initiativeModalRows;
        [SerializeField] private RectTransform[] initiativeModalLayouts;

        [Header("Health Bar UI References")]
        [SerializeField] private Slider healthBarUI;
        [SerializeField] private TextMeshProUGUI currentHealthText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [Header("Armour Bar UI References")]
        [SerializeField] private Slider armourBarUI;
        [SerializeField] private TextMeshProUGUI currentArmourText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [Header("End + Delay Turn Button Components")]
        public Button endTurnButton;
        public Button delayTurnButton;
        public Image delayTurnButtonImage;
        public Sprite delayTurnButtonReadySprite;
        public Sprite delayTurnButtonNotReadySprite;

        #region Getters + Accessors

        public PerkLayoutPanel PerkPanel => perkPanel;
        public EnergyPanelView EnergyBar => energyBar;
        public TextMeshProUGUI CurrentArmourText => currentArmourText;
        public AbilityButton[] AbilityButtons => abilityButtons;
        public bool ViewIsActive => canvasRoot.isActiveAndEnabled;

        #endregion

        #region Build Show, Hide and Animate Main Views

        public void BuildAndShowViewsOnTurnStart(HexCharacterModel character)
        {
            SetInteractability(true);
            canvasRoot.enabled = true;

            middlePanelTransform.localPosition = middlePanelOffScreenPos.localPosition;
            rightPanelTransform.localPosition = rightPanelOffScreenPos.localPosition;
            leftPanelTransform.localPosition = leftPanelOffScreenPos.localPosition;

            // Ability Bar
            BuildHexCharacterAbilityBar(character);

            // Energy Bar
            energyBar.UpdateIcons(character.currentActionPoints);

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
            UpdateStressComponents(character);

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
            DelayUtils.DelayedCall(speed * 0.5f, () =>
            {
                mainCanvasCg.DOFade(0f, speed * 0.5f).OnComplete(() =>
                {
                    canvasRoot.enabled = false;
                    if (tracker != null)
                    {
                        tracker.MarkAsCompleted();
                    }
                });

            });
        }
        private void MovePanelsOnScreen(float moveSpeed = 0.5f)
        {
            // Stop all animations + reset
            middlePanelTransform.DOKill();
            rightPanelTransform.DOKill();
            leftPanelTransform.DOKill();
            middlePanelTransform.position = middlePanelOffScreenPos.position;
            rightPanelTransform.position = rightPanelOffScreenPos.position;
            leftPanelTransform.position = leftPanelOffScreenPos.position;

            // Move on screen
            middlePanelTransform.DOMove(middlePanelOnScreenPos.position, moveSpeed).SetEase(Ease.OutQuad);
            rightPanelTransform.DOMove(rightPanelOnScreenPos.position, moveSpeed).SetEase(Ease.OutQuad);
            leftPanelTransform.DOMove(leftPanelOnScreenPos.position, moveSpeed).SetEase(Ease.OutQuad);
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
            int delayPenalty = StatCalculator.GetTurnDelayPenaltyToInitiative(character);
            initiativeModalRows.ForEach(x => x.gameObject.SetActive(false));

            initiativeModalRows[0].Build("Base Initiative: " + TextLogic.ReturnColoredText(baseInitiative.ToString(), TextLogic.blueNumber), DotStyle.Green);
            if (delayPenalty != 0)
            {
                initiativeModalRows[1].Build("Delay Turn Penalty: " + TextLogic.ReturnColoredText("-" + delayPenalty, TextLogic.redText), DotStyle.Red);
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
            float healthBarFloat = 0f;
            if (maxHealth > 0)
            {
                healthBarFloat = currentHealthFloat / currentMaxHealthFloat;
            }

            // Modify UI elements
            healthBarUI.value = healthBarFloat;
            currentHealthText.text = currentHealthFloat + "/" + currentMaxHealthFloat;
        }
        public void UpdateArmourComponents(int armour, int maxArmour)
        {
            // Convert health int values to floats
            float currentArmourFloat = armour;
            float currentMaxArmourFloat = maxArmour;
            float armourBarFloat = 0f;
            if (maxArmour > 0)
            {
                armourBarFloat = currentArmourFloat / currentMaxArmourFloat;
            }

            // Modify UI elements
            armourBarUI.value = armourBarFloat;
            currentArmourText.text = currentArmourFloat + "/" + currentMaxArmourFloat;
        }
        public void UpdateStressComponents(HexCharacterModel character)
        {
            stressPanel.BuildPanelViews(character);
        }
        public void OnMoraleIconMouseEnter()
        {
            if(TurnController.Instance.EntityActivated.controller == Controller.Player)
            {
                MainModalController.Instance.BuildAndShowModal(TurnController.Instance.EntityActivated.currentMoraleState);
            }
           
        }
        public void OnMoraleIconMouseExit()
        {
            MainModalController.Instance.HideModal();
        }

        #endregion

        #region Energy Bar Logic

        #endregion

        #region Ability Bar Logic

        private void BuildHexCharacterAbilityBar(HexCharacterModel character)
        {
            ResetCharacterAbilityBar();

            for (int i = 0; i < character.abilityBook.activeAbilities.Count; i++)
            {
                abilityButtons[i].BuildButton(character.abilityBook.activeAbilities[i]);
            }
        }
        private void ResetCharacterAbilityBar()
        {
            foreach (AbilityButton b in abilityButtons)
            {
                b.ResetButton();
            }
        }
        public AbilityButton FindAbilityButton(AbilityData ability)
        {
            if (ability == null || ability.myCharacter == null)
            {
                return null;
            }

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
            if (character.hasRequestedTurnDelay || TurnController.Instance.LastToActivate == character)
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
