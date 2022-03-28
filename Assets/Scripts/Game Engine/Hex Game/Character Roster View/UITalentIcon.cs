using HexGameEngine.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace HexGameEngine.UI
{
    public class UITalentIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties + Components
        #region
        [SerializeField] Image talentImage;
        [SerializeField] TextMeshProUGUI talentLevelText;
        [SerializeField] GameObject talentLevelParent;
        [SerializeField] private PopupPositon popupPositon;
        private TalentDataSO myTalentData;
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
        public TalentDataSO MyTalentData
        {
            get { return myTalentData; }
        }
        public PopupPositon PopupPositon
        {
            get { return popupPositon; }
        }
        #endregion

        // Input
        #region
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (myTalentData != null)
                UIController.Instance.OnTalentButtonMouseOver(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UIController.Instance.OnTalentButtonMouseExit();
        }
        #endregion

        // Misc
        #region
        public void BuildFromTalentPairing(TalentPairing tp)
        {
            TalentDataSO talentData = CharacterDataController.Instance.GetTalentDataFromTalentEnum(tp.talentSchool);
            myTalentData = talentData;
            gameObject.SetActive(true);
            TalentImage.sprite = talentData.talentSprite;
            talentLevelParent.SetActive(true);
            talentLevelText.text = tp.level.ToString();
        }      
        public void HideAndReset()
        {
            gameObject.SetActive(false);
            talentLevelParent.SetActive(false);
            myTalentData = null;
        }
        #endregion
    }
}