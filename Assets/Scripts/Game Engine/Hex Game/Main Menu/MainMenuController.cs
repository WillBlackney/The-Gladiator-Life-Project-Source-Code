using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WeAreGladiators.Abilities;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.GameOrigin;
using WeAreGladiators.Items;
using WeAreGladiators.Perks;
using WeAreGladiators.Persistency;
using WeAreGladiators.UCM;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.MainMenu
{
    public class MainMenuController : Singleton<MainMenuController>
    {
        // Properties + Components
        #region

        [Header("Front Screen Components")]
        [SerializeField] private GameObject frontScreenParent;
        [SerializeField] private GameObject frontScreenBgParent;
        [SerializeField] private CanvasGroup frontScreenGuiCg;
        [SerializeField] private GameObject mainMenuButtonsParent;
        [SerializeField] private GameObject newGameButtonParent;
        [SerializeField] private GameObject continueButtonParent;
        [SerializeField] private GameObject abandonRunButtonParent;
        [SerializeField] private GameObject abandonRunPopupParent;
        [SerializeField] private CrowdRowAnimator[] frontScreenCrowdRows;
        [Space(20)]
        [Header("Settings Screen Components")]
        [SerializeField]
        private GameObject settingsScreenVisualParent;
        [SerializeField] private CanvasGroup settingsScreenContentCg;
        [SerializeField] private Image settingsScreenBlackUnderlay;
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private RectTransform settingsScreenOnScreenPosition;
        [SerializeField] private RectTransform settingsScreenOffScreenPosition;
        [SerializeField] private RectTransform settingsScreenMovementParent;

        [Header("In Game Menu Components")]
        [SerializeField] private GameObject inGameMenuScreenParent;
        [SerializeField] private CanvasGroup inGameMenuScreenCg;
        [Space(40)]
        [Title("Custom Character Screen Components")]
        [SerializeField]
        private UniversalCharacterModel customCharacterScreenUCM;
        [SerializeField] private GameObject chooseCharacterScreenVisualParent;
        [SerializeField] private CharacterModelTemplateBasket[] modelTemplateBaskets;
        [Space(20)]
        [Header("Custom Character Screen Data")]
        [SerializeField]
        private SerializedAttrbuteSheet baselineAttributes;
        [SerializeField] private int maxAllowedAttributePoints = 15;
        [SerializeField] private int individualAttributeBoostLimit = 10;
        [Space(20)]
        [Header("Custom Character Screen Header Tab Refs")]
        [SerializeField]
        private Sprite tabSelectedSprite;
        [SerializeField] private Sprite tabUnselectedSprite;
        [SerializeField] private Color tabSelectedFontColour;
        [SerializeField] private Color tabUnselectedFontColour;
        [SerializeField] private Image[] headerTabImages;
        [SerializeField] private TextMeshProUGUI[] headerTabTexts;
        [Space(20)]
        [Header("Custom Character Screen Pages")]
        [SerializeField]
        private GameObject ccsOriginPanel;
        [SerializeField] private GameObject ccsPresetPanel;
        [SerializeField] private GameObject ccsItemsPanel;
        [SerializeField] private GameObject ccsPerkPanel;
        [SerializeField] private GameObject ccsAbilityPanel;
        [SerializeField] private GameObject ccsTalentPanel;

        [Space(20)]
        [Header("Perk Panel Components")]
        [SerializeField]
        private List<CustomCharacterChoosePerkPanel> allChoosePerkPanels;
        [SerializeField] private Transform choosePerkPanelsParent;
        [SerializeField] private GameObject choosePerkPanelPrefab;
        [SerializeField] private TextMeshProUGUI availablePerkPointsText;

        [Space(20)]
        [Header("Origin Panel Components")]
        [SerializeField] private TMP_InputField characterNameInputField;
        [SerializeField] private TextMeshProUGUI originPanelRacialNameText;
        [SerializeField] private UIRaceIcon originPanelRacialIcon;
        [SerializeField] private TextMeshProUGUI originPanelRacialDescriptionText;
        [SerializeField] private TextMeshProUGUI originPanelPresetNameText;
        [SerializeField] private UIAbilityIcon[] originPanelAbilityIcons;
        [SerializeField] private UITalentRow[] originPanelTalentRows;
        [Space(20)]
        [Header("Preset Panel Components")]
        [SerializeField] private TextMeshProUGUI presetPanelAttributePointsText;
        [SerializeField] private CustomCharacterAttributeRow[] presetPanelAttributeRows;
        [SerializeField] private UIAbilityIcon[] presetPanelAbilityIcons;
        [SerializeField] private UITalentIcon[] presetPanelTalentIcons;
        [Space(20)]
        [Header("Choose Ability Panel Components")]
        [SerializeField] private RectTransform[] chooseAbilityPanelLayouts;
        [SerializeField] private TextMeshProUGUI totalChosenAbilitiesText;
        [SerializeField] private TextMeshProUGUI chooseAbilityButtonsHeaderTextOne;
        [SerializeField] private ChooseAbilityButton[] chooseAbilityButtonsSectionOne;
        [SerializeField] private TextMeshProUGUI chooseAbilityButtonsHeaderTextTwo;
        [SerializeField] private ChooseAbilityButton[] chooseAbilityButtonsSectionTwo;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI totalChosenTalentsText;
        [SerializeField] private ChooseTalentButton[] chooseTalentButtons;

        [Space(20)]
        [Header("Items Panel Components")]
        [SerializeField] private TextMeshProUGUI currentBodyModelText;
        [SerializeField] private ItemDataSO[] allStartingHeadItems;
        [SerializeField] private ItemDataSO[] allStartingBodyItems;
        [SerializeField] private ItemDataSO[] allStartingMainHandItems;
        [SerializeField] private ItemDataSO[] allStartingOffHandItems;
        [SerializeField] private UIAbilityIcon[] itemsPanelAbilityIcons;
        [SerializeField] private CustomItemIcon mainHandItemIcon;
        [SerializeField] private CustomItemIcon offHandItemIcon;

        [Space(20)]
        [Header("Character Validty Components")]
        [SerializeField]
        private GameObject validityModalParent;
        [SerializeField] private GameObject[] reasonRows;
        [SerializeField] private GameObject invalidNameRow;
        [SerializeField] private GameObject unusedPerkRow;
        [SerializeField] private GameObject unusedTalentsRow;
        [SerializeField] private GameObject unusedAbilitiesRow;
        [SerializeField] private GameObject unusedAttributesRow;

        // Non inspector proerties
        private HexCharacterData currentPreset;
        private RaceDataSO currentCustomCharacterRace;
        private CharacterModelTemplateSO currentModelTemplate;
        private readonly List<PerkIconData> starterLevelUpPerks = new List<PerkIconData>();
        private int availableChoosePerkPoints;
        private const int totalAllowedPerkChoices = 1;

        private void Start()
        {
            RenderMenuButtons();
        }

        #endregion

        // Init
        #region

        private Resolution[] resolutions;
        protected override void Awake()
        {
            base.Awake();
            BuildResolutionsDropdown();
        }

        #endregion

        // Getters + Accessors
        #region

        public HexCharacterData CharacterBuild { get; private set; }
        public GameObject InGameMenuScreenParent => inGameMenuScreenParent;

        #endregion

        // Custom Character Validity Logic
        #region

        private void BuildAndShowValidityModal(List<string> validationErrors)
        {
            validityModalParent.SetActive(true);
            reasonRows.ForEach(i => i.gameObject.SetActive(false));
            if (validationErrors.Contains("Name"))
            {
                invalidNameRow.SetActive(true);
            }
            if (validationErrors.Contains("Perk"))
            {
                unusedPerkRow.SetActive(true);
            }
            if (validationErrors.Contains("Talents"))
            {
                unusedTalentsRow.SetActive(true);
            }
            if (validationErrors.Contains("Abilities"))
            {
                unusedAbilitiesRow.SetActive(true);
            }
            if (validationErrors.Contains("Attributes"))
            {
                unusedAttributesRow.SetActive(true);
            }
        }
        private void HideValidityModal()
        {
            validityModalParent.SetActive(false);
        }
        public void OnValidityModalBackButtonClicked()
        {
            HideValidityModal();
        }
        private List<string> GetValidationErrors()
        {
            List<string> validationErrors = new List<string>();
            if (characterNameInputField.text.Length < 2)
            {
                validationErrors.Add("Name");
            }
            if (availableChoosePerkPoints > 0)
            {
                validationErrors.Add("Perk");
            }
            if (CharacterBuild.talentPairings.Count < 2)
            {
                validationErrors.Add("Talents");
            }
            if (CharacterBuild.abilityBook.GetAllKnownNonItemSetAbilities().Count < 3)
            {
                validationErrors.Add("Abilities");
            }
            if (GetTotalAttributePointsSpent() < maxAllowedAttributePoints)
            {
                validationErrors.Add("Attributes");
            }
            return validationErrors;
        }

        #endregion

        // On Button Click Events
        #region

        public void OnStartGameButtonClicked()
        {
            // TO DO: validate selections
            List<string> errors = GetValidationErrors();

            // Start process
            if (errors.Count == 0)
            {
                GameController.Instance.HandleStartNewGameFromMainMenuEvent();
            }
            else
            {
                BuildAndShowValidityModal(errors);
            }
        }
        public void OnBackToMainMenuButtonClicked()
        {
            BlackScreenController.Instance.FadeOutAndBackIn(0.5f, 0.25f, 0.5f, () =>
            {
                HideChooseCharacterScreen();
                ShowFrontScreen();
            });
        }
        public void OnMenuNewGameButtonClicked()
        {
            EventSystem.current.SetSelectedGameObject(null);
            GameOriginController.Instance.ShowOriginScreen();
        }
        public void OnMenuContinueButtonClicked()
        {
            EventSystem.current.SetSelectedGameObject(null);
            GameController.Instance.HandleLoadSavedGameFromMainMenuEvent();
        }
        public void OnMenuSettingsButtonClicked()
        {
            EventSystem.current.SetSelectedGameObject(null);
            ShowSettingsScreen();

        }
        public void OnMenuQuitButtonClicked()
        {
            Application.Quit();
        }
        public void OnMenuAbandonRunButtonClicked()
        {
            ShowAbandonRunPopup();
        }
        public void OnAbandonPopupAbandonRunButtonClicked()
        {
            PersistencyController.Instance.DeleteSaveFileOnDisk();
            RenderMenuButtons();
            HideAbandonRunPopup();
        }
        public void OnAbandonPopupCancelButtonClicked()
        {
            HideAbandonRunPopup();
        }
        public void OnTopBarSettingsButtonClicked()
        {
            if (inGameMenuScreenParent.activeSelf)
            {
                HideInGameMenuView();
            }
            else
            {
                ShowInGameMenuView();
            }

        }
        public void OnInGameBackToGameButtonClicked()
        {
            EventSystem.current.SetSelectedGameObject(null);
            HideInGameMenuView();
        }
        public void OnInGameSettingsButtonClicked()
        {
            EventSystem.current.SetSelectedGameObject(null);
            ShowSettingsScreen();
        }
        public void OnInGameSaveAndQuitClicked()
        {
            EventSystem.current.SetSelectedGameObject(null);
            GameController.Instance.HandleQuitToMainMenuFromInGame();
        }

        #endregion

        // Custom Character Screen Logic : General
        #region

        public void ShowChooseCharacterScreen()
        {
            chooseCharacterScreenVisualParent.SetActive(true);
        }
        public void HideChooseCharacterScreen()
        {
            chooseCharacterScreenVisualParent.SetActive(false);
        }
        public void SetCustomCharacterDataDefaultState()
        {
            // Set up modular character data
            HexCharacterData startingClassTemplate = CharacterDataController.Instance.AllCustomCharacterTemplates[0];
            CharacterBuild = CharacterDataController.Instance.CloneCharacterData(startingClassTemplate);
            CharacterBuild.attributeSheet = new AttributeSheet();
            CharacterBuild.PerkTree = new PerkTreeData();
            baselineAttributes.CopyValuesIntoOther(CharacterBuild.attributeSheet);
            CharacterBuild.mySubName = "The Kid";
            CharacterBuild.background = CharacterDataController.Instance.GetBackgroundData(CharacterBackground.TheKid);
            CharacterBuild.dailyWage = RandomGenerator.NumberBetween(CharacterBuild.background.dailyWageMin, CharacterBuild.background.dailyWageMax);
            HandleChangeClassPreset(startingClassTemplate);

            // Start at level 2
            CharacterBuild.currentLevel = 2;
            CharacterBuild.currentMaxXP = CharacterDataController.Instance.GetMaxXpCapForLevel(2);

            // Set to template 1 
            ResetHeaderTabColourStates();
            RebuildAndShowOriginPageView();

            // Set to race 1
            HandleChangeRace(CharacterDataController.Instance.PlayableRaces[0]);
        }
        private void HandleChangeClassPreset(HexCharacterData preset)
        {
            // Update cached preset
            currentPreset = preset;

            // Update abilities
            CharacterBuild.abilityBook = new AbilityBook(preset.abilityBook);

            // Update talents
            CharacterBuild.talentPairings.Clear();
            CharacterBuild.talentPairings.AddRange(preset.talentPairings);

            // Update attributes
            preset.attributeSheet.CopyValuesIntoOther(CharacterBuild.attributeSheet);

            // Update perks
            PerkController.Instance.BuildPassiveManagerFromOtherPassiveManager(preset.passiveManager, CharacterBuild.passiveManager);

            int maxHealth = StatCalculator.GetTotalMaxHealth(CharacterBuild);
            CharacterDataController.Instance.OnConstitutionOrMaxHealthChanged(CharacterBuild, maxHealth);
            CharacterDataController.Instance.SetCharacterHealth(CharacterBuild, maxHealth);

            // Update weapons + clothing items
            HandleSetItemsFromPreset(preset);

            // Rebuild Origin page
            RebuildAndShowOriginPageView();
        }
        private void CloseAllCustomCharacterScreenPanels()
        {
            ccsOriginPanel.SetActive(false);
            ccsPresetPanel.SetActive(false);
            ccsItemsPanel.SetActive(false);
            ccsPerkPanel.SetActive(false);
            ccsAbilityPanel.SetActive(false);
            ccsTalentPanel.SetActive(false);
        }

        #endregion

        // Custom Character Screen Logic : Header nav tabs
        #region

        public void OnOriginHeaderTabClicked()
        {
            CloseAllCustomCharacterScreenPanels();
            ResetHeaderTabColourStates();

            headerTabImages[0].sprite = tabSelectedSprite;
            headerTabTexts[0].color = tabSelectedFontColour;
            RebuildAndShowOriginPageView();
        }
        public void OnPresetHeaderTabClicked()
        {
            CloseAllCustomCharacterScreenPanels();
            ResetHeaderTabColourStates();

            headerTabImages[1].sprite = tabSelectedSprite;
            headerTabTexts[1].color = tabSelectedFontColour;

            RebuildAndShowPresetPanel();
        }
        public void OnItemsHeaderTabClicked()
        {
            CloseAllCustomCharacterScreenPanels();
            ResetHeaderTabColourStates();

            headerTabImages[2].sprite = tabSelectedSprite;
            headerTabTexts[2].color = tabSelectedFontColour;

            RebuildAndShowItemsPanel();
        }
        public void OnPerksHeaderTabClicked()
        {
            CloseAllCustomCharacterScreenPanels();
            ResetHeaderTabColourStates();
            headerTabImages[3].sprite = tabSelectedSprite;
            headerTabTexts[3].color = tabSelectedFontColour;
            RebuildAndShowPerkPanel();
        }
        private void ResetHeaderTabColourStates()
        {
            for (int i = 0; i < headerTabImages.Length; i++)
            {
                headerTabImages[i].sprite = tabUnselectedSprite;
                headerTabTexts[i].color = tabUnselectedFontColour;
            }
        }

        #endregion

        // Custom Character Screen Logic : Origin Panel
        #region

        private void RebuildAndShowOriginPageView()
        {
            // Route to page
            CloseAllCustomCharacterScreenPanels();
            ccsOriginPanel.SetActive(true);

            ResetHeaderTabColourStates();
            headerTabImages[0].sprite = tabSelectedSprite;
            headerTabTexts[0].color = tabSelectedFontColour;

            // Set name text
            originPanelPresetNameText.text = currentPreset.myName;

            // Reset and build ability icons
            List<AbilityData> nonItemAbilities = CharacterBuild.abilityBook.GetAllKnownNonItemSetAbilities();
            foreach (UIAbilityIcon a in originPanelAbilityIcons)
            {
                a.HideAndReset();
            }
            for (int i = 0; i < nonItemAbilities.Count && i < originPanelAbilityIcons.Length; i++)
            {
                originPanelAbilityIcons[i].BuildFromAbilityData(nonItemAbilities[i]);
            }

            // Reset and build talent rows
            foreach (UITalentRow r in originPanelTalentRows)
            {
                r.HideAndReset();
            }
            for (int i = 0; i < CharacterBuild.talentPairings.Count && i < originPanelTalentRows.Length; i++)
            {
                originPanelTalentRows[i].BuildFromTalentPairing(CharacterBuild.talentPairings[i]);
            }

            customCharacterScreenUCM.SetModeFromItemSet(CharacterBuild.itemSet);

        }
        public void OnNextClassPresetButtonClicked()
        {
            HexCharacterData[] templates = CharacterDataController.Instance.AllCustomCharacterTemplates;
            HexCharacterData nextTemplate = null;

            int currentIndex = Array.IndexOf(templates, currentPreset);
            if (currentIndex == templates.Length - 1)
            {
                nextTemplate = templates[0];
            }

            else
            {
                nextTemplate = templates[currentIndex + 1];
            }

            HandleChangeClassPreset(nextTemplate);
        }
        public void OnPreviousClassPresetButtonClicked()
        {
            HexCharacterData[] templates = CharacterDataController.Instance.AllCustomCharacterTemplates;
            HexCharacterData nextTemplate = null;
            int currentIndex = Array.IndexOf(templates, currentPreset);
            if (currentIndex == 0)
            {
                nextTemplate = templates[templates.Length - 1];
            }

            else
            {
                nextTemplate = templates[currentIndex - 1];
            }

            HandleChangeClassPreset(nextTemplate);
        }
        public void OnNameInputFieldValueChanged()
        {
            CharacterBuild.myName = characterNameInputField.text;
            CharacterBuild.mySubName = "The Kid";
        }

        #endregion

        // Custom Character Screen Logic: Preset Panel
        #region

        private void RebuildAndShowPresetPanel()
        {
            // Show preset panel + hide other panels
            CloseAllCustomCharacterScreenPanels();
            ccsPresetPanel.SetActive(true);

            RebuildAttributeSection();
            RebuildPresetPanelAbilitySection();
            RebuildPresetPanelTalentSection();
        }
        private void RebuildAttributeSection()
        {
            // Rebuild each row
            foreach (CustomCharacterAttributeRow row in presetPanelAttributeRows)
            {
                RebuildAttributeRow(row);
            }

            // Update availble attribute points text
            presetPanelAttributePointsText.text = (maxAllowedAttributePoints - GetTotalAttributePointsSpent()).ToString();

        }
        private void RebuildPresetPanelAbilitySection()
        {
            // Reset and build ability icons
            List<AbilityData> nonItemAbilities = CharacterBuild.abilityBook.GetAllKnownNonItemSetAbilities();
            foreach (UIAbilityIcon a in presetPanelAbilityIcons)
            {
                a.HideAndReset();
            }
            for (int i = 0; i < nonItemAbilities.Count && i < presetPanelAbilityIcons.Length; i++)
            {
                presetPanelAbilityIcons[i].BuildFromAbilityData(nonItemAbilities[i]);
            }

        }
        private void RebuildPresetPanelTalentSection()
        {
            // Reset and build talent rows
            foreach (UITalentIcon r in presetPanelTalentIcons)
            {
                r.HideAndReset();
            }
            for (int i = 0; i < CharacterBuild.talentPairings.Count && i < presetPanelTalentIcons.Length; i++)
            {
                presetPanelTalentIcons[i].BuildFromTalentPairing(CharacterBuild.talentPairings[i]);
            }
        }
        private void RebuildAttributeRow(CustomCharacterAttributeRow row)
        {
            // Calculate stat value + difference from baseline
            int dif = GetCharacterAttributeDifference(row.Attribute);
            int value = GetCharacterAttributeValue(row.Attribute);

            row.AmountText.text = value.ToString();

            // Set plus and minus button view states
            row.MinusButtonParent.SetActive(false);
            row.PlusButtonParent.SetActive(false);
            if (dif > 0)
            {
                row.MinusButtonParent.SetActive(true);
            }
            if (dif < individualAttributeBoostLimit)
            {
                row.PlusButtonParent.SetActive(true);
            }

            // Hide plus button if player already spent all points
            if (GetTotalAttributePointsSpent() >= maxAllowedAttributePoints)
            {
                row.PlusButtonParent.SetActive(false);
            }

            // Set text colouring
            if (dif > 0)
            {
                row.AmountText.color = row.BoostedStatTextColor;
            }
            else
            {
                row.AmountText.color = row.NormalStatTextColor;
            }

        }
        private int GetCharacterAttributeDifference(CoreAttribute att)
        {
            int dif = 0;

            if (att == CoreAttribute.Might)
            {
                dif = CharacterBuild.attributeSheet.might.value - baselineAttributes.might.value;
            }

            else if (att == CoreAttribute.Accuracy)
            {
                dif = CharacterBuild.attributeSheet.accuracy.value - baselineAttributes.accuracy.value;
            }

            else if (att == CoreAttribute.Dodge)
            {
                dif = CharacterBuild.attributeSheet.dodge.value - baselineAttributes.dodge.value;
            }

            else if (att == CoreAttribute.Constitution)
            {
                dif = CharacterBuild.attributeSheet.constitution.value - baselineAttributes.constitution.value;
            }

            else if (att == CoreAttribute.Resolve)
            {
                dif = CharacterBuild.attributeSheet.resolve.value - baselineAttributes.resolve.value;
            }

            else if (att == CoreAttribute.Wits)
            {
                dif = CharacterBuild.attributeSheet.wits.value - baselineAttributes.wits.value;
            }

            Debug.Log("MainMenuController.GetCharacterAttributeDifference() returning " + dif +
                " for attribute: " + att);

            return dif;
        }
        private int GetCharacterAttributeValue(CoreAttribute att)
        {
            int value = 0;

            if (att == CoreAttribute.Might)
            {
                value = CharacterBuild.attributeSheet.might.value;
            }

            else if (att == CoreAttribute.Accuracy)
            {
                value = CharacterBuild.attributeSheet.accuracy.value;
            }

            else if (att == CoreAttribute.Dodge)
            {
                value = CharacterBuild.attributeSheet.dodge.value;
            }

            else if (att == CoreAttribute.Constitution)
            {
                value = CharacterBuild.attributeSheet.constitution.value;
            }

            else if (att == CoreAttribute.Resolve)
            {
                value = CharacterBuild.attributeSheet.resolve.value;
            }

            else if (att == CoreAttribute.Wits)
            {
                value = CharacterBuild.attributeSheet.wits.value;
            }

            Debug.Log("MainMenuController.GetCharacterAttributeValue) returning " + value +
                " for attribute: " + att);

            return value;
        }
        private int GetTotalAttributePointsSpent()
        {
            int dif = 0;
            dif += GetCharacterAttributeDifference(CoreAttribute.Might);
            dif += GetCharacterAttributeDifference(CoreAttribute.Constitution);
            dif += GetCharacterAttributeDifference(CoreAttribute.Accuracy);
            dif += GetCharacterAttributeDifference(CoreAttribute.Dodge);
            dif += GetCharacterAttributeDifference(CoreAttribute.Resolve);
            dif += GetCharacterAttributeDifference(CoreAttribute.Wits);
            dif += GetCharacterAttributeDifference(CoreAttribute.Fitness);
            Debug.Log("MainMenuController.GetTotalAttributePointsSpent() returning: " + dif);
            return dif;
        }
        public void OnDecreaseAttributeButtonClicked(CustomCharacterAttributeRow row)
        {
            if (row.Attribute == CoreAttribute.Might)
            {
                CharacterBuild.attributeSheet.might.value -= 1;
            }
            else if (row.Attribute == CoreAttribute.Accuracy)
            {
                CharacterBuild.attributeSheet.accuracy.value -= 1;
            }
            else if (row.Attribute == CoreAttribute.Dodge)
            {
                CharacterBuild.attributeSheet.dodge.value -= 1;
            }
            else if (row.Attribute == CoreAttribute.Constitution)
            {
                CharacterBuild.attributeSheet.constitution.value -= 1;
            }
            else if (row.Attribute == CoreAttribute.Resolve)
            {
                CharacterBuild.attributeSheet.resolve.value -= 1;
            }
            else if (row.Attribute == CoreAttribute.Wits)
            {
                CharacterBuild.attributeSheet.wits.value -= 1;
            }

            RebuildAttributeSection();
        }
        public void OnIncreaseAttributeButtonClicked(CustomCharacterAttributeRow row)
        {
            if (row.Attribute == CoreAttribute.Might)
            {
                CharacterBuild.attributeSheet.might.value += 1;
            }
            else if (row.Attribute == CoreAttribute.Accuracy)
            {
                CharacterBuild.attributeSheet.accuracy.value += 1;
            }
            else if (row.Attribute == CoreAttribute.Dodge)
            {
                CharacterBuild.attributeSheet.dodge.value += 1;
            }
            else if (row.Attribute == CoreAttribute.Constitution)
            {
                CharacterBuild.attributeSheet.constitution.value += 1;
            }
            else if (row.Attribute == CoreAttribute.Resolve)
            {
                CharacterBuild.attributeSheet.resolve.value += 1;
            }
            else if (row.Attribute == CoreAttribute.Wits)
            {
                CharacterBuild.attributeSheet.wits.value += 1;
            }

            RebuildAttributeSection();
        }

        #endregion

        // Custom Character Screen Logic: Perks Panel
        #region

        private void RebuildAndShowPerkPanel()
        {
            // Show preset panel + hide other panels
            CloseAllCustomCharacterScreenPanels();
            ccsPerkPanel.SetActive(true);
            SetAvailableChoosePerkPoints(totalAllowedPerkChoices);

            // Get and cache all level up perks if havent already
            if (starterLevelUpPerks.Count == 0)
            {
                foreach (PerkIconData p in PerkController.Instance.GetAllPerkTreePerks())
                {
                    if (p.perkTreeTier == 1)
                    {
                        starterLevelUpPerks.Add(p);
                    }
                }
            }

            // Reset all perk panels to default state
            for (int i = 0; i < allChoosePerkPanels.Count; i++)
            {
                allChoosePerkPanels[i].Reset();
            }

            // Build a choose perk panel for each level up perk option
            for (int i = 0; i < starterLevelUpPerks.Count; i++)
            {
                // Create new panels in list if there arent enough to show all the perks
                if (allChoosePerkPanels.Count <= i)
                {
                    CustomCharacterChoosePerkPanel newPanel = Instantiate(choosePerkPanelPrefab, choosePerkPanelsParent).GetComponent<CustomCharacterChoosePerkPanel>();
                    allChoosePerkPanels.Add(newPanel);
                }

                // Build the panel from perk
                CustomCharacterChoosePerkPanel panel = allChoosePerkPanels[i];
                panel.Build(new ActivePerk(starterLevelUpPerks[i].perkTag, 1));

                // If already has the perk, set that perk as selected and deduct a perk choice point.
                if (PerkController.Instance.DoesCharacterHavePerk(CharacterBuild.passiveManager, starterLevelUpPerks[i].perkTag))
                {
                    SetAvailableChoosePerkPoints(availableChoosePerkPoints - 1);
                    panel.SetSelectedViewState(true);
                }

            }
        }
        public void HandleChoosePerkPanelClicked(CustomCharacterChoosePerkPanel panel)
        {
            bool hasPerk = PerkController.Instance.DoesCharacterHavePerk(CharacterBuild.passiveManager, panel.PerkIcon.ActivePerk.perkTag);

            // Already selected: unselect
            if (hasPerk)
            {
                SetAvailableChoosePerkPoints(availableChoosePerkPoints + 1);
                panel.SetSelectedViewState(false);
                PerkController.Instance.ModifyPerkOnCharacterData(CharacterBuild.passiveManager, panel.PerkIcon.ActivePerk.perkTag, -panel.PerkIcon.ActivePerk.stacks);
            }
            // No current selection: make this the new selection.
            else if (!hasPerk && availableChoosePerkPoints > 0)
            {
                SetAvailableChoosePerkPoints(availableChoosePerkPoints - 1);
                panel.SetSelectedViewState(true);
                PerkController.Instance.ModifyPerkOnCharacterData(CharacterBuild.passiveManager, panel.PerkIcon.ActivePerk.perkTag, panel.PerkIcon.ActivePerk.stacks);

            }
            // Already made a selection: unselect current and make this the current selection
            else if (!hasPerk && availableChoosePerkPoints == 0)
            {
                foreach (CustomCharacterChoosePerkPanel pPanel in allChoosePerkPanels)
                {
                    if (PerkController.Instance.DoesCharacterHavePerk(CharacterBuild.passiveManager, pPanel.PerkIcon.ActivePerk.perkTag))
                    {
                        PerkController.Instance.ModifyPerkOnCharacterData(CharacterBuild.passiveManager, pPanel.PerkIcon.ActivePerk.perkTag, -pPanel.PerkIcon.ActivePerk.stacks);
                        pPanel.SetSelectedViewState(false);
                        break;
                    }
                }

                panel.SetSelectedViewState(true);
                PerkController.Instance.ModifyPerkOnCharacterData(CharacterBuild.passiveManager, panel.PerkIcon.ActivePerk.perkTag, panel.PerkIcon.ActivePerk.stacks);
            }
            int newMaxHealth = StatCalculator.GetTotalMaxHealth(CharacterBuild);
            CharacterDataController.Instance.SetCharacterHealth(CharacterBuild, newMaxHealth);
        }
        private void SetAvailableChoosePerkPoints(int amount)
        {
            availableChoosePerkPoints = amount;
            availablePerkPointsText.text = amount.ToString();
        }

        #endregion

        // Custom Character Screen Logic : Items Page
        #region

        private void RebuildAndShowItemsPanel()
        {
            // Route to page
            CloseAllCustomCharacterScreenPanels();
            ccsItemsPanel.SetActive(true);

            // to do: set racial model preset text

            // Reset and build weapon ability icons
            RebuildItemPanelWeaponAbilityIcons();
        }
        private void RebuildItemPanelWeaponAbilityIcons()
        {
            // Reset and build ability icons
            foreach (UIAbilityIcon a in itemsPanelAbilityIcons)
            {
                a.HideAndReset();
            }

            // Determine + create weapon abilities
            List<AbilityData> weaponAbilities = CharacterBuild.abilityBook.GetAbilitiesFromItemSet(CharacterBuild.itemSet);

            // Build icons from weapon abilities
            for (int i = 0; i < weaponAbilities.Count && i < itemsPanelAbilityIcons.Length; i++)
            {
                itemsPanelAbilityIcons[i].BuildFromAbilityData(weaponAbilities[i]);
            }
        }
        private void HandleSetItemsFromPreset(HexCharacterData preset)
        {
            ItemController.Instance.CopyItemManagerDataIntoOtherItemManager(preset.itemSet, CharacterBuild.itemSet);
            CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);

            // Rebuild icon views for items + abilities
            UpdateItemSlotViews();
            RebuildItemPanelWeaponAbilityIcons();
        }
        private void UpdateItemSlotViews()
        {
            //headItemIcon.BuildFromData(characterBuild.itemSet.headArmour);
            if (CharacterBuild.itemSet.headArmour == null)
            {
                CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeHeadWear);
            }
            //bodyItemIcon.BuildFromData(characterBuild.itemSet.bodyArmour);
            mainHandItemIcon.BuildFromData(CharacterBuild.itemSet.mainHandItem);
            offHandItemIcon.BuildFromData(CharacterBuild.itemSet.offHandItem);
            if (CharacterBuild.itemSet.offHandItem == null)
            {
                CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
            }
        }
        public void OnNextHeadItemClicked()
        {

            // Not currently wearing a head item
            if (CharacterBuild.itemSet.headArmour == null)
            {
                ItemData headItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingHeadItems[0].itemName));
                CharacterBuild.itemSet.headArmour = headItem;
                CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                // headItemIcon.BuildFromData(headItem);
            }

            else
            {
                ItemDataSO currentHead = null;
                foreach (ItemDataSO headData in allStartingHeadItems)
                {
                    if (headData.itemName == CharacterBuild.itemSet.headArmour.itemName)
                    {
                        currentHead = headData;
                    }
                }

                int currentIndex = Array.IndexOf(allStartingHeadItems, currentHead);

                // Set no item
                if (currentIndex == allStartingHeadItems.Length - 1)
                {
                    CharacterBuild.itemSet.headArmour = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeHeadWear);
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    //headItemIcon.BuildFromData(null);
                }

                // Set next item
                else
                {
                    ItemData headItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingHeadItems[currentIndex + 1].itemName));
                    CharacterBuild.itemSet.headArmour = headItem;
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    //headItemIcon.BuildFromData(headItem);
                }

            }

        }
        public void OnPreviousHeadItemClicked()
        {
            // Not currently wearing a head item
            if (CharacterBuild.itemSet.headArmour == null)
            {
                ItemData headItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingHeadItems[allStartingHeadItems.Length - 1].itemName));
                CharacterBuild.itemSet.headArmour = headItem;
                CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                //headItemIcon.BuildFromData(headItem);
            }

            else
            {
                ItemDataSO currentHead = null;
                foreach (ItemDataSO headData in allStartingHeadItems)
                {
                    if (headData.itemName == CharacterBuild.itemSet.headArmour.itemName)
                    {
                        currentHead = headData;
                    }
                }
                int currentIndex = Array.IndexOf(allStartingHeadItems, currentHead);

                // Set no item
                if (currentIndex == 0)
                {
                    CharacterBuild.itemSet.headArmour = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeHeadWear);
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    //headItemIcon.BuildFromData(null);
                }
                // Set previous item
                else
                {
                    ItemData headItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingHeadItems[currentIndex - 1].itemName));
                    CharacterBuild.itemSet.headArmour = headItem;
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    //headItemIcon.BuildFromData(headItem);
                }

            }

        }
        public void OnNextBodyItemClicked()
        {
            // Not currently wearing a head item
            if (CharacterBuild.itemSet.bodyArmour == null)
            {
                ItemData bodyItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingBodyItems[0].itemName));
                CharacterBuild.itemSet.bodyArmour = bodyItem;
                CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                //bodyItemIcon.BuildFromData(bodyItem);
            }

            else
            {
                ItemDataSO currentBody = null;
                foreach (ItemDataSO bodyData in allStartingBodyItems)
                {
                    if (bodyData.itemName == CharacterBuild.itemSet.bodyArmour.itemName)
                    {
                        currentBody = bodyData;
                    }
                }

                int currentIndex = Array.IndexOf(allStartingBodyItems, currentBody);

                // Set no item
                if (currentIndex == allStartingBodyItems.Length - 1)
                {
                    CharacterBuild.itemSet.bodyArmour = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeChestWear);
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    //bodyItemIcon.BuildFromData(null);
                }

                // Set next item
                else
                {
                    ItemData bodyItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingBodyItems[currentIndex + 1].itemName));
                    CharacterBuild.itemSet.bodyArmour = bodyItem;
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    //bodyItemIcon.BuildFromData(bodyItem);
                }

            }

        }
        public void OnPreviousBodyItemClicked()
        {
            // Not currently wearing a head item
            if (CharacterBuild.itemSet.bodyArmour == null)
            {
                ItemData bodyItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingBodyItems[allStartingBodyItems.Length - 1].itemName));
                CharacterBuild.itemSet.bodyArmour = bodyItem;
                CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                //bodyItemIcon.BuildFromData(bodyItem);
            }

            else
            {
                ItemDataSO currentBody = null;
                foreach (ItemDataSO bodyData in allStartingBodyItems)
                {
                    if (bodyData.itemName == CharacterBuild.itemSet.bodyArmour.itemName)
                    {
                        currentBody = bodyData;
                    }
                }
                int currentIndex = Array.IndexOf(allStartingBodyItems, currentBody);

                // Set no item
                if (currentIndex == 0)
                {
                    CharacterBuild.itemSet.bodyArmour = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeChestWear);
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    //bodyItemIcon.BuildFromData(null);
                }
                // Set previous item
                else
                {
                    ItemData bodyItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingBodyItems[currentIndex - 1].itemName));
                    CharacterBuild.itemSet.bodyArmour = bodyItem;
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    //bodyItemIcon.BuildFromData(bodyItem);
                }

            }

        }
        public void OnNextMainHandItemClicked()
        {
            if (CharacterBuild.itemSet.mainHandItem == null)
            {
                ItemData item = ItemController.Instance.GetItemDataByName(allStartingMainHandItems[0].itemName);
                if (item != null)
                {
                    item = ItemController.Instance.GenerateNewItemWithRandomEffects(item);
                }
                CharacterBuild.itemSet.mainHandItem = item;
                CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                mainHandItemIcon.BuildFromData(item);

                if (CharacterBuild.itemSet.offHandItem != null && item.handRequirement == HandRequirement.TwoHanded)
                {
                    CharacterBuild.itemSet.offHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
                    offHandItemIcon.BuildFromData(null);
                }
            }
            else
            {

                ItemDataSO currentMH = null;
                foreach (ItemDataSO mainHandItem in allStartingMainHandItems)
                {
                    if (mainHandItem.itemName == CharacterBuild.itemSet.mainHandItem.itemName)
                    {
                        currentMH = mainHandItem;
                    }
                }

                int currentIndex = Array.IndexOf(allStartingMainHandItems, currentMH);

                // Set no item
                if (currentIndex == allStartingMainHandItems.Length - 1)
                {
                    CharacterBuild.itemSet.mainHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeMainHandWeapon);
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    mainHandItemIcon.BuildFromData(null);
                }

                // Set next item
                else
                {
                    ItemData item = ItemController.Instance.GetItemDataByName(allStartingMainHandItems[currentIndex + 1].itemName);
                    if (item != null)
                    {
                        item = ItemController.Instance.GenerateNewItemWithRandomEffects(item);
                    }
                    CharacterBuild.itemSet.mainHandItem = item;
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    mainHandItemIcon.BuildFromData(item);

                    // Remove off hand item when equipping a 2h weapon
                    if (CharacterBuild.itemSet.offHandItem != null && item.handRequirement == HandRequirement.TwoHanded)
                    {
                        CharacterBuild.itemSet.offHandItem = null;
                        CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
                        offHandItemIcon.BuildFromData(null);
                    }
                }
            }
            customCharacterScreenUCM.SetModeFromItemSet(CharacterBuild.itemSet);
            RebuildItemPanelWeaponAbilityIcons();

            // On equip SFX
            if (CharacterBuild.itemSet.mainHandItem != null)
            {
                AudioManager.Instance.PlaySound(CharacterBuild.itemSet.mainHandItem.equipSFX);
            }
        }
        public void OnPreviousMainHandItemClicked()
        {
            if (CharacterBuild.itemSet.mainHandItem == null)
            {
                ItemData item = ItemController.Instance.GetItemDataByName(allStartingMainHandItems[allStartingMainHandItems.Length - 1].itemName);
                if (item != null)
                {
                    item = ItemController.Instance.GenerateNewItemWithRandomEffects(item);
                }
                CharacterBuild.itemSet.mainHandItem = item;
                CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                mainHandItemIcon.BuildFromData(item);

                if (CharacterBuild.itemSet.offHandItem != null && item.handRequirement == HandRequirement.TwoHanded)
                {
                    CharacterBuild.itemSet.offHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
                    offHandItemIcon.BuildFromData(null);
                }
            }

            else
            {
                ItemDataSO currentMH = null;
                foreach (ItemDataSO mhData in allStartingMainHandItems)
                {
                    if (mhData.itemName == CharacterBuild.itemSet.mainHandItem.itemName)
                    {
                        currentMH = mhData;
                    }
                }
                int currentIndex = Array.IndexOf(allStartingMainHandItems, currentMH);

                // Set no item
                if (currentIndex == 0)
                {
                    CharacterBuild.itemSet.mainHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeMainHandWeapon);
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    mainHandItemIcon.BuildFromData(null);
                }
                // Set previous item
                else
                {
                    ItemData mhItem = ItemController.Instance.GetItemDataByName(allStartingMainHandItems[currentIndex - 1].itemName);
                    if (mhItem != null)
                    {
                        mhItem = ItemController.Instance.GenerateNewItemWithRandomEffects(mhItem);
                    }
                    CharacterBuild.itemSet.mainHandItem = mhItem;
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    mainHandItemIcon.BuildFromData(mhItem);

                    // Remove off hand item when equipping a 2h weapon
                    if (CharacterBuild.itemSet.offHandItem != null && mhItem.handRequirement == HandRequirement.TwoHanded)
                    {
                        CharacterBuild.itemSet.offHandItem = null;
                        CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
                        offHandItemIcon.BuildFromData(null);
                    }
                }
            }
            customCharacterScreenUCM.SetModeFromItemSet(CharacterBuild.itemSet);
            RebuildItemPanelWeaponAbilityIcons();

            // On equip SFX
            if (CharacterBuild.itemSet.mainHandItem != null)
            {
                AudioManager.Instance.PlaySound(CharacterBuild.itemSet.mainHandItem.equipSFX);
            }
        }
        public void OnNextOffHandItemClicked()
        {
            if (CharacterBuild.itemSet.mainHandItem != null &&
                CharacterBuild.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
            {
                return;
            }

            if (CharacterBuild.itemSet.offHandItem == null)
            {
                ItemData item = ItemController.Instance.GetItemDataByName(allStartingOffHandItems[0].itemName);
                if (item != null)
                {
                    item = ItemController.Instance.GenerateNewItemWithRandomEffects(item);
                }
                CharacterBuild.itemSet.offHandItem = item;
                CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                offHandItemIcon.BuildFromData(item);

                if (CharacterBuild.itemSet.mainHandItem != null && CharacterBuild.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
                {
                    CharacterBuild.itemSet.mainHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeMainHandWeapon);
                    mainHandItemIcon.BuildFromData(null);
                }
            }

            else
            {
                ItemDataSO currentOH = null;
                foreach (ItemDataSO offhandItem in allStartingOffHandItems)
                {
                    if (offhandItem.itemName == CharacterBuild.itemSet.offHandItem.itemName)
                    {
                        currentOH = offhandItem;
                    }
                }

                int currentIndex = Array.IndexOf(allStartingOffHandItems, currentOH);

                // Set no item
                if (currentIndex == allStartingOffHandItems.Length - 1)
                {
                    CharacterBuild.itemSet.offHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    offHandItemIcon.BuildFromData(null);
                }

                // Set next item
                else
                {
                    ItemData item = ItemController.Instance.GetItemDataByName(allStartingOffHandItems[currentIndex + 1].itemName);
                    if (item != null)
                    {
                        item = ItemController.Instance.GenerateNewItemWithRandomEffects(item);
                    }
                    CharacterBuild.itemSet.offHandItem = item;
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    offHandItemIcon.BuildFromData(item);

                    if (CharacterBuild.itemSet.mainHandItem != null && CharacterBuild.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
                    {
                        CharacterBuild.itemSet.mainHandItem = null;
                        CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeMainHandWeapon);
                        mainHandItemIcon.BuildFromData(null);
                    }
                }

            }
            customCharacterScreenUCM.SetModeFromItemSet(CharacterBuild.itemSet);
            RebuildItemPanelWeaponAbilityIcons();

            // On equip SFX
            if (CharacterBuild.itemSet.offHandItem != null)
            {
                AudioManager.Instance.PlaySound(CharacterBuild.itemSet.offHandItem.equipSFX);
            }
        }
        public void OnPreviousOffHandItemClicked()
        {
            if (CharacterBuild.itemSet.mainHandItem != null &&
                CharacterBuild.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
            {
                return;
            }

            // Not currently using an off hand item
            if (CharacterBuild.itemSet.offHandItem == null)
            {
                ItemData ohItem = ItemController.Instance.GetItemDataByName(allStartingOffHandItems[allStartingOffHandItems.Length - 1].itemName);
                if (ohItem != null)
                {
                    ohItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ohItem);
                }
                CharacterBuild.itemSet.offHandItem = ohItem;
                CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                offHandItemIcon.BuildFromData(ohItem);

                if (CharacterBuild.itemSet.mainHandItem != null && CharacterBuild.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
                {
                    CharacterBuild.itemSet.mainHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeMainHandWeapon);
                    mainHandItemIcon.BuildFromData(null);
                }
            }

            else
            {
                ItemDataSO currentOH = null;
                foreach (ItemDataSO bodyData in allStartingOffHandItems)
                {
                    if (bodyData.itemName == CharacterBuild.itemSet.offHandItem.itemName)
                    {
                        currentOH = bodyData;
                    }
                }
                int currentIndex = Array.IndexOf(allStartingOffHandItems, currentOH);

                // Set no item
                if (currentIndex == 0)
                {
                    CharacterBuild.itemSet.offHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    offHandItemIcon.BuildFromData(null);
                }
                // Set previous item
                else
                {
                    ItemData ohItem = ItemController.Instance.GetItemDataByName(allStartingOffHandItems[currentIndex - 1].itemName);
                    if (ohItem != null)
                    {
                        ohItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ohItem);
                    }
                    CharacterBuild.itemSet.offHandItem = ohItem;
                    CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
                    offHandItemIcon.BuildFromData(ohItem);

                    if (CharacterBuild.itemSet.mainHandItem != null && CharacterBuild.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
                    {
                        CharacterBuild.itemSet.mainHandItem = null;
                        CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeMainHandWeapon);
                        mainHandItemIcon.BuildFromData(null);
                    }
                }

            }
            customCharacterScreenUCM.SetModeFromItemSet(CharacterBuild.itemSet);
            RebuildItemPanelWeaponAbilityIcons();

            // On equip SFX
            if (CharacterBuild.itemSet.offHandItem != null)
            {
                AudioManager.Instance.PlaySound(CharacterBuild.itemSet.offHandItem.equipSFX);
            }
        }
        public void OnNextRacialModelButtonClicked()
        {
            HandleChangeRacialModel(GetNextRacialModel(CharacterBuild.race));
        }
        public void OnPreviousRacialModelButtonClicked()
        {
            HandleChangeRacialModel(GetPreviousRacialModel(CharacterBuild.race));
        }

        #endregion

        // Custom Character Screen Logic : Choose Abilities Sub Screeb
        #region

        public void OnAbilitySectionEditButtonClicked()
        {
            BuildChooseAbilitiesPanel();
            TransformUtils.RebuildLayouts(chooseAbilityPanelLayouts);
        }
        public void OnAbilitySectionConfirmButtonClicked()
        {
            CloseAllCustomCharacterScreenPanels();

            // save ability changes
            List<AbilityData> newAbilities = new List<AbilityData>();
            foreach (ChooseAbilityButton b in GetSelectedAbilities())
            {
                newAbilities.Add(b.AbilityIcon.MyDataRef);
            }
            CharacterBuild.abilityBook.ForgetAllAbilities();
            CharacterBuild.abilityBook.HandleLearnNewAbilities(newAbilities);

            // show preset panel
            RebuildAndShowPresetPanel();
        }
        private void BuildChooseAbilitiesPanel()
        {
            if (CharacterBuild.talentPairings.Count == 0)
            {
                return;
            }

            CloseAllCustomCharacterScreenPanels();
            ccsAbilityPanel.SetActive(true);
            TalentSchool talentSchoolOne = CharacterBuild.talentPairings[0].talentSchool;
            chooseAbilityButtonsHeaderTextOne.text = talentSchoolOne + " Abilities";
            List<AbilityData> allTalentOneAbilities = AbilityController.Instance.GetAllAbilitiesOfTalent(talentSchoolOne);

            // Reset all first
            for (int i = 0; i < chooseAbilityButtonsSectionOne.Length; i++)
            {
                chooseAbilityButtonsSectionOne[i].ResetAndHide();
            }

            // Rebuild
            for (int i = 0; i < allTalentOneAbilities.Count && i < chooseAbilityButtonsSectionOne.Length; i++)
            {
                chooseAbilityButtonsSectionOne[i].BuildAndShow(allTalentOneAbilities[i]);
            }

            // Select buttons based on known abilities of character
            foreach (AbilityData a in CharacterBuild.abilityBook.knownAbilities)
            {
                foreach (ChooseAbilityButton button in chooseAbilityButtonsSectionOne)
                {
                    if (button.AbilityIcon.MyDataRef != null && a.abilityName == button.AbilityIcon.MyDataRef.abilityName)
                    {
                        button.HandleChangeSelectionState(true);
                        break;
                    }
                }
            }

            // Build for 2nd talent (if character has a second talent
            if (CharacterBuild.talentPairings.Count > 1)
            {
                TalentSchool talentSchoolTwo = CharacterBuild.talentPairings[1].talentSchool;
                chooseAbilityButtonsHeaderTextTwo.gameObject.SetActive(true);
                chooseAbilityButtonsHeaderTextTwo.text = talentSchoolTwo + " Abilities";
                List<AbilityData> allTalentTwoAbilities = AbilityController.Instance.GetAllAbilitiesOfTalent(talentSchoolTwo);

                // Reset all first
                for (int i = 0; i < chooseAbilityButtonsSectionTwo.Length; i++)
                {
                    chooseAbilityButtonsSectionTwo[i].ResetAndHide();
                }

                // Rebuild
                for (int i = 0; i < allTalentTwoAbilities.Count && i < chooseAbilityButtonsSectionTwo.Length; i++)
                {
                    chooseAbilityButtonsSectionTwo[i].BuildAndShow(allTalentTwoAbilities[i]);
                }

                // Select buttons based on known abilities of character
                foreach (AbilityData a in CharacterBuild.abilityBook.knownAbilities)
                {
                    foreach (ChooseAbilityButton button in chooseAbilityButtonsSectionTwo)
                    {
                        if (button.AbilityIcon.MyDataRef != null && a.abilityName == button.AbilityIcon.MyDataRef.abilityName)
                        {
                            button.HandleChangeSelectionState(true);
                            break;
                        }
                    }
                }
            }
            else
            {
                // Reset all 
                chooseAbilityButtonsHeaderTextTwo.gameObject.SetActive(false);
                for (int i = 0; i < chooseAbilityButtonsSectionTwo.Length; i++)
                {
                    chooseAbilityButtonsSectionTwo[i].ResetAndHide();
                }
            }

            UpdateChosenAbilitiesText();
        }
        public List<ChooseAbilityButton> GetSelectedAbilities()
        {
            List<ChooseAbilityButton> ret = new List<ChooseAbilityButton>();

            List<ChooseAbilityButton> allButtons = new List<ChooseAbilityButton>();
            allButtons.AddRange(chooseAbilityButtonsSectionOne);
            allButtons.AddRange(chooseAbilityButtonsSectionTwo);

            foreach (ChooseAbilityButton b in allButtons)
            {
                if (b.Selected)
                {
                    ret.Add(b);
                }
            }

            return ret;
        }
        public void UpdateChosenAbilitiesText()
        {
            totalChosenAbilitiesText.text = GetSelectedAbilities().Count + "/3";
        }
        private void HandleUnlearnAbilitiesOnTalentsChanged()
        {
            List<AbilityData> invalidAbilities = new List<AbilityData>();
            List<TalentSchool> newTalents = new List<TalentSchool>();

            // Determine current talents
            foreach (TalentPairing tp in CharacterBuild.talentPairings)
            {
                newTalents.Add(tp.talentSchool);
            }

            // Find invalid abilities
            foreach (AbilityData a in CharacterBuild.abilityBook.knownAbilities)
            {
                if (newTalents.Contains(a.talentRequirementData.talentSchool) == false)
                {
                    invalidAbilities.Add(a);
                }
            }

            // Remove invalid abilities
            foreach (AbilityData a in invalidAbilities)
            {
                CharacterBuild.abilityBook.HandleUnlearnAbility(a.abilityName);
            }
        }

        #endregion

        // Custom Character Screen Logic : Choose Talents Sub Screeb
        #region

        public void BuildChooseTalentsPanel()
        {
            // Reset all first
            for (int i = 0; i < chooseTalentButtons.Length; i++)
            {
                chooseTalentButtons[i].ResetAndHide();
            }

            // Rebuild
            for (int i = 0; i < chooseTalentButtons.Length; i++)
            {
                chooseTalentButtons[i].BuildAndShow();
            }

            // Select buttons based on known abilities of character
            foreach (TalentPairing tp in CharacterBuild.talentPairings)
            {
                foreach (ChooseTalentButton button in chooseTalentButtons)
                {
                    if (tp.talentSchool == button.TalentPairing.talentSchool)
                    {
                        button.HandleChangeSelectionState(true);
                        break;
                    }
                }
            }
            UpdateChosenTalentsText();
        }
        public void OnTalentSectionEditButtonClicked()
        {
            CloseAllCustomCharacterScreenPanels();
            ccsTalentPanel.SetActive(true);
            BuildChooseTalentsPanel();
        }
        public void OnTalentSectionConfirmButtonClicked()
        {
            CloseAllCustomCharacterScreenPanels();

            // Apply and save talent changes
            CharacterBuild.talentPairings.Clear();
            foreach (ChooseTalentButton b in GetSelectedTalents())
            {
                CharacterBuild.talentPairings.Add(new TalentPairing(b.TalentPairing.talentSchool, 1));
            }

            // Unlearn abilities from old talents
            HandleUnlearnAbilitiesOnTalentsChanged();

            // Show preset panel
            RebuildAndShowPresetPanel();
        }
        public List<ChooseTalentButton> GetSelectedTalents()
        {
            List<ChooseTalentButton> ret = new List<ChooseTalentButton>();
            foreach (ChooseTalentButton b in chooseTalentButtons)
            {
                if (b.Selected)
                {
                    ret.Add(b);
                }
            }

            return ret;
        }
        public void UpdateChosenTalentsText()
        {
            totalChosenTalentsText.text = GetSelectedTalents().Count + "/2";
        }

        #endregion

        // Custom Character Screen Logic : Update Model + Race
        #region

        private void HandleChangeRace(CharacterRace race)
        {
            // Get + cache race data
            RaceDataSO racialData = CharacterDataController.Instance.GetRaceData(race);
            currentCustomCharacterRace = racialData;
            CharacterBuild.race = racialData.racialTag;
            CharacterBuild.audioProfile = CharacterDataController.Instance.GetAudioProfileForRace(race);

            // Build race icon image
            originPanelRacialIcon.BuildFromRacialData(racialData);

            // Build racial texts
            originPanelRacialNameText.text = racialData.racialTag.ToString();

            // Rebuild UCM as default model of race
            HandleChangeRacialModel(GetRacialModelBasket(CharacterBuild.race).templates[0]);
        }
        private void HandleChangeRacialModel(CharacterModelTemplateSO newModel)
        {
            currentModelTemplate = newModel;
            CharacterBuild.modelParts = newModel.bodyParts;
            CharacterModeller.BuildModelFromStringReferences(customCharacterScreenUCM, newModel.bodyParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(CharacterBuild.itemSet, customCharacterScreenUCM);
            int index = Array.IndexOf(GetRacialModelBasket(CharacterBuild.race).templates, currentModelTemplate);
            currentBodyModelText.text = "Body " + (index + 1);
        }
        public void OnChooseRaceNextButtonClicked()
        {
            Debug.Log("MainMenuController.OnChooseRaceNextButtonClicked() called...");
            CharacterRace[] playableRaces = CharacterDataController.Instance.PlayableRaces.ToArray();
            CharacterRace nextValidRace = CharacterRace.None;
            int currentIndex = Array.IndexOf(playableRaces, currentCustomCharacterRace.racialTag);

            if (currentIndex == playableRaces.Length - 1)
            {
                nextValidRace = playableRaces[0];
            }

            else
            {
                nextValidRace = playableRaces[currentIndex + 1];
            }

            HandleChangeRace(nextValidRace);

        }
        public void OnChooseRacePreviousButtonClicked()
        {
            Debug.Log("MainMenuController.OnChooseRacePreviousButtonClicked() called...");
            CharacterRace[] playableRaces = CharacterDataController.Instance.PlayableRaces.ToArray();
            CharacterRace nextValidRace = CharacterRace.None;
            int currentIndex = Array.IndexOf(playableRaces, currentCustomCharacterRace.racialTag);

            if (currentIndex == 0)
            {
                nextValidRace = playableRaces[playableRaces.Length - 1];
            }

            else
            {
                nextValidRace = playableRaces[currentIndex - 1];
            }

            HandleChangeRace(nextValidRace);
        }
        private CharacterModelTemplateBasket GetRacialModelBasket(CharacterRace race)
        {
            CharacterModelTemplateBasket ret = null;

            foreach (CharacterModelTemplateBasket basket in modelTemplateBaskets)
            {
                if (basket.race == race)
                {
                    ret = basket;
                    break;
                }
            }

            return ret;

        }
        private CharacterModelTemplateSO GetNextRacialModel(CharacterRace race)
        {
            CharacterModelTemplateSO ret = null;

            foreach (CharacterModelTemplateBasket b in modelTemplateBaskets)
            {
                if (b.race == race)
                {
                    int index = Array.IndexOf(b.templates, currentModelTemplate);
                    if (index == b.templates.Length - 1)
                    {
                        ret = b.templates[0];
                    }
                    else
                    {
                        ret = b.templates[index + 1];
                    }

                    break;
                }
            }

            return ret;
        }
        private CharacterModelTemplateSO GetPreviousRacialModel(CharacterRace race)
        {
            CharacterModelTemplateSO ret = null;

            foreach (CharacterModelTemplateBasket b in modelTemplateBaskets)
            {
                if (b.race == race)
                {
                    int index = Array.IndexOf(b.templates, currentModelTemplate);
                    if (index == 0)
                    {
                        ret = b.templates[b.templates.Length - 1];
                    }
                    else
                    {
                        ret = b.templates[index - 1];
                    }

                    break;
                }
            }

            return ret;
        }

        #endregion

        // Front Screen Logic
        #region

        public void RenderMenuButtons()
        {
            AutoSetAbandonRunButtonViewState();
            AutoSetContinueButtonViewState();
            AutoSetNewGameButtonState();
        }
        public void ShowFrontScreen()
        {
            frontScreenParent.SetActive(true);
            foreach (CrowdRowAnimator cra in frontScreenCrowdRows)
            {
                cra.StopAnimation();
                cra.PlayAnimation();
            }
        }
        public void HideFrontScreen()
        {
            frontScreenParent.SetActive(false);
            foreach (CrowdRowAnimator cra in frontScreenCrowdRows)
            {
                cra.StopAnimation();
            }
        }
        private bool ShouldShowContinueButton()
        {
            return PersistencyController.Instance.DoesSaveFileExist();
        }
        private bool ShouldShowNewGameButton()
        {
            return PersistencyController.Instance.DoesSaveFileExist() == false;
        }
        private void AutoSetContinueButtonViewState()
        {
            if (ShouldShowContinueButton())
            {
                ShowContinueButton();
            }
            else
            {
                HideContinueButton();
            }
        }
        private void AutoSetNewGameButtonState()
        {
            if (ShouldShowNewGameButton())
            {
                ShowNewGameButton();
            }
            else
            {
                HideNewGameButton();
            }
        }
        private void AutoSetAbandonRunButtonViewState()
        {
            if (ShouldShowContinueButton())
            {
                ShowAbandonRunButton();
            }
            else
            {
                HideAbandonRunButton();
            }
        }
        private void ShowNewGameButton()
        {
            newGameButtonParent.SetActive(true);
        }
        private void HideNewGameButton()
        {
            newGameButtonParent.SetActive(false);
        }
        private void ShowContinueButton()
        {
            continueButtonParent.SetActive(true);
        }
        private void HideContinueButton()
        {
            continueButtonParent.SetActive(false);
        }
        private void ShowAbandonRunButton()
        {
            abandonRunButtonParent.SetActive(true);
        }
        private void HideAbandonRunButton()
        {
            abandonRunButtonParent.SetActive(false);
        }
        private void ShowAbandonRunPopup()
        {
            abandonRunPopupParent.SetActive(true);
        }
        private void HideAbandonRunPopup()
        {
            abandonRunPopupParent.SetActive(false);
        }

        #endregion

        // Settings Screen Logic
        #region

        public void ShowSettingsScreen()
        {
            settingsScreenVisualParent.SetActive(true);
            settingsScreenContentCg.DOKill();
            settingsScreenContentCg.interactable = false;
            settingsScreenBlackUnderlay.DOKill();
            settingsScreenBlackUnderlay.DOFade(0f, 0f);
            settingsScreenMovementParent.DOKill();
            settingsScreenMovementParent.position = settingsScreenOffScreenPosition.position;

            // to do: build content to default state

            settingsScreenBlackUnderlay.DOFade(0.5f, 0.5f);
            settingsScreenMovementParent.DOMove(settingsScreenOnScreenPosition.position, 0.65f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                settingsScreenContentCg.interactable = true;
            });
        }
        private void HideSettingsScreen(float speed = 0f)
        {
            settingsScreenVisualParent.SetActive(true);
            settingsScreenContentCg.DOKill();
            settingsScreenContentCg.interactable = false;
            settingsScreenBlackUnderlay.DOKill();
            settingsScreenMovementParent.DOKill();

            // to do: build content to default state

            settingsScreenBlackUnderlay.DOFade(0f, speed * 0.75f);
            settingsScreenMovementParent.DOMove(settingsScreenOffScreenPosition.position, speed).SetEase(Ease.InBack).OnComplete(() =>
            {
                settingsScreenVisualParent.SetActive(false);
            });
        }
        public void OnSettingsPageCloseButtonClicked()
        {
            HideSettingsScreen(0.65f);
        }
        public void SetQuality(int quality)
        {
            QualitySettings.SetQualityLevel(quality);
        }
        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }
        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
        private void BuildResolutionsDropdown()
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        #endregion

        // In Game Menu Logic
        #region

        public void ShowInGameMenuView()
        {
            inGameMenuScreenCg.alpha = 0;
            inGameMenuScreenParent.SetActive(true);
            inGameMenuScreenCg.DOFade(1, 0.25f);
        }
        public void HideInGameMenuView()
        {
            inGameMenuScreenParent.SetActive(false);
            inGameMenuScreenCg.alpha = 0;
        }

        #endregion

        // Animations + Sequences
        #region

        public void DoGameStartMainMenuRevealSequence()
        {
            StartCoroutine(DoGameStartMainMenuRevealSequenceCoroutine());
        }
        private IEnumerator DoGameStartMainMenuRevealSequenceCoroutine()
        {
            mainMenuButtonsParent.SetActive(false);
            ShowFrontScreen();

            frontScreenGuiCg.alpha = 0;
            frontScreenBgParent.transform.DOScale(1.25f, 0f);
            AudioManager.Instance.PlayMainMenuMusic();
            yield return new WaitForSeconds(0.5f);

            BlackScreenController.Instance.FadeInScreen(1.5f);
            yield return new WaitForSeconds(1.5f);

            Action showMenu = () =>
            {
                frontScreenBgParent.transform.DOKill();
                frontScreenBgParent.transform.DOScale(1f, 1.5f).SetEase(Ease.InOutQuad);
                DelayUtils.DelayedCall(1f, () =>
                {
                    mainMenuButtonsParent.SetActive(true);
                    frontScreenGuiCg.DOFade(1f, 1f).SetEase(Ease.OutCubic);
                });
            };

            // Alpha warning
            if (!AlphaWarningController.Instance.HasShownWarningThisSession && GlobalSettings.Instance.ShowAlphaWarning)
            {
                AlphaWarningController.Instance.ShowWarningPage(showMenu);
            }
            else
            {
                showMenu.Invoke();
            }
        }

        #endregion
    }
}
