using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeAreGladiators.Perks;

namespace WeAreGladiators.UI
{

    public class UIPerkIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // Properties + Components
        #region

        [SerializeField] private Image perkImage;
        [SerializeField] private PerkIconData perkDataRef;

        #endregion

        // Getters + Accessors
        #region

        public Image PerkImage => perkImage;
        public PerkIconData PerkDataRef => perkDataRef;
        public ActivePerk ActivePerk { get; private set; }

        #endregion

        // Logic
        #region

        public void SetMyDataReference(PerkIconData data)
        {
            perkDataRef = data;
        }
        public void BuildFromActivePerk(ActivePerk p)
        {
            ActivePerk = p;
            SetMyDataReference(p.Data);
            gameObject.SetActive(true);
            PerkImage.sprite = p.Data.passiveSprite;
        }

        public void HideAndReset()
        {
            gameObject.SetActive(false);
            perkDataRef = null;
            ActivePerk = null;
        }

        #endregion

        // Input
        #region

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (ActivePerk != null)
            {
                MainModalController.Instance.BuildAndShowModal(ActivePerk);
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(ActivePerk.Data.keywords.ToList());
            }

        }
        public void OnPointerExit(PointerEventData eventData)
        {
            MainModalController.Instance.HideModal();
            KeyWordLayoutController.Instance.FadeOutMainView();
        }

        #endregion
    }
}
