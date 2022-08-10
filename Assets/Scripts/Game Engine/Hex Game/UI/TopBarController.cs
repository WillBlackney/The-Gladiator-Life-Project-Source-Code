using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using HexGameEngine.Utilities;

namespace HexGameEngine.UI
{
    public class TopBarController : Singleton<TopBarController>
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] private GameObject mainTopBarVisualParent;
        [SerializeField] private GameObject combatTopBarVisualParent;

        [Header("Text Components")]
        [SerializeField] private TextMeshProUGUI currentGoldText;
        [SerializeField] private TextMeshProUGUI currentChapterText;
        [SerializeField] private TextMeshProUGUI currentDaytext;
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
        public TextMeshProUGUI CurrentChapterText
        {
            get { return currentChapterText; }
        }
       
        #endregion

        // Core Functions
        #region
        public void ShowMainTopBar()
        {
            mainTopBarVisualParent.SetActive(true);
            HideCombatTopBar();
        }
        public void HideMainTopBar()
        {
            mainTopBarVisualParent.SetActive(false);
        }
        public void ShowCombatTopBar()
        {
            combatTopBarVisualParent.SetActive(true);
            HideMainTopBar();
        }
        public void HideCombatTopBar()
        {
            combatTopBarVisualParent.SetActive(false);
        }

        #endregion

        // Keypad Control Logic
        #region
        private void Update()
        {
            // Handle key board input
            if (mainTopBarVisualParent.activeSelf == true)
            {
               // if (Input.GetKeyDown(KeyCode.C)) CharacterRosterViewController.Instance.OnCharacterRosterButtonClicked();
                //else if (Input.GetKeyDown(KeyCode.M)) MapSystem.MapView.Instance.OnWorldMapButtonClicked();
              //  else if (Input.GetKeyDown(KeyCode.Escape)) MainMenuController.Instance.OnTopBarSettingsButtonClicked();
            }
        }
        #endregion

        // Update Texts
        #region
        
        public void UpdateGoldText(string value)
        {
            currentGoldText.text = value;
        }      
       
        #endregion
    }
}