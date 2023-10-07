using System.Collections.Generic;
using TMPro;
using UnityEditor.Graphs;
using UnityEngine;
using WeAreGladiators.Abilities;
using WeAreGladiators.Characters;
using WeAreGladiators.Libraries;
using WeAreGladiators.Persistency;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Items
{
    public class InventoryController : Singleton<InventoryController>
    {
        #region Properties + Components       

        private readonly int maxInventorySize = 10000;

        #endregion

        #region Getters + Accessors
        public List<InventoryItem> PlayerInventory { get; private set; } = new List<InventoryItem>();

        #endregion

        #region Conditional Checks

        public bool HasFreeInventorySpace(int freeSpace = 1)
        {
            return PlayerInventory.Count + freeSpace <= maxInventorySize;
        }

        #endregion

        
        #region Persistency Logic

        public void SaveMyDataToSaveFile(SaveGameData saveData)
        {
            saveData.inventory = PlayerInventory;
        }
        public void BuildMyDataFromSaveFile(SaveGameData saveData)
        {
            PlayerInventory = saveData.inventory;
        }

        #endregion

        #region Create Inventory items

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
            List<ItemData> items = ItemController.Instance.GetAllShopSpawnableItems();
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
                {
                    abilities.Add(a);
                }
            }
            abilities.Shuffle();

            for (int i = 0; i < amount; i++)
            {
                AddItemToInventory(abilities[i]);
            }

        }

        #endregion

        #region Modify Inventory Contents

        public void AddItemToInventory(InventoryItem item, bool ignoreSpaceRequirement = false, int inventoryIndex = -1)
        {
            if (HasFreeInventorySpace() ||
                !HasFreeInventorySpace() && ignoreSpaceRequirement)
            {
                if (inventoryIndex >= 0)
                {
                    PlayerInventory.Insert(inventoryIndex, item);
                }
                else
                {
                    PlayerInventory.Add(item);
                }
            }

        }
        public void AddItemToInventory(ItemData item, bool ignoreSpaceRequirement = false, int inventoryIndex = -1)
        {
            if (HasFreeInventorySpace() ||
                !HasFreeInventorySpace() && ignoreSpaceRequirement)
            {
                InventoryItem inventoryItem = CreateInventoryItemFromItemData(item);
                if (inventoryIndex >= 0)
                {
                    PlayerInventory.Insert(inventoryIndex, inventoryItem);
                }
                else
                {
                    PlayerInventory.Add(inventoryItem);
                }
            }

        }
        public void AddItemToInventory(AbilityData ability, bool ignoreSpaceRequirement = false, int inventoryIndex = -1)
        {
            if (HasFreeInventorySpace() ||
                !HasFreeInventorySpace() && ignoreSpaceRequirement)
            {
                InventoryItem inventoryItem = CreateInventoryItemAbilityData(ability);
                if (inventoryIndex >= 0)
                {
                    PlayerInventory.Insert(inventoryIndex, inventoryItem);
                }
                else
                {
                    PlayerInventory.Add(inventoryItem);
                }
            }

        }
        public void RemoveItemFromInventory(InventoryItem item)
        {
            if (PlayerInventory.Contains(item))
            {
                PlayerInventory.Remove(item);
            }
        }

        #endregion

        #region Misc

        public void PopulateInventoryWithMockDataItems(int totalItemsGenerated)
        {
            for (int i = 0; i < totalItemsGenerated; i++)
            {
                AddItemToInventory(GenerateRandomEquipableItem());
            }
        }
        public void PopulateInventoryWithMockAbilityBooks(int totalItemsGenerated)
        {
            AddRandomAbilityBookToInventory(totalItemsGenerated);
        }

        #endregion

        #region Item Confirm Action Screen Logic

       
       

        #endregion
    }

    public enum ItemActionType
    {
        None = 0,
        Use = 1,
        Discard = 2
    }
}
