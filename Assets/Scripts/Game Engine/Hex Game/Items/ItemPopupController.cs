﻿using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Perks;
using WeAreGladiators.RewardSystems;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Items
{
    public class ItemPopupController : Singleton<ItemPopupController>
    {
        #region Properties + Components

        [Header("Core Panel Components")]
        [SerializeField] private GameObject visualParent;
        [SerializeField] private RectTransform[] transformsRebuilt;
        [SerializeField] private CanvasGroup mainCg;

        [Header("Name + Icon Row Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image itemImage;

        [Header("Type + Rarity Row Components")]
        [SerializeField] private GameObject rarityRowParent;
        [SerializeField] private TextMeshProUGUI itemTypeText;
        [SerializeField] private TextMeshProUGUI rarityText;
        [SerializeField] private TextMeshProUGUI goldValueText;

        [Header("Damage Modifier Row Components")]
        [SerializeField] private GameObject damageModParent;
        [SerializeField] private TextMeshProUGUI armourDamageText;
        [SerializeField] private TextMeshProUGUI healthDamageText;
        [SerializeField] private TextMeshProUGUI penetrationText;

        [Header("Granted Ability Row Components")]
        [SerializeField] private GameObject abilitiesParent;
        [SerializeField] private UiAbilityIconRow[] abilityRows;

        [Header("Effects Row Components")]
        [SerializeField] private GameObject effectsParent;
        [SerializeField] private ModalDottedRow[] effectRows;

        [Header("Armour Components")]
        [SerializeField] private GameObject armourParent;
        [SerializeField] private TextMeshProUGUI armourText;

        [Header("Fatigue Penalty Components")]
        [SerializeField] private GameObject fatiguePenaltyParent;
        [SerializeField] private TextMeshProUGUI fatiguePenaltyText;

        #endregion

        #region Input

        public void OnCombatItemLootIconMousedOver(CombatLootIcon icon)
        {
            FadeInPanel();
            BuildPanelFromItemData(icon.ItemReward);
            PlacePanelRightOfTransform(icon.transform);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelRightOfTransform(icon.transform);
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
            BuildPanelFromItemData(item.MyItemData.Item);
            PlacePanelAtShopItemSlotPosition(item);
            TransformUtils.RebuildLayouts(transformsRebuilt);
            PlacePanelAtShopItemSlotPosition(item);
        }
        public void OnInventoryItemMouseExit()
        {
            HidePanel();
        }

        #endregion

        #region Show + Hide Panel Logic

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

        #region Place + Position Panel

        private void PlacePanelRightOfTransform(Transform t)
        {
            RectTransform rect = visualParent.GetComponent<RectTransform>();
            float xOffset = rect.rect.width / 2 + 80;
            float yOffset = 100;

            rect.position = t.transform.position;
            rect.localPosition = new Vector3(rect.localPosition.x + xOffset, rect.localPosition.y + yOffset, 0);
        }

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
            float yOffset = 100;

            rect.position = view.transform.position;
            rect.localPosition = new Vector3(rect.localPosition.x + xOffset, rect.localPosition.y + yOffset, 0);
        }
        private void PlacePanelAtShopItemSlotPosition(ItemShopSlot view)
        {
            RectTransform rect = visualParent.GetComponent<RectTransform>();
            float xOffset = rect.rect.width / 2 + 80;
            float yOffset = 100;

            rect.position = view.transform.position;
            rect.localPosition = new Vector3(rect.localPosition.x + xOffset, rect.localPosition.y + yOffset, 0);
        }
        private void PlacePanelAtRosterItemSlotPosition(RosterItemSlot view)
        {
            RectTransform rect = visualParent.GetComponent<RectTransform>();
            float xOffset = rect.rect.width / 2 + 80;
            float yOffset = rect.rect.height * 0.5f;//25; //25 + rect.rect.height;

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

        #region Build + Place Panel

        private void BuildPanelFromItemData(ItemData item)
        {
            // Top row
            nameText.text = item.itemName;
            descriptionText.text = item.itemDescription;
            itemImage.sprite = item.ItemSprite;

            // Row 2
            itemTypeText.text = GetItemTypeString(item);
            rarityRowParent.gameObject.SetActive(false);
            if (item.itemType == ItemType.Trinket)
            {
                rarityRowParent.gameObject.SetActive(true);
                rarityText.text = item.rarity.ToString();
            }
            goldValueText.text = item.baseGoldValue.ToString();

            // Row 3
            BuildArmourSection(item);

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
                else
                {
                    ret = TextLogic.SplitByCapitals(item.handRequirement.ToString());
                }
                ret += " " + item.weaponClass;
            }
            else if (item.itemType == ItemType.Trinket)
            {
                ret = "Trinket";
            }
            else if (item.itemType == ItemType.Head)
            {
                ret = "Head Armour";
            }
            else if (item.itemType == ItemType.Body)
            {
                ret = "Chest Armour";
            }

            return ret;
        }
        private void BuildGrantedAbilitiesSection(ItemData item)
        {
            abilitiesParent.SetActive(false);
            for (int i = 0; i < abilityRows.Length; i++)
            {
                abilityRows[i].Hide();
            }

            if (item.grantedAbilities.Count > 0)
            {
                abilitiesParent.SetActive(true);
                for (int i = 0; i < item.grantedAbilities.Count; i++)
                {
                    abilityRows[i].Build(item.grantedAbilities[i]);
                }
            }
        }

        private void BuildDamageModRows(ItemData item)
        {
            damageModParent.SetActive(false);

            if (item.IsMeleeWeapon || item.IsRangedWeapon)
            {
                damageModParent.SetActive(true);
                healthDamageText.text = Math.Round(item.healthDamage * 100f) + "%";
                armourDamageText.text = Math.Round(item.armourDamage * 100f) + "%";
                penetrationText.text = Math.Round(item.armourPenetration * 100f) + "%";
            }
        }
        private void BuildArmourSection(ItemData item)
        {
            armourParent.SetActive(false);
            if (item.armourAmount != 0)
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
            {
                effectRows[i].gameObject.SetActive(false);
            }

            int iBoost = 0;
            if (item.itemEffects.Count > 0)
            {
                effectsParent.SetActive(true);
                for (int i = 0; i < item.itemEffects.Count; i++)
                {
                    ModalDottedRow row = effectRows[i + iBoost];
                    ItemEffect effect = item.itemEffects[i];
                    DotStyle dotStyle = DotStyle.Neutral;

                    if (effect.effectType == ItemEffectType.ModifyAttribute)
                    {
                        dotStyle = DotStyle.Green;
                        string attributeText = TextLogic.SplitByCapitals(effect.attributeModified.ToString());
                        string modAmountString = "";
                        if (effect.modAmount >= 0)
                        {
                            modAmountString += "+";
                        }
                        else
                        {
                            dotStyle = DotStyle.Red;
                        }
                        modAmountString += effect.modAmount.ToString();
                        if (effect.attributeModified == ItemCoreAttribute.WeaponDamageBonus)
                        {
                            modAmountString += "%";
                            attributeText = TextLogic.ReturnColoredText("Weapon Ability", TextLogic.neutralYellow) + " damage";
                        }
                        else
                        {
                            attributeText = TextLogic.ReturnColoredText(attributeText, TextLogic.neutralYellow);
                        }

                        row.Build(TextLogic.ReturnColoredText(modAmountString + " ", TextLogic.blueNumber) + attributeText, dotStyle);

                    }

                    else if (effect.effectType == ItemEffectType.OnHitEffect)
                    {
                        string zero = TextLogic.ReturnColoredText(effect.perkApplied.passiveStacks.ToString(), TextLogic.blueNumber);
                        string one = TextLogic.ReturnColoredText(TextLogic.SplitByCapitals(effect.perkApplied.perkTag.ToString()), TextLogic.neutralYellow);
                        string two = effect.effectChance.ToString();

                        row.Build(string.Format("On hit: Apply {0} {1} ({2}% chance).", zero, one, two), DotStyle.Green);

                    }
                    else if (item.itemEffects[i].effectType == ItemEffectType.GainPerk)
                    {
                        PerkIconData pd = effect.perkGained.Data;
                        string s = TextLogic.ConvertCustomStringListToString(pd.passiveDescription);
                        s = s.Replace("X", effect.perkGained.stacks.ToString());
                        row.Build(s, dotStyle);
                    }

                    else if (item.itemEffects[i].effectType == ItemEffectType.GainPerkTurnStart)
                    {
                        row.Build("On turn start, " + TextLogic.ReturnColoredText(effect.gainPerkChance + "%", TextLogic.blueNumber) +
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
                        if (item.itemEffects[i].innateItemEffectType == InnateItemEffectType.InnateAccuracyModifier)
                        {
                            // +X Hit Chance with attacks using this weapon.
                            dotStyle = DotStyle.Green;
                            string zero = "+";
                            string one = TextLogic.ReturnColoredText(effect.innateAccuracyMod.ToString(), TextLogic.blueNumber);
                            string two = TextLogic.ReturnColoredText("Accuracy", TextLogic.neutralYellow);

                            if (effect.innateAccuracyMod < 0)
                            {
                                dotStyle = DotStyle.Red;
                                zero = "";
                            }
                            row.Build(string.Format("{0}{1} {2} with attacks using this weapon.", zero, one, two), dotStyle);
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
                            row.Build(string.Format("{0}{1} {2} with attacks using this weapon against adjacent targets.", zero, one, two), dotStyle);
                        }
                        else if (item.itemEffects[i].innateItemEffectType == InnateItemEffectType.InnatePerkGainedOnUse)
                        {
                            // whenever this weapon is used, apply X Y to self.
                            dotStyle = DotStyle.Green;
                            string zero = TextLogic.ReturnColoredText(effect.innatePerkGainedOnUse.stacks.ToString(), TextLogic.blueNumber);
                            string one = TextLogic.ReturnColoredText(TextLogic.SplitByCapitals(effect.innatePerkGainedOnUse.perkTag.ToString()), TextLogic.neutralYellow);

                            row.Build(string.Format("Whenever you attack with this weapon, apply {0} {1} to self.", zero, one), DotStyle.Red);
                        }
                        else if (item.itemEffects[i].innateItemEffectType == InnateItemEffectType.BonusMeleeRange)
                        {
                            dotStyle = DotStyle.Red;
                            string symbol = "";
                            if (effect.innateWeaponRangeBonus > 0)
                            {
                                dotStyle = DotStyle.Green;
                                symbol = "+";
                            }
                            string zero = TextLogic.ReturnColoredText(symbol + effect.innateWeaponRangeBonus, TextLogic.blueNumber);
                            row.Build(string.Format("{0} range with attacks using this weapon.", zero), dotStyle);

                        }
                        else if (item.itemEffects[i].innateItemEffectType == InnateItemEffectType.PenetrationBonusOnBackstab)
                        {
                            // bonus penetration on backstab
                            dotStyle = DotStyle.Green;
                            string zero = TextLogic.ReturnColoredText("+" + effect.innateBackstabPenetrationBonus + "%", TextLogic.blueNumber);
                            string one = TextLogic.ReturnColoredText("Penetration", TextLogic.neutralYellow);
                            string two = TextLogic.ReturnColoredText("Backstabbing", TextLogic.neutralYellow);
                            row.Build(string.Format("{0} {1} when {2} with this weapon.", zero, one, two), dotStyle);

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
                fatiguePenaltyText.text = "-" + item.fatiguePenalty;
            }
        }

        #endregion
    }
}
