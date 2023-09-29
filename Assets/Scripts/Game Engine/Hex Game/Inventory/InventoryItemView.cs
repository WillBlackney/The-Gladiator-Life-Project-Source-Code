using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.UI;

namespace WeAreGladiators.Items
{
    public class InventoryItemView : MonoBehaviour
    {
        // Properties + Components
        #region

        [Header("Ability Book Components")]
        [SerializeField] private GameObject bookVisualParent;
        [SerializeField] private Image bookImage;

        [Header("Item Components")]
        [SerializeField] private GameObject weaponVisualParent;
        [SerializeField] private Image weaponImage;
        [SerializeField] private Image rarityOutline;

        [Header("Cost Components")]
        [SerializeField] private GameObject goldCostParent;
        [SerializeField] private TextMeshProUGUI goldCostText;

        public static InventoryItemView itemDragged { get; private set; }

        // Drag values
        [HideInInspector] public bool currentlyBeingDragged;
        private Canvas dragCanvas;
        private RectTransform dragTransform;

        #endregion

        // Getters + Accessors
        #region

        public GameObject GoldCostParent => goldCostParent;
        public TextMeshProUGUI GoldCostText => goldCostText;
        public GameObject BookVisualParent => bookVisualParent;
        public Image BookImage => bookImage;
        public GameObject WeaponVisualParent => weaponVisualParent;
        public Image WeaponImage => weaponImage;
        public Image RarityOutline => rarityOutline;
        public InventoryItem MyItemRef { get; private set; }

        #endregion

        // Input
        #region

