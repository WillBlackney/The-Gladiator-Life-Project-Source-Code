using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using WeAreGladiators.Characters;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Pathfinding;
using WeAreGladiators.Perks;
using WeAreGladiators.TurnLogic;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.Combat
{
    public class MoveActionController : Singleton<MoveActionController>
    {
        // Properties + Components
        #region

        //private HexCharacterModel selectedCharacter;
        private LevelNode clickedHex;
        private Path currentPath;

        [Header("Path Cost Pop Up Components")]
        [SerializeField]
        private Canvas pathCostRootCanvas;
        [SerializeField] private GameObject pathCostPositionParent;
        [SerializeField] private CanvasGroup pathCostCg;
        [SerializeField] private TextMeshProUGUI pathCostText;

        #endregion

        // Input
        #region

        private void Update()
        {
            if (GameController.Instance.GameState != GameState.CombatActive)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                clickedHex = null;
                ResetSelectionState();
            }

            // Move path cost panel if the screen moves
            if (clickedHex && pathCostRootCanvas.isActiveAndEnabled &&
                pathCostPositionParent.transform.position != clickedHex.WorldPosition)
            {
                pathCostPositionParent.transform.position = clickedHex.WorldPosition;
            }
        }
        public void HandleHexClicked(LevelNode h)
        {
            Debug.Log("MoveActionController.HandleHexClicked() called on hex: " + h.GridPosition.x + ", " + h.GridPosition.y);
            if (TurnController.Instance.EntityActivated == null)
            {
                return;
            }

            HexCharacterModel activatedCharacter = TurnController.Instance.EntityActivated;
            if (activatedCharacter.controller != Controller.Player)
            {
                return;
            }

            // Return if character is immobile
            if (!HexCharacterController.Instance.IsCharacterAbleToMove(activatedCharacter))
            {
                Debug.Log(activatedCharacter.myName + " is unable to move, cancelling move action request");
                ActionErrorGuidanceController.Instance.ShowErrorMessage(activatedCharacter, "Character is unable to move");
                return;
            }

            // First hex selection
            if (clickedHex == null)
            {
                List<LevelNode> validMoveLocations = Pathfinder.GetAllValidPathableDestinations(activatedCharacter, activatedCharacter.currentTile, LevelController.Instance.AllLevelNodes.ToList());
                LevelController.Instance.MarkTilesInRange(validMoveLocations);
                //TargetGuidanceController.Instance.BuildAndShow(GuidanceInstruction.SelectDestination, 0.5f);
                HandleFirstHexSelection(h, activatedCharacter);
                CursorController.Instance.SetFallbackCursor(CursorType.MoveClick);
                CursorController.Instance.SetCursor(CursorType.MoveClick);
            }

            // Second hex selection
            else if (clickedHex != null && h == clickedHex && currentPath != null)
            {
                // Move + hide targeting/helper UI
                LevelController.Instance.HandleMoveDownPath(activatedCharacter, currentPath);
                ResetSelectionState();

            }

            // New hex selection
            else if (clickedHex != null && h != clickedHex)
            {
                ClearPath();
                HandleFirstHexSelection(h, activatedCharacter);
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
            bool allowNimbleCheck = true;
            foreach (LevelNode h in tilesMovedFrom)
            {
                // Enemies dont free strike when a character moves through an ally
                if (h.myCharacter != null && h.myCharacter != characterMoving)
                {
                    continue;
                }

                // Not free strikes if first Nimble move
                if (h == p.HexsOnPath[0] &&
                    allowNimbleCheck &&
                    characterMoving.tilesMovedThisTurn == 0 &&
                    PerkController.Instance.DoesCharacterHavePerk(characterMoving.pManager, Perk.Nimble))
                {
                    allowNimbleCheck = false;
                    continue;
                }

                allowNimbleCheck = false;

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
            if (PerkController.Instance.DoesCharacterHavePerk(characterMoving.pManager, Perk.Slippery))
            {
                return new List<HexCharacterModel>();
            }
            // Check Free strike opportunities along the path.
            List<LevelNode> tilesMovedFrom = new List<LevelNode>();
            List<HexCharacterModel> freeStrikers = new List<HexCharacterModel>();
            tilesMovedFrom.Add(characterMoving.currentTile);
            tilesMovedFrom.AddRange(p.HexsOnPath);
            tilesMovedFrom.Remove(p.Destination);

            // Determine which characters are able to free strike the tile.
            bool allowNimbleCheck = true;
            foreach (LevelNode h in tilesMovedFrom)
            {
                // Enemies dont free strike when a character moves through an ally
                if (h.myCharacter != null && h.myCharacter != characterMoving)
                {
                    continue;
                }

                // Not free strikes if first Nimble move
                if (h == p.HexsOnPath[0] &&
                    allowNimbleCheck &&
                    characterMoving.tilesMovedThisTurn == 0 &&
                    PerkController.Instance.DoesCharacterHavePerk(characterMoving.pManager, Perk.Nimble))
                {
                    allowNimbleCheck = false;
                    continue;
                }

                allowNimbleCheck = false;

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
            if (p != null)
            {
                clickedHex = hexClicked;
                currentPath = p;

                // Show guidance modal
                TargetGuidanceController.Instance.BuildAndShow(GuidanceInstruction.SelectDestination, 0.5f);

                // Show move markers for each tile on the path                
                foreach (LevelNode h in currentPath.HexsOnPath)
                {
                    h.ShowMoveMarker();
                }

                // Draw dotted path
                List<Vector2> points = new List<Vector2>();
                points.Add(p.Start.WorldPosition);
                for (int i = 0; i < p.HexsOnPath.Count; i++)
                {
                    points.Add(p.HexsOnPath[i].ElevationPositioningParent.transform.position);
                }
                DottedLine.Instance.DrawPathAlongPoints(points);

                // Show UI indicators
                int energyCost = Pathfinder.GetActionPointCostOfPath(character, character.currentTile, p.HexsOnPath);
                ShowPathCostPopup(energyCost);
                CombatUIController.Instance.EnergyBar.OnAbilityButtonMouseEnter(character.currentActionPoints, energyCost);
                // CombatUIController.Instance.DoFatigueCostDemo(Pathfinder.GetFatigueCostOfPath(character, character.currentTile, p.HexsOnPath),
                //    character.currentFatigue, StatCalculator.GetTotalMaxFatigue(character));

                // Characters with Slippery perk are immune to free strikes.
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Slippery))
                {
                    return;
                }

                // Check Free strike opportunities along the path.
                HexCharacterController.Instance.HideAllFreeStrikeIndicators();
                List<HexCharacterModel> freeStrikers = GetFreeStrikersAndSpearWallStrikersOnPath(character, p);

                // disable all character free strike indicators
                foreach (HexCharacterModel enemy in freeStrikers)
                {
                    // Show the character's free strike indicator
                    HexCharacterController.Instance.ShowFreeStrikeIndicator(enemy.hexCharacterView);
                }
            }
            else
            {
                HidePathCostPopup();
                CursorController.Instance.SetCursor(CursorType.NormalPointer);
                CombatUIController.Instance.EnergyBar.UpdateIcons(character.currentActionPoints, 0.25f);
            }

        }

        #endregion

        // Misc
        #region

        private void ClearPath()
        {
            if (currentPath == null)
            {
                return;
            }

            foreach (LevelNode h in currentPath.HexsOnPath)
            {
                h.HideMoveMarker();
            }
            currentPath = null;
            DottedLine.Instance.DestroyAllPaths();
        }
        public void ResetSelectionState(bool resetEnergyBar = true)
        {
            CursorController.Instance.SetFallbackCursor(CursorType.NormalPointer);
            CursorController.Instance.SetCursor(CursorType.NormalPointer);
            LevelController.Instance.UnmarkAllTiles();
            LevelController.Instance.UnmarkAllSubTargetMarkers();
            TargetGuidanceController.Instance.Hide();
            HexCharacterController.Instance.HideAllFreeStrikeIndicators();
            TargetGuidanceController.Instance.Hide();
            ClearPath();
            HidePathCostPopup();
            if (TurnController.Instance.EntityActivated != null && resetEnergyBar)
            {
                CombatUIController.Instance.EnergyBar.UpdateIcons(TurnController.Instance.EntityActivated.currentActionPoints, 0.25f);
            }

            clickedHex = null;
        }

        #endregion

        // Path Energy Cost Pop Up Logic
        #region

        public void ShowPathCostPopup(int cost)
        {
            pathCostRootCanvas.enabled = true;
            pathCostText.text = cost.ToString();
            pathCostCg.DOKill();
            pathCostCg.DOFade(1, 0.25f);
        }
        public void HidePathCostPopup()
        {
            pathCostCg.DOKill();
            pathCostRootCanvas.enabled = false;
            pathCostCg.alpha = 0;
        }

        #endregion
    }

    public enum PointOnPath
    {
        Middle = 0,
        First = 1,
        Last = 2
    }
}
