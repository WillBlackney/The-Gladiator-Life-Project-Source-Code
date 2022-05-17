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

        [Header("Origin Panel Components")]
        [SerializeField] private TextMeshProUGUI originPanelRacialNameText;
        [SerializeField] private UIRaceIcon originPanelRacialIcon;
        [SerializeField] private TextMeshProUGUI originPanelRacialDescriptionText; 
        [SerializeField] private TextMeshProUGUI originPanelPresetNameText;
        [SerializeField] private UIAbilityIcon[] originPanelAbilityIcons;
        [SerializeField] private UITalentRow[] originPanelTalentRows;

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
            currentPreset = CharacterDataController.Instance.AllCustomCharacterTemplates[0];
            characterBuild = ObjectCloner.CloneJSON(CharacterDataController.Instance.AllCustomCharacterTemplates[0]);
            HandleChangeClassPreset(currentPreset);

            // set to template 1 
            RebuildOriginPageView();

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

            // to do: update core attributes, weapons, items

            // Rebuild Origin page
            RebuildOriginPageView();
        }

        #endregion

        // Custom Character Screen Logic : Origin Panel
        #region
        private void RebuildOriginPageView()
        {            
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