        public void RightClick()
        {
            Debug.Log("InventoryItemView.RightClick()");
            if (itemDragged != null)
            {
                return;
            }

            // TRY SELL
            if (TownController.Instance.ArmouryViewIsActive && !CharacterRosterViewController.Instance.MainVisualParent.activeSelf)
            {
                ItemController.Instance.HandleSellItemToArmoury(MyItemRef);
                return;
            }

            // TRY EQUIP
            if (MyItemRef.itemData == null)
            {
                return;
            }
            HexCharacterData character = CharacterRosterViewController.Instance.CharacterCurrentlyViewing;

            // Check equipping 2h item with two 1h items already equip without enough inventory space.
            if (MyItemRef.itemData.handRequirement == HandRequirement.TwoHanded &&
                character.itemSet.mainHandItem != null &&
                character.itemSet.offHandItem != null &&
                !InventoryController.Instance.HasFreeInventorySpace(2))
            {
                return;
            }

            // check equipping off hand item when using a 2H item
            if (MyItemRef.itemData.allowedSlot == WeaponSlot.Offhand &&
                character.itemSet.mainHandItem != null &&
                character.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
            {
                return;
            }

            // Get correct slot
            List<RosterItemSlot> allSlots = new List<RosterItemSlot>
            {
                CharacterRosterViewController.Instance.MainHandSlot,
                CharacterRosterViewController.Instance.OffHandSLot,
                CharacterRosterViewController.Instance.BodySLot,
                CharacterRosterViewController.Instance.HeadSlot,
                CharacterRosterViewController.Instance.TrinketSlot
            };

            RosterItemSlot matchingSlot = null;
            foreach (RosterItemSlot ris in allSlots)
            {
                if (ItemController.Instance.IsItemValidOnSlot(MyItemRef.itemData, ris))
                {
                    matchingSlot = ris;
                    break;
                }
            }

            if (matchingSlot != null)
            {
                // Add item to player
                ItemController.Instance.HandleGiveItemToCharacterFromInventory(character, MyItemRef, matchingSlot);

                // Rebuild roster, inventory and model views
                CharacterRosterViewController.Instance.HandleRedrawRosterOnCharacterUpdated();
                CharacterScrollPanelController.Instance.RebuildViews();
                InventoryController.Instance.RebuildInventoryView();
            }

        }
        public void MouseEnter()
        {
            Debug.Log("InventoryItemView.MouseEnter()");
            if (itemDragged != null)
            {
                return;
            }
            if (MyItemRef.abilityData != null)
            {
                AbilityPopupController.Instance.OnAbilityBookItemMousedOver(this);
            }
            if (MyItemRef.itemData != null)
            {
                ItemPopupController.Instance.OnInventoryItemMousedOver(this);
            }
        }
        public void MouseExit()
        {
            Debug.Log("InventoryItemView.MouseExit()");
            if (itemDragged != null)
            {
                return;
            }
            AbilityPopupController.Instance.HidePanel();
            ItemPopupController.Instance.HidePanel();
        }

        #endregion

        // Drag Logic
        #region

        private void OnMouseUp()
        {
            if (currentlyBeingDragged)
            {
                currentlyBeingDragged = false;
                itemDragged = null;
                HexCharacterData character = CharacterRosterViewController.Instance.CharacterCurrentlyViewing;

                // Stop dragging SFX
                AudioManager.Instance.FadeOutSound(Sound.UI_Dragging_Constant, 0.2f);

                // Check equipping 2h item with two 1h items already equip without enough inventory space.
                if (MyItemRef.itemData != null &&
                    MyItemRef.itemData.handRequirement == HandRequirement.TwoHanded &&
                    character.itemSet.mainHandItem != null &&
                    character.itemSet.offHandItem != null &&
                    !InventoryController.Instance.HasFreeInventorySpace(2)
                   )
                {
                    Debug.Log("InventoryItemView() warning, not enough inventory space to equip a 2H item while holding two 1H items, cancelling...");

                    // Move item back towards slot position
                    Sequence s = DOTween.Sequence();
                    InventorySlot slot = InventoryController.Instance.AllInventorySlots[InventoryController.Instance.Inventory.IndexOf(MyItemRef)];
                    s.Append(transform.DOMove(slot.transform.position, 0.25f));

                    // Re-parent self on arrival
                    s.OnComplete(() => transform.SetParent(slot.transform));
                    return;
                }

                // Was the drag succesful?
                if (DragSuccessful())
                {
                    // Snap drag item view back to inventory slot position
                    InventorySlot slot = InventoryController.Instance.AllInventorySlots[InventoryController.Instance.Inventory.IndexOf(MyItemRef)];

                    Sequence s = DOTween.Sequence();
                    s.Append(transform.DOMove(slot.transform.position, 0f));
                    s.OnComplete(() => transform.SetParent(slot.transform));

                    if (MyItemRef.itemData != null)
                    {
                        // add item to player
                        ItemController.Instance.HandleGiveItemToCharacterFromInventory
                            (character, MyItemRef, RosterItemSlot.SlotMousedOver);

                        // re build roster, inventory and model views
                        CharacterRosterViewController.Instance.HandleRedrawRosterOnCharacterUpdated();
                        CharacterScrollPanelController.Instance.RebuildViews();
                        InventoryController.Instance.RebuildInventoryView();
                    }
                    else if (MyItemRef.abilityData != null)
                    {
                        // to do: only run this code if the library page is actually open
                        AudioManager.Instance.PlaySound(Sound.UI_Drag_Drop_End);
                        TownController.Instance.LibraryAbilitySlot.BuildFromAbility(MyItemRef.abilityData);
                    }
                }

                else
                {
                    // Move item back towards slot position
                    Sequence s = DOTween.Sequence();
                    InventorySlot slot = InventoryController.Instance.AllInventorySlots[InventoryController.Instance.Inventory.IndexOf(MyItemRef)];
                    s.Append(transform.DOMove(slot.transform.position, 0.25f));

                    // Re-parent self on arrival
                    s.OnComplete(() => transform.SetParent(slot.transform));
                }
            }
        }
        public void OnMouseDrag()
        {
            Debug.Log("InventoryItemView.Drag()");
            //if (myItemRef == null || myItemRef.itemData == null) return;
            if (MyItemRef == null)
            {
                return;
            }

            // On drag start logic
            if (currentlyBeingDragged == false)
            {
                currentlyBeingDragged = true;
                itemDragged = this;

                // Play dragging SFX
                AudioManager.Instance.FadeInSound(Sound.UI_Dragging_Constant, 0.2f);

                // Hide pop up window
                ItemPopupController.Instance.HidePanel();
            }

            // Unparent from vert fitter, so it wont be masked while dragging
            transform.SetParent(InventoryController.Instance.DragParent);

            // Get the needed components, if we dont have them already
            if (dragCanvas == null)
            {
                dragCanvas = InventoryController.Instance.VisualParent.GetComponent<Canvas>();
            }

            if (dragTransform == null)
            {
                dragTransform = InventoryController.Instance.VisualParent.transform as RectTransform;
            }

            // Weird hoki poki magic for dragging in local space on a non screen overlay canvas
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragTransform, Input.mousePosition,
                dragCanvas.worldCamera, out pos);

            // Follow the mouse
            transform.position = dragCanvas.transform.TransformPoint(pos);

        }
        private bool DragSuccessful()
        {
            bool bRet = false;

            if (MyItemRef.itemData != null && RosterItemSlot.SlotMousedOver != null)
            {
                ItemData mainHandItem = CharacterRosterViewController.Instance.CharacterCurrentlyViewing.itemSet.mainHandItem;

                // Cant equip off hand if main hand weapon is 2 handed
                if (RosterItemSlot.SlotMousedOver.SlotType == RosterSlotType.OffHand &&
                    mainHandItem != null &&
                    mainHandItem.handRequirement == HandRequirement.TwoHanded)
                {
                    return false;
                }

                if (ItemController.Instance.IsItemValidOnSlot(MyItemRef.itemData, RosterItemSlot.SlotMousedOver,
                        CharacterRosterViewController.Instance.CharacterCurrentlyViewing))
                {
                    bRet = true;
                }
            }

            else if (MyItemRef.abilityData != null)
            {
                bRet = true;
            }

            return bRet;
        }

        #endregion

        // Misc
        #region

        public void Reset()
        {
            gameObject.SetActive(false);
            bookVisualParent.SetActive(false);
            weaponVisualParent.SetActive(false);
            goldCostParent.SetActive(false);
            MyItemRef = null;
        }
        public void SetMyItemRef(InventoryItem item)
        {
            MyItemRef = item;
        }

        #endregion
    }
}
