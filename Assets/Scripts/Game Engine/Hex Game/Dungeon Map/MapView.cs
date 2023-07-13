﻿using DG.Tweening;
using WeAreGladiators.CameraSystems;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WeAreGladiators.DungeonMap
{
    public class MapView : Singleton<MapView>
    {     
        [Header("Scene Object References References")]
        [SerializeField] private MapManager mapManager;
        [SerializeField] private GameObject masterMapParent;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Assets")]
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private List<DungeonMapSeed> allMapConfigs;       
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Orientation Settings")]
        [SerializeField] private MapOrientation orientation;
        [Tooltip("Offset of the start/end nodes of the map from the edges of the screen")]
        [SerializeField] private float orientationOffset;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Background Settings")]
        [Tooltip("If the background sprite is null, background will not be shown")]
        [SerializeField] private Material mapMaterial;
        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private Color32 backgroundColor;
        [SerializeField] private float mapBgWidth;
        [SerializeField] private float mapBgYOffset;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Line Path Settings")]
        [SerializeField] private GameObject linePrefab;
        [Tooltip("Line point count should be > 2 to get smooth color gradients")]
        [Range(3, 10)]
        [SerializeField] private int linePointsCount = 10;
        [Tooltip("Distance from the node till the line starting point")]
        [SerializeField] private float offsetFromNodes = 0.5f;
        [Tooltip("Visited or available path color")]
        [SerializeField] private Color32 lineVisitedColor = Color.white;
        [Tooltip("Unavailable path color")]
        [SerializeField] private Color32 lineLockedColor = Color.gray;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Sorting Layer Properties")]
        [SerializeField] private int baseMapSortingLayer = 26000;
        [SerializeField] private GameObject mapKeyVisualParent;
        [SerializeField] private Canvas blackUnderlayCanvas;
        [SerializeField] private CanvasGroup blackUnderlayCg;
        [SerializeField] private GameObject blackUnderlayParent;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        // Non inspector fields
        private GameObject firstParent;
        private GameObject mapParent;
        private readonly List<MapNode> MapNodes = new List<MapNode>();
        private readonly List<LineConnection> lineConnections = new List<LineConnection>();

      
        public int BaseMapSortingLayer
        {
            get { return baseMapSortingLayer; }
            private set { baseMapSortingLayer = value; }
        }
        public GameObject MasterMapParent
        {
            get { return masterMapParent; }
            private set { masterMapParent = value; }
        }

        public void OnWorldMapButtonClicked()
        {
            if (!MasterMapParent.activeSelf)
            {
                ShowMainMapView();
                /*
                if (CharacterRosterViewController.Instance.MainVisualParent.activeSelf)
                {
                    CharacterRosterViewController.Instance.DisableMainView();
                }
                */
            }
            else
            {
                HideMainMapView();
            }
        }

        public void ShowMainMapView()
        {
            Debug.Log("MapView.ShowMainMapView() called...");
            MasterMapParent.SetActive(true);
            ShowMap(MapManager.Instance.CurrentMap);

            // Make map centre pn current encounter row
            int playerPos = 1;//RunController.Instance.CurrentJourneyPosition;
            if(playerPos > 3)
                mapParent.transform.localPosition = new Vector3(mapParent.transform.localPosition.x, -(playerPos * 2 -2), mapParent.transform.localPosition.z);

            blackUnderlayCanvas.sortingOrder = BaseMapSortingLayer - 1;
            blackUnderlayParent.SetActive(true);
            mapKeyVisualParent.SetActive(true);
            blackUnderlayCg.DOKill();
            blackUnderlayCg.DOFade(1, 0.25f);

        }
        public void HideMainMapView()
        {
            Debug.Log("MapView.HideMainMapView() called...");
            MasterMapParent.SetActive(false);

            mapKeyVisualParent.SetActive(false);
            blackUnderlayParent.SetActive(false);
            blackUnderlayCg.DOKill();
            blackUnderlayCg.DOFade(0, 0);
        }

        private void ClearMap()
        {
            if (firstParent != null)
                Destroy(firstParent);

            MapNodes.Clear();
            lineConnections.Clear();
        }

        private void ShowMap(Map m)
        {
            Debug.Log("MapView.ShowMap() called...");

            if (m == null)
            {
                Debug.LogWarning("Map was null in MapView.ShowMap()");
                return;
            }

            ClearMap();

            CreateMapParent();

            CreateNodes(m.nodes);

            DrawLines();

            SetOrientation();

            ResetNodesRotation();

            SetAttainableNodes();

            SetLineColors();

            CreateMapBackground(m);

            SetMapScale(0.7f);
        }

        private void CreateMapBackground(Map m)
        {
            if (backgroundSprite == null) return;

            var backgroundObject = new GameObject("Background");
            backgroundObject.transform.SetParent(mapParent.transform);
            var bossNode = MapNodes.FirstOrDefault(node => node.Node.NodeType == EncounterType.BossEnemy);
            var span = m.DistanceBetweenFirstAndLastLayers();
            backgroundObject.transform.localPosition = new Vector3(bossNode.transform.localPosition.x, (span / 2f), 0f);
            backgroundObject.transform.localRotation = Quaternion.identity;
            var sr = backgroundObject.AddComponent<SpriteRenderer>();
            sr.color = backgroundColor;
            sr.sortingOrder = baseMapSortingLayer;
            sr.material = mapMaterial;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.sprite = backgroundSprite;
            sr.size = new Vector2(mapBgWidth, span + mapBgYOffset * 2f);
        }

        private void CreateMapParent()
        {
            firstParent = new GameObject("OuterMapParent");
            firstParent.transform.SetParent(MasterMapParent.transform);
            mapParent = new GameObject("MapParentWithAScroll");
            mapParent.transform.SetParent(firstParent.transform);
            var scrollNonUi = mapParent.AddComponent<ScrollNonUI>();
            scrollNonUi.freezeX = orientation == MapOrientation.BottomToTop || orientation == MapOrientation.TopToBottom;
            scrollNonUi.freezeY = orientation == MapOrientation.LeftToRight || orientation == MapOrientation.RightToLeft;
            var boxCollider = mapParent.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(100, 100, 0.5f);
        }
        private void SetMapScale(float scale)
        {
            firstParent.transform.localScale = new Vector3(scale, scale, 1);
        }

        private void CreateNodes(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                var mapNode = CreateMapNode(node);
                MapNodes.Add(mapNode);
            }
        }

        private MapNode CreateMapNode(Node node)
        {
            var mapNodeObject = Instantiate(nodePrefab, mapParent.transform);
            var mapNode = mapNodeObject.GetComponent<MapNode>();
            var blueprint = GetBlueprint(node.BlueprintName);
            mapNode.SetUp(node, blueprint);
            mapNode.transform.localPosition = node.position;
            return mapNode;
        }

        public void SetAttainableNodes()
        {
            // first set all the nodes as unattainable/locked:
            foreach (var node in MapNodes)
                node.SetState(NodeStates.Locked);

            if (mapManager.CurrentMap.path.Count == 0)
            {
                // we have not started traveling on this map yet, set entire first layer as attainable:
                foreach (var node in MapNodes.Where(n => n.Node.point.y == 0))
                    node.SetState(NodeStates.Attainable);
            }
            else
            {
                // we have already started moving on this map, first highlight the path as visited:
                foreach (var point in mapManager.CurrentMap.path)
                {
                    var mapNode = GetNode(point);
                    if (mapNode != null)
                        mapNode.SetState(NodeStates.Visited);
                }

                var currentPoint = mapManager.CurrentMap.path[mapManager.CurrentMap.path.Count - 1];
                var currentNode = mapManager.CurrentMap.GetNode(currentPoint);

                // set all the nodes that we can travel to as attainable:
                foreach (var point in currentNode.outgoing)
                {
                    var mapNode = GetNode(point);
                    if (mapNode != null)
                        mapNode.SetState(NodeStates.Attainable);
                }
            }
        }

        public void SetLineColors()
        {
            // set all lines to grayed out first:
            foreach (var connection in lineConnections)
                connection.SetColor(lineLockedColor);

            // set all lines that are a part of the path to visited color:
            // if we have not started moving on the map yet, leave everything as is:
            if (mapManager.CurrentMap.path.Count == 0)
                return;

            // in any case, we mark outgoing connections from the final node with visible/attainable color:
            var currentPoint = mapManager.CurrentMap.path[mapManager.CurrentMap.path.Count - 1];
            var currentNode = mapManager.CurrentMap.GetNode(currentPoint);

            foreach (var point in currentNode.outgoing)
            {
                var lineConnection = lineConnections.FirstOrDefault(conn => conn.from.Node == currentNode &&
                                                                            conn.to.Node.point.Equals(point));
                lineConnection?.SetColor(lineVisitedColor);
            }

            if (mapManager.CurrentMap.path.Count <= 1) return;

            for (var i = 0; i < mapManager.CurrentMap.path.Count - 1; i++)
            {
                var current = mapManager.CurrentMap.path[i];
                var next = mapManager.CurrentMap.path[i + 1];
                var lineConnection = lineConnections.FirstOrDefault(conn => conn.@from.Node.point.Equals(current) &&
                                                                            conn.to.Node.point.Equals(next));
                lineConnection?.SetColor(lineVisitedColor);
            }
        }

        private void SetOrientation()
        {
            var scrollNonUi = mapParent.GetComponent<ScrollNonUI>();
            var span = mapManager.CurrentMap.DistanceBetweenFirstAndLastLayers();
            var bossNode = MapNodes.FirstOrDefault(node => node.Node.NodeType == EncounterType.BossEnemy);
            Debug.Log("Map span in set orientation: " + span + " camera aspect: " + CameraController.Instance.MainCamera.aspect);

            // setting first parent to be right in front of the camera first:
            firstParent.transform.position = new Vector3(CameraController.Instance.MainCamera.transform.position.x, CameraController.Instance.MainCamera.transform.position.y, 0f);
            var offset = orientationOffset;
            switch (orientation)
            {
                case MapOrientation.BottomToTop:
                    if (scrollNonUi != null)
                    {
                        scrollNonUi.yConstraints.max = 0;
                        scrollNonUi.yConstraints.min = -(span + 2f * offset);
                    }
                    firstParent.transform.localPosition += new Vector3(0, offset, 0);
                    break;
                case MapOrientation.TopToBottom:
                    mapParent.transform.eulerAngles = new Vector3(0, 0, 180);
                    if (scrollNonUi != null)
                    {
                        scrollNonUi.yConstraints.min = 0;
                        scrollNonUi.yConstraints.max = span + 2f * offset;
                    }
                    // factor in map span:
                    firstParent.transform.localPosition += new Vector3(0, -offset, 0);
                    break;
                case MapOrientation.RightToLeft:
                    offset *= CameraController.Instance.MainCamera.aspect;
                    mapParent.transform.eulerAngles = new Vector3(0, 0, 90);
                    // factor in map span:
                    firstParent.transform.localPosition -= new Vector3(offset, bossNode.transform.position.y, 0);
                    if (scrollNonUi != null)
                    {
                        scrollNonUi.xConstraints.max = span + 2f * offset;
                        scrollNonUi.xConstraints.min = 0;
                    }
                    break;
                case MapOrientation.LeftToRight:
                    offset *= CameraController.Instance.MainCamera.aspect;
                    mapParent.transform.eulerAngles = new Vector3(0, 0, -90);
                    firstParent.transform.localPosition += new Vector3(offset, -bossNode.transform.position.y, 0);
                    if (scrollNonUi != null)
                    {
                        scrollNonUi.xConstraints.max = 0;
                        scrollNonUi.xConstraints.min = -(span + 2f * offset);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawLines()
        {
            foreach (var node in MapNodes)
            {
                foreach (var connection in node.Node.outgoing)
                    AddLineConnection(node, GetNode(connection));
            }
        }

        private void ResetNodesRotation()
        {
            foreach (var node in MapNodes)
                node.transform.rotation = Quaternion.identity;
        }

        public void AddLineConnection(MapNode from, MapNode to)
        {
            var lineObject = Instantiate(linePrefab, mapParent.transform);
            var lineRenderer = lineObject.GetComponent<LineRenderer>();
            lineRenderer.sortingOrder = BaseMapSortingLayer + 1;
            var fromPoint = from.transform.position +
                            (to.transform.position - from.transform.position).normalized * offsetFromNodes;

            var toPoint = to.transform.position +
                          (from.transform.position - to.transform.position).normalized * offsetFromNodes;

            // drawing lines in local space:
            lineObject.transform.position = fromPoint;
            lineRenderer.useWorldSpace = false;

            // line renderer with 2 points only does not handle transparency properly:
            lineRenderer.positionCount = linePointsCount;
            for (var i = 0; i < linePointsCount; i++)
            {
                lineRenderer.SetPosition(i,
                    Vector3.Lerp(Vector3.zero, toPoint - fromPoint, (float)i / (linePointsCount - 1)));
            }

            var dottedLine = lineObject.GetComponent<DottedLineRenderer>();
            if (dottedLine != null) dottedLine.ScaleMaterial();

            lineConnections.Add(new LineConnection(lineRenderer, from, to));
        }

        private MapNode GetNode(Point p)
        {
            return MapNodes.FirstOrDefault(n => n.Node.point.Equals(p));
        }

        private DungeonMapSeed GetConfig(string configName)
        {
            return allMapConfigs.FirstOrDefault(c => c.name == configName);
        }

        public NodeBlueprint GetBlueprint(EncounterType type)
        {
            var config = GetConfig(mapManager.CurrentMap.configName);
            return config.nodeBlueprints.FirstOrDefault(n => n.nodeType == type);
        }

        public NodeBlueprint GetBlueprint(string blueprintName)
        {
            var config = GetConfig(mapManager.CurrentMap.configName);
            if(config == null)
            {
                Debug.LogWarning("map config is null");
            }
            return config.nodeBlueprints.FirstOrDefault(n => n.name == blueprintName);
        }
    }
}
