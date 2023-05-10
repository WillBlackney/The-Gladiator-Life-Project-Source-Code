using HexGameEngine.Abilities;
using HexGameEngine.Characters;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TbsFramework.Cells;
using UnityEngine;

namespace HexGameEngine.HexTiles
{
    public class LevelNode : Hexagon
    {
        // Components + Properties
        #region
        [Header("Properties")]
        [Tooltip("If marked false, this node will be removed from view and game play logic. Use this to shrink/expand the size of the play area.")]
        [SerializeField] bool exists = true;
        [SerializeField] Vector2 gridPosition;
        [HideInInspector] public string tileName;

        [Header("Parent References")]
        public GameObject mouseOverParent;
        [SerializeField] Transform elevationPositioningParent;
        [SerializeField] GameObject elevationPillarImagesParent;
        [SerializeField] SpriteRenderer[] elevationPillarCircleRenderers;
        [SerializeField] SpriteRenderer[] elevationPillarSquareRenderers;

        [Header("Marker Components")]
        [SerializeField] GameObject inRangeMarkerNeutral;
        [SerializeField] GameObject inRangeMarkerEnemy;
        [SerializeField] GameObject activationMarker;
        [SerializeField] GameObject moveMarker;

        [Header("Obstruction Components")]
        [SerializeField] GameObject obstacleParent;
        [SerializeField] GameObject[] obstacleImages;        

        [Header("Pillar Sprites")]
        [SerializeField] private Sprite nightTimeCircle;
        [SerializeField] private Sprite nightTimeSquare;
        [Space(10)]
        [SerializeField] private Sprite dayTimeCircle;
        [SerializeField] private Sprite dayTimeSquare;

        [Header("Tile Type Sprites")]
        [SerializeField] GameObject mudParent;
        [SerializeField] GameObject waterParent;
        [SerializeField] GameObject grassParent;

        private List<LevelNode> neighbourNodes = null;
        private TileElevation elevation;
        private bool obstructed = false;
        [HideInInspector] public HexCharacterModel myCharacter;
        private HexDataSO tileData;
        #endregion

        // Getters + Accessors
        #region
        public bool Exists
        {
            get { return exists; }
        }
        public bool Obstructed
        {
            get { return obstructed; }
        }
        public HexDataSO TileData
        {
            get { return tileData; }
        }
        public TileElevation Elevation
        {
            get { return elevation; }
        }
        public Transform ElevationPositioningParent
        {
            get { return elevationPositioningParent; }
        }
        public Vector2 GridPosition
        {
            get { return gridPosition; }
        }
        public Vector3 WorldPosition
        {
            get { return new Vector3(elevationPositioningParent.position.x, elevationPositioningParent.position.y + 0.1f, elevationPositioningParent.position.z); }
        }
        public List<LevelNode> NeighbourNodes(List<LevelNode> otherHexs = null)
        {
            if (neighbourNodes == null)
            {
                if (otherHexs == null) otherHexs = LevelController.Instance.AllLevelNodes.ToList();
                neighbourNodes = new List<LevelNode>();
                foreach (var direction in _directions)
                {
                    Vector2 dir = direction;
                    var neighbour = otherHexs.Find(c => c.OffsetCoord == CubeToOffsetCoords(CubeCoord + direction));
                    //var neighbour = otherHexs.Find(c => c.GridPosition == GridPosition + dir);
                    if (neighbour == null) continue;

                    neighbourNodes.Add(neighbour);
                }
                Debug.Log("NeighbourNodes() total neighbours found on node " + gridPosition.x.ToString() + ", " + gridPosition.y.ToString() + ": " + neighbourNodes.Count);
            }
            return neighbourNodes;
        }
        public int BaseMoveActionPointCost
        {
            get 
            {
                int sum = 2;
                if (tileData != null) sum += tileData.moveCostModifier;
                if (sum < 0) sum = 0;
                return sum; 
            }
        }
        public int BaseMoveFatigueCost
        {
            get
            {
                int sum = 2;
                if (tileData != null) sum += tileData.fatigueCostModifier;
                if(sum < 0) sum = 0;
                return sum;
            }
        }

        public int Distance(LevelNode other)
        {
            return (int)(Mathf.Abs(CubeCoord.x - other.CubeCoord.x) + Mathf.Abs(CubeCoord.y - other.CubeCoord.y) +
                Mathf.Abs(CubeCoord.z - other.CubeCoord.z)) / 2;
        }
        #endregion

        // Events
        #region
        public void BuildFromData(HexDataSO data)
        {
            tileData = data;
            tileName = data.tileName;

            mudParent.SetActive(false);
            grassParent.SetActive(false);
            waterParent.SetActive(false);

            if (tileName == "Grass" && !obstructed) grassParent.SetActive(true);
            else if (tileName == "Mud") mudParent.SetActive(true);
            else if (tileName == "Water") waterParent.SetActive(true);
        }
        private void Start()
        {
            _offsetCoord = gridPosition;
            HexGridType = HexGridType.odd_q;
            if (!Exists) gameObject.SetActive(false);
        }
        private void OnEnable()
        {
            _offsetCoord = gridPosition;
            HexGridType = HexGridType.odd_q;
        }
        #endregion

