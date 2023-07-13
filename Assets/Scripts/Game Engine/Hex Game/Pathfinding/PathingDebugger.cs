using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Characters;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Pathfinding
{
    public class PathingDebugger : Singleton<PathingDebugger>
    {
        // Properties + Components
        #region
        [SerializeField] TextMeshProUGUI startHexText;
        [SerializeField] TextMeshProUGUI endHexText;
        [SerializeField] TextMeshProUGUI moveCostText;
        [SerializeField] TextMeshProUGUI pathCountText;

        private Hex firstHex;
        private Hex secondHex;
        private List<Hex> currentPath = null;
        private List<Hex> pathableHexs = null;

        private HexCharacterModel testCharacter;
        #endregion

        // Initialization
        #region
        private void Start()
        {
            HandleSetup();
            
        }
        private void HandleSetup()
        {
            testCharacter = new HexCharacterModel();
        }
        #endregion

        // Input
        #region

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                HandleRightClick();
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                HandleEPressed();
            }
        }
        public void HandleHexClicked(Hex hex)
        {
            if (!firstHex)
            {
                SetFirstHex(hex);
               // Pathfinder.PlaceCharacterOnHex(testCharacter, hex);
            }
            else if (firstHex && !secondHex)
            {
                SetSecondHex(hex);

                Path p = null;//Pathfinder.GetPath(testCharacter, firstHex, secondHex, LevelController.Instance.CurrentHexMap.Hexs);
                if (p != null)
                {
                   // SetCurrentPath(p.HexsOnPath);
                    pathCountText.text = "Path Length: " + currentPath.Count;
                    moveCostText.text = "AP Cost: " + Pathfinder.GetActionPointCostOfPath(testCharacter, testCharacter.currentTile, p.HexsOnPath);
                }
                else
                {
                    Debug.Log("Second hex is NOT in range");
                }

            }
        }
        void HandleRightClick()
        {
            if (secondHex && firstHex)
            {
                ClearSecondHex();
                ClearCurrentPath();
            }
               
            else if (!secondHex && firstHex)
            {
                ClearFirstHex();
            }
               


        }
        void HandleEPressed()
        {
            /*
            if(firstHex && !secondHex && pathableHexs == null)
            {
                ClearPathableHexs();
                SetPathableHexs(Pathfinder.GetAllValidPathableDestinations(testCharacter, firstHex, LevelController.Instance.CurrentHexMap.Hexs));
            }
            else if(pathableHexs != null)
            {
                ClearPathableHexs();
            }
            */
        }
        #endregion

        // Set First and Second hexs
        #region
        private void SetFirstHex(Hex hex)
        {
            firstHex = hex;
            startHexText.text = "Start: " + hex.GridPosition.x.ToString() + ", " + hex.GridPosition.y.ToString();
        }
        private void ClearFirstHex()
        {
            firstHex = null;
            startHexText.text = "No Selection";
        }
        private void SetSecondHex(Hex hex)
        {
            secondHex = hex;
            endHexText.text = "End: " + hex.GridPosition.x.ToString() + ", " + hex.GridPosition.y.ToString();
        }
        private void ClearSecondHex()
        {
            secondHex = null;
            endHexText.text = "No Selection";
        }
        #endregion

        // Set + Clear Pathable Hexs
        #region
        private void SetPathableHexs(List<Hex> newHexs)
        {
            pathableHexs = newHexs;
            foreach(Hex h in pathableHexs)
            {
                h.TileSprite.color = Color.cyan;
            }
        }
        private void ClearPathableHexs()
        {
            if(pathableHexs != null)
            {
                foreach (Hex h in pathableHexs)
                {
                   // h.TileSprite.color = Color.white;
                }
            }
            pathableHexs = null;
           
        }
        #endregion

        // Set + Clear Path
        #region
        private void SetCurrentPath(List<Hex> newPath)
        {
            ClearCurrentPath();
            currentPath = newPath;

            for (int i = 0; i < currentPath.Count; i++)
            {
                // highlight last/destination hex in yellow
                if(i == currentPath.Count - 1)
                {
                    //currentPath[i].TileSprite.color = Color.yellow;
                }
                else
                {
                   // currentPath[i].TileSprite.color = Color.blue;
                }
            }

        }
        private void ClearCurrentPath()
        {
            if (currentPath == null) return;

            foreach (Hex h in currentPath)
            {
                //h.TileSprite.color = Color.white;
            }

            currentPath = null;
            pathCountText.text = "NO SELECTION";
            moveCostText.text = "NO SELECTION";
        }
        #endregion
     
       

    }
}