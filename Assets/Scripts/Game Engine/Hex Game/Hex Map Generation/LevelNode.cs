using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.Utilities;
using TbsFramework.Cells;
using UnityEngine;
using WeAreGladiators.Abilities;
using WeAreGladiators.Characters;

namespace WeAreGladiators.HexTiles
{
    public class LevelNode : Hexagon
    {

        // Elevation Pillar sprite logic
        #region

        public void SetPillarSprites(bool dayTime)
        {
            if (dayTime)
            {
                foreach (SpriteRenderer sr in elevationPillarCircleRenderers)
                {
                    sr.sprite = dayTimeCircle;
                }

                foreach (SpriteRenderer sr in elevationPillarSquareRenderers)
                {
                    sr.sprite = dayTimeSquare;
                }
            }
            else if (!dayTime)
            {
                foreach (SpriteRenderer sr in elevationPillarCircleRenderers)
                {
                    sr.sprite = nightTimeCircle;
                }

                foreach (SpriteRenderer sr in elevationPillarSquareRenderers)
                {
                    sr.sprite = nightTimeSquare;
                }
            }
        }

        #endregion
        // Components + Properties
        #region

        [Header("Properties")]
        [Tooltip("If marked false, this node will be removed from view and game play logic. Use this to shrink/expand the size of the play area.")]
        [SerializeField]
        private bool exists = true;
        [SerializeField] private Vector2 gridPosition;
        [HideInInspector] public string tileName;

        [Header("Parent References")]
        public GameObject mouseOverParent;
        [SerializeField] private Transform elevationPositioningParent;
        [SerializeField] private GameObject elevationPillarImagesParent;
        [SerializeField] private SpriteRenderer[] elevationPillarCircleRenderers;
        [SerializeField] private SpriteRenderer[] elevationPillarSquareRenderers;

        [Header("Marker Components")]
        [SerializeField]
        private GameObject inRangeMarkerNeutral;
        [SerializeField] private GameObject inRangeMarkerEnemy;
        [SerializeField] private ParticleSystem activationMarker;
        [SerializeField] private GameObject moveMarker;

        [Header("Obstruction Components")]
        [SerializeField] private GameObject nodeBaseRing;
        [SerializeField] private GameObject obstacleParent;
        [SerializeField] private GameObject[] obstacleImages;

        [Header("Pillar Sprites")]
        [SerializeField] private Sprite nightTimeCircle;
        [SerializeField] private Sprite nightTimeSquare;
        [Space(10)]
        [SerializeField] private Sprite dayTimeCircle;
        [SerializeField] private Sprite dayTimeSquare;

        [Header("Tile Type Sprites")]
        [SerializeField]
        private GameObject mudParent;
        [SerializeField] private GameObject waterParent;
        [SerializeField] private GameObject grassParent;

        [Header("Sub Target Marker Components")]
        [SerializeField]
        private GameObject subTargettingMarker;
        [SerializeField] private Transform subTargettingMarkerFrameParent;
        [SerializeField] private SpriteRenderer[] subTargettingMarkerFrameSprites;
        [SerializeField] private Color subTargetWhite;
        [SerializeField] private Color subTargetRed;

        private List<LevelNode> neighbourNodes;
        [HideInInspector] public HexCharacterModel myCharacter;

        #endregion

        // Getters + Accessors
        #region

        public bool Exists => exists;
        public bool Obstructed { get; private set; }
        public HexDataSO TileData { get; private set; }
        public TileElevation Elevation { get; private set; }
        public Transform ElevationPositioningParent => elevationPositioningParent;
        public Vector2 GridPosition => gridPosition;
        public Vector3 WorldPosition => new Vector3(elevationPositioningParent.position.x, elevationPositioningParent.position.y + 0.1f, elevationPositioningParent.position.z);
        public List<LevelNode> NeighbourNodes(List<LevelNode> otherHexs = null)
        {
            if (neighbourNodes == null)
            {
                if (otherHexs == null)
                {
                    otherHexs = LevelController.Instance.AllLevelNodes.ToList();
                }
                neighbourNodes = new List<LevelNode>();
                foreach (Vector3 direction in _directions)
                {
                    Vector2 dir = direction;
                    LevelNode neighbour = otherHexs.Find(c => c.OffsetCoord == CubeToOffsetCoords(CubeCoord + direction));
                    //var neighbour = otherHexs.Find(c => c.GridPosition == GridPosition + dir);
                    if (neighbour == null)
                    {
                        continue;
                    }

                    neighbourNodes.Add(neighbour);
                }
                Debug.Log("NeighbourNodes() total neighbours found on node " + gridPosition.x + ", " + gridPosition.y + ": " + neighbourNodes.Count);
            }
            return neighbourNodes;
        }
        public int BaseMoveActionPointCost
        {
            get
            {
                int sum = 2;
                if (TileData != null)
                {
                    sum += TileData.moveCostModifier;
                }
                if (sum < 0)
                {
                    sum = 0;
                }
                return sum;
            }
        }
        public int BaseMoveFatigueCost
        {
            get
            {
                int sum = 2;
                //if (tileData != null) sum += tileData.fatigueCostModifier;
                if (sum < 0)
                {
                    sum = 0;
                }
                return sum;
            }
        }

