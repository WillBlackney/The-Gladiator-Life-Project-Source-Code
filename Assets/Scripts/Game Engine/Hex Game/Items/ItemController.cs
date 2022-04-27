using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.UCM;
using HexGameEngine.Characters;
using HexGameEngine.Abilities;
using HexGameEngine.Utilities;
using System;
using System.Linq;
using HexGameEngine.UI;
using HexGameEngine.Perks;

namespace HexGameEngine.Items
{
    public class ItemController : Singleton<ItemController>
    {
        // Variables + Properties
        #region
        [Header("Item Library Properties")]
        [SerializeField] private ItemDataSO[] allItemScriptableObjects;
        private ItemData[] allItems;
        #endregion

        // Getters
        #region
        public ItemData[] AllItems
        {
            get { return allItems; }
            private set { allItems = value; }
        }
        public ItemDataSO[] AllItemScriptableObjects
        {
            get { return allItemScriptableObjects; }
            private set { allItemScriptableObjects = value; }
        }
        #endregion

        // Library Logic + Initialization
        #region
        protected override void Awake()
        {
            base.Awake();
            BuildItemLibrary();
        }
        private void BuildItemLibrary()
        {
            Debug.Log("ItemController.BuildItemLibrary() called...");

            List<ItemData> tempList = new List<ItemData>();

            foreach (ItemDataSO dataSO in allItemScriptableObjects)
            {
                if (dataSO.includeInLibrary)
                    tempList.Add(BuildItemDataFromScriptableObjectData(dataSO));
            }

            AllItems = tempList.ToArray();
        }      
        public ItemData GetItemDataByName(string name)
        {
            ItemData itemReturned = null;

            foreach (ItemData icon in AllItems)
            {
                if (icon.itemName == name)
                {
                    itemReturned = icon;
                    break;
                }
            }

            if (itemReturned == null)
            {
                Debug.Log("ItemController.GetItemDataByName() could not find a item SO with the name " +
                    name + ", returning null...");
            }

            return itemReturned;
        }
        public List<ItemData> GetAllLootableItems()
        {
            return Array.FindAll(allItems, i => i.lootable).ToList();
        }
        public List<ItemData> GetAllLootableItems(Rarity rarity)
        {
            return Array.FindAll(allItems, i => i.lootable && i.rarity == rarity).ToList();
        }
        public ItemData GetRandomLootableItemByRarity(Rarity rarity)
        {
            ItemData ret = null;
            var items = GetAllLootableItems();
            items.Shuffle();
            for(int i = 0; i < items.Count; i++)
            {
                if(items[i].rarity == rarity)
                {
                    ret = items[i];
                    break;
                }                      
            }

            return ret;
        }
       
        #endregion

        // Data Conversion
        #region 
        public ItemData BuildItemDataFromScriptableObjectData(ItemDataSO d)
        {
            ItemData i = new ItemData();
            i.itemName = d.itemName;
            i.itemDescription = d.itemDescription;
            i.itemType = d.itemType;
            i.lootable = d.lootable;
            i.includeInLibrary = d.includeInLibrary;

            i.rarity = d.rarity;
            i.weaponClass = d.weaponClass;
            i.handRequirement = d.handRequirement;
            i.allowedSlot = d.allowedSlot;
            i.injuryTypesCaused = d.injuryTypesCaused;
            i.itemEffectSets = d.itemEffectSets;

            foreach (AbilityDataSO ability in d.grantedAbilities)
            {
                i.grantedAbilities.Add(AbilityController.Instance.BuildAbilityDataFromScriptableObjectData(ability));
            }
            return i;
        }
        public ItemData CloneItem(ItemData original)
        {
            ItemData i = new ItemData();
            i.itemName = original.itemName;
            i.itemDescription = original.itemDescription;
            i.itemType = original.itemType;
            i.lootable = original.lootable;
            i.includeInLibrary = original.includeInLibrary;

            i.rarity = original.rarity;
            i.weaponClass = original.weaponClass;
            i.handRequirement = original.handRequirement;
            i.allowedSlot = original.allowedSlot;
            i.injuryTypesCaused = original.injuryTypesCaused;
            i.itemEffectSets = original.itemEffectSets;
            i.grantedAbilities = original.grantedAbilities;
            return i;
        }
        public ItemData GenerateNewItemWithRandomEffects(ItemData original)
        {
            ItemData ret = CloneItem(original);
            ret.itemEffects = GenerateRandomItemEffects(original);
            Debug.Log("GenerateNewItemWithRandomEffects() Granted abilities = " + ret.grantedAbilities.Count());
            return ret;
        }
        private List<ItemEffect> GenerateRandomItemEffects(ItemData itemData)
        {
            List<ItemEffect> ret = new List<ItemEffect>();

            // Generate armour
            itemData.armourAmount = RandomGenerator.NumberBetween(itemData.minArmourRoll, itemData.maxArmourRoll);

            // Generate effects
            for(int i = 0; i < itemData.itemEffectSets.Length; i++)
            {
                ItemEffectSet set = itemData.itemEffectSets[i];
                if (i == 0)
                {
                    ret.Add(set.possibleEffects[RandomGenerator.NumberBetween(0, set.possibleEffects.Length - 1)]);
                }
                else
                {
                    // Prevent doubling up on effect types
                    List<ItemEffect> validEffects = new List<ItemEffect>();
                    foreach (ItemEffect ie1 in set.possibleEffects)
                    {
                        foreach (ItemEffect ie2 in ret)
                        {
                            if (ie2.attributeModified != ie1.attributeModified)
                            {
                                validEffects.Add(ie1);
                            }
                        }
                    }

                    validEffects.Shuffle();
                    ret.Add(validEffects[0]);
                }
            }


            return ret;
        }
        #endregion

