using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using HexGameEngine.UI;
using HexGameEngine.Characters;

namespace HexGameEngine.Abilities
{
    public class AbilityButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties
        #region
        private AbilityData myAbilityData;
        [SerializeField] Image abilityImage;
        [SerializeField] GameObject abilityImageParent;
        [SerializeField] GameObject cooldownOverlay;
        [SerializeField] TextMeshProUGUI cooldownText;

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
                    CombatUIController.Instance.EnergyBar.UpdateIcons(MyAbilityData.myCharacter.currentEnergy, 0.25f);
                    KeyWordLayoutController.Instance.FadeOutMainView();
                }                   
             
                CurrentButtonMousedOver = null;
            }
               

        }
        #endregion

        public void ResetButton()
        {
            myAbilityData = null;
            cooldownOverlay.SetActive(false);
            abilityImageParent.SetActive(false);
            cooldownText.gameObject.SetActive(false);
        }
        public void BuildButton(AbilityData data)
        {
            // link data to button
            myAbilityData = data;

            // set sprite
            abilityImage.sprite = data.AbilitySprite;
            abilityImageParent.SetActive(true);

            // set cooldown text + views if needed
            UpdateAbilityButtonCooldownView();
        }
        public void UpdateAbilityButtonCooldownView()
        {
            if (myAbilityData == null) return;
            CooldownText.text = myAbilityData.currentCooldown.ToString();
            if (myAbilityData.currentCooldown == 0)
            {
                CooldownOverlay.SetActive(false);
                CooldownText.gameObject.SetActive(false);
            }
            else
            {
                CooldownOverlay.SetActive(true);
                CooldownText.gameObject.SetActive(true);
            }
        }
    }
}