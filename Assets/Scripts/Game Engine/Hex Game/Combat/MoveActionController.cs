﻿using System.Collections;
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
        public List<HexCharacterModel> GetFreeStrikersAndSpearWallStrikersOnPath(HexCharacterModel characterMoving, Path p)
        {
            // Check Free strike opportunities along the path.
            List<LevelNode> tilesMovedFrom = new List<LevelNode>();
            List<HexCharacterModel> freeStrikers = new List<HexCharacterModel>();
            tilesMovedFrom.Add(characterMoving.currentTile);
            tilesMovedFrom.AddRange(p.HexsOnPath);
            tilesMovedFrom.Remove(p.Destination);

            // Determine which characters are able to free strike the tile.
            //HexCharacterController.Instance.HideAllFreeStrikeIndicators();
            foreach (LevelNode h in tilesMovedFrom)
            {
                // Enemies dont free strike when a character moves through an ally
                if (h.myCharacter != null && h.myCharacter != characterMoving)
                    continue;

                List<LevelNode> meleeTiles = LevelController.Instance.GetAllHexsWithinRange(h, 1);
                foreach (LevelNode meleeHex in meleeTiles)
                {
                    // Check validity of free strike
                    if (meleeHex.myCharacter != null &&
                        !HexCharacterController.Instance.IsTargetFriendly(characterMoving, meleeHex.myCharacter) &&
                         HexCharacterController.Instance.IsCharacterAbleToMakeFreeStrikes(meleeHex.myCharacter) &&
                         !freeStrikers.Contains(meleeHex.myCharacter))
                    {
                        freeStrikers.Add(meleeHex.myCharacter);
                    }
                }
            }

            // Determine spear wall attacks
            tilesMovedFrom.Add(p.Destination);
            foreach (LevelNode h in tilesMovedFrom)
            {
                List<LevelNode> meleeTiles = LevelController.Instance.GetAllHexsWithinRange(h, 1);
                foreach (LevelNode meleeHex in meleeTiles)
                {
                    // Check validity of free strike
                    if (meleeHex.myCharacter != null &&
                         HexCharacterController.Instance.IsCharacterAbleToMakeSpearWallAttack(meleeHex.myCharacter) &&
                        !HexCharacterController.Instance.IsTargetFriendly(characterMoving, meleeHex.myCharacter) &&                        
                         !freeStrikers.Contains(meleeHex.myCharacter))
                    {
                        freeStrikers.Add(meleeHex.myCharacter);
                    }
                }
            }


            return freeStrikers;
        }
        public List<HexCharacterModel> GetFreeStrikersOnPath(HexCharacterModel characterMoving, Path p)
        {
            if (PerkController.Instance.DoesCharacterHavePerk(characterMoving.pManager, Perk.Slippery)) return new List<HexCharacterModel>();
            // Check Free strike opportunities along the path.
            List<LevelNode> tilesMovedFrom = new List<LevelNode>();
            List<HexCharacterModel> freeStrikers = new List<HexCharacterModel>();
            tilesMovedFrom.Add(characterMoving.currentTile);
            tilesMovedFrom.AddRange(p.HexsOnPath);
            tilesMovedFrom.Remove(p.Destination);

            // Determine which characters are able to free strike the tile.
            foreach (LevelNode h in tilesMovedFrom)
            {
                // Enemies dont free strike when a character moves through an ally
                if (h.myCharacter != null && h.myCharacter != characterMoving)
                    continue;

                List<LevelNode> meleeTiles = LevelController.Instance.GetAllHexsWithinRange(h, 1);
                foreach (LevelNode meleeHex in meleeTiles)
                {
                    // Check validity of free strike
                    if (meleeHex.myCharacter != null &&
                        !HexCharacterController.Instance.IsTargetFriendly(characterMoving, meleeHex.myCharacter) &&
                         HexCharacterController.Instance.IsCharacterAbleToMakeFreeStrikes(meleeHex.myCharacter) &&
                         !freeStrikers.Contains(meleeHex.myCharacter))
                    {
                        freeStrikers.Add(meleeHex.myCharacter);
                    }
                }
            }

            return freeStrikers;
        }
        private void HandleFirstHexSelection(LevelNode hexClicked, HexCharacterModel character)
        {
            Path p = Pathfinder.GetValidPath(character, character.currentTile, hexClicked, LevelController.Instance.AllLevelNodes.ToList());
            if (Pathfinder.IsPathValid(p))
            {
                clickedHex = hexClicked;
                currentPath = p;

                // Show move markers for each tile on the path
                
                foreach(LevelNode h in currentPath.HexsOnPath)
                {
                    h.ShowMoveMarker();
                }

                // Draw dotted path
                List<Vector2> points = new List<Vector2>();
                points.Add(p.Start.WorldPosition);
                for (int i = 0; i < p.HexsOnPath.Count; i++) points.Add(p.HexsOnPath[i].ElevationPositioningParent.transform.position);
                CardGameEngine.DottedLine.Instance.DrawPathAlongPoints(points);

                // Show UI indicators
                int energyCost = Pathfinder.GetActionPointCostOfPath(character, character.currentTile, p.HexsOnPath);
                ShowPathCostPopup(energyCost);
                CombatUIController.Instance.EnergyBar.OnAbilityButtonMouseEnter(character.currentEnergy, energyCost);
                CombatUIController.Instance.DoFatigueCostDemo(Pathfinder.GetFatigueCostOfPath(character, character.currentTile, p.HexsOnPath),
                    character.currentFatigue, StatCalculator.GetTotalMaxFatigue(character));

                // Characters with Slippery perk are immune to free strikes.
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Slippery)) return;

                // Check Free strike opportunities along the path.
                HexCharacterController.Instance.HideAllFreeStrikeIndicators();
                List<HexCharacterModel> freeStrikers = GetFreeStrikersAndSpearWallStrikersOnPath(character, p);

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
                CombatUIController.Instance.EnergyBar.UpdateIcons(character.currentEnergy, 0.25f);
                CombatUIController.Instance.ResetFatigueCostPreview();
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
            }
            currentPath = null;
            CardGameEngine.DottedLine.Instance.DestroyAllPaths();
        }
        public void ResetSelectionState(bool resetEnergyBar = true)
        {
            LevelController.Instance.UnmarkAllTiles();
            HexCharacterController.Instance.HideAllFreeStrikeIndicators();
            ClearPath();
            HidePathCostPopup();
            if(TurnController.Instance.EntityActivated != null && resetEnergyBar)
            {
                CombatUIController.Instance.EnergyBar.UpdateIcons(TurnController.Instance.EntityActivated.currentEnergy, 0.25f);
                CombatUIController.Instance.ResetFatigueCostPreview();
            }
               
            clickedHex = null;
        }
        #endregion

        // Path Energy Cost Pop Up Logic
        #region
        public void ShowPathCostPopup(int cost)
        {
            pathCostVisualParent.SetActive(true);
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