using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using WeAreGladiators.Characters;
using WeAreGladiators.UI;
using WeAreGladiators.Persistency;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Audio;
using System;
using Sirenix.Utilities;
using System.Linq;

namespace WeAreGladiators.GameIntroEvent
{
    public class GameIntroController : Singleton<GameIntroController>
    {
        [Header("Core Components")]
        [SerializeField] GameObject mainVisualParent;
        [SerializeField] TextMeshProUGUI headerPanelText;
        [SerializeField] Image eventImage;
        [SerializeField] TextMeshProUGUI bodyText;
        [SerializeField] ScrollRect mainContentScrollView;
        [SerializeField] Scrollbar contentScrollbar;
        [SerializeField] GameIntroButton[] choiceButtons;
        [SerializeField] RectTransform[] allFitters;
        [SerializeField] HexCharacterTemplateSO[] possibleCharacterPoolOne;
        [SerializeField] HexCharacterTemplateSO[] possibleCharacterPoolTwo;
        [SerializeField] GameIntroPageData[] allPages;

        [Header("Page Animation Components")]
        [SerializeField] RectTransform pageMovementParent;
        [SerializeField] RectTransform pageOffscreenPos;
        [SerializeField] RectTransform pageOnscreenPos;
        [SerializeField] Image blackUnderlay;        
        
        [Header("Page One Settings")]
        [SerializeField] string pageOneHeaderText;
        [SerializeField] Sprite pageOneSprite;
        [TextArea(0, 200)]
        [SerializeField] string pageOneBodyText;

        [Header("Final Pages")]
        [SerializeField] GameIntroPageData nipplePincherPage;
        [SerializeField] GameIntroPageData broomWielderPage;
        [SerializeField] GameIntroPageData militiamanPage;
        [SerializeField] GameIntroPageData noChoicePage;

        private List<HexCharacterData> chosenCharacters = new List<HexCharacterData>();

        public bool ViewIsActive
        {
            get { return mainVisualParent.activeSelf; }
        }


        #region Start + End Event Logic
        public void StartEvent()
        {
            chosenCharacters.Clear();

            // Reset            
            mainVisualParent.SetActive(true);
            pageMovementParent.position = pageOffscreenPos.position;
            blackUnderlay.DOFade(0f, 0f);

            // Build first page content
            BuildPageFromTag(PageTag.Start);

            // Fade in and move page down
            blackUnderlay.DOFade(0.5f, 1f).OnComplete(() =>
            {
                AudioManager.Instance.PlaySound(Sound.Effects_Story_Event_Start);
                pageMovementParent.DOMove(pageOnscreenPos.position, 1f).SetEase(Ease.OutBack);
            });
            
        }
        private void FinishEvent()
        {
            RunController.Instance.SetCheckPoint(SaveCheckPoint.Town);
            GameController.Instance.SetGameState(GameState.Town);
            PersistencyController.Instance.AutoUpdateSaveFile();

            ResetChoiceButtons();
            pageMovementParent.DOKill();
            pageMovementParent.DOMove(pageOffscreenPos.position, 0.75f).SetEase(Ease.InBack);
            blackUnderlay.DOKill();
            blackUnderlay.DOFade(0f, 1.25f).OnComplete(() =>
            {
                mainVisualParent.SetActive(false);                
            });
        }
        #endregion       
       
        #region Build Specific Pages

       
        private void BuildPageFromTag(PageTag tag)
        {
            if (tag == PageTag.Final)
            {
                BuildFinalPage();
                return;
            }

            GameIntroPageData pageData = GetPage(tag);
            ResetPageBeforeNextPageBuilt();
            BuildPageTextBodies(pageData.BodyText);
            headerPanelText.text = pageData.HeaderText;
            eventImage.sprite = pageData.PageSprite;

            for(int i = 0; i < pageData.Choices.Length; i++)
            {
                choiceButtons[i].Build(pageData.Choices[i]);
            }

            if (tag.ToString().Contains("Four") && chosenCharacters.Count > 0)
            {
                choiceButtons[2].HideAndReset();
                bodyText.text += "\n \nA tempting proposal indeed, but you've only enough room in the budget for one of them...";
            }

            TransformUtils.RebuildLayouts(allFitters);
        }
        private void BuildFinalPage()
        {
            GameIntroPageData pageData = null;
            if (chosenCharacters[0].background.backgroundType == CharacterBackground.Thief)
            {
                pageData = nipplePincherPage;
            }
            else if (chosenCharacters[0].background.backgroundType == CharacterBackground.Lumberjack)
            {
                pageData = broomWielderPage;
            }
            else if (chosenCharacters[0].background.backgroundType == CharacterBackground.ImperialDeserter)
            {
                pageData = militiamanPage;
            }
            else
            {
                pageData = noChoicePage;
            }

            ResetPageBeforeNextPageBuilt();
            BuildPageTextBodies(pageData.BodyText);
            headerPanelText.text = pageData.HeaderText;
            eventImage.sprite = pageData.PageSprite;
            choiceButtons[0].BuildAndShow(pageData.Choices[0].buttonText, FinishEvent);
            TransformUtils.RebuildLayouts(allFitters);
        }
        #endregion

        #region Misc Logic
        public void HandleChoiceButtonClicked(GameIntroButton button)
        {
            if (button.ChoiceData == null) return;
            if (button.ChoiceData.charactersGained != null)
            {
                button.ChoiceData.charactersGained.ForEach((c) =>
                {
                    HexCharacterData character = CharacterDataController.Instance.ConvertCharacterTemplateToCharacterData(c);
                    CharacterDataController.Instance.RandomizeOpeningEventCharacter(character);
                    CharacterDataController.Instance.AddCharacterToRoster(character);
                    CharacterScrollPanelController.Instance.RebuildViews();
                    chosenCharacters.Add(character);
                });
            }

            List<PageTag> possiblePages = button.ChoiceData.possiblePagesLoaded.ToList();
            possiblePages.Shuffle();
            BuildPageFromTag(possiblePages[0]);

        }
        private void ResetChoiceButtons()
        {
            foreach (GameIntroButton b in choiceButtons) b.HideAndReset();
        }
        private void ResetPageBeforeNextPageBuilt()
        {
            ResetChoiceButtons();
            mainContentScrollView.verticalNormalizedPosition = 1;
            contentScrollbar.value = 1f;
        }
        public void HideAllViews()
        {
            mainVisualParent.SetActive(false);
        }
        private void BuildPageTextBodies(string text)
        {
            bodyText.text = text;
        }
        #endregion

        #region Page Specific On Click Events
        private GameIntroPageData GetPage(PageTag tag)
        {
            return Array.Find(allPages, p => p.PageTag == tag);
        }

        public void Page2bChoice1()
        {

        }
        public void Page2bChoice2()
        {

        }

        #endregion
    }
}