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
    public class EnemyInfoPanel : Singleton<EnemyInfoPanel>
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private Scrollbar[] scrollBarResets;
        [Space(20)]
        [Header("Left Panel Components")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI xpRewardValueText;
        [SerializeField] private TextMeshProUGUI totalArmourText;
        [SerializeField] private List<UIAbilityIcon> abilityIcons;
        [SerializeField] private Transform uiAbilityIconsParent;
        [SerializeField] private GameObject uiAbilityIconPrefab;
        [SerializeField] private UIPerkIcon[] passiveIcons;
        [SerializeField] private UniversalCharacterModel characterPanelUcm;

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

        [Header("Secondary Attribute Text Components")]
        [SerializeField] private TextMeshProUGUI criticalChanceText;
        [SerializeField] private TextMeshProUGUI criticalModifierText;
        [SerializeField] private TextMeshProUGUI initiativeText;
        [SerializeField] private TextMeshProUGUI visionText;
        [Space(20)]
        [Header("Resistances Text Components")]
        [SerializeField] private TextMeshProUGUI physicalResistanceText;
        [SerializeField] private TextMeshProUGUI magicResistanceText;
        [SerializeField] private TextMeshProUGUI injuryResistanceText;
        [SerializeField] private TextMeshProUGUI debuffResistanceText;
        [Space(20)]

        private HexCharacterData characterCurrentlyViewing;
        #endregion

        // Show, Hide and Build
        #region
        
        public void HandleBuildAndShowPanel(HexCharacterData data)
        {
            characterCurrentlyViewing = data;
            if (!mainVisualParent.activeInHierarchy)
                for (int i = 0; i < scrollBarResets.Length; i++)
                    scrollBarResets[i].value = 1;
            mainVisualParent.SetActive(true);
            BuildPanelForEnemy(data);
        }
        private void BuildPanelForEnemy(HexCharacterData data)
        {
            // Build all sections
            characterCurrentlyViewing = data;
            characterNameText.text = data.myName;
            totalArmourText.text = data.baseArmour.ToString();
            xpRewardValueText.text = data.xpReward.ToString();

            BuildPerkViews(data);
            BuildAttributeSection(data);
            BuildCharacterViewPanelModel(data);
            BuildAbilitiesSection(data);
        }
        public void HidePanel()
        {
            characterCurrentlyViewing = null;
            mainVisualParent.SetActive(false);
        }
        #endregion

        // Build Sections
        #region
        private void BuildPerkViews(HexCharacterData character)
        {
            foreach (UIPerkIcon b in passiveIcons)
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
                passiveIcons[i].BuildFromActivePerk(allPerks[i]);
        }
        private void BuildAttributeSection(HexCharacterData character)
        {
            mightText.text = StatCalculator.GetTotalMight(character).ToString();
            BuildStars(mightStars, 0);

            constitutionText.text = (StatCalculator.GetTotalConstitution(character) + character.currentHealth).ToString();
            BuildStars(constitutionStars, 0);

            accuracyText.text = StatCalculator.GetTotalAccuracy(character).ToString();
            BuildStars(accuracyStars, 0);

            dodgeText.text = StatCalculator.GetTotalDodge(character).ToString();
            BuildStars(dodgeStars, 0);

            resolveText.text = StatCalculator.GetTotalResolve(character).ToString();
            BuildStars(resolveStars, 0);

            witsText.text = StatCalculator.GetTotalWits(character).ToString();
            BuildStars(witsStars, 0);

            criticalChanceText.text = StatCalculator.GetTotalCriticalChance(character).ToString() + "%";
            criticalModifierText.text = StatCalculator.GetTotalCriticalModifier(character).ToString() + "%";
            initiativeText.text = StatCalculator.GetTotalInitiative(character).ToString();
            visionText.text = StatCalculator.GetTotalVision(character).ToString();

            physicalResistanceText.text = StatCalculator.GetTotalPhysicalResistance(character).ToString();
            magicResistanceText.text = StatCalculator.GetTotalMagicResistance(character).ToString();
            injuryResistanceText.text = StatCalculator.GetTotalInjuryResistance(character).ToString();
            debuffResistanceText.text = StatCalculator.GetTotalDebuffResistance(character).ToString();
        }
        private void BuildStars(GameObject[] arr, int starCount)
        {
            // Reset
            for (int i = 0; i < arr.Length; i++)
                arr[i].gameObject.SetActive(false);

            for (int i = 0; i < starCount; i++)
                arr[i].gameObject.SetActive(true);
        }
        private void BuildCharacterViewPanelModel(HexCharacterData character)
        {
            CharacterModeller.BuildModelFromStringReferences(characterPanelUcm, character.modelParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(character.itemSet, characterPanelUcm);
            characterPanelUcm.SetIdleAnim();
        }
        private void BuildAbilitiesSection(HexCharacterData character)
        {
            Debug.Log("CharacterRosterViewController.BuildAbilitiesSection() called...");

            // reset ability buttons
            foreach (UIAbilityIcon b in abilityIcons)
                b.HideAndReset();

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