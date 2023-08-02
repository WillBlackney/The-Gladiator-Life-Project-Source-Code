using System.Collections;
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
        [SerializeField] Canvas mainCanvas;
        [SerializeField] CombatLogEntryView logEntryViewPrefab;
        [SerializeField] Transform logEntryViewParent;
        [SerializeField] GameObject logContentParent;
        [SerializeField] Scrollbar logSlider;
        [SerializeField] List<CombatLogEntryView> allEntryViews = new List<CombatLogEntryView>();

        [Space(10)]

        [Header("Maximize/Minimize Log Button")]
        [SerializeField] Button minMaxButton;
        [SerializeField] Image minMaxButtonImage;

        [Space(10)]

        [Header("Sprites")]
        [SerializeField] Sprite turnCycleStart;
        [SerializeField] Sprite abilityUsed;
        [SerializeField] Sprite hit;
        [SerializeField] Sprite miss;
        [SerializeField] Sprite tookDamage;
        [SerializeField] Sprite lostHealth;
        [SerializeField] Sprite death;
        [SerializeField] Sprite knockDown;
        [SerializeField] Sprite characterStartTurn;
        [SerializeField] Sprite characterEndTurn;
        [SerializeField] Sprite characterDelayTurn;
        [SerializeField] Sprite gainedInjury;
        [SerializeField] Sprite stressGained;
        [SerializeField] Sprite stressStateChanged;
        [SerializeField] Sprite gainedPositivePassive;
        [SerializeField] Sprite gainedNegativePassive;
        [SerializeField] Sprite resistedNegativePassive;

        List<CombatLogEntryData> allEntryData = new List<CombatLogEntryData>();
        bool maximized = false;
        int maxEntriesShown = 50;
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
            if (maximize) MaximizeLog();
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
            List<CombatLogEntryData> source = new List<CombatLogEntryData>();
            source.AddRange(allEntryData);
            VisualEventManager.CreateVisualEvent(() =>
            {
                int dataStartIndex = source.Count - 1 - maxEntriesShown;
                if(dataStartIndex < 0) dataStartIndex = 0;

                allEntryViews.ForEach(v => v.gameObject.SetActive(false));
                for (int i = 0; i < source.Count && i < maxEntriesShown; i++)
                {
                    if (i >= allEntryViews.Count) CreateNewEntryView();
                    allEntryViews[i].Build(source[i + dataStartIndex]);
                }
                logSlider.value = 0f;
                TransformUtils.RebuildLayout(logEntryViewParent.GetComponent<RectTransform>());
                logSlider.value = 0f;
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
            if (maximized) MinimizeLog();
            else MaximizeLog();
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
            if (character.allegiance == Allegiance.Enemy) col = TextLogic.lightRed;
            string nameText = TextLogic.ReturnColoredText(character.myName, col);
            CreateNewEntry(characterStartTurn, nameText + " started their turn.");
        }
        public void CreateCharacterTurnDelayEntry(HexCharacterModel character)
        {
            string col = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy) col = TextLogic.lightRed;
            string nameText = TextLogic.ReturnColoredText(character.myName, col);
            CreateNewEntry(characterDelayTurn, nameText + " delayed their turn.");
        }
        public void CreateCharacterTurnEndEntry(HexCharacterModel character)
        {
            string col = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy) col = TextLogic.lightRed;
            string nameText = TextLogic.ReturnColoredText(character.myName, col);
            CreateNewEntry(characterEndTurn, nameText + " ended their turn.");
        }
        public void CreateCharacterUsedAbilityEntry(HexCharacterModel character, AbilityData ability, HexCharacterModel target = null)
        {
            string casterCol = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy) casterCol = TextLogic.lightRed;
            string casterNameText = TextLogic.ReturnColoredText(character.myName, casterCol);

            string targetCol = TextLogic.lightGreen;
            string targetNameText = "";
            if (target != null)
            {
                if (target.allegiance == Allegiance.Enemy) targetCol = TextLogic.lightRed;
                targetNameText = TextLogic.ReturnColoredText(target.myName, targetCol);
            }

            string abilityText = TextLogic.ReturnColoredText(ability.abilityName, TextLogic.neutralYellow);
            if(target == null) CreateNewEntry(abilityUsed, casterNameText + " used ability " + abilityText + ".");
            else CreateNewEntry(abilityUsed, casterNameText + " used ability " + abilityText + " on " + targetNameText + ".");
        }
        public void CreateCharacterHitResultEntry(HexCharacterModel character, HexCharacterModel target, HitRoll data, bool critical = false)
        {
            string casterCol = TextLogic.lightGreen;
            if (character.allegiance == Allegiance.Enemy) casterCol = TextLogic.lightRed;
            string casterNameText = TextLogic.ReturnColoredText(character.myName, casterCol);

            string targetCol = TextLogic.lightGreen;
            if (target.allegiance == Allegiance.Enemy) targetCol = TextLogic.lightRed;
            string targetNameText = TextLogic.ReturnColoredText(target.myName, targetCol);
            string hitText = "hit";
            Sprite icon = hit;

            if (data.Result == HitRollResult.Miss)
            {
                icon = miss;
                hitText = "missed";
            }
            else if (critical) hitText = "critically hit";
            string rollText = TextLogic.ReturnColoredText(data.Roll.ToString(), TextLogic.blueNumber);
            string requiredText = TextLogic.ReturnColoredText(data.RequiredRoll.ToString(), TextLogic.blueNumber);
            CreateNewEntry(icon, casterNameText + " " + hitText + " " + targetNameText + " (rolled " + rollText + ", required " + requiredText + " or less).");
        }
        public void CreateCharacterDamageEntry(HexCharacterModel target, int healthDamage, int armourDamage)
        {
            string casterCol = TextLogic.lightGreen;
            if (target.allegiance == Allegiance.Enemy) casterCol = TextLogic.lightRed;
            string casterNameText = TextLogic.ReturnColoredText(target.myName, casterCol);
            string totalDamageText = TextLogic.ReturnColoredText((healthDamage + armourDamage).ToString(), TextLogic.blueNumber);
            string healthNameText = TextLogic.ReturnColoredText("Health", TextLogic.neutralYellow);
            string healthText = TextLogic.ReturnColoredText(healthDamage.ToString(), TextLogic.blueNumber);
            string armourNameText = TextLogic.ReturnColoredText("Armour", TextLogic.neutralYellow);
            string armourText = TextLogic.ReturnColoredText(armourDamage.ToString(), TextLogic.blueNumber);
            CreateNewEntry(tookDamage, casterNameText + " took " + totalDamageText + " damage (" + healthText + " to " + healthNameText + ", " + armourText + " to " + armourNameText + ").");
        }
        #endregion


        #region Misc Logic
        private Sprite GetIconSprite(EntryIcon type) 
        { 
            Sprite sprite = null;

            return sprite;
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
        ResistedNegativePassive = 16,

    }
}