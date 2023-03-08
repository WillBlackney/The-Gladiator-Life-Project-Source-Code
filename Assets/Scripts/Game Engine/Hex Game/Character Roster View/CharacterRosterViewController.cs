using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Characters;
using HexGameEngine.Perks;
using TMPro;
using UnityEngine.UI;
using HexGameEngine.Items;
using HexGameEngine.UCM;
using HexGameEngine.Abilities;
using HexGameEngine.Cards;
using DG.Tweening;
using CardGameEngine.UCM;
using UnityEngine.TextCore.Text;

namespace HexGameEngine.UI
{
    public class CharacterRosterViewController : Singleton<CharacterRosterViewController>
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private Scrollbar[] scrollBarResets;
        [Space(20)]
        [Header("Health Components")]
        [SerializeField] private TextMeshProUGUI healthBarText;
        [SerializeField] private Slider healthBar;
        [Space(20)]
        [Header("Stress Components")]
        [SerializeField] private TextMeshProUGUI stressBarText;
        [SerializeField] private Slider stressBar;
        [Space(20)]
        [Header("XP + Level Components")]
        [SerializeField] private UIRaceIcon racialIcon;
        [SerializeField] private UIBackgroundIcon backgroundIcon;
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private TextMeshProUGUI xpBarText;
        [SerializeField] private Slider xpbar;
        [Space(20)]
        [Header("Level Up Components")]
        [SerializeField] private LevelUpButton attributeLevelUpButton;
        [SerializeField] private AttributeLevelUpPage attributeLevelUpPageComponent;
        [SerializeField] private LevelUpButton perkLevelUpButton;
        [SerializeField] private LevelUpButton talentLevelUpButton;
        [SerializeField] private PerkTalentLevelUpPage talentLevelUpPage;
        [SerializeField] private UILevelUpPerkIcon[] perkLevelUpIcons;
        [SerializeField] private UICard perkLevelUpScreenUICard;
        [SerializeField] private GameObject perkLevelUpConfirmChoiceScreenParent;
        private UILevelUpPerkIcon currentSelectedLevelUpPerkChoice;
        [Space(20)]
        [Header("Abilities Section Components")]
        [SerializeField] private List<UIAbilityIconSelectable> selectableAbilityButtons;
        [SerializeField] private GameObject selectableAbilityButtonPrefab;
        [SerializeField] private TextMeshProUGUI activeAbilitiesText;
        [SerializeField] private Transform selectableAbilityButtonsParent;
        [Space(20)]
        [Header("Talent Section Components")]
        [SerializeField] private UITalentIcon[] talentButtons;
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
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI characterClassText;
        [SerializeField] private TextMeshProUGUI dailyWageText;
        [Space(20)]

        [Header("Core Attribute Components")]
        [SerializeField] private TextMeshProUGUI mightText;
        [SerializeField] private GameObject[] mightStars;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI constitutionText;
        [SerializeField] private GameObject[] constitutionStars;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private GameObject[] accuracyStars;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI dodgeText;
        [SerializeField] private GameObject[] dodgeStars;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI resolveText;
        [SerializeField] private GameObject[] resolveStars;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI witsText;
        [SerializeField] private GameObject[] witsStars;
        [Space(20)]
        [SerializeField] private TextMeshProUGUI fitnessText;
        [SerializeField] private GameObject[] fitnessStars;
        [Space(20)]

        [Header("Secondary Attribute Text Components")]
        [SerializeField] private TextMeshProUGUI criticalChanceText;
        [SerializeField] private TextMeshProUGUI criticalModifierText;
        [SerializeField] private TextMeshProUGUI energyRecoveryText;
        [SerializeField] private TextMeshProUGUI maxEnergyText;
        [SerializeField] private TextMeshProUGUI maxFatigueText;
        [SerializeField] private TextMeshProUGUI fatigueRecoveryText;
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
        [SerializeField] CharacterRosterPageButton perkPageButton;
        [SerializeField] CharacterRosterPageButton talentPageButton;
        [SerializeField] CharacterRosterPageButton abilityPageButton;

        [SerializeField] GameObject perkPageParent;
        [SerializeField] GameObject talentPageParent;
        [SerializeField] GameObject abilityPageParent;

