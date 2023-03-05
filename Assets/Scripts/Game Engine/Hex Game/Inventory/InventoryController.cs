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
        [SerializeField] private List<InventorySlot> allInventorySlots = new List<InventorySlot>();
        [SerializeField] private List<InventoryItemView> allInventoryItemViews = new List<InventoryItemView>();
        [SerializeField] private TextMeshProUGUI slotsMaxCountText;
        [SerializeField] private InventorySlot inventorySlotPrefab;
        [SerializeField] private Transform slotsParent;
        private int maxInventorySize = 1000;

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
        public List<InventorySlot> AllInventorySlots
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
        public void BuildAndShowInventoryView()
        {
            mainVisualParent.SetActive(true);
            BuildInventoryView();
            UpdateMaxSlotsTexts();
        }
        public void HideInventoryView()
        {
            mainVisualParent.SetActive(false);
        }
        #endregion

        // Input Logic
        #region
        public void OnInventoryTopBarButtonClicked()
        {
            if (GameController.Instance.GameState == GameState.StoryEvent) return;
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

            // Always show at least 24 slots
            for (int i = 0; i < 24; i++)
                AllInventorySlots[i].Show();

            for(int i = 0; i < inventory.Count; i++)
            {
                // Create a new slot if not enough available
                if(i >= allInventorySlots.Count)
                {
                    InventorySlot newSlot = Instantiate(inventorySlotPrefab, slotsParent).GetComponent<InventorySlot>();
                    allInventorySlots.Add(newSlot);
                    allInventoryItemViews.Add(newSlot.MyItemView);
                }
                allInventorySlots[i].Show();

                BuildInventoryItemViewFromInventoryItemData(allInventoryItemViews[i], inventory[i]);
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
            //slotsMaxCountText.text = inventory.Count.ToString() + " / " + maxInventorySize.ToString();
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
            var items = ItemController.Instance.GetAllShopSpawnableItems();
            items.Shuffle();
            return CreateInventoryItemFromItemData(ItemController.Instance.GenerateNewItemWithRandomEffects(items[0]));
        }
        private InventoryItem GenerateRandomNonEquipableItem()
        {
            // to do in future: add arguments for rarity and type
            var items = ItemController.Instance.GetAllNonShopAndContractItems();
            items.Shuffle();
            return CreateInventoryItemFromItemData(ItemController.Instance.GenerateNewItemWithRandomEffects(items[0]));
        }
        private void AddRandomAbilityBookToInventory(int amount)
        {
            List<AbilityData> abilities = new List<AbilityData>();
            foreach (AbilityData a in AbilityController.Instance.AllAbilities)
            {
                if (a.talentRequirementData.talentSchool != TalentSchool.None &&
                    a.talentRequirementData.talentSchool != TalentSchool.Neutral)
                    abilities.Add(a);
            }
            abilities.Shuffle();

            for(int i = 0; i < amount; i++)
            {
                AddItemToInventory(CreateInventoryItemAbilityData(abilities[i]));
            }
           
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
        public bool HasFreeInventorySpace(int freeSpace = 1)
        {
            return inventory.Count + freeSpace <= maxInventorySize;
        }
       
        #endregion

        // Misc
        #region
        public void PopulateInventoryWithMockDataItems(int totalItemsGenerated)
        {
            for (int i = 0; i < totalItemsGenerated; i++)
            {
                AddItemToInventory(GenerateRandomNonEquipableItem());
            }
        }
        public void PopulateInventoryWithMockAbilityBooks(int totalItemsGenerated)
        {
            AddRandomAbilityBookToInventory(totalItemsGenerated);
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
            // TO DO: Recreate discard item screen pop up, the uncomment
            /*
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
            */


        }
        public void OnConfirmActionScreenConfirmButtonClicked()
        {           
            HexCharacterData c = CharacterRosterViewController.Instance.CharacterCurrentlyViewing;

            if (currentActionType == ItemActionType.Use && itemInSelection.abilityData != null && c != null)
            {
                c.abilityBook.HandleLearnNewAbility(itemInSelection.abilityData);
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