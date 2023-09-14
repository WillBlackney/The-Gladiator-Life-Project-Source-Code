using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeAreGladiators.Characters;

namespace WeAreGladiators.UI
{
    public class UIBackgroundIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties + Components
        #region

        [SerializeField] private Image backgroundImage;

        #endregion

        // Getters + Accessors
        #region

        public Image BackgroundImage => backgroundImage;
        public BackgroundData BackgroundData { get; private set; }

        #endregion

        // Logic
        #region

        public void BuildFromBackgroundData(BackgroundData p)
        {
            HideAndReset();
            BackgroundData = p;
            gameObject.SetActive(true);
            backgroundImage.sprite = p.BackgroundSprite;
        }

        public void HideAndReset()
        {
            gameObject.SetActive(false);
            BackgroundData = null;
        }

        #endregion

        // Input
        #region

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (BackgroundData != null)
            {
                MainModalController.Instance.BuildAndShowModal(BackgroundData);
            }
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            MainModalController.Instance.HideModal();
        }

        #endregion
    }
}