        private HexCharacterData characterCurrentlyViewing;
        #endregion

        // Getters + Accessors
        #region
        public UniversalCharacterModel CharacterPanelUcm
        {
            get { return characterPanelUcm; }
        }
        public HexCharacterData CharacterCurrentlyViewing
        {
            get { return characterCurrentlyViewing; }
        }
        public GameObject MainVisualParent
        {
            get { return mainVisualParent; }
        }
        public RosterItemSlot MainHandSlot
        {
            get { return mainHandSlot; }
        }
        public RosterItemSlot OffHandSLot
        {
            get { return offHandSlot; }
        }
        public RosterItemSlot HeadSlot
        {
            get { return headSlot; }
        }
        public RosterItemSlot BodySLot
        {
            get { return bodySlot; }
        }
        public RosterItemSlot TrinketSlot
        {
            get { return trinketSlot; }
        }
        #endregion

        // Show + Hide Main View Logic
        #region
        public void OnCharacterRosterTopbarButtonClicked()
        {
            if (GameController.Instance.GameState == GameState.StoryEvent) return;

            if (mainVisualParent.activeSelf)
                HideCharacterRosterScreen();
            else
                HandleBuildAndShowCharacterRoster();
        }
        public void BuildAndShowFromCharacterData(HexCharacterData data)
        {
            if (!mainVisualParent.activeInHierarchy)
                for (int i = 0; i < scrollBarResets.Length; i++)
                scrollBarResets[i].value = 1;
            mainVisualParent.SetActive(true);
            BuildRosterForCharacter(data);
        }
        private void HandleBuildAndShowCharacterRoster()
        {
            Debug.Log("ShowCharacterRosterScreen()");
            if (!mainVisualParent.activeInHierarchy)
                for (int i = 0; i < scrollBarResets.Length; i++)
                    scrollBarResets[i].value = 1;
            mainVisualParent.SetActive(true);
            HexCharacterData data = CharacterDataController.Instance.AllPlayerCharacters[0];
            BuildRosterForCharacter(data);
        }
        public void HandleRedrawRosterOnCharacterUpdated()
        {
            BuildRosterForCharacter(CharacterCurrentlyViewing);
        }
        private void BuildRosterForCharacter(HexCharacterData data)
        {
            // Build all sections
            characterCurrentlyViewing = data;
            characterNameText.text = data.myName;
            characterClassText.text = data.myClassName;
            dailyWageText.text = data.dailyWage.ToString();         
            BuildPerkViews(data);
            BuildAttributeSection(data);
            BuildGeneralInfoSection(data);
            BuildItemSlots(data);
            BuildCharacterViewPanelModel(data);
            BuildAbilitiesPage(data);
            BuildTalentsPage(data);
            BuildPerkPage(data);

            OnPerksPageButtonClicked();
            
        }
        public void HideCharacterRosterScreen()
        {
            Debug.Log("HideCharacterRosterScreen()");
            characterCurrentlyViewing = null;
            mainVisualParent.SetActive(false);
        }

        #endregion

        // Input
        #region
        public void OnFormationButtonClicked(RosterFormationButton button)
        {
            if (button.CharacterDataRef == null) return;

            BuildRosterForCharacter(button.CharacterDataRef);
        }
        public void OnPreviousCharacterButtonClicked()
        {
            Debug.Log("OnPreviousCharacterButtonClicked");
            int index = CharacterDataController.Instance.AllPlayerCharacters.IndexOf(characterCurrentlyViewing);
            if (CharacterDataController.Instance.AllPlayerCharacters.Count == 0) return;
            int nextIndex = 0;
            if (index == 0) nextIndex = CharacterDataController.Instance.AllPlayerCharacters.Count - 1;
            else nextIndex = index - 1;
            BuildRosterForCharacter(CharacterDataController.Instance.AllPlayerCharacters[nextIndex]);
        }
        public void OnNextCharacterButtonClicked()
        {
            Debug.Log("OnNextCharacterButtonClicked");
            int index = CharacterDataController.Instance.AllPlayerCharacters.IndexOf(characterCurrentlyViewing);
            if (CharacterDataController.Instance.AllPlayerCharacters.Count == 0) return;
            int nextIndex = 0;
            if (index == CharacterDataController.Instance.AllPlayerCharacters.Count - 1) nextIndex = 0;
            else nextIndex = index + 1;
            BuildRosterForCharacter(CharacterDataController.Instance.AllPlayerCharacters[nextIndex]);
        }
        public void OnLevelUpAttributeButtonClicked()
        {
            attributeLevelUpPageComponent.ShowAndBuildPage(characterCurrentlyViewing);
        }       
        public void OnLevelUpTalentButtonClicked()
        {
            talentLevelUpPage.ShowAndBuildForTalentReward(characterCurrentlyViewing);
        }
        #endregion              

