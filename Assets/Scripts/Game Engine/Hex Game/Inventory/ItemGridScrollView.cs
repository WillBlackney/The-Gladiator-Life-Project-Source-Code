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

        [Space(10)]
        [Header("Components")]
        [SerializeField] private List<InventorySlot> slots;
        [SerializeField] private Canvas dragCanvas;

        [Header("Misc")]
        [SerializeField] private RectTransform[] layoutsRebuilt;
        [SerializeField] private ScrollRect scrollView;
        [SerializeField] private Scrollbar scrollBar;

        private List<InventoryItemView> itemViews = new List<InventoryItemView>();
        public List<InventorySlot> Slots  => slots;
        public Canvas DragCanvas => dragCanvas;
        public RectTransform DragTransform => dragCanvas.transform as RectTransform;

        private void GetItemViewsComponents()
        {
            itemViews.Clear();
            slots.ForEach(slot => itemViews.Add(slot.MyItemView));
        }

        public void BuildInventoryView(bool resetSliders = false)
        {
            GetItemViewsComponents();

            // Reset item views + slots
            foreach (InventoryItemView i in itemViews)
            {
                i.Reset();
            }
            foreach (InventorySlot s in slots)
            {
                s.Reset();
            }

            // Show minimum slots
            for (int i = 0; i < minimumSlotsShown; i++)
            {
                slots[i].Show();
            }

            // Build item view for each player item
            for (int i = 0; i < InventoryController.Instance.PlayerInventory.Count; i++)
            {
                if (i >= minimumSlotsShown)
                {
                    slots[i].Show();
                }
                BuildInventoryItemViewFromInventoryItemData(itemViews[i], InventoryController.Instance.PlayerInventory[i]);
            }

            TransformUtils.RebuildLayouts(layoutsRebuilt);
            if(resetSliders) ResetScrollView();
        }

        private void BuildInventoryItemViewFromInventoryItemData(InventoryItemView view, InventoryItem item)
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

            /*
            if (TownController.Instance.ArmouryViewIsActive)
            {
                view.GoldCostParent.SetActive(true);
                view.GoldCostText.text = item.GetSellPrice().ToString();
            }*/

        }

        private void ResetScrollView()
        {
            scrollView.verticalNormalizedPosition = 1;
            scrollBar.value = 1;
        }

        private void OnEnable()
        {
            ResetScrollView();
        }


    }
}