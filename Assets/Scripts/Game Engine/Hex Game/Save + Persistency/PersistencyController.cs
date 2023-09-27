﻿using System.IO;
using Sirenix.Serialization;
using UnityEngine;
using WeAreGladiators.Boons;
using WeAreGladiators.Characters;
using WeAreGladiators.Items;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.MainMenu;
using WeAreGladiators.Player;
using WeAreGladiators.RewardSystems;
using WeAreGladiators.Scoring;
using WeAreGladiators.StoryEvents;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Persistency
{
    public class PersistencyController : Singleton<PersistencyController>
    {

        // Conditionals + Checks
        #region

        public bool DoesSaveFileExist()
        {
            if (File.Exists(GetSaveFileDirectory()))
            {
                Debug.Log("PersistencyManager.DoesSaveFileExist() confirmed save file exists, returning true");
                return true;
            }
            Debug.Log("PersistencyManager.DoesSaveFileExist() could not find the save file, returning false");
            return false;
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
            ScoreController.Instance.BuildMyDataFromSaveFile(newLoad);
        }

        #endregion
        // Properties + Getters
        #region

        private const string SAVE_DIRECTORY = "/SaveFile.json";
        private const string TEST_SAVE_DIRECTORY = "/TestSaveFile.json";
        private string GetSaveFileDirectory()
        {
            if (GlobalSettings.Instance == null)
            {
                return Application.persistentDataPath + SAVE_DIRECTORY;
            }
            if (GlobalSettings.Instance.GameMode == GameMode.Standard)
            {
                return Application.persistentDataPath + SAVE_DIRECTORY;
            }
            return Application.persistentDataPath + TEST_SAVE_DIRECTORY;
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
            if (startingCharacter == null)
            {
                // Get player custom made character data
                startingCharacter = MainMenuController.Instance.CharacterBuild;

                // Adjust perk tree to accomodate starting perk choice
                startingCharacter.PerkTree.HandleAdjustTreeIfStartingPerkChoiceAlreadyMade(startingCharacter);

                // Learn abilites from selected items for starting character
                startingCharacter.abilityBook.HandleLearnAbilitiesFromItemSet(startingCharacter.itemSet);
            }

            CharacterDataController.Instance.AddCharacterToRoster(startingCharacter);

            // Build character recruit deck
            CharacterDataController.Instance.AutoGenerateAndCacheNewCharacterDeck();

            // Setup town
            TownController.Instance.GenerateDailyRecruits(RandomGenerator.NumberBetween(5, 7));
            TownController.Instance.GenerateDailyCombatContracts();
            TownController.Instance.GenerateDailyAbilityTomes();
            TownController.Instance.GenerateDailyArmouryItems();

            // Player Data
            PlayerDataController.Instance.SetGameStartValues();

            // Inventory
            InventoryController.Instance.Inventory.Clear();

            // Score data
            ScoreController.Instance.GenerateGameStartValues();

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
            ScoreController.Instance.SaveMyDataToSaveFile(newSave);

            // START SAVE!        
            SaveGameToDisk(newSave);
        }

        #endregion

        // Save, Load and Delete From Disk 
        #region

        private void SaveGameToDisk(SaveGameData saveFile)
        {
            byte[] bytes = SerializationUtility.SerializeValue(saveFile, DataFormat.Binary);
            File.WriteAllBytes(GetSaveFileDirectory(), bytes);
        }
        private SaveGameData LoadGameFromDisk()
        {
            SaveGameData newLoad;
            byte[] bytes = File.ReadAllBytes(GetSaveFileDirectory());
            newLoad = SerializationUtility.DeserializeValue<SaveGameData>(bytes, DataFormat.Binary);
            return newLoad;
        }
        public void DeleteSaveFileOnDisk()
        {
            if (DoesSaveFileExist())
            {
                File.Delete(GetSaveFileDirectory());
            }
        }

        #endregion
    }
}
