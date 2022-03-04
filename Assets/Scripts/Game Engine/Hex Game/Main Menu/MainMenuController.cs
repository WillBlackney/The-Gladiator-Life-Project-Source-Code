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
        [SerializeField] GameObject chooseCharacterScreenVisualParent;
        [SerializeField] private CanvasGroup chooseCharacterScreenCg;
        [SerializeField] ChooseCharacterBox[] allChooseCharacterBoxes;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("In Game Menu Components")]
        [SerializeField] private GameObject inGameMenuScreenParent;
        [SerializeField] private CanvasGroup inGameMenuScreenCg;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
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
        public void ShowChooseCharacterScreen()
        {
            chooseCharacterScreenVisualParent.SetActive(true);
        }
        public void HideChooseCharacterScreen()
        {
            chooseCharacterScreenVisualParent.SetActive(false);
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