        public int Distance(LevelNode other)
        {
            return (int) (Mathf.Abs(CubeCoord.x - other.CubeCoord.x) + Mathf.Abs(CubeCoord.y - other.CubeCoord.y) +
                Mathf.Abs(CubeCoord.z - other.CubeCoord.z)) / 2;
        }

        #endregion

        // Events
        #region

        public void BuildFromData(HexDataSO data)
        {
            TileData = data;
            tileName = data.tileName;

            mudParent.SetActive(false);
            grassParent.SetActive(false);
            waterParent.SetActive(false);

            if (tileName == "Grass" && !Obstructed)
            {
                grassParent.SetActive(true);
            }
            else if (tileName == "Mud")
            {
                mudParent.SetActive(true);
            }
            else if (tileName == "Water")
            {
                waterParent.SetActive(true);
            }
        }
        private void Start()
        {
            _offsetCoord = gridPosition;
            HexGridType = HexGridType.odd_q;
            if (!Exists)
            {
                gameObject.SetActive(false);
            }
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
            if (AbilityButton.CurrentButtonMousedOver == null)
            {
                LevelController.Instance.OnHexClicked(this);
            }
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
            if (neutral)
            {
                inRangeMarkerNeutral.SetActive(true);
            }
            else
            {
                inRangeMarkerEnemy.SetActive(true);
            }
        }
        public void HideInRangeMarker()
        {
            inRangeMarkerNeutral.SetActive(false);
            inRangeMarkerEnemy.SetActive(false);
        }
        public void ShowActivationMarker()
        {
            HideActivationMarkers();
            activationMarker.Play();
            CurrentActivationNode = this;
        }
        public static void HideActivationMarkers()
        {
            LevelController.Instance.AllLevelNodes.ForEach(x => x.activationMarker.Stop());
            CurrentActivationNode = null;
        }

        public static LevelNode CurrentActivationNode
        {
            get;
            private set;
        }

        #endregion

        // Elevation Logic
        #region

        public void SetHexTileElevation(TileElevation elevation)
        {
            Elevation = elevation;
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
            Obstructed = obstructed;
            nodeBaseRing.SetActive(true);
            if (obstructed)
            {
                nodeBaseRing.SetActive(false);
                obstacleParent.SetActive(true);
                obstacleImages.GetRandomElement().SetActive(true);
                grassParent.SetActive(false);
            }
        }
        private void DisableObstructionViews()
        {
            foreach (GameObject g in obstacleImages)
            {
                g.SetActive(false);
            }

            obstacleParent.SetActive(false);
            nodeBaseRing.SetActive(true);
        }

        #endregion

        // Sub Targetting Logic
        #region

        public void ShowSubTargettingMarker(LevelNodeColor frameColor)
        {
            subTargettingMarker.SetActive(true);
            subTargettingMarkerFrameParent.DOKill();
            subTargettingMarkerFrameParent.DOScale(1, 0);
            subTargettingMarkerFrameParent.DOScale(1.2f, 0.25f).SetLoops(-1, LoopType.Yoyo);
            subTargettingMarkerFrameSprites.ForEach(x =>
            {
                x.color = GetSubTargetColor(frameColor);
                x.DOKill();
                x.DOFade(0.6f, 0f);
                x.DOFade(1f, 0.25f).SetLoops(-1, LoopType.Yoyo);
            });
        }
        public void HideSubTargettingMarker()
        {
            subTargettingMarker.SetActive(false);
            subTargettingMarkerFrameParent.DOKill();
            subTargettingMarkerFrameParent.DOScale(1, 0);
            subTargettingMarkerFrameSprites.ForEach(x =>
            {
                x.DOKill();
                x.DOFade(0.5f, 0f);
            });
        }
        private Color GetSubTargetColor(LevelNodeColor c)
        {
            if (c == LevelNodeColor.Red)
            {
                return subTargetRed;
            }
            return subTargetWhite;
        }

        #endregion

        // Misc
        #region

        public string PrintGridPosition()
        {
            return "X:" + GridPosition.x + ", Y:" + GridPosition.y;
        }
        public void ResetNode()
        {
            DisableObstructionViews();
            Obstructed = false;
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
        public TileElevation elevation;
        public Vector2 gridPosition;
        public bool obstructed;
        private HexDataSO tileData;
        [Header("Properties")]
        public string tileName;
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

            foreach (HexDataSO data in LevelController.Instance.AllHexTileData)
            {
                if (data.tileName == tileName)
                {
                    ret = data;
                    break;
                }
            }

            return ret;
        }
    }

    public enum LevelNodeColor
    {
        White = 0,
        Red = 1
    }

}
