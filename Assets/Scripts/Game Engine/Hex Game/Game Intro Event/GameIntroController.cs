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

            // Second Pool pool
            for (int i = 0; i < 2 && i < possibleCharacterPoolTwo.Length; i++)
            {
                BackgroundData data = CharacterDataController.Instance.GetBackgroundData(possibleCharacterPoolTwo[i]);
                ret.Add(CharacterDataController.Instance.GenerateRecruitCharacter(data));
            }

            return ret;
        }

        public void StartIntroEvent()
        {
            offeredRecruits = GenerateRecruits();

            // Reset
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
            blackUnderlay.DOFade(0.5f, 0.5f);
            pageMovementParent.DOMove(pageOnscreenPos.position, 0.5f).SetEase(Ease.OutBack);
        }
        private void ResetChoiceButtons()
        {
            foreach (GameIntroButton b in choiceButtons) b.Hide();
        }
        private void BuildViewsAsPageOne()
        {
            ResetChoiceButtons();
            contentScrollbar.value = 1f;
            headerPanelText.text = pageOneHeaderText;
            bodyText.text = pageOneBodyText;
            eventImage.sprite = pageOneSprite;
            choiceButtons[0].BuildAndShow("This had better be worth it...", () =>
            {
                BuildViewsAsPageTwo();
            });
        }
        private void BuildViewsAsPageTwo()
        {
            ResetChoiceButtons();
            contentScrollbar.value = 1f;
            bodyText.text = pageTwoBodyText;
            eventImage.sprite = pageTwoSprite;
            choiceButtons[0].BuildAndShow("Eh, why not? What's the worst that could happen?", () =>
            {
                BuildViewsAsPageThree();
            });
        }
        private void BuildViewsAsPageThree()
        {
            ResetChoiceButtons();
            contentScrollbar.value = 1f;
            //eventImage.sprite = pageTwoSprite;

            HexCharacterData characterOne = offeredRecruits[1];
            HexCharacterData characterTwo = offeredRecruits[2];
            string initialSpeechText = "You make a speech blah blah blah. Two members of the crowd step forward eagerly: ";
            string characterOneDetails = GenerateCharacterIntroString(characterOne);
            string characterTwoDetails = GenerateCharacterIntroString(characterTwo);

            bodyText.text = System.String.Format("{0}{1}, and {2}.", initialSpeechText, characterOneDetails, characterTwoDetails);

            string buttonOneText = System.String.Format("{0} the {1} {2} seems like a good pick.", characterOne.myName, characterOne.race.ToString(), TextLogic.SplitByCapitals(characterOne.background.backgroundType.ToString()));
            string buttonTwoText = System.String.Format("{0} the {1} {2} would fit right in us.", characterTwo.myName, characterTwo.race.ToString(), TextLogic.SplitByCapitals(characterTwo.background.backgroundType.ToString()));

            choiceButtons[0].BuildAndShow(buttonOneText, () =>
            {
                CharacterDataController.Instance.AddCharacterToRoster(characterOne);
                //BuildViewsAsPageFour();
            });
            choiceButtons[1].BuildAndShow(buttonTwoText, () =>
            {
                CharacterDataController.Instance.AddCharacterToRoster(characterTwo);
                //BuildViewsAsPageFur();
            });
        }
        public string GenerateCharacterIntroString(HexCharacterData character)
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
    }
}