        // Build Perk Section
        #region      
        private void BuildPerkViews(HexCharacterData character)
        {
            foreach (UIPerkIcon b in perkButtons)            
                b.HideAndReset();            

            // Build Icons
            List<ActivePerk> allPerks = new List<ActivePerk>();

            // Get character perks
            for (int i = 0; i < character.passiveManager.perks.Count; i++)            
                allPerks.Add(character.passiveManager.perks[i]);            

            // Add perks from items
            allPerks.AddRange(ItemController.Instance.GetActivePerksFromItemSet(character.itemSet));

            // Build perk button for each perk
            for (int i = 0; i < allPerks.Count; i++)
                perkButtons[i].BuildFromActivePerk(allPerks[i]);          

            Debug.Log("Active perk icons = " + allPerks.Count.ToString());           

            if (character.perkPoints > 0)
                perkLevelUpButton.ShowAndAnimate();
            else perkLevelUpButton.Hide();

        }
       
        #endregion

        // Build Attribute Section
        #region
        private void BuildAttributeSection(HexCharacterData character)
        {
            mightText.text = StatCalculator.GetTotalMight(character).ToString();
            BuildStars(mightStars, character.attributeSheet.might.stars);

            constitutionText.text = StatCalculator.GetTotalConstitution(character).ToString();
            BuildStars(constitutionStars, character.attributeSheet.constitution.stars);

            accuracyText.text = StatCalculator.GetTotalAccuracy(character).ToString();
            BuildStars(accuracyStars, character.attributeSheet.accuracy.stars);

            dodgeText.text = StatCalculator.GetTotalDodge(character).ToString();
            BuildStars(dodgeStars, character.attributeSheet.dodge.stars);

            resolveText.text = StatCalculator.GetTotalResolve(character).ToString();
            BuildStars(resolveStars, character.attributeSheet.resolve.stars);

            witsText.text = StatCalculator.GetTotalWits(character).ToString();
            BuildStars(witsStars, character.attributeSheet.wits.stars);

            fitnessText.text = StatCalculator.GetTotalFitness(character).ToString();
            BuildStars(fitnessStars, character.attributeSheet.fitness.stars);

            criticalChanceText.text = StatCalculator.GetTotalCriticalChance(character).ToString() + "%";
            criticalModifierText.text = StatCalculator.GetTotalCriticalModifier(character).ToString() + "%";
            energyRecoveryText.text = StatCalculator.GetTotalActionPointRecovery(character).ToString();
            maxEnergyText.text = StatCalculator.GetTotalMaxActionPoints(character).ToString();
            maxFatigueText.text = StatCalculator.GetTotalMaxFatigue(character).ToString();
            fatigueRecoveryText.text = StatCalculator.GetTotalFatigueRecovery(character).ToString();
            initiativeText.text = StatCalculator.GetTotalInitiative(character).ToString();
            visionText.text = StatCalculator.GetTotalVision(character).ToString();
            physicalDamageText.text = StatCalculator.GetTotalPhysicalDamageBonus(character).ToString() + "%";
            magicDamageText.text = StatCalculator.GetTotalMagicDamageBonus(character).ToString() + "%";

            physicalResistanceText.text = StatCalculator.GetTotalPhysicalResistance(character).ToString() + "%";
            magicResistanceText.text = StatCalculator.GetTotalMagicResistance(character).ToString() + "%";
            stressResistanceText.text = StatCalculator.GetTotalStressResistance(character).ToString() + "%";
            injuryResistanceText.text = StatCalculator.GetTotalInjuryResistance(character).ToString() + "%";
            debuffResistanceText.text = StatCalculator.GetTotalDebuffResistance(character).ToString() + "%";
            deathResistanceText.text = StatCalculator.GetTotalDeathResistance(character).ToString() + "%";

            if (character.attributeRolls.Count > 0)
                attributeLevelUpButton.ShowAndAnimate();
            else attributeLevelUpButton.Hide();
        }
        private void BuildStars(GameObject[] arr, int starCount)
        {
            // Reset
            for(int i = 0; i < arr.Length; i++)            
                arr[i].gameObject.SetActive(false);

            for (int i = 0; i < starCount; i++)
                arr[i].gameObject.SetActive(true);
        }
        #endregion

