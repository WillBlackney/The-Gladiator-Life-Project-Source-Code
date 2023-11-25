using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Abilities;
using WeAreGladiators.Characters;
using WeAreGladiators.Combat;
using WeAreGladiators.TurnLogic;
using WeAreGladiators.Utilities;
using WeAreGladiators.VisualEvents;

namespace WeAreGladiators.CombatLog
{
    public class CombatLogController : Singleton<CombatLogController>
    {
        #region Components + Variables

        [Header("Core Components")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private CombatLogEntryView logEntryViewPrefab;
        [SerializeField] private Transform logEntryViewParent;
        [SerializeField] private GameObject logContentParent;
        [SerializeField] private ScrollRect logScrollRect;
        [SerializeField] private Scrollbar logSlider;
        [SerializeField] private List<CombatLogEntryView> allEntryViews = new List<CombatLogEntryView>();

        [Space(10)]
        [Header("Maximize/Minimize Log Button")]
        [SerializeField] private Button minMaxButton;
        [SerializeField] private Image minMaxButtonImage;

        [Space(10)]
        [Header("Sprites")]
        [SerializeField] private Sprite turnCycleStart;
        [SerializeField] private Sprite abilityUsed;
        [SerializeField] private Sprite hit;
        [SerializeField] private Sprite miss;
        [SerializeField] private Sprite tookDamage;
        [SerializeField] private Sprite lostHealth;
        [SerializeField] private Sprite death;
        [SerializeField] private Sprite knockDown;
        [SerializeField] private Sprite characterStartTurn;
        [SerializeField] private Sprite characterEndTurn;
        [SerializeField] private Sprite characterDelayTurn;
        [SerializeField] private Sprite gainedInjury;
        [SerializeField] private Sprite stressGained;
        [SerializeField] private Sprite stressStateChanged;
        [SerializeField] private Sprite gainedPositivePassive;
        [SerializeField] private Sprite gainedNegativePassive;
        [SerializeField] private Sprite resistedNegativePassive;

        private List<CombatLogEntryData> allEntryData = new List<CombatLogEntryData>();
        private bool maximized;
        private int maxEntriesShown = 50;

        #endregion                     

        #region Core Logic

        private void Start()
        {
            minMaxButton.onClick.RemoveAllListeners();
            minMaxButton.onClick.AddListener(OnMinMaxButtonClicked);
        }
        public void ShowLog(bool maximize = false)
        {
            allEntryViews.ForEach(v => v.gameObject.SetActive(false));
            mainCanvas.enabled = true;
            if (maximize)
            {
                MaximizeLog();
            }
        }
        public void HideLog()
        {
            allEntryViews.ForEach(v => v.gameObject.SetActive(false));
            mainCanvas.enabled = false;
        }

        #endregion

        #region Draw Entries Logic

        private void RenderEntries()
        {
            if(!maximized || allEntryData.Count == 0)
            {
                return;
            }

            List<CombatLogEntryData> source = new List<CombatLogEntryData>();
            source.AddRange(allEntryData);
            VisualEventManager.CreateVisualEvent(() =>
            {
                int dataStartIndex = source.Count - 1 - maxEntriesShown;
                int overflow = 0;
                if(source.Count > maxEntriesShown)
                {
                    overflow = 1;
                }
                if (dataStartIndex < 0)
                {
                    dataStartIndex = 0;
                }

                allEntryViews.ForEach(v => { if (v != null) v.gameObject.SetActive(false); });
                for (int i = 0; i < source.Count && i < maxEntriesShown; i++)
                {
                    if (i >= allEntryViews.Count)
                    {
                        CreateNewEntryView();
                    }
                    allEntryViews[i].Build(source[i + dataStartIndex + overflow]);
                }

                DelayUtils.DoNextFrame(() =>
                {
                    TransformUtils.RebuildLayout(logEntryViewParent.GetComponent<RectTransform>());
                    logScrollRect.verticalNormalizedPosition = 0f;
                });
            });

        }
        private CombatLogEntryView CreateNewEntryView()
        {
            CombatLogEntryView newEntry = Instantiate(logEntryViewPrefab, logEntryViewParent).GetComponent<CombatLogEntryView>();
            allEntryViews.Add(newEntry);
            return newEntry;
        }

        #endregion

        #region Min / Max Logic

        private void OnMinMaxButtonClicked()
        {
            if (maximized)
            {
                MinimizeLog();
            }
            else
            {
                MaximizeLog();
            }
        }
        private void MaximizeLog()
        {
            maximized = true;
            logContentParent.SetActive(true);
            minMaxButtonImage.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            RenderEntries();
        }
        private void MinimizeLog()
        {
            maximized = false;
            minMaxButtonImage.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
            logContentParent.SetActive(false);
        }

        #endregion

        #region Modify Entries Logic

        public void ClearAllEntries()
        {
            allEntryData.Clear();
        }
        private void CreateNewEntry(Sprite icon, string description)
        {
            allEntryData.Add(new CombatLogEntryData(icon, description));
            RenderEntries();
        }
        public void CreateTurnCycleStartEntry()
        {
            int turnNumber = TurnController.Instance.CurrentTurn;
            CreateNewEntry(turnCycleStart, "Turn " + TextLogic.ReturnColoredText(turnNumber.ToString(), TextLogic.blueNumber) + " started!");
        }
        public void CreateCharacterTurnStartEntry(HexCharacterModel character)
        {
            string col = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy)
            {
                col = TextLogic.lightRed;
            }
            string nameText = TextLogic.ReturnColoredText(character.myName, col);
            CreateNewEntry(characterStartTurn, nameText + " started their turn.");
        }
        public void CreateCharacterTurnDelayEntry(HexCharacterModel character)
        {
            string col = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy)
            {
                col = TextLogic.lightRed;
            }
            string nameText = TextLogic.ReturnColoredText(character.myName, col);
            CreateNewEntry(characterDelayTurn, nameText + " delayed their turn.");
        }
        public void CreateCharacterTurnEndEntry(HexCharacterModel character)
        {
            string col = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy)
            {
                col = TextLogic.lightRed;
            }
            string nameText = TextLogic.ReturnColoredText(character.myName, col);
            CreateNewEntry(characterEndTurn, nameText + " ended their turn.");
        }
        public void CreateCharacterUsedAbilityEntry(HexCharacterModel character, AbilityData ability, HexCharacterModel target = null)
        {
            string casterCol = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy)
            {
                casterCol = TextLogic.lightRed;
            }
            string casterNameText = TextLogic.ReturnColoredText(character.myName, casterCol);

            string targetCol = TextLogic.lightGreen;
            string targetNameText = "";
            if (target != null)
            {
                if (target.allegiance == Allegiance.Enemy)
                {
                    targetCol = TextLogic.lightRed;
                }
                targetNameText = TextLogic.ReturnColoredText(target.myName, targetCol);
            }

            string abilityText = TextLogic.ReturnColoredText(ability.abilityName, TextLogic.neutralYellow);
            if (target == null)
            {
                CreateNewEntry(abilityUsed, casterNameText + " used ability " + abilityText + ".");
            }
            else
            {
                CreateNewEntry(abilityUsed, casterNameText + " used ability " + abilityText + " on " + targetNameText + ".");
            }
        }
        public void CreateCharacterHitResultEntry(HexCharacterModel character, HexCharacterModel target, HitRoll data, bool critical = false)
        {
            string casterCol = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy)
            {
                casterCol = TextLogic.lightRed;
            }
            string casterNameText = TextLogic.ReturnColoredText(character.myName, casterCol);

            string targetCol = TextLogic.lightGreen;
            if (target.allegiance == Allegiance.Enemy)
            {
                targetCol = TextLogic.lightRed;
            }
            string targetNameText = TextLogic.ReturnColoredText(target.myName, targetCol);
            string hitText = "hit";
            Sprite icon = hit;

            if (data.Result == HitRollResult.Miss)
            {
                icon = miss;
                hitText = "missed";
            }
            else if (critical)
            {
                hitText = "critically hit";
            }
            string rollText = TextLogic.ReturnColoredText(data.Roll.ToString(), TextLogic.blueNumber);
            string requiredText = TextLogic.ReturnColoredText(data.RequiredRoll.ToString(), TextLogic.blueNumber);
            CreateNewEntry(icon, casterNameText + " " + hitText + " " + targetNameText + " (rolled " + rollText + ", required " + requiredText + " or less).");
        }
        public void CreateCharacterDamageEntry(HexCharacterModel target, int healthDamage, int armourDamage)
        {
            string casterCol = TextLogic.lightGreen;
            if (target.allegiance == Allegiance.Enemy)
            {
                casterCol = TextLogic.lightRed;
            }
            string casterNameText = TextLogic.ReturnColoredText(target.myName, casterCol);
            string totalDamageText = TextLogic.ReturnColoredText((healthDamage + armourDamage).ToString(), TextLogic.blueNumber);
            string healthNameText = TextLogic.ReturnColoredText("Health", TextLogic.neutralYellow);
            string healthText = TextLogic.ReturnColoredText(healthDamage.ToString(), TextLogic.blueNumber);
            string armourNameText = TextLogic.ReturnColoredText("Armour", TextLogic.neutralYellow);
            string armourText = TextLogic.ReturnColoredText(armourDamage.ToString(), TextLogic.blueNumber);
            CreateNewEntry(tookDamage, casterNameText + " took " + totalDamageText + " damage (" + healthText + " to " + healthNameText + ", " + armourText + " to " + armourNameText + ").");
        }
        public void CreateCharacterDiedEntry(HexCharacterModel character, DeathRollResult result)
        {
            string col = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy)
            {
                col = TextLogic.lightRed;
            }
            string nameText = TextLogic.ReturnColoredText(character.myName, col);

            if (result != null)
            {
                string rollText = TextLogic.ReturnColoredText(result.roll.ToString(), TextLogic.blueNumber);
                string requiredText = TextLogic.ReturnColoredText(result.required.ToString(), TextLogic.blueNumber);
                CreateNewEntry(death, nameText + " was killed (rolled " + rollText + ", required " + requiredText + " or less to survive).");

            }
            else
            {
                CreateNewEntry(death, nameText + " was killed.");
            }
        }
        public void CreatePermanentInjuryEntry(HexCharacterModel character, DeathRollResult result, string injuryName)
        {
            string col = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy)
            {
                col = TextLogic.lightRed;
            }
            string nameText = TextLogic.ReturnColoredText(character.myName, col);
            string injuryText = TextLogic.ReturnColoredText(injuryName, TextLogic.neutralYellow);

            if (result != null)
            {
                string rollText = TextLogic.ReturnColoredText(result.roll.ToString(), TextLogic.blueNumber);
                string requiredText = TextLogic.ReturnColoredText(result.required.ToString(), TextLogic.blueNumber);
                CreateNewEntry(death, nameText + " survived a killing blow and suffered a permanent injury: " + injuryText + " (rolled " + rollText + ", required " + requiredText + " or less to survive).");

            }
            else
            {
                CreateNewEntry(death, nameText + " was killed.");
            }
        }
        public void CreateInjuryEntry(HexCharacterModel character, string injuryName, int roll, int required)
        {
            string col = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy)
            {
                col = TextLogic.lightRed;
            }
            string nameText = TextLogic.ReturnColoredText(character.myName, col);
            string injuryText = TextLogic.ReturnColoredText(injuryName, TextLogic.neutralYellow);
            string rollText = TextLogic.ReturnColoredText(roll.ToString(), TextLogic.blueNumber);
            string requiredText = TextLogic.ReturnColoredText(required.ToString(), TextLogic.blueNumber);
            CreateNewEntry(gainedInjury, nameText + " suffered an injury: " + injuryText + " (rolled " + rollText + ", required more than " + requiredText + " to resist).");

        }
        public void CreateStressStateChangedEntry(HexCharacterModel character, string stressStateName)
        {
            string col = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy)
            {
                col = TextLogic.lightRed;
            }
            string nameText = TextLogic.ReturnColoredText(character.myName, col);
            string stressText = TextLogic.ReturnColoredText(stressStateName, TextLogic.neutralYellow);
            CreateNewEntry(stressStateChanged, nameText + " is now " + stressStateName + ".");
        }
        public void CreateCharacterGainedPassive(HexCharacterModel target, HexCharacterModel applier, int stacks, string passiveName)
        {
            string stacksText = TextLogic.ReturnColoredText(stacks.ToString(), TextLogic.blueNumber);
            string tCol = TextLogic.lightGreen;
            if (target.allegiance == Allegiance.Enemy)
            {
                tCol = TextLogic.lightRed;
            }
            string tNameText = TextLogic.ReturnColoredText(target.myName, tCol);

            string applCol = TextLogic.lightGreen;
            string appNameText = "";
            if (applier != null)
            {
                if (applier.allegiance == Allegiance.Enemy)
                {
                    applCol = TextLogic.lightRed;
                }
                appNameText = TextLogic.ReturnColoredText(applier.myName, applCol);
            }

            string passiveNameText = TextLogic.ReturnColoredText(passiveName, TextLogic.neutralYellow);
            if (applier == null || applier == target)
            {
                CreateNewEntry(gainedPositivePassive, tNameText + " gained " + stacksText + " " + passiveNameText + ".");
            }
            else
            {
                CreateNewEntry(gainedPositivePassive, tNameText + " gained " + stacksText + " " + passiveNameText + " from " + appNameText + ".");
            }
        }

        #endregion
    }

    public enum EntryIcon
    {
        None = 0,
        TurnCycleStart = 1,
        AbilityUsed = 2,
        Hit = 3,
        Miss = 4,
        TookDamage = 5,
        LostHealth = 17,
        Death = 6,
        KnockDown = 7,
        CharacterStartTurn = 8,
        CharacterStartEnd = 9,
        CharacterStartDelay = 10,
        GainedInjury = 11,
        StressGained = 12,
        StressStateChanged = 13,
        GainedPositivePassive = 14,
        GainedNegativePassive = 15,
        ResistedNegativePassive = 16

    }
}
