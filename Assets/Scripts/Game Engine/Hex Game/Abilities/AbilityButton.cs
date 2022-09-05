using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using HexGameEngine.UI;
using HexGameEngine.Characters;
using DG.Tweening;

namespace HexGameEngine.Abilities
{
    public class AbilityButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties
        #region
        private AbilityData myAbilityData;
        [SerializeField] Image[] abilityImages;
        [SerializeField] GameObject abilityImageParent;
        [SerializeField] GameObject[] unusableOverlays;
        [SerializeField] GameObject cooldownOverlay;
        [SerializeField] TextMeshProUGUI cooldownText;
        [SerializeField] CanvasGroup selectedGlow;

        #endregion

        // Getters + Accessors
        #region
        public AbilityData MyAbilityData
        {
            get { return myAbilityData; }
        }
        public GameObject CooldownOverlay
        {
            get { return cooldownOverlay; }
        }
        public GameObject[] UnusableOverlays
        {
            get { return unusableOverlays; }
        }
        public TextMeshProUGUI CooldownText
        {
            get { return cooldownText; }
        }
        #endregion

        // Input Listeners
        #region

        public static AbilityButton CurrentButtonMousedOver
        {
            get; private set;
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            AbilityController.Instance.OnAbilityButtonClicked(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CurrentButtonMousedOver = this;
            if (MyAbilityData != null && MyAbilityData.myCharacter != null)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(MyAbilityData.keyWords);
                AbilityPopupController.Instance.OnAbilityButtonMousedOver(this);
                CombatUIController.Instance.EnergyBar.OnAbilityButtonMouseEnter
                              (MyAbilityData.myCharacter.currentEnergy, AbilityController.Instance.GetAbilityEnergyCost(MyAbilityData.myCharacter, MyAbilityData));

            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (CurrentButtonMousedOver == this || CurrentButtonMousedOver == null)
            {
                if(MyAbilityData != null && MyAbilityData.myCharacter != null)
                {
                    AbilityPopupController.Instance.OnAbilityButtonMousedExit();
                    KeyWordLayoutController.Instance.FadeOutMainView();
                    if (AbilityController.Instance.CurrentAbilityAwaiting != null)
                    {
                        AbilityData a = AbilityController.Instance.CurrentAbilityAwaiting;
                        CombatUIController.Instance.EnergyBar.UpdateIcons(a.myCharacter.currentEnergy);
                        CombatUIController.Instance.EnergyBar.OnAbilityButtonMouseEnter
                          (a.myCharacter.currentEnergy, AbilityController.Instance.GetAbilityEnergyCost(a.myCharacter, a));

                    }
                    else
                    {
                        CombatUIController.Instance.EnergyBar.UpdateIcons(MyAbilityData.myCharacter.currentEnergy, 0.25f);
                    }
                    
                    
                }                   
             
                CurrentButtonMousedOver = null;
            }
               

        }
        #endregion

        public void ResetButton()
        {
            myAbilityData = null;
            for(int i = 0; i < unusableOverlays.Length; i++)
                unusableOverlays[i].SetActive(false);
            abilityImageParent.SetActive(false);
            cooldownText.gameObject.SetActive(false);
        }
        public void BuildButton(AbilityData data)
        {
            // link data to button
            myAbilityData = data;

            // set sprite
            for(int i = 0; i < abilityImages.Length; i++)
                abilityImages[i].sprite = data.AbilitySprite;
            abilityImageParent.SetActive(true);

            // set cooldown text + views if needed
            UpdateAbilityButtonUnusableOverlay();
        }
        public void UpdateAbilityButtonUnusableOverlay()
        {
            if (myAbilityData == null) return;
            CooldownText.text = myAbilityData.currentCooldown.ToString();
            if(myAbilityData.myCharacter != null &&
                myAbilityData.currentCooldown == 0 && 
                myAbilityData.myCharacter.currentEnergy >= AbilityController.Instance.GetAbilityEnergyCost(myAbilityData.myCharacter, myAbilityData) &&
                AbilityController.Instance.IsAbilityUseable(myAbilityData.myCharacter, myAbilityData))
            {
                for (int i = 0; i < unusableOverlays.Length; i++)
                    unusableOverlays[i].SetActive(false);
                CooldownText.gameObject.SetActive(false);
                cooldownOverlay.gameObject.SetActive(false);
            }
            else if (myAbilityData.currentCooldown > 0)
            {
                for (int i = 0; i < unusableOverlays.Length; i++)
                    unusableOverlays[i].SetActive(true);
                CooldownText.gameObject.SetActive(true);
                cooldownOverlay.gameObject.SetActive(true);
            }
            else
            {
                for (int i = 0; i < unusableOverlays.Length; i++)
                    unusableOverlays[i].SetActive(true);
                CooldownText.gameObject.SetActive(false);
                cooldownOverlay.gameObject.SetActive(false);
            }
        }
        public void SetSelectedGlow(bool onOrOff)
        {
            selectedGlow.DOKill();
            selectedGlow.alpha = 0;
            if (onOrOff == true)            
                selectedGlow.DOFade(1f, 0.25f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            
        }
    }
}