        // Build General Info Section
        #region
        private void BuildGeneralInfoSection(HexCharacterData character)
        {
            BuildHealthBar(character);
            BuildXpBar(character);
            BuildStressBar(character);
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
            healthBarText.text = character.currentHealth.ToString() + " / " + maxHealth.ToString();
        }
        private void BuildXpBar(HexCharacterData character)
        {
            // Update slider
            float maxXp = character.currentMaxXP;
            float xp = character.currentXP;
            float xpBarFloat = xp / maxXp;
            xpbar.value = xpBarFloat;

            // Update xp text + level text
            xpBarText.text = xp.ToString() + " / " + maxXp.ToString();
            currentLevelText.text = character.currentLevel.ToString();
        }
        private void BuildStressBar(HexCharacterData character)
        {
            // Update slider
            float maxStress = 20;
            float stress = character.currentStress;
            float stresBarFloat = stress / maxStress;
            stressBar.value = stresBarFloat;

            // Update stress text
            stressBarText.text = stress.ToString() + " / " + maxStress.ToString();
        }
        #endregion

        // Build Character View Panel Section
        #region
        private void BuildCharacterViewPanelModel(HexCharacterData character)
        {
            CharacterModeller.BuildModelFromStringReferences(characterPanelUcm, character.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, characterPanelUcm);
            characterPanelUcm.SetIdleAnim();
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
            if (item == null) return;
            slot.SetMyDataReference(item);
            slot.ItemImage.sprite = item.ItemSprite;
            slot.ItemImage.gameObject.SetActive(true);
        }
        private void ResetItemSlot(RosterItemSlot slot)
        {
            slot.ItemImage.gameObject.SetActive(false);
            slot.SetMyDataReference(null);
        }
        #endregion

        // Perk Tree Section Logic
        #region
        private void BuildPerkPage(HexCharacterData character)
        {
            if (character.PerkTree == null) return;

            for(int i = 0; i < character.PerkTree.PerkChoices.Count; i++)
            {
                perkLevelUpIcons[i].BuildFromCharacterAndPerkData(character, character.PerkTree.PerkChoices[i]);
            }
        }
        public void OnPerkTreeIconClicked(UILevelUpPerkIcon icon)
        {
            if (icon.alreadyKnown || 
                icon.myCharacter.perkPoints == 0 || 
                icon.myCharacter.currentLevel - 1 < icon.myPerkData.tier ||
                icon.myPerkData.tier != icon.myCharacter.PerkTree.nextAvailableTier) return;

            currentSelectedLevelUpPerkChoice = icon;

            // Build and show confirm perk choice
            perkLevelUpConfirmChoiceScreenParent.SetActive(true);

            // Build page card
            perkLevelUpScreenUICard.BuildCard(
                icon.perkIcon.PerkDataRef.passiveName, 
                TextLogic.ConvertCustomStringListToString(icon.perkIcon.PerkDataRef.passiveDescription), 
                icon.perkIcon.PerkDataRef.passiveSprite);
        }
        public void OnConfirmLevelUpPerkPageConfirmButtonClicked()
        {
            HexCharacterData character = currentSelectedLevelUpPerkChoice.myCharacter;

            // Pay perk point + increment perk tree tier
            character.perkPoints--;
            character.PerkTree.nextAvailableTier += 1;

            // Learn new perk
            PerkController.Instance.ModifyPerkOnCharacterData(character.passiveManager, currentSelectedLevelUpPerkChoice.perkIcon.ActivePerk.perkTag, 1);
            
            // Close views
            perkLevelUpConfirmChoiceScreenParent.SetActive(false);
            currentSelectedLevelUpPerkChoice = null;

            // Rebuild character roster views
            HandleRedrawRosterOnCharacterUpdated();
            CharacterScrollPanelController.Instance.RebuildViews();
        }
        public void OnConfirmLevelUpPerkPageCancelButtonClicked()
        {
            perkLevelUpConfirmChoiceScreenParent.SetActive(false);
            currentSelectedLevelUpPerkChoice = null;
        }
        #endregion

        // Build Abilities Section
        #region
        private void BuildAbilitiesPage(HexCharacterData character)
        {
            Debug.Log("CharacterRosterViewController.BuildAbilitiesSection() called...");

            // reset ability buttons
            foreach (UIAbilityIconSelectable b in selectableAbilityButtons)            
                b.Hide();            

            // build an icon for each known ability
            // set selected state of icons by comparing against active abilities
            // update selected abilities text

            for(int i = 0; i < character.abilityBook.knownAbilities.Count; i++)
            {
                if(selectableAbilityButtons.Count < character.abilityBook.knownAbilities.Count)
                {
                    UIAbilityIconSelectable newIcon = Instantiate(selectableAbilityButtonPrefab, selectableAbilityButtonsParent).GetComponent<UIAbilityIconSelectable>();
                    newIcon.Hide();
                    selectableAbilityButtons.Add(newIcon);
                }

                bool active = character.abilityBook.activeAbilities.Contains(character.abilityBook.knownAbilities[i]);
                selectableAbilityButtons[i].Build(character.abilityBook.knownAbilities[i], active);
            }

            // Update active abilities text
            activeAbilitiesText.text = character.abilityBook.activeAbilities.Count.ToString() + 
                " / " + AbilityBook.ActiveAbilityLimit.ToString();

        }
        public void OnSelectableAbilityButtonClicked(UIAbilityIconSelectable button)
        {
            AbilityBook book = characterCurrentlyViewing.abilityBook;
            AbilityData ability = button.icon.MyDataRef;

            // Make inactive ability go active
            if (!book.HasActiveAbility(ability.abilityName) &&
                book.activeAbilities.Count < AbilityBook.ActiveAbilityLimit)
            {
                characterCurrentlyViewing.abilityBook.SetAbilityAsActive(book.GetKnownAbility(ability.abilityName));
                button.SetSelectedViewState(true);
                activeAbilitiesText.text = book.activeAbilities.Count.ToString() + " / " + AbilityBook.ActiveAbilityLimit.ToString();
            }

            // Make active ability go inactive
            else if (book.HasActiveAbility(ability.abilityName) &&
                !ability.derivedFromItemLoadout && 
                !ability.derivedFromWeapon)
            {
                characterCurrentlyViewing.abilityBook.SetAbilityAsInactive(book.GetKnownAbility(ability.abilityName));
                button.SetSelectedViewState(false);
                activeAbilitiesText.text = book.activeAbilities.Count.ToString() + " / " + AbilityBook.ActiveAbilityLimit.ToString();
            }
        }
        #endregion

        // Build Talent Section Parent
        #region       
        private void BuildTalentsPage(HexCharacterData character)
        {
            // reset buttons
            foreach(UITalentIcon b in talentButtons)
            {
                b.HideAndReset();
            }

            for(int i = 0; i < character.talentPairings.Count; i++)
            {
                talentButtons[i].BuildFromTalentPairing(character.talentPairings[i]);
            }

            if (character.talentRolls.Count > 0)
                talentLevelUpButton.ShowAndAnimate();
            else talentLevelUpButton.Hide();
        }

        #endregion


        // NEW LOGIC
        public void OnPerksPageButtonClicked()
        {
            perkPageParent.SetActive(true);
            talentPageParent.SetActive(false);
            abilityPageParent.SetActive(false);

            perkPageButton.SetSelectedViewState(0.25f);
        }
        public void OnTalentsPageButtonClicked()
        {
            perkPageParent.SetActive(false);
            talentPageParent.SetActive(true);
            abilityPageParent.SetActive(false);

            talentPageButton.SetSelectedViewState(0.25f);
        }
        public void OnAbilitiesPageButtonClicked()
        {
            perkPageParent.SetActive(false);
            talentPageParent.SetActive(false);
            abilityPageParent.SetActive(true);

            abilityPageButton.SetSelectedViewState(0.25f);
        }




    }
}