        // Input 
        #region
        protected override void OnMouseDown()
        {
            if(AbilityButton.CurrentButtonMousedOver == null)
               LevelController.Instance.OnHexClicked(this);
        }
        protected override void OnMouseEnter()
        {
            LevelController.Instance.OnHexMouseEnter(this);
        }
        protected override void OnMouseExit()
        {
            LevelController.Instance.OnHexMouseExit(this);
        }
        #endregion

        // Inherited Logic
        #region
        public override Vector3 GetCellDimensions()
        {
            return new Vector3(1, 1, 0);
        }

        public override void MarkAsHighlighted()
        {
        }

        public override void MarkAsPath()
        {
        }

        public override void MarkAsReachable()
        {
        }

        public override void UnMark()
        {
        }
        #endregion

        // Marking Logic
        #region
        public void ShowMoveMarker()
        {
            Debug.Log("ShowMoveMarker() called...");
            moveMarker.SetActive(true);
        }
        public void HideMoveMarker()
        {
            Debug.Log("HideMoveMarker() called...");
            moveMarker.SetActive(false);
        }
        public void ShowInRangeMarker(bool neutral = true)
        {
            if(neutral) inRangeMarkerNeutral.SetActive(true);
            else inRangeMarkerEnemy.SetActive(true);
        }
        public void HideInRangeMarker()
        {
            inRangeMarkerNeutral.SetActive(false);
            inRangeMarkerEnemy.SetActive(false);
        }
        public void ShowActivationMarker()
        {
            activationMarker.SetActive(true);
            LevelController.Instance.AllLevelNodes.ForEach(x => x.HideActivationMarker());
            CurrentActivationNode = this;
        }
        public void HideActivationMarker()
        {
            activationMarker.SetActive(false);
            CurrentActivationNode = null;
        }

        public static LevelNode CurrentActivationNode
        {
            get; private set;
        }

        #endregion

        // Elevation Logic
        #region
        public void SetHexTileElevation(TileElevation elevation)
        {
            this.elevation = elevation;
            if (elevation == TileElevation.Elevated)
            {
                ElevationPositioningParent.position += new Vector3(0, 0.2f, 0);
                elevationPillarImagesParent.SetActive(true);
            }
               
            else if (elevation == TileElevation.Ground)
            {
                ElevationPositioningParent.localPosition = new Vector3(0, 0f, 0);
                elevationPillarImagesParent.SetActive(false);
            }
               
        }
        public void SetHexObstruction(bool obstructed)
        {
            DisableObstructionViews();
            this.obstructed = obstructed;
            if (obstructed)
            {
                obstacleParent.SetActive(true);
                obstacleImages.GetRandomElement().SetActive(true);
                grassParent.SetActive(false);
            }
        }
        private void DisableObstructionViews()
        {
            foreach (GameObject g in obstacleImages)
                g.SetActive(false);

            obstacleParent.SetActive(false);
        }
        #endregion

        // Elevation Pillar sprite logic
        #region
        public void SetPillarSprites(bool dayTime)
        {
            if (dayTime)
            {
                foreach (SpriteRenderer sr in elevationPillarCircleRenderers)
                    sr.sprite = dayTimeCircle;

                foreach (SpriteRenderer sr in elevationPillarSquareRenderers)
                    sr.sprite = dayTimeSquare;
            }
            else if (!dayTime)
            {
                foreach (SpriteRenderer sr in elevationPillarCircleRenderers)
                    sr.sprite = nightTimeCircle;

                foreach (SpriteRenderer sr in elevationPillarSquareRenderers)
                    sr.sprite = nightTimeSquare;
            }
        }
        #endregion

        // Misc
        #region
        public string PrintGridPosition()
        {
            return "X:" + GridPosition.x.ToString() + ", Y:" + GridPosition.y.ToString();
        }
        public void ResetNode()
        {
            DisableObstructionViews();
            obstructed = false;
            SetHexTileElevation(TileElevation.Ground);
            elevationPillarImagesParent.SetActive(false);
            myCharacter = null;
        }
        #endregion
    }

    public class SerializedCombatMapData
    {
        public List<SerializedLevelNodeData> nodes = new List<SerializedLevelNodeData>();
    }

    public class SerializedLevelNodeData
    {
        [Header("Properties")]
        public string tileName;
        public Vector2 gridPosition;
        public TileElevation elevation;
        private HexDataSO tileData;
        public bool obstructed;

        public HexDataSO TileData
        {
            get 
            { 
                if (tileData == null)
                {
                    tileData = FindMyTileData();
                }
                return tileData;
            }
        }

        private HexDataSO FindMyTileData()
        {
            HexDataSO ret = null;

            foreach(HexDataSO data in LevelController.Instance.AllHexTileData)
            {
                if(data.tileName == tileName)
                {
                    ret = data;
                    break;
                }
            }

            return ret;
        }
        public SerializedLevelNodeData()
        {

        }

        public SerializedLevelNodeData(LevelNode node)
        {
            gridPosition = node.GridPosition;
            elevation = node.Elevation;
            tileData = node.TileData;
            obstructed = node.Obstructed;
            tileName = node.TileData.tileName;
        }
    }

}
