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
        [SerializeField] private GameObject visualParent;

        [Header("Text Components")]
        [SerializeField] private TextMeshProUGUI currentGoldText;
        [SerializeField] private TextMeshProUGUI currentChapterText;
        [SerializeField] private TextMeshProUGUI currentDaytext;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Main Button Components")]
        [SerializeField] private GameObject characterRosterButton;
        [SerializeField] private Image characterRosterButtonGlow;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Misc Components")]
        [SerializeField] private GameObject goldTopBarImage;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        private bool charRosterGlowIsPlaying = false;
        #endregion

        // Getters + Accessors
        #region
        public TextMeshProUGUI CurrentGoldText
        {
            get { return currentGoldText; }
        }       
        public GameObject GoldTopBarImage
        {
            get { return goldTopBarImage; }
        }
        public TextMeshProUGUI CurrentDaytext
        {
            get { return currentDaytext; }
        }
        public TextMeshProUGUI CurrentChapterText
        {
            get { return currentChapterText; }
        }
        public GameObject CharacterRosterButton
        {
            get { return characterRosterButton; }
        }
        #endregion

        // Core Functions
        #region
        public void ShowTopBar()
        {
            visualParent.SetActive(true);
        }
        public void HideTopBar()
        {
            visualParent.SetActive(false);
        }
        public void ShowCharacterRosterButtonGlow()
        {
            if (charRosterGlowIsPlaying == false)
            {
                charRosterGlowIsPlaying = true;
                characterRosterButtonGlow.DOKill();
                characterRosterButtonGlow.DOFade(0.33f, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }

        }
        public void HideCharacterRosterButtonGlow()
        {
            charRosterGlowIsPlaying = false;
            characterRosterButtonGlow.DOKill();
            characterRosterButtonGlow.DOFade(0, 0);
        }
        #endregion

        // Keypad Control Logic
        #region
        private void Update()
        {
            // Handle key board input
            if (visualParent.activeSelf == true)
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
        public void UpdateDayText(int day)
        {
            currentChapterText.text = "Day " + day.ToString();
        }
        public void UpdateHourText(int hour)
        {
            if(hour > 12)
            {
                currentDaytext.text = (hour - 12).ToString() + " PM";
            }
            else
            {
                currentDaytext.text = hour.ToString() + " AM";
            }
        }
        #endregion
    }
}