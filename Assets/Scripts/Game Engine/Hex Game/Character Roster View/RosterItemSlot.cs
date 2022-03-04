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
        public RosterSlotType SlotType
        {
            get { return slotType; }
        }
        #endregion

        // Misc
        #region
        public void SetMyDataReference(ItemData data)
        {
            itemDataRef = data;
        }
        #endregion

        // Input
        #region
        private void OnMouseOver()
        {
            if (CharacterRosterViewController.Instance.rosterSlotMousedOver != this)
            {
                CharacterRosterViewController.Instance.rosterSlotMousedOver = this;

                if (itemDataRef == null)
                    return;

                ItemPopupController.Instance.OnRosterItemSlotMousedOver(this);
            }
           
        }
        public void OnMouseExit()
        {
            if (CharacterRosterViewController.Instance.rosterSlotMousedOver == this)
                CharacterRosterViewController.Instance.rosterSlotMousedOver = null;

            ItemPopupController.Instance.OnInventoryItemMouseExit();

        }
        #endregion
    }
}