using Sirenix.Serialization;
using System.IO;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Characters;
using HexGameEngine.JourneyLogic;
using HexGameEngine.RewardSystems;
using HexGameEngine.MainMenu;
using HexGameEngine.Player;
using HexGameEngine.TownFeatures;
using HexGameEngine.Items;
using HexGameEngine.StoryEvents;
using HexGameEngine.Boons;

namespace HexGameEngine.Persistency
{
    public class PersistencyController : Singleton<PersistencyController>
    {
        // Properties + Getters
        #region
        private const string SAVE_DIRECTORY = "/SaveFile.json";
        private const string TEST_SAVE_DIRECTORY = "/TestSaveFile.json";
        private string GetSaveFileDirectory()
        {
            if (GlobalSettings.Instance == null)
                return Application.persistentDataPath + SAVE_DIRECTORY;
            else if (GlobalSettings.Instance.GameMode == GameMode.Standard)
                return Application.persistentDataPath + SAVE_DIRECTORY;
            else return Application.persistentDataPath + TEST_SAVE_DIRECTORY;
        }
        #endregion

        // Conditionals + Checks
        #region
        public bool DoesSaveFileExist()
        {
            if (File.Exists(GetSaveFileDirectory()))
            {
                Debug.Log("PersistencyManager.DoesSaveFileExist() confirmed save file exists, returning true");
                return true;
            }
            else
            {
                Debug.Log("PersistencyManager.DoesSaveFileExist() could not find the save file, returning false");
                return false;
            }
        }
        #endregion

        // Build Save Files Data
        #region
        public void BuildNewSaveFileOnNewGameStarted(HexCharacterData startingCharacter = null)
        {
            // Run data
            RunController.Instance.SetGameStartValues();

            // Clear any previous character roster data + rebuild character deck
            CharacterDataController.Instance.ClearCharacterRoster();
            CharacterDataController.Instance.ClearCharacterDeck();

            // Determine charactes + add them to roster
            if(startingCharacter == null)
            {
                // Get player custom made character data
                startingCharacter = MainMenuController.Instance.CharacterBuild;

                // Learn abilites from selected items for starting character
                startingCharacter.abilityBook.HandleLearnAbilitiesFromItemSet(startingCharacter.itemSet);
            }               
            
            CharacterDataController.Instance.AddCharacterToRoster(startingCharacter);
           
            // Build character recruit deck
            CharacterDataController.Instance.AutoGenerateAndCacheNewCharacterDeck();

            // Setup town
            TownController.Instance.GenerateDailyRecruits(RandomGenerator.NumberBetween(4,6));
            TownController.Instance.GenerateDailyCombatContracts();
            TownController.Instance.GenerateDailyAbilityTomes();
            TownController.Instance.GenerateDailyArmouryItems();

            // Player Data
            PlayerDataController.Instance.SetGameStartValues();                       

            // Inventory
            InventoryController.Instance.Inventory.Clear();

            // START SAVE!        
            AutoUpdateSaveFile();
        }
        public void AutoUpdateSaveFile()
        {
            Debug.Log("PersistencyManager.AutoUpdateSaveFile() called...");

            // Setup empty save file
            SaveGameData newSave = new SaveGameData();

            RunController.Instance.SaveMyDataToSaveFile(newSave);
            CharacterDataController.Instance.SaveMyDataToSaveFile(newSave);
            TownController.Instance.SaveMyDataToSaveFile(newSave);
            PlayerDataController.Instance.SaveMyDataToSaveFile(newSave);
            CombatRewardController.Instance.SaveMyDataToSaveFile(newSave);
            InventoryController.Instance.SaveMyDataToSaveFile(newSave);
            StoryEventController.Instance.SaveMyDataToSaveFile(newSave);
            BoonController.Instance.SaveMyDataToSaveFile(newSave);

            // START SAVE!        
            SaveGameToDisk(newSave);
        }

        #endregion

        // Build Session Data
        #region
        public void SetUpGameSessionDataFromSaveFile()
        {
            // Build save file from persistency
            SaveGameData newLoad = LoadGameFromDisk();

            RunController.Instance.BuildMyDataFromSaveFile(newLoad);
            CharacterDataController.Instance.BuildMyDataFromSaveFile(newLoad);
            TownController.Instance.BuildMyDataFromSaveFile(newLoad);
            PlayerDataController.Instance.BuildMyDataFromSaveFile(newLoad);
            CombatRewardController.Instance.BuildMyDataFromSaveFile(newLoad);
            InventoryController.Instance.BuildMyDataFromSaveFile(newLoad);
            StoryEventController.Instance.BuildMyDataFromSaveFile(newLoad);
            BoonController.Instance.BuildMyDataFromSaveFile(newLoad);
        }
        #endregion

        // Save, Load and Delete From Disk 
        #region
        private void SaveGameToDisk(SaveGameData saveFile)
        {
            byte[] bytes = SerializationUtility.SerializeValue(saveFile, DataFormat.Binary);
            File.WriteAllBytes(GetSaveFileDirectory(), bytes);
        }
        public SaveGameData LoadGameFromDisk()
        {
            SaveGameData newLoad;
            byte[] bytes = File.ReadAllBytes(GetSaveFileDirectory());
            newLoad = SerializationUtility.DeserializeValue<SaveGameData>(bytes, DataFormat.Binary);
            return newLoad;
        }
        public void DeleteSaveFileOnDisk()
        {
            Debug.Log("PersistencyManager.DeleteSaveFileOnDisk() called");

            if (DoesSaveFileExist())
            {
                File.Delete(GetSaveFileDirectory());
            }
        }
        #endregion



    }
}