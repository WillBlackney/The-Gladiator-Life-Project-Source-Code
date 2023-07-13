using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using WeAreGladiators.Utilities;
using WeAreGladiators.Characters;
using WeAreGladiators.MainMenu;
using WeAreGladiators.Items;

namespace WeAreGladiators.UI
{
    public class TopBarController : Singleton<TopBarController>
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] private Canvas mainTopBarRootCanvas;
        [SerializeField] private Canvas combatTopBarRootCanvas;

        [Header("Text Components")]
        [SerializeField] private TextMeshProUGUI currentGoldText;
        [SerializeField] private TextMeshProUGUI currentDaytext;
        [SerializeField] private TextMeshProUGUI currentGoldInfoPopupText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        #endregion

        // Getters + Accessors
        #region
        public TextMeshProUGUI CurrentGoldText
        {
            get { return currentGoldText; }
        }     
        public TextMeshProUGUI CurrentDaytext
        {
            get { return currentDaytext; }
        }
       
        #endregion

        // Core Functions
        #region
        public void ShowMainTopBar()
        {
            mainTopBarRootCanvas.enabled = true;
            HideCombatTopBar();
        }
        public void HideMainTopBar()
        {
            mainTopBarRootCanvas.enabled = false;
        }
        public void ShowCombatTopBar()
        {
            combatTopBarRootCanvas.enabled = true;
            HideMainTopBar();
        }
        public void HideCombatTopBar()
        {
            combatTopBarRootCanvas.enabled = false;
        }

        #endregion

        // Keypad Control Logic
        #region
        private void Update()
        {
            // Handle key board input
            if ((mainTopBarRootCanvas.isActiveAndEnabled == true || combatTopBarRootCanvas.isActiveAndEnabled == true) && Input.GetKeyDown(KeyCode.C)) CharacterRosterViewController.Instance?.OnCharacterRosterTopbarButtonClicked();
            else if (mainTopBarRootCanvas.isActiveAndEnabled == true && Input.GetKeyDown(KeyCode.I)) InventoryController.Instance?.OnInventoryTopBarButtonClicked();
            else if ((mainTopBarRootCanvas.isActiveAndEnabled == true || combatTopBarRootCanvas.isActiveAndEnabled == true) && Input.GetKeyDown(KeyCode.Escape)) MainMenuController.Instance.OnTopBarSettingsButtonClicked();

        }
        #endregion

        // Update Texts
        #region

        public void UpdateGoldText(string value)
        {
            currentGoldText.text = value;
        }      
       
        #endregion

        public void OnGoldPanelMouseEnter()
        {
            string one = TextLogic.ReturnColoredText(CharacterDataController.Instance.GetTotalRosterDailyWage().ToString(), TextLogic.blueNumber);
            string two = TextLogic.ReturnColoredText("Gold", TextLogic.neutralYellow);
            currentGoldInfoPopupText.text = System.String.Format("Currently paying {0} {1} each day in wages.", one, two);
        }
    }
}