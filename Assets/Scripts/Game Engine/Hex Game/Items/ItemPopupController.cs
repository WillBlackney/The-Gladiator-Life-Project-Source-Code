using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using WeAreGladiators.Abilities;
using WeAreGladiators.UI;
using WeAreGladiators.Perks;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.RewardSystems;
using System;

namespace WeAreGladiators.Items
{
    public class ItemPopupController : Singleton<ItemPopupController>
    {
        // Properties + Components
        #region
        [Header("Core Panel Components")]
        [SerializeField] GameObject visualParent;
        [SerializeField] RectTransform[] transformsRebuilt;
        [SerializeField] CanvasGroup mainCg;

        [Header("Name + Icon Row Components")]
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] Image itemImage;

        [Header("Type + Rarity Row Components")]
        [SerializeField] TextMeshProUGUI itemTypeText;
        [SerializeField] TextMeshProUGUI rarityText;
        [SerializeField] TextMeshProUGUI goldValueText;

        [Header("Damage Modifier Row Components")]
        [SerializeField] GameObject damageModParent;
        [SerializeField] TextMeshProUGUI armourDamageText;
        [SerializeField] TextMeshProUGUI healthDamageText;
        [SerializeField] TextMeshProUGUI penetrationText;

        [Header("Granted Ability Row Components")]
        [SerializeField] GameObject abilitiesParent;
        [SerializeField] UiAbilityIconRow[] abilityRows;

        [Header("Effects Row Components")]
        [SerializeField] GameObject effectsParent;
        [SerializeField] ModalDottedRow[] effectRows;

        [Header("Armour Components")]
        [SerializeField] GameObject armourParent;
        [SerializeField] TextMeshProUGUI armourText;

