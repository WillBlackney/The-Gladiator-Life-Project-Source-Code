using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeAreGladiators.Characters;

namespace WeAreGladiators.UI
{
    public class UITalentIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties + Components
        #region

        [SerializeField] private Image talentImage;
        [SerializeField] private TextMeshProUGUI talentLevelText;
        [SerializeField] private GameObject talentLevelParent;

        #endregion

        // Getters + Accessors
        #region

        public Image TalentImage => talentImage;
        public TextMeshProUGUI TalentLevelText => talentLevelText;
        public TalentPairing MyTalentPairing { get; private set; }

        #endregion

        // Input
        #region

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (MyTalentPairing != null)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(MyTalentPairing.Data.keyWords);
                MainModalController.Instance.BuildAndShowModal(MyTalentPairing);
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
            MyTalentPairing = tp;
            gameObject.SetActive(true);
            TalentImage.sprite = tp.Data.talentSprite;
            if (talentLevelParent)
            {
                talentLevelParent.SetActive(true);
            }
            if (talentLevelText)
            {
                talentLevelText.text = tp.level.ToString();
            }
        }
        public void HideAndReset()
        {
            gameObject.SetActive(false);
            if (talentLevelParent)
            {
                talentLevelParent.SetActive(false);
            }
            MyTalentPairing = null;
        }

        #endregion
    }
}
