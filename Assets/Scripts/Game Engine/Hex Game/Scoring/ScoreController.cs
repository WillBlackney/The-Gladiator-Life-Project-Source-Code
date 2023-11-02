using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Audio;
using WeAreGladiators.Characters;
using WeAreGladiators.Items;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Perks;
using WeAreGladiators.Persistency;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Scoring
{
    public class ScoreController : Singleton<ScoreController>
    {
        #region Components + Variables

        [Header("Core Components")]
        [SerializeField] private GameObject visualParent;
        [SerializeField] private CanvasGroup mainCg;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private Button continueButton;
        [SerializeField] private ScoreElementPanel[] scorePanels;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private GameObject contentCompleteWindow;

        #endregion

        #region Getters + Accessors

        public PlayerScoreTracker CurrentScoreData { get; private set; }

        #endregion

        #region Misc Logic

        public void GenerateMockScoreData()
        {
            CurrentScoreData.basicCombatsCompleted += 2;
            CurrentScoreData.basicCombatsCompletedWithoutDeath += 2;
            CurrentScoreData.eliteCombatsCompleted += 2;
            CurrentScoreData.eliteCombatsCompletedWithoutDeath += 2;
            CurrentScoreData.bossCombatsCompleted += 2;
            CurrentScoreData.bossCombatsCompletedWithoutDeath += 2;
            CurrentScoreData.playerCharactersKilled += 2;
            CurrentScoreData.injuriesGained += 2;
            CurrentScoreData.combatDefeats += 2;
        }

        #endregion     

        #region Score Value Constants

        public const int DAYS_PASSED_SCORE = 5;
        public const int BASIC_COMBAT_VICTORY_SCORE = 5;
        public const int ELITE_COMBAT_VICTORY_SCORE = 10;
        public const int BOSS_COMBAT_VICTORY_SCORE = 20;
        public const int PANACHE_SCORE = 10;
        public const int GIANT_SLAYER_SCORE = 20;
        public const int GODLIKE_SCORE = 40;
        public const int PLAYER_CHARACTERS_KILLED_SCORE = 5;
        public const int INJURIES_GAINED_SCORE = 2;
        public const int COMBAT_DEFEATS_SCORE = 10;
        public const int WELL_ARMED_SCORE = 5;
        public const int WELL_ARMOURED_SCORE = 5;
        public const int FATHER_OF_THE_YEAR_SCORE = 20;

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
            CurrentScoreData = new PlayerScoreTracker();
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
            CurrentScoreData = new PlayerScoreTracker();
        }

        #endregion

        #region UI Logic

        public void CalculateAndShowScoreScreen(PlayerScoreTracker data, bool defeat = false)
        {
            Debug.LogWarning("CalculateAndShowScoreScreen() called, defeat  = " + defeat);
            List<ScoreElementData> scoreElements = GenerateScoringElements(data);
            if (defeat)
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
                {
                    break;
                }

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
                return "Gain " + TextLogic.ReturnColoredText(DAYS_PASSED_SCORE.ToString(), TextLogic.blueNumber) + " for each day passed.";
            }
            if (element == ScoreElementType.BasicCombatVictories)
            {
                return "Gain " + TextLogic.ReturnColoredText(BASIC_COMBAT_VICTORY_SCORE.ToString(), TextLogic.blueNumber) + " points for each successfully completed basic arena fight.";
            }
            if (element == ScoreElementType.EliteCombatVictories)
            {
                return "Gain " + TextLogic.ReturnColoredText(ELITE_COMBAT_VICTORY_SCORE.ToString(), TextLogic.blueNumber) + " points for each successfully completed elite arena fight.";
            }
            if (element == ScoreElementType.BossCombatVictories)
            {
                return "Gain " + TextLogic.ReturnColoredText(BOSS_COMBAT_VICTORY_SCORE.ToString(), TextLogic.blueNumber) + " points for each successfully completed boss arena fight.";
            }
            if (element == ScoreElementType.Panache)
            {
                return "Gain " + TextLogic.ReturnColoredText(PANACHE_SCORE.ToString(), TextLogic.blueNumber) + " points for each basic arena fight completed without losing a character.";
            }
            if (element == ScoreElementType.GiantSlayer)
            {
                return "Gain " + TextLogic.ReturnColoredText(GIANT_SLAYER_SCORE.ToString(), TextLogic.blueNumber) + " points for each elite arena fight completed without losing a character.";
            }
            if (element == ScoreElementType.Godlike)
            {
                return "Gain " + TextLogic.ReturnColoredText(GODLIKE_SCORE.ToString(), TextLogic.blueNumber) + " points for each boss arena fight completed without losing a character.";
            }
            if (element == ScoreElementType.WellArmoured)
            {
                return "Gain " + TextLogic.ReturnColoredText(WELL_ARMOURED_SCORE.ToString(), TextLogic.blueNumber) + " points for each item collected with " + TextLogic.ReturnColoredText("150", TextLogic.blueNumber) + " or more " +
                    TextLogic.ReturnColoredText("Armour", TextLogic.neutralYellow) + ".";
            }
            if (element == ScoreElementType.WellArmed)
            {
                return "Gain " + TextLogic.ReturnColoredText(WELL_ARMED_SCORE.ToString(), TextLogic.blueNumber) + " points for each epic weapon collected.";
            }
            if (element == ScoreElementType.FatherOfTheYear)
            {
                return "Gain " + TextLogic.ReturnColoredText(FATHER_OF_THE_YEAR_SCORE.ToString(), TextLogic.blueNumber) + " points if 'The Kid' did not suffer a " + TextLogic.ReturnColoredText("Permanent Injury", TextLogic.neutralYellow) + ".";
            }

            // Penalties
            if (element == ScoreElementType.InjuriesGained)
            {
                return "Lose " + TextLogic.ReturnColoredText(INJURIES_GAINED_SCORE.ToString(), TextLogic.blueNumber) + " points for every " +
                    TextLogic.ReturnColoredText("Injury", TextLogic.neutralYellow) + " gained.";
            }
            if (element == ScoreElementType.PlayerCharactersKilled)
            {
                return "Lose " + TextLogic.ReturnColoredText(PLAYER_CHARACTERS_KILLED_SCORE.ToString(), TextLogic.blueNumber) + " points for each of your characters that was killed.";
            }
            if (element == ScoreElementType.CombatDefeats)
            {
                return "Lose " + TextLogic.ReturnColoredText(COMBAT_DEFEATS_SCORE.ToString(), TextLogic.blueNumber) + " points for each time you were defeated in combat.";
            }

            return "";

        }

        #endregion

        #region Score Calculation Logic

        private List<ScoreElementData> GenerateScoringElements(PlayerScoreTracker scoreData)
        {
            List<ScoreElementData> scoreElements = new List<ScoreElementData>();

            // basics defeated
            if (scoreData.basicCombatsCompleted > 0)
            {
                scoreElements.Add(new ScoreElementData(scoreData.basicCombatsCompleted * BASIC_COMBAT_VICTORY_SCORE, ScoreElementType.BasicCombatVictories));
            }

            // elites defeated
            if (scoreData.eliteCombatsCompleted > 0)
            {
                scoreElements.Add(new ScoreElementData(scoreData.eliteCombatsCompleted * ELITE_COMBAT_VICTORY_SCORE, ScoreElementType.EliteCombatVictories));
            }

            // bosses defeated
            if (scoreData.bossCombatsCompleted > 0)
            {
                scoreElements.Add(new ScoreElementData(scoreData.bossCombatsCompleted * BOSS_COMBAT_VICTORY_SCORE, ScoreElementType.BossCombatVictories));
            }

            // basic defeated without losing a character
            if (scoreData.basicCombatsCompletedWithoutDeath > 0)
            {
                scoreElements.Add(new ScoreElementData(scoreData.basicCombatsCompletedWithoutDeath * PANACHE_SCORE, ScoreElementType.Panache));
            }

            // elites defeated without losing a character
            if (scoreData.eliteCombatsCompletedWithoutDeath > 0)
            {
                scoreElements.Add(new ScoreElementData(scoreData.eliteCombatsCompletedWithoutDeath * GIANT_SLAYER_SCORE, ScoreElementType.GiantSlayer));
            }

            // bosses defeated without losing a character
            if (scoreData.bossCombatsCompletedWithoutDeath > 0)
            {
                scoreElements.Add(new ScoreElementData(scoreData.bossCombatsCompletedWithoutDeath * GODLIKE_SCORE, ScoreElementType.Godlike));
            }

            // Penalties
            // Injuries
            if (scoreData.injuriesGained > 0)
            {
                scoreElements.Add(new ScoreElementData(-scoreData.injuriesGained * INJURIES_GAINED_SCORE, ScoreElementType.InjuriesGained));
            }

            // Characters KIA
            if (scoreData.playerCharactersKilled > 0)
            {
                scoreElements.Add(new ScoreElementData(-scoreData.playerCharactersKilled * PLAYER_CHARACTERS_KILLED_SCORE, ScoreElementType.PlayerCharactersKilled));
            }

            // Combats Lost
            if (scoreData.combatDefeats > 0)
            {
                scoreElements.Add(new ScoreElementData(-scoreData.combatDefeats * COMBAT_DEFEATS_SCORE, ScoreElementType.CombatDefeats));
            }

            // CALCULATE SCORING ELEMENTS THAT ONLY OCCUR AT RUN END!

            // Days passed
            ScoreElementData dp = CalculateDaysPassedScore();
            if (dp.totalScore > 0)
            {
                scoreElements.Add(dp);
            }

            // Father of the Year
            ScoreElementData foty = CalculateFatherOfTheYear();
            if (foty.totalScore > 0)
            {
                scoreElements.Add(foty);
            }

            // Well Armed
            ScoreElementData wellArmed = CalculateWellArmedScore();
            if (wellArmed.totalScore > 0)
            {
                scoreElements.Add(wellArmed);
            }

            // Well Armoured
            ScoreElementData wellArmoured = CalculateWellArmouredScore();
            if (wellArmoured.totalScore > 0)
            {
                scoreElements.Add(wellArmoured);
            }

            return scoreElements;
        }
        private int CalculateFinalScore(List<ScoreElementData> scoreElements)
        {
            int score = 0;
            foreach (ScoreElementData s in scoreElements)
            {
                score += s.totalScore;
            }

            return score;
        }

        #endregion

        #region Specific Score Calculators

        private ScoreElementData CalculateWellArmedScore()
        {
            // Gain +5 score for each tier 3 weapon collected
            int score = 0;

            foreach (InventoryItem i in InventoryController.Instance.PlayerInventory)
            {
                if (i.itemData != null &&
                    i.itemData.rarity == Rarity.Epic &&
                    i.itemData.itemType == ItemType.Weapon)
                {
                    score += 5;
                }
            }

            foreach (HexCharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
            {
                if (character.itemSet.mainHandItem != null && character.itemSet.mainHandItem.rarity == Rarity.Epic)
                {
                    score += WELL_ARMED_SCORE;
                }
                if (character.itemSet.offHandItem != null && character.itemSet.offHandItem.rarity == Rarity.Epic)
                {
                    score += WELL_ARMED_SCORE;
                }
            }

            return new ScoreElementData(score, ScoreElementType.WellArmed);
        }
        private ScoreElementData CalculateWellArmouredScore()
        {
            // Gain +5 score for head/body piece with 150 or more armour
            int score = 0;

            foreach (InventoryItem i in InventoryController.Instance.PlayerInventory)
            {
                if (i.itemData != null &&
                    i.itemData.armourAmount >= 150 &&
                    (i.itemData.itemType == ItemType.Head || i.itemData.itemType == ItemType.Body))
                {
                    score += WELL_ARMOURED_SCORE;
                }
            }

            foreach (HexCharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
            {
                if (character.itemSet.headArmour != null &&
                    character.itemSet.headArmour.armourAmount >= 150)
                {
                    score += WELL_ARMOURED_SCORE;
                }
                if (character.itemSet.bodyArmour != null &&
                    character.itemSet.bodyArmour.armourAmount >= 150)
                {
                    score += WELL_ARMOURED_SCORE;
                }
            }

            return new ScoreElementData(score, ScoreElementType.WellArmoured);
        }
        private ScoreElementData CalculateFatherOfTheYear()
        {
            // Gain +20 score if the kid is still alive.
            int score = 0;

            foreach (HexCharacterData character in CharacterDataController.Instance.AllPlayerCharacters)
            {
                if (character.background.backgroundType == CharacterBackground.TheKid &&
                    PerkController.Instance.GetAllPermanentInjuriesOnCharacter(character).Count == 0)
                {
                    score += FATHER_OF_THE_YEAR_SCORE;
                    break;
                }
            }

            return new ScoreElementData(score, ScoreElementType.FatherOfTheYear);
        }
        private ScoreElementData CalculateDaysPassedScore()
        {
            // Gain +5 score for each day passed
            int score = RunController.Instance.CurrentDay * DAYS_PASSED_SCORE;

            return new ScoreElementData(score, ScoreElementType.DaysPassed);
        }

        #endregion
    }
}
