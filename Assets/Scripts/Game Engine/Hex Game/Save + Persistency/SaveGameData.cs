using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HexGameEngine.Characters;
using HexGameEngine.RewardSystems;
using HexGameEngine.DungeonMap;
using HexGameEngine.Items;
using HexGameEngine.TownFeatures;

namespace HexGameEngine.Persistency
{
    [Serializable]
    public class SaveGameData
    {
        // Character data
        public List<HexCharacterData> characterRoster = new List<HexCharacterData>();

        // Journey + Run data
        public int currentDay;
        public int currentChapter;
        public SaveCheckPoint saveCheckPoint;
        public EncounterType currentEncounterType;
        public float runTimer;

        public CombatContractData currentCombatContractData;
        public List<CharacterWithSpawnData> playerCombatCharacters = new List<CharacterWithSpawnData>();
        public string currentStoryEvent;
        public List<EnemyEncounterData> encounteredCombats = new List<EnemyEncounterData>();
        public List<string> encounteredStoryEvents = new List<string>();
        public List<CharacterCombatStatData> currentCombatStatResult = new List<CharacterCombatStatData>();

        // Items
        public List<InventoryItem> inventory = new List<InventoryItem>();

        // Map
        public string map;

        // Player data
        public int currentGold;
        public int deploymentLimit;

        // Story events
       

        // Town data
        public List<HexCharacterData> characterDeck = new List<HexCharacterData>();
        public List<HexCharacterData> townRecruits = new List<HexCharacterData>();
        public List<CombatContractData> currentDailyCombatContracts = new List<CombatContractData>();
        public List<AbilityTomeShopData> currentLibraryTomes = new List<AbilityTomeShopData>();
        public List<ItemShopData> currentItems = new List<ItemShopData>();

        // Loot data
        public RewardContainerSet currentLootResult;

        // Camp site data
        //public int campPointRegen;
        //public int campCardDraw;
       // public List<CampCardData> campDeck = new List<CampCardData>();

        // KBC Data
        //public List<KingsChoicePairingModel> kbcChoices = new List<KingsChoicePairingModel>();

        // Shop
        //public ShopContentResultModel shopData;

        // Scoring
       // public PlayerScoreTracker scoreData;

    }
}