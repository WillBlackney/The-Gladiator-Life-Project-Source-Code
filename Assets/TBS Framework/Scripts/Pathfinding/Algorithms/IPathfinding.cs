using System.Collections.Generic;
using System.Linq;
using TbsFramework.Pathfinding.DataStructs;
using UnityEngine;

namespace TbsFramework.Pathfinding.Algorithms
{
    public abstract class IPathfinding
    {
        /// <summary>
        /// Method finds path between two nodes in a graph.
        /// </summary>
        /// <param name="edges">
        /// Graph edges represented as nested dictionaries. Outer dictionary contains all nodes in the graph, inner dictionary contains 
        /// its neighbouring nodes with edge weight.
        /// </param>
        /// <returns>
        /// If a path exist, method returns list of nodes that the path consists of. Otherwise, empty list is returned.
        /// </returns>
        public abstract List<T> FindPath<T>(Dictionary<T, Dictionary<T, float>> edges, T originNode, T destinationNode) where T : IGraphNode;
                
        protected List<T> GetNeigbours<T>(Dictionary<T, Dictionary<T, float>> edges, T node)
        {

            if (!edges.ContainsKey(node))
            {
                //Debug.Log("IPathfinding.GetNeigbours() neighbours found no neighbours");
                return new List<T>();
            }
            //Debug.Log("IPathfinding.GetNeigbours() neighbours found: " + edges[node].Keys.ToList().Count.ToString());
            return edges[node].Keys.ToList();
        }
        
    }
}