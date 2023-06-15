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
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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
        [SerializeField] Canvas tileInfoRootCanvas;
        [SerializeField] GameObject tileInfoPositionParent;
        [SerializeField] CanvasGroup tileInfoCg;
        [SerializeField] TextMeshProUGUI tileInfoNameText;
        [SerializeField] TextMeshProUGUI tileInfoDescriptionText;
        [SerializeField] ModalDottedRow[] tileEffectDotRows;
        [SerializeField] RectTransform[] tileInfoPopUpLayoutRebuilds;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        private float popupDelay = 0.25f;

        private List<LevelNode> markedTiles = new List<LevelNode>();

        [Header("Arena Scenery References")]
        [SerializeField] private GameObject mainArenaViewParent;
        [SerializeField] private GameObject[] allNightTimeArenaParents;
        [SerializeField] private GameObject[] allDayTimeArenaParents;
        [SerializeField] private CrowdRowAnimator[] crowdRowAnimators;
        [SerializeField] private List<CrowdMember> allCrowdMembers = new List<CrowdMember>();
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        private LevelNode[] runtimeNodes;
        bool hasSetupRuntimeNodes = false;

        #endregion

        // Getters + Accessors
        #region
        public List<CrowdMember> AllCrowdMembers
        {
            get
            {
                return allCrowdMembers;
            }
        }
        public HexDataSO[] AllHexTileData
        {
            get { return allHexTileData; }
        }
        public LevelNode[] AllLevelNodes
        {
            get 
            {
                if (!hasSetupRuntimeNodes)
                {
                    hasSetupRuntimeNodes = true;
                    runtimeNodes = allLevelNodes.Where(node => node.Exists).ToArray();
                }
                return runtimeNodes;

            }
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
        public void HandleTearDownAllCombatViews()
        {
            AbilityController.Instance.HideHitChancePopup();
            EnemyInfoModalController.Instance.HideModal();
            AbilityPopupController.Instance.HidePanel();
            MoveActionController.Instance.HidePathCostPopup();
            DisableAllArenas();
            HideTileInfoPopup();
            HideAllNodeViews();
        }

        public SerializedCombatMapData GenerateLevelNodes()
        {
            SerializedCombatMapData ret = new SerializedCombatMapData();

            // Get seed
            CombatMapSeedDataSO seed = allCombatMapSeeds.GetRandomElement();
            if (GlobalSettings.Instance.GameMode == GameMode.CombatSandbox &&
                GlobalSettings.Instance.SandboxCombatMapSeed != null) seed = GlobalSettings.Instance.SandboxCombatMapSeed;

            if (!seed) return null;

            // Reset type, obstacle and elevation on all nodes
            foreach (LevelNode n in AllLevelNodes)
                n.ResetNode();

            // Get banned nodes for obstacles
            List<LevelNode> spawnPositions = GetPlayerSpawnZone();
            spawnPositions.AddRange(GetEnemySpawnZone());

            int elevations = 0;
            int obstructions = 0;

            // Determine tiling probabilities
            List<TileProbability> tileSpawnData = new List<TileProbability>();
            TileProbability previous = null;
            for(int i = 0; i < seed.tilingConfigs.Length; i++)
            {
                int lower = 0;
                int upper = 0;
                HexMapTilingConfig data = seed.tilingConfigs[i];

                if(previous == null)
                {
                    lower = 1;
                    upper = data.spawnChance;
                }
               
                else if(previous != null)
                {
                    lower = previous.upperLimit + 1;
                    upper = lower + data.spawnChance - 1;
                }

                TileProbability newData = new TileProbability(lower, upper, data.hexData);
                previous = newData;
                tileSpawnData.Add(newData);
            }

            // Rebuild
            foreach (LevelNode n in AllLevelNodes)
            {
                int elevationRoll = RandomGenerator.NumberBetween(1, 100);
                if (elevationRoll >= 1 && elevationRoll <= seed.elevationPercentage &&
                    elevations < seed.maximumElevations)
                {
                    n.SetHexTileElevation(TileElevation.Elevated);
                    elevations += 1;
                }                    

                // Set up tile type + data
                int tileTypeRoll = RandomGenerator.NumberBetween(1, 100);
                HexDataSO randomHexType = seed.defaultTile;
                foreach (TileProbability c in tileSpawnData)
                {
                    if (tileTypeRoll >= c.lowerLimit && 
                        tileTypeRoll <= c.upperLimit &&
                        (n.Elevation == TileElevation.Ground || (n.Elevation == TileElevation.Elevated && c.tileData.allowElevation)))
                    {
                        randomHexType = c.tileData;
                        break;
                    }
                }
                n.BuildFromData(randomHexType);

                // To do: randomize and set obstacle
                int obstructionRoll = RandomGenerator.NumberBetween(1, 100);
                if (obstructions < seed.maximumObstructions &&
                    obstructionRoll >= 1 && obstructionRoll <= seed.obstructionPercentage &&
                    Pathfinder.IsHexSpawnable(n) &&
                    !spawnPositions.Contains(n) &&
                    (n.Elevation == TileElevation.Ground || (seed.allowObstaclesOnElevation && n.Elevation == TileElevation.Elevated)))
                {
                    n.SetHexObstruction(true);
                    obstructions += 1;
                }                    

                SerializedLevelNodeData saveableNode = new SerializedLevelNodeData(n);
                ret.nodes.Add(saveableNode);
            }

            return ret;

        }
        public void GenerateLevelNodes(SerializedCombatMapData data)
        {
            foreach(LevelNode n in AllLevelNodes)
            {
                foreach(SerializedLevelNodeData d in data.nodes)
                {
                    if(n.GridPosition == d.gridPosition)
                    {
                        n.ResetNode();
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
            foreach (LevelNode n in AllLevelNodes)
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
                if(n.GridPosition.x == -3 || n.GridPosition.x == -2)
                {
                    nodes.Add(n);
                }
            }

            nodes.OrderBy(n => n.GridPosition.x);
            return nodes;
        }
        public List<LevelNode> GetSanboxOrderPlayerSpawnZone()
        {
            List<LevelNode> nodes = new List<LevelNode>();
            List<LevelNode> frontRank = new List<LevelNode>();
            List<LevelNode> backRank = new List<LevelNode>();
            List<LevelNode> remaining = new List<LevelNode>();

            foreach (LevelNode n in AllLevelNodes)
            {
                if (n.GridPosition.x == -2 && (n.GridPosition.y == 0 || n.GridPosition.y == 1))
                {
                    frontRank.Add(n);
                }
                else if (n.GridPosition.x == -3 && (n.GridPosition.y == 0 || n.GridPosition.y == 1))
                {
                    backRank.Add(n);
                }
                else remaining.Add(n);
            }
            nodes.AddRange(frontRank);
            nodes.AddRange(backRank);
            nodes.AddRange(remaining);

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
                    VisualEventManager.CreateVisualEvent(()=> character.ucmVisualParent.transform.localScale = new Vector3(scale, Mathf.Abs(scale)));
                }
            }

            else
            {
                if (character.ucmVisualParent != null)
                {
                    VisualEventManager.CreateVisualEvent(() => character.ucmVisualParent.transform.localScale = new Vector3(-scale, Mathf.Abs(scale)));
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
                // Pay energy + fatigue costs
                HexCharacterController.Instance.ModifyActionPoints(character, -Pathfinder.GetActionPointCostOfPath(character, character.currentTile, path.HexsOnPath));
                HexCharacterController.Instance.ModifyCurrentFatigue(character, Pathfinder.GetFatigueCostOfPath(character, character.currentTile, path.HexsOnPath));
            }

            // Play movement animation, hide activation node marker before moving
            VisualEventManager.CreateVisualEvent(() => LevelNode.HideActivationMarkers());
            VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayMoveAnimation(character.hexCharacterView));

            // Move to first hex           
            HandleMoveToHex(character, path.HexsOnPath[0], 5, true, PointOnPath.First);

            if (path.HexsOnPath.Count > 1)
            {
                for (int i = 0; i < path.HexsOnPath.Count - 1; i++)
                {
                    PointOnPath pop = PointOnPath.Middle;
                    if (path.HexsOnPath[i] == path.Destination) pop = PointOnPath.Last;
                    HandleMoveToHex(character, path.HexsOnPath[i + 1], 5, true, pop);
                }
            }

            // Handle stress event: Enemy moved into my melee range + moved into back arc (on destination only)
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

            // Finished movement, go idle animation
            LevelNode hex2 = character.currentTile;
            VisualEventManager.CreateVisualEvent(() =>
            {
                if (hex2 != null) hex2.ShowActivationMarker();
                HexCharacterController.Instance.PlayIdleAnimation(character.hexCharacterView);
            });        

        }
        public void ChargeDownPath(HexCharacterModel character, List<LevelNode> path)
        {
            // Play movement animation
            VisualEventManager.CreateVisualEvent(() => LevelNode.HideActivationMarkers());
            VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayChargeAnimation(character.hexCharacterView));

            // Move to first hex
            HandleMoveToHex(character, path[0], 2.5f, true, PointOnPath.First);

            if (path.Count > 1)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    HandleMoveToHex(character, path[i + 1], 2.5f);
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
            if(character.hexCharacterView != null)
            {
                LevelNode hex2 = character.currentTile;
                if (character.hexCharacterView.CurrentAnimation == AnimationEventController.CHARGE)
                {
                    VisualEventManager.CreateVisualEvent(() =>
                    {
                        if (hex2 != null) hex2.ShowActivationMarker();
                        HexCharacterController.Instance.PlayChargeEndAnimation(character.hexCharacterView);
                    });
                }
                else
                {
                    VisualEventManager.CreateVisualEvent(() =>
                    {
                        if (hex2 != null) hex2.ShowActivationMarker();
                        HexCharacterController.Instance.PlayIdleAnimation(character.hexCharacterView);
                    });
                }                   
            }
        }

        
        private void HandleMoveToHex(HexCharacterModel character, LevelNode destination, float moveSpeed = 5f, bool runAnimation = true, PointOnPath pop = PointOnPath.Middle)
        {
            // Check and resolve free strikes + spear wall attacks before moving
            List<HexCharacterModel> allEnemies = HexCharacterController.Instance.GetAllEnemiesOfCharacter(character);
            List<HexCharacterModel> freeStrikeEnemies = new List<HexCharacterModel>();
            List<HexCharacterModel> spearWallEnemies = new List<HexCharacterModel>();
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
                    VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayIdleAnimation(character.hexCharacterView));
                }

                // Resolve free strike for each character
                foreach (HexCharacterModel c in freeStrikeEnemies)
                {
                    if (character.currentHealth > 0 && character.livingState == LivingState.Alive)
                    {
                        HexCharacterView modelView = c.hexCharacterView;

                        // Start free strike attack
                        AbilityController.Instance.UseAbility(c, AbilityController.Instance.FreeStrikeAbility, character);
                        VisualEventManager.InsertTimeDelayInQueue(1f);
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
                if(runAnimation) VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayMoveAnimation(character.hexCharacterView));
                else VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayChargeAnimation(character.hexCharacterView));
            }

            // Face towards destination hex
            FaceCharacterTowardsHex(character, destination);

            // Lock player to hex if it is unoccupied
            var previousTile = character.currentTile;
            if (destination.myCharacter == null)
                PlaceCharacterOnHex(character, destination);

            // Move animation
            Ease ease = Ease.Linear;

            // uncomment when ready to do dynamic movement easing
            //if (pop == PointOnPath.First) ease = Ease.InQuad;
            //else if (pop == PointOnPath.Last) ease = Ease.OutQuad;

            TaskTracker cData = new TaskTracker();
            VisualEventManager.CreateVisualEvent(() => DoCharacterMoveVisualEventDOTWEEN
                (character.hexCharacterView, destination, cData, moveSpeed, ease)).SetCoroutineData(cData);

            // TO DO: events that trigger when the character steps onto a new tile go here (maybe?)...
            character.tilesMovedThisTurn++;

            // Determine which enemies are valid to take a spear wall attack
            if (!PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Slippery))
            {
                foreach (HexCharacterModel c in allEnemies)
                {
                    List<LevelNode> aTiles = GetAllHexsWithinRange(c.currentTile, 1);
                    if (aTiles.Contains(destination) &&
                        !aTiles.Contains(previousTile) && 
                        HexCharacterController.Instance.IsCharacterAbleToMakeSpearWallAttack(c))
                    {
                        spearWallEnemies.Add(c);
                    }
                }

                if (spearWallEnemies.Count > 0)
                {
                    didPauseMoveAnim = true;
                    VisualEventManager.CreateVisualEvent(() => HexCharacterController.Instance.PlayIdleAnimation(character.hexCharacterView));
                }

                // Resolve spear wall strike for each character
                foreach (HexCharacterModel c in spearWallEnemies)
                {
                    if (character.currentHealth > 0 && character.livingState == LivingState.Alive)
                    {
                        // Start free strike attack
                        AbilityController.Instance.UseAbility(c, AbilityController.Instance.SpearWallStrikeAbility, character);
                        VisualEventManager.InsertTimeDelayInQueue(1f);
                    }
                }
            }

            // Cancel if character was killed from spear wall strikes
            if (character == null ||
                character.currentHealth == 0 ||
                character.livingState == LivingState.Dead)
            {
                return;
            }

            // TO DO: Need to update this as it will remove flight when using an ability like charge or sprint which is not good.
            // Remove flight
            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Flight))
                PerkController.Instance.ModifyPerkOnCharacterEntity(character.pManager, Perk.Flight, -1);

        }
        public void DoCharacterMoveVisualEventDOTWEEN(HexCharacterView view, LevelNode hex, TaskTracker cData, float moveSpeed = 5f, Ease ease = Ease.Linear, Action onCompleteCallback = null)
        {
            // Set up
            Vector3 destination = new Vector3(hex.WorldPosition.x, hex.WorldPosition.y, 0);
            float finalMoveSpeed = moveSpeed * 0.1f;
            view.mainMovementParent.transform.DOMove(destination, finalMoveSpeed).SetEase(ease).OnComplete(() =>
            {
                // Resolve event
                if (cData != null) cData.MarkAsCompleted();
                if (onCompleteCallback != null) onCompleteCallback.Invoke();
                
            });            
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
            VisualEventManager.CreateVisualEvent(() => {
                VisualEffectManager.Instance.CreateTeleportEffect(view.WorldPosition);
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(view, null, 0.2f);
                HexCharacterController.Instance.FadeOutCharacterModel(view.ucm, 0.2f);
                HexCharacterController.Instance.FadeOutCharacterShadow(view, 0.2f);
            });
          
            VisualEventManager.InsertTimeDelayInQueue(0.2f);
            VisualEventManager.CreateVisualEvent(() => SnapCharacterViewToHex(view, destination));
            // Create teleport VFX at character's destination

            // Make character model + world space UI reappear
            bool updateActivationHex = TurnController.Instance.EntityActivated == character;
            VisualEventManager.CreateVisualEvent(() => {
                VisualEffectManager.Instance.CreateTeleportEffect(view.WorldPosition);
                HexCharacterController.Instance.FadeInCharacterWorldCanvas(view, null, 0.2f);
                HexCharacterController.Instance.FadeInCharacterModel(view.ucm, 0.2f);
                HexCharacterController.Instance.FadeInCharacterShadow(view, 0.2f);
                if (updateActivationHex) character.currentTile.ShowActivationMarker();
            });

            // Handle stress event: Enemy moved into my melee range + moved into back arc (on destination only)
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
                VisualEventManager.CreateVisualEvent(() => {
                    VisualEffectManager.Instance.CreateTeleportEffect(viewA.WorldPosition);
                    HexCharacterController.Instance.FadeOutCharacterWorldCanvas(viewA, null, 0.2f);
                    HexCharacterController.Instance.FadeOutCharacterModel(viewA.ucm, 0.2f);
                    HexCharacterController.Instance.FadeOutCharacterShadow(viewA, 0.2f);
                });

                // B
                VisualEventManager.CreateVisualEvent(() => {
                    VisualEffectManager.Instance.CreateTeleportEffect(viewB.WorldPosition);
                    HexCharacterController.Instance.FadeOutCharacterWorldCanvas(viewB, null, 0.2f);
                    HexCharacterController.Instance.FadeOutCharacterModel(viewB.ucm, 0.2f);
                    HexCharacterController.Instance.FadeOutCharacterShadow(viewB, 0.2f);
                });

                // Brief Delay
                VisualEventManager.InsertTimeDelayInQueue(0.2f);

                // Snap characters to new positions visually
                VisualEventManager.CreateVisualEvent(() => SnapCharacterViewToHex(viewA, aDestination));
                VisualEventManager.CreateVisualEvent(() => SnapCharacterViewToHex(viewB, bDestination));
                LevelNode activationHex = TurnController.Instance.EntityActivated.currentTile;
                VisualEventManager.CreateVisualEvent(() => activationHex.ShowActivationMarker());

                // Create teleport VFX at character's destination, fade back in views
                VisualEventManager.CreateVisualEvent(() => {
                    VisualEffectManager.Instance.CreateTeleportEffect(viewA.WorldPosition);
                    HexCharacterController.Instance.FadeInCharacterWorldCanvas(viewA, null, 0.2f);
                    HexCharacterController.Instance.FadeInCharacterModel(viewA.ucm, 0.2f);
                    HexCharacterController.Instance.FadeInCharacterShadow(viewA, 0.2f);
                });
                VisualEventManager.CreateVisualEvent(() => {
                    VisualEffectManager.Instance.CreateTeleportEffect(viewB.WorldPosition);
                    HexCharacterController.Instance.FadeInCharacterWorldCanvas(viewB, null, 0.2f);
                    HexCharacterController.Instance.FadeInCharacterModel(viewB.ucm, 0.2f);
                    HexCharacterController.Instance.FadeInCharacterShadow(viewB, 0.2f);
                });
            }
            else
            {
                VisualEventManager.CreateVisualEvent(() => DoCharacterMoveVisualEventDOTWEEN
                (viewA, aDestination, null));

                VisualEventManager.CreateVisualEvent(() => DoCharacterMoveVisualEventDOTWEEN
                (viewB, bDestination, null));

                VisualEventManager.InsertTimeDelayInQueue(0.25f);

                LevelNode activationHex = TurnController.Instance.EntityActivated.currentTile;
                VisualEventManager.CreateVisualEvent(() => activationHex.ShowActivationMarker());
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
                
                TaskTracker cData = new TaskTracker();
                VisualEventManager.CreateVisualEvent(() => 
                {
                    HexCharacterController.Instance.PlayHurtAnimation(character.hexCharacterView);
                    DoCharacterMoveVisualEventDOTWEEN(character.hexCharacterView, destination, cData, 5f, Ease.OutBack);
                }).SetCoroutineData(cData);
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

                // check valid time to for character to use ability
                if (TurnController.Instance.EntityActivated.controller == Controller.Player &&
                    TurnController.Instance.EntityActivated.activationPhase == ActivationPhase.ActivationPhase)
                {

                    // Normal Abilities, or 2 target selection abilities in the start phase
                    if (AbilityController.Instance.CurrentAbilityAwaiting.secondaryTargetRequirement == SecondaryTargetRequirement.None ||
                        (AbilityController.Instance.CurrentAbilityAwaiting.secondaryTargetRequirement != SecondaryTargetRequirement.None && AbilityController.Instance.CurrentSelectionPhase == AbilitySelectionPhase.None))
                    {
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

            HexMousedOver = h;
            if(!h.Obstructed) h.mouseOverParent.SetActive(true);

            // Glow activation window
            if (h.myCharacter != null && h.myCharacter.hexCharacterView != null &&
                h.myCharacter.hexCharacterView.myActivationWindow != null) h.myCharacter.hexCharacterView.myActivationWindow.MouseEnter();

            // Show the world space UI of the character on the tile
            if (h.myCharacter != null)
            {
                h.myCharacter.hexCharacterView.armourTextWorld.gameObject.SetActive(true);
                h.myCharacter.hexCharacterView.healthTextWorld.gameObject.SetActive(true);
                h.myCharacter.hexCharacterView.mouseOverModel = true;
                if(UIController.Instance.CharacterWorldUiState == ShowCharacterWorldUiState.OnMouseOver)
                    HexCharacterController.Instance.FadeInCharacterWorldCanvas(h.myCharacter.hexCharacterView, null, 0.25f);
            }


            // Tile modal
            if(h.myCharacter == null &&
                !AbilityController.Instance.AwaitingAbilityOrder() &&
                TurnController.Instance.EntityActivated != null)
                StartCoroutine(ShowTileInfoPopup(h, TurnController.Instance.EntityActivated.currentTile));

            // Hit chance modal
            else if (AbilityController.Instance.AwaitingAbilityOrder())
            {
                AbilityController.Instance.ShowHitChancePopup(TurnController.Instance.EntityActivated, h.myCharacter, AbilityController.Instance.CurrentAbilityAwaiting, TurnController.Instance.EntityActivated.itemSet.mainHandItem);
                HandleSubTargettingOnTileMouseEnter(AbilityController.Instance.CurrentAbilityAwaiting, TurnController.Instance.EntityActivated, h);
            }

            // Character info modal
            else if(h.myCharacter != null)
            {
                EnemyInfoModalController.Instance.BuildAndShowModal(h.myCharacter);
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
                    h.myCharacter.hexCharacterView.armourTextWorld.gameObject.SetActive(false);
                    h.myCharacter.hexCharacterView.healthTextWorld.gameObject.SetActive(false);
                    h.myCharacter.hexCharacterView.mouseOverModel = false;
                    if (h.myCharacter.hexCharacterView.mouseOverModel == false &&
                         h.myCharacter.hexCharacterView.mouseOverWorldUI == false)
                    HexCharacterController.Instance.FadeOutCharacterWorldCanvas(HexMousedOver.myCharacter.hexCharacterView, null, 0.25f, 0.001f);
                }                   

                HexMousedOver = null;
            }
                
            h.mouseOverParent.SetActive(false);
            if (h.myCharacter != null && h.myCharacter.hexCharacterView != null &&
               h.myCharacter.hexCharacterView.myActivationWindow != null) h.myCharacter.hexCharacterView.myActivationWindow.MouseExit();

            HideTileInfoPopup();
            UnmarkAllSubTargetMarkers();
            AbilityController.Instance.HideHitChancePopup();         
            EnemyInfoModalController.Instance.HideModal();
        }
        #endregion

        // Tile Marking
        #region
        private void HandleSubTargettingOnTileMouseEnter(AbilityData ability, HexCharacterModel caster, LevelNode mousedOverTile)
        {
            if(ability.targetRequirement == TargetRequirement.Enemy && mousedOverTile && mousedOverTile.myCharacter != null)
            {
                mousedOverTile.ShowSubTargettingMarker(LevelNodeColor.Red);
            }
        }
        public void UnmarkAllSubTargetMarkers()
        {
            AllLevelNodes.ForEach(x => x.HideSubTargettingMarker());
        }
        public void MarkTilesInRange(List<LevelNode> tiles, bool neutral = true)
        {
            Debug.Log("LevelController.MarkTilesInRange(), marking " + tiles.Count.ToString() + " tiles...");
            foreach (LevelNode h in tiles)
            {
                if(h.Obstructed == false)
                {
                    h.ShowInRangeMarker(neutral);
                    markedTiles.Add(h);
                }                
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
            foreach(LevelNode n in AllLevelNodes)
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
            if (GameController.Instance.GameState != GameState.CombatActive) yield break;

            yield return new WaitForSeconds(popupDelay);
            if (HexMousedOver != destination) yield break;

            HexDataSO data = destination.TileData;
            if (!data) yield break;

            tileInfoRootCanvas.enabled = true;
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
                tileEffectDotRows[0].Build("This location is obstructed and cannot be moved on or through.", DotStyle.Red);            

            else if(start == null)
            {
                tileEffectDotRows[0].Build("Costs " + TextLogic.ReturnColoredText(destination.BaseMoveActionPointCost.ToString(), TextLogic.blueNumber) +
                   " " + TextLogic.ReturnColoredText("Action Points", TextLogic.neutralYellow) + " and " +
                    TextLogic.ReturnColoredText(destination.BaseMoveFatigueCost.ToString(), TextLogic.blueNumber) +
                    " " + TextLogic.ReturnColoredText("Fatigue", TextLogic.neutralYellow) +
                    " to traverse.", DotStyle.Neutral);
            }
            else if (start != null && 
                start.myCharacter != null &&
                start.myCharacter.controller == Controller.Player &&
                start.myCharacter.activationPhase == ActivationPhase.ActivationPhase)
            {
                int apCostDifference = Pathfinder.GetActionPointCostBetweenHexs(start.myCharacter, start, destination) - destination.BaseMoveActionPointCost;
                int fatigueCostDifference = Pathfinder.GetFatigueCostBetweenHexs(start.myCharacter, start, destination) - destination.BaseMoveFatigueCost;
                if (apCostDifference > 0)
                {
                    tileEffectDotRows[0].Build("Costs " + TextLogic.ReturnColoredText(destination.BaseMoveActionPointCost.ToString(), TextLogic.blueNumber) +
                     " + " + TextLogic.ReturnColoredText(apCostDifference.ToString(), TextLogic.blueNumber) + " " +
                    TextLogic.ReturnColoredText("Action Points", TextLogic.neutralYellow) + " and " +
                    TextLogic.ReturnColoredText(destination.BaseMoveFatigueCost.ToString(), TextLogic.blueNumber) +
                     " + " + TextLogic.ReturnColoredText(fatigueCostDifference.ToString(), TextLogic.blueNumber) +
                     TextLogic.ReturnColoredText(" Fatigue", TextLogic.neutralYellow) +
                    " to traverse due to elevation difference.", DotStyle.Neutral);
                }
                else
                {
                    tileEffectDotRows[0].Build("Costs " + TextLogic.ReturnColoredText(destination.BaseMoveActionPointCost.ToString(), TextLogic.blueNumber) +
                    " " + TextLogic.ReturnColoredText("Action Points", TextLogic.neutralYellow) + " and " +
                    TextLogic.ReturnColoredText(destination.BaseMoveFatigueCost.ToString(), TextLogic.blueNumber) +
                    " " + TextLogic.ReturnColoredText("Fatigue", TextLogic.neutralYellow) +
                    " to traverse.", DotStyle.Neutral);
                }
          
            }

            // to do: add dotted rows for elevated tile (+10 accuracy and dodge ahainst unelevated enemies, +1 range)

            int extraDotRows = 0;

            if(destination.Elevation == TileElevation.Elevated && !destination.Obstructed)
            {
                tileEffectDotRows[1].Build(TextLogic.ReturnColoredText("+10 ", TextLogic.blueNumber) + TextLogic.ReturnColoredText("Accuracy ", TextLogic.neutralYellow) +
                    " and " + TextLogic.ReturnColoredText("+10 ", TextLogic.blueNumber) + TextLogic.ReturnColoredText("Dodge ", TextLogic.neutralYellow) +
                    " against non elevated enemies.", DotStyle.Green);
                tileEffectDotRows[2].Build(TextLogic.ReturnColoredText("+1 ", TextLogic.blueNumber) + TextLogic.ReturnColoredText("Vision", TextLogic.neutralYellow) +
                   ".", DotStyle.Green);
                extraDotRows = 2;
            }

            // Build effect rows
            for(int i = 0; i < destination.TileData.effectDescriptions.Length; i++)
            {
                tileEffectDotRows[i + 1 + extraDotRows].Build(destination.TileData.effectDescriptions[i]);
            }

            TransformUtils.RebuildLayouts(tileInfoPopUpLayoutRebuilds);
        }
        public void HideTileInfoPopup()
        {
            tileInfoCg.alpha = 1;
            tileInfoRootCanvas.enabled = false;
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
        public void EnableDayTimeArenaScenery()
        {
            DisableAllArenas();
            mainArenaViewParent.SetActive(true);
            EnableRandomDayTimeArena();
            PlayCrowdAnims();
        }
        private void EnableRandomDayTimeArena()
        {
            allDayTimeArenaParents[RandomGenerator.NumberBetween(0, allDayTimeArenaParents.Length - 1)].SetActive(true);
        }
        public void DisableArenaView()
        {
            DisableAllArenas();
            StopCrowdAnims();
            mainArenaViewParent.SetActive(false);
        }
        private void DisableAllArenas()
        {
            for (int i = 0; i < allDayTimeArenaParents.Length; i++)            
                allDayTimeArenaParents[i].SetActive(false);            
        }


        #endregion

        #region Crowd Animation Logic
        private void PlayCrowdAnims()
        {
            foreach (CrowdRowAnimator cra in crowdRowAnimators)
                cra.PlayAnimation();
        }
        private void StopCrowdAnims()
        {
            foreach (CrowdRowAnimator cra in crowdRowAnimators)
                cra.StopAnimation();
        }
        public void AnimateCrowdOnHit()
        {                   
            int minAnims = (int)(AllCrowdMembers.Count * 0.2f);
            int maxAnims = (int)(AllCrowdMembers.Count * 0.3f);
            int totalAnims = RandomGenerator.NumberBetween(minAnims, maxAnims);
            List<CrowdMember> animatedMembers = AllCrowdMembers.GetRandomElements(totalAnims);
            for (int i = 0; i < totalAnims; i++) animatedMembers[i].DoCheerAnimation();
        }
        public void AnimateCrowdOnMiss()
        {
            int minAnims = (int)(AllCrowdMembers.Count * 0.2f);
            int maxAnims = (int)(AllCrowdMembers.Count * 0.3f);
            int totalAnims = RandomGenerator.NumberBetween(minAnims, maxAnims);
            List<CrowdMember> animatedMembers = AllCrowdMembers.GetRandomElements(totalAnims);
            for (int i = 0; i < totalAnims; i++) animatedMembers[i].DoDissapointedAnimation();
        }
        public void AnimateCrowdOnCombatVictory()
        {
            int totalAnims = (int)(AllCrowdMembers.Count * 0.7f);
            List<CrowdMember> animatedMembers = AllCrowdMembers.GetRandomElements(totalAnims);
            for (int i = 0; i < totalAnims; i++) animatedMembers[i].DoCombatFinishedAnimation();
        }
        public void StopAllCrowdMembers(bool reset = false)
        {
            for (int i = 0; i < AllCrowdMembers.Count; i++)
            {
                CrowdMember x = AllCrowdMembers[i];
                x.ResetSelf();
                if (reset == true) x.StartSelfMove();
            }
        }
        #endregion

    }

    class TileProbability
    {
        public HexDataSO tileData;
        public int lowerLimit;
        public int upperLimit;

        public TileProbability(int lowerLimit, int upperLimit, HexDataSO tileData)
        {
            this.lowerLimit = lowerLimit;
            this.upperLimit = upperLimit;
            this.tileData = tileData;
        }
    }


}