using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Characters;
using System;
using Sirenix.OdinInspector;
using HexGameEngine.Persistency;
using DG.Tweening;
using UnityEngine.EventSystems;
using HexGameEngine.Audio;
using UnityEngine.UI;
using TMPro;
using HexGameEngine.UI;
using HexGameEngine.UCM;
using CardGameEngine.UCM;
using HexGameEngine.Abilities;
using HexGameEngine.Items;
using HexGameEngine.Perks;

namespace HexGameEngine.MainMenu
{
    public class MainMenuController : Singleton<MainMenuController>
    {
        // Properties + Components
        #region
        [Header("Front Screen Components")]
        [SerializeField] private GameObject frontScreenParent;
        [SerializeField] private GameObject frontScreenBgParent;
        [SerializeField] private CanvasGroup frontScreenGuiCg;
        [SerializeField] private GameObject newGameButtonParent;
        [SerializeField] private GameObject continueButtonParent;
        [SerializeField] private GameObject abandonRunButtonParent;
        [SerializeField] private GameObject abandonRunPopupParent;
        [SerializeField] private CrowdRowAnimator[] frontScreenCrowdRows;
        [Space(20)]

        [Header("In Game Menu Components")]
        [SerializeField] private GameObject inGameMenuScreenParent;
        [SerializeField] private CanvasGroup inGameMenuScreenCg;
        [Space(40)]
        [Title("Custom Character Screen Components")]    
        [SerializeField] UniversalCharacterModel customCharacterScreenUCM;
        [SerializeField] GameObject chooseCharacterScreenVisualParent;
        [SerializeField] CharacterModelTemplateBasket[] modelTemplateBaskets;
        [Space(20)]
        [Header("Custom Character Screen Data")]
        [SerializeField] SerializedAttrbuteSheet baselineAttributes;
        [SerializeField] int maxAllowedAttributePoints = 15;
        [SerializeField] int individualAttributeBoostLimit = 10;
        [Space(20)]
        [Header("Custom Character Screen Header Tab Refs")]
        [SerializeField] Sprite tabSelectedSprite;
        [SerializeField] Sprite tabUnselectedSprite;
        [SerializeField] Color tabSelectedFontColour;
        [SerializeField] Color tabUnselectedFontColour;
        [SerializeField] Image[] headerTabImages;
        [SerializeField] TextMeshProUGUI[] headerTabTexts;
        [Space(20)]
        [Header("Custom Character Screen Pages")]
        [SerializeField] GameObject ccsOriginPanel;
        [SerializeField] GameObject ccsPresetPanel;
        [SerializeField] GameObject ccsItemsPanel;
        [SerializeField] GameObject ccsPerkPanel;
        [SerializeField] GameObject ccsAbilityPanel;
        [SerializeField] GameObject ccsTalentPanel;

        [Space(20)]
        [Header("Perk Panel Components")]
        [SerializeField] List<CustomCharacterChoosePerkPanel> allChoosePerkPanels;
        [SerializeField] Transform choosePerkPanelsParent;
        [SerializeField] GameObject choosePerkPanelPrefab;
        [SerializeField] TextMeshProUGUI availablePerkPointsText;

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
        [SerializeField] private CustomItemIcon headItemIcon;
        [SerializeField] private CustomItemIcon bodyItemIcon;
        [SerializeField] private CustomItemIcon mainHandItemIcon;
        [SerializeField] private CustomItemIcon offHandItemIcon;

        // Non inspector proerties
        private HexCharacterData characterBuild;
        private HexCharacterData currentPreset;
        private RaceDataSO currentCustomCharacterRace;
        private CharacterModelTemplateSO currentModelTemplate;
        private List<PerkIconData> allLevelUpPerks = new List<PerkIconData>();
        private int availableChoosePerkPoints = 0;
        private const int totalAllowedPerkChoices = 1;

        private void Start()
        {
            RenderMenuButtons();
        }
        #endregion

        // Getters + Accessors
        #region
        public HexCharacterData CharacterBuild
        {
            get { return characterBuild; }
        }
        #endregion

