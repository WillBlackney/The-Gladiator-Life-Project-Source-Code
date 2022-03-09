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
        [SerializeField] GameObject talentImageParent;
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
        public GameObject TalentImageParent
        {
            get { return talentImageParent; }
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
        public void BuildFromTalentData(TalentPairing tp)
        {
            TalentDataSO talentData = CharacterDataController.Instance.GetTalentDataFromTalentEnum(tp.talentSchool);
            SetMyTalent(talentData);
            gameObject.SetActive(true);
            TalentImageParent.SetActive(true);
            TalentImage.sprite = talentData.talentSprite;

            // to do in future: we may want to set the talent level as a text on the icon
        }
        public void SetMyTalent(TalentDataSO talent)
        {
            myTalentData = talent;
        }
        public void HideAndReset()
        {
            gameObject.SetActive(false);
            myTalentData = null;
        }
        #endregion
    }
}