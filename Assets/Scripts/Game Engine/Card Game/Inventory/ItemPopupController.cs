using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using HexGameEngine.Abilities;
using HexGameEngine.UI;
using HexGameEngine.Perks;
using HexGameEngine.TownFeatures;
using HexGameEngine.RewardSystems;

namespace HexGameEngine.Items
{
    public class ItemPopupController : Singleton<ItemPopupController>
    {
        // Properties + Components
        #region
        [Header("Core Panel Components")]
        [SerializeField] GameObject visualParent;
        [SerializeField] RectTransform[] transformsRebuilt;
        [SerializeField] CanvasGroup mainCg;

        [Header("Row 1 Components")]
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] Image itemImage;

        [Header("Row 2 Components")]
        [SerializeField] TextMeshProUGUI itemTypeText;
        [SerializeField] TextMeshProUGUI rarityText;

        [Header("Row 3 Components")]
        [SerializeField] GameObject abilitiesParent;
        [SerializeField] TextMeshProUGUI abilitiesText;

        [Header("Row 4 Components")]
        [SerializeField] GameObject effectsParent;
        [SerializeField] TextMeshProUGUI effectsText;

        [Header("Armour Components")]
        [SerializeField] GameObject armourParent;
        [SerializeField] TextMeshProUGUI armourText;
        #endregion

        // Getters + Accessors
        #region

        #endregion

        // Input
        #region
        public void OnCombatItemLootIconMousedOver(CombatLootIcon icon)
        {
            FadeInPanel();
            BuildPanelFromItemData(icon.ItemReward);
            PlacePanelAboveTransform(icon.transform);
            ForceRebuildLayouts();
        }
        public void OnCombatContractItemIconMousedOver(CombatContractCard card)
        {
            FadeInPanel();
            BuildPanelFromItemData(card.MyContractData.combatRewardData.item);
            PlacePanelAboveTransform(card.ItemImage.transform);
            ForceRebuildLayouts();
        }
        public void OnInventoryItemMousedOver(InventoryItemView item)
        {
            FadeInPanel();
            BuildPanelFromItemData(item.MyItemRef.itemData);
            PlacePanelAtInventoryItemPosition(item);
            ForceRebuildLayouts();
        }
        public void OnRosterItemSlotMousedOver(RosterItemSlot item)
        {
            FadeInPanel();
            BuildPanelFromItemData(item.ItemDataRef);
            PlacePanelAtRosterItemSlotPosition(item);
            ForceRebuildLayouts();
        }
        public void OnShopItemMousedOver(ItemShopSlot item)
        {
            FadeInPanel();
            BuildPanelFromItemData(item.MyData.item);
            PlacePanelAtShopItemSlotPosition(item);
            ForceRebuildLayouts();
        }
        public void OnInventoryItemMouseExit()
        {
            HidePanel(); 
        }
        #endregion

        // Show + Hide Panel Logic
        #region
        private void FadeInPanel()
        {
            visualParent.transform.DOKill();
            mainCg.DOKill();
            visualParent.SetActive(true);
            mainCg.alpha = 0.0001f;
            mainCg.DOFade(1, 0.25f);

        }
        public void HidePanel()
        {
            visualParent.transform.DOKill();
            mainCg.DOKill();
            mainCg.alpha = 0f;
            visualParent.SetActive(false);
        }
        #endregion

