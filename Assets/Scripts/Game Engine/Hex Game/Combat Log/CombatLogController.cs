using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] Slider logSlider;
        [SerializeField] List<CombatLogEntryView> allEntryViews = new List<CombatLogEntryView>();

        [Space(10)]

        [Header("Maximize/Minimize Log Button")]
        [SerializeField] Button minMaxButton;
        [SerializeField] GameObject maximizedIcon;
        [SerializeField] GameObject minimizedIcon;

        [Space(10)]

        [Header("Sprites")]
        [SerializeField] Sprite turnCycleStart;
        [SerializeField] Sprite abilityUsed;
        [SerializeField] Sprite hit;
        [SerializeField] Sprite miss;
        [SerializeField] Sprite damage;
        [SerializeField] Sprite death;
        [SerializeField] Sprite knockDown;
        [SerializeField] Sprite characterStartTurn;
        [SerializeField] Sprite characterStartEnd;
        [SerializeField] Sprite characterStartDelay;
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
        public void ShowLog(bool maximize = true)
        {
            mainCanvas.enabled = true;
            if (maximize) MaximizeLog();
        }
        public void HideLog()
        {

        }

        #endregion

        #region Draw Entries Logic
        private void RenderEntries()
        {
            List<CombatLogEntryData> source = new List<CombatLogEntryData>();
            source.AddRange(allEntryData);
            VisualEventManager.CreateVisualEvent(() =>
            {
                allEntryViews.ForEach(v => v.gameObject.SetActive(false));
                for (int i = 0; i < source.Count && i < maxEntriesShown; i++)
                {
                    if (i >= allEntryViews.Count) CreateNewEntryView();

                }
                TransformUtils.RebuildLayout(logEntryViewParent.GetComponent<RectTransform>());
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
            RenderEntries();
        }
        private void MinimizeLog()
        {
            maximized = false;
            logContentParent.SetActive(false);
        }
        #endregion

        #region Modify Entries Logic
        public void ClearAllEntries()
        {
            allEntryData.Clear();
        }
        public void CreateNewEntry(EntryIcon icon, string description)
        {
            allEntryData.Add(new CombatLogEntryData(GetIconSprite(icon), description));
            RenderEntries();            
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
        Star = 1,
        Skull = 2,

    }
}