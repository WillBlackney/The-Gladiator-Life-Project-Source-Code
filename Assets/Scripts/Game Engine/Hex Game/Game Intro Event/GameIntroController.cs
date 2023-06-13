using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using HexGameEngine.Characters;
using HexGameEngine.UI;
using HexGameEngine.Persistency;
using HexGameEngine.JourneyLogic;
using HexGameEngine.Audio;

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
        [SerializeField] Sprite pageOneSprite;
        [TextArea(0, 200)]
        [SerializeField] string pageOneBodyText;

        [Space(20)]

        [Header("Page Two Settings")]
        [SerializeField] Sprite pageTwoSprite;
        [TextArea(0, 200)]
        [SerializeField] string pageTwoBodyText;

        [Space(20)]

        [Header("Page Three Settings")]
        [SerializeField] Sprite pageThreeSprite;
        [TextArea(0,200)]
        [SerializeField] string pageThreeBodyText;

        [Space(20)]

        [Header("Page Four Settings")]
        [SerializeField] Sprite pageFourSprite;
        [TextArea(0, 200)]
        [SerializeField] string pageFourBodyText;

        [Space(20)]

        [Header("Page Four Settings")]
        [SerializeField] Sprite pageFiveSprite;
        [TextArea(0, 200)]
        [SerializeField] string pageFiveBodyText;


        private List<HexCharacterData> offeredRecruits = new List<HexCharacterData>();


        #region Start + End Event Logic
        public void StartEvent()
        {
            offeredRecruits = GenerateRecruits();

            // Reset            
            mainVisualParent.SetActive(false);
            pageMovementParent.DOKill();
            pageMovementParent.DOMove(pageOffscreenPos.position, 0f);
            blackUnderlay.DOKill();
            blackUnderlay.DOFade(0f, 0f);

            // Build first page content
            mainVisualParent.SetActive(true);
            BuildViewsAsPageOne();

            // Fade in and move page down
            blackUnderlay.DOFade(0.5f, 0.5f).OnComplete(() =>
            {
                AudioManager.Instance.PlaySound(Sound.Effects_Story_Event_Start);
                pageMovementParent.DOMove(pageOffscreenPos.position, 0f);
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

        private void BuildViewsAsPageOne()
        {
            ResetPageBeforeNextPageBuilt();
            BuildPageTextBodies(pageOneBodyText);
            headerPanelText.text = pageOneHeaderText;
            eventImage.sprite = pageOneSprite;
            choiceButtons[0].BuildAndShow("This had better be worth it...", () =>
            {
                BuildViewsAsPageTwo();
            });

            StartCoroutine(TransformUtils.RebuildLayoutsNextFrame(allFitters));
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

            mainVisualParent.SetActive(false);
            TransformUtils.RebuildLayouts(allFitters);

            mainVisualParent.SetActive(true);
            StartCoroutine(TransformUtils.RebuildLayoutsNextFrame(allFitters));

        }
        private void BuildViewsAsPageThree()
        {
            ResetPageBeforeNextPageBuilt();
            eventImage.sprite = pageThreeSprite;

            HexCharacterData characterOne = offeredRecruits[0];
            HexCharacterData characterTwo = offeredRecruits[1];

            string initialSpeechText = pageThreeBodyText;           
            string characterOneDetails = GenerateCharacterIntroString(characterOne, "by the name of");
            string characterTwoDetails = GenerateCharacterIntroString(characterTwo);
            initialSpeechText = initialSpeechText.Replace("AAA", characterOneDetails);
            initialSpeechText = initialSpeechText.Replace("BBB", characterTwoDetails);
            initialSpeechText = initialSpeechText.Replace("CCC", characterOne.myName);

            BuildPageTextBodies(initialSpeechText);

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
            /*
            choiceButtons[2].BuildAndShow("Neither of you have what it takes.", () =>
            {
                BuildViewsAsPageFour();
            });*/

            StartCoroutine(TransformUtils.RebuildLayoutsNextFrame(allFitters));
        }
        private void BuildViewsAsPageFour()
        {
            CharacterScrollPanelController.Instance.RebuildViews();

            ResetPageBeforeNextPageBuilt();
            eventImage.sprite = pageFourSprite;

            HexCharacterData characterOne = offeredRecruits[2];
            HexCharacterData characterTwo = offeredRecruits[3];
            string initialSpeechText = pageFourBodyText;
            string characterOneDetails = GenerateCharacterIntroString(characterOne, "by the name of");
            string characterOneDetailsPart2 = GetCharacterSubDescriptionFromBackground(characterOne.background.backgroundType);
            string characterTwoDetails = GenerateCharacterIntroString(characterTwo);
            string characterTwoDetailsPart2 = GetCharacterSubDescriptionFromBackground(characterTwo.background.backgroundType);

            initialSpeechText = initialSpeechText.Replace("AAA", characterOneDetails + characterOneDetailsPart2);
            initialSpeechText = initialSpeechText.Replace("BBB", characterTwoDetails + characterTwoDetailsPart2);


            BuildPageTextBodies(initialSpeechText);

            string buttonOneText = System.String.Format("We could use someone like {0} the {1}.", characterOne.myName, TextLogic.SplitByCapitals(characterOne.background.backgroundType.ToString()));
            string buttonTwoText = System.String.Format("{0} the {1} {2} is the type of killer we need.", characterTwo.myName, characterTwo.race.ToString(), TextLogic.SplitByCapitals(characterTwo.background.backgroundType.ToString()));

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
            /*
            choiceButtons[2].BuildAndShow("No thanks, you both look useless.", () =>
            {
                BuildViewsAsPageFive();
            });*/

            StartCoroutine(TransformUtils.RebuildLayoutsNextFrame(allFitters));
        }
        private void BuildViewsAsPageFive()
        {
            CharacterScrollPanelController.Instance.RebuildViews();
            eventImage.sprite = pageFiveSprite;
            ResetPageBeforeNextPageBuilt();

            string finalText = pageFiveBodyText;
            finalText = finalText.Replace("AAA", CharacterDataController.Instance.AllPlayerCharacters[0].myName);
            finalText = finalText.Replace("BBB", CharacterDataController.Instance.AllPlayerCharacters[1].myName);
            finalText = finalText.Replace("CCC", CharacterDataController.Instance.AllPlayerCharacters[2].myName);

            BuildPageTextBodies(finalText);

            choiceButtons[0].BuildAndShow("Gold and glory shall be ours!", () =>
            {
                FinishEvent();
            });

            StartCoroutine(TransformUtils.RebuildLayoutsNextFrame(allFitters));
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
                ret.Add(CharacterDataController.Instance.GenerateRecruitCharacter(data, false));
            }

            // Second pool
            for (int i = 0; i < 2 && i < possibleCharacterPoolTwo.Length; i++)
            {
                BackgroundData data = CharacterDataController.Instance.GetBackgroundData(possibleCharacterPoolTwo[i]);
                ret.Add(CharacterDataController.Instance.GenerateRecruitCharacter(data, false));
            }

            return ret;
        }
        private string GenerateCharacterIntroString(HexCharacterData character, string joiner = "called")
        {
            string ret = "";

            string prefix = "a";
            if (character.race == CharacterRace.Undead || character.race == CharacterRace.Orc || character.race == CharacterRace.Elf) prefix = "an";

            ret = System.String.Format("{0} {1} {2} {3} {4}", 
                prefix, 
                character.race.ToString(), 
                TextLogic.SplitByCapitals(character.background.backgroundType.ToString()),
                joiner,
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
            bodyText.text = text;
            TransformUtils.RebuildLayout(bodyText2.transform as RectTransform);
            //if (bodyText2.text == "") bodyText2.gameObject.SetActive(false);
        }
        private string GetCharacterSubDescriptionFromBackground(CharacterBackground bg)
        {
            string ret = "";

            if(bg == CharacterBackground.Scholar)
            {
                ret = ", whose blue robes sparkle with magic and whose staff is primed for action";
            }
            else if (bg == CharacterBackground.Zealot)
            {
                ret = " covered from head to toe in grim tattoos and religious markings";
            }
            else if (bg == CharacterBackground.Poacher)
            {
                ret = ", whose rugged exterior and sharp wit betray a lifetime of surviving in the wild";
            }

            return ret;
        }
        #endregion
    }
}