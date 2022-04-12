using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using HexGameEngine.UI;

namespace HexGameEngine.Abilities
{
    public class AbilityButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties
        #region
        [HideInInspector] public AbilityData myAbilityData;
        [SerializeField] Image abilityImage;
        [SerializeField] GameObject cooldownOverlay;
        [SerializeField] TextMeshProUGUI cooldownText;
        #endregion

        // Getters + Accessors
        #region
        public Image AbilityImage
        {
            get { return abilityImage; }
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
            KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(myAbilityData.keyWords);             
            AbilityPopupController.Instance.OnAbilityButtonMousedOver(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (CurrentButtonMousedOver == this || CurrentButtonMousedOver == null)
            {
                KeyWordLayoutController.Instance.FadeOutMainView();
                AbilityPopupController.Instance.OnAbilityButtonMousedExit();
                CurrentButtonMousedOver = null;
            }
               

        }
        #endregion
    }
}