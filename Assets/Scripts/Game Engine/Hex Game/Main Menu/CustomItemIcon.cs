using HexGameEngine.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HexGameEngine.UI
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
        private void OnMouseOver()
        {
            //if (itemDataRef == null) return;
            //ItemPopupController.Instance.OnRosterItemSlotMousedOver(this);
        }
        public void OnMouseExit()
        {
            //ItemPopupController.Instance.OnInventoryItemMouseExit();
        }
        #endregion
    }
}