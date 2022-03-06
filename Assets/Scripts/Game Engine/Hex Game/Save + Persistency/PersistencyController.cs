using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Characters;
using HexGameEngine.JourneyLogic;
using HexGameEngine.RewardSystems;
using HexGameEngine.MainMenu;
using HexGameEngine.Player;
using HexGameEngine.TownFeatures;
using HexGameEngine.DungeonMap;
using HexGameEngine.Items;

namespace HexGameEngine.Persistency
{
    public class PersistencyController : Singleton<PersistencyController>
    {

        // Properties + Getters
        #region
        public const string SAVE_DIRECTORY = "/SaveFile.json";
        public string GetSaveFileDirectory()
        {
            return Application.persistentDataPath + SAVE_DIRECTORY;
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
        public void BuildNewSaveFileOnNewGameStarted()
        {
            // Clear any previous character roster data + rebuild character deck
            CharacterDataController.Instance.ClearCharacterRoster();
            CharacterDataController.Instance.ClearCharacterDeck();

            // Determine charactes + add them to roster
            HexCharacterData chosenCharacter = MainMenuController.Instance.GetChosenCharacterDataFiles()[0];
            CharacterDataController.Instance.AddCharacterToRoster(chosenCharacter);
           
            // Build character recruit deck
            CharacterDataController.Instance.AutoGenerateAndCacheNewCharacterDeck();

            // Generate map data
            MapManager.Instance.SetCurrentMap(MapManager.Instance.GenerateNewMap());           
            MapPlayerTracker.Instance.LockMap();

            // Player Data
            PlayerDataController.Instance.SetGameStartValues();

            // Run data
            RunController.Instance.SetGameStartValues();

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
            OLDTownController.Instance.SaveMyDataToSaveFile(newSave);
            PlayerDataController.Instance.SaveMyDataToSaveFile(newSave);
            RewardController.Instance.SaveMyDataToSaveFile(newSave);
            MapManager.Instance.SaveMyDataToSaveFile(newSave);
            InventoryController.Instance.SaveMyDataToSaveFile(newSave);

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
            OLDTownController.Instance.BuildMyDataFromSaveFile(newLoad);
            PlayerDataController.Instance.BuildMyDataFromSaveFile(newLoad);
            RewardController.Instance.BuildMyDataFromSaveFile(newLoad);
            MapManager.Instance.BuildMyDataFromSaveFile(newLoad);
            InventoryController.Instance.BuildMyDataFromSaveFile(newLoad);
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