        // On Button Click Events
        #region
        public void OnStartGameButtonClicked()
        {
            // TO DO: validate selections

            // Start process
            GameController.Instance.HandleStartNewGameFromMainMenuEvent();
        }
        public void OnBackToMainMenuButtonClicked()
        {
            BlackScreenController.Instance.FadeOutAndBackIn(0.5f, 0f, 0.5f, () =>
            {
                HideChooseCharacterScreen();
                ShowFrontScreen();
            });
        }
        public void OnMenuNewGameButtonClicked()
        {
            // disable button highlight
            EventSystem.current.SetSelectedGameObject(null);
            BlackScreenController.Instance.FadeOutAndBackIn(0.5f, 0f, 0.5f, () =>
            {
                ShowChooseCharacterScreen();
                HideFrontScreen();
            });
        }
        public void OnMenuContinueButtonClicked()
        {
            // disable button highlight
            EventSystem.current.SetSelectedGameObject(null);
            GameController.Instance.HandleLoadSavedGameFromMainMenuEvent();
        }
        public void OnMenuSettingsButtonClicked()
        {
            // disable button highlight
            EventSystem.current.SetSelectedGameObject(null);
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
        public void OnInGameSaveAndQuitClicked()
        {
            EventSystem.current.SetSelectedGameObject(null);
            GameController.Instance.HandleQuitToMainMenuFromInGame();
        }
        #endregion

        // Custom Character Screen Logic : General
        #region
        private void ShowChooseCharacterScreen()
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
            characterBuild = CharacterDataController.Instance.CloneCharacterData(startingClassTemplate);
            characterBuild.attributeSheet = new AttributeSheet();
            characterBuild.PerkTree = new PerkTreeData();
            baselineAttributes.CopyValuesIntoOther(characterBuild.attributeSheet);
            characterBuild.background = CharacterDataController.Instance.GetBackgroundData(CharacterBackground.Unknown);
            HandleChangeClassPreset(startingClassTemplate);

            // Set to template 1 
            RebuildAndShowOriginPageView();

            // Set to race 1
            HandleChangeRace(CharacterDataController.Instance.PlayableRaces[0]);
        }
        private void HandleChangeClassPreset(HexCharacterData preset)
        {
            // Update cached preset
            currentPreset = preset;

            // Update abilities
            characterBuild.abilityBook = new AbilityBook(preset.abilityBook);

            // Update talents
            characterBuild.talentPairings.Clear();
            characterBuild.talentPairings.AddRange(preset.talentPairings);

            // Update attributes
            preset.attributeSheet.CopyValuesIntoOther(characterBuild.attributeSheet);

            // Update perks
            PerkController.Instance.BuildPassiveManagerFromOtherPassiveManager(preset.passiveManager, characterBuild.passiveManager);

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

            // Reset 'selected' visual state on all tabs
            for (int i = 0; i < headerTabImages.Length; i++)
            {
                headerTabImages[i].sprite = tabUnselectedSprite;
                headerTabTexts[i].color = tabUnselectedFontColour;
            }

            headerTabImages[0].sprite = tabSelectedSprite;
            headerTabTexts[0].color = tabSelectedFontColour;
            RebuildAndShowOriginPageView();
        }
        public void OnPresetHeaderTabClicked()
        {
            CloseAllCustomCharacterScreenPanels();

            // Reset 'selected' visual state on all tabs
            for (int i = 0; i < headerTabImages.Length; i++)
            {
                headerTabImages[i].sprite = tabUnselectedSprite;
                headerTabTexts[i].color = tabUnselectedFontColour;
            }

            headerTabImages[1].sprite = tabSelectedSprite;
            headerTabTexts[1].color = tabSelectedFontColour;

            RebuildAndShowPresetPanel();
        }
        public void OnItemsHeaderTabClicked()
        {
            CloseAllCustomCharacterScreenPanels();

            // Reset 'selected' visual state on all tabs
            for (int i = 0; i < headerTabImages.Length; i++)
            {
                headerTabImages[i].sprite = tabUnselectedSprite;
                headerTabTexts[i].color = tabUnselectedFontColour;
            }

            headerTabImages[2].sprite = tabSelectedSprite;
            headerTabTexts[2].color = tabSelectedFontColour;

            RebuildAndShowItemsPanel();
        }
        public void OnPerksHeaderTabClicked()
        {
            CloseAllCustomCharacterScreenPanels();

            // Reset 'selected' visual state on all tabs
            for (int i = 0; i < headerTabImages.Length; i++)
            {
                headerTabImages[i].sprite = tabUnselectedSprite;
                headerTabTexts[i].color = tabUnselectedFontColour;
            }

            headerTabImages[3].sprite = tabSelectedSprite;
            headerTabTexts[3].color = tabSelectedFontColour;
            RebuildAndShowPerkPanel();
        }
        #endregion

