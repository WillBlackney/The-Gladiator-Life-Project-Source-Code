using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Characters;

namespace WeAreGladiators.Pathfinding
{
    public class Path
    {

        // Properties
        #region
        LevelNode start;
        List<LevelNode> hexsOnPath = new List<LevelNode>();
        HexCharacterModel character;

        #endregion

        // Getters + Accessors
        #region
        public LevelNode Start
        {
            get { return start; }
        }
        public LevelNode Destination
        {
            get { return HexsOnPath.LastOrDefault(); }
        }
        public List<LevelNode> HexsOnPath
        {
            get { return hexsOnPath; }
        }
        public HexCharacterModel Character
        {
            get { return character; }
        }
        public int Length
        {
            get { return hexsOnPath.Count; }
        }
        #endregion


        public Path(LevelNode start, List<LevelNode> hexsOnPath, HexCharacterModel character )
        {
            this.start = start;
            this.hexsOnPath = hexsOnPath;
            this.character = character;
        }

        public void LogPath()
        {
            Debug.Log(":::: PATH DATA START ::::");
            Debug.Log("Path Length: " + hexsOnPath.Count.ToString());
            Debug.Log("Start Grid Pos: " + start.GridPosition.x.ToString() + ", " + start.GridPosition.y.ToString());

            int pathIndex = 1;
            foreach (LevelNode h in hexsOnPath)
            {
                Debug.Log("Hex " + pathIndex.ToString() + " Grid Pos: " + h.GridPosition.x.ToString() + ", " + h.GridPosition.y.ToString());
                pathIndex++;
            }

            Debug.Log("Destination = " + Destination.GridPosition.x.ToString() + ", " + Destination.GridPosition.y.ToString());
            //Debug.Log("Path validity: " + IsValid.ToString());
            Debug.Log(":::: PATH DATA END ::::");
        }
    }
}