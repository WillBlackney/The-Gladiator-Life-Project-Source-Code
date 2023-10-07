using System;
using System.Collections.Generic;
using WeAreGladiators.Boons;
using WeAreGladiators.Characters;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Items;
using WeAreGladiators.RewardSystems;
using WeAreGladiators.Scoring;
using WeAreGladiators.TownFeatures;

namespace WeAreGladiators.Persistency
{
    [Serializable]
    public class SaveGameData
    {

        // Journey + Run data
        public int currentDay;
        public int currentChapter;
        public SaveCheckPoint saveCheckPoint;
        public float runTimer;
        public string currentStoryEvent;
        public List<string> encounteredStoryEvents = new List<string>();

        // Player data
        public int currentGold;
        public List<BoonData> activePlayerBoons = new List<BoonData>();

        // Story events       

        // Town data
        public List<HexCharacterData> characterDeck = new List<HexCharacterData>();
        // Character data
        public List<HexCharacterData> characterRoster = new List<HexCharacterData>();

        public CombatContractData currentCombatContractData;
        public SerializedCombatMapData currentCombatMapData;
        public List<CharacterCombatStatData> currentCombatStatResult = new List<CharacterCombatStatData>();
        public List<ItemData> currentBonusLoot = new List<ItemData>();
        public List<CombatContractData> currentDailyCombatContracts = new List<CombatContractData>();
        public List<ItemShopData> currentItems = new List<ItemShopData>();
        public List<ItemShopData> currentLibraryTomes = new List<ItemShopData>();

        // Loot data
        public RewardContainerSet currentLootResult;
        public List<EnemyEncounterData> encounteredCombats = new List<EnemyEncounterData>();

        // Items
        public List<InventoryItem> inventory = new List<InventoryItem>();
        public List<CharacterWithSpawnData> playerCombatCharacters = new List<CharacterWithSpawnData>();
        public PlayerScoreTracker scoreData;
        public List<HexCharacterData> townRecruits = new List<HexCharacterData>();

        // Scoring
        // public PlayerScoreTracker scoreData;
    }
}
