using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeAreGladiators.Abilities;

namespace WeAreGladiators.UI
{
    public class UIAbilityIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties + Components
        #region

        [SerializeField] private Image abilityImage;
        [SerializeField] private bool placeModalAbove = true;

        #endregion

        // Getters + Accessors
        #region

        public Image AbilityImage => abilityImage;
        public AbilityData MyDataRef { get; private set; }

        #endregion

        // Input
        #region

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (MyDataRef != null)
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
            MyDataRef = data;
        }
        public void HideAndReset()
        {
            gameObject.SetActive(false);
            MyDataRef = null;
        }

        #endregion
    }
}
