using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TbsFramework.Cells;
using TbsFramework.Pathfinding.DataStructs;
using TMPro;
using HexGameEngine.Characters;
using HexGameEngine.Abilities;

namespace HexGameEngine.HexTiles
{   
    public class Hex : Hexagon
    {
        // Properties + Components
        #region
        [Header("Elevation + Transform Components")]
        [SerializeField] Transform elevationParent;
        [SerializeField] GameObject grassOverlayParent;

        [Header("Sprite Components")]
        [SerializeField] SpriteRenderer tileSprite;
        [SerializeField] GameObject inRangeMarker;
        [SerializeField] GameObject zoneOfControlMarker;
        [SerializeField] SpriteRenderer[] allRenderers;
        [SerializeField] GameObject moveMarker;
        [SerializeField] GameObject mouseOverParent;

        [Header("Coord Components")]
        [SerializeField] GameObject gridPositionVisualParent;
        [SerializeField] TextMeshProUGUI xText;
        [SerializeField] TextMeshProUGUI yText;

        [HideInInspector] public TileElevation elevation;
        [HideInInspector] public HexDataSO myHexData;

        private List<Hex> neighbourHexs = null;
        public HexCharacterModel myCharacter;
        private HexObstacle myHexObstacle;
        #endregion        

        // Getters + Accessors
        #region
        public HexObstacle MyHexObstacle
        {
            get { return myHexObstacle; }
        }
        public string PrintGridPosition()
        {
            return "X:" + GridPosition.x.ToString() + ", Y:" + GridPosition.y.ToString();
        }
        public List<Hex> NeighbourHexs(List<Hex> otherHexs = null)
        {
            if (neighbourHexs == null)
            {
                neighbourHexs = new List<Hex>();
                foreach (var direction in _directions)
                {
                    var neighbour = otherHexs.Find(c => c.OffsetCoord == CubeToOffsetCoords(CubeCoord + direction));
                    if (neighbour == null) continue;
                    neighbourHexs.Add(neighbour);
                }
               // Debug.Log("Neighbours found = " + myNeighbours.Count);
            }
            return neighbourHexs;
        }
        public int Distance(Hex other)
        {
            return (int)(Mathf.Abs(CubeCoord.x - other.CubeCoord.x) + Mathf.Abs(CubeCoord.y - other.CubeCoord.y) +
                Mathf.Abs(CubeCoord.z - other.CubeCoord.z)) / 2;
        }
        public GameObject GridPositionVisualParent
        {
            get { return gridPositionVisualParent; }
        }
        public GameObject MouseOverParent
        {
            get { return mouseOverParent; }
        }
        public TextMeshProUGUI XText
        {
            get { return xText; }
        }
        public TextMeshProUGUI YText
        {
            get { return yText; }
        }
        public SpriteRenderer TileSprite
        {
            get { return tileSprite; }
        }
        public Transform ElevationParent
        {
            get { return elevationParent; }
        }
        public GameObject GrassOverlayParent
        {
            get { return grassOverlayParent; }
        }
        public Vector3 WorldPosition
        {
            get { return elevationParent.transform.position; }
        }
        public Vector2 GridPosition
        {
            get { return OffsetCoord; }
        }        
        #endregion

        // Inherited Logic
        #region
        public override Vector3 GetCellDimensions()
        {
            return new Vector3(5.3f / 4f, 4.6f / 4f, 0f );
        }
        public override void MarkAsReachable()
        {
            SetColor(new Color(1, 0.92f, 0.016f, 1));
        }
        public override void MarkAsPath()
        {
            SetColor(new Color(0, 1, 0, 1));
        }
        public override void MarkAsHighlighted()
        {
            SetColor(new Color(0.5f, 0.5f, 0.5f, 0.25f));
        }
        public override void UnMark()
        {
            SetColor(new Color(1, 1, 1, 0));
        }

        #endregion

        // Colour + Sort Order Logic
        #region
        private void SetColor(Color color)
        {
        }
        public void AutoSetSortingOrder()
        {
            foreach(SpriteRenderer sr in allRenderers)
            {
                sr.sortingOrder += Mathf.RoundToInt(elevationParent.position.y * 100f) * -1;
            }
            if(myHexObstacle != null)
            {
                myHexObstacle.RandomizeObstacleSprite();
                myHexObstacle.MySR.sortingOrder += Mathf.RoundToInt(elevationParent.position.y * 100f) * -1;
            }
        }
        #endregion

        // Input 
        #region
        protected override void OnMouseDown()
        {
           // if(AbilityButton.CurrentButtonMousedOver == null)
            //    LevelController.Instance.OnHexClicked(this);
        }
        protected override void OnMouseEnter()
        {
           // LevelController.Instance.OnHexMouseEnter(this);
        }
        protected override void OnMouseExit()
        {
            //LevelController.Instance.OnHexMouseExit(this);
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
        public void ShowZoneOfControlMarker()
        {
            zoneOfControlMarker.SetActive(true);
        }
        public void HideZoneOfControlMarker()
        {
            zoneOfControlMarker.SetActive(false);
        }
        #endregion

        // Obstacle Logic
        #region
        public void SetMyHexObstacle(HexObstacle o)
        {
            myHexObstacle = o;
        }
        #endregion
    }



}