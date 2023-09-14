using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeAreGladiators.Characters;
using WeAreGladiators.HexTiles;

namespace WeAreGladiators.Pathfinding
{
    public class Path
    {

        public Path(LevelNode start, List<LevelNode> hexsOnPath, HexCharacterModel character)
        {
            Start = start;
            HexsOnPath = hexsOnPath;
            Character = character;
        }

        public void LogPath()
        {
            Debug.Log(":::: PATH DATA START ::::");
            Debug.Log("Path Length: " + HexsOnPath.Count);
            Debug.Log("Start Grid Pos: " + Start.GridPosition.x + ", " + Start.GridPosition.y);

            int pathIndex = 1;
            foreach (LevelNode h in HexsOnPath)
            {
                Debug.Log("Hex " + pathIndex + " Grid Pos: " + h.GridPosition.x + ", " + h.GridPosition.y);
                pathIndex++;
            }

            Debug.Log("Destination = " + Destination.GridPosition.x + ", " + Destination.GridPosition.y);
            //Debug.Log("Path validity: " + IsValid.ToString());
            Debug.Log(":::: PATH DATA END ::::");
        }

        // Properties
        #region

        #endregion

        // Getters + Accessors
        #region

        public LevelNode Start { get; }
        public LevelNode Destination => HexsOnPath.LastOrDefault();
        public List<LevelNode> HexsOnPath { get; } = new List<LevelNode>();
        public HexCharacterModel Character { get; }
        public int Length => HexsOnPath.Count;

        #endregion
    }
}
