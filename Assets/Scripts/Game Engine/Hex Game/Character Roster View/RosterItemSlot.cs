using HexGameEngine.Perks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HexGameEngine.Items;

namespace HexGameEngine.UI
{
    public class RosterItemSlot : MonoBehaviour
    {
        // Properties + Components
        #region
        [SerializeField] private Image itemImage;
        [SerializeField] private ItemData itemDataRef;
        [SerializeField] private RosterSlotType slotType;
        [SerializeField] private Image slotTypeImage;

        private static RosterItemSlot slotMousedOver;
        #endregion

        // Getters + Accessors
        #region
        public static RosterItemSlot SlotMousedOver
        {
            get { return slotMousedOver; }
        }
        public Image ItemImage
        {
            get { return itemImage; }
        }
        public ItemData ItemDataRef
        {
            get { return itemDataRef; }
        }
        public RosterSlotType SlotType
        {
            get { return slotType; }
        }
        #endregion

        // Misc
        #region       
        public void SetMyDataReference(ItemData data)
        {
            slotTypeImage.gameObject.SetActive(false);
            if (data == null) slotTypeImage.gameObject.SetActive(true);
            itemDataRef = data;
        }
        #endregion

        // Input
        #region
        private void HandleRightClick()
        {
            Debug.Log("RosterItemSlot.HandleRightClick()");
            if(itemDataRef != null && InventoryController.Instance.HasFreeInventorySpace())
            {
                var character = CharacterRosterViewController.Instance.CharacterCurrentlyViewing;
                ItemPopupController.Instance.OnInventoryItemMouseExit();
                ItemController.Instance.HandleSendItemFromCharacterToInventory(character, this);
            }
          
        }
        private void OnMouseOver()
        {            
            if (slotMousedOver != this)
            {
                slotMousedOver = this;
                if (itemDataRef == null) return;
                ItemPopupController.Instance.OnRosterItemSlotMousedOver(this);
                
            }
            // Right click
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                HandleRightClick();                
            }
            CursorController.Instance.SetCursor(CursorType.HandPointer);

        }
        public void OnMouseExit()
        {
            if (slotMousedOver == this)
                slotMousedOver = null;

            ItemPopupController.Instance.OnInventoryItemMouseExit();
            CursorController.Instance.SetCursor(CursorType.NormalPointer);

        }
        #endregion
    }
}