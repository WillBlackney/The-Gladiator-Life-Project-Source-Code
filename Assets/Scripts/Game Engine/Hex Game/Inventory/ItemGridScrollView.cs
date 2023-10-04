using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
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
                    BuildInventoryItemViewFromData(inventoryItemViews[i], InventoryController.Instance.PlayerInventory[i]);
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
                    armouryItemViews[i].BuildFromItemShopData(TownController.Instance.currentArmouryItems[i]);
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


    }

    public enum ItemCollectionSource
    {
        PlayerInventoryCharacterRoster = 0,
        PlayerInventoryShop = 1,
        ArmouryShop = 2,
        LibraryShop = 3,
    }
}