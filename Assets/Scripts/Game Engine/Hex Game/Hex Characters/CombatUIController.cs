using CardGameEngine.UCM;
using DG.Tweening;
using HexGameEngine.Abilities;
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
        [SerializeField] private Canvas uiCanvas;
        [SerializeField] private GameObject uiCanvasParent;
        [SerializeField] private CanvasGroup uiCanvasCg;     
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Middle Section Components")]
        [SerializeField] private AbilityButton[] abilityButtons;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Left Section Components")]
        [SerializeField] private UniversalCharacterModel uiPotraitUCM;
        [SerializeField] private TextMeshProUGUI characterNameTextUI;
        [SerializeField] private StressPanelView stressPanel;
        [SerializeField] private TextMeshProUGUI currentArmourText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Health Bar UI References")]
        [SerializeField] private Slider healthBarUI;
        [SerializeField] private TextMeshProUGUI healthTextUI;
        [SerializeField] private TextMeshProUGUI maxHealthTextUI;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Stress Bar UI References")]
        [SerializeField] private Slider stressBarUI;
        [SerializeField] private TextMeshProUGUI stressTextUI;
        [SerializeField] private TextMeshProUGUI maxStressTextUI;

        #region Getters + Accessors
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
                return uiCanvasParent.activeSelf; 
            } 
        }
        #endregion

        #region Build Show, Hide and Move Main View
        public void BuildAndShowView(HexCharacterModel character)
        {
            // Ability Bar
            BuildHexCharacterAbilityBar(character);

            // Energy Bar

            // Portrait
            characterNameTextUI.text = character.myName;
            CharacterModeller.BuildModelFromStringReferences(uiPotraitUCM, character.characterData.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, uiPotraitUCM);

            // Health 
            UpdateHealthComponents(character.currentHealth, StatCalculator.GetTotalMaxHealth(character));

            // Stress Components
            UpdateStressComponents(character.currentStress, character);

            // Armour
            currentArmourText.text = character.currentArmour.ToString();
        }
        public void FadeInCharacterUICanvas(HexCharacterView view, CoroutineData cData)
        {
            // TO DO:
            // build all the views to current character state
            // animate the canvas on screen from off screen

            if (view == null)
            {
                if (cData != null) cData.MarkAsCompleted();
                return;
            }
            uiCanvasParent.SetActive(true);
            uiCanvasCg.alpha = 0;
            Sequence s = DOTween.Sequence();
            s.Append(uiCanvasCg.DOFade(1, 0.75f));
            s.OnComplete(() =>
            {
                // Resolve
                if (cData != null)
                    cData.MarkAsCompleted();
            });
        }
        public void FadeOutCharacterUICanvas(CoroutineData cData)
        {
            // TO DO:
            // animate the canvas on screen from off screen and fade out

            uiCanvasParent.SetActive(true);
            Sequence s = DOTween.Sequence();
            s.Append(uiCanvasCg.DOFade(0, 0.75f));
            s.OnComplete(() =>
            {
                uiCanvasParent.SetActive(false);

                // Resolve
                if (cData != null)
                {
                    cData.MarkAsCompleted();
                }
            });
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
            healthTextUI.text = health.ToString();
            maxHealthTextUI.text = maxHealth.ToString();
        }
        public void UpdateStressComponents(int stress, HexCharacterModel character)
        {
            float currentStressFloat = stress;
            float currentMaxHealthFloat = 100f;
            float stressBarFloat = currentStressFloat / currentMaxHealthFloat;

            // Modify UI elements
            stressBarUI.value = stressBarFloat;
            stressTextUI.text = stress.ToString();
            maxStressTextUI.text = "100";

            stressPanel.BuildPanelViews(character);
        }
        #endregion

        #region Energy Bar Logic

        #endregion

        #region Ability Bar Logic
        private void BuildHexCharacterAbilityBar(HexCharacterModel character)
        {
            Debug.Log("AbilityController.BuildHexCharacterAbilityBar() called...");

            ResetCharacterAbilityBar(character);

            for (int i = 0; i < character.abilityBook.activeAbilities.Count; i++)
            {
                BuildAbilityButton(abilityButtons[i], character.abilityBook.activeAbilities[i]);
            }

        }
        private void BuildAbilityButton(AbilityButton button, AbilityData data)
        {
            // enable GO
            button.gameObject.SetActive(true);

            // set sprite
            button.AbilityImage.sprite = data.AbilitySprite;

            // link data to button
            button.myAbilityData = data;

            // set cooldown text + views if needed
            UpdateAbilityButtonCooldownView(button);
        }
        private void ResetAbilityButton(AbilityButton button)
        {
            button.gameObject.SetActive(false);
            button.myAbilityData = null;
        }
        private void ResetCharacterAbilityBar(HexCharacterModel character)
        {
            foreach (AbilityButton b in abilityButtons)
            {
                ResetAbilityButton(b);
            }
        }
        public void UpdateAbilityButtonCooldownView(AbilityButton b)
        {
            if (b == null) return;

            b.CooldownText.text = b.myAbilityData.currentCooldown.ToString();
            if (b.myAbilityData.currentCooldown == 0)
            {
                b.CooldownOverlay.SetActive(false);
                b.CooldownText.gameObject.SetActive(false);
            }
            else
            {
                b.CooldownOverlay.SetActive(true);
                b.CooldownText.gameObject.SetActive(true);
            }
        }
        public AbilityButton FindAbilityButton(AbilityData ability)
        {
            if (ability.myCharacter == null) return null;

            AbilityButton bRet = null;
            foreach (AbilityButton b in abilityButtons)
            {
                if (b.myAbilityData == ability)
                {
                    bRet = b;
                    break;
                }
            }

            return bRet;
        }
        #endregion
    }
}