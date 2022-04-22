using HexGameEngine.Abilities;
using HexGameEngine.Characters;
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
        [HideInInspector] public HexCharacterModel myEntity;
        [HideInInspector] public bool occupied;
        [SerializeField] Vector2 gridPosition;

        [Header("Parent References")]
        public GameObject mouseOverParent;
        [SerializeField] Transform elevationPositioningParent;
        [SerializeField] GameObject elevationPillarImagesParent;

        [Header("Marker Components")]
        [SerializeField] GameObject inRangeMarker;
        [SerializeField] GameObject moveMarker;

        [Header("Obstruction Components")]
        [SerializeField] GameObject obstacleParent;
        [SerializeField] GameObject[] obstacleImages;

        private List<LevelNode> neighbourNodes = null;
        private TileElevation elevation;
        private bool obstructed = false;
        [HideInInspector]public HexCharacterModel myCharacter;
        #endregion

        // Getters + Accessors
        #region
        public bool Obstructed
        {
            get { return obstructed; }
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
        public int BaseMoveCost
        {
            get { return 2; }
        }
        /*
        public int Distance(LevelNode other)
        {
            int yDif = (int) Mathf.Abs(gridPosition.y - other.gridPosition.y);
            int xDif = (int) Mathf.Abs(gridPosition.x - other.gridPosition.x);
            if (xDif > yDif) return xDif;
            else return yDif;
        }
        */
        public int Distance(LevelNode other)
        {
            return (int)(Mathf.Abs(CubeCoord.x - other.CubeCoord.x) + Mathf.Abs(CubeCoord.y - other.CubeCoord.y) +
                Mathf.Abs(CubeCoord.z - other.CubeCoord.z)) / 2;
        }
        #endregion

        // Events
        #region
        private void Start()
        {
            _offsetCoord = gridPosition;
            HexGridType = HexGridType.odd_q;
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
        public void ShowInRangeMarker()
        {
            inRangeMarker.SetActive(true);
        }
        public void HideInRangeMarker()
        {
            inRangeMarker.SetActive(false);
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
            }
        }
        private void DisableObstructionViews()
        {
            foreach (GameObject g in obstacleImages)
                g.SetActive(false);

            obstacleParent.SetActive(false);
        }
        #endregion

        // Misc
        #region
        public string PrintGridPosition()
        {
            return "X:" + GridPosition.x.ToString() + ", Y:" + GridPosition.y.ToString();
        }
        public void Reset()
        {
            DisableObstructionViews();
            obstructed = false;
            SetHexTileElevation(TileElevation.Ground);
            elevationPillarImagesParent.SetActive(false);        
        }
        #endregion
    }

}
