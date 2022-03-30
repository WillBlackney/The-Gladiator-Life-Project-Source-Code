using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Persistency;
using TMPro;
using HexGameEngine.Libraries;
using HexGameEngine.Abilities;
using HexGameEngine.UI;
using HexGameEngine.Characters;
using HexGameEngine.Cards;

namespace HexGameEngine.Items
{
    public class InventoryController : Singleton<InventoryController>
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private InventorySlot[] allInventorySlots;
        [SerializeField] private InventoryItemView[] allInventoryItemViews;
        [SerializeField] private TextMeshProUGUI slotsMaxCountText;
        private int maxInventorySize = 30;

        [Header("Confirm Action Screen Components")]
        [SerializeField] private GameObject confirmActionScreenVisualParent;
        [SerializeField] private TextMeshProUGUI confirmActionScreenText;
        private InventoryItem itemInSelection;
        private ItemActionType currentActionType;

        [Header("Misc Stuff")]
        [SerializeField] private Transform dragParent;


        private List<InventoryItem> inventory = new List<InventoryItem>();
        #endregion

        // Getters + Accessors
        #region
        public GameObject MainVisualParent
        {
            get { return mainVisualParent; }
        }
        public Transform DragParent
        {
            get { return dragParent; }
        }
        public List<InventoryItem> Inventory
        {
            get { return inventory; }
        }
        public InventorySlot[] AllInventorySlots
        {
            get { return allInventorySlots; }
        }
        #endregion

        // Persistency Logic
        #region
        public void SaveMyDataToSaveFile(SaveGameData saveData)
        {
            saveData.inventory = inventory;
        }
        public void BuildMyDataFromSaveFile(SaveGameData saveData)
        {
            inventory = saveData.inventory;
        }
        #endregion

        // Build + Show Inventory Views
        #region
        private void BuildAndShowInventoryView()
        {
            mainVisualParent.SetActive(true);
            BuildInventoryView();
            UpdateMaxSlotsTexts();
        }
        private void HideInventoryView()
        {
            mainVisualParent.SetActive(false);
        }
        #endregion

        // Input Logic
        #region
        public void OnInventoryTopBarButtonClicked()
        {
            if (!mainVisualParent.activeSelf)
                BuildAndShowInventoryView();
            else HideInventoryView();
        }
        #endregion

        // Build Inventory View
        #region
        public void RebuildInventoryView()
        {
            BuildInventoryView();
            UpdateMaxSlotsTexts();
        }
        private void BuildInventoryView()
        {
            // Reset item views + slots
            foreach (InventoryItemView i in allInventoryItemViews)
                i.Reset();
            foreach(InventorySlot s in allInventorySlots)            
                s.Reset();            

            for(int i = 0; i < maxInventorySize; i++)
            {
                allInventorySlots[i].gameObject.SetActive(true);

                if (i < inventory.Count)
                {
                    BuildInventoryItemViewFromInventoryItemData(allInventoryItemViews[i], inventory[i]);
                }
            }            
        }
        private void BuildInventoryItemViewFromInventoryItemData(InventoryItemView view, InventoryItem item)
        {
            view.gameObject.SetActive(true);
            view.SetMyItemRef(item);

            if (item.abilityData != null)
            {
                view.BookVisualParent.SetActive(true);
                view.BookImage.sprite = SpriteLibrary.Instance.
                    GetTalentSchoolBookSprite(item.abilityData.talentRequirementData.talentSchool);
            }
            else if(item.itemData != null)
            {
                view.WeaponVisualParent.SetActive(true);
                view.WeaponImage.sprite = item.itemData.ItemSprite;
                view.RarityOutline.color = ColorLibrary.Instance.GetRarityColor(item.itemData.rarity);
            }

        }
        private void UpdateMaxSlotsTexts()
        {
            slotsMaxCountText.text = inventory.Count.ToString() + " / " + maxInventorySize.ToString();
        }
        #endregion

        // Create Inventory items
        #region
        public InventoryItem CreateInventoryItemAbilityData(AbilityData abilityData)
        {
            return new InventoryItem(abilityData);
        }
        public InventoryItem CreateInventoryItemFromItemData(ItemData itemData)
        {
            return new InventoryItem(itemData);
        }
        private InventoryItem GenerateRandomEquipableItem()
        {
            // to do in future: add arguments for rarity and type
            var items = ItemController.Instance.GetAllLootableItems();
            items.Shuffle();
            return CreateInventoryItemFromItemData(ItemController.Instance.GenerateNewItemWithRandomEffects(items[0]));
        }
        #endregion

        // Modify Inventory Contents
        #region
        public void AddItemToInventory(InventoryItem item, bool ignoreSpaceRequirement = false, int inventoryIndex = -1)
        {
            if(HasFreeInventorySpace() ||
               (!HasFreeInventorySpace() && ignoreSpaceRequirement == true))
            {
                if(inventoryIndex >= 0) inventory.Insert(inventoryIndex, item);
                else inventory.Add(item);
            }
                
        }
        public void RemoveItemFromInventory(InventoryItem item)
        {
            if (inventory.Contains(item))
                inventory.Remove(item);
        }
        #endregion