        // Character Logic
        #region
        public void RunItemSetupOnHexCharacterFromItemSet(HexCharacterModel character, ItemSet itemSet)
        {
            Debug.Log("ItemController.RunItemSetupOnCharacterEntityFromItemManagerData() called on character: " + character.myName);

            character.itemSet = new ItemSet();
            CopyItemManagerDataIntoOtherItemManager(itemSet, character.itemSet);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, character.hexCharacterView.ucm);

        }
        public void CopyItemManagerDataIntoOtherItemManager(ItemSet originalData, ItemSet clone)
        {
            clone.mainHandItem = originalData.mainHandItem;
            clone.offHandItem = originalData.offHandItem;
            clone.bodyArmour = originalData.bodyArmour;
            clone.headArmour = originalData.headArmour;
            clone.trinket = originalData.trinket;
        }
        public void CopySerializedItemManagerIntoStandardItemManager(SerializedItemSet data, ItemSet iManager)
        {
            if (data.mainHandItem != null)            
                iManager.mainHandItem = GenerateNewItemWithRandomEffects(GetItemDataByName(data.mainHandItem.itemName));     
            if (data.offHandItem != null)            
                iManager.offHandItem = GenerateNewItemWithRandomEffects(GetItemDataByName(data.offHandItem.itemName));  
            if (data.chestArmour != null)            
                iManager.bodyArmour = GenerateNewItemWithRandomEffects(GetItemDataByName(data.chestArmour.itemName));            
            if (data.headArmour != null)            
                iManager.headArmour = GenerateNewItemWithRandomEffects(GetItemDataByName(data.headArmour.itemName));
            if (data.trinket != null)
                iManager.trinket = GenerateNewItemWithRandomEffects(GetItemDataByName(data.trinket.itemName));

        }
        public void HandleGiveItemToCharacterFromInventory(HexCharacterData character, InventoryItem newItem, RosterItemSlot slot)
        {
            Debug.LogWarning("ItemController.HandleGiveItemToCharacterFromInventory() called, character = " +
                character.myName + ", item = " + newItem.itemData.itemName + ", slot = " + slot.SlotType.ToString());

            ItemData previousItem = slot.ItemDataRef;

            // remove new item from inventory
            int index = InventoryController.Instance.Inventory.IndexOf(newItem);
            InventoryController.Instance.RemoveItemFromInventory(newItem);

            if (previousItem != null)
            {
                Debug.LogWarning("Item " + previousItem.itemName + " already in slot: " + slot.SlotType.ToString() + ", returning it to inventory...");
                InventoryController.Instance.AddItemToInventory(new InventoryItem(previousItem), false, index);
            }

            // Main hand
            if (slot.SlotType == RosterSlotType.MainHand)            
                character.itemSet.mainHandItem = newItem.itemData;            

            // Off hand
            else if (slot.SlotType == RosterSlotType.OffHand)            
                character.itemSet.offHandItem = newItem.itemData;            

            // Trinket
            else if (slot.SlotType == RosterSlotType.Trinket)            
                character.itemSet.trinket = newItem.itemData;

            // Head
            else if (slot.SlotType == RosterSlotType.Head)
                character.itemSet.headArmour = newItem.itemData;

            // Trinket
            else if (slot.SlotType == RosterSlotType.Body)
                character.itemSet.bodyArmour = newItem.itemData;

        }
        #endregion

