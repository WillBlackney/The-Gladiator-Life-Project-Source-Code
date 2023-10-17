using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Characters;
using WeAreGladiators.Items;

namespace WeAreGladiators.UI
{
    public class RosterItemSlot : MonoBehaviour
    {
        #region Properties + Components

        [SerializeField] private Image itemImage;
        [SerializeField] private GameObject itemViewParent;
        [SerializeField] private ItemData itemDataRef;
        [SerializeField] private RosterSlotType slotType;

        #endregion

        #region Getters + Accessors
        public static RosterItemSlot SlotMousedOver { get; private set; }
        public Image ItemImage => itemImage;
        public GameObject ItemViewParent => itemViewParent;
        public ItemData ItemDataRef => itemDataRef;
        public RosterSlotType SlotType => slotType;
        #endregion

        #region Misc
        public void SetMyDataReference(ItemData data)
        {
            itemViewParent.SetActive(true);
            if (data == null)
            {
                itemViewParent.SetActive(false);
            }
            itemDataRef = data;
        }
        #endregion

        #region Input
        private void HandleRightClick()
        {
            Debug.Log("RosterItemSlot.HandleRightClick()");

            if (GameController.Instance.GameState != GameState.Town ||
                CharacterRosterViewController.Instance.PerkTalentLevelUpPageViewIsActive ||
                CharacterRosterViewController.Instance.AttributeLevelUpPageComponent.ViewIsActive ||
                !CharacterRosterViewController.Instance.MainVisualParent.activeSelf)
            {
                return;
            }

            if (itemDataRef != null && InventoryController.Instance.HasFreeInventorySpace())
            {
                HexCharacterData character = CharacterRosterViewController.Instance.CharacterCurrentlyViewing;
                ItemPopupController.Instance.OnInventoryItemMouseExit();
                ItemController.Instance.HandleSendItemFromCharacterToInventory(character, this);
            }

        }
        private void OnMouseOver()
        {
            if(CharacterRosterViewController.Instance.PerkTalentLevelUpPageViewIsActive ||
                    CharacterRosterViewController.Instance.AttributeLevelUpPageComponent.ViewIsActive)
            {
                return;
            }

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
            if (CharacterRosterViewController.Instance.PerkTalentLevelUpPageViewIsActive ||
                   CharacterRosterViewController.Instance.AttributeLevelUpPageComponent.ViewIsActive)
            {
                return;
            }

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
