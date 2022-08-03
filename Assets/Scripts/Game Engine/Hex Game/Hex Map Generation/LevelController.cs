using DG.Tweening;
using HexGameEngine.Abilities;
using HexGameEngine.Characters;
using HexGameEngine.Combat;
using HexGameEngine.Pathfinding;
using HexGameEngine.Perks;
using HexGameEngine.TurnLogic;
using HexGameEngine.UI;
using HexGameEngine.Utilities;
using HexGameEngine.VisualEvents;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexGameEngine.HexTiles
{
    public class LevelController : Singleton<LevelController>
    {
        // Properties + Components
        #region
        [Header("Prefabs + Core Components")]
        [SerializeField] CombatMapSeedDataSO[] allCombatMapSeeds;
        [SerializeField] HexDataSO[] allHexTileData;
        [SerializeField] GameObject obstaclePrefab;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Node References")]
        [SerializeField] private GameObject nodesVisualParent;
        [SerializeField] private LevelNode[] allLevelNodes;       
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Off Screen Transform References")]
        [SerializeField] private LevelNode enemyOffScreenNode;
        [SerializeField] private LevelNode defenderOffScreenNode;

        [Header("Tile Info Pop Up Components")]
        [SerializeField] GameObject obstructionIndicator;
        [SerializeField] GameObject tileInfoVisualParent;
        [SerializeField] GameObject tileInfoPositionParent;
        [SerializeField] CanvasGroup tileInfoCg;
        [SerializeField] TextMeshProUGUI tileInfoNameText;
        [SerializeField] TextMeshProUGUI tileInfoDescriptionText;
        [SerializeField] ModalDottedRow[] tileEffectDotRows;
        [SerializeField] RectTransform[] tileInfoPopUpLayoutRebuilds;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        private float popupDelay = 0.5f;

        private List<LevelNode> markedTiles = new List<LevelNode>();

        [Header("KBC Scenery References")]
        [SerializeField] private GameObject graveyardSceneryParent;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Dungeon Scenery References")]
        [SerializeField] private GameObject mainArenaViewParent;
        [SerializeField] private GameObject[] allNightTimeArenaParents;
        [SerializeField] private GameObject[] allDayTimeArenaParents;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        #endregion

        // Getters + Accessors
        #region
        public HexDataSO[] AllHexTileData
        {
            get { return allHexTileData; }
        }
        public LevelNode[] AllLevelNodes
        {
            get { return allLevelNodes; }
        }
        public static LevelNode HexMousedOver
        {
            get; private set;
        }
        public LevelNode EnemyOffScreenNode
        {
            get { return enemyOffScreenNode; }
            private set { enemyOffScreenNode = value; }
        }
        public LevelNode DefenderOffScreenNode
        {
            get { return defenderOffScreenNode; }
            private set { defenderOffScreenNode = value; }
        }

        #endregion

        // Map Generation + Teardown
        #region
        public void HandleTearDownCombatViews()
        {
            AbilityController.Instance.HideHitChancePopup();
            AbilityPopupController.Instance.HidePanel();
            MoveActionController.Instance.HidePathCostPopup();
            DisableAllArenas();
            HideTileInfoPopup();
            HideAllNodeViews();
        }

        public SerializedCombatMapData GenerateLevelNodes()
        {
            SerializedCombatMapData ret = new SerializedCombatMapData();

            // Get random seed
            CombatMapSeedDataSO seed = allCombatMapSeeds.GetRandomElement();
            if (!seed) return null;

            // Reset type, obstacle and elevation on all nodes
            foreach (LevelNode n in allLevelNodes)
                n.Reset();

            // Get banned nodes for obstacles
            List<LevelNode> spawnPositions = GetPlayerSpawnZone();
            spawnPositions.AddRange(GetEnemySpawnZone());

            // Rebuild
            foreach (LevelNode n in allLevelNodes)
            {
                int elevationRoll = RandomGenerator.NumberBetween(1, 100);
                if (elevationRoll >= 1 && elevationRoll <= seed.elevationPercentage)
                    n.SetHexTileElevation(TileElevation.Elevated);

                // Set up tile type + data
                int tileTypeRoll = RandomGenerator.NumberBetween(1, 100);
                HexMapTilingConfig randomHexConfig = null;
                foreach (HexMapTilingConfig c in seed.tilingConfigs)
                {
                    if (tileTypeRoll >= c.lowerProbability && tileTypeRoll <= c.upperProbability)
                    {
                        randomHexConfig = c;
                        break;
                    }
                }
                n.BuildFromData(randomHexConfig.hexData);

                // To do: randomize and set obstacle
                int obstructionRoll = RandomGenerator.NumberBetween(1, 100);
                if (obstructionRoll >= 1 && obstructionRoll <= seed.obstructionPercentage &&
                    Pathfinder.IsHexSpawnable(n) &&
                    !spawnPositions.Contains(n) &&
                    (n.Elevation == TileElevation.Ground || (seed.allowObstaclesOnElevation && n.Elevation == TileElevation.Elevated)))        
                    n.SetHexObstruction(true);

                SerializedLevelNodeData saveableNode = new SerializedLevelNodeData(n);
                ret.nodes.Add(saveableNode);
            }

            return ret;

        }
        public void GenerateLevelNodes(SerializedCombatMapData data)
        {
            foreach(LevelNode n in allLevelNodes)
            {
                foreach(SerializedLevelNodeData d in data.nodes)
                {
                    if(n.GridPosition == d.gridPosition)
                    {
                        n.Reset();
                        n.SetHexTileElevation(d.elevation);
                        n.BuildFromData(d.TileData);
                        n.SetHexObstruction(d.obstructed);
                        break;
                    }
                }
            }
        }
        public void SetLevelNodeDayOrNightViewState(bool dayTime)
        {
            foreach (LevelNode n in allLevelNodes)
                n.SetPillarSprites(dayTime);
        }
        #endregion

        // Spawn Zone Logic
        #region
        public List<LevelNode> GetPlayerSpawnZone()
        {
            List<LevelNode> nodes = new List<LevelNode>();
            foreach(LevelNode n in AllLevelNodes)
            {
                if(n.GridPosition.x == -2 || n.GridPosition.x == -3)
                {
                    nodes.Add(n);
                }
            }

            nodes.OrderBy(n => n.GridPosition.x);
            return nodes;
        }
        public List<LevelNode> GetEnemySpawnZone()
        {
            List<LevelNode> nodes = new List<LevelNode>();
            foreach (LevelNode n in AllLevelNodes)
            {
                if (n.GridPosition.x == 2 || n.GridPosition.x == 3)
                {
                    nodes.Add(n);
                }
            }

            nodes.OrderByDescending(n => n.GridPosition.x);
            return nodes;
        }
        public LevelNode GetRandomSpawnableLevelNode(List<LevelNode> possibleHexs, bool orderAscending = true)
        {
            List<LevelNode> validHexs = new List<LevelNode>();
            foreach (LevelNode h in possibleHexs)
            {
                if (Pathfinder.IsHexSpawnable(h))
                    validHexs.Add(h);
            }
            if(orderAscending) validHexs.OrderBy(n => n.GridPosition.x);
            else validHexs.OrderByDescending(n => n.GridPosition.x);

            if (validHexs.Count == 1)
                return validHexs[0];
            else
                return validHexs[RandomGenerator.NumberBetween(0, validHexs.Count - 1)];
        }
        #endregion

        // Character Direction + Facing
        #region
        public void FaceCharacterTowardsTargetCharacter(HexCharacterModel character, HexCharacterModel target)
        {
            FaceCharacterTowardsHex(character, target.currentTile);
        }
        public void FaceCharacterTowardsHex(HexCharacterModel character, LevelNode hex)
        {
            if(hex.GridPosition.x > character.currentTile.GridPosition.x)
            {
                SetCharacterFacing(character, Facing.Right);
            }
            else if (hex.GridPosition.x < character.currentTile.GridPosition.x)
            {
                SetCharacterFacing(character, Facing.Left);
            }
        }
        public void SetCharacterFacing(HexCharacterModel character, Facing direction)
        {
            character.currentFacing = direction;

            Debug.Log("LevelController.SetDirection() called, setting direction of " + direction.ToString());
            if (direction == Facing.Left)
            {
                FlipCharacterSprite(character.hexCharacterView, Facing.Left);
            }
            else if (direction == Facing.Right)
            {
                FlipCharacterSprite(character.hexCharacterView, Facing.Right);
            }
        }
        private void FlipCharacterSprite(HexCharacterView character, Facing direction)
        {
            Debug.Log("PositionLogic.FlipCharacterSprite() called...");
            float scale = Mathf.Abs(character.ucmVisualParent.transform.localScale.x);

            if (direction == Facing.Right)
            {
                if (character.ucmVisualParent != null)
                {
                    VisualEventManager.Instance.CreateVisualEvent(()=> character.ucmVisualParent.transform.localScale = new Vector3(scale, Mathf.Abs(scale)));
                }
            }

            else
            {
                if (character.ucmVisualParent != null)
                {
                    VisualEventManager.Instance.CreateVisualEvent(() => character.ucmVisualParent.transform.localScale = new Vector3(-scale, Mathf.Abs(scale)));
                }
            }

        }
        #endregion

        // Placement + Moving
        #region
        public void DisconnectCharacterFromTheirHex(HexCharacterModel character)
        {
            LevelNode h = character.currentTile;
            if(h != null)
                h.myCharacter = null;
            character.currentTile = null;
        }
        public void PlaceCharacterOnHex(HexCharacterModel entity, LevelNode hex, bool snapCharacterViewToHex = false)
        {
            // NOTE: should only be used to place characters that spawn at the start of the game
            Debug.Log("LevelController.PlaceCharacterOnHex() called...");
            DisconnectCharacterFromTheirHex(entity);

            hex.myCharacter = entity;
            entity.currentTile = hex;
            if (snapCharacterViewToHex)
                SnapCharacterViewToHex(entity.hexCharacterView, hex);
        }
        public void HandleMoveDownPath(HexCharacterModel character, Path path, bool payMovementCosts = true)
        {
            if (payMovementCosts)
            {
                // Pay energy cost
                HexCharacterController.Instance.ModifyEnergy(character, -Pathfinder.GetEnergyCostOfPath(character, character.currentTile, path.HexsOnPath));
            }

            // Play movement animation
            VisualEventManager.Instance.CreateVisualEvent(()=> HexCharacterController.Instance.PlayMoveAnimation(character.hexCharacterView));

            // Move to first hex           
            HandleMoveToHex(character, path.HexsOnPath[0]);

            if (path.HexsOnPath.Count > 1)
            {
                for(int i = 0; i < path.HexsOnPath.Count - 1; i++)
                {
                    HandleMoveToHex(character, path.HexsOnPath[i + 1]);
                }
            }

            // Handle stress event: Enemy moved into my melee range + moved into back arc (on destination only)
            if (character.allegiance == Allegiance.Enemy)
            {
                List<LevelNode> tiles = GetAllHexsWithinRange(character.currentTile, 1);
                List<HexCharacterModel> enemies = HexCharacterController.Instance.GetAllEnemiesOfCharacter(character);
                foreach (HexCharacterModel enemy in enemies)
                {
                    if (tiles.Contains(enemy.currentTile))
                    {
                        // Moved into back arc 
                        if (HexCharacterController.Instance.GetCharacterBackArcTiles(enemy).Contains(character.currentTile))
                        {
                            CombatController.Instance.CreateStressCheck(enemy, StressEventType.EnemyMovedBehindMe);
                        }
                        // Moved into a non back arc tile
                        else
                        {
                            CombatController.Instance.CreateStressCheck(enemy, StressEventType.EnemyMovedIntoMelee);
                        }

                    }
                }
            }

            // Finished movement, go idle animation
            VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlayIdleAnimation(character.hexCharacterView));

        }
        public void HandleMoveDownPath(HexCharacterModel character, List<LevelNode> path)
        {
            // Play movement animation
            VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlayMoveAnimation(character.hexCharacterView));

            // Move to first hex
            HandleMoveToHex(character, path[0]);

            if (path.Count > 1)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    HandleMoveToHex(character, path[i + 1]);
                }
            }

            // Handle stress event: Enemy moved into my melee range + moved into back arc (on destination only)
            if (character.allegiance == Allegiance.Enemy)
            {
                List<LevelNode> tiles = GetAllHexsWithinRange(character.currentTile, 1);
                List<HexCharacterModel> enemies = HexCharacterController.Instance.GetAllEnemiesOfCharacter(character);
                foreach (HexCharacterModel enemy in enemies)
                {
                    if (tiles.Contains(enemy.currentTile))
                    {
                        // Moved into back arc 
                        if (HexCharacterController.Instance.GetCharacterBackArcTiles(enemy).Contains(character.currentTile))
                        {
                            CombatController.Instance.CreateStressCheck(enemy, StressEventType.EnemyMovedBehindMe);
                        }
                        // Moved into a non back arc tile
                        else
                        {
                            CombatController.Instance.CreateStressCheck(enemy, StressEventType.EnemyMovedIntoMelee);
                        }

                    }
                }
            }

            // Finished movement, go idle animation
            VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlayIdleAnimation(character.hexCharacterView));

        }
        private void HandleMoveToHex(HexCharacterModel character, LevelNode destination, bool connectCharacterWithHex = true)
        {
            // Check and resolve free strikes before moving
            List<HexCharacterModel> allEnemies = HexCharacterController.Instance.GetAllEnemiesOfCharacter(character);
            List<HexCharacterModel> freeStrikeEnemies = new List<HexCharacterModel>();
            List<LevelNode> adjacentTiles = GetAllHexsWithinRange(character.currentTile, 1);
            bool didPauseMoveAnim = false;

            // Determine which enemies are valid and able to take a free strike
            if (!PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Slippery))
            {
                foreach (HexCharacterModel c in allEnemies)
                {
                    if (adjacentTiles.Contains(c.currentTile) &&
                        HexCharacterController.Instance.IsCharacterAbleToMakeFreeStrikes(c) &&
                        destination.myCharacter == null)
                    {
                        freeStrikeEnemies.Add(c);
                    }
                }
                               
                if (freeStrikeEnemies.Count > 0)
                {
                    didPauseMoveAnim = true;
                    VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlayIdleAnimation(character.hexCharacterView));
                }

                // Resolve free strike for each character
                foreach (HexCharacterModel c in freeStrikeEnemies)
                {
                    if (character.currentHealth > 0 && character.livingState == LivingState.Alive)
                    {
                        // Start free strike attack
                        AbilityController.Instance.UseAbility(c, AbilityController.Instance.FreeStrikeAbility, character);
                        VisualEventManager.Instance.InsertTimeDelayInQueue(1f);
                    }
                }
            }

            // Cancel if character was killed from free strikes
            if (character == null ||
                character.currentHealth == 0 ||
                character.livingState == LivingState.Dead)
            {
                return;
            }

            // Resume movement anim if it stopped previously during free strike sequence
            if (didPauseMoveAnim)
            {
                VisualEventManager.Instance.CreateVisualEvent(() => HexCharacterController.Instance.PlayMoveAnimation(character.hexCharacterView));
            }

            // Face towards destination hex
            FaceCharacterTowardsHex(character, destination);

            // Lock player to hex if it is unoccupied
            if(destination.myCharacter == null)
                PlaceCharacterOnHex(character, destination);

            // Move animation
            CoroutineData cData = new CoroutineData();
            VisualEventManager.Instance.CreateVisualEvent(() => DoCharacterMoveVisualEvent
                (character.hexCharacterView, destination, cData), cData, QueuePosition.Back);

            // TO DO: events that trigger when the character steps onto a new tile go here (maybe?)...
            character.tilesMovedThisTurn++;

            // Remove flight
            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Flight))
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Flight, -1);

        }
        public void DoCharacterMoveVisualEvent(HexCharacterView view, LevelNode hex, CoroutineData cData, Action onCompleteCallback = null)
        {
            StartCoroutine(DoCharacterMoveVisualEventCoroutine(view, hex, cData, onCompleteCallback));
        }
        private IEnumerator DoCharacterMoveVisualEventCoroutine(HexCharacterView view, LevelNode hex, CoroutineData cData, Action onCompleteCallback = null)
        {
            // Set up
            bool reachedDestination = false;
            Vector3 destination = new Vector3(hex.WorldPosition.x, hex.WorldPosition.y, 0);
            float moveSpeed = 5;

            // Move
            while (reachedDestination == false)
            {
                view.mainMovementParent.transform.position = Vector2.MoveTowards(view.WorldPosition, destination, moveSpeed * Time.deltaTime);

                if (view.WorldPosition == destination)
                {
                    Debug.Log("CharacterEntityController.MoveEntityToNodeCentreCoroutine() detected destination was reached...");
                    reachedDestination = true;
                }
                yield return null;
            }

            // Resolve event
            if (cData != null)
            {
                cData.MarkAsCompleted();
            }

            if (onCompleteCallback != null)
            {
                onCompleteCallback.Invoke();
            }
        }
        private void SnapCharacterViewToHex(HexCharacterView view, LevelNode hex)
        {
            Vector3 destination = new Vector3(hex.WorldPosition.x, hex.WorldPosition.y, 0);
            view.mainMovementParent.transform.position = destination;
        }
        #endregion

        // Teleportation Logic
        #region
        public void HandleTeleportCharacter(HexCharacterModel character, LevelNode destination)
        {
            // Cancel if character is immune to teleport
            if(!HexCharacterController.Instance.IsCharacterTeleportable(character))
            {
                return;
            }

            DisconnectCharacterFromTheirHex(character);
            PlaceCharacterOnHex(character, destination);

            HexCharacterView view = character.hexCharacterView;

            // Create teleport VFX on character position

            // Make character model + world space UI vanish
            VisualEventManager.Instance.CreateVisualEvent(() => {
                VisualEffectManager.Instance.CreateTeleportEffect(view.WorldPosition);
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(view, null, 0.2f);
                HexCharacterController.Instance.FadeOutCharacterModel(view.ucm, 0.2f);
                HexCharacterController.Instance.FadeOutCharacterShadow(view, 0.2f);
            });
          
            VisualEventManager.Instance.InsertTimeDelayInQueue(0.2f, QueuePosition.Back);
            VisualEventManager.Instance.CreateVisualEvent(() => SnapCharacterViewToHex(view, destination));
            // Create teleport VFX at character's destination

            // Make character model + world space UI reappear
            VisualEventManager.Instance.CreateVisualEvent(() => {
                VisualEffectManager.Instance.CreateTeleportEffect(view.WorldPosition);
                HexCharacterController.Instance.FadeInCharacterWorldCanvas(view, null, 0.2f);
                HexCharacterController.Instance.FadeInCharacterModel(view.ucm, 0.2f);
                HexCharacterController.Instance.FadeInCharacterShadow(view, 0.2f);
            });

            // Handle stress event: Enemy moved into my melee range + moved into back arc (on destination only)
            if (character.allegiance == Allegiance.Enemy)
            {
                List<LevelNode> tiles = GetAllHexsWithinRange(character.currentTile, 1);
                List<HexCharacterModel> enemies = HexCharacterController.Instance.GetAllEnemiesOfCharacter(character);
                foreach (HexCharacterModel enemy in enemies)
                {
                    if (tiles.Contains(enemy.currentTile))
                    {
                        // Moved into back arc 
                        if (HexCharacterController.Instance.GetCharacterBackArcTiles(enemy).Contains(character.currentTile))
                        {
                            CombatController.Instance.CreateStressCheck(enemy, StressEventType.EnemyMovedBehindMe);
                        }
                        // Moved into a non back arc tile
                        else
                        {
                            CombatController.Instance.CreateStressCheck(enemy, StressEventType.EnemyMovedIntoMelee);
                        }

                    }
                }
            }
        }
        public void HandleTeleportSwitchTwoCharacters(HexCharacterModel a, HexCharacterModel b, bool normalTeleportVFX = true)
        {
            // Cancel if either character is immune to teleport
            if (!HexCharacterController.Instance.IsCharacterTeleportable(a) ||
                !HexCharacterController.Instance.IsCharacterTeleportable(b))
            {
                return;
            }

            LevelNode aDestination = b.currentTile;
            LevelNode bDestination = a.currentTile;

            DisconnectCharacterFromTheirHex(a);
            DisconnectCharacterFromTheirHex(b);

            PlaceCharacterOnHex(a, aDestination);
            PlaceCharacterOnHex(b, bDestination);


            HexCharacterView viewA = a.hexCharacterView;
            HexCharacterView viewB = b.hexCharacterView;

            // Create teleport VFX on character position
            if (normalTeleportVFX)
            {
                // Make character model + world space UI vanish
                // A
                VisualEventManager.Instance.CreateVisualEvent(() => {
                    VisualEffectManager.Instance.CreateTeleportEffect(viewA.WorldPosition);
                    HexCharacterController.Instance.FadeOutCharacterWorldCanvas(viewA, null, 0.2f);
                    HexCharacterController.Instance.FadeOutCharacterModel(viewA.ucm, 0.2f);
                    HexCharacterController.Instance.FadeOutCharacterShadow(viewA, 0.2f);
                });

                // B
                VisualEventManager.Instance.CreateVisualEvent(() => {
                    VisualEffectManager.Instance.CreateTeleportEffect(viewB.WorldPosition);
                    HexCharacterController.Instance.FadeOutCharacterWorldCanvas(viewB, null, 0.2f);
                    HexCharacterController.Instance.FadeOutCharacterModel(viewB.ucm, 0.2f);
                    HexCharacterController.Instance.FadeOutCharacterShadow(viewB, 0.2f);
                });

                // Brief Delay
                VisualEventManager.Instance.InsertTimeDelayInQueue(0.2f, QueuePosition.Back);

                // Snap characters to new positions visually
                VisualEventManager.Instance.CreateVisualEvent(() => SnapCharacterViewToHex(viewA, aDestination));
                VisualEventManager.Instance.CreateVisualEvent(() => SnapCharacterViewToHex(viewB, bDestination
                    ));

                // Create teleport VFX at character's destination, fade back in views
                VisualEventManager.Instance.CreateVisualEvent(() => {
                    VisualEffectManager.Instance.CreateTeleportEffect(viewA.WorldPosition);
                    HexCharacterController.Instance.FadeInCharacterWorldCanvas(viewA, null, 0.2f);
                    HexCharacterController.Instance.FadeInCharacterModel(viewA.ucm, 0.2f);
                    HexCharacterController.Instance.FadeInCharacterShadow(viewA, 0.2f);
                });
                VisualEventManager.Instance.CreateVisualEvent(() => {
                    VisualEffectManager.Instance.CreateTeleportEffect(viewB.WorldPosition);
                    HexCharacterController.Instance.FadeInCharacterWorldCanvas(viewB, null, 0.2f);
                    HexCharacterController.Instance.FadeInCharacterModel(viewB.ucm, 0.2f);
                    HexCharacterController.Instance.FadeInCharacterShadow(viewB, 0.2f);
                });
            }
            else
            {
                VisualEventManager.Instance.CreateVisualEvent(() => DoCharacterMoveVisualEvent
                (viewA, aDestination, null), QueuePosition.Back);

                VisualEventManager.Instance.CreateVisualEvent(() => DoCharacterMoveVisualEvent
                (viewB, bDestination, null), QueuePosition.Back);

                VisualEventManager.Instance.InsertTimeDelayInQueue(0.25f);
            }
           

        }
        #endregion

        // Knock Back Logic
        #region
        public LevelNode GetAdjacentHexByDirection(LevelNode start, HexDirection direction)
        {
            Debug.Log("GetAdjacentHexByDirection() called, start = " + start.PrintGridPosition() + ", direction = " + direction.ToString());

            LevelNode hRet = null;
            foreach(LevelNode h in GetAllHexsWithinRange(start, 1))
            {
                if(GetDirectionToTargetHex(start, h) == direction)
                {
                    hRet = h;
                    break;
                }
            }

            if(hRet == null)
            {
                Debug.LogWarning("GetAdjacentHexByDirection() did not find an adjacent hex in the direction of " + direction.ToString() + " from start " +
                    start.PrintGridPosition() + ", returning null...");
            }
            return hRet;
        }
        public HexDirection GetDirectionToTargetHex(LevelNode start, LevelNode target)
        {
            HexDirection direction = HexDirection.None;

            float startX = start.transform.position.x;
            float startY = start.transform.position.y;
            float targetX = target.transform.position.x;
            float targetY = target.transform.position.y;

            // North East
            if (targetX > startX && targetY > startY)
            {
                direction = HexDirection.NorthEast;
            }

            // South East
            else if (targetX > startX && targetY < startY)
            {
                direction = HexDirection.SouthEast;
            }

            // South West
            else if (targetX < startX && targetY < startY)
            {
                direction = HexDirection.SouthWest;
            }

            // North West
            else if (targetX < startX && targetY > startY)
            {
                direction = HexDirection.NorthWest;
            }

            // North
            else if (targetX == startX && targetY > startY)
            {
                direction = HexDirection.North;
            }

            // South
            else if (targetX == startX && targetY < startY)
            {
                direction = HexDirection.South;
            }

            // East
            else if (targetX > startX && targetY == startY)
            {
                direction = HexDirection.East;
            }

            // West
            else if (targetX < startX && targetY == startY)
            {
                direction = HexDirection.West;
            }
            Debug.Log("DIRECTION CHECK: Start: " + startX.ToString() + ", " + startY.ToString() + ". Target: " + targetX.ToString() + ", " + targetY.ToString() + ". Direction = " + direction.ToString());

            return direction;
        }
        public void HandleKnockBackCharacter(HexCharacterModel character, LevelNode destination)
        {
            // Fortified + Implaccable characters cant be knocked back
            if(HexCharacterController.Instance.IsCharacterKnockBackable(character))
            {
                DisconnectCharacterFromTheirHex(character);
                PlaceCharacterOnHex(character, destination);

                HexCharacterView view = character.hexCharacterView;

                CoroutineData cData = new CoroutineData();
                VisualEventManager.Instance.CreateVisualEvent(() => DoCharacterMoveVisualEvent
                    (character.hexCharacterView, destination, cData), cData, QueuePosition.Back);
            }       
        }
        #endregion

        // Hex Clicking
        #region
        public void OnHexClicked(LevelNode h)
        {
            if (GameController.Instance.GameState != GameState.CombatActive) return;

            // Clicked on hex while awaiting ability target
            if (AbilityController.Instance.AwaitingAbilityOrder())
            {
                if (TurnController.Instance.EntityActivated == null) return;

                if ( // check valid time to for character to use ability
                    //!EventSystem.current.IsPointerOverGameObject() &&
                    TurnController.Instance.EntityActivated.controller == Controller.Player &&
                    TurnController.Instance.EntityActivated.activationPhase == ActivationPhase.ActivationPhase
                    )
                {
                    // Normal Abilities, or 2 target selection abilities in the start phase
                    if (AbilityController.Instance.CurrentAbilityAwaiting.secondaryTargetRequirement == SecondaryTargetRequirement.None ||
                        (AbilityController.Instance.CurrentAbilityAwaiting.secondaryTargetRequirement != SecondaryTargetRequirement.None && AbilityController.Instance.CurrentSelectionPhase == AbilitySelectionPhase.None))
                    {
                        //if (h.myCharacter == null || (h.myCharacter != null && AbilityController.Instance.CurrentAbilityAwaiting.targetRequirement == TargetRequirement.Hex))
                        //    AbilityController.Instance.HandleTargetSelectionMade(h);

                        if (AbilityController.Instance.CurrentAbilityAwaiting.targetRequirement == TargetRequirement.Hex)
                            AbilityController.Instance.HandleTargetSelectionMade(h);

                        else if (h.myCharacter != null)
                            AbilityController.Instance.HandleTargetSelectionMade(h.myCharacter);
                    }

                    // 2 target selection abilities in the first selection phase
                    else if (AbilityController.Instance.CurrentAbilityAwaiting.secondaryTargetRequirement != SecondaryTargetRequirement.None &&
                        AbilityController.Instance.CurrentSelectionPhase == AbilitySelectionPhase.First)
                    {
                        if(AbilityController.Instance.CurrentAbilityAwaiting.secondaryTargetRequirement == SecondaryTargetRequirement.UnoccupiedHexWithinRangeOfTarget)
                        {
                            AbilityController.Instance.HandleTargetSelectionMade(h);
                        }
                    }

                }
            }

            // Clicked for movement
            else if (!AbilityController.Instance.AwaitingAbilityOrder())
            {
                MoveActionController.Instance.HandleHexClicked(h);
            }
        }
        public void OnHexMouseEnter(LevelNode h)
        {
            if (GameController.Instance.GameState != GameState.CombatActive) return;

            obstructionIndicator.SetActive(false);
            HexMousedOver = h;
            h.mouseOverParent.SetActive(true);

            // Show the world space UI of the character on the tile
            if(h.myCharacter != null && UIController.Instance.CharacterWorldUiState == ShowCharacterWorldUiState.OnMouseOver)
            {
                h.myCharacter.hexCharacterView.mouseOverModel = true;
                HexCharacterController.Instance.FadeInCharacterWorldCanvas(h.myCharacter.hexCharacterView, null, 0.25f);
            }


            // Hex Pop up info
            if(!AbilityController.Instance.AwaitingAbilityOrder() &&
                Pathfinder.CanHexBeOccupied(h) &&
                TurnController.Instance.EntityActivated != null)
                StartCoroutine(ShowTileInfoPopup(h, TurnController.Instance.EntityActivated.currentTile));

            // Hit chance pop up
            else if (AbilityController.Instance.AwaitingAbilityOrder())
            {
                AbilityController.Instance.ShowHitChancePopup(TurnController.Instance.EntityActivated, h.myCharacter, AbilityController.Instance.CurrentAbilityAwaiting);
            }
        }
        public void OnHexMouseExit(LevelNode h)
        {
            if (GameController.Instance.GameState != GameState.CombatActive) return;

            if (HexMousedOver == h)
            {
                // Hide the world space UI of the character on the tile
                if (HexMousedOver.myCharacter != null && UIController.Instance.CharacterWorldUiState == ShowCharacterWorldUiState.OnMouseOver)
                {
                    h.myCharacter.hexCharacterView.mouseOverModel = false;
                    if (h.myCharacter.hexCharacterView.mouseOverModel == false &&
                         h.myCharacter.hexCharacterView.mouseOverWorldUI == false)
                    HexCharacterController.Instance.FadeOutCharacterWorldCanvas(HexMousedOver.myCharacter.hexCharacterView, null, 0.25f, 0.25f);
                }                   

                HexMousedOver = null;
            }
                
            h.mouseOverParent.SetActive(false);
            HideTileInfoPopup();
            AbilityController.Instance.HideHitChancePopup();
        }
        #endregion

        // Tile Marking
        #region
        public void MarkTilesInRange(List<LevelNode> tiles)
        {
            Debug.Log("LevelController.MarkTilesInRange(), marking " + tiles.Count.ToString() + " tiles...");
            foreach (LevelNode h in tiles)
            {
                h.ShowInRangeMarker();
                markedTiles.Add(h);
            }
        }
        public void UnmarkAllTiles()
        {
            Debug.Log("LevelController.UnmarkAllTilesCalled()...");
            foreach(LevelNode h in markedTiles)
            {
                h.HideInRangeMarker();
            }
            markedTiles.Clear();
        }
        #endregion

        // Get Hexs Logic
        #region
        public LevelNode GetHexAtGridPosition(Vector2 gridPos)
        {
            LevelNode ret = null;
            foreach(LevelNode n in allLevelNodes)
            {
                if(n.GridPosition == gridPos)
                {
                    ret = n;
                    break;
                }
            }

            return ret;
        }
        public List<LevelNode> GetAllHexsWithinRange(LevelNode start, int range, bool includeStart = false)
        {
            List<LevelNode> hexsRet = new List<LevelNode>();

            if (range == 1)
                hexsRet = start.NeighbourNodes(AllLevelNodes.ToList());

            else
            {
                foreach (LevelNode h in AllLevelNodes)
                {
                    // check x first
                    if (h != start &&
                        h.Distance(start) <= range)
                    {
                        hexsRet.Add(h);
                    }
                }
            }           

            if (includeStart)
                hexsRet.Add(start);


            Debug.Log("LevelController.GetAllHexsWithinRange() found " + hexsRet.Count.ToString() +" in range of hex " + start.GridPosition.x.ToString() + ", " +
                start.GridPosition.y.ToString());

            return hexsRet;
        }
        public LevelNode GetClosestAvailableHexFromStart(LevelNode start, List<LevelNode> possibleHexs)
        {
            // looks through all the possible hexs, and returns the one that is closest to the start
            LevelNode closestHex = null;
            int bestDistance = 10000;
            foreach (LevelNode h in possibleHexs)
            {
                int distance = h.Distance(start);
                if (distance < bestDistance && Pathfinder.CanHexBeOccupied(h))
                {
                    closestHex = h;
                    bestDistance = distance;
                }
            }

            return closestHex;
        }

        #endregion

        // Tile Info Pop up Logic
        #region
        private IEnumerator ShowTileInfoPopup(LevelNode destination, LevelNode start = null)
        {
            yield return new WaitForSeconds(popupDelay);
            if (HexMousedOver != destination) yield break;

            HexDataSO data = destination.TileData;
            if (!data) yield break;

            tileInfoVisualParent.SetActive(true);
            tileInfoPositionParent.transform.position = destination.WorldPosition;
            tileInfoCg.alpha = 0;
            tileInfoCg.DOFade(1, 0.5f);

            // build views logic
            tileInfoNameText.text = data.tileName;
            tileInfoDescriptionText.text = TextLogic.ConvertCustomStringListToString(data.description);

            // Hide + reset dotted rows
            foreach (ModalDottedRow r in tileEffectDotRows)
                r.gameObject.SetActive(false);

            if (destination.Obstructed)
                tileEffectDotRows[0].Build("This location is obstructed and cannot be moved on or through.", DotStyle.Neutral);            

            else if(start == null)
            {
                tileEffectDotRows[0].Build("Costs " + TextLogic.ReturnColoredText(destination.BaseMoveCost.ToString(), TextLogic.blueNumber) +
                   " " + TextLogic.ReturnColoredText("Energy", TextLogic.neutralYellow) + " to traverse.", DotStyle.Neutral);
            }
            else if (start != null && 
                start.myCharacter != null &&
                start.myCharacter.controller == Controller.Player &&
                start.myCharacter.activationPhase == ActivationPhase.ActivationPhase)
            {
                int energyCostDifference = Pathfinder.GetEnergyCostBetweenHexs(start.myCharacter, start, destination) - destination.BaseMoveCost;
                if (energyCostDifference > 0)
                {
                    tileEffectDotRows[0].Build("Costs " + TextLogic.ReturnColoredText(destination.BaseMoveCost.ToString(), TextLogic.blueNumber) +
                     " + " + TextLogic.ReturnColoredText(energyCostDifference.ToString(), TextLogic.blueNumber) + " " +
                    TextLogic.ReturnColoredText("Energy", TextLogic.neutralYellow) + " to traverse due to elevation difference.", DotStyle.Neutral);
                }
                else
                {
                    tileEffectDotRows[0].Build("Costs " + TextLogic.ReturnColoredText(destination.BaseMoveCost.ToString(), TextLogic.blueNumber) +
                    " " + TextLogic.ReturnColoredText("Energy", TextLogic.neutralYellow) +
                    " to traverse.", DotStyle.Neutral);
                }
          
            }

            // Build effect rows
            for(int i = 0; i < destination.TileData.effectDescriptions.Length; i++)
            {
                tileEffectDotRows[i + 1].Build(destination.TileData.effectDescriptions[i]);
            }

            // Rebuild fitters
            for(int i = 0; i < 2; i++)
            {
                foreach(RectTransform rt in tileInfoPopUpLayoutRebuilds)                
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rt);                
            }
        }
        private void HideTileInfoPopup()
        {
            tileInfoCg.alpha = 1;
            tileInfoVisualParent.SetActive(false);
            tileInfoCg.alpha = 0;
        }
        #endregion

        // Obstruction Logic
        #region
        public LevelNode IsTargetObstructed(HexCharacterModel attacker, HexCharacterModel target)
        {
            // used in range attack accuracy calculations. determines if the LoS
            // to the target is obstructed by a nearby character or obstacle.
            // if it is, this function returns the hex that is obstructing it

            LevelNode hRet = null;

            List<LevelNode> adjacentHexs = GetAllHexsWithinRange(target.currentTile, 1);

            // Return if attacker is adjacent to target (impossible to be obstructed)
            if (adjacentHexs.Contains(attacker.currentTile))
                return null;

            HexDirection directionToAttacker = GetDirectionToTargetHex(target.currentTile, attacker.currentTile);

            foreach(LevelNode h in adjacentHexs)
            {
                if(GetDirectionToTargetHex(target.currentTile, h) == directionToAttacker)
                {
                    if(h.myCharacter != null) //|| h.MyHexObstacle != null)
                    {
                        // Obstruction detected
                        Debug.Log("LevelController.IsTargetObstructed() determined obstructed line of sight from " + attacker.myName + " to " + target.myName);
                        hRet = h;
                        break;
                    }
                }
            }

            return hRet;

        }
        public void ShowObstructionIndicator(Hex target, Hex obstructedTile)
        {
            Vector3 pos = target.WorldPosition + obstructedTile.WorldPosition;
            pos /= 2;

            obstructionIndicator.SetActive(true);
            obstructionIndicator.transform.position = pos;
        }
        #endregion

        // Level Node View Logic
        #region
        public void HideAllNodeViews()
        {
            nodesVisualParent.SetActive(false);
        }
        public void ShowAllNodeViews()
        {
            nodesVisualParent.SetActive(true);
        }
        #endregion

        // Scenery Logic
        #region        

        // Arena
        public void EnableNightTimeArenaScenery()
        {
            DisableAllArenas();
            mainArenaViewParent.SetActive(true);
            EnableRandomNightTimeArena();
        }
        private void EnableRandomNightTimeArena()
        {
            allNightTimeArenaParents[RandomGenerator.NumberBetween(0, allNightTimeArenaParents.Length - 1)].SetActive(true);

        }
        public void EnableDayTimeArenaScenery()
        {
            DisableAllArenas();
            mainArenaViewParent.SetActive(true);
            EnableRandomDayTimeArena();
        }
        private void EnableRandomDayTimeArena()
        {
            allDayTimeArenaParents[RandomGenerator.NumberBetween(0, allDayTimeArenaParents.Length - 1)].SetActive(true);

        }
        public void DisableArenaView()
        {
            DisableAllArenas();
            mainArenaViewParent.SetActive(false);
        }
        private void DisableAllArenas()
        {
            for (int i = 0; i < allNightTimeArenaParents.Length; i++)
            {
                allNightTimeArenaParents[i].SetActive(false);
            }
            for (int i = 0; i < allDayTimeArenaParents.Length; i++)
            {
                allDayTimeArenaParents[i].SetActive(false);
            }
        }
        

        #endregion

    }


}