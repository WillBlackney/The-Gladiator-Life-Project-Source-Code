using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using HexGameEngine.Characters;
using Spriter2UnityDX.Importing;
using UnityEngine.TextCore.Text;
using HexGameEngine.UI;
using HexGameEngine.Persistency;
using HexGameEngine.JourneyLogic;

namespace HexGameEngine.GameIntroEvent
{
    public class GameIntroController : Singleton<GameIntroController>
    {
        [Header("Core Components")]
        [SerializeField] GameObject mainVisualParent;
        [SerializeField] GameObject topSectionParent;
        [SerializeField] TextMeshProUGUI headerPanelText;
        [SerializeField] Image eventImage;
        [SerializeField] TextMeshProUGUI bodyText;
        [SerializeField] TextMeshProUGUI bodyText2;
        [SerializeField] Scrollbar contentScrollbar;
        [SerializeField] GameIntroButton[] choiceButtons;
        [SerializeField] RectTransform[] allFitters;
        [SerializeField] CharacterBackground[] possibleCharacterPoolOne;
        [SerializeField] CharacterBackground[] possibleCharacterPoolTwo;

        [Header("Page Animation Components")]
        [SerializeField] RectTransform pageMovementParent;
        [SerializeField] RectTransform pageOffscreenPos;
        [SerializeField] RectTransform pageOnscreenPos;
        [SerializeField] Image blackUnderlay;        
        
        [Header("Page One Settings")]
        [SerializeField] string pageOneHeaderText;
        [TextArea]
        [SerializeField] string pageOneBodyText;
        [SerializeField] Sprite pageOneSprite;

        [Header("Page Two Settings")]
        [TextArea]
        [SerializeField] string pageTwoBodyText;
        [SerializeField] Sprite pageTwoSprite;

        private List<HexCharacterData> offeredRecruits = new List<HexCharacterData>();

      

        #region Start + End Event Logic
        public void StartEvent()
        {
            offeredRecruits = GenerateRecruits();

            // Reset
            ResetPageBeforeNextPageBuilt();
            mainVisualParent.SetActive(false);
            pageMovementParent.DOKill();
            pageMovementParent.DOMove(pageOffscreenPos.position, 0f);
            blackUnderlay.DOKill();
            blackUnderlay.DOFade(0f, 0f);
            ResetChoiceButtons();

            // Build first page content
            mainVisualParent.SetActive(true);
            BuildViewsAsPageOne();
            TransformUtils.RebuildLayouts(allFitters);

            // Fade in and move page down
            blackUnderlay.DOFade(0.5f, 0.75f);
            pageMovementParent.DOMove(pageOnscreenPos.position, 0.75f).SetEase(Ease.OutBack);
        }
        private void FinishEvent()
        {
            ResetChoiceButtons();
            pageMovementParent.DOKill();
            pageMovementParent.DOMove(pageOffscreenPos.position, 0.5f).SetEase(Ease.InBack);
            blackUnderlay.DOKill();
            blackUnderlay.DOFade(0f, 0.55f).OnComplete(() =>
            {
                mainVisualParent.SetActive(false);
                RunController.Instance.SetCheckPoint(SaveCheckPoint.Town);
                GameController.Instance.SetGameState(GameState.Town);
                PersistencyController.Instance.AutoUpdateSaveFile();
            });
        }
        #endregion       
       
        #region Build Specific Pages

