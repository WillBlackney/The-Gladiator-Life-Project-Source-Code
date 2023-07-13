using WeAreGladiators.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WeAreGladiators.UI
{
    public class CustomItemIcon : MonoBehaviour
    {
        // Properties + Components
        #region
        [SerializeField] private Image itemImage;
        [SerializeField] private ItemData itemDataRef;

        #endregion

        // Getters + Accessors
        #region       
        public Image ItemImage
        {
            get { return itemImage; }
        }
        public ItemData ItemDataRef
        {
            get { return itemDataRef; }
        }
        #endregion

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

        // Input
        #region
        public void MouseEnter()
        {
            if (itemDataRef == null) return;
            ItemPopupController.Instance.OnCustomCharacterItemSlotMousedOver(this);
        }
        public void MouseExit()
        {
            ItemPopupController.Instance.OnInventoryItemMouseExit();
        }
        #endregion
    }
}