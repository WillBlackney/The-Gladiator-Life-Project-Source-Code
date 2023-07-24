using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WeAreGladiators.Characters;
using WeAreGladiators.RewardSystems;
using WeAreGladiators.DungeonMap;
using WeAreGladiators.Items;
using WeAreGladiators.TownFeatures;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Boons;
using WeAreGladiators.Scoring;

namespace WeAreGladiators.Persistency
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
        public List<BoonData> activePlayerBoons = new List<BoonData>();
        public PlayerScoreTracker scoreData;

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