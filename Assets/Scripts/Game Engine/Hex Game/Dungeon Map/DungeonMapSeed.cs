using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.DungeonMap
{
    [CreateAssetMenu]
    public class DungeonMapSeed : ScriptableObject
    {
        public List<NodeBlueprint> nodeBlueprints;
        public int GridWidth => Mathf.Max(numOfPreBossNodes.max, numOfStartingNodes.max);

        public IntMinMax numOfPreBossNodes;
        public IntMinMax numOfStartingNodes;

        public MapLayer[] layers;
    }
}