        // Item Effects Logic
        #region
        public int GetTotalAttributeBonusFromItemSet(ItemCoreAttribute attribute, ItemSet set)
        {
            if (set == null) return 0;
            int ret = 0;
            ret += GetTotalAttributeBonusFromItemData(attribute, set.mainHandItem);
            ret += GetTotalAttributeBonusFromItemData(attribute, set.offHandItem);
            ret += GetTotalAttributeBonusFromItemData(attribute, set.trinket);
            return ret;
        }   
        public int GetTotalArmourBonusFromItemSet(ItemSet set)
        {
            int ret = 0;
            if (set.mainHandItem != null) ret += set.mainHandItem.armourAmount;
            if (set.offHandItem != null) ret += set.offHandItem.armourAmount;
            if (set.trinket != null) ret += set.trinket.armourAmount;
            if (set.headArmour != null) ret += set.headArmour.armourAmount;
            if (set.bodyArmour != null) ret += set.bodyArmour.armourAmount;

            Debug.Log("ItemController.GetTotalArmourBonusFromItemSet() returning: " + ret.ToString());
            return ret;
        }
        private int GetTotalAttributeBonusFromItemData(ItemCoreAttribute attribute, ItemData item)
        {
            int ret = 0;
            if (item == null) return 0;

            for(int i = 0; i < item.itemEffects.Count; i++)
            {
                if(item.itemEffects[i].effectType == ItemEffectType.ModifyAttribute &&
                    item.itemEffects[i].attributeModified == attribute)
                {
                    ret += item.itemEffects[i].modAmount;
                }
            }

            return ret;
        }
        public int GetTotalStacksOfPerkFromItemSet(Perk perk, ItemSet set)
        {
            if (set == null) return 0;
            int ret = 0;
            ret += GetTotalStacksOfPerkFromItem(perk, set.mainHandItem);
            ret += GetTotalStacksOfPerkFromItem(perk, set.offHandItem);
            ret += GetTotalStacksOfPerkFromItem(perk, set.trinket);
            return ret;
        }
        private int GetTotalStacksOfPerkFromItem(Perk perk, ItemData item)
        {
            int ret = 0;
            if (item == null) return 0;
            for (int i = 0; i < item.itemEffects.Count; i++)
            {
                if (item.itemEffects[i].effectType == ItemEffectType.GainPerk &&
                    item.itemEffects[i].perkGained.perkTag == perk)
                {
                    ret += item.itemEffects[i].perkGained.stacks;
                }
            }
            return ret;
        }
        public List<ActivePerk> GetActivePerksFromItemSet(ItemSet set)
        {
            List<ActivePerk> perks = new List<ActivePerk>();

            perks.AddRange(GetActivePerksFromItem(set.mainHandItem));
            perks.AddRange(GetActivePerksFromItem(set.offHandItem));
            perks.AddRange(GetActivePerksFromItem(set.trinket));

            Debug.Log("ItemController.GetActivePerksFromItemSet() found " + perks.Count.ToString() +
                " perks from items");
            return perks;
        }
        private List<ActivePerk> GetActivePerksFromItem(ItemData item)
        {
            Debug.Log("ItemController.GetActivePerksFromItem() called");

            List<ActivePerk> perks = new List<ActivePerk>();
            if (item == null) return perks;

            foreach (ItemEffect i in item.itemEffects)
            {
                if(i.effectType == ItemEffectType.GainPerk)
                {
                    Debug.Log("Found a perk effect on item!");
                    ActivePerk oldPerk = null;
                    bool createNew = true;
                    foreach(ActivePerk ap in perks)
                    {
                        if(ap.perkTag == i.perkGained.perkTag)
                        {
                            oldPerk = ap;
                            createNew = false;
                            break;
                        }
                    }

                    if (createNew) perks.Add(new ActivePerk(i.perkGained.perkTag, i.perkGained.stacks));
                    else oldPerk.stacks += i.perkGained.stacks;
                }
            }

            return perks;
        }
        #endregion

    }
}
