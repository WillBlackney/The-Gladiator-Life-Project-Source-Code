using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Characters;
using WeAreGladiators.Items;

namespace WeAreGladiators.UI
{
    public class RosterItemSlot : MonoBehaviour
    {

        // Misc
        #region

        public void SetMyDataReference(ItemData data)
        {
            slotTypeImage.gameObject.SetActive(false);
            if (data == null)
            {
                slotTypeImage.gameObject.SetActive(true);
            }
            itemDataRef = data;
        }

        #endregion
        // Properties + Components
        #region

        [SerializeField] private Image itemImage;
        [SerializeField] private ItemData itemDataRef;
        [SerializeField] private RosterSlotType slotType;
        [SerializeField] private Image slotTypeImage;

        #endregion

        // Getters + Accessors
        #region

        public static RosterItemSlot SlotMousedOver { get; private set; }
        public Image ItemImage => itemImage;
        public ItemData ItemDataRef => itemDataRef;
        public RosterSlotType SlotType => slotType;

        #endregion

        // Input
        #region

        private void HandleRightClick()
        {
            Debug.Log("RosterItemSlot.HandleRightClick()");
            if (itemDataRef != null && InventoryController.Instance.HasFreeInventorySpace())
            {
                HexCharacterData character = CharacterRosterViewController.Instance.CharacterCurrentlyViewing;
                ItemPopupController.Instance.OnInventoryItemMouseExit();
                ItemController.Instance.HandleSendItemFromCharacterToInventory(character, this);
            }

        }
        private void OnMouseOver()
        {
            if (SlotMousedOver != this)
            {
                SlotMousedOver = this;
                if (itemDataRef == null)
                {
                    return;
                }
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
            if (SlotMousedOver == this)
            {
                SlotMousedOver = null;
            }

            ItemPopupController.Instance.OnInventoryItemMouseExit();
            CursorController.Instance.SetCursor(CursorType.NormalPointer);

        }

        #endregion
    }
}
