using DG.Tweening;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.Items;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Persistency;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Scoring
{
    public class ScoreController : Singleton<ScoreController>
    {
        #region Components + Variables
        [Header("Core Components")]
        [SerializeField] GameObject visualParent;
        [SerializeField] CanvasGroup mainCg;
        [SerializeField] TextMeshProUGUI headerText;
        [SerializeField] Button continueButton;
        [SerializeField] ScoreElementPanel[] scorePanels;
        [SerializeField] TextMeshProUGUI finalScoreText;
        [SerializeField] GameObject contentCompleteWindow;

        private PlayerScoreTracker currentScoreData;
        #endregion

        #region Getters + Accessors
        public PlayerScoreTracker CurrentScoreData
        {
            get { return currentScoreData; }
            private set { currentScoreData = value; }
        }
        #endregion

        #region Initialization
        private void Start()
        {
            SetupContinueButton();
        }
        private void OnEnable()
        {
            SetupContinueButton();
        }
        private void SetupContinueButton()
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueButtonClicked);
        }
        #endregion


        #region Persistency
        public void GenerateGameStartValues()
        {
            currentScoreData = new PlayerScoreTracker();
        }
        public void BuildMyDataFromSaveFile(SaveGameData saveFile)
        {
            CurrentScoreData = saveFile.scoreData;
        }
        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            saveFile.scoreData = CurrentScoreData;
        }
        public void GenerateNewScoreDataOnGameStart()
        {
            currentScoreData = new PlayerScoreTracker();
        }
        #endregion

        #region UI Logic
        public void CalculateAndShowScoreScreen(PlayerScoreTracker data, bool defeat = false)
        {
            Debug.LogWarning("CalculateAndShowScoreScreen() called, defeat  = " + defeat.ToString());
            List<ScoreElementData> scoreElements = GenerateScoringElements(data);
            if(defeat)
            {
                contentCompleteWindow.gameObject.SetActive(false);
                headerText.text = "The journey ends...";
                AudioManager.Instance.PlaySound(Sound.Music_Defeat_Fanfare);
            }
            else
            {
                contentCompleteWindow.gameObject.SetActive(true);
                headerText.text = "A triumphant end!";
                AudioManager.Instance.PlaySound(Sound.Music_Victory_Fanfare);
            }
            HideAndResetAllScoreTabs();
            BuildAllScoreTabs(scoreElements);
            finalScoreText.text = "Your score:   " + TextLogic.ReturnColoredText(CalculateFinalScore(scoreElements).ToString(), TextLogic.neutralYellow);
            RevealScoreScreen();
            DelayUtils.DelayedCall(1.25f, () => StartCoroutine(FadeInScoreTabs()));

        }
        private void RevealScoreScreen()
        {
            visualParent.SetActive(true);
            continueButton.interactable = false;
            continueButton.GetComponent<CanvasGroup>().DOFade(0, 0);
            mainCg.alpha = 0.02f;
            mainCg.DOFade(1f, 1f);
        }
        public void HideScoreScreen(float speed = 1f)
        {
            mainCg.DOFade(speed, 0f).OnComplete(() => visualParent.SetActive(false));
        }
        private IEnumerator FadeInScoreTabs()
        {
            foreach (ScoreElementPanel t in scorePanels)
            {
                if (t.IsActive)
                {
                    t.gameObject.SetActive(true);
                    t.MyCg.DOFade(1, 0.5f);
                    yield return new WaitForSeconds(0.25f);
                }
            }
            continueButton.interactable = false;
            continueButton.GetComponent<CanvasGroup>().DOFade(1, 0.75f).OnComplete(() => continueButton.interactable = true);
        }
        private void BuildAllScoreTabs(List<ScoreElementData> scoreElements)
        {
            for (int i = 0; i < scoreElements.Count; i++)
            {
                // prevent exceeding array bounds accidently (max 15 score tab views, but there could be 15+ score elements)
                if (i > scorePanels.Length - 1)
                    break;

                ScoreElementPanel tab = scorePanels[i];
                ScoreElementData data = scoreElements[i];

                tab.gameObject.SetActive(true);
                tab.IsActive = true;
                tab.Description = GetScoreElementDescription(data.type);
                tab.ValueText.text = data.totalScore.ToString();
                tab.NameText.text = TextLogic.SplitByCapitals(data.type.ToString());
                tab.MyCg.alpha = 0.02f;
            }
        }
        private void HideAndResetAllScoreTabs()
        {
            foreach (ScoreElementPanel tab in scorePanels)
            {
                tab.gameObject.SetActive(false);
                tab.MyCg.alpha = 0.02f;
                tab.IsActive = false;
            }

        }
        private void OnContinueButtonClicked()
        {
            GameController.Instance.HandleQuitToMainMenuFromInGame();
        }
        private string GetScoreElementDescription(ScoreElementType element)
        {
            //return "placeholder description";

            if (element == ScoreElementType.DaysPassed)
            {
                return "Gain " + TextLogic.ReturnColoredText("5", TextLogic.blueNumber) + " for each day passed.";
            }           
            else if (element == ScoreElementType.BasicCombatVictories)
            {
                return "Gain " + TextLogic.ReturnColoredText("5", TextLogic.blueNumber) + " points for each successfully completed basic arena fight.";
            }
            else if (element == ScoreElementType.EliteCombatVictories)
            {
                return "Gain " + TextLogic.ReturnColoredText("10", TextLogic.blueNumber) + " points for each successfully completed elite arena fight.";
            }
            else if (element == ScoreElementType.BossCombatVictories)
            {
                return "Gain " + TextLogic.ReturnColoredText("20", TextLogic.blueNumber) + " points for each successfully completed boss arena fight.";
            }
            else if (element == ScoreElementType.Panache)
            {
                return "Gain " + TextLogic.ReturnColoredText("10", TextLogic.blueNumber) + " points for each basic arena fight completed without losing a character.";
            }
            else if (element == ScoreElementType.GiantSlayer)
            {
                return "Gain " + TextLogic.ReturnColoredText("20", TextLogic.blueNumber) + " points for each elite arena fight completed without losing a character.";
            }
            else if (element == ScoreElementType.Godlike)
            {
                return "Gain " + TextLogic.ReturnColoredText("40", TextLogic.blueNumber) + " points for each boss arena fight completed without losing a character.";
            }
            else if (element == ScoreElementType.WellArmoured)
            {
                return "Gain " + TextLogic.ReturnColoredText("5", TextLogic.blueNumber) + " points for each item collected with " + TextLogic.ReturnColoredText("150", TextLogic.blueNumber) + " or more " +
                    TextLogic.ReturnColoredText("Armour", TextLogic.neutralYellow) + ".";
            }
            else if (element == ScoreElementType.WellArmed)
            {
                return "Gain " + TextLogic.ReturnColoredText("5", TextLogic.blueNumber) + " points for each epic weapon collected.";
            }
            else if (element == ScoreElementType.FatherOfTheYear)
            {
                return "Gain " + TextLogic.ReturnColoredText("20", TextLogic.blueNumber) + " points if 'The Kid' was not killed.";
            }

            // Penalties
            else if (element == ScoreElementType.InjuriesGained)
            {
                return "Lose " + TextLogic.ReturnColoredText("2", TextLogic.blueNumber) + " points for every " +
                    TextLogic.ReturnColoredText("Injury", TextLogic.neutralYellow) + " gained.";
            }
            else if (element == ScoreElementType.PlayerCharactersKilled)
            {
                return "Lose " + TextLogic.ReturnColoredText("5", TextLogic.blueNumber) + " points for each of your characters that was killed.";
            }
            else if (element == ScoreElementType.CombatDefeats)
            {
                return "Lose " + TextLogic.ReturnColoredText("10", TextLogic.blueNumber) + " points for each time you were defeated in combat.";
            }

            return "";


        }
        
        #endregion

        #region Score Calculation Logic

        private List<ScoreElementData> GenerateScoringElements(PlayerScoreTracker scoreData)
        {
            List<ScoreElementData> scoreElements = new List<ScoreElementData>();

            // basics defeated
            if (scoreData.oneSkullContractsCompleted > 0)
                scoreElements.Add(new ScoreElementData(scoreData.oneSkullContractsCompleted * 5, ScoreElementType.BasicCombatVictories));

            // elites defeated
            if (scoreData.twoSkullContractsCompleted > 0)
                scoreElements.Add(new ScoreElementData(scoreData.twoSkullContractsCompleted * 10, ScoreElementType.EliteCombatVictories));

            // bosses defeated
            if (scoreData.threeSkullContractsCompleted > 0)
                scoreElements.Add(new ScoreElementData(scoreData.threeSkullContractsCompleted * 20, ScoreElementType.BossCombatVictories));

            // basic defeated without losing a character
            if (scoreData.oneSkullContractsCompletedWithoutDeath > 0)
                scoreElements.Add(new ScoreElementData(scoreData.oneSkullContractsCompletedWithoutDeath * 10, ScoreElementType.Panache));

            // elites defeated without losing a character
            if (scoreData.twoSkullContractsCompletedWithoutDeath > 0)
                scoreElements.Add(new ScoreElementData(scoreData.twoSkullContractsCompletedWithoutDeath * 20, ScoreElementType.GiantSlayer));

            // bosses defeated without losing a character
            if (scoreData.threeSkullContractsCompletedWithoutDeath > 0)
                scoreElements.Add(new ScoreElementData(scoreData.threeSkullContractsCompletedWithoutDeath * 40, ScoreElementType.Godlike));

            // Penalties
            // Injuries
            if (scoreData.injuriesGained > 0)
                scoreElements.Add(new ScoreElementData(-scoreData.injuriesGained * 2, ScoreElementType.InjuriesGained));

            // Characters KIA
            if (scoreData.playerCharactersKilled > 0)
                scoreElements.Add(new ScoreElementData(-scoreData.playerCharactersKilled * 5, ScoreElementType.PlayerCharactersKilled));

            // Combats Lost
            if (scoreData.combatDefeats > 0)
                scoreElements.Add(new ScoreElementData(-scoreData.combatDefeats * 10, ScoreElementType.CombatDefeats));

            // CALCULATE SCORING ELEMENTS THAT ONLY OCCUR AT RUN END!


            // Days passed
            ScoreElementData dp = CalculateDaysPassedScore();
            if (dp.totalScore > 0)
                scoreElements.Add(dp);

            // Father of the Year
            ScoreElementData foty = CalculateFatherOfTheYear();
            if (foty.totalScore > 0)
                scoreElements.Add(foty);

            // Well Armed
            ScoreElementData wellArmed = CalculateWellArmedScore();
            if (wellArmed.totalScore > 0)
                scoreElements.Add(wellArmed);

            // Well Armoured
            ScoreElementData wellArmoured = CalculateWellArmouredScore();
            if (wellArmoured.totalScore > 0)
                scoreElements.Add(wellArmoured);

            return scoreElements;
        }
        private int CalculateFinalScore(List<ScoreElementData> scoreElements)
        {
            int score = 0;
            foreach (ScoreElementData s in scoreElements)
                score += s.totalScore;

            return score;
        }
        #endregion


        #region Specific Score Calculators
        private ScoreElementData CalculateWellArmedScore()
        {
            // Gain +5 score for each tier 3 weapon collected
            int score = 0;

            foreach(InventoryItem i in InventoryController.Instance.Inventory)
            {
                if(i.itemData != null &&
                    i.itemData.rarity == Rarity.Epic &&
                    i.itemData.itemType == ItemType.Weapon)
                {
                    score += 5;
                }
            }

            foreach(HexCharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
            {
                if (character.itemSet.mainHandItem != null && character.itemSet.mainHandItem.rarity == Rarity.Epic) score += 5;
                if (character.itemSet.offHandItem != null && character.itemSet.offHandItem.rarity == Rarity.Epic) score += 5;
            }

            return new ScoreElementData(score, ScoreElementType.WellArmed);
        }
        private ScoreElementData CalculateWellArmouredScore()
        {
            // Gain +5 score for head/body piece with 150 or more armour
            int score = 0;

            foreach (InventoryItem i in InventoryController.Instance.Inventory)
            {
                if (i.itemData != null &&
                    i.itemData.armourAmount >= 150 &&
                    (i.itemData.itemType == ItemType.Head || i.itemData.itemType == ItemType.Body))
                {
                    score += 5;
                }
            }

            foreach (HexCharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
            {
                if (character.itemSet.headArmour != null &&
                    character.itemSet.headArmour.armourAmount >= 150)
                {
                    score += 5;
                }
                if (character.itemSet.bodyArmour != null &&
                   character.itemSet.bodyArmour.armourAmount >= 150)
                {
                    score += 5;
                }
            }

            return new ScoreElementData(score, ScoreElementType.WellArmed);
        }
        private ScoreElementData CalculateFatherOfTheYear()
        {
            // Gain +20 score if the kid is still alive.
            int score = 0;

            foreach(HexCharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
            {
                if(character.background.backgroundType == CharacterBackground.TheKid)
                {
                    score += 20;
                    break;
                }
            }

            return new ScoreElementData(score, ScoreElementType.FatherOfTheYear);
        }
        private ScoreElementData CalculateDaysPassedScore()
        {
            // Gain +5 score for each day passed
            int score = RunController.Instance.CurrentDay * 5;           

            return new ScoreElementData(score, ScoreElementType.DaysPassed);
        }
        #endregion
    }
}