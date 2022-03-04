using HexGameEngine.UI;
using HexGameEngine.VisualEvents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Persistency;

namespace HexGameEngine.Player
{
    public class PlayerDataController : Singleton<PlayerDataController>
    {
        // Properties + Components
        #region
        private int currentGold;
        private int currentFood;
        #endregion

        // Getters + Accessors
        #region
        public int CurrentGold
        {
            get { return currentGold; }
        }
        public int CurrentFood
        {
            get { return currentFood; }
        }
        #endregion

        // Initialization
        #region       
        public void SetGameStartValues()
        {
            ModifyPlayerGold(GlobalSettings.Instance.BaseStartingGold);
        }
        #endregion

        // Persistency
        #region
        public void BuildMyDataFromSaveFile(SaveGameData saveFile)
        {
            ModifyPlayerGold(-currentGold + saveFile.currentGold);
        }
        public void SaveMyDataToSaveFile(SaveGameData saveData)
        {
            saveData.currentGold = currentGold;
        }
        #endregion

        // Food Logic
        #region
        public void ModifyPlayerFood(int foodGainedOrLost)
        {
            currentFood += foodGainedOrLost;
            int vEventValue = currentFood;

            // update food text;
            VisualEventManager.Instance.CreateVisualEvent(() => TopBarController.Instance.UpdateFoodText(vEventValue.ToString()));
        }
        public bool HasEnoughFood(int required)
        {
            return currentFood >= required;
        }
        #endregion

        // Gold Logic
        #region
        public void ModifyPlayerGold(int goldGainedOrLost)
        {
            currentGold += goldGainedOrLost;
            int vEventValue = currentGold;

            // update food text;
            VisualEventManager.Instance.CreateVisualEvent(() => TopBarController.Instance.UpdateGoldText(vEventValue.ToString()));
        }
        #endregion



    }
}