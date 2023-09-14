using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.DungeonMap
{
    public class MapGenerator : Singleton<MapGenerator>
    {

        public void ResetGenerationSettings()
        {
            SpawnedCampSites = 0;
            SpawnedShops = 0;
            SpawnedElites = 0;

            TimeTilCampSiteAllowed = 0;
            TimeTilShopAllowed = 0;
            TimeTilEliteAllowed = 0;
        }

        public Map GetMap(DungeonMapSeed conf)
        {
            if (conf == null)
            {
                Debug.LogWarning("Config was null in MapGenerator.Generate()");
                return null;
            }

            ResetGenerationSettings();

            config = conf;
            nodes.Clear();

            GenerateLayerDistances();

            for (int i = 0; i < conf.layers.Length; i++)
            {
                PlaceLayer(i);
            }

            GeneratePaths();

            RandomizeNodePositions();

            SetUpConnections();

            RemoveCrossConnections();

            // select all the nodes with connections:
            List<Node> nodesList = nodes.SelectMany(n => n).Where(n => n.incoming.Count > 0 || n.outgoing.Count > 0).ToList();

            // pick a random name of the boss level for this map:
            // var bossNodeName = config.nodeBlueprints.Where(b => b.nodeType == EncounterType.BossEnemy).ToList().Random().name;
            string bossNodeName = "King Herp Derp Boss";
            return new Map(conf.name, bossNodeName, nodesList, new List<Point>());
        }

        private void GenerateLayerDistances()
        {
            layerDistances = new List<float>();
            foreach (MapLayer layer in config.layers)
            {
                layerDistances.Add(layer.layerXDifference.GetValue());
            }
            //layerDistances.Add(5);
        }

        private float GetDistanceToLayer(int layerIndex)
        {
            if (layerIndex < 0 || layerIndex > layerDistances.Count)
            {
                return 0f;
            }

            return layerDistances.Take(layerIndex + 1).Sum();
        }

        private void PlaceLayer(int layerIndex)
        {
            MapLayer layer = config.layers[layerIndex];
            List<Node> nodesOnThisLayer = new List<Node>();
            bool atleastOneRandom = false;
            bool atleastOneOriginal = false;

            // offset of this layer to make all the nodes centered:
            float offset = layer.nodeYDistance * config.GridWidth / 2f;

            for (int i = 0; i < config.GridWidth; i++)
            {
                EncounterType nodeType = Random.Range(0f, 1f) < layer.randomizeNodes ? GetRandomNode(layer.possibleRandomNodeTypes) : layer.nodeType;
                if (nodeType != layer.nodeType)
                {
                    atleastOneRandom = true;
                }
                else if (nodeType == layer.nodeType)
                {
                    atleastOneOriginal = true;
                }

                string blueprintName = config.nodeBlueprints.Where(b => b.nodeType == nodeType).ToList().GetRandomElement().name;
                Node node = new Node(nodeType, blueprintName, new Point(i, layerIndex))
                {
                    position = new Vector2(-offset + i * layer.nodeYDistance, GetDistanceToLayer(layerIndex))
                };
                nodesOnThisLayer.Add(node);
            }

            if (!atleastOneRandom && layer.guaranteeAtleastOneRandom)
            {
                Debug.LogWarning("Didn't hit a random node, rerolling for a random node on layer " + layerIndex);

                // get a random node + type on the layer
                Node randomNode = nodesOnThisLayer[RandomGenerator.NumberBetween(0, nodesOnThisLayer.Count - 1)];
                EncounterType randomNodeType = GetRandomNode(layer.possibleRandomNodeTypes);

                // change the node to new random type
                string blueprintName = config.nodeBlueprints.Where(b => b.nodeType == randomNodeType).ToList().GetRandomElement().name;
                randomNode.RerollType(randomNodeType, blueprintName);
            }

            if (!atleastOneOriginal && layer.guaranteeAtleastOneOfChosenType)
            {
                Debug.LogWarning("Didn't hit an originally selected node type, rerolling to guarantee an original node choice on layer " + layerIndex);

                // get a random node + type on the layer
                Node randomNode = nodesOnThisLayer[RandomGenerator.NumberBetween(0, nodesOnThisLayer.Count - 1)];
                EncounterType fixedNodeType = layer.nodeType;

                // change the node to new random type
                string blueprintName = config.nodeBlueprints.Where(b => b.nodeType == fixedNodeType).ToList().GetRandomElement().name;
                randomNode.RerollType(fixedNodeType, blueprintName);
            }

            nodes.Add(nodesOnThisLayer);

            // update previous layer data
            previousLayerData.Clear();
            previousLayerData.AddRange(nodesOnThisLayer);
        }
        private bool DoesLayerContainEncounterType(EncounterType type, List<Node> layerNodes)
        {
            bool bRet = false;
            foreach (Node node in layerNodes)
            {
                if (node.NodeType == type)
                {
                    bRet = true;
                    break;
                }

            }

            return bRet;
        }
        /*
        private static EncounterType CalculateNodeType()
        {

        }
        */

        // Handle Remove duplicate nodes function
        // Handle Reroll node type

        private void RandomizeNodePositions()
        {
            for (int index = 0; index < nodes.Count; index++)
            {
                List<Node> list = nodes[index];
                MapLayer layer = config.layers[index];
                float distToNextLayer = index + 1 >= layerDistances.Count
                    ? 0f
                    : layerDistances[index + 1];
                float distToPreviousLayer = layerDistances[index];

                foreach (Node node in list)
                {
                    float xRnd = Random.Range(-1f, 1f);
                    float yRnd = Random.Range(-1f, 1f);

                    float x = xRnd * layer.nodeYDistance / 2f;
                    float y = yRnd < 0 ? distToPreviousLayer * yRnd / 2f : distToNextLayer * yRnd / 2f;

                    node.position += new Vector2(x, y) * layer.randomizePosition;
                }
            }
        }

        private void SetUpConnections()
        {
            foreach (List<Point> path in paths)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    Node node = GetNode(path[i]);

                    if (i > 0)
                    {
                        // previous because the path is flipped
                        Node nextNode = GetNode(path[i - 1]);
                        nextNode.AddIncoming(node.point);
                        node.AddOutgoing(nextNode.point);
                    }

                    if (i < path.Count - 1)
                    {
                        Node previousNode = GetNode(path[i + 1]);
                        previousNode.AddOutgoing(node.point);
                        node.AddIncoming(previousNode.point);
                    }
                }
            }
        }

        private void RemoveCrossConnections()
        {
            for (int i = 0; i < config.GridWidth - 1; i++)
            for (int j = 0; j < config.layers.Length - 1; j++)
            {
                Node node = GetNode(new Point(i, j));
                if (node == null || node.HasNoConnections())
                {
                    continue;
                }
                Node right = GetNode(new Point(i + 1, j));
                if (right == null || right.HasNoConnections())
                {
                    continue;
                }
                Node top = GetNode(new Point(i, j + 1));
                if (top == null || top.HasNoConnections())
                {
                    continue;
                }
                Node topRight = GetNode(new Point(i + 1, j + 1));
                if (topRight == null || topRight.HasNoConnections())
                {
                    continue;
                }

                // Debug.Log("Inspecting node for connections: " + node.point);
                if (!node.outgoing.Any(element => element.Equals(topRight.point)))
                {
                    continue;
                }
                if (!right.outgoing.Any(element => element.Equals(top.point)))
                {
                    continue;
                }

                // Debug.Log("Found a cross node: " + node.point);

                // we managed to find a cross node:
                // 1) add direct connections:
                node.AddOutgoing(top.point);
                top.AddIncoming(node.point);

                right.AddOutgoing(topRight.point);
                topRight.AddIncoming(right.point);

                float rnd = Random.Range(0f, 1f);
                if (rnd < 0.2f)
                {
                    // remove both cross connections:
                    // a) 
                    node.RemoveOutgoing(topRight.point);
                    topRight.RemoveIncoming(node.point);
                    // b) 
                    right.RemoveOutgoing(top.point);
                    top.RemoveIncoming(right.point);
                }
                else if (rnd < 0.6f)
                {
                    // a) 
                    node.RemoveOutgoing(topRight.point);
                    topRight.RemoveIncoming(node.point);
                }
                else
                {
                    // b) 
                    right.RemoveOutgoing(top.point);
                    top.RemoveIncoming(right.point);
                }
            }
        }

        private Node GetNode(Point p)
        {
            if (p.y >= nodes.Count)
            {
                return null;
            }
            if (p.x >= nodes[p.y].Count)
            {
                return null;
            }

            return nodes[p.y][p.x];
        }

        private Point GetFinalNode()
        {
            int y = config.layers.Length - 1;
            if (config.GridWidth % 2 == 1)
            {
                return new Point(config.GridWidth / 2, y);
            }

            return Random.Range(0, 2) == 0
                ? new Point(config.GridWidth / 2, y)
                : new Point(config.GridWidth / 2 - 1, y);
        }

        private void GeneratePaths()
        {
            Point finalNode = GetFinalNode();
            paths = new List<List<Point>>();
            int numOfStartingNodes = config.numOfStartingNodes.GetValue();
            int numOfPreBossNodes = config.numOfPreBossNodes.GetValue();

            List<int> candidateXs = new List<int>();
            for (int i = 0; i < config.GridWidth; i++)
            {
                candidateXs.Add(i);
            }

            candidateXs.Shuffle();
            IEnumerable<int> preBossXs = candidateXs.Take(numOfPreBossNodes);
            List<Point> preBossPoints = (from x in preBossXs select new Point(x, finalNode.y - 1)).ToList();
            int attempts = 0;

            // start by generating paths from each of the preBossPoints to the 1st layer:
            foreach (Point point in preBossPoints)
            {
                List<Point> path = Path(point, 0, config.GridWidth);
                path.Insert(0, finalNode);
                paths.Add(path);
                attempts++;
            }

            while (!PathsLeadToAtLeastNDifferentPoints(paths, numOfStartingNodes) && attempts < 100)
            {
                Point randomPreBossPoint = preBossPoints[Random.Range(0, preBossPoints.Count)];
                List<Point> path = Path(randomPreBossPoint, 0, config.GridWidth);
                path.Insert(0, finalNode);
                paths.Add(path);
                attempts++;
            }

            Debug.Log("Attempts to generate paths: " + attempts);
        }

        private bool PathsLeadToAtLeastNDifferentPoints(IEnumerable<List<Point>> paths, int n)
        {
            return (from path in paths select path[path.Count - 1].x).Distinct().Count() >= n;
        }

        private List<Point> Path(Point from, int toY, int width, bool firstStepUnconstrained = false)
        {
            if (from.y == toY)
            {
                Debug.LogError("Points are on same layers, return");
                return null;
            }

            // making one y step in this direction with each move
            int direction = from.y > toY ? -1 : 1;

            List<Point> path = new List<Point>
            {
                from
            };
            while (path[path.Count - 1].y != toY)
            {
                Point lastPoint = path[path.Count - 1];
                List<int> candidateXs = new List<int>();
                if (firstStepUnconstrained && lastPoint.Equals(from))
                {
                    for (int i = 0; i < width; i++)
                    {
                        candidateXs.Add(i);
                    }
                }
                else
                {
                    // forward
                    candidateXs.Add(lastPoint.x);
                    // left
                    if (lastPoint.x - 1 >= 0)
                    {
                        candidateXs.Add(lastPoint.x - 1);
                    }
                    // right
                    if (lastPoint.x + 1 < width)
                    {
                        candidateXs.Add(lastPoint.x + 1);
                    }
                }

                Point nextPoint = new Point(candidateXs[Random.Range(0, candidateXs.Count)], lastPoint.y + direction);
                path.Add(nextPoint);
            }

            return path;
        }

        private EncounterType GetRandomNode()
        {
            if (RandomNodes.Count == 1)
            {
                return RandomNodes[0];
            }
            return RandomNodes[RandomGenerator.NumberBetween(0, RandomNodes.Count - 1)];
        }
        private EncounterType GetRandomNode(EncounterType[] possibleRandomNodes)
        {
            EncounterType nodeReturned = EncounterType.BasicEnemy;

            // Filter out possible choices
            List<EncounterType> filteredChoices = new List<EncounterType>();

            if (possibleRandomNodes.Contains(EncounterType.CampSite) &&
                SpawnedCampSites <= maximumCampSites &&
                TimeTilCampSiteAllowed <= 0 &&
                !DoesLayerContainEncounterType(EncounterType.CampSite, previousLayerData))
            {
                filteredChoices.Add(EncounterType.CampSite);
            }
            if (possibleRandomNodes.Contains(EncounterType.EliteEnemy) &&
                SpawnedElites <= maximumElites &&
                TimeTilEliteAllowed <= 0 &&
                !DoesLayerContainEncounterType(EncounterType.EliteEnemy, previousLayerData))
            {
                filteredChoices.Add(EncounterType.EliteEnemy);
            }
            /*
            if (possibleRandomNodes.Contains(EncounterType.Shop) &&
              (SpawnedShops <= maximumShops) &&
                TimeTilShopAllowed <= 0 &&
                !DoesLayerContainEncounterType(EncounterType.Shop, previousLayerData))
            {
                filteredChoices.Add(EncounterType.Shop);
            }
            */

            // Randomly pick an encounter type
            if (filteredChoices.Count == 0)
            {
                nodeReturned = EncounterType.BasicEnemy;
            }
            else if (filteredChoices.Count == 1)
            {
                nodeReturned = filteredChoices[0];
            }
            else
            {
                nodeReturned = filteredChoices[RandomGenerator.NumberBetween(0, filteredChoices.Count - 1)];
            }

            if (nodeReturned == EncounterType.BasicEnemy)
            {
                TimeTilShopAllowed--;
                TimeTilCampSiteAllowed--;
                TimeTilEliteAllowed--;
            }
            if (nodeReturned == EncounterType.EliteEnemy)
            {
                SpawnedElites++;
                TimeTilEliteAllowed = nodeFrequencyLimit;
                TimeTilShopAllowed--;
                TimeTilCampSiteAllowed--;

            }
            /*
            else if (nodeReturned == EncounterType.Shop)
            {
                SpawnedShops++;
                TimeTilShopAllowed = nodeFrequencyLimit;
                TimeTilEliteAllowed--;
                TimeTilCampSiteAllowed--;
            }
            */
            else if (nodeReturned == EncounterType.CampSite)
            {
                SpawnedCampSites++;
                TimeTilCampSiteAllowed = nodeFrequencyLimit;
                TimeTilShopAllowed--;
                TimeTilEliteAllowed--;
            }

            return nodeReturned;

        }
        // Properties + Components
        #region

        private DungeonMapSeed config;

        private readonly List<EncounterType> RandomNodes = new List<EncounterType>
        {
            EncounterType.BasicEnemy,
            EncounterType.EliteEnemy,
            EncounterType.BossEnemy,
            EncounterType.CampSite
        };

        private List<float> layerDistances;
        private List<List<Point>> paths;
        // ALL nodes by layer:
        private readonly List<List<Node>> nodes = new List<List<Node>>();

        [Header("Map Encounter Properties")]
        public int maximumCampSites = 6;
        public int maximumShops = 6;
        public int maximumElites = 8;

        [Header("Misc Properties")]
        public int nodeFrequencyLimit = 3;

        private readonly List<Node> previousLayerData = new List<Node>();

        #endregion

        // Getters + Accessors
        #region

        public int SpawnedCampSites { get; private set; }
        public int SpawnedShops { get; private set; }
        public int SpawnedElites { get; private set; }
        public int TimeTilCampSiteAllowed { get; private set; }
        public int TimeTilShopAllowed { get; private set; }
        public int TimeTilEliteAllowed { get; private set; }

        #endregion
    }

}
