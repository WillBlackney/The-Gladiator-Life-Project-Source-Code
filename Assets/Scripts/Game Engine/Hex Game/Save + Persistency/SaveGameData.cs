using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HexGameEngine.Characters;
using HexGameEngine.RewardSystems;
using HexGameEngine.DungeonMap;
using HexGameEngine.Items;
using HexGameEngine.TownFeatures;
using HexGameEngine.HexTiles;

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
        public float runTimer;

        public CombatContractData currentCombatContractData;
        public SerializedCombatMapData currentCombatMapData;
        public List<CharacterWithSpawnData> playerCombatCharacters = new List<CharacterWithSpawnData>();       
        public List<EnemyEncounterData> encounteredCombats = new List<EnemyEncounterData>();
        public string currentStoryEvent;
        public List<string> encounteredStoryEvents = new List<string>();
        public List<CharacterCombatStatData> currentCombatStatResult = new List<CharacterCombatStatData>();

        // Items
        public List<InventoryItem> inventory = new List<InventoryItem>();

        // Player data
        public int currentGold;

        // Story events       

        // Town data
        public List<HexCharacterData> characterDeck = new List<HexCharacterData>();
        public List<HexCharacterData> townRecruits = new List<HexCharacterData>();
        public List<CombatContractData> currentDailyCombatContracts = new List<CombatContractData>();
        public List<AbilityTomeShopData> currentLibraryTomes = new List<AbilityTomeShopData>();
        public List<ItemShopData> currentItems = new List<ItemShopData>();

        // Loot data
        public RewardContainerSet currentLootResult;

        // Scoring
       // public PlayerScoreTracker scoreData;

    }
}