using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Libraries;
using WeAreGladiators.RewardSystems;
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
        [SerializeField] private InventorySlot slotPrefab;
        [SerializeField] private List<InventorySlot> slots;
        [SerializeField] private Canvas dragCanvas;
        [Space(20)]

        [Header("Misc")]
        [SerializeField] private RectTransform[] layoutsRebuilt;
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private Scrollbar scrollBar;


        private GridLayoutGroup gridLayoutFitter;
        private List<InventoryItemView> inventoryItemViews = new List<InventoryItemView>();
        private List<ItemShopSlot> armouryItemViews = new List<ItemShopSlot>();
        private List<CombatLootIcon> lootIconViews = new List<CombatLootIcon>();
        private bool initialized = false;
        private FilterSetting filterSetting = FilterSetting.All;

        public ItemCollectionSource CollectionSource => collectionSource;
        public List<InventorySlot> Slots => slots;
        public Canvas DragCanvas => dragCanvas;
        public RectTransform DragTransform => dragCanvas.transform as RectTransform;
        public GridLayoutGroup GridLayoutFitter
        {
            get
            {
                if(gridLayoutFitter == null)
                {
                    gridLayoutFitter = GetComponentInChildren<GridLayoutGroup>(true);
                }
                return gridLayoutFitter;
            }
        }

        private void GetItemViewsComponents()
        {
            if (initialized) return;

            initialized = true;

            inventoryItemViews.Clear();
            slots.ForEach(slot => inventoryItemViews.Add(slot.MyItemView));

            armouryItemViews.Clear();
            slots.ForEach(slot => armouryItemViews.Add(slot.MyArmouryItemView));

            lootIconViews.Clear();
            slots.ForEach(slot => lootIconViews.Add(slot.MylootIconView));
        }

        public void BuildItemCollectionView(bool resetSliders = false)
        {
            GetItemViewsComponents();
            ResetSlots();

            int slotIndex = 0;
            if (collectionSource == ItemCollectionSource.PlayerInventoryShop ||
                collectionSource == ItemCollectionSource.PlayerInventoryCharacterRoster)
            {
                EnsureEnoughSlots(InventoryController.Instance.PlayerInventory.Count);

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
                        inventoryItemViews[slotIndex].allowKeyWordModal = true;
                        if (collectionSource == ItemCollectionSource.PlayerInventoryCharacterRoster)
                        {
                            inventoryItemViews[slotIndex].allowKeyWordModal = false;
                        }
                        slotIndex += 1;
                    }
                }
            }

            else if (collectionSource == ItemCollectionSource.ArmouryShop)
            {
                EnsureEnoughSlots(TownController.Instance.currentArmouryItems.Count);

                // Build items
                for (int i = 0; i < TownController.Instance.currentArmouryItems.Count; i++)
                {
                    if (i >= minimumSlotsShown)
                    {
                        slots[i].Show();
                    }

                    if (DoesItemMatchFilter(TownController.Instance.currentArmouryItems[i].Item, filterSetting))
                    {
                        armouryItemViews[slotIndex].BuildFromItemShopData(TownController.Instance.currentArmouryItems[i]);
                        slotIndex += 1;
                    }
                }
            }

            else if (collectionSource == ItemCollectionSource.LibraryShop)
            {
                EnsureEnoughSlots(TownController.Instance.currentLibraryItems.Count);

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

            EnsureEvenRows(slotIndex);
            TransformUtils.RebuildLayouts(layoutsRebuilt);
            if (resetSliders) ResetScrollView();
        }

        public void BuildForLootRewards(CombatContractData contract, List<ItemData> extraLoot)
        {
            int nextSlotIndex = 0;
            ResetSlots();
            BuildItemCollectionView(true);
            if(contract.combatRewardData.goldAmount > 0)
            {
                lootIconViews[nextSlotIndex].BuildAsGoldReward(contract.combatRewardData.goldAmount);
                nextSlotIndex += 1;
            }
            if (contract.combatRewardData.abilityAwarded != null)
            {
                lootIconViews[nextSlotIndex].BuildAsAbilityReward(contract.combatRewardData.abilityAwarded);
                nextSlotIndex += 1;
            }
            if (contract.combatRewardData.item != null)
            {
                lootIconViews[nextSlotIndex].BuildAsItemReward(contract.combatRewardData.item);
                nextSlotIndex += 1;
            }

            // Bonus loot
            for(int i = 0; i < extraLoot.Count; i++)
            {
                lootIconViews[nextSlotIndex + i].BuildAsItemReward(extraLoot[i]);
            }

        }

        public void ResetSlots()
        {
            // Reset item views + slots
            inventoryItemViews.ForEach(i => i.Reset());
            armouryItemViews.ForEach(i => i.Reset());
            lootIconViews.ForEach(i => i.Reset());
            slots.ForEach(i => i.Reset());

            // Show minimum slots
            for (int i = 0; i < minimumSlotsShown; i++)
            {
                slots[i].Show();
            }
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

        private void EnsureEvenRows(int slotsInUse)
        {
            if (slotsInUse < minimumSlotsShown) return;
            int slotsPerRow = GridLayoutFitter.constraintCount;
            int slotsOnIncompleteRow = slotsInUse % slotsPerRow;
            int toShow = slotsPerRow - slotsOnIncompleteRow;
            Debug.Log("Slots in use = " + slotsInUse.ToString() + ", loose slots = " + slotsOnIncompleteRow.ToString());
            for (int i = 0; i < toShow; i++)
            {
                slots[slotsInUse + i].Show();
            }
        }

        private void EnsureEnoughSlots(int requiredSlots)
        {
            int slotsToCreate = requiredSlots - slots.Count;
            if(slotsToCreate <= 0)
            {
                return;
            }

            slotsToCreate += GridLayoutFitter.constraintCount;

            for (int i = 0; i < slotsToCreate; i++)
            {
                InventorySlot newSlot = Instantiate(slotPrefab, GridLayoutFitter.transform);
                newSlot.MyItemView.AssignGrid(this);
                slots.Add(newSlot);
                newSlot.Reset();
            }

            initialized = false;
            GetItemViewsComponents();
        }


    }

    public enum ItemCollectionSource
    {
        PlayerInventoryCharacterRoster = 0,
        PlayerInventoryShop = 1,
        ArmouryShop = 2,
        LibraryShop = 3,
        RewardScreenLoot = 4,
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