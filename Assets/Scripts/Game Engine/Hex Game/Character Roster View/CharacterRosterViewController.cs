using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.UIElements;
using WeAreGladiators.Abilities;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.Items;
using WeAreGladiators.Libraries;
using WeAreGladiators.Perks;
using WeAreGladiators.UCM;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class CharacterRosterViewController : Singleton<CharacterRosterViewController>
    {
        // Properties + Components
        #region

        [Header("Core Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private ScrollRect statsPageScrollView;
        [SerializeField] private Scrollbar statsPageScrollBar;
        [SerializeField] private ItemGridScrollView playerInventory;
        [SerializeField] private RectTransform[] layoutsRebuilt;
        [Space(20)]
        [Header("Health Components")]
        [SerializeField] private TextMeshProUGUI healthBarText;
        [SerializeField] private UnityEngine.UI.Slider healthBar;
        [Space(20)]
        [Header("Morale Components")]
        [SerializeField] private UnityEngine.UI.Image moraleImage;

        [Space(20)]
        [Header("XP + Level Components")]
        [SerializeField] private UIRaceIcon racialIcon;
        [SerializeField] private UIBackgroundIcon backgroundIcon;
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private TextMeshProUGUI xpBarText;
        [SerializeField] private UnityEngine.UI.Slider xpbar;
        [Space(20)]
        [Header("Level Up Components")]
        [SerializeField] private LevelUpButton attributeLevelUpButton;
        [SerializeField] private AttributeLevelUpPage attributeLevelUpPageComponent;
        [SerializeField] private UILevelUpPerkIcon[] perkLevelUpIcons;
        [SerializeField] private UICard perkLevelUpScreenUICard;
        [SerializeField] private GameObject perkLevelUpConfirmChoiceScreenParent;
        [SerializeField] private TextMeshProUGUI perkLevelUpConfirmChoiceScreenHeaderText;
        private UILevelUpPerkIcon currentSelectedLevelUpPerkChoice;
        private UILevelUpTalentIcon currentSelectedLevelUpTalentChoice;
        [Space(20)]
        [Header("Abilities Section Components")]
        [SerializeField] private List<UIAbilityIconSelectable> selectableAbilityButtons;
        [SerializeField] private GameObject selectableAbilityButtonPrefab;
        [SerializeField] private Transform selectableAbilityButtonsParent;
        [Space(20)]
        [Header("Talent Section Components")]
        [SerializeField] private UILevelUpTalentIcon[] talentLevelUpIcons;
        [Space(20)]
        [Header("Perk Section Components")]
        [SerializeField] private UIPerkIcon[] perkButtons;
        [Space(20)]
        [Header("Character View Panel Components")]
        [SerializeField] private UniversalCharacterModel characterPanelUcm;
        [SerializeField] private RosterItemSlot mainHandSlot;
        [SerializeField] private RosterItemSlot offHandSlot;
        [SerializeField] private RosterItemSlot trinketSlot;
        [SerializeField] private RosterItemSlot headSlot;
        [SerializeField] private RosterItemSlot bodySlot;
        [SerializeField] private TextMeshProUGUI totalArmourText;
        [Space(20)]
        [Header("Overview Panel Components")]
        [SerializeField] private TMP_InputField characterNameInputField;
        [SerializeField] private TMP_InputField characterSubNameInputField;
        [SerializeField] private TextMeshProUGUI dailyWageText;
        [Space(20)]
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
        [SerializeField] private TextMeshProUGUI energyRecoveryText;
        [SerializeField] private TextMeshProUGUI maxEnergyText;
        [SerializeField] private TextMeshProUGUI initiativeText;
        [SerializeField] private TextMeshProUGUI visionText;
        [SerializeField] private TextMeshProUGUI physicalDamageText;
        [SerializeField] private TextMeshProUGUI magicDamageText;
        [Space(20)]
        [Header("Resistances Text Components")]
        [SerializeField] private TextMeshProUGUI physicalResistanceText;
        [SerializeField] private TextMeshProUGUI magicResistanceText;
        [SerializeField] private TextMeshProUGUI stressResistanceText;
        [SerializeField] private TextMeshProUGUI injuryResistanceText;
        [SerializeField] private TextMeshProUGUI debuffResistanceText;
        [SerializeField] private TextMeshProUGUI deathResistanceText;
        [Space(20)]

        [Header("New Components")]
        [SerializeField] private CharacterRosterPageButton perksTalentsPageButton;
        [SerializeField] private CharacterRosterPageButton statsPageButton;

        [SerializeField] private GameObject perksTalentsPageParent;
        [SerializeField] private GameObject statsPageParent;

        [SerializeField] private TextMeshProUGUI perkPointsText;
        [SerializeField] private TextMeshProUGUI talentsPointsText;

        [Header("Dismiss Character Page Components")]
        [SerializeField]
        private GameObject dismissVisualParent;
        [SerializeField] private UniversalCharacterModel dismissUcm;
        [SerializeField] private TextMeshProUGUI dismissPageHeaderText;

        private bool currentlyEditingName;

        private InventoryItemView currentSelectedAbilityTome;

        #endregion

        #region Getters + Accessors

        public UniversalCharacterModel CharacterPanelUcm => characterPanelUcm;
        public HexCharacterData CharacterCurrentlyViewing { get; private set; }
        public GameObject MainVisualParent => mainVisualParent;
        public RosterItemSlot MainHandSlot => mainHandSlot;
        public RosterItemSlot OffHandSLot => offHandSlot;
        public RosterItemSlot HeadSlot => headSlot;
        public RosterItemSlot BodySLot => bodySlot;
        public RosterItemSlot TrinketSlot => trinketSlot;
        public ItemGridScrollView PlayerInventory => playerInventory;
        #endregion

        // Build Perk Section
        #region

        private void BuildActivePerksSection(HexCharacterData character)
        {
            foreach (UIPerkIcon b in perkButtons)
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
                perkButtons[i].BuildFromActivePerk(allPerks[i]);
            }

            Debug.Log("Active perk icons = " + allPerks.Count);

        }

        #endregion

        // NEW LOGIC

        public void OnPerksTalentsPageButtonClicked()
        {
            perksTalentsPageParent.SetActive(true);
            statsPageParent.SetActive(false);
            perksTalentsPageButton.SetSelectedViewState();
        }  
              

        // Show + Hide Main View Logic
        #region

        public void OnCharacterRosterTopbarButtonClicked()
        {
            if (GameController.Instance.GameState == GameState.StoryEvent ||
                currentlyEditingName)
            {
                return;
            }

            if (mainVisualParent.activeSelf)
            {
                HideCharacterRosterScreen();
            }
            else
            {
                HandleBuildAndShowCharacterRoster();
            }
        }
        public void BuildAndShowFromCharacterData(HexCharacterData data)
        {
            if (CharacterDataController.Instance.AllPlayerCharacters.Count == 0)
            {
                return;
            }
            if (!mainVisualParent.activeInHierarchy)
            {
                statsPageScrollView.verticalNormalizedPosition = 1;
                statsPageScrollBar.value = 1;
            }
            mainVisualParent.SetActive(true);
            BuildRosterForCharacter(data);
            characterPanelUcm.SetIdleAnim();
        }
        private void HandleBuildAndShowCharacterRoster()
        {
            Debug.Log("ShowCharacterRosterScreen()");
            if (CharacterDataController.Instance.AllPlayerCharacters.Count == 0)
            {
                return;
            }
            if (!mainVisualParent.activeInHierarchy)
            {
                statsPageScrollView.verticalNormalizedPosition = 1;
                statsPageScrollBar.value = 1;
            }
            mainVisualParent.SetActive(true);
            HexCharacterData data = CharacterDataController.Instance.AllPlayerCharacters[0];
            BuildRosterForCharacter(data);
            characterPanelUcm.SetIdleAnim();
            playerInventory.BuildInventoryView(true);
        }
        public void HandleRedrawRosterOnCharacterUpdated()
        {
            BuildRosterForCharacter(CharacterCurrentlyViewing);
        }
        private void BuildRosterForCharacter(HexCharacterData data)
        {
            if (CharacterDataController.Instance.AllPlayerCharacters.Count == 0)
            {
                return;
            }
            // Build all sections
            if (CharacterCurrentlyViewing != data)
            {
                AudioManager.Instance.PlaySound(data.AudioProfile, AudioSet.TurnStart);
            }
            CharacterCurrentlyViewing = data;
            characterNameInputField.text = data.myName;
            characterSubNameInputField.text = data.mySubName;
            dailyWageText.text = data.dailyWage.ToString();
            BuildActivePerksSection(data);
            BuildAttributeSection(data);
            BuildGeneralInfoSection(data);
            BuildItemSlots(data);
            BuildCharacterViewPanelModel(data);
            BuildAbilitiesPage(data);
            BuildPerksAndTalentsPage(data);
            bool wasViewingPerks = perksTalentsPageParent.activeSelf;
            if (wasViewingPerks) OnPerksTalentsPageButtonClicked();
            else OnStatsPageButtonClicked();

            perksTalentsPageButton.ShowLevelUpIcon(CharacterCurrentlyViewing.talentPoints > 0 || CharacterCurrentlyViewing.perkPoints > 0);
            statsPageButton.ShowLevelUpIcon(CharacterCurrentlyViewing.attributeRolls.Count > 0);

            TransformUtils.RebuildLayouts(layoutsRebuilt);
        }
        public void HideCharacterRosterScreen()
        {
            Debug.Log("HideCharacterRosterScreen()");
            CharacterCurrentlyViewing = null;
            mainVisualParent.SetActive(false);
        }

        #endregion

        // Input
        #region
        public void OnMoraleIconMouseEnter()
        {
            MainModalController.Instance.BuildAndShowModal(CharacterCurrentlyViewing.currentMoraleState);
        }
        public void OnMoraleIconMouseExit()
        {
            MainModalController.Instance.HideModal();
        }

        public void OnFormationButtonClicked(RosterFormationButton button)
        {
            if (button.CharacterDataRef == null)
            {
                return;
            }

            BuildRosterForCharacter(button.CharacterDataRef);
        }
        public void OnPreviousCharacterButtonClicked()
        {
            Debug.Log("OnPreviousCharacterButtonClicked");
            int index = CharacterDataController.Instance.AllPlayerCharacters.IndexOf(CharacterCurrentlyViewing);
            if (CharacterDataController.Instance.AllPlayerCharacters.Count == 0)
            {
                return;
            }
            int nextIndex = 0;
            if (index == 0)
            {
                nextIndex = CharacterDataController.Instance.AllPlayerCharacters.Count - 1;
            }
            else
            {
                nextIndex = index - 1;
            }
            BuildRosterForCharacter(CharacterDataController.Instance.AllPlayerCharacters[nextIndex]);
           
        }
        public void OnNextCharacterButtonClicked()
        {
            Debug.Log("OnNextCharacterButtonClicked");
            int index = CharacterDataController.Instance.AllPlayerCharacters.IndexOf(CharacterCurrentlyViewing);
            if (CharacterDataController.Instance.AllPlayerCharacters.Count == 0)
            {
                return;
            }
            int nextIndex = 0;
            if (index == CharacterDataController.Instance.AllPlayerCharacters.Count - 1)
            {
                nextIndex = 0;
            }
            else
            {
                nextIndex = index + 1;
            }
            BuildRosterForCharacter(CharacterDataController.Instance.AllPlayerCharacters[nextIndex]);
        }
        public void OnLevelUpAttributeButtonClicked()
        {
            attributeLevelUpPageComponent.ShowAndBuildPage(CharacterCurrentlyViewing);
        }
        public void OnNameInputFieldValueChanged()
        {
            currentlyEditingName = false;
            CharacterCurrentlyViewing.myName = characterNameInputField.text;
            CharacterScrollPanelController.Instance.RebuildViews();
        }
        public void OnSubInputFieldValueChanged()
        {
            CharacterCurrentlyViewing.mySubName = characterSubNameInputField.text;
            CharacterScrollPanelController.Instance.RebuildViews();
            currentlyEditingName = false;
        }
        public void OnAnyNameInputFieldEditStart()
        {
            currentlyEditingName = true;
        }

        #endregion

        // Build Attribute Section
        #region

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
            stressResistanceText.text = StatCalculator.GetTotalBravery(character) + "%";
            injuryResistanceText.text = StatCalculator.GetTotalInjuryResistance(character) + "%";
            debuffResistanceText.text = StatCalculator.GetTotalDebuffResistance(character) + "%";
            deathResistanceText.text = StatCalculator.GetTotalDeathResistance(character) + "%";

            if (character.attributeRolls.Count > 0)
            {
                attributeLevelUpButton.ShowAndAnimate();
            }
            else
            {
                attributeLevelUpButton.Hide();
            }
        }       

        #endregion

        // Build General Info Section
        #region

        private void BuildGeneralInfoSection(HexCharacterData character)
        {
            BuildHealthBar(character);
            BuildXpBar(character);
            BuildMoraleSection(character);
            racialIcon.BuildFromRacialData(CharacterDataController.Instance.GetRaceData(character.race));
            backgroundIcon.BuildFromBackgroundData(character.background);
            totalArmourText.text = ItemController.Instance.GetTotalArmourBonusFromItemSet(character.itemSet).ToString();
        }
        private void BuildHealthBar(HexCharacterData character)
        {
            // Update slider
            float maxHealth = StatCalculator.GetTotalMaxHealth(character);
            float health = character.currentHealth;
            float healthBarFloat = health / maxHealth;
            healthBar.value = healthBarFloat;

            // Update health text
            healthBarText.text = character.currentHealth + " / " + maxHealth;
        }
        private void BuildXpBar(HexCharacterData character)
        {
            // Update slider
            float maxXp = character.currentMaxXP;
            float xp = character.currentXP;
            float xpBarFloat = xp / maxXp;
            xpbar.value = xpBarFloat;

            // Update xp text + level text
            xpBarText.text = xp + " / " + maxXp;
            currentLevelText.text = character.currentLevel.ToString();
        }
        private void BuildMoraleSection(HexCharacterData character)
        {
            moraleImage.sprite = SpriteLibrary.Instance.GetMoraleStateSprite(character.currentMoraleState);
           // moraleStateText.text = character.currentMoraleState.ToString();
        }

        #endregion

        // Build Character View Panel Section
        #region

        private void BuildCharacterViewPanelModel(HexCharacterData character)
        {
            CharacterModeller.BuildModelFromStringReferences(characterPanelUcm, character.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, characterPanelUcm);
            //characterPanelUcm.SetIdleAnim();
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

        #endregion

        // Perk Tree Section Logic
        #region

        public void OnStatsPageButtonClicked()
        {
            perksTalentsPageParent.SetActive(false);
            statsPageParent.SetActive(true);
            statsPageButton.SetSelectedViewState();
        }
        private void BuildPerksAndTalentsPage(HexCharacterData character)
        {
            if (character.PerkTree == null)
            {
                return;
            }

            for (int i = 0; i < character.PerkTree.PerkChoices.Count; i++)
            {
                perkLevelUpIcons[i].BuildFromCharacterAndPerkData(character, character.PerkTree.PerkChoices[i]);
            }

            perkPointsText.text = character.perkPoints.ToString();

            // reset buttons
            foreach (UILevelUpTalentIcon b in talentLevelUpIcons)
            {
                b.HideAndReset();
            }

            for (int i = 0; i < CharacterDataController.Instance.AllTalentData.Length && i < talentLevelUpIcons.Length; i++)
            {
                talentLevelUpIcons[i].BuildFromCharacterAndTalentData(character, CharacterDataController.Instance.AllTalentData[i]);
            }

            talentsPointsText.text = character.talentPoints.ToString();
        }
        public void OnPerkTreeIconClicked(UILevelUpPerkIcon icon)
        {
            Debug.Log("CharacterRosterViewController.OnPerkTreeIconClicked, tier = " + icon.myPerkData.tier.ToString() + 
                ", next available tier = " + icon.myCharacter.perkTree.nextAvailableTier);
            if (icon.alreadyKnown ||
                icon.myCharacter.perkPoints == 0 ||
                icon.myCharacter.currentLevel - 1 < icon.myPerkData.tier ||
                icon.myPerkData.tier != icon.myCharacter.PerkTree.nextAvailableTier)
            {
                Debug.Log("CharacterRosterViewController.OnPerkTreeIconClicked() cancelling");
                return;
            }

            currentSelectedLevelUpPerkChoice = icon;
            currentSelectedLevelUpTalentChoice = null;
            currentSelectedAbilityTome = null;

            // Build and show confirm perk choice
            perkLevelUpConfirmChoiceScreenParent.SetActive(true);
            perkLevelUpConfirmChoiceScreenHeaderText.text = "Are you sure you want to learn this perk?";

            // Build page card
            perkLevelUpScreenUICard.BuildCard(
                icon.perkIcon.PerkDataRef.passiveName,
                TextLogic.ConvertCustomStringListToString(icon.perkIcon.PerkDataRef.passiveDescription),
                icon.perkIcon.PerkDataRef.passiveSprite);
        }
        public void OnAbilityTomeRightClicked(InventoryItemView abilityTome)
        {
            // check that current user is valid
            var ability = abilityTome.MyItemRef.abilityData;

            // Doesn't meet talent req
            if (!CharacterDataController.Instance.DoesCharacterHaveTalent(CharacterCurrentlyViewing.talentPairings,
                    ability.talentRequirementData.talentSchool, ability.talentRequirementData.level))
            {
                // to do: warning guidance "this character does not have the required talent"
                return;
            }

            // Already knows the ability
            else if (CharacterCurrentlyViewing.abilityBook.KnowsAbility(ability.abilityName))
            {
                // to do: warning guidance "this character already knows this ability"
                return;
            }

            currentSelectedAbilityTome = abilityTome;
            currentSelectedLevelUpPerkChoice = null;
            currentSelectedLevelUpTalentChoice = null;

            // Build and show confirm perk choice
            perkLevelUpConfirmChoiceScreenParent.SetActive(true);
            perkLevelUpConfirmChoiceScreenHeaderText.text = "Are you sure you want to learn this ability?";

            // Build page card
            perkLevelUpScreenUICard.BuildCard(
                ability.displayedName,
                TextLogic.ConvertCustomStringListToString(ability.dynamicDescription),
                ability.AbilitySprite);
        }
        public void OnConfirmLevelUpPerkPageConfirmButtonClicked()
        {
            HexCharacterData character = null;
            UILevelUpPerkIcon perk = null;
            UILevelUpTalentIcon talent = null;
            if (currentSelectedLevelUpPerkChoice != null)
            {
                perk = currentSelectedLevelUpPerkChoice;
                character = currentSelectedLevelUpPerkChoice.myCharacter;

                // Gain perk 
                if (perk != null)
                {
                    // Pay perk point + increment perk tree tier
                    character.perkPoints--;
                    character.PerkTree.nextAvailableTier += 1;

                    // Learn new perk
                    PerkController.Instance.ModifyPerkOnCharacterData(character.passiveManager, currentSelectedLevelUpPerkChoice.perkIcon.ActivePerk.perkTag, 1);
                }
            }
            else if (currentSelectedLevelUpTalentChoice != null)
            {
                talent = currentSelectedLevelUpTalentChoice;
                character = currentSelectedLevelUpTalentChoice.myCharacter;

                // Learn talent
                if (talent != null)
                {
                    character.talentPoints--;
                    CharacterDataController.Instance.HandleLearnNewTalent(character, talent.myTalentData.talentSchool);
                }
            }

            else if (currentSelectedAbilityTome != null)
            {
                // Teach ability to character
                CharacterCurrentlyViewing.abilityBook.HandleLearnNewAbility(currentSelectedAbilityTome.MyItemRef.abilityData);

                // Remove tome from inventory
                foreach (InventoryItem i in InventoryController.Instance.PlayerInventory)
                {
                    if (i.abilityData != null &&
                        i.abilityData.abilityName == currentSelectedAbilityTome.MyItemRef.abilityData.abilityName)
                    {
                        InventoryController.Instance.RemoveItemFromInventory(i);
                        break;
                    }
                }
            }                         

            // Close views
            perkLevelUpConfirmChoiceScreenParent.SetActive(false);
            currentSelectedLevelUpPerkChoice = null;
            currentSelectedLevelUpTalentChoice = null;
            currentSelectedAbilityTome = null;

            // Rebuild character roster views
            playerInventory.BuildInventoryView();
            AudioManager.Instance.PlaySound(Sound.Effects_Confirm_Level_Up);
            HandleRedrawRosterOnCharacterUpdated();
            CharacterScrollPanelController.Instance.RebuildViews();
        }
        public void OnConfirmLevelUpPerkPageCancelButtonClicked()
        {
            perkLevelUpConfirmChoiceScreenParent.SetActive(false);
            currentSelectedLevelUpPerkChoice = null;
            currentSelectedLevelUpTalentChoice = null;
            currentSelectedAbilityTome = null;
        }

        #endregion

        // Build Abilities Section
        #region

        private void BuildAbilitiesPage(HexCharacterData character)
        {
            Debug.Log("CharacterRosterViewController.BuildAbilitiesSection() called...");

            // reset ability buttons
            foreach (UIAbilityIconSelectable b in selectableAbilityButtons)
            {
                b.Hide();
            }

            bool showSelectionState = character.abilityBook.knownAbilities.Count > AbilityBook.ActiveAbilityLimit;

            for (int i = 0; i < character.abilityBook.knownAbilities.Count; i++)
            {
                if (selectableAbilityButtons.Count < character.abilityBook.knownAbilities.Count)
                {
                    UIAbilityIconSelectable newIcon = Instantiate(selectableAbilityButtonPrefab, selectableAbilityButtonsParent).GetComponent<UIAbilityIconSelectable>();
                    newIcon.Hide();
                    selectableAbilityButtons.Add(newIcon);
                }

                bool active = character.abilityBook.activeAbilities.Contains(character.abilityBook.knownAbilities[i]);
                selectableAbilityButtons[i].Build(character.abilityBook.knownAbilities[i], active, showSelectionState);
            }

            // Update active abilities text
            //activeAbilitiesText.text = character.abilityBook.activeAbilities.Count +
            //    " / " + AbilityBook.ActiveAbilityLimit;

        }
        public void OnSelectableAbilityButtonClicked(UIAbilityIconSelectable button)
        {
            AbilityBook book = CharacterCurrentlyViewing.abilityBook;
            AbilityData ability = button.icon.MyDataRef;

            if (book.activeAbilities.Count <= AbilityBook.ActiveAbilityLimit) return;

            // Make inactive ability go active
            if (!book.HasActiveAbility(ability.abilityName) &&
                book.activeAbilities.Count < AbilityBook.ActiveAbilityLimit)
            {
                CharacterCurrentlyViewing.abilityBook.SetAbilityAsActive(book.GetKnownAbility(ability.abilityName));
                button.SetSelectedViewState(true);
               // activeAbilitiesText.text = book.activeAbilities.Count + " / " + AbilityBook.ActiveAbilityLimit;
            }

            // Make active ability go inactive
            else if (book.HasActiveAbility(ability.abilityName) &&
                     !ability.derivedFromItemLoadout &&
                     !ability.derivedFromWeapon)
            {
                CharacterCurrentlyViewing.abilityBook.SetAbilityAsInactive(book.GetKnownAbility(ability.abilityName));
                button.SetSelectedViewState(false);
                //activeAbilitiesText.text = book.activeAbilities.Count + " / " + AbilityBook.ActiveAbilityLimit;
            }
        }

        #endregion


        #region Build Talent Section Parent

        public void OnLevelUpTalentIconClicked(UILevelUpTalentIcon icon)
        {
            if (icon.alreadyKnown ||
                icon.myCharacter.talentPoints == 0)
            {
                return;
            }

            currentSelectedLevelUpTalentChoice = icon;
            currentSelectedLevelUpPerkChoice = null;
            currentSelectedAbilityTome = null;

            // Build and show confirm perk choice
            perkLevelUpConfirmChoiceScreenParent.SetActive(true);
            perkLevelUpConfirmChoiceScreenHeaderText.text = "Are you sure you want to learn this talent?";

            // Build page card
            perkLevelUpScreenUICard.BuildCard(
                TextLogic.SplitByCapitals(icon.myTalentData.talentSchool.ToString()),
                TextLogic.ConvertCustomStringListToString(icon.myTalentData.talentDescription),
                icon.myTalentData.talentSprite);
        }

        #endregion

        #region Dismiss Character Modal Logic

        public void OnDismissButtonClicked()
        {
            if (CharacterDataController.Instance.AllPlayerCharacters.Count > 1)
            {
                BuildAndShowDismissPage(CharacterCurrentlyViewing);
            }
        }
        private void BuildAndShowDismissPage(HexCharacterData character)
        {
            dismissVisualParent.SetActive(true);

            // Set header text
            dismissPageHeaderText.text = "Are you sure you want to dismiss " + character.myName + " " + character.mySubName + "?";

            // Build model
            CharacterModeller.BuildModelFromStringReferences(dismissUcm, character.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, dismissUcm);
            dismissUcm.SetIdleAnim();
        }
        public void OnConfirmDismissButtonClicked()
        {
            dismissVisualParent.SetActive(false);

            // Strip items and send to inventory
            if (CharacterCurrentlyViewing.itemSet.headArmour != null)
            {
                InventoryController.Instance.AddItemToInventory(CharacterCurrentlyViewing.itemSet.headArmour);
            }
            if (CharacterCurrentlyViewing.itemSet.bodyArmour != null)
            {
                InventoryController.Instance.AddItemToInventory(CharacterCurrentlyViewing.itemSet.bodyArmour);
            }
            if (CharacterCurrentlyViewing.itemSet.mainHandItem != null)
            {
                InventoryController.Instance.AddItemToInventory(CharacterCurrentlyViewing.itemSet.mainHandItem);
            }
            if (CharacterCurrentlyViewing.itemSet.offHandItem != null)
            {
                InventoryController.Instance.AddItemToInventory(CharacterCurrentlyViewing.itemSet.offHandItem);
            }
            if (CharacterCurrentlyViewing.itemSet.trinket != null)
            {
                InventoryController.Instance.AddItemToInventory(CharacterCurrentlyViewing.itemSet.trinket);
            }

            // Add character to roster
            CharacterDataController.Instance.RemoveCharacterFromRoster(CharacterCurrentlyViewing);

            // Rebuild character scroll roster + normal roster
            CharacterScrollPanelController.Instance.RebuildViews();
            if(CharacterDataController.Instance.AllPlayerCharacters.Count > 0)
            {
                HandleBuildAndShowCharacterRoster();
            }
            else
            {
                HideCharacterRosterScreen();
            }
            
        }
        public void OnCancelDismissButtonClicked()
        {
            dismissVisualParent.SetActive(false);
        }

        #endregion
    }
}
