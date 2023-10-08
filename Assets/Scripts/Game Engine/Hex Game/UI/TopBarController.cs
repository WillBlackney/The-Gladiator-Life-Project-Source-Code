using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using WeAreGladiators.Characters;
using WeAreGladiators.MainMenu;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class TopBarController : Singleton<TopBarController>
    {
        #region Properties + Components
        [Header("Core Components")]
        [SerializeField] private Canvas mainTopBarRootCanvas;
        [SerializeField] private Canvas combatTopBarRootCanvas;

        [Header("Text Components")]
        [SerializeField] private TextMeshProUGUI currentGoldText;
        [SerializeField] private TextMeshProUGUI currentDaytext;
        [SerializeField] private TextMeshProUGUI currentGoldInfoPopupText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        #endregion

        #region Getters + Accessors

        public TextMeshProUGUI CurrentGoldText
        {
            get { return currentGoldText; }
        }
        public TextMeshProUGUI CurrentDaytext => currentDaytext;

        #endregion

        #region Keypad Control Logic

        private void Update()
        {
            // Handle key board input
            if ((mainTopBarRootCanvas.isActiveAndEnabled || combatTopBarRootCanvas.isActiveAndEnabled) && Input.GetKeyDown(KeyCode.C))
            {
                CharacterRosterViewController.Instance?.OnCharacterRosterTopbarButtonClicked();
            }
            else if ((mainTopBarRootCanvas.isActiveAndEnabled || combatTopBarRootCanvas.isActiveAndEnabled) && Input.GetKeyDown(KeyCode.Escape))
            {
                MainMenuController.Instance.OnTopBarSettingsButtonClicked();
            }
        }

        #endregion

        #region Update Texts
        public void UpdateGoldText(string value)
        {
            currentGoldText.text = value;
        }

        #endregion        

        #region Core Functions
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
        public void OnGoldPanelMouseEnter()
        {
            string one = TextLogic.ReturnColoredText(CharacterDataController.Instance.GetTotalRosterDailyWage().ToString(), TextLogic.blueNumber);
            string two = TextLogic.ReturnColoredText("Gold", TextLogic.neutralYellow);
            currentGoldInfoPopupText.text = string.Format("Currently paying {0} {1} each day in wages.", one, two);
        }

        #endregion
    }
}
