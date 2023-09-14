using System.Collections.Generic;
using TbsFramework.Cells;
using TMPro;
using UnityEngine;
using WeAreGladiators.Characters;

namespace WeAreGladiators.HexTiles
{
    public class Hex : Hexagon
    {

        // Obstacle Logic
        #region

        public void SetMyHexObstacle(HexObstacle o)
        {
            MyHexObstacle = o;
        }

        #endregion
        // Properties + Components
        #region

        [Header("Elevation + Transform Components")]
        [SerializeField]
        private Transform elevationParent;
        [SerializeField] private GameObject grassOverlayParent;

        [Header("Sprite Components")]
        [SerializeField]
        private SpriteRenderer tileSprite;
        [SerializeField] private GameObject inRangeMarker;
        [SerializeField] private GameObject zoneOfControlMarker;
        [SerializeField] private SpriteRenderer[] allRenderers;
        [SerializeField] private GameObject moveMarker;
        [SerializeField] private GameObject mouseOverParent;

        [Header("Coord Components")]
        [SerializeField]
        private GameObject gridPositionVisualParent;
        [SerializeField] private TextMeshProUGUI xText;
        [SerializeField] private TextMeshProUGUI yText;

        [HideInInspector] public TileElevation elevation;
        [HideInInspector] public HexDataSO myHexData;

        private List<Hex> neighbourHexs;
        public HexCharacterModel myCharacter;

        #endregion

        // Getters + Accessors
        #region

        public HexObstacle MyHexObstacle { get; private set; }
        public string PrintGridPosition()
        {
            return "X:" + GridPosition.x + ", Y:" + GridPosition.y;
        }
        public List<Hex> NeighbourHexs(List<Hex> otherHexs = null)
        {
            if (neighbourHexs == null)
            {
                neighbourHexs = new List<Hex>();
                foreach (Vector3 direction in _directions)
                {
                    Hex neighbour = otherHexs.Find(c => c.OffsetCoord == CubeToOffsetCoords(CubeCoord + direction));
                    if (neighbour == null)
                    {
                        continue;
                    }
                    neighbourHexs.Add(neighbour);
                }
                // Debug.Log("Neighbours found = " + myNeighbours.Count);
            }
            return neighbourHexs;
        }
        public int Distance(Hex other)
        {
            return (int) (Mathf.Abs(CubeCoord.x - other.CubeCoord.x) + Mathf.Abs(CubeCoord.y - other.CubeCoord.y) +
                Mathf.Abs(CubeCoord.z - other.CubeCoord.z)) / 2;
        }
        public GameObject GridPositionVisualParent => gridPositionVisualParent;
        public GameObject MouseOverParent => mouseOverParent;
        public TextMeshProUGUI XText => xText;
        public TextMeshProUGUI YText => yText;
        public SpriteRenderer TileSprite => tileSprite;
        public Transform ElevationParent => elevationParent;
        public GameObject GrassOverlayParent => grassOverlayParent;
        public Vector3 WorldPosition => elevationParent.transform.position;
        public Vector2 GridPosition => OffsetCoord;

        #endregion

        // Inherited Logic
        #region

        public override Vector3 GetCellDimensions()
        {
            return new Vector3(5.3f / 4f, 4.6f / 4f, 0f);
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
            foreach (SpriteRenderer sr in allRenderers)
            {
                sr.sortingOrder += Mathf.RoundToInt(elevationParent.position.y * 100f) * -1;
            }
            if (MyHexObstacle != null)
            {
                MyHexObstacle.RandomizeObstacleSprite();
                MyHexObstacle.MySR.sortingOrder += Mathf.RoundToInt(elevationParent.position.y * 100f) * -1;
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
    }

}