        [Header("Fatigue Penalty Components")]
        [SerializeField] GameObject fatiguePenaltyParent;
        [SerializeField] TextMeshProUGUI fatiguePenaltyText;
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
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelAboveTransform(icon.transform);
        }
        public void OnCombatContractItemIconMousedOver(CombatContractCard card)
        {
            FadeInPanel();
            BuildPanelFromItemData(card.MyContractData.combatRewardData.item);
            PlacePanelAboveTransform(card.ItemImage.transform);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelAboveTransform(card.ItemImage.transform);
        }
        public void OnInventoryItemMousedOver(InventoryItemView item)
        {
            FadeInPanel();
            BuildPanelFromItemData(item.MyItemRef.itemData);
            PlacePanelAtInventoryItemPosition(item);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelAtInventoryItemPosition(item);
        }
        public void OnRosterItemSlotMousedOver(RosterItemSlot item)
        {
            FadeInPanel();
            BuildPanelFromItemData(item.ItemDataRef);
            PlacePanelAtRosterItemSlotPosition(item);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelAtRosterItemSlotPosition(item);
        }
        public void OnCustomCharacterItemSlotMousedOver(CustomItemIcon item)
        {
            FadeInPanel();
            BuildPanelFromItemData(item.ItemDataRef);
            PlacePanelAtCustomCharacterScreenItemSlotPosition(item);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelAtCustomCharacterScreenItemSlotPosition(item);
        }
        public void OnShopItemMousedOver(ItemShopSlot item)
        {
            FadeInPanel();
            BuildPanelFromItemData(item.MyData.Item);
            PlacePanelAtShopItemSlotPosition(item);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelAtShopItemSlotPosition(item);
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
            goldValueText.text = item.baseGoldValue.ToString();

            // Row 3
            BuildArmourSection(item);
            BuildMaxFatigueSection(item);

            // Row 4
            BuildDamageModRows(item);

            // Row 5
            BuildGrantedAbilitiesSection(item);

            // Row 6
            BuildGrantedEffectsSection(item);

          
        }
        private string GetItemTypeString(ItemData item)
        {
            string ret = "";

            // If weapon
            if (item.itemType == ItemType.Weapon)
            {
                if (item.weaponClass == WeaponClass.ThrowingNet &&
                    item.weaponClass == WeaponClass.Shield &&
                    item.weaponClass == WeaponClass.Holdable)
                {
                    ret = "Offhand";
                }
                else ret = TextLogic.SplitByCapitals(item.handRequirement.ToString());
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

        private void BuildDamageModRows(ItemData item)
        {
            damageModParent.SetActive(false);

            if(item.IsMeleeWeapon || item.IsRangedWeapon)
            {
                damageModParent.SetActive(true);
                healthDamageText.text = Math.Round(item.healthDamage * 100f).ToString() +"%";
                armourDamageText.text = Math.Round(item.armourDamage * 100f).ToString() + "%";
                penetrationText.text = Math.Round(item.armourPenetration * 100f).ToString() + "%";
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
            // Reset
            effectsParent.SetActive(false);
            for (int i = 0; i < effectRows.Length; i++)
                effectRows[i].gameObject.SetActive(false);

            int iBoost = 0;
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
                        string zero = TextLogic.ReturnColoredText(effect.perkApplied.passiveStacks.ToString(), TextLogic.blueNumber);
                        string one = TextLogic.ReturnColoredText(TextLogic.SplitByCapitals(effect.perkApplied.perkTag.ToString()), TextLogic.neutralYellow);
                        string two = effect.effectChance.ToString();

                        row.Build(String.Format("On hit: Apply {0} {1} ({2}% chance).", zero, one, two), DotStyle.Green);

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
                    else if (item.itemEffects[i].effectType == ItemEffectType.InnateWeaponEffect)
                    {
                        if(item.itemEffects[i].innateItemEffectType == InnateItemEffectType.InnateAccuracyModifier)
                        {
                            // +X Hit Chance with attacks using this weapon.
                            dotStyle = DotStyle.Green;
                            string zero = "+";
                            string one = TextLogic.ReturnColoredText(effect.innateAccuracyMod.ToString(), TextLogic.blueNumber);
                            string two = TextLogic.ReturnColoredText("Accuracy", TextLogic.neutralYellow);
                            
                            if(effect.innateAccuracyMod < 0)
                            {
                                dotStyle = DotStyle.Red;
                                zero = "";
                            }
                            row.Build(String.Format("{0}{1} {2} with attacks using this weapon.", zero, one, two), dotStyle);
                        }
                        else if (item.itemEffects[i].innateItemEffectType == InnateItemEffectType.InnateAccuracyAgainstAdjacentModifier)
                        {
                            // +X or -X Hit Chance with attacks against adjacent targets using this weapon.
                            dotStyle = DotStyle.Green;
                            string zero = "+";
                            string one = TextLogic.ReturnColoredText(effect.innateAccuracyAgainstAdjacentMod.ToString(), TextLogic.blueNumber);
                            string two = TextLogic.ReturnColoredText("Accuracy", TextLogic.neutralYellow);

                            if (effect.innateAccuracyAgainstAdjacentMod < 0)
                            {
                                dotStyle = DotStyle.Red;
                                zero = "";
                            }
                            row.Build(String.Format("{0}{1} {2} with attacks using this weapon against adjacent targets.", zero, one, two), dotStyle);
                        }
                        else if (item.itemEffects[i].innateItemEffectType == InnateItemEffectType.InnatePerkGainedOnUse)
                        {
                            // whenever this weapon is used, apply X Y to self.
                            dotStyle = DotStyle.Green;
                            string zero = TextLogic.ReturnColoredText(effect.innatePerkGainedOnUse.stacks.ToString(), TextLogic.blueNumber);
                            string one = TextLogic.ReturnColoredText(TextLogic.SplitByCapitals(effect.innatePerkGainedOnUse.perkTag.ToString()), TextLogic.neutralYellow);

                            row.Build(String.Format("Whenever you attack with this weapon, apply {0} {1} to self.", zero, one), DotStyle.Red);
                        }
                        else if (item.itemEffects[i].innateItemEffectType == InnateItemEffectType.BonusMeleeRange)
                        {
                            dotStyle = DotStyle.Red;
                            string symbol = "";
                            if(effect.innateWeaponRangeBonus > 0)
                            {
                                dotStyle = DotStyle.Green;
                                symbol = "+";
                            }
                            string zero = TextLogic.ReturnColoredText(symbol + effect.innateWeaponRangeBonus.ToString(), TextLogic.blueNumber);
                            row.Build(String.Format("{0} range with attacks using this weapon.", zero), dotStyle);

                        }
                        else if (item.itemEffects[i].innateItemEffectType == InnateItemEffectType.PenetrationBonusOnBackstab)
                        {
                            // bonus penetration on backstab
                            dotStyle = DotStyle.Green;
                            string zero = TextLogic.ReturnColoredText("+" + effect.innateBackstabPenetrationBonus.ToString() +"%", TextLogic.blueNumber);
                            string one = TextLogic.ReturnColoredText("Penetration", TextLogic.neutralYellow);
                            string two = TextLogic.ReturnColoredText("Backstabbing" ,TextLogic.neutralYellow);
                            row.Build(String.Format("{0} {1} when {2} with this weapon.", zero, one, two), dotStyle);

                        }
                    }
                }
            }
        }
        private void BuildMaxFatigueSection(ItemData item)
        {
            fatiguePenaltyParent.SetActive(false);

            if (item.fatiguePenalty > 0)
            {
                fatiguePenaltyParent.SetActive(true);
                fatiguePenaltyText.text = "-" + item.fatiguePenalty.ToString();
            }
        }
        #endregion

    }
}