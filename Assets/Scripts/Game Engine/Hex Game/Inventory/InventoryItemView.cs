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
        [SerializeField] private ItemGridScrollView myItemGrid;

        [Header("Ability Book Components")]
        [SerializeField] private GameObject bookVisualParent;
        [SerializeField] private Image bookImage;

        [Header("Item Components")]
        [SerializeField] private GameObject weaponVisualParent;
        [SerializeField] private Image weaponImage;

        [Header("Cost Components")]
        [SerializeField] private GameObject goldCostParent;
        [SerializeField] private TextMeshProUGUI goldCostText;

        public static InventoryItemView ItemDragged { get; private set; }

        // Drag values
        [HideInInspector] public bool currentlyBeingDragged;
        

        #endregion

        // Getters + Accessors
        #region

        public GameObject GoldCostParent => goldCostParent;
        public TextMeshProUGUI GoldCostText => goldCostText;
        public GameObject BookVisualParent => bookVisualParent;
        public Image BookImage => bookImage;
        public GameObject WeaponVisualParent => weaponVisualParent;
        public Image WeaponImage => weaponImage;
        public InventoryItem MyItemRef { get; private set; }
        public void AssignGrid(ItemGridScrollView grid)
        {
            myItemGrid = grid;
        }

        #endregion

        // Input
        #region

        public void RightClick()
        {
            Debug.Log("InventoryItemView.RightClick()");
            if (ItemDragged != null)
            {
                return;
            }            

            if (MyItemRef.itemData == null && MyItemRef.abilityData == null)
            {
                return;
            }
            
            // TRY SELL
            if (myItemGrid.CollectionSource == ItemCollectionSource.PlayerInventoryShop &&
                !CharacterRosterViewController.Instance.MainVisualParent.activeSelf)
            {
                ItemController.Instance.HandleSellItem(MyItemRef);
                return;
            }

            // TRY EQUIP
            else if (myItemGrid.CollectionSource == ItemCollectionSource.PlayerInventoryCharacterRoster)
            {
                if (MyItemRef.abilityData != null)
                {
                    CharacterRosterViewController.Instance.OnAbilityTomeRightClicked(this);
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
                    myItemGrid.BuildItemCollectionView();
                }
            }
            

        }
        public void MouseEnter()
        {
            Debug.Log("InventoryItemView.MouseEnter()");
            if (ItemDragged != null)
            {
                return;
            }
            if (MyItemRef.abilityData != null)
            {
                AbilityPopupController.Instance.OnAbilityBookItemMousedOver(this);
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(MyItemRef.abilityData.keyWords);
            }
            if (MyItemRef.itemData != null)
            {
                ItemPopupController.Instance.OnInventoryItemMousedOver(this);
            }
        }
        public void MouseExit()
        {
            Debug.Log("InventoryItemView.MouseExit()");
            if (ItemDragged != null)
            {
                return;
            }
            AbilityPopupController.Instance.HidePanel();
            ItemPopupController.Instance.HidePanel();
            KeyWordLayoutController.Instance.FadeOutMainView();
        }

        #endregion

        // Drag Logic
        #region

        private void OnMouseUp()
        {
            myItemGrid.SetVerticalScrolling(true);
            if (myItemGrid.CollectionSource == ItemCollectionSource.PlayerInventoryShop)
            {
                return;
            }
            if (MyItemRef == null || (MyItemRef != null && MyItemRef.abilityData != null)) return;

            if (currentlyBeingDragged)
            {
                currentlyBeingDragged = false;
                ItemDragged = null;
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
                    InventorySlot slot = myItemGrid.Slots[InventoryController.Instance.PlayerInventory.IndexOf(MyItemRef)];
                    s.Append(transform.DOMove(slot.transform.position, 0.25f));

                    // Re-parent self on arrival
                    s.OnComplete(() => transform.SetParent(slot.transform));
                    return;
                }

                // Was the drag succesful?
                if (DragSuccessful())
                {
                    // Snap drag item view back to inventory slot position
                    InventorySlot slot = myItemGrid.Slots[InventoryController.Instance.PlayerInventory.IndexOf(MyItemRef)];

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
                        myItemGrid.BuildItemCollectionView();
                    }
                }

                else
                {
                    // Move item back towards slot position
                    Sequence s = DOTween.Sequence();
                    InventorySlot slot = myItemGrid.Slots[InventoryController.Instance.PlayerInventory.IndexOf(MyItemRef)];
                    s.Append(transform.DOMove(slot.transform.position, 0.25f));

                    // Re-parent self on arrival
                    s.OnComplete(() => transform.SetParent(slot.transform));
                }
            }
        }
        public void OnMouseDrag()
        {
            Debug.Log("InventoryItemView.Drag()");
            if(myItemGrid.CollectionSource == ItemCollectionSource.PlayerInventoryShop)
            {
                return;
            }
            if (MyItemRef == null || (MyItemRef != null && MyItemRef.abilityData != null))
            {
                return;
            }

            myItemGrid.SetVerticalScrolling(false);

            // On drag start logic
            if (currentlyBeingDragged == false)
            {
                currentlyBeingDragged = true;
                ItemDragged = this;

                // Play dragging SFX
                AudioManager.Instance.FadeInSound(Sound.UI_Dragging_Constant, 0.2f);

                // Hide pop up window
                ItemPopupController.Instance.HidePanel();
            }

            transform.SetParent(myItemGrid.DragTransform);

            // Weird hoki poki magic for dragging in local space on a non screen overlay canvas
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(myItemGrid.DragTransform, Input.mousePosition,
                myItemGrid.DragCanvas.worldCamera, out pos);

            // Follow the mouse
            transform.position = myItemGrid.DragCanvas.transform.TransformPoint(pos);

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
