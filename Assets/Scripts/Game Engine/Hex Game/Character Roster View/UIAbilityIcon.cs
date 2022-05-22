using HexGameEngine.Abilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HexGameEngine.UI
{
    public class UIAbilityIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties + Components
        #region
        [SerializeField] Image abilityImage;
        private AbilityData myDataRef;
        [SerializeField] bool placeModalAbove = true;
        #endregion

        // Getters + Accessors
        #region
        public Image AbilityImage
        {
            get { return abilityImage; }
        }       
        public AbilityData MyDataRef
        {
            get { return myDataRef; }
        }


        #endregion

        // Input
        #region
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(MyDataRef != null)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(MyDataRef.keyWords);
                AbilityPopupController.Instance.OnRosterAbilityButtonMousedOver(this, placeModalAbove);
            }                       
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (MyDataRef != null)
            {
                KeyWordLayoutController.Instance.FadeOutMainView();
                AbilityPopupController.Instance.OnAbilityButtonMousedExit();
            }            
        }
        #endregion

        // Misc
        #region
        public void BuildFromAbilityData(AbilityData d)
        {
            AbilityImage.sprite = d.AbilitySprite;
            gameObject.SetActive(true);
            SetMyDataReference(d);
        }
        public void SetMyDataReference(AbilityData data)
        {
            myDataRef = data;
        }
        public void HideAndReset()
        {
            gameObject.SetActive(false);
            myDataRef = null;
        }
        #endregion

    }
}