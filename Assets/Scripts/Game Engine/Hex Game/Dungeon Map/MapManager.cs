﻿using UnityEngine;
using WeAreGladiators.Persistency;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.DungeonMap
{
    public class MapManager : Singleton<MapManager>
    {

        // Getters + Accessors
        #region

        public Map CurrentMap { get; private set; }

        #endregion
        // Components + Properties
        #region

        [Header("Components")]
        public DungeonMapSeed config;
        public MapView view;

        [Header("Properties")]
        [SerializeField] private bool testingMode;

        #endregion

        // Generate + Set Map
        #region

        private void Start()
        {
            if (testingMode)
            {
                SetCurrentMap(GenerateNewMap());
                MapView.Instance.ShowMainMapView();
            }
        }
        public Map GenerateNewMap()
        {
            Map map = MapGenerator.Instance.GetMap(config);
            LogMapReport(map);
            return map;
        }
        public void SetCurrentMap(Map map)
        {
            CurrentMap = map;
        }
        public void LogMapReport(Map map)
        {
            Debug.Log(":::LOG MAP REPORT:::");

            int totalCamps = 0;
            int totalBasics = 0;
            int totalElites = 0;
            int totalShops = 0;
            int totalBosses = 0;

            for (int i = 0; i < map.nodes.Count; i++)
            {
                if (map.nodes[i].NodeType == EncounterType.BasicEnemy)
                {
                    totalBasics++;
                }
                else if (map.nodes[i].NodeType == EncounterType.CampSite)
                {
                    totalCamps++;
                }
                /*
                else if (map.nodes[i].NodeType == EncounterType.Shop)
                {
                    totalShops++;
                }
                */
                else if (map.nodes[i].NodeType == EncounterType.EliteEnemy)
                {
                    totalElites++;
                }
                else if (map.nodes[i].NodeType == EncounterType.BossEnemy)
                {
                    totalBosses++;
                }
            }

            Debug.Log("Total Basic Enemies: " + totalBasics);
            Debug.Log("Total Elite Enemies: " + totalElites);
            Debug.Log("Total Shops: " + totalShops);
            Debug.Log("Total Camp Sites: " + totalCamps);
            Debug.Log("Total Boss: " + totalBosses);

        }

        #endregion

        // Save + Load Logic
        #region

        public void SaveMyDataToSaveFile(SaveGameData saveFile)
        {
            if (CurrentMap == null)
            {
                Debug.LogWarning("MapManager.SaveMyDataToSaveFile() failed, current map is null...");
            }
            //saveFile.map = CurrentMap.ToJson();
        }
        public void BuildMyDataFromSaveFile(SaveGameData saveFile)
        {
            //CurrentMap = JsonConvert.DeserializeObject<Map>(saveFile.map);
        }

        #endregion
    }

}
