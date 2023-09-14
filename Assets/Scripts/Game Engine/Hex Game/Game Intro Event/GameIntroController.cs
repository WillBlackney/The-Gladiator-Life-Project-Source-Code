using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Persistency;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.GameIntroEvent
{
    public class GameIntroController : Singleton<GameIntroController>
    {
        [Header("Core Components")]
        [SerializeField]
        private GameObject mainVisualParent;
        [SerializeField] private TextMeshProUGUI headerPanelText;
        [SerializeField] private Image eventImage;
        [SerializeField] private TextMeshProUGUI bodyText;
        [SerializeField] private ScrollRect mainContentScrollView;
        [SerializeField] private Scrollbar contentScrollbar;
        [SerializeField] private GameIntroButton[] choiceButtons;
        [SerializeField] private RectTransform[] allFitters;
        [SerializeField] private HexCharacterTemplateSO[] possibleCharacterPoolOne;
        [SerializeField] private HexCharacterTemplateSO[] possibleCharacterPoolTwo;
        [SerializeField] private GameIntroPageData[] allPages;

        [Header("Page Animation Components")]
        [SerializeField]
        private RectTransform pageMovementParent;
        [SerializeField] private RectTransform pageOffscreenPos;
        [SerializeField] private RectTransform pageOnscreenPos;
        [SerializeField] private Image blackUnderlay;

        [Header("Page One Settings")]
        [SerializeField]
        private string pageOneHeaderText;
        [SerializeField] private Sprite pageOneSprite;
        [TextArea(0, 200)]
        [SerializeField]
        private string pageOneBodyText;

        [Header("Final Pages")]
        [SerializeField]
        private GameIntroPageData nipplePincherPage;
        [SerializeField] private GameIntroPageData broomWielderPage;
        [SerializeField] private GameIntroPageData militiamanPage;
        [SerializeField] private GameIntroPageData noChoicePage;

        private readonly List<HexCharacterData> chosenCharacters = new List<HexCharacterData>();

        public bool ViewIsActive => mainVisualParent.activeSelf;

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

            for (int i = 0; i < pageData.Choices.Length; i++)
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
            if (button.ChoiceData == null)
            {
                return;
            }
            if (button.ChoiceData.charactersGained != null)
            {
                button.ChoiceData.charactersGained.ForEach(c =>
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
            foreach (GameIntroButton b in choiceButtons)
            {
                b.HideAndReset();
            }
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
