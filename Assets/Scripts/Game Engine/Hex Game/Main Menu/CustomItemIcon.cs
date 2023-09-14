using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Items;

namespace WeAreGladiators.UI
{
    public class CustomItemIcon : MonoBehaviour
    {

        // Misc
        #region

        public void BuildFromData(ItemData data)
        {
            itemDataRef = data;
            itemImage.gameObject.SetActive(true);
            if (data == null)
            {
                itemImage.gameObject.SetActive(false);
                return;
            }
            itemImage.sprite = data.ItemSprite;
        }

        #endregion
        // Properties + Components
        #region

        [SerializeField] private Image itemImage;
        [SerializeField] private ItemData itemDataRef;

        #endregion

        // Getters + Accessors
        #region

        public Image ItemImage => itemImage;
        public ItemData ItemDataRef => itemDataRef;

        #endregion

        // Input
        #region

        public void MouseEnter()
        {
            if (itemDataRef == null)
            {
                return;
            }
            ItemPopupController.Instance.OnCustomCharacterItemSlotMousedOver(this);
        }
        public void MouseExit()
        {
            ItemPopupController.Instance.OnInventoryItemMouseExit();
        }

        #endregion
    }
}
