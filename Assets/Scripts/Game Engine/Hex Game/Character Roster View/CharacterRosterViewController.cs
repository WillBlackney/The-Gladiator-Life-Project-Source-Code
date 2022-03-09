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

        [Header("Health Components")]
        [SerializeField] private TextMeshProUGUI healthBarText;
        [SerializeField] private Slider healthBar;

        [Header("Stress Components")]
        [SerializeField] private TextMeshProUGUI stressBarText;
        [SerializeField] private Slider stressBar;

        [Header("XP + Level Components")]
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private TextMeshProUGUI xpBarText;
        [SerializeField] private Slider xpbar;

        [Header("Abilities Section Components")]
        [SerializeField] private UIAbilityIcon[] abilityButtons;

        [Header("Talent Section Components")]
        [SerializeField] private UITalentIcon[] talentButtons;

        [Header("Perk Section Components")]
        [SerializeField] private UIPerkIcon[] perkButtons;
        [SerializeField] private GameObject[] perkRows;

        [Header("Formation Section Components")]
        [SerializeField] private RosterFormationButton[] formationButtons;

        [Header("Character View Panel Components")]
        [SerializeField] private UniversalCharacterModel characterPanelUcm;
        [SerializeField] private RosterItemSlot mainHandSlot;
        [SerializeField] private RosterItemSlot offHandSlot;
        [SerializeField] private RosterItemSlot trinketSlot;

        [Header("Overview Panel Components")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI characterClassText;

        [Header("Core Attribute Text Components")]
        [SerializeField] private TextMeshProUGUI strengthText;
        [SerializeField] private TextMeshProUGUI intelligenceText;
        [SerializeField] private TextMeshProUGUI constitutionText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private TextMeshProUGUI dodgeText;
        [SerializeField] private TextMeshProUGUI resolveText;
        [SerializeField] private TextMeshProUGUI witsText;

        [Header("Secondary Attribute Text Components")]
        [SerializeField] private TextMeshProUGUI criticalChanceText;
        [SerializeField] private TextMeshProUGUI criticalModifierText;
        [SerializeField] private TextMeshProUGUI staminaText;
        [SerializeField] private TextMeshProUGUI maxEnergyText;
        [SerializeField] private TextMeshProUGUI initiativeText;
        [SerializeField] private TextMeshProUGUI visionText;

        [Header("Resistances Text Components")]
        [SerializeField] private TextMeshProUGUI physicalResistanceText;
        [SerializeField] private TextMeshProUGUI magicResistanceText;
        [SerializeField] private TextMeshProUGUI stressResistanceText;
        [SerializeField] private TextMeshProUGUI injuryResistanceText;
        [SerializeField] private TextMeshProUGUI debuffResistanceText;


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
        private void HandleBuildAndShowCharacterRoster()
        {
            Debug.Log("ShowCharacterRosterScreen()");
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
            BuildPerkViews(data);
            BuildAttributeSection(data);
            BuildGeneralInfoSection(data);
            BuildWeaponSlots(data);
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
        #endregion              

        // Build Perk Section
        #region      
        private void BuildPerkViews(HexCharacterData character)
        {
            // Reset views
            foreach (GameObject g in perkRows)
            {
                g.SetActive(false);
            }
            foreach (UIPerkIcon b in perkButtons)
            {
                b.gameObject.SetActive(false);
            }

            // Build Icons
            int activeIcons = 0;
            List<ActivePerk> allPerks = new List<ActivePerk>();

            // Get character perks
            for (int i = 0; i < character.passiveManager.perks.Count; i++)            
                allPerks.Add(character.passiveManager.perks[i]);            

            // Add perks from items
            allPerks.AddRange(ItemController.Instance.GetActivePerksFromItemSet(character.itemSet));

            // Build perk button for each perk
            for(int i = 0; i < allPerks.Count; i++)            
                UIController.Instance.BuildPerkButton(allPerks[i], perkButtons[i]);            

            activeIcons = allPerks.Count;
            Debug.Log("Active perk icons = " + activeIcons.ToString());

            // Enable rows
            int rows = activeIcons / 4;
            rows++;
            for (int i = 0; i < rows; i++)
            {
                perkRows[i].SetActive(true);
            }

        }
       
        #endregion

        // Build Attribute Section
        #region
        private void BuildAttributeSection(HexCharacterData character)
        {
            strengthText.text = StatCalculator.GetTotalStrength(character).ToString();
            intelligenceText.text = StatCalculator.GetTotalIntelligence(character).ToString();
            constitutionText.text = StatCalculator.GetTotalConstitution(character).ToString();
            accuracyText.text = StatCalculator.GetTotalAccuracy(character).ToString();
            dodgeText.text = StatCalculator.GetTotalDodge(character).ToString();
            resolveText.text = StatCalculator.GetTotalResolve(character).ToString();
            witsText.text = StatCalculator.GetTotalWits(character).ToString();

            criticalChanceText.text = StatCalculator.GetTotalCriticalChance(character).ToString();
            criticalModifierText.text = StatCalculator.GetTotalCriticalModifier(character).ToString();
            staminaText.text = StatCalculator.GetTotalStamina(character).ToString();
            maxEnergyText.text = StatCalculator.GetTotalMaxEnergy(character).ToString();
            initiativeText.text = StatCalculator.GetTotalInitiative(character).ToString();

            physicalResistanceText.text = StatCalculator.GetTotalPhysicalResistance(character).ToString();
            magicResistanceText.text = StatCalculator.GetTotalMagicResistance(character).ToString();
            stressResistanceText.text = StatCalculator.GetTotalStressResistance(character).ToString();
            injuryResistanceText.text = StatCalculator.GetTotalInjuryResistance(character).ToString();
            debuffResistanceText.text = StatCalculator.GetTotalDebuffResistance(character).ToString();
        }
        #endregion

        // Build General Info Section
        #region
        private void BuildGeneralInfoSection(HexCharacterData character)
        {
            BuildHealthBar(character);
            BuildXpBar(character);
            BuildStressBar(character);
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
        private void BuildWeaponSlots(HexCharacterData character)
        {
            // Reset slots
            ResetItemSlot(mainHandSlot);
            ResetItemSlot(offHandSlot);
            ResetItemSlot(trinketSlot);

            BuildItemSlotFromItemData(mainHandSlot, character.itemSet.mainHandItem);
            BuildItemSlotFromItemData(offHandSlot, character.itemSet.offHandItem);
            BuildItemSlotFromItemData(trinketSlot, character.itemSet.trinket);
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
                b.AbilityImageParent.SetActive(false);
                b.SetMyDataReference(null);
            }

            // Main hand weapon abilities
            Debug.LogWarning("main hand weapon abilities = " + character.itemSet.mainHandItem.grantedAbilities.Count.ToString());
            int newIndexCount = 0;
            for (int i = 0; i < character.itemSet.mainHandItem.grantedAbilities.Count; i++)
            {
                Debug.LogWarning("main hand weapon is not null");
                // Characters dont gain special weapon ability if they have an off hand item
                if (character.itemSet.offHandItem == null || (character.itemSet.offHandItem != null && character.itemSet.mainHandItem.grantedAbilities[i].weaponAbilityType == WeaponAbilityType.Basic))
                {
                    Debug.LogWarning("gained main hand weapon ability");
                    BuildAbilityButtonFromAbility(abilityButtons[i], character.itemSet.mainHandItem.grantedAbilities[i]);
                    newIndexCount++;
                }
            }

            // Off hand weapon abilities
            if (character.itemSet.offHandItem != null)
            {
                for (int i = 0; i < character.itemSet.offHandItem.grantedAbilities.Count; i++)
                {
                    BuildAbilityButtonFromAbility(abilityButtons[i + newIndexCount], character.itemSet.offHandItem.grantedAbilities[i]);
                    newIndexCount++;
                }
            }


            // Build non item derived abilities
            for (int i = 0; i < character.abilityBook.allKnownAbilities.Count; i++)
            {
                BuildAbilityButtonFromAbility(abilityButtons[i + newIndexCount], character.abilityBook.allKnownAbilities[i]);
            }
        }
        public void BuildAbilityButtonFromAbility(UIAbilityIcon b, AbilityData d)
        {
            Debug.Log("CharacterRosterViewController.BuildAbilityButtonFromAbility() building from ability: " + d.abilityName);
            b.AbilityImage.sprite = d.AbilitySprite;
            b.AbilityImageParent.SetActive(true);
            b.SetMyDataReference(d);
        }
        #endregion

        // Build Talent Section Parent
        #region       
        private void BuildTalentsSection(HexCharacterData character)
        {
            // reset buttons
            foreach(UITalentIcon b in talentButtons)
            {
                b.TalentImageParent.SetActive(false);
                b.SetMyTalent(null);
            }

            for(int i = 0; i < character.talentPairings.Count; i++)
            {
                UIController.Instance.BuildTalentButton(talentButtons[i], character.talentPairings[i].talentSchool);
            }
        }

        #endregion

       



    }
}