        // Conditional Checks
        #region
        private bool HasFreeInventorySpace()
        {
            return inventory.Count < maxInventorySize;
        }
        public bool IsItemValidOnSlot(ItemData item, RosterItemSlot slot, HexCharacterData character = null)
        {
            bool bRet = false;
            ItemType itemType = item.itemType;

            if (!slot)
            {
                Debug.Log("IsItemValidOnSlot() returning false: slot is null");
                return false;
            }

            if (itemType == ItemType.Trinket && slot.SlotType == RosterSlotType.Trinket)
                bRet = true;
            else if (itemType == ItemType.Weapon)
            {
                if ((item.allowedSlot == WeaponSlot.MainHand && slot.SlotType == RosterSlotType.MainHand) ||
                    (item.allowedSlot == WeaponSlot.Offhand && slot.SlotType == RosterSlotType.OffHand))
                    bRet = true;
            }

            // check if adding offhand item while holding a 2h item in the main hand (not allowed)
            if(character != null)
            {
                if(character.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded &&
                    slot.SlotType == RosterSlotType.OffHand)
                {
                    Debug.Log("IsItemValidOnSlot() returning false: cant add off hand item while using a 2H weapon");
                    return false;
                }

                if (character.itemSet.offHandItem != null && item.handRequirement == HandRequirement.TwoHanded)
                {
                    Debug.Log("IsItemValidOnSlot() returning false: cant add two handed item while holding an off hand item");
                    return false;
                }
            }

            Debug.LogWarning("IsItemValidOnSlot() returning " + bRet.ToString());

            return bRet;

        }

        #endregion

        // Misc
        #region
        public void PopulateInventoryWithMockDataItems(int totalItemsGenerated)
        {
            for(int i = 0; i < totalItemsGenerated; i++)
            {
                //AddItemToInventory(GenerateRandomAbilityBookItem());
                AddItemToInventory(GenerateRandomEquipableItem());
                /*
                if(RandomGenerator.NumberBetween(1,2) == 1)
                    AddItemToInventory(GenerateRandomAbilityBookItem());
                else
                    AddItemToInventory(GenerateRandomEquipableItem());
                */
            }
        }
        #endregion

        // Item Usage Logic
        #region
        public void OnItemViewClicked(InventoryItemView view)
        {
            // Close pop views
            AbilityPopupController.Instance.HidePanel();
            ItemPopupController.Instance.HidePanel();

            // Left click (use)
            if (Input.GetKey(KeyCode.Mouse0))
                BuildAndShowConfirmActionScreenFromItem(view.MyItemRef, ItemActionType.Use);

            // Right click (destroy)
            else if (Input.GetKey(KeyCode.Mouse1))
                BuildAndShowConfirmActionScreenFromItem(view.MyItemRef, ItemActionType.Discard);
        }
       
        #endregion

        // Item Confirm Action Screen Logic
        #region
        private void BuildAndShowConfirmActionScreenFromItem(InventoryItem item, ItemActionType actionType)
        {
            HexCharacterData c = CharacterRosterViewController.Instance.CharacterCurrentlyViewing;            
            itemInSelection = item;
            currentActionType = actionType;
            confirmActionScreenVisualParent.SetActive(true);

            if(item.abilityData != null && c != null && actionType == ItemActionType.Use)
            {
                // check ability learn is valid
                if (!AbilityController.Instance.DoesCharacterMeetAbilityBookRequirements(c, item.abilityData) ||
                    GameController.Instance.GameState == GameState.CombatActive)
                {
                    itemInSelection = null;
                    currentActionType = ItemActionType.None;
                    confirmActionScreenVisualParent.SetActive(false);
                }

                confirmActionScreenText.text = "Are you sure you want to teach " + item.abilityData.abilityName +
                    " to " + c.myName + "?";
            }
            else if (actionType == ItemActionType.Discard)
            {
                string itemName = "";
                if (item.abilityData != null)
                    itemName = item.abilityData.abilityName + " Book";
                else if (item.itemData != null)
                    itemName = item.itemData.itemName;

                confirmActionScreenText.text = "Are you sure you want to discard " + itemName + "?";
            }
            else
            {
                itemInSelection = null;
                currentActionType = ItemActionType.None;
                confirmActionScreenVisualParent.SetActive(false);
            }


        }
        public void OnConfirmActionScreenConfirmButtonClicked()
        {           
            HexCharacterData c = CharacterRosterViewController.Instance.CharacterCurrentlyViewing;

            if (currentActionType == ItemActionType.Use && itemInSelection.abilityData != null && c != null)
            {
                AbilityController.Instance.HandleCharacterDataLearnNewAbility(c, c.abilityBook, itemInSelection.abilityData);

                // discard the ability book on usage

                // rerender character roster screen + inventory
                CharacterRosterViewController.Instance.HandleRedrawRosterOnCharacterUpdated();              
                RemoveItemFromInventory(itemInSelection);
                BuildAndShowInventoryView();
            }
            else if (currentActionType == ItemActionType.Discard)
            {
                // discard item logic here
                RemoveItemFromInventory(itemInSelection);
                BuildAndShowInventoryView();
            }

            currentActionType = ItemActionType.None;
            itemInSelection = null;
            confirmActionScreenVisualParent.SetActive(false);
        }
        public void OnConfirmActionScreenCancelButtonClicked()
        {
            itemInSelection = null;
            currentActionType = ItemActionType.None;
            confirmActionScreenVisualParent.SetActive(false);
        }

        #endregion

    }

    public enum ItemActionType
    {
        None = 0,
        Use = 1,
        Discard = 2,
    }
}