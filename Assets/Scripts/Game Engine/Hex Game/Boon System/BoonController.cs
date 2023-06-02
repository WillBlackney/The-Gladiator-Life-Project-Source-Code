using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Persistency;

namespace HexGameEngine.Boons
{
    public class BoonController : Singleton<BoonController>
    {
        #region Components
        [Header("Boon Library Properties")]
        [SerializeField] private BoonDataSO[] allBoonData;

        [Space(10)]

        [Header("UI Components")]
        [SerializeField] private Canvas boonIconsPanelCanvas;       
        [SerializeField] private Transform boonIconsParent;
        [SerializeField] private GameObject uiBoonIconPrefab;
        [SerializeField] private List<UIBoonIcon> boonIcons = new List<UIBoonIcon>();

        [Space(10)]

        private List<BoonData> activePlayerBoons = new List<BoonData>();
        #endregion


        #region Persitency Logic
        public void BuildMyDataFromSaveFile(SaveGameData save)
        {
            activePlayerBoons = save.activePlayerBoons;
        }
        public void SaveMyDataToSaveFile(SaveGameData save)
        {
            save.activePlayerBoons = activePlayerBoons;
        }
        #endregion

        #region Library Logic
        public BoonDataSO GetBoonDataByTag(BoonTag tag)
        {
            BoonDataSO ret = null;

            foreach (BoonDataSO b in allBoonData)
            {
                if (b.boonTag == tag)
                {
                    ret = b;
                    break;
                }
            }

            if (ret == null)
                Debug.LogWarning("BoonController.GetBoonDataByTag() did not find a state matching the search term: " +
                    name + ", returning null...");

            return ret;
        }
        #endregion

        #region UI + Visual Logic
        public void BuildAndShowBoonIconsPanel()
        {
            Debug.Log("BoonController.BuildAndShowBoonIconsPanel() called");
            if(activePlayerBoons.Count == 0)
            {
                HideBoonIconsPanel();
                return;
            }

            boonIconsPanelCanvas.enabled = true;

            // Reset icons
            boonIcons.ForEach(i => i.HideAndReset());

            // Build an icon for each active boon the player has
            for (int i = 0; i < activePlayerBoons.Count; i++)
            {
                // Create a new UI icon if not enough to show every player boon
                if (i >= boonIcons.Count)
                {
                    UIBoonIcon newIcon = Instantiate(uiBoonIconPrefab, boonIconsParent).GetComponent<UIBoonIcon>();
                    boonIcons.Add(newIcon);
                }

                // Build and show icon
                boonIcons[i].BuildAndShowFromBoonData(activePlayerBoons[i]);

            }
        }
        public void HideBoonIconsPanel()
        {
            boonIconsPanelCanvas.enabled = false;
        }
        #endregion

        #region Modify Player Boons
        public void HandleGainBoon(BoonData newBoon)
        {
            Debug.Log("BoonController.HandleGainBoon() called, gaining boon: " + newBoon.boonDisplayName);
            activePlayerBoons.Add(newBoon);
            newBoon.currentTimerStacks = RandomGenerator.NumberBetween(newBoon.minDuration, newBoon.maxDuration);
            BuildAndShowBoonIconsPanel();
        }

        #endregion

        #region Conditional Checks + Bools
        public bool DoesPlayerHaveBoon(BoonTag tag)
        {
            bool ret = false;

            foreach(BoonData b in activePlayerBoons)
            {
                if(b.boonTag == tag)
                {
                    ret = true;
                    break;
                }
            }

            return ret;

        }

        #endregion
    }
}