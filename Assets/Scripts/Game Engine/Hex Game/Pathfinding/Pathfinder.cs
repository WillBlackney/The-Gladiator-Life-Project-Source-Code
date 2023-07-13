using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TbsFramework.Pathfinding.Algorithms;
using TbsFramework.Pathfinding.DataStructs;
using WeAreGladiators.HexTiles;
using WeAreGladiators.Characters;
using WeAreGladiators.Perks;

namespace WeAreGladiators.Pathfinding
{
    public static class Pathfinder
    {
        // Properties 
        #region
        private static DPathfinder dijikstra = new DPathfinder();
        #endregion

        // Action Point + Fatigue Cost Logic
        #region
        public static int GetFatigueCostBetweenHexs(HexCharacterModel character, LevelNode start, LevelNode destination)
        {
            int cost = destination.BaseMoveFatigueCost;
            if (start.Elevation != destination.Elevation)
                cost += 2;

            if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Pathfinder))
                cost = 0;

            return cost;
        }
        public static int GetFatigueCostOfPath(HexCharacterModel character, LevelNode start, List<LevelNode> path)
        {
            int pathCost = 0;
            if (path.Count == 0)
                return 0;

            else if (path.Count == 1)
            {
                pathCost += GetFatigueCostBetweenHexs(character, start, path[0]);
            }
            else if (path.Count > 1)
            {
                List<LevelNode> fullPath = new List<LevelNode>();
                fullPath.Add(start);
                fullPath.AddRange(path);
                for (int i = 0; i < fullPath.Count - 1; i++)                
                    pathCost += GetFatigueCostBetweenHexs(character, fullPath[i], fullPath[i + 1]);                
            }

            Debug.Log("Fatigue cost of path = " + pathCost.ToString());
            return pathCost;
        }
        public static int GetActionPointCostBetweenHexs(HexCharacterModel character, LevelNode start, LevelNode destination)
        {
            int cost = destination.BaseMoveActionPointCost;
            if (start.Elevation != destination.Elevation)
                cost += 1;
            
            // Injuries
            if(!PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.FleshAscension))
            {
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.BruisedLeg))
                    cost += 1;

                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.BrokenLeg))
                    cost += 2;

                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.TornKneeLigament))
                    cost += 1;
            }            

            if (cost > 2 && PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Pathfinder))
                cost = 2;

            return cost;
        }       
        public static int GetActionPointCostOfPath(HexCharacterModel character, LevelNode start, List<LevelNode> path)
        {
            int pathCost = 0;
            if (path.Count == 0)
                return 0;

            else if (path.Count == 1)
            {
                if((PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Nimble) && character.tilesMovedThisTurn == 0)
                   || PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Flight))
                {

                }
                else
                    pathCost += GetActionPointCostBetweenHexs(character, start, path[0]);
            }
            else if (path.Count > 1)
            {
                List<LevelNode> fullPath = new List<LevelNode>();
                fullPath.Add(start);
                fullPath.AddRange(path);
                int freeMoves = 0;
                if (PerkController.Instance.DoesCharacterHavePerk(character.pManager, Perk.Nimble) && character.tilesMovedThisTurn == 0)
                    freeMoves++;
                freeMoves += PerkController.Instance.GetStackCountOfPerkOnCharacter(character.pManager, Perk.Flight);

                for (int i = 0; i < fullPath.Count - 1; i++)
                {
                    if(i >= freeMoves)
                    {
                        pathCost += GetActionPointCostBetweenHexs(character, fullPath[i], fullPath[i + 1]);
                    }
                }
                
                /*
                pathCost += GetEnergyCostBetweenHexs(character, start, path[0]);
                for (int i = 0; i < path.Count - 1; i++)
                {
                    pathCost += GetEnergyCostBetweenHexs(character, path[i], path[i + 1]);
                }
                */
            }

            Debug.Log("Energy cost of path = " + pathCost.ToString());
            return pathCost;
        }       
     
        #endregion

        // Get Paths + Valid Destinations
        #region
       
        public static Path GetValidPath(HexCharacterModel character, LevelNode start, LevelNode destination, List<LevelNode> allHexes)
        {
            // Simple way to get a path from A to B. If it is impossible to draw a path between A and B, 
            // or the path is invalid for the character (not enough energy, untraversable destination, etc),
            // this function returns a null path. ALWAYS check the result of this function using the 
            // function 'IsPathValid(Path p) before trying to use the path returned...

            Dictionary<LevelNode, List<LevelNode>> paths = GetDijisktraPaths(character, start, allHexes);
            Path pathReturned = null;
            List<Path> possiblePaths = new List<Path>();

            foreach (LevelNode key in paths.Keys)
            {
                // Found the path with matching destination and start
                if (key == destination)
                {
                    Debug.Log("GetPath() found a path that connects start to destination");
                    List<LevelNode> hexsOnPath = paths[key];
                    hexsOnPath.Reverse();

                    // Check traversability
                    bool traversable = true;
                    foreach (LevelNode h in hexsOnPath)
                    {
                        if (IsHexTraversable(h, character) == false)
                        {
                            traversable = false;
                            break;
                        }
                    }

                    // Is the path between start/destination actually valid?
                    if (traversable &&
                        CanHexBeOccupied(key) &&
                        GetActionPointCostOfPath(character, start, hexsOnPath) <= character.currentActionPoints &&
                        GetFatigueCostOfPath(character, start, hexsOnPath) <= StatCalculator.GetTotalMaxFatigue(character) - character.currentFatigue)
                    {
                        Debug.Log("GetPath() found a valid path!");
                        pathReturned = new Path(start, hexsOnPath, character);
                        possiblePaths.Add(pathReturned);
                    }
                    //break;
                }


            }

            Debug.Log("PathFinder.GetPath() valid paths between " + start.PrintGridPosition() + " and " + destination.PrintGridPosition() + " = " + possiblePaths.Count.ToString());

            return pathReturned;
        }
        public static List<LevelNode> GetAllValidPathableDestinations(HexCharacterModel character, LevelNode start, List<LevelNode> possibleHexs)
        {
            // Returns all hexs that a character can make a valid path to (can afford the energy cost, destination and path not obstructed, etc).

            List<Path> allPaths = GetAllValidPathsFromStart(character, start, possibleHexs);
            List<LevelNode> validDestinations = new List<LevelNode>();
            foreach (Path p in allPaths)
            {
                validDestinations.Add(p.Destination);
            }

            Debug.Log("Pathfinder.GetAllValidPathableDestinations() found " + validDestinations.Count.ToString() + " valid pathable destinations from start hex: " +
                start.GridPosition.x.ToString() + ", " + start.GridPosition.y.ToString());

            return validDestinations;
        }
        public static List<Path> GetAllValidPathsFromStart(HexCharacterModel character, LevelNode start, List<LevelNode> possibleHexs)
        {
            // returns all possible valid paths a character can make from the start hex.
            // this function is good for caching paths and avoiding cpu usage by repeatidly calling
            // djisktra algo.

            Dictionary<LevelNode, List<LevelNode>> paths = GetDijisktraPaths(character, start, possibleHexs);
            List<Path> pathsReturned = new List<Path>();

            foreach (LevelNode key in paths.Keys)
            {
                List<LevelNode> hexsOnPath = paths[key];
                hexsOnPath.Reverse();

                // check traversability
                bool traversable = true;
                foreach (LevelNode h in hexsOnPath)
                {
                    if (IsHexTraversable(h, character) == false)
                    {
                        traversable = false;
                        break;
                    }
                }

                // Is the path between start/destination actually valid?
                if (traversable &&
                    CanHexBeOccupied(key) &&
                    GetActionPointCostOfPath(character, start, hexsOnPath) <= character.currentActionPoints &&
                    GetFatigueCostOfPath(character, start, hexsOnPath) <= StatCalculator.GetTotalMaxFatigue(character) - character.currentFatigue)
                {
                    pathsReturned.Add(new Path(start, hexsOnPath, character));
                }
            }

            Debug.Log("Pathfinder.GetAllValidPathsFromStart() found " + pathsReturned.Count.ToString() + " valid paths from start hex " +
                start.GridPosition.x.ToString() + ", " + start.GridPosition.y.ToString());

            return pathsReturned;

        }
        #endregion

        // Movement + Placement
        #region

        public static void PlaceCharacterOnHex(HexCharacterModel character, LevelNode hex)
        {
            if (character.currentTile != null)
            {
                character.currentTile.CurrentUnit = null;
            }

            hex.myCharacter = character;
            character.currentTile = hex;
        }      
        #endregion

        // Misc
        #region
        private static Dictionary<LevelNode, Dictionary<LevelNode, float>> GetGraphEdges(HexCharacterModel character, List<LevelNode> cells)
        {
            Dictionary<LevelNode, Dictionary<LevelNode, float>> ret = new Dictionary<LevelNode, Dictionary<LevelNode, float>>();

            foreach (var cell in cells)
            {
                if (cell.Equals(character.currentTile) || IsHexTraversable(cell, character))
                {
                    ret[cell] = new Dictionary<LevelNode, float>();
                    foreach (var neighbour in cell.NeighbourNodes(cells))
                    {
                        ret[cell][neighbour] = GetActionPointCostBetweenHexs(character, cell, neighbour);
                    }
                }
            }

            Debug.Log("GetGraphEdges() graph edges count: " + ret.Count.ToString());
            return ret;
        }

        #endregion

        // Combat Grid Bools And Checks
        #region
        public static bool IsPathValid(Path p)
        {
            // in future, may possible put more conditions here, so place holder for now
            if (p != null)
            {
                return true;
            }
            else
                return false;
        }
        public static bool CanHexBeOccupied(LevelNode hex)
        {
            // to do in future: should also check if there is an obstacle or thing on the hex that prevents a character standing on it,
            // not just checking if a character is already standing on it
            // TO DO: uncomment when we have obnstacles
            return hex.myCharacter == null && hex.Obstructed == false;
        }
        public static bool IsHexTraversable(LevelNode hex, HexCharacterModel mover)
        {
            // can the character move THROUGH the hex enroute to some other hex?
            // NOTE: characters can move through allies: if enemy or obstruction on tile, should return false, else true

            if (hex.Obstructed)
                return false;

            //else 
            if (hex.myCharacter != null)
            {
                // Characters can move over and through allies
                if (HexCharacterController.Instance.GetAllAlliesOfCharacter(mover).Contains(hex.myCharacter))                
                    return true;                
                else return false;
            }

            else return true;
            
        }
        public static bool IsHexSpawnable(LevelNode hex)
        {
            return hex.myCharacter == null && hex.Obstructed == false; 
        }
        #endregion        

        // Dijisktra Logic
        #region
        private static Dictionary<LevelNode, List<LevelNode>> GetDijisktraPaths(HexCharacterModel character, LevelNode start, List<LevelNode> cells)
        {
            var edges = GetGraphEdges(character, cells);
            var paths = dijikstra.FindAllPaths(edges, start);
            return paths;
        }
       
        #endregion

    }
    class DPathfinder : IPathfinding
    {
        public Dictionary<LevelNode, List<LevelNode>> FindAllPaths(Dictionary<LevelNode, Dictionary<LevelNode, float>> edges, LevelNode originNode)
        {
            IPriorityQueue<LevelNode> frontier = new HeapPriorityQueue<LevelNode>();
            frontier.Enqueue(originNode, 0);

            Dictionary<LevelNode, LevelNode> cameFrom = new Dictionary<LevelNode, LevelNode>();
            cameFrom.Add(originNode, default(LevelNode));
            Dictionary<LevelNode, float> costSoFar = new Dictionary<LevelNode, float>();
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                var neighbours = GetNeigbours(edges, current);
                foreach (var neighbour in neighbours)
                {
                    var newCost = costSoFar[current] + edges[current][neighbour];
                    if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        frontier.Enqueue(neighbour, newCost);
                    }
                }
            }

            Dictionary<LevelNode, List<LevelNode>> paths = new Dictionary<LevelNode, List<LevelNode>>();
            foreach (LevelNode destination in cameFrom.Keys)
            {
                List<LevelNode> path = new List<LevelNode>();
                var current = destination;
                while (!current.Equals(originNode))
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                paths.Add(destination, path);
            }
            return paths;
        }
        public override List<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode, T destinationNode)
        {
            IPriorityQueue<T> frontier = new HeapPriorityQueue<T>();
            frontier.Enqueue(originNode, 0);

            Dictionary<T, T> cameFrom = new Dictionary<T, T>();
            cameFrom.Add(originNode, default(T));
            Dictionary<T, float> costSoFar = new Dictionary<T, float>();
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                var neighbours = GetNeigbours(edges, current);
                foreach (var neighbour in neighbours)
                {
                    var newCost = costSoFar[current] + edges[current][neighbour];
                    if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        frontier.Enqueue(neighbour, newCost);
                    }
                }
                if (current.Equals(destinationNode)) break;
            }
            List<T> path = new List<T>();
            if (!cameFrom.ContainsKey(destinationNode))
                return path;

            path.Add(destinationNode);
            var temp = destinationNode;

            while (!cameFrom[temp].Equals(originNode))
            {
                var currentPathElement = cameFrom[temp];
                path.Add(currentPathElement);

                temp = currentPathElement;
            }

            return path;
        }
        /*
        public Dictionary<LevelNode, List<LevelNode>> FindAllPaths(Dictionary<LevelNode, Dictionary<LevelNode, float>> edges, LevelNode originNode)
        {
            IPriorityQueue<LevelNode> frontier = new HeapPriorityQueue<LevelNode>();
            frontier.Enqueue(originNode, 0);

            Dictionary<LevelNode, LevelNode> cameFrom = new Dictionary<LevelNode, LevelNode>();
            cameFrom.Add(originNode, default(LevelNode));
            Dictionary<LevelNode, float> costSoFar = new Dictionary<LevelNode, float>();
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                var neighbours = GetNeigbours(edges, current);
               // Debug.Log("DPathfinder.FindAllPaths() neighbours found: " + neighbours.Count.ToString());
                foreach (var neighbour in neighbours)
                {
                    var newCost = costSoFar[current] + edges[current][neighbour];
                    if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        var priority = newCost + Heuristic(current, neighbour);
                        frontier.Enqueue(neighbour, priority);
                        //frontier.Enqueue(neighbour, newCost);
                    }
                }
            }

            Dictionary<LevelNode, List<LevelNode>> paths = new Dictionary<LevelNode, List<LevelNode>>();
            foreach (LevelNode destination in cameFrom.Keys)
            {
                List<LevelNode> path = new List<LevelNode>();
                var current = destination;
                while (!current.Equals(originNode))
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                paths.Add(destination, path);
            }

            /*
            Debug.Log("FindAllPaths() paths found: " + paths.Count.ToString());
            foreach(KeyValuePair<LevelNode, List<LevelNode>> kvp in paths)
            {
                Debug.Log("nodes on pathes::::");
                foreach (LevelNode n in kvp.Value)
                {
                    n.PrintGridPosition();
                }
            }
            
            return paths;
        }
        public override List<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode, T destinationNode)
        {
            IPriorityQueue<T> frontier = new HeapPriorityQueue<T>();
            frontier.Enqueue(originNode, 0);

            Dictionary<T, T> cameFrom = new Dictionary<T, T>();
            cameFrom.Add(originNode, default(T));
            Dictionary<T, float> costSoFar = new Dictionary<T, float>();
            costSoFar.Add(originNode, 0);

            while (frontier.Count != 0)
            {
                var current = frontier.Dequeue();
                var neighbours = GetNeigbours(edges, current);
                foreach (var neighbour in neighbours)
                {
                    var newCost = costSoFar[current] + edges[current][neighbour];
                    if (!costSoFar.ContainsKey(neighbour) || newCost < costSoFar[neighbour])
                    {
                        costSoFar[neighbour] = newCost;
                        cameFrom[neighbour] = current;
                        frontier.Enqueue(neighbour, newCost);
                    }
                }
                if (current.Equals(destinationNode)) break;
            }
            List<T> path = new List<T>();
            if (!cameFrom.ContainsKey(destinationNode))
                return path;

            path.Add(destinationNode);
            var temp = destinationNode;

            while (!cameFrom[temp].Equals(originNode))
            {
                var currentPathElement = cameFrom[temp];
                path.Add(currentPathElement);

                temp = currentPathElement;
            }
            Debug.Log("FindPath() level nodes on path: " + path.Count.ToString());
            return path;
        }
        private int Heuristic<T>(T a, T b) where T : IGraphNode
        {
            return a.GetDistance(b);
        }
        */
    }

}