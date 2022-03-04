using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using HexGameEngine.HexTiles;
using HexGameEngine.Pathfinding;
using HexGameEngine.TurnLogic;
using HexGameEngine.Characters;
using TMPro;
using DG.Tweening;
using HexGameEngine.Utilities;
using HexGameEngine.Perks;

namespace HexGameEngine.Combat
{
    public class MoveActionController : Singleton<MoveActionController>
    {
        // Properties + Components
        #region
        //private HexCharacterModel selectedCharacter;
        private LevelNode clickedHex;
        private Path currentPath;

        [Header("Path Cost Pop Up Components")]
        [SerializeField] GameObject pathCostVisualParent;
        [SerializeField] GameObject pathCostPositionParent;
        [SerializeField] CanvasGroup pathCostCg;
        [SerializeField] TextMeshProUGUI pathCostText;
        

        #endregion

        // Input
        #region
        private void Update()
        {
            if (GameController.Instance.GameState != GameState.CombatActive) return;

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                ClearPath();
                clickedHex = null;
                ResetSelectionState();
            }

            // Move path cost panel if the screen moves
            if (clickedHex && pathCostVisualParent.activeSelf == true &&
                pathCostPositionParent.transform.position != clickedHex.WorldPosition)
                pathCostPositionParent.transform.position = clickedHex.WorldPosition;
        }
        public void HandleHexClicked(LevelNode h)
        {
            Debug.Log("MoveActionController.HandleHexClicked() called on hex: " + h.GridPosition.x.ToString() + ", " + h.GridPosition.y.ToString());
            if (TurnController.Instance.EntityActivated == null) return;

            HexCharacterModel character = TurnController.Instance.EntityActivated;
            if (character.controller != Controller.Player) return;

            // Crippled characters cant move
            if(!HexCharacterController.Instance.IsCharacterAbleToMove(character))
            {
                Debug.Log(character.myName + " is unable to move, cancelling move action request");
                return;
            }

            // First hex selection
            if(clickedHex == null)
            {
                List<LevelNode> validMoveLocations =  Pathfinder.GetAllValidPathableDestinations(character, character.currentTile, LevelController.Instance.AllLevelNodes.ToList());
                LevelController.Instance.MarkTilesInRange(validMoveLocations);
                HandleFirstHexSelection(h, character);
            }     
            
            // Second hex selection
            else if(clickedHex != null && h == clickedHex && currentPath != null)
            {
                // do move stuff
                LevelController.Instance.UnmarkAllTiles();
                LevelController.Instance.HandleMoveDownPath(character, currentPath);
                ResetSelectionState();

            }

            // New hex selection
            else if (clickedHex != null && h != clickedHex)
            {
                ClearPath();
                HandleFirstHexSelection(h, character);
            }
        }       
        private void HandleFirstHexSelection(LevelNode hexClicked, HexCharacterModel character)
        {
            Path p = Pathfinder.GetPath(character, character.currentTile, hexClicked, LevelController.Instance.AllLevelNodes.ToList());
            if (Pathfinder.IsPathValid(p))
            {
                clickedHex = hexClicked;
                currentPath = p;

                // Sho move markers for each tile on the path
                foreach(LevelNode h in currentPath.HexsOnPath)
                {
                    h.ShowMoveMarker();
                }

                ShowPathCostPopup(Pathfinder.GetEnergyCostOfPath(character, character.currentTile, p.HexsOnPath));

                // Characters with Slippery perk are immune to free strikes.
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Slippery)) return;

                // Check Free strike opportunities along the path.
                List<LevelNode> tilesMovedFrom = new List<LevelNode>();
                List<HexCharacterModel> freeStrikers = new List<HexCharacterModel>();
                tilesMovedFrom.Add(character.currentTile);
                tilesMovedFrom.AddRange(p.HexsOnPath);
                tilesMovedFrom.Remove(p.Destination);

                // Determine which characters are able to free strike the tile.
                foreach(LevelNode h in tilesMovedFrom)
                {
                    List<LevelNode> meleeTiles = LevelController.Instance.GetAllHexsWithinRange(h, 1);
                    foreach(LevelNode meleeHex in meleeTiles)
                    {
                        if(meleeHex.myCharacter != null &&
                            !HexCharacterController.Instance.IsTargetFriendly(character, meleeHex.myCharacter) &&
                             HexCharacterController.Instance.IsCharacterAbleToMakeFreeStrikes(meleeHex.myCharacter) &&
                             !freeStrikers.Contains(meleeHex.myCharacter))
                        {
                            freeStrikers.Add(meleeHex.myCharacter);
                        }
                    }
                }

                // disable all character free strike indicators
                foreach(HexCharacterModel enemy in freeStrikers)
                {
                    // Show the character's free strike indicator
                    HexCharacterController.Instance.ShowFreeStrikeIndicator(enemy.hexCharacterView);
                }

            }
            else
            {
                HidePathCostPopup();
            }

        }
        #endregion    

        // Misc
        #region
       
        private void ClearPath()
        {
            if (currentPath == null) return;

            foreach (LevelNode h in currentPath.HexsOnPath)
            {
                h.HideMoveMarker();
               // h.TileSprite.color = Color.white;
            }
            currentPath = null;

           // HexCharacterController.Instance.HideAllFreeStrikeIndicators();
           // HidePathCostPopup();
        }
        public void ResetSelectionState()
        {
            LevelController.Instance.UnmarkAllTiles();
            HexCharacterController.Instance.HideAllFreeStrikeIndicators();
            ClearPath();
            HidePathCostPopup();
            clickedHex = null;
        }
        #endregion

        // Path Energy Cost Pop Up Logic
        #region
        public void ShowPathCostPopup(int cost)
        {
            pathCostVisualParent.SetActive(true);
            //pathCostPositionParent.transform.position = destination.WorldPosition;
            pathCostText.text = cost.ToString();
            pathCostCg.alpha = 0;
            pathCostCg.DOFade(1, 0.5f);
        }
        public void HidePathCostPopup()
        {
            pathCostCg.alpha = 1;
            pathCostVisualParent.SetActive(false);
            pathCostCg.alpha = 0;
        }
        #endregion
    }
}