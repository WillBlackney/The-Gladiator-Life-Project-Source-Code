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

namespace HexGameEngine.MainMenu
{
    public class MainMenuController : Singleton<MainMenuController>
    {
        // Properties + Components
        #region
        [Header("Front Screen Components")]
        [SerializeField] private GameObject frontScreenParent;
        public GameObject frontScreenBgParent;
        public CanvasGroup frontScreenGuiCg;
        [SerializeField] private GameObject newGameButtonParent;
        [SerializeField] private GameObject continueButtonParent;
        [SerializeField] private GameObject abandonRunButtonParent;
        [SerializeField] private GameObject abandonRunPopupParent;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("New Game Screen Components")]      
        [SerializeField] ChooseCharacterBox[] allChooseCharacterBoxes;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("In Game Menu Components")]
        [SerializeField] private GameObject inGameMenuScreenParent;
        [SerializeField] private CanvasGroup inGameMenuScreenCg;
        [PropertySpace(SpaceBefore = 30, SpaceAfter = 0)]

        [Title("Custom Character Screen Components")]    
        [SerializeField] UniversalCharacterModel customCharacterScreenUCM;
        [SerializeField] GameObject chooseCharacterScreenVisualParent;
        [SerializeField] CharacterModelTemplateBasket[] modelTemplateBaskets;

        [Header("Custom Character Screen Data")]
        [SerializeField] SerializedAttrbuteSheet baselineAttributes;
        [SerializeField] int maxAllowedAttributePoints = 15;
        [SerializeField] int individualAttributeBoostLimit = 10;

        [Header("Custom Character Screen Header Tab Refs")]
        [SerializeField] Sprite tabSelectedSprite;
        [SerializeField] Sprite tabUnselectedSprite;
        [SerializeField] Color tabSelectedFontColour;
        [SerializeField] Color tabUnselectedFontColour;
        [SerializeField] Image[] headerTabImages;
        [SerializeField] TextMeshProUGUI[] headerTabTexts;

        [Header("Custom Character Screen Pages")]
        [SerializeField] GameObject ccsOriginPanel;
        [SerializeField] GameObject ccsPresetPanel;
        [SerializeField] GameObject ccsItemsPanel;      
        [SerializeField] GameObject ccsAbilityPanel;
        [SerializeField] GameObject ccsTalentPanel;

        [Header("Origin Panel Components")]
        [SerializeField] private TextMeshProUGUI originPanelRacialNameText;
        [SerializeField] private UIRaceIcon originPanelRacialIcon;
        [SerializeField] private TextMeshProUGUI originPanelRacialDescriptionText; 
        [SerializeField] private TextMeshProUGUI originPanelPresetNameText;
        [SerializeField] private UIAbilityIcon[] originPanelAbilityIcons;
        [SerializeField] private UITalentRow[] originPanelTalentRows;

        [Header("Preset Panel Components")]
        [SerializeField] private TextMeshProUGUI presetPanelAttributePointsText;
        [SerializeField] private CustomCharacterAttributeRow[] presetPanelAttributeRows;
        [SerializeField] private UIAbilityIcon[] presetPanelAbilityIcons;
        [SerializeField] private UITalentIcon[] presetPanelTalentIcons;

        // Non inspector proerties
        private HexCharacterData characterBuild;
        private HexCharacterData currentPreset;
        private RaceDataSO currentCustomRace;
        private CharacterModelTemplateSO currentModelTemplate;

        private void Start()
        {
            RenderMenuButtons();
        }
        #endregion

        // On Button Click Events
        #region
        public void OnStartGameButtonClicked()
        {
            // TO DO: validate selections
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
        public void OnChooseCharacterBoxNextButtonClicked(ChooseCharacterBox box)
        {
            Debug.Log("MainMenuController.OnChooseCharacterBoxNextButtonClicked() called...");
            HexCharacterData[] characters = CharacterDataController.Instance.AllCharacterTemplates;
            HexCharacterData nextValidCharacter = null;
            int totalLoops = 0;
            int currentIndex = Array.IndexOf(characters, box.currentCharacterSelection);
            while (nextValidCharacter == null && totalLoops < 1000)
            {
                if (!IsCharacterAlreadySelected(characters[currentIndex]))
                {
                    nextValidCharacter = characters[currentIndex];
                    break;
                }

                if (currentIndex == characters.Length - 1)
                {
                    currentIndex = 0;
                }
                else currentIndex++;

                totalLoops++;

            }
            SetChooseCharacterBoxCharacterSelection(box, nextValidCharacter);
        }
        public void OnChooseCharacterBoxPreviousButtonClicked(ChooseCharacterBox box)
        {
            Debug.Log("MainMenuController.OnChooseCharacterBoxPreviousButtonClicked() called...");
            HexCharacterData[] characters = CharacterDataController.Instance.AllCharacterTemplates;
            HexCharacterData previousValidCharacter = null;
            int totalLoops = 0;
            int currentIndex = Array.IndexOf(characters, box.currentCharacterSelection);
            while (previousValidCharacter == null && totalLoops < 1000)
            {
                if (!IsCharacterAlreadySelected(characters[currentIndex]))
                {
                    previousValidCharacter = characters[currentIndex];
                    break;
                }

                if (currentIndex == 0)
                {
                    currentIndex = characters.Length - 1;
                }
                else currentIndex--;

                totalLoops++;

            }

            SetChooseCharacterBoxCharacterSelection(box, previousValidCharacter);

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
            //characterBuild = ObjectCloner.CloneJSON(CharacterDataController.Instance.AllCustomCharacterTemplates[0]);
            characterBuild = CharacterDataController.Instance.CloneCharacterData(CharacterDataController.Instance.AllCustomCharacterTemplates[0]);
            baselineAttributes.CopyValuesIntoOther(characterBuild.attributeSheet);
            HandleChangeClassPreset(CharacterDataController.Instance.AllCustomCharacterTemplates[0]);

            // set to template 1 
            RebuildAndShowOriginPageView();

            // Set to race 1
            HandleChangeRace(CharacterDataController.Instance.PlayableRaces[0]);
        }
        private void HandleChangeClassPreset(HexCharacterData preset)
        {
            // Update cached preset
            currentPreset = preset;

            // Update abilities
            characterBuild.abilityBook = new AbilityBook();
            characterBuild.abilityBook.allKnownAbilities.AddRange(preset.abilityBook.allKnownAbilities);

            // Update talents
            characterBuild.talentPairings.Clear();
            characterBuild.talentPairings.AddRange(preset.talentPairings);

            // Update attributes
            ApplyAttributesFromPreset(preset);

            // to do: update weapons, items

            // Rebuild Origin page
            RebuildAndShowOriginPageView();
        }       
        private void CloseAllCustomCharacterScreenPanels()
        {
            ccsOriginPanel.SetActive(false);
            ccsPresetPanel.SetActive(false);
            ccsItemsPanel.SetActive(false);
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

            ccsItemsPanel.SetActive(true);
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
            foreach (UIAbilityIcon a in originPanelAbilityIcons)
                a.HideAndReset();
            for(int i = 0; i < characterBuild.abilityBook.allKnownAbilities.Count && i < originPanelAbilityIcons.Length; i++)            
                originPanelAbilityIcons[i].BuildFromAbilityData(characterBuild.abilityBook.allKnownAbilities[i]);

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
            foreach (UIAbilityIcon a in presetPanelAbilityIcons)
                a.HideAndReset();
            for (int i = 0; i < characterBuild.abilityBook.allKnownAbilities.Count && i < presetPanelAbilityIcons.Length; i++)
                presetPanelAbilityIcons[i].BuildFromAbilityData(characterBuild.abilityBook.allKnownAbilities[i]);

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

            if (att == CoreAttribute.Strength)
            {
                dif = characterBuild.attributeSheet.strength.value - baselineAttributes.strength.value;
            }
            else if (att == CoreAttribute.Intelligence)
            {
                dif = characterBuild.attributeSheet.intelligence.value - baselineAttributes.intelligence.value;
            }
            else if (att == CoreAttribute.Accuracy)
            {
                dif = characterBuild.attributeSheet.accuracy.value - baselineAttributes.accuracy.value;
            }
            else if (att == CoreAttribute.Dodge)
            {
                dif = characterBuild.attributeSheet.dodge.value - baselineAttributes.dodge.value;
            }
            else if (att == CoreAttribute.Constituition)
            {
                dif = characterBuild.attributeSheet.constitution.value - baselineAttributes.constitution.value;
            }
            else if (att == CoreAttribute.Resolve)
            {
                dif = characterBuild.attributeSheet.resolve.value - baselineAttributes.resolve.value;
            }
            else if (att == CoreAttribute.Wits)
            {
                dif = characterBuild.attributeSheet.wits.value - baselineAttributes.wits.value;
            }

            Debug.Log("MainMenuController.GetCharacterAttributeDifference() returning " + dif.ToString() +
                " for attribute: " + att.ToString());

            return dif;
        }
        private int GetCharacterAttributeValue(CoreAttribute att)
        {
            int value = 0;

            if (att == CoreAttribute.Strength)            
                value = characterBuild.attributeSheet.strength.value;
            
            else if (att == CoreAttribute.Intelligence)            
                value = characterBuild.attributeSheet.intelligence.value;
            
            else if (att == CoreAttribute.Accuracy)            
                value = characterBuild.attributeSheet.accuracy.value;
            
            else if (att == CoreAttribute.Dodge)            
                value = characterBuild.attributeSheet.dodge.value;
            
            else if (att == CoreAttribute.Constituition)            
                value = characterBuild.attributeSheet.constitution.value;
            
            else if (att == CoreAttribute.Resolve)            
                value = characterBuild.attributeSheet.resolve.value;
            
            else if (att == CoreAttribute.Wits)            
                value = characterBuild.attributeSheet.wits.value;
            

            Debug.Log("MainMenuController.GetCharacterAttributeValue) returning " + value.ToString() +
               " for attribute: " + att.ToString());

            return value;
        }
        private int GetTotalAttributePointsSpent()
        {
            int dif = 0;
            dif += GetCharacterAttributeDifference(CoreAttribute.Strength) ;
            dif += GetCharacterAttributeDifference(CoreAttribute.Intelligence) ;
            dif += GetCharacterAttributeDifference(CoreAttribute.Constituition) ;
            dif += GetCharacterAttributeDifference(CoreAttribute.Accuracy) ;
            dif += GetCharacterAttributeDifference(CoreAttribute.Dodge) ;
            dif += GetCharacterAttributeDifference(CoreAttribute.Resolve) ;
            dif += GetCharacterAttributeDifference(CoreAttribute.Wits) ;

            Debug.Log("MainMenuController.GetTotalAttributePointsSpent() returning: " + dif.ToString());
            return dif;
        }           
        private void ApplyAttributesFromPreset(HexCharacterData preset)
        {
            // Reset to base stats
            preset.attributeSheet.CopyValuesIntoOther(characterBuild.attributeSheet);

            // Apply stat difference from preset
            /*
            characterBuild.attributeSheet.strength.value += preset.attributeSheet.strength.value - characterBuild.attributeSheet.strength.value;
            characterBuild.attributeSheet.intelligence.value += preset.attributeSheet.intelligence.value - characterBuild.attributeSheet.intelligence.value;
            characterBuild.attributeSheet.accuracy.value += preset.attributeSheet.accuracy.value - characterBuild.attributeSheet.accuracy.value;
            characterBuild.attributeSheet.dodge.value += preset.attributeSheet.dodge.value - characterBuild.attributeSheet.dodge.value;
            characterBuild.attributeSheet.resolve.value += preset.attributeSheet.resolve.value - characterBuild.attributeSheet.resolve.value;
            characterBuild.attributeSheet.constitution.value += preset.attributeSheet.constitution.value - characterBuild.attributeSheet.constitution.value;
            characterBuild.attributeSheet.wits.value += preset.attributeSheet.wits.value - characterBuild.attributeSheet.wits.value;
        
        */}
        public void OnDecreaseAttributeButtonClicked(CustomCharacterAttributeRow row)
        {
            if (row.Attribute == CoreAttribute.Strength)
                characterBuild.attributeSheet.strength.value -= 1;
            else if (row.Attribute == CoreAttribute.Intelligence)
                characterBuild.attributeSheet.intelligence.value -= 1;
            else if (row.Attribute == CoreAttribute.Accuracy)
                characterBuild.attributeSheet.accuracy.value -= 1;
            else if (row.Attribute == CoreAttribute.Dodge)
                characterBuild.attributeSheet.dodge.value -= 1;
            else if (row.Attribute == CoreAttribute.Constituition)
                characterBuild.attributeSheet.constitution.value -= 1;
            else if (row.Attribute == CoreAttribute.Resolve)
                characterBuild.attributeSheet.resolve.value -= 1;
            else if (row.Attribute == CoreAttribute.Wits)
                characterBuild.attributeSheet.wits.value -= 1;

            RebuildAttributeSection();
        }
        public void OnIncreaseAttributeButtonClicked(CustomCharacterAttributeRow row)
        {
            if (row.Attribute == CoreAttribute.Strength)
                characterBuild.attributeSheet.strength.value += 1;
            else if (row.Attribute == CoreAttribute.Intelligence)
                characterBuild.attributeSheet.intelligence.value += 1;
            else if (row.Attribute == CoreAttribute.Accuracy)
                characterBuild.attributeSheet.accuracy.value += 1;
            else if (row.Attribute == CoreAttribute.Dodge)
                characterBuild.attributeSheet.dodge.value += 1;
            else if (row.Attribute == CoreAttribute.Constituition)
                characterBuild.attributeSheet.constitution.value += 1;
            else if (row.Attribute == CoreAttribute.Resolve)
                characterBuild.attributeSheet.resolve.value += 1;
            else if (row.Attribute == CoreAttribute.Wits)
                characterBuild.attributeSheet.wits.value += 1;

            RebuildAttributeSection();
        }

        #endregion

        // Custom Character Screen Logic : Update Model + Race
        #region
        private void HandleChangeRace(CharacterRace race)
        {
            // Get + cache race data
            currentCustomRace = CharacterDataController.Instance.GetRaceData(race);
            characterBuild.race = currentCustomRace.racialTag;

            // Build race icon image
            originPanelRacialIcon.BuildFromRacialData(currentCustomRace);

            // Build racial texts
            originPanelRacialDescriptionText.text = currentCustomRace.loreDescription;
            originPanelRacialNameText.text = currentCustomRace.racialTag.ToString();

            // Rebuild UCM as default model of race
            HandleChangeRacialModel(GetRacialModelBasket(characterBuild.race).templates[0]);
        }
        private void HandleChangeRacialModel(CharacterModelTemplateSO newModel)
        {
            currentModelTemplate = newModel;
            CharacterModeller.BuildModelFromStringReferences(customCharacterScreenUCM, newModel.bodyParts);
        }
        public void OnChooseRaceNextButtonClicked()
        {
            Debug.Log("MainMenuController.OnChooseRaceNextButtonClicked() called...");
            CharacterRace[] playableRaces = CharacterDataController.Instance.PlayableRaces.ToArray();
            CharacterRace nextValidRace = CharacterRace.None;
            int currentIndex = Array.IndexOf(playableRaces, currentCustomRace.racialTag);

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
            int currentIndex = Array.IndexOf(playableRaces, currentCustomRace.racialTag);

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

        // Choose Character Screen Logic
        #region
        public void SetChooseCharacterBoxStartingStates()
        {
            for(int i = 0; i < allChooseCharacterBoxes.Length; i++)
            {
                SetChooseCharacterBoxCharacterSelection(allChooseCharacterBoxes[i], CharacterDataController.Instance.AllCharacterTemplates[i]);
            }
        }
        private void SetChooseCharacterBoxCharacterSelection(ChooseCharacterBox box, HexCharacterData character)
        {
            Debug.Log("MainMenuController.SetChooseCharacterBoxCharacterSelection() new character selection = " + character.myClassName);
            box.currentCharacterSelection = character;
            box.ClassNameText.text = character.myClassName;
        }
        private bool IsCharacterAlreadySelected(HexCharacterData character)
        {
            bool bRet = false;
            foreach(ChooseCharacterBox box in allChooseCharacterBoxes)
            {
                if(box.currentCharacterSelection == character)
                {
                    bRet = true;
                    break;
                }
            }

            return bRet;
        }        
        public List<HexCharacterData> GetChosenCharacterDataFiles()
        {
            List<HexCharacterData> characters = new List<HexCharacterData>();
            foreach (ChooseCharacterBox box in allChooseCharacterBoxes)
                characters.Add(box.currentCharacterSelection);
            return characters;
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
        }
        public void HideFrontScreen()
        {
            frontScreenParent.SetActive(false);
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
            frontScreenBgParent.transform.DOScale(1.2f, 0f);
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