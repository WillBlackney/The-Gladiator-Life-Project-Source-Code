using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Libraries;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Items
{
    public class ItemGridScrollView : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int minimumSlotsShown;
        [SerializeField] private ItemCollectionSource collectionSource;
        [Space(20)]

        [Header("Components")]
        [SerializeField] private List<InventorySlot> slots;
        [SerializeField] private Canvas dragCanvas;
        [Space(20)]

        [Header("Misc")]
        [SerializeField] private RectTransform[] layoutsRebuilt;
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private Scrollbar scrollBar;

        private List<InventoryItemView> inventoryItemViews = new List<InventoryItemView>();
        private List<ItemShopSlot> armouryItemViews = new List<ItemShopSlot>();
        private bool initialized = false;
        private FilterSetting filterSetting = FilterSetting.All;

        public ItemCollectionSource CollectionSource => collectionSource;
        public List<InventorySlot> Slots => slots;
        public Canvas DragCanvas => dragCanvas;
        public RectTransform DragTransform => dragCanvas.transform as RectTransform;

        private void GetItemViewsComponents()
        {
            if (initialized) return;

            initialized = true;

            inventoryItemViews.Clear();
            slots.ForEach(slot => inventoryItemViews.Add(slot.MyItemView));

            armouryItemViews.Clear();
            slots.ForEach(slot => armouryItemViews.Add(slot.MyArmouryItemView));
        }

        public void BuildInventoryView(bool resetSliders = false)
        {
            GetItemViewsComponents();

            // Reset item views + slots
            foreach (InventoryItemView i in inventoryItemViews)
            {
                i.Reset();
            }
            foreach (InventorySlot s in slots)
            {
                s.Reset();
            }
            foreach (ItemShopSlot s in armouryItemViews)
            {
                s.Reset();
            }


            // Show minimum slots
            for (int i = 0; i < minimumSlotsShown; i++)
            {
                slots[i].Show();
            }

            int slotIndex = 0;
            if (collectionSource == ItemCollectionSource.PlayerInventoryShop ||
                collectionSource == ItemCollectionSource.PlayerInventoryCharacterRoster)
            {
                // Build item view for each player item
                for (int i = 0; i < InventoryController.Instance.PlayerInventory.Count; i++)
                {
                    if (i >= minimumSlotsShown)
                    {
                        slots[i].Show();
                    }
                    if (DoesInventoryItemMatchFilter(InventoryController.Instance.PlayerInventory[i], filterSetting))
                    {
                        BuildInventoryItemViewFromData(inventoryItemViews[slotIndex], InventoryController.Instance.PlayerInventory[i]);
                        slotIndex += 1;
                    }
                       
                }
            }

            else if (collectionSource == ItemCollectionSource.ArmouryShop)
            {
                // Build items
                for (int i = 0; i < TownController.Instance.currentArmouryItems.Count; i++)
                {
                    if (i >= minimumSlotsShown)
                    {
                        slots[i].Show();
                    }

                    if(DoesItemMatchFilter(TownController.Instance.currentArmouryItems[i].Item, filterSetting))
                    {
                        armouryItemViews[slotIndex].BuildFromItemShopData(TownController.Instance.currentArmouryItems[i]);
                        slotIndex += 1;
                    }                    
                }
            }

            else if (collectionSource == ItemCollectionSource.LibraryShop)
            {
                // Build items
                for (int i = 0; i < TownController.Instance.currentLibraryItems.Count; i++)
                {

                    if (i >= minimumSlotsShown)
                    {
                        slots[i].Show();
                    }

                    if (DoesItemMatchFilter(TownController.Instance.currentLibraryItems[i].Item, filterSetting))
                    {
                        armouryItemViews[slotIndex].BuildFromItemShopData(TownController.Instance.currentLibraryItems[i]);
                        slotIndex += 1;
                    }                 
                }
            }

            TransformUtils.RebuildLayouts(layoutsRebuilt);
            if (resetSliders) ResetScrollView();
        }

        private void BuildInventoryItemViewFromData(InventoryItemView view, InventoryItem item)
        {
            view.gameObject.SetActive(true);
            view.SetMyItemRef(item);

            if (item.abilityData != null)
            {
                view.BookVisualParent.SetActive(true);
                view.BookImage.sprite = SpriteLibrary.Instance.GetTalentSchoolBookSprite(item.abilityData.talentRequirementData.talentSchool);
            }
            else if (item.itemData != null)
            {
                view.WeaponVisualParent.SetActive(true);
                view.WeaponImage.sprite = item.itemData.ItemSprite;
            }

            if(collectionSource == ItemCollectionSource.PlayerInventoryShop)
            {
                view.GoldCostParent.SetActive(true);
                view.GoldCostText.text = item.GetSellPrice().ToString();
            }
        }

        private void ResetScrollView()
        {
            scrollView.verticalNormalizedPosition = 1;
            scrollBar.value = 1;
        }

        private void OnEnable()
        {
            ResetScrollView();
            SetVerticalScrolling(true);
        }

        public void SetVerticalScrolling(bool onOrOff)
        {
            scrollView.vertical = onOrOff;
        }

        public bool DoesItemMatchFilter(ItemData item, FilterSetting filter)
        {
            bool ret = false;
            if(filter == FilterSetting.All)
            {
                ret = true;
            }
            else if (filter == FilterSetting.AbilityBooks && item == null)
            {
                ret = true;
            }
            else if(filter == FilterSetting.Weapons &&
                item != null &&
                item.itemType != ItemType.Trinket &&
                (item.allowedSlot == WeaponSlot.Offhand ||
                item.allowedSlot == WeaponSlot.MainHand ||
                item.allowedSlot == WeaponSlot.EitherHand))
            {
                ret = true;
            }
            else if (filter == FilterSetting.Head && item != null && item.itemType == ItemType.Head)
            {
                ret = true;
            }
            else if (filter == FilterSetting.Body && item != null && item.itemType == ItemType.Body)
            {
                ret = true;
            }
            else if (filter == FilterSetting.Trinket && item != null && item.itemType == ItemType.Trinket)
            {
                ret = true;
            }

            return ret;
        }
        public bool DoesInventoryItemMatchFilter(InventoryItem item, FilterSetting filter)
        {
            bool ret = false;
            if (filter == FilterSetting.All)
            {
                ret = true;
            }
            else if(item.abilityData != null && filter == FilterSetting.AbilityBooks)
            {
                ret = true;
            }
            else if (filter == FilterSetting.Weapons &&
                item.itemData != null &&
                item.itemData.itemType != ItemType.Trinket &&
                (item.itemData.allowedSlot == WeaponSlot.Offhand ||
                item.itemData.allowedSlot == WeaponSlot.MainHand ||
                item.itemData.allowedSlot == WeaponSlot.EitherHand))
            {
                ret = true;
            }
            else if (filter == FilterSetting.Head && item.itemData != null && item.itemData.itemType == ItemType.Head)
            {
                ret = true;
            }
            else if (filter == FilterSetting.Body && item.itemData != null && item.itemData.itemType == ItemType.Body)
            {
                ret = true;
            }
            else if (filter == FilterSetting.Trinket && item.itemData != null && item.itemData.itemType == ItemType.Trinket)
            {
                ret = true;
            }

            return ret;
        }

        public void SetFilter(FilterSetting filter)
        {
            filterSetting = filter;
        }


    }

    public enum ItemCollectionSource
    {
        PlayerInventoryCharacterRoster = 0,
        PlayerInventoryShop = 1,
        ArmouryShop = 2,
        LibraryShop = 3,
    }

    public enum FilterSetting
    {
        All = 0,
        AbilityBooks = 1,
        Weapons = 2,
        Head = 3,
        Body = 4,
        Trinket = 5,

    }
}