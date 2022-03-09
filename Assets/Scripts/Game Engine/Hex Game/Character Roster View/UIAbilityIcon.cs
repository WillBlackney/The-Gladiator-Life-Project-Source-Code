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
        [SerializeField] GameObject abilityImageParent;
        [SerializeField] PopupPositon popupPosition;
        private AbilityData myDataRef;
        #endregion

        // Getters + Accessors
        #region
        public Image AbilityImage
        {
            get { return abilityImage; }
        }
        public GameObject AbilityImageParent
        {
            get { return abilityImageParent; }
        }
        public AbilityData MyDataRef
        {
            get { return myDataRef; }
        }
        public PopupPositon PopupPosition
        {
            get { return popupPosition; }
        }


        #endregion

        // Input
        #region
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(MyDataRef != null)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(MyDataRef.keyWords);
                AbilityPopupController.Instance.OnRosterAbilityButtonMousedOver(this);
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
            AbilityImageParent.SetActive(true);
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