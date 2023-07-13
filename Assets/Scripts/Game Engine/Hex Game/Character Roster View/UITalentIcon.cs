using WeAreGladiators.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace WeAreGladiators.UI
{
    public class UITalentIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties + Components
        #region
        [SerializeField] Image talentImage;
        [SerializeField] TextMeshProUGUI talentLevelText;
        [SerializeField] GameObject talentLevelParent;
        private TalentPairing myTalentPairing;
        #endregion

        // Getters + Accessors
        #region
        public Image TalentImage
        {
            get { return talentImage; }
        }
        public TextMeshProUGUI TalentLevelText
        {
            get { return talentLevelText; }
        }        
        public TalentPairing MyTalentPairing
        {
            get { return myTalentPairing; }
        }
       
        #endregion

        // Input
        #region
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (myTalentPairing != null)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(myTalentPairing.Data.keyWords);
                MainModalController.Instance.BuildAndShowModal(myTalentPairing);
            }
                
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            MainModalController.Instance.HideModal();
            KeyWordLayoutController.Instance.FadeOutMainView();
        }
        #endregion

        // Misc
        #region
        public void BuildFromTalentPairing(TalentPairing tp)
        {
            myTalentPairing = tp;
            gameObject.SetActive(true);
            TalentImage.sprite = tp.Data.talentSprite;
            if (talentLevelParent) talentLevelParent.SetActive(true);
            if (talentLevelText) talentLevelText.text = tp.level.ToString();
        }      
        public void HideAndReset()
        {
            gameObject.SetActive(false);
            if (talentLevelParent) talentLevelParent.SetActive(false);
            myTalentPairing = null;
        }
        #endregion
    }
}