        // Build + Place Panel
        #region
        private void PlacePanelAboveTransform(Transform t)
        {
            RectTransform rect = visualParent.GetComponent<RectTransform>();
            float yOffset = 50 + rect.rect.height;

            rect.position = t.position;
            rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y + yOffset, 0);
        }
        private void PlacePanelAtInventoryItemPosition(InventoryItemView view)
        {
            RectTransform rect = visualParent.GetComponent<RectTransform>();
            float xOffset = rect.rect.width / 2 + 80;
            float yOffset = 0;
           
            rect.position = view.transform.position;
            rect.localPosition = new Vector3(rect.localPosition.x - xOffset, rect.localPosition.y - yOffset, 0);
        }
        private void PlacePanelAtShopItemSlotPosition(ItemShopSlot view)
        {
            RectTransform rect = visualParent.GetComponent<RectTransform>();
            float xOffset = rect.rect.width / 2 + 80;
            float yOffset = rect.rect.height / 2;

            rect.position = view.transform.position;
            rect.localPosition = new Vector3(rect.localPosition.x - xOffset, rect.localPosition.y + yOffset, 0);
        }
        private void PlacePanelAtRosterItemSlotPosition(RosterItemSlot view)
        {
            RectTransform rect = visualParent.GetComponent<RectTransform>();
            float xOffset = rect.rect.width / 2 + 80;
            float yOffset = 0;

            rect.position = view.transform.position;
            rect.localPosition = new Vector3(rect.localPosition.x + xOffset, rect.localPosition.y - yOffset, 0);
        }
        private void BuildPanelFromItemData(ItemData item)
        {
            // Top row
            nameText.text = item.itemName;
            descriptionText.text = item.itemDescription;
            itemImage.sprite = item.ItemSprite;

            // Row 2
            itemTypeText.text = GetItemTypeString(item);
            rarityText.text = item.rarity.ToString();

            // Row 3
            BuildGrantedAbilitiesSection(item);

            // Row 4
            BuildGrantedEffectsSection(item);

            // Armour Section
            BuildArmourSection(item);
        }
        private string GetItemTypeString(ItemData item)
        {
            string ret = "";

            // If weapon
            if(item.itemType == ItemType.Weapon)
            {
                ret = TextLogic.SplitByCapitals(item.handRequirement.ToString());
                ret += " " + item.weaponClass.ToString();
            }        
            else if (item.itemType == ItemType.Trinket)
                ret = "Trinket";
            else if (item.itemType == ItemType.Head)
                ret = "Head Armour"; 
            else if (item.itemType == ItemType.Body)
                ret = "Chest Armour";

            return ret;
        }
        private void BuildGrantedAbilitiesSection(ItemData item)
        {
            abilitiesParent.SetActive(false);
            abilitiesText.text = "";

            if(item.grantedAbilities.Count > 0)
            {
                abilitiesParent.SetActive(true);
                for(int i = 0; i < item.grantedAbilities.Count; i++)
                {
                    abilitiesText.text += "- " + item.grantedAbilities[i].abilityName;
                    if (i < item.grantedAbilities.Count - 1)
                        abilitiesText.text += "\n";
                }
               
            }
        }
        private void BuildArmourSection(ItemData item)
        {
            armourParent.SetActive(false);
            if(item.armourAmount != 0)
            {
                armourParent.SetActive(true);
                armourText.text = item.armourAmount.ToString();
            }
        }
        private void BuildGrantedEffectsSection(ItemData item)
        {
            effectsParent.SetActive(false);
            effectsText.text = "";

            // to do: update with better coloured dot point tabs for each effect
            if (item.itemEffects.Count > 0)
            {
                effectsParent.SetActive(true);
                for (int i = 0; i < item.itemEffects.Count; i++)
                {
                    // need to update this when we add item effects that arent stat boosts
                    if(item.itemEffects[i].effectType == ItemEffectType.ModifyAttribute)
                    {
                        string col = TextLogic.blueNumber;
                        string symbol = " +";
                        if (item.itemEffects[i].modAmount < 0)
                        {
                            col = TextLogic.lightRed;
                            symbol = " ";
                        }
                        effectsText.text += "- " + TextLogic.SplitByCapitals(item.itemEffects[i].attributeModified.ToString()) +
                           TextLogic.ReturnColoredText(
                           symbol +
                           item.itemEffects[i].modAmount.ToString(), col);
                    }               
                       
                    else if (item.itemEffects[i].effectType == ItemEffectType.OnHitEffect)
                    {
                        effectsText.text += "- On hit: Apply " + TextLogic.ReturnColoredText(item.itemEffects[i].perkApplied.passiveStacks.ToString(),TextLogic.blueNumber) + " " +
                            TextLogic.ReturnColoredText(TextLogic.SplitByCapitals(item.itemEffects[i].perkApplied.perkTag.ToString()), TextLogic.neutralYellow) + " (" +
                            item.itemEffects[i].effectChance.ToString() + "% chance).";
                    }
                    else if (item.itemEffects[i].effectType == ItemEffectType.GainPerk)
                    {
                        var pd = item.itemEffects[i].perkGained.Data;
                        //var pd = PerkController.Instance.GetPerkIconDataByTag(item.itemEffects[i].perkGained.perkTag);
                        string s = TextLogic.ConvertCustomStringListToString(pd.passiveDescription);
                        s = s.Replace("X", item.itemEffects[i].perkGained.stacks.ToString());
                        effectsText.text += "- " + s;
                    }

                    if (i < item.itemEffects.Count - 1)
                        effectsText.text += "\n";
                }

            }


        }
        #endregion

        // Misc
        #region
        private void ForceRebuildLayouts()
        {
            for (int i = 0; i < 2; i++)
                foreach (RectTransform t in transformsRebuilt)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(t);
        }
        #endregion
    }
}