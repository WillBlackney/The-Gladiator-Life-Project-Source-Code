using WeAreGladiators.Persistency;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;
using WeAreGladiators.VisualEvents;

namespace WeAreGladiators.Player
{
    public class PlayerDataController : Singleton<PlayerDataController>
    {

        // Initialization
        #region

        public void SetGameStartValues()
        {
            ModifyPlayerGold(GlobalSettings.Instance.BaseStartingGold);
            //DeploymentLimit = GlobalSettings.Instance.StartingDeploymentLimit;
        }

        #endregion
        // Properties + Components
        #region

        #endregion

        // Getters + Accessors
        #region

        public int CurrentGold { get; private set; }
        public int CurrentFood { get; }

        #endregion

        // Persistency
        #region

        public void BuildMyDataFromSaveFile(SaveGameData saveFile)
        {
            SetPlayerGold(saveFile.currentGold);
        }
        public void SaveMyDataToSaveFile(SaveGameData saveData)
        {
            saveData.currentGold = CurrentGold;
        }

        #endregion

        // Gold Logic
        #region

        public void ModifyPlayerGold(int goldGainedOrLost)
        {
            CurrentGold += goldGainedOrLost;
            int vEventValue = CurrentGold;

            // update food text;
            VisualEventManager.CreateVisualEvent(() => TopBarController.Instance.UpdateGoldText(vEventValue.ToString()));
        }
        private void SetPlayerGold(int newValue)
        {
            CurrentGold = newValue;
            int vEventValue = CurrentGold;

            // update food text;
            VisualEventManager.CreateVisualEvent(() => TopBarController.Instance.UpdateGoldText(vEventValue.ToString()));
        }

        #endregion
    }
}