        // Custom Character Screen Logic : Origin Panel
        #region
        private void RebuildAndShowOriginPageView()
        {
            // Route to page
            CloseAllCustomCharacterScreenPanels();
            ccsOriginPanel.SetActive(true);

            // Set name text
            originPanelPresetNameText.text = currentPreset.myClassName.ToString();

            // Reset and build ability icons
            var nonItemAbilities = characterBuild.abilityBook.GetAllKnownNonItemSetAbilities();
            foreach (UIAbilityIcon a in originPanelAbilityIcons)
                a.HideAndReset();
            for(int i = 0; i < nonItemAbilities.Count && i < originPanelAbilityIcons.Length; i++)            
                originPanelAbilityIcons[i].BuildFromAbilityData(nonItemAbilities[i]);

            // Reset and build talent rows
            foreach (UITalentRow r in originPanelTalentRows)
                r.HideAndReset();
            for (int i = 0; i < characterBuild.talentPairings.Count && i < originPanelTalentRows.Length; i++)
                originPanelTalentRows[i].BuildFromTalentPairing(characterBuild.talentPairings[i]);

        }
        public void OnNextClassPresetButtonClicked()
        {
            HexCharacterData[] templates = CharacterDataController.Instance.AllCustomCharacterTemplates;
            HexCharacterData nextTemplate = null;

            int currentIndex = Array.IndexOf(templates, currentPreset);
            if (currentIndex == templates.Length - 1)
                nextTemplate = templates[0];

            else
                nextTemplate = templates[currentIndex + 1];

            HandleChangeClassPreset(nextTemplate);
        }
        public void OnPreviousClassPresetButtonClicked()
        {
            HexCharacterData[] templates = CharacterDataController.Instance.AllCustomCharacterTemplates;
            HexCharacterData nextTemplate = null;
            int currentIndex = Array.IndexOf(templates, currentPreset);
            if (currentIndex == 0)
                nextTemplate = templates[templates.Length - 1];

            else
                nextTemplate = templates[currentIndex - 1];

            HandleChangeClassPreset(nextTemplate);
        }
        public void OnNameInputFieldValueChanged()
        {
            characterBuild.myName = characterNameInputField.text;
            characterBuild.myClassName = "Captain";
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
            foreach(CustomCharacterAttributeRow row in presetPanelAttributeRows)            
                RebuildAttributeRow(row);

            // Update availble attribute points text
            presetPanelAttributePointsText.text = (maxAllowedAttributePoints - GetTotalAttributePointsSpent()).ToString();

        }
        private void RebuildPresetPanelAbilitySection()
        {
            // Reset and build ability icons
            var nonItemAbilities = characterBuild.abilityBook.GetAllKnownNonItemSetAbilities();
            foreach (UIAbilityIcon a in presetPanelAbilityIcons)
                a.HideAndReset();
            for (int i = 0; i < nonItemAbilities.Count && i < presetPanelAbilityIcons.Length; i++)
                presetPanelAbilityIcons[i].BuildFromAbilityData(nonItemAbilities[i]);

        }
        private void RebuildPresetPanelTalentSection()
        {
            // Reset and build talent rows
            foreach (UITalentIcon r in presetPanelTalentIcons)
                r.HideAndReset();
            for (int i = 0; i < characterBuild.talentPairings.Count && i < presetPanelTalentIcons.Length; i++)
                presetPanelTalentIcons[i].BuildFromTalentPairing(characterBuild.talentPairings[i]);
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
            if (dif > 0) row.MinusButtonParent.SetActive(true);            
            if (dif < individualAttributeBoostLimit) row.PlusButtonParent.SetActive(true);

            // Hide plus button if player already spent all points
            if (GetTotalAttributePointsSpent() >= maxAllowedAttributePoints)
                row.PlusButtonParent.SetActive(false);

            // Set text colouring
            if (dif > 0) row.AmountText.color = row.BoostedStatTextColor;
            else row.AmountText.color = row.NormalStatTextColor;

        }
        private int GetCharacterAttributeDifference(CoreAttribute att)
        {
            int dif = 0;

            if (att == CoreAttribute.Might)            
                dif = characterBuild.attributeSheet.might.value - baselineAttributes.might.value;
            
            else if (att == CoreAttribute.Accuracy)            
                dif = characterBuild.attributeSheet.accuracy.value - baselineAttributes.accuracy.value;
            
            else if (att == CoreAttribute.Dodge)            
                dif = characterBuild.attributeSheet.dodge.value - baselineAttributes.dodge.value;
            
            else if (att == CoreAttribute.Constitution)            
                dif = characterBuild.attributeSheet.constitution.value - baselineAttributes.constitution.value;
            
            else if (att == CoreAttribute.Resolve)            
                dif = characterBuild.attributeSheet.resolve.value - baselineAttributes.resolve.value;
            
            else if (att == CoreAttribute.Wits)            
                dif = characterBuild.attributeSheet.wits.value - baselineAttributes.wits.value;
            
            else if (att == CoreAttribute.Fitness)            
                dif = characterBuild.attributeSheet.fitness.value - baselineAttributes.fitness.value;            

            Debug.Log("MainMenuController.GetCharacterAttributeDifference() returning " + dif.ToString() +
                " for attribute: " + att.ToString());

            return dif;
        }
        private int GetCharacterAttributeValue(CoreAttribute att)
        {
            int value = 0;

            if (att == CoreAttribute.Might)            
                value = characterBuild.attributeSheet.might.value;
                        
            else if (att == CoreAttribute.Accuracy)            
                value = characterBuild.attributeSheet.accuracy.value;
            
            else if (att == CoreAttribute.Dodge)            
                value = characterBuild.attributeSheet.dodge.value;
            
            else if (att == CoreAttribute.Constitution)            
                value = characterBuild.attributeSheet.constitution.value;
            
            else if (att == CoreAttribute.Resolve)            
                value = characterBuild.attributeSheet.resolve.value;
            
            else if (att == CoreAttribute.Wits)            
                value = characterBuild.attributeSheet.wits.value;

            else if (att == CoreAttribute.Fitness)
                value = characterBuild.attributeSheet.fitness.value;

            Debug.Log("MainMenuController.GetCharacterAttributeValue) returning " + value.ToString() +
               " for attribute: " + att.ToString());

            return value;
        }
        private int GetTotalAttributePointsSpent()
        {
            int dif = 0;
            dif += GetCharacterAttributeDifference(CoreAttribute.Might) ;
            dif += GetCharacterAttributeDifference(CoreAttribute.Constitution) ;
            dif += GetCharacterAttributeDifference(CoreAttribute.Accuracy) ;
            dif += GetCharacterAttributeDifference(CoreAttribute.Dodge) ;
            dif += GetCharacterAttributeDifference(CoreAttribute.Resolve) ;
            dif += GetCharacterAttributeDifference(CoreAttribute.Wits) ;
            dif += GetCharacterAttributeDifference(CoreAttribute.Fitness);
            Debug.Log("MainMenuController.GetTotalAttributePointsSpent() returning: " + dif.ToString());
            return dif;
        }           
        public void OnDecreaseAttributeButtonClicked(CustomCharacterAttributeRow row)
        {
            if (row.Attribute == CoreAttribute.Might)
                characterBuild.attributeSheet.might.value -= 1;
            else if (row.Attribute == CoreAttribute.Accuracy)
                characterBuild.attributeSheet.accuracy.value -= 1;
            else if (row.Attribute == CoreAttribute.Dodge)
                characterBuild.attributeSheet.dodge.value -= 1;
            else if (row.Attribute == CoreAttribute.Constitution)
                characterBuild.attributeSheet.constitution.value -= 1;
            else if (row.Attribute == CoreAttribute.Resolve)
                characterBuild.attributeSheet.resolve.value -= 1;
            else if (row.Attribute == CoreAttribute.Wits)
                characterBuild.attributeSheet.wits.value -= 1;
            else if (row.Attribute == CoreAttribute.Fitness)
                characterBuild.attributeSheet.fitness.value -= 1;

            RebuildAttributeSection();
        }
        public void OnIncreaseAttributeButtonClicked(CustomCharacterAttributeRow row)
        {
            if (row.Attribute == CoreAttribute.Might)
                characterBuild.attributeSheet.might.value += 1;
            else if (row.Attribute == CoreAttribute.Accuracy)
                characterBuild.attributeSheet.accuracy.value += 1;
            else if (row.Attribute == CoreAttribute.Dodge)
                characterBuild.attributeSheet.dodge.value += 1;
            else if (row.Attribute == CoreAttribute.Constitution)
                characterBuild.attributeSheet.constitution.value += 1;
            else if (row.Attribute == CoreAttribute.Resolve)
                characterBuild.attributeSheet.resolve.value += 1;
            else if (row.Attribute == CoreAttribute.Wits)
                characterBuild.attributeSheet.wits.value += 1;
            else if (row.Attribute == CoreAttribute.Fitness)
                characterBuild.attributeSheet.fitness.value += 1;

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
            if (allLevelUpPerks.Count == 0)            
               allLevelUpPerks =  PerkController.Instance.GetAllLevelUpPerks();

            // Reset all perk panels to default state
            for(int i = 0; i < allChoosePerkPanels.Count; i++)            
                allChoosePerkPanels[i].Reset();
            
            // Build a choose perk panel for each level up perk option
            for(int i = 0; i < allLevelUpPerks.Count; i++)
            {
                // Create new panels in list if there arent enough to show all the perks
                if(allChoosePerkPanels.Count <= i)
                {
                    CustomCharacterChoosePerkPanel newPanel = Instantiate(choosePerkPanelPrefab, choosePerkPanelsParent).GetComponent<CustomCharacterChoosePerkPanel>();
                    allChoosePerkPanels.Add(newPanel);
                }

                // Build the panel from perk
                var panel = allChoosePerkPanels[i];
                panel.Build(new ActivePerk(allLevelUpPerks[i].perkTag, 1));

                // If already has the perk, set that perk as selected and deduct a perk choice point.
                if(PerkController.Instance.DoesCharacterHavePerk(characterBuild.passiveManager, allLevelUpPerks[i].perkTag))
                {
                    SetAvailableChoosePerkPoints(availableChoosePerkPoints - 1);
                    panel.SetSelectedViewState(true);
                }

            }
        }
        public void HandleChoosePerkPanelClicked(CustomCharacterChoosePerkPanel panel)
        {
            bool hasPerk = PerkController.Instance.DoesCharacterHavePerk(characterBuild.passiveManager, panel.PerkIcon.ActivePerk.perkTag);

            if (hasPerk)
            {
                SetAvailableChoosePerkPoints(availableChoosePerkPoints + 1);
                panel.SetSelectedViewState(false);
                PerkController.Instance.ModifyPerkOnCharacterData(characterBuild.passiveManager, panel.PerkIcon.ActivePerk.perkTag, -panel.PerkIcon.ActivePerk.stacks);
            }
            else if(!hasPerk && availableChoosePerkPoints > 0)
            {
                SetAvailableChoosePerkPoints(availableChoosePerkPoints - 1);
                panel.SetSelectedViewState(true);
                PerkController.Instance.ModifyPerkOnCharacterData(characterBuild.passiveManager, panel.PerkIcon.ActivePerk.perkTag, panel.PerkIcon.ActivePerk.stacks);

            }
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
                a.HideAndReset();

            // Determine + create weapon abilities
            List<AbilityData> weaponAbilities = characterBuild.abilityBook.GetAbilitiesFromItemSet(characterBuild.itemSet);

            // Build icons from weapon abilities
            for (int i = 0; i < weaponAbilities.Count && i < itemsPanelAbilityIcons.Length; i++)
                itemsPanelAbilityIcons[i].BuildFromAbilityData(weaponAbilities[i]);
        }
        private void HandleSetItemsFromPreset(HexCharacterData preset)
        {
            ItemController.Instance.CopyItemManagerDataIntoOtherItemManager(preset.itemSet, characterBuild.itemSet);
            CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);

            // Rebuild icon views for items + abilities
            UpdateItemSlotViews();
            RebuildItemPanelWeaponAbilityIcons();
        }
        private void UpdateItemSlotViews()
        {
            headItemIcon.BuildFromData(characterBuild.itemSet.headArmour);
            if (characterBuild.itemSet.headArmour == null) CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeHeadWear);
            bodyItemIcon.BuildFromData(characterBuild.itemSet.bodyArmour);
            mainHandItemIcon.BuildFromData(characterBuild.itemSet.mainHandItem);
            offHandItemIcon.BuildFromData(characterBuild.itemSet.offHandItem);
            if (characterBuild.itemSet.offHandItem == null) CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
        }
        public void OnNextHeadItemClicked()
        {

            // Not currently wearing a head item
            if (characterBuild.itemSet.headArmour == null)
            {
                ItemData headItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingHeadItems[0].itemName));
                characterBuild.itemSet.headArmour = headItem;
                CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                headItemIcon.BuildFromData(headItem);
            }

            else
            {
                ItemDataSO currentHead = null;
                foreach (ItemDataSO headData in allStartingHeadItems)
                {
                    if (headData.itemName == characterBuild.itemSet.headArmour.itemName)
                    {
                        currentHead = headData;
                    }
                }

                int currentIndex = Array.IndexOf(allStartingHeadItems, currentHead);

                // Set no item
                if (currentIndex == allStartingHeadItems.Length - 1)
                {
                    characterBuild.itemSet.headArmour = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeHeadWear);
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    headItemIcon.BuildFromData(null);
                }

                // Set next item
                else
                {
                    ItemData headItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingHeadItems[currentIndex + 1].itemName));
                    characterBuild.itemSet.headArmour = headItem;
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    headItemIcon.BuildFromData(headItem);
                }

            }


        }
        public void OnPreviousHeadItemClicked()
        {
            // Not currently wearing a head item
            if (characterBuild.itemSet.headArmour == null)
            {
                ItemData headItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingHeadItems[allStartingHeadItems.Length - 1].itemName));
                characterBuild.itemSet.headArmour = headItem;
                CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                headItemIcon.BuildFromData(headItem);
            }

            else
            {
                ItemDataSO currentHead = null;
                foreach (ItemDataSO headData in allStartingHeadItems)
                {
                    if (headData.itemName == characterBuild.itemSet.headArmour.itemName)
                    {
                        currentHead = headData;
                    }
                }
                int currentIndex = Array.IndexOf(allStartingHeadItems, currentHead);

                // Set no item
                if (currentIndex == 0)
                {
                    characterBuild.itemSet.headArmour = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeHeadWear);
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    headItemIcon.BuildFromData(null);
                }
                // Set previous item
                else
                {
                    ItemData headItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingHeadItems[currentIndex - 1].itemName));
                    characterBuild.itemSet.headArmour = headItem;
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    headItemIcon.BuildFromData(headItem);
                }

            }

        }
        public void OnNextBodyItemClicked()
        {
            // Not currently wearing a head item
            if (characterBuild.itemSet.bodyArmour == null)
            {
                ItemData bodyItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingBodyItems[0].itemName));
                characterBuild.itemSet.bodyArmour = bodyItem;
                CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                bodyItemIcon.BuildFromData(bodyItem);
            }

            else
            {
                ItemDataSO currentBody = null;
                foreach (ItemDataSO bodyData in allStartingBodyItems)
                {
                    if (bodyData.itemName == characterBuild.itemSet.bodyArmour.itemName)
                        currentBody = bodyData;
                }

                int currentIndex = Array.IndexOf(allStartingBodyItems, currentBody);

                // Set no item
                if (currentIndex == allStartingBodyItems.Length - 1)
                {
                    characterBuild.itemSet.bodyArmour = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeChestWear);
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    bodyItemIcon.BuildFromData(null);
                }

                // Set next item
                else
                {
                    ItemData bodyItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingBodyItems[currentIndex + 1].itemName));
                    characterBuild.itemSet.bodyArmour = bodyItem;
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    bodyItemIcon.BuildFromData(bodyItem);
                }

            }


        }
        public void OnPreviousBodyItemClicked()
        {
            // Not currently wearing a head item
            if (characterBuild.itemSet.bodyArmour == null)
            {
                ItemData bodyItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingBodyItems[allStartingBodyItems.Length - 1].itemName));
                characterBuild.itemSet.bodyArmour = bodyItem;
                CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                bodyItemIcon.BuildFromData(bodyItem);
            }

            else
            {
                ItemDataSO currentBody = null;
                foreach (ItemDataSO bodyData in allStartingBodyItems)
                {
                    if (bodyData.itemName == characterBuild.itemSet.bodyArmour.itemName)
                        currentBody = bodyData;
                }
                int currentIndex = Array.IndexOf(allStartingBodyItems, currentBody);

                // Set no item
                if (currentIndex == 0)
                {
                    characterBuild.itemSet.bodyArmour = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeChestWear);
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    bodyItemIcon.BuildFromData(null);
                }
                // Set previous item
                else
                {
                    ItemData bodyItem = ItemController.Instance.GenerateNewItemWithRandomEffects(ItemController.Instance.GetItemDataByName(allStartingBodyItems[currentIndex - 1].itemName));
                    characterBuild.itemSet.bodyArmour = bodyItem;
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    bodyItemIcon.BuildFromData(bodyItem);
                }

            }

        }
        public void OnNextMainHandItemClicked()
        {
            if (characterBuild.itemSet.mainHandItem == null)
            {
                ItemData item = ItemController.Instance.GetItemDataByName(allStartingMainHandItems[0].itemName);
                characterBuild.itemSet.mainHandItem = item;
                CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                mainHandItemIcon.BuildFromData(item);

                if (characterBuild.itemSet.offHandItem != null && item.handRequirement == HandRequirement.TwoHanded)
                {
                    characterBuild.itemSet.offHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
                    offHandItemIcon.BuildFromData(null);
                }
            }
            else
            {

                ItemDataSO currentMH = null;
                foreach (ItemDataSO mainHandItem in allStartingMainHandItems)
                {
                    if (mainHandItem.itemName == characterBuild.itemSet.mainHandItem.itemName)
                        currentMH = mainHandItem;
                }

                int currentIndex = Array.IndexOf(allStartingMainHandItems, currentMH);

                // Set no item
                if (currentIndex == allStartingMainHandItems.Length - 1)
                {
                    characterBuild.itemSet.mainHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeMainHandWeapon);
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    mainHandItemIcon.BuildFromData(null);
                }

                // Set next item
                else
                {
                    ItemData item = ItemController.Instance.GetItemDataByName(allStartingMainHandItems[currentIndex + 1].itemName);
                    characterBuild.itemSet.mainHandItem = item;
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    mainHandItemIcon.BuildFromData(item);

                    // Remove off hand item when equipping a 2h weapon
                    if (characterBuild.itemSet.offHandItem != null && item.handRequirement == HandRequirement.TwoHanded)
                    {
                        characterBuild.itemSet.offHandItem = null;
                        CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
                        offHandItemIcon.BuildFromData(null);
                    }
                }                
            }

            RebuildItemPanelWeaponAbilityIcons();
        }
        public void OnPreviousMainHandItemClicked()
        {
            if (characterBuild.itemSet.mainHandItem == null)
            {
                ItemData bodyItem = ItemController.Instance.GetItemDataByName(allStartingMainHandItems[allStartingMainHandItems.Length - 1].itemName);
                characterBuild.itemSet.mainHandItem = bodyItem;
                CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                mainHandItemIcon.BuildFromData(bodyItem);

                if (characterBuild.itemSet.offHandItem != null && bodyItem.handRequirement == HandRequirement.TwoHanded)
                {
                    characterBuild.itemSet.offHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
                    offHandItemIcon.BuildFromData(null);
                }
            }

            else
            {
                ItemDataSO currentMH = null;
                foreach (ItemDataSO bodyData in allStartingMainHandItems)
                {
                    if (bodyData.itemName == characterBuild.itemSet.mainHandItem.itemName)
                        currentMH = bodyData;
                }
                int currentIndex = Array.IndexOf(allStartingMainHandItems, currentMH);

                // Set no item
                if (currentIndex == 0)
                {
                    characterBuild.itemSet.mainHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeMainHandWeapon);
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    mainHandItemIcon.BuildFromData(null);
                }
                // Set previous item
                else
                {
                    ItemData mhItem = ItemController.Instance.GetItemDataByName(allStartingMainHandItems[currentIndex - 1].itemName);
                    characterBuild.itemSet.mainHandItem = mhItem;
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    mainHandItemIcon.BuildFromData(mhItem);

                    // Remove off hand item when equipping a 2h weapon
                    if (characterBuild.itemSet.offHandItem != null && mhItem.handRequirement == HandRequirement.TwoHanded)
                    {
                        characterBuild.itemSet.offHandItem = null;
                        CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
                        offHandItemIcon.BuildFromData(null);
                    }
                }
            }
            RebuildItemPanelWeaponAbilityIcons();
        }
        public void OnNextOffHandItemClicked()
        {
            if (characterBuild.itemSet.mainHandItem != null &&
                characterBuild.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
                return;

            if (characterBuild.itemSet.offHandItem == null)
            {
                ItemData item = ItemController.Instance.GetItemDataByName(allStartingOffHandItems[0].itemName);
                characterBuild.itemSet.offHandItem = item;
                CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                offHandItemIcon.BuildFromData(item);

                if (characterBuild.itemSet.mainHandItem != null && characterBuild.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
                {
                    characterBuild.itemSet.mainHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeMainHandWeapon);
                    mainHandItemIcon.BuildFromData(null);
                }
            }

            else
            {
                ItemDataSO currentOH = null;
                foreach (ItemDataSO offhandItem in allStartingOffHandItems)
                {
                    if (offhandItem.itemName == characterBuild.itemSet.offHandItem.itemName)
                        currentOH = offhandItem;
                }

                int currentIndex = Array.IndexOf(allStartingOffHandItems, currentOH);

                // Set no item
                if (currentIndex == allStartingOffHandItems.Length - 1)
                {
                    characterBuild.itemSet.offHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    offHandItemIcon.BuildFromData(null);
                }

                // Set next item
                else
                {
                    ItemData item = ItemController.Instance.GetItemDataByName(allStartingOffHandItems[currentIndex + 1].itemName);
                    characterBuild.itemSet.offHandItem = item;
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    offHandItemIcon.BuildFromData(item);

                    if (characterBuild.itemSet.mainHandItem != null && characterBuild.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
                    {
                        characterBuild.itemSet.mainHandItem = null;
                        CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeMainHandWeapon);
                        mainHandItemIcon.BuildFromData(null);
                    }
                }

            }

            RebuildItemPanelWeaponAbilityIcons();
        }
        public void OnPreviousOffHandItemClicked()
        {
            if (characterBuild.itemSet.mainHandItem != null &&
               characterBuild.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
                return;

            // Not currently using an off hand item
            if (characterBuild.itemSet.offHandItem == null)
            {
                ItemData bodyItem = ItemController.Instance.GetItemDataByName(allStartingOffHandItems[allStartingOffHandItems.Length - 1].itemName);
                characterBuild.itemSet.offHandItem = bodyItem;
                CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                offHandItemIcon.BuildFromData(bodyItem);

                if (characterBuild.itemSet.mainHandItem != null && characterBuild.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
                {
                    characterBuild.itemSet.mainHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeMainHandWeapon);
                    mainHandItemIcon.BuildFromData(null);
                }
            }

            else
            {
                ItemDataSO currentOH = null;
                foreach (ItemDataSO bodyData in allStartingOffHandItems)
                {
                    if (bodyData.itemName == characterBuild.itemSet.offHandItem.itemName)
                        currentOH = bodyData;
                }
                int currentIndex = Array.IndexOf(allStartingOffHandItems, currentOH);

                // Set no item
                if (currentIndex == 0)
                {
                    characterBuild.itemSet.offHandItem = null;
                    CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeOffHandWeapon);
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    offHandItemIcon.BuildFromData(null);
                }
                // Set previous item
                else
                {
                    ItemData bodyItem = ItemController.Instance.GetItemDataByName(allStartingOffHandItems[currentIndex - 1].itemName);
                    characterBuild.itemSet.offHandItem = bodyItem;
                    CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
                    offHandItemIcon.BuildFromData(bodyItem);

                    if (characterBuild.itemSet.mainHandItem != null && characterBuild.itemSet.mainHandItem.handRequirement == HandRequirement.TwoHanded)
                    {
                        characterBuild.itemSet.mainHandItem = null;
                        CharacterModeller.DisableAndClearElementOnModel(customCharacterScreenUCM, customCharacterScreenUCM.activeMainHandWeapon);
                        mainHandItemIcon.BuildFromData(null);
                    }
                }

            }
            RebuildItemPanelWeaponAbilityIcons();
        }
        public void OnNextRacialModelButtonClicked()
        {
            HandleChangeRacialModel(GetNextRacialModel(characterBuild.race));
        }
        public void OnPreviousRacialModelButtonClicked()
        {
            HandleChangeRacialModel(GetPreviousRacialModel(characterBuild.race));
        }
        #endregion

        // Custom Character Screen Logic : Choose Abilities Sub Screeb
        #region
        public void OnAbilitySectionEditButtonClicked()
        {
            CloseAllCustomCharacterScreenPanels();
            ccsAbilityPanel.SetActive(true);
            BuildChooseAbilitiesPanel();
            TransformUtils.RebuildLayouts(chooseAbilityPanelLayouts);
        }
        public void OnAbilitySectionConfirmButtonClicked()
        {
            CloseAllCustomCharacterScreenPanels();

            // save ability changes
            List<AbilityData> newAbilities = new List<AbilityData>();
            foreach(ChooseAbilityButton b in GetSelectedAbilities())
            {
                newAbilities.Add(b.AbilityIcon.MyDataRef);
            }
            characterBuild.abilityBook.ForgetAllAbilities();
            characterBuild.abilityBook.HandleLearnNewAbilities(newAbilities);

            // show preset panel
            RebuildAndShowPresetPanel();
        }       
        private void BuildChooseAbilitiesPanel()
        {
            TalentSchool talentSchoolOne = characterBuild.talentPairings[0].talentSchool;
            chooseAbilityButtonsHeaderTextOne.text = talentSchoolOne.ToString() + " Abilities";
            List<AbilityData> allTalentOneAbilities = AbilityController.Instance.GetAllAbilitiesOfTalent(talentSchoolOne);

            // Reset all first
            for(int i = 0; i < chooseAbilityButtonsSectionOne.Length; i++)            
                chooseAbilityButtonsSectionOne[i].ResetAndHide();
            
            // Rebuild
            for (int i = 0; i < allTalentOneAbilities.Count && i < chooseAbilityButtonsSectionOne.Length; i++)            
                chooseAbilityButtonsSectionOne[i].BuildAndShow(allTalentOneAbilities[i]);

            // Select buttons based on known abilities of character
            foreach(AbilityData a in characterBuild.abilityBook.knownAbilities)
            {
                foreach(ChooseAbilityButton button in chooseAbilityButtonsSectionOne)
                {
                    if(button.AbilityIcon.MyDataRef != null && a.abilityName == button.AbilityIcon.MyDataRef.abilityName)
                    {
                        button.HandleChangeSelectionState(true);
                        break;
                    }
                }
            }

            // Build for 2nd talent (if character has a second talent
            if(characterBuild.talentPairings.Count > 1)
            {
                TalentSchool talentSchoolTwo = characterBuild.talentPairings[1].talentSchool;
                chooseAbilityButtonsHeaderTextTwo.text = talentSchoolTwo.ToString() + " Abilities";
                List<AbilityData> allTalentTwoAbilities = AbilityController.Instance.GetAllAbilitiesOfTalent(talentSchoolTwo);

                // Reset all first
                for (int i = 0; i < chooseAbilityButtonsSectionTwo.Length; i++)
                    chooseAbilityButtonsSectionTwo[i].ResetAndHide();

                // Rebuild
                for (int i = 0; i < allTalentTwoAbilities.Count && i < chooseAbilityButtonsSectionTwo.Length; i++)
                    chooseAbilityButtonsSectionTwo[i].BuildAndShow(allTalentTwoAbilities[i]);

                // Select buttons based on known abilities of character
                foreach (AbilityData a in characterBuild.abilityBook.knownAbilities)
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

            UpdateChosenAbilitiesText();
        }
        public List<ChooseAbilityButton> GetSelectedAbilities()
        {
            List<ChooseAbilityButton> ret = new List<ChooseAbilityButton>();

            List<ChooseAbilityButton> allButtons = new List<ChooseAbilityButton>();
            allButtons.AddRange(chooseAbilityButtonsSectionOne);
            allButtons.AddRange(chooseAbilityButtonsSectionTwo);

            foreach(ChooseAbilityButton b in allButtons)
            {
                if (b.Selected)
                    ret.Add(b);
            }

            return ret;
        }
        public void UpdateChosenAbilitiesText()
        {
            totalChosenAbilitiesText.text = GetSelectedAbilities().Count.ToString() + "/3";
        }
        private void HandleUnlearnAbilitiesOnTalentsChanged()
        {
            List<AbilityData> invalidAbilities = new List<AbilityData>();
            List<TalentSchool> newTalents = new List<TalentSchool>();

            // Determine current talents
            foreach (TalentPairing tp in characterBuild.talentPairings)
                newTalents.Add(tp.talentSchool);

            // Find invalid abilities
            foreach(AbilityData a in characterBuild.abilityBook.knownAbilities)
            {
                if(newTalents.Contains(a.talentRequirementData.talentSchool) == false)                
                    invalidAbilities.Add(a);                
            }

            // Remove invalid abilities
            foreach(AbilityData a in invalidAbilities)            
                characterBuild.abilityBook.HandleUnlearnAbility(a.abilityName);            
        }
        #endregion

        // Custom Character Screen Logic : Choose Talents Sub Screeb
        #region
        public void BuildChooseTalentsPanel()
        {
            // Reset all first
            for (int i = 0; i < chooseTalentButtons.Length; i++)
                chooseTalentButtons[i].ResetAndHide();

            // Rebuild
            for (int i = 0; i < chooseTalentButtons.Length; i++)
                chooseTalentButtons[i].BuildAndShow();

            // Select buttons based on known abilities of character
            foreach (TalentPairing tp in characterBuild.talentPairings)
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
            characterBuild.talentPairings.Clear();
            foreach (ChooseTalentButton b in GetSelectedTalents())            
                characterBuild.talentPairings.Add(new TalentPairing(b.TalentPairing.talentSchool, 1));

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
                    ret.Add(b);
            }

            return ret;
        }
        public void UpdateChosenTalentsText()
        {
            totalChosenTalentsText.text = GetSelectedTalents().Count.ToString() + "/2";
        }
        #endregion

        // Custom Character Screen Logic : Update Model + Race
        #region
        private void HandleChangeRace(CharacterRace race)
        {
            // Get + cache race data
            currentCustomCharacterRace = CharacterDataController.Instance.GetRaceData(race);
            characterBuild.race = currentCustomCharacterRace.racialTag;

            // Build race icon image
            originPanelRacialIcon.BuildFromRacialData(currentCustomCharacterRace);

            // Build racial texts
            originPanelRacialDescriptionText.text = currentCustomCharacterRace.loreDescription;
            originPanelRacialNameText.text = currentCustomCharacterRace.racialTag.ToString();

            // Rebuild UCM as default model of race
            HandleChangeRacialModel(GetRacialModelBasket(characterBuild.race).templates[0]);
        }
        private void HandleChangeRacialModel(CharacterModelTemplateSO newModel)
        {
            currentModelTemplate = newModel;
            characterBuild.modelParts = newModel.bodyParts;
            CharacterModeller.BuildModelFromStringReferences(customCharacterScreenUCM, newModel.bodyParts);
            CharacterModeller.ApplyItemSetToCharacterModelView(characterBuild.itemSet, customCharacterScreenUCM);
            int index = Array.IndexOf(GetRacialModelBasket(characterBuild.race).templates, currentModelTemplate);
            currentBodyModelText.text = "Body " + (index + 1).ToString();
        }
        public void OnChooseRaceNextButtonClicked()
        {
            Debug.Log("MainMenuController.OnChooseRaceNextButtonClicked() called...");
            CharacterRace[] playableRaces = CharacterDataController.Instance.PlayableRaces.ToArray();
            CharacterRace nextValidRace = CharacterRace.None;
            int currentIndex = Array.IndexOf(playableRaces, currentCustomCharacterRace.racialTag);

            if (currentIndex == playableRaces.Length - 1)
                nextValidRace = playableRaces[0];

            else
                nextValidRace = playableRaces[currentIndex + 1];

            HandleChangeRace(nextValidRace);

        }
        public void OnChooseRacePreviousButtonClicked()
        {
            Debug.Log("MainMenuController.OnChooseRacePreviousButtonClicked() called...");
            CharacterRace[] playableRaces = CharacterDataController.Instance.PlayableRaces.ToArray();
            CharacterRace nextValidRace = CharacterRace.None;
            int currentIndex = Array.IndexOf(playableRaces, currentCustomCharacterRace.racialTag);

            if (currentIndex == 0)
                nextValidRace = playableRaces[playableRaces.Length - 1];

            else
                nextValidRace = playableRaces[currentIndex - 1];

            HandleChangeRace(nextValidRace);
        }
        private CharacterModelTemplateBasket GetRacialModelBasket(CharacterRace race)
        {
            CharacterModelTemplateBasket ret = null;

            foreach(CharacterModelTemplateBasket basket in modelTemplateBaskets)
            {
                if(basket.race == race)
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
                if(b.race == race)
                {
                    int index = Array.IndexOf(b.templates, currentModelTemplate);
                    if (index == b.templates.Length - 1)
                        ret = b.templates[0];
                    else ret = b.templates[index + 1];

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
                        ret = b.templates[b.templates.Length - 1];
                    else ret = b.templates[index - 1];

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
            foreach(CrowdRowAnimator cra in frontScreenCrowdRows)
            {
                cra.StopAnimation();
                cra.PlayAnimation();
            }
        }
        public void HideFrontScreen()
        {
            frontScreenParent.SetActive(false);
            foreach (CrowdRowAnimator cra in frontScreenCrowdRows)            
                cra.StopAnimation();            
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
            ShowFrontScreen();

            frontScreenGuiCg.alpha = 0;
            frontScreenBgParent.transform.DOScale(1.25f, 0f);
            yield return new WaitForSeconds(1);

            AudioManager.Instance.FadeInSound(Sound.Music_Main_Menu_Theme_1, 3f);
            BlackScreenController.Instance.FadeInScreen(2f);
            yield return new WaitForSeconds(2);

            frontScreenBgParent.transform.DOKill();
            frontScreenBgParent.transform.DOScale(1f, 1.5f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(1f);

            frontScreenGuiCg.DOFade(1f, 1f).SetEase(Ease.OutCubic);
        }
        #endregion


    }
}