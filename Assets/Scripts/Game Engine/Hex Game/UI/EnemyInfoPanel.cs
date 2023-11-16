using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Abilities;
using WeAreGladiators.Characters;
using WeAreGladiators.Items;
using WeAreGladiators.Perks;
using WeAreGladiators.UCM;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class EnemyInfoPanel : Singleton<EnemyInfoPanel>
    {
        #region Properties + Components

        [Header("Core Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private Image blackUnderlay;
        [SerializeField] private Transform scalingParent;
        [Space(20)]
        [Header("Left Panel Components")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI characterDescriptionText;
        [SerializeField] private TextMeshProUGUI totalArmourText;
        [SerializeField] private List<UIAbilityIcon> abilityIcons;
        [SerializeField] private Transform uiAbilityIconsParent;
        [SerializeField] private GameObject uiAbilityIconPrefab;
        [SerializeField] private UIPerkIcon[] passiveIcons;
        [SerializeField] private UniversalCharacterModel characterPanelUcm;
        [SerializeField] private GameObject ucmParent;
        [SerializeField] private GameObject nonUcmParent;
        [SerializeField] private ScrollRect rightContentScrollView;
        [SerializeField] private Scrollbar rightContentScrollBar;
        [SerializeField] private RectTransform[] layoutsRebuilt;

        [SerializeField] private RosterItemSlot mainHandSlot;
        [SerializeField] private RosterItemSlot offHandSlot;
        [SerializeField] private RosterItemSlot trinketSlot;
        [SerializeField] private RosterItemSlot headSlot;
        [SerializeField] private RosterItemSlot bodySlot;

        [Header("Core Attribute Components")]
        [SerializeField] private UIAttributeSlider accuracySlider;
        [SerializeField] private UIAttributeSlider dodgeSlider;
        [SerializeField] private UIAttributeSlider constitutionSlider;
        [SerializeField] private UIAttributeSlider mightSlider;
        [SerializeField] private UIAttributeSlider resolveSlider;
        [SerializeField] private UIAttributeSlider witsSlider;
        [Space(20)]
        [Header("Secondary Attribute Text Components")]
        [SerializeField] private TextMeshProUGUI criticalChanceText;
        [SerializeField] private TextMeshProUGUI criticalModifierText;
        [SerializeField] private TextMeshProUGUI initiativeText;
        [SerializeField] private TextMeshProUGUI energyRecoveryText;
        [SerializeField] private TextMeshProUGUI maxEnergyText;
        [SerializeField] private TextMeshProUGUI physicalDamageText;
        [SerializeField] private TextMeshProUGUI magicDamageText;
        [SerializeField] private TextMeshProUGUI visionText;
        [Space(20)]
        [Header("Resistances Text Components")]
        [SerializeField] private TextMeshProUGUI physicalResistanceText;
        [SerializeField] private TextMeshProUGUI magicResistanceText;
        [SerializeField] private TextMeshProUGUI braveryText;
        [SerializeField] private TextMeshProUGUI injuryResistanceText;
        [SerializeField] private TextMeshProUGUI debuffResistanceText;

        private CharacterModel currentModel;

        public bool PanelIsActive => mainVisualParent.activeSelf;

        #endregion

        #region Show, Hide and Build

        public void HandleBuildAndShowPanel(HexCharacterData data)
        {
            if (!mainVisualParent.activeInHierarchy)
            {
                RevealAnimateCharacterRoster();
            }
            mainVisualParent.SetActive(true);
            BuildPanelForEnemy(data);

            AbilityController.Instance.HideHitChancePopup();
            EnemyInfoModalController.Instance.HideModal();
            TransformUtils.RebuildLayouts(layoutsRebuilt);
            rightContentScrollView.verticalNormalizedPosition = 1;
            rightContentScrollBar.value = 1;
        }
        private void BuildPanelForEnemy(HexCharacterData data)
        {
            // Build all sections
            characterNameText.text = data.myName;
            characterDescriptionText.text = data.myDescription;
            totalArmourText.text = (ItemController.Instance.GetTotalArmourBonusFromItemSet(data.itemSet) + data.baseArmour).ToString();

            BuildPerkViews(data);
            BuildAttributeSection(data);
            BuildCharacterViewPanelModel(data);
            BuildAbilitiesSection(data);
            BuildItemSlots(data);
        }

        private void RevealAnimateCharacterRoster()
        {
            blackUnderlay.DOKill();
            blackUnderlay.DOFade(0.5f, 0.5f);

            scalingParent.DOKill();
            if (scalingParent.localScale.x == 1)
            {
                scalingParent.DOScale(0.001f, 0);
            }

            scalingParent.DOScale(1f, 0.25f).SetEase(Ease.OutCubic);
        }
        private void HideAnimateCharacterRoster()
        {
            blackUnderlay.DOKill();
            blackUnderlay.DOFade(0f, 0.25f);

            scalingParent.DOKill();
            scalingParent.DOScale(0.001f, 0.25f).SetEase(Ease.OutCubic).OnComplete(() => mainVisualParent.SetActive(false));
        }

        private void BuildItemSlots(HexCharacterData character)
        {
            // Reset slots
            ResetItemSlot(mainHandSlot);
            ResetItemSlot(offHandSlot);
            ResetItemSlot(trinketSlot);
            ResetItemSlot(bodySlot);
            ResetItemSlot(headSlot);

            BuildItemSlotFromItemData(mainHandSlot, character.itemSet.mainHandItem);
            BuildItemSlotFromItemData(offHandSlot, character.itemSet.offHandItem);
            BuildItemSlotFromItemData(trinketSlot, character.itemSet.trinket);
            BuildItemSlotFromItemData(headSlot, character.itemSet.headArmour);
            BuildItemSlotFromItemData(bodySlot, character.itemSet.bodyArmour);
        }

        private void BuildItemSlotFromItemData(RosterItemSlot slot, ItemData item)
        {
            if (item == null)
            {
                return;
            }
            slot.SetMyDataReference(item);
            slot.ItemImage.sprite = item.ItemSprite;
            slot.ItemViewParent.SetActive(true);
        }

        private void ResetItemSlot(RosterItemSlot slot)
        {
            slot.ItemViewParent.SetActive(false);
            slot.SetMyDataReference(null);
        }
        public void HidePanel()
        {
            HideAnimateCharacterRoster();
        }

        #endregion

        #region Build Sections

        private void BuildPerkViews(HexCharacterData character)
        {
            foreach (UIPerkIcon b in passiveIcons)
            {
                b.HideAndReset();
            }

            // Build Icons
            List<ActivePerk> allPerks = new List<ActivePerk>();

            // Get character perks
            for (int i = 0; i < character.passiveManager.perks.Count; i++)
            {
                allPerks.Add(character.passiveManager.perks[i]);
            }

            // Add perks from items
            allPerks.AddRange(ItemController.Instance.GetActivePerksFromItemSet(character.itemSet));

            // Build perk button for each perk
            for (int i = 0; i < allPerks.Count; i++)
            {
                passiveIcons[i].BuildFromActivePerk(allPerks[i]);
            }

            Debug.Log("Active perk icons = " + allPerks.Count);
        }
        private void BuildAttributeSection(HexCharacterData character)
        {
            accuracySlider.Build(StatCalculator.GetTotalAccuracy(character), character.attributeSheet.accuracy.stars);
            dodgeSlider.Build(StatCalculator.GetTotalDodge(character), character.attributeSheet.dodge.stars);
            constitutionSlider.Build(StatCalculator.GetTotalConstitution(character), character.attributeSheet.constitution.stars);
            mightSlider.Build(StatCalculator.GetTotalMight(character), character.attributeSheet.might.stars);
            resolveSlider.Build(StatCalculator.GetTotalResolve(character), character.attributeSheet.resolve.stars);
            witsSlider.Build(StatCalculator.GetTotalWits(character), character.attributeSheet.wits.stars);

            criticalChanceText.text = StatCalculator.GetTotalCriticalChance(character) + "%";
            criticalModifierText.text = StatCalculator.GetTotalCriticalModifier(character) + "%";
            energyRecoveryText.text = StatCalculator.GetTotalActionPointRecovery(character).ToString();
            maxEnergyText.text = StatCalculator.GetTotalMaxActionPoints(character).ToString();
            initiativeText.text = StatCalculator.GetTotalInitiative(character).ToString();
            visionText.text = StatCalculator.GetTotalVision(character).ToString();
            physicalDamageText.text = StatCalculator.GetTotalPhysicalDamageBonus(character) + "%";
            magicDamageText.text = StatCalculator.GetTotalMagicDamageBonus(character) + "%";

            physicalResistanceText.text = StatCalculator.GetTotalPhysicalResistance(character) + "%";
            magicResistanceText.text = StatCalculator.GetTotalMagicResistance(character) + "%";
            braveryText.text = StatCalculator.GetTotalBravery(character).ToString();
            injuryResistanceText.text = StatCalculator.GetTotalInjuryResistance(character) + "%";
            debuffResistanceText.text = StatCalculator.GetTotalDebuffResistance(character) + "%";
        }
        private void BuildCharacterViewPanelModel(HexCharacterData character)
        {
            ucmParent.SetActive(false);
            nonUcmParent.SetActive(false);
            if (currentModel != null)
            {
                Destroy(currentModel.gameObject);
                currentModel = null;
            }

            if (character.ModelPrefab != null)
            {
                nonUcmParent.SetActive(true);
                CharacterModel newCM = Instantiate(character.ModelPrefab, nonUcmParent.transform);
                newCM.myAnimator.SetTrigger("IDLE");
                currentModel = newCM;
                newCM.myEntityRenderer.SortingOrder = 1032;
                newCM.transform.DOScale(4, 0f);
            }
            else
            {
                ucmParent.SetActive(true);
                CharacterModeller.BuildModelFromStringReferences(characterPanelUcm, character.modelParts);
                CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, characterPanelUcm);
                characterPanelUcm.SetIdleAnim();
            }

        }
        private void BuildAbilitiesSection(HexCharacterData character)
        {
            // reset ability buttons
            foreach (UIAbilityIcon b in abilityIcons)
            {
                b.HideAndReset();
            }

            for (int i = 0; i < character.abilityBook.knownAbilities.Count; i++)
            {
                if (abilityIcons.Count < character.abilityBook.knownAbilities.Count)
                {
                    UIAbilityIcon newIcon = Instantiate(uiAbilityIconPrefab, uiAbilityIconsParent).GetComponent<UIAbilityIcon>();
                    newIcon.HideAndReset();
                    abilityIcons.Add(newIcon);
                }

                bool active = character.abilityBook.activeAbilities.Contains(character.abilityBook.knownAbilities[i]);
                abilityIcons[i].BuildFromAbilityData(character.abilityBook.knownAbilities[i]);
            }
        }

        #endregion
    }
}