        private void BuildViewsAsPageOne()
        {
            ResetPageBeforeNextPageBuilt();
            headerPanelText.text = pageOneHeaderText;
            bodyText2.gameObject.SetActive(true);
            bodyText.text = pageOneBodyText;
            if (bodyText2.text == "") bodyText2.gameObject.SetActive(false);
            eventImage.sprite = pageOneSprite;
            choiceButtons[0].BuildAndShow("This had better be worth it...", () =>
            {
                BuildViewsAsPageTwo();
            });

            TransformUtils.RebuildLayouts(allFitters);
        }
        private void BuildViewsAsPageTwo()
        {
            ResetPageBeforeNextPageBuilt();
            BuildPageTextBodies(pageTwoBodyText);
            eventImage.sprite = pageTwoSprite;
            choiceButtons[0].BuildAndShow("Eh, why not? What's the worst that could happen?", () =>
            {
                BuildViewsAsPageThree();
            });

            TransformUtils.RebuildLayouts(allFitters);
        }
        private void BuildViewsAsPageThree()
        {
            ResetPageBeforeNextPageBuilt();
            //eventImage.sprite = pageTwoSprite;

            HexCharacterData characterOne = offeredRecruits[0];
            HexCharacterData characterTwo = offeredRecruits[1];
            string initialSpeechText = "You make a speech blah blah blah. Two members of the crowd step forward eagerly: ";
            string characterOneDetails = GenerateCharacterIntroString(characterOne);
            string characterTwoDetails = GenerateCharacterIntroString(characterTwo);

            string finalText = System.String.Format("{0}{1}, and {2}.", initialSpeechText, characterOneDetails, characterTwoDetails);
            BuildPageTextBodies(finalText);

            string buttonOneText = System.String.Format("{0} the {1} {2} seems like a good pick.", characterOne.myName, characterOne.race.ToString(), TextLogic.SplitByCapitals(characterOne.background.backgroundType.ToString()));
            string buttonTwoText = System.String.Format("{0} the {1} {2} would fit right in us.", characterTwo.myName, characterTwo.race.ToString(), TextLogic.SplitByCapitals(characterTwo.background.backgroundType.ToString()));

            choiceButtons[0].BuildAndShow(buttonOneText, () =>
            {
                CharacterDataController.Instance.AddCharacterToRoster(characterOne);
                BuildViewsAsPageFour();
            });
            choiceButtons[1].BuildAndShow(buttonTwoText, () =>
            {
                CharacterDataController.Instance.AddCharacterToRoster(characterTwo);
                BuildViewsAsPageFour();
            });
            choiceButtons[2].BuildAndShow("Neither of you have what it takes.", () =>
            {
                BuildViewsAsPageFour();
            });

            TransformUtils.RebuildLayouts(allFitters);
        }
        private void BuildViewsAsPageFour()
        {
            CharacterScrollPanelController.Instance.RebuildViews();

            ResetPageBeforeNextPageBuilt();
            //eventImage.sprite = pageTwoSprite;

            HexCharacterData characterOne = offeredRecruits[2];
            HexCharacterData characterTwo = offeredRecruits[3];
            string initialSpeechText = "The next round of prospects step up. They are ";
            string characterOneDetails = GenerateCharacterIntroString(characterOne);
            string characterTwoDetails = GenerateCharacterIntroString(characterTwo);

            string finalText = System.String.Format("{0}{1}, and {2}.", initialSpeechText, characterOneDetails, characterTwoDetails);
            BuildPageTextBodies(finalText);

            string buttonOneText = System.String.Format("We could use someone like {0} the {1}.", characterOne.myName, TextLogic.SplitByCapitals(characterOne.background.backgroundType.ToString()));
            string buttonTwoText = System.String.Format("{0} the {1} {2} will do well.", characterTwo.myName, characterTwo.race.ToString(), TextLogic.SplitByCapitals(characterTwo.background.backgroundType.ToString()));

            choiceButtons[0].BuildAndShow(buttonOneText, () =>
            {
                CharacterDataController.Instance.AddCharacterToRoster(characterOne);
                BuildViewsAsPageFive();
            });
            choiceButtons[1].BuildAndShow(buttonTwoText, () =>
            {
                CharacterDataController.Instance.AddCharacterToRoster(characterTwo);
                BuildViewsAsPageFive();
            });
            choiceButtons[2].BuildAndShow("No thanks, you both look useless.", () =>
            {
                BuildViewsAsPageFive();
            });

            TransformUtils.RebuildLayouts(allFitters);
        }
        private void BuildViewsAsPageFive()
        {
            CharacterScrollPanelController.Instance.RebuildViews();

            ResetPageBeforeNextPageBuilt();

            string finalText = "You have your starting gladiators, now it's time to go make a name for yourselves!";         
            BuildPageTextBodies(finalText);

            choiceButtons[0].BuildAndShow("Glory shall be ours!", () =>
            {
                FinishEvent();
            });

        }
        #endregion

        #region Misc Logic
        private void ResetChoiceButtons()
        {
            foreach (GameIntroButton b in choiceButtons) b.HideAndReset();
        }
        private List<HexCharacterData> GenerateRecruits()
        {
            List<HexCharacterData> ret = new List<HexCharacterData>();
            possibleCharacterPoolOne.Shuffle();
            possibleCharacterPoolTwo.Shuffle();

            // First pool
            for (int i = 0; i < 2 && i < possibleCharacterPoolOne.Length; i++)
            {
                BackgroundData data = CharacterDataController.Instance.GetBackgroundData(possibleCharacterPoolOne[i]);
                ret.Add(CharacterDataController.Instance.GenerateRecruitCharacter(data));
            }

            // Second pool
            for (int i = 0; i < 2 && i < possibleCharacterPoolTwo.Length; i++)
            {
                BackgroundData data = CharacterDataController.Instance.GetBackgroundData(possibleCharacterPoolTwo[i]);
                ret.Add(CharacterDataController.Instance.GenerateRecruitCharacter(data));
            }

            return ret;
        }
        private string GenerateCharacterIntroString(HexCharacterData character)
        {
            string ret = "";

            string prefix = "a";
            if (character.race == CharacterRace.Undead || character.race == CharacterRace.Orc || character.race == CharacterRace.Elf) prefix = "an";

            ret = System.String.Format("{0} {1} {2} called {3}", 
                prefix, 
                character.race.ToString(), 
                TextLogic.SplitByCapitals(character.background.backgroundType.ToString()),
                character.myName);

            return ret;
        }
        private void ResetPageBeforeNextPageBuilt()
        {
            ResetChoiceButtons();
            contentScrollbar.value = 1f;
        }
        public void HideAllViews()
        {
            mainVisualParent.SetActive(false);
        }
        private void BuildPageTextBodies(string text)
        {
            bodyText2.gameObject.SetActive(true);
            bodyText.text = text;
            if (bodyText2.text == "") bodyText2.gameObject.SetActive(false);
        }
        #endregion
    }
}