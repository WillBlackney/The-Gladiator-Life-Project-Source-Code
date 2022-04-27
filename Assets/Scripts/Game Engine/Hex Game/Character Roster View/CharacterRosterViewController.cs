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

namespace HexGameEngine.UI
{
    public class CharacterRosterViewController : Singleton<CharacterRosterViewController>
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private Scrollbar rightPanelScrollBar;
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
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private TextMeshProUGUI xpBarText;
        [SerializeField] private Slider xpbar;
        [Space(20)]
        [Header("Level Up Components")]
        [SerializeField] private GameObject attributeLevelUpButton;
        [SerializeField] private AttributeLevelUpPage attributeLevelUpPageComponent;
        [SerializeField] private GameObject perkLevelUpButton;
        [SerializeField] private GameObject talentLevelUpButton;
        [SerializeField] private PerkTalentLevelUpPage perkTalentLevelUpPage;
        [Space(20)]
        [Header("Abilities Section Components")]
        [SerializeField] private UIAbilityIcon[] abilityButtons;
        [Space(20)]
        [Header("Talent Section Components")]
        [SerializeField] private UITalentIcon[] talentButtons;
        [Space(20)]
        [Header("Perk Section Components")]
        [SerializeField] private UIPerkIcon[] perkButtons;
        [Space(20)]
        [Header("Formation Section Components")]
        [SerializeField] private RosterFormationButton[] formationButtons;
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
        [SerializeField] private TextMeshProUGUI strengthText;
        [SerializeField] private GameObject[] strengthStars;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI intelligenceText;
        [SerializeField] private GameObject[] intelligenceStars;
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

        [Header("Secondary Attribute Text Components")]
        [SerializeField] private TextMeshProUGUI criticalChanceText;
        [SerializeField] private TextMeshProUGUI criticalModifierText;
        [SerializeField] private TextMeshProUGUI energyRecoveryText;
        [SerializeField] private TextMeshProUGUI maxEnergyText;
        [SerializeField] private TextMeshProUGUI initiativeText;
        [SerializeField] private TextMeshProUGUI visionText;
        [Space(20)]
        [Header("Resistances Text Components")]
        [SerializeField] private TextMeshProUGUI physicalResistanceText;
        [SerializeField] private TextMeshProUGUI magicResistanceText;
        [SerializeField] private TextMeshProUGUI stressResistanceText;
        [SerializeField] private TextMeshProUGUI injuryResistanceText;
        [SerializeField] private TextMeshProUGUI debuffResistanceText;
        [SerializeField] private TextMeshProUGUI deathResistanceText;
        [Space(20)]

        [HideInInspector] public RosterItemSlot rosterSlotMousedOver = null;

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
        #endregion

        // Show + Hide Main View Logic
        #region
        public void OnCharacterRosterTopbarButtonClicked()
        {
            if (mainVisualParent.activeSelf)
                HideCharacterRosterScreen();
            else
                HandleBuildAndShowCharacterRoster();
        }
        public void BuildAndShowFromCharacterData(HexCharacterData data)
        {
            if(!mainVisualParent.activeInHierarchy) rightPanelScrollBar.value = 1;
            mainVisualParent.SetActive(true);
            BuildRosterForCharacter(data);
        }
        private void HandleBuildAndShowCharacterRoster()
        {
            Debug.Log("ShowCharacterRosterScreen()");
            if (!mainVisualParent.activeInHierarchy) rightPanelScrollBar.value = 1;
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
            totalArmourText.text = ItemController.Instance.GetTotalArmourBonusFromItemSet(data.itemSet).ToString();
            BuildPerkViews(data);
            BuildAttributeSection(data);
            BuildGeneralInfoSection(data);
            BuildItemSlots(data);
            BuildCharacterViewPanelModel(data);
            BuildAbilitiesSection(data);
            BuildTalentsSection(data);
            BuildFormationButtons(CharacterDataController.Instance.AllPlayerCharacters);
            
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
        public void OnLevelUpPerkButtonClicked()
        {
            perkTalentLevelUpPage.ShowAndBuildForPerkReward(characterCurrentlyViewing);
        }
        public void OnLevelUpTalentButtonClicked()
        {
            perkTalentLevelUpPage.ShowAndBuildForTalentReward(characterCurrentlyViewing);
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

            if (character.perkRolls.Count > 0)
                perkLevelUpButton.SetActive(true);
            else perkLevelUpButton.SetActive(false);

        }
       
        #endregion

        // Build Attribute Section
        #region
        private void BuildAttributeSection(HexCharacterData character)
        {
            strengthText.text = StatCalculator.GetTotalStrength(character).ToString();
            BuildStars(strengthStars, character.attributeSheet.strength.stars);

            intelligenceText.text = StatCalculator.GetTotalIntelligence(character).ToString();
            BuildStars(intelligenceStars, character.attributeSheet.intelligence.stars);

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

            criticalChanceText.text = StatCalculator.GetTotalCriticalChance(character).ToString();
            criticalModifierText.text = StatCalculator.GetTotalCriticalModifier(character).ToString();
            energyRecoveryText.text = StatCalculator.GetTotalEnergyRecovery(character).ToString();
            maxEnergyText.text = StatCalculator.GetTotalMaxEnergy(character).ToString();
            initiativeText.text = StatCalculator.GetTotalInitiative(character).ToString();
            visionText.text = StatCalculator.GetTotalVision(character).ToString();

            physicalResistanceText.text = StatCalculator.GetTotalPhysicalResistance(character).ToString();
            magicResistanceText.text = StatCalculator.GetTotalMagicResistance(character).ToString();
            stressResistanceText.text = StatCalculator.GetTotalStressResistance(character).ToString();
            injuryResistanceText.text = StatCalculator.GetTotalInjuryResistance(character).ToString();
            debuffResistanceText.text = StatCalculator.GetTotalDebuffResistance(character).ToString();
            deathResistanceText.text = StatCalculator.GetTotalDeathResistance(character).ToString();

            if (character.attributeRolls.Count > 0)            
                attributeLevelUpButton.SetActive(true);            
            else attributeLevelUpButton.SetActive(false);
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
            float maxStress = 100;
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

        // Build Formation Section
        #region
        private void BuildFormationButtons(List<HexCharacterData> characters)
        {
            // Reset buttons
            foreach (RosterFormationButton b in formationButtons)
                b.Reset();

            // Build buttons for each character
            foreach (HexCharacterData c in characters)
            {
                foreach (RosterFormationButton b in formationButtons)
                {
                    if (c.formationPosition == b.FormationPosition)
                    {
                        // build button
                        BuildFormationButtonFromCharacter(b, c);
                        break;
                    }
                }
            }
        }
        private void BuildFormationButtonFromCharacter(RosterFormationButton button, HexCharacterData character)
        {
            button.SetMyCharacter(character);
            button.CharacterVisualParent.SetActive(true);
            CharacterModeller.BuildModelFromStringReferencesAsMugshot(button.Ucm, character.modelParts);
            button.Ucm.SetBaseAnim();
        }
        #endregion

        // Build Abilities Section
        #region
        private void BuildAbilitiesSection(HexCharacterData character)
        {
            Debug.Log("CharacterRosterViewController.BuildAbilitiesSection() called...");

            // reset ability buttons
            foreach (UIAbilityIcon b in abilityButtons)
            {
                b.HideAndReset();
            }

            // Main hand weapon abilities
            int newIndexCount = 0;
            if(character.itemSet.mainHandItem != null)
            {
                for (int i = 0; i < character.itemSet.mainHandItem.grantedAbilities.Count; i++)
                {
                    Debug.LogWarning("main hand weapon is not null");
                    // Characters dont gain special weapon ability if they have an off hand item
                    if (character.itemSet.offHandItem == null || (character.itemSet.offHandItem != null && character.itemSet.mainHandItem.grantedAbilities[i].weaponAbilityType == WeaponAbilityType.Basic))
                    {
                        Debug.LogWarning("gained main hand weapon ability");
                        abilityButtons[i].BuildFromAbilityData(character.itemSet.mainHandItem.grantedAbilities[i]);
                        newIndexCount++;
                    }
                }
            }               

            // Off hand weapon abilities
            if (character.itemSet.offHandItem != null)
            {
                for (int i = 0; i < character.itemSet.offHandItem.grantedAbilities.Count; i++)
                {
                    abilityButtons[i + newIndexCount].BuildFromAbilityData(character.itemSet.offHandItem.grantedAbilities[i]);
                    newIndexCount++;
                }
            }

            // Build non item derived abilities
            for (int i = 0; i < character.abilityBook.allKnownAbilities.Count; i++)
            {
                abilityButtons[i + newIndexCount].BuildFromAbilityData(character.abilityBook.allKnownAbilities[i]);
                newIndexCount++;
            }
        }
        public void BuildAbilityButtonFromAbility(UIAbilityIcon b, AbilityData d)
        {
            b.BuildFromAbilityData(d);
        }
        #endregion

        // Build Talent Section Parent
        #region       
        private void BuildTalentsSection(HexCharacterData character)
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
                talentLevelUpButton.SetActive(true);
            else talentLevelUpButton.SetActive(false);
        }

        #endregion

       



    }
}
