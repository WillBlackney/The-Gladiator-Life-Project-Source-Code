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
        [SerializeField] UiAbilityIconRow[] abilityRows;

        [Header("Row 4 Components")]
        [SerializeField] GameObject effectsParent;
        [SerializeField] ModalDottedRow[] effectRows;

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
        public void OnCustomCharacterItemSlotMousedOver(CustomItemIcon item)
        {
            FadeInPanel();
            BuildPanelFromItemData(item.ItemDataRef);
            PlacePanelAtCustomCharacterScreenItemSlotPosition(item);
            ForceRebuildLayouts();
        }
        public void OnShopItemMousedOver(ItemShopSlot item)
        {
            FadeInPanel();
            BuildPanelFromItemData(item.MyData.Item);
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

        // Place + Position Panel
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
            rect.localPosition = new Vector3(rect.localPosition.x + xOffset, rect.localPosition.y + yOffset, 0);
        }
        private void PlacePanelAtRosterItemSlotPosition(RosterItemSlot view)
        {
            RectTransform rect = visualParent.GetComponent<RectTransform>();
            float xOffset = rect.rect.width / 2 + 40;
            float yOffset = 25 + rect.rect.height;

            rect.position = view.transform.position;
            rect.localPosition = new Vector3(rect.localPosition.x + xOffset, rect.localPosition.y + yOffset, 0);
        }
        private void PlacePanelAtCustomCharacterScreenItemSlotPosition(CustomItemIcon view)
        {
            RectTransform rect = visualParent.GetComponent<RectTransform>();
            float xOffset = rect.rect.width / 2 + 80;
            float yOffset = 0;

            rect.position = view.transform.position;
            rect.localPosition = new Vector3(rect.localPosition.x - xOffset, rect.localPosition.y - yOffset, 0);
        }
        #endregion

        // Build + Place Panel
        #region

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
            for(int i = 0; i < abilityRows.Length; i++)            
                abilityRows[i].Hide();
            
            if(item.grantedAbilities.Count > 0)
            {
                abilitiesParent.SetActive(true);
                for(int i = 0; i < item.grantedAbilities.Count; i++)
                {
                    abilityRows[i].Build(item.grantedAbilities[i]);
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

            for (int i = 0; i < effectRows.Length; i++)
                effectRows[i].gameObject.SetActive(false);

            int iBoost = 0;
            if(item.fatiguePenalty > 0)
            {
                iBoost += 1;
                effectsParent.SetActive(true);
                ModalDottedRow row = effectRows[iBoost - 1];
                row.Build(TextLogic.ReturnColoredText("-" + item.fatiguePenalty.ToString(), TextLogic.blueNumber) + " Fatigue", DotStyle.Red);
            }

            if (item.weaponClass == WeaponClass.Crossbow)
            {
                iBoost += 1;
                effectsParent.SetActive(true);
                ModalDottedRow row = effectRows[iBoost - 1];
                row.Build("Can only be fired once per turn.", DotStyle.Red);
            }

            if (item.itemEffects.Count > 0)
            {
                effectsParent.SetActive(true);
                for(int i = 0; i < item.itemEffects.Count;i++)
                {
                    ModalDottedRow row = effectRows[i + iBoost];
                    ItemEffect effect = item.itemEffects[i];
                    DotStyle dotStyle = DotStyle.Neutral;

                    if (effect.effectType == ItemEffectType.ModifyAttribute)
                    {
                        dotStyle = DotStyle.Green;
                        string attributeText = TextLogic.SplitByCapitals(effect.attributeModified.ToString());
                        string modAmountString = "";
                        if (effect.modAmount >= 0) modAmountString += "+";
                        else dotStyle = DotStyle.Red;
                        modAmountString += effect.modAmount.ToString();
                        if (effect.attributeModified == ItemCoreAttribute.WeaponDamageBonus)
                        {
                            modAmountString += "%";
                            attributeText = TextLogic.ReturnColoredText("Weapon Ability", TextLogic.neutralYellow) + " damage";
                        }
                        else attributeText = TextLogic.ReturnColoredText(attributeText, TextLogic.neutralYellow);

                        row.Build(TextLogic.ReturnColoredText(modAmountString + " ", TextLogic.blueNumber) + attributeText, dotStyle);

                    }

                    else if (effect.effectType == ItemEffectType.OnHitEffect)
                    {
                        row.Build("On hit: Apply " + TextLogic.ReturnColoredText(effect.perkApplied.passiveStacks.ToString(), TextLogic.blueNumber) + " " +
                            TextLogic.ReturnColoredText(TextLogic.SplitByCapitals(effect.perkApplied.perkTag.ToString()), TextLogic.neutralYellow) + " (" +
                            effect.effectChance.ToString() + "% chance).", DotStyle.Green);
                    }
                    else if (item.itemEffects[i].effectType == ItemEffectType.GainPerk)
                    {
                        var pd = effect.perkGained.Data;
                        string s = TextLogic.ConvertCustomStringListToString(pd.passiveDescription);
                        s = s.Replace("X", effect.perkGained.stacks.ToString());
                        row.Build(s, dotStyle);
                    }

                    else if (item.itemEffects[i].effectType == ItemEffectType.GainPerkTurnStart)
                    {
                        row.Build("On turn start, " + TextLogic.ReturnColoredText(effect.gainPerkChance.ToString() + "%", TextLogic.blueNumber) + 
                           " chance to gain " + TextLogic.ReturnColoredText(effect.perkGained.stacks.ToString(), TextLogic.blueNumber) + " " +
                           TextLogic.ReturnColoredText(TextLogic.SplitByCapitals(effect.perkGained.perkTag.ToString()), TextLogic.neutralYellow), DotStyle.Green);
                    }
                    else if (item.itemEffects[i].effectType == ItemEffectType.GainPerkCombatStart)
                    {
                        row.Build("On combat start, gain " + TextLogic.ReturnColoredText(effect.perkGained.stacks.ToString(), TextLogic.blueNumber) + " " +
                           TextLogic.ReturnColoredText(TextLogic.SplitByCapitals(effect.perkGained.perkTag.ToString()), TextLogic.neutralYellow) + ".", DotStyle.Green);
                    }
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