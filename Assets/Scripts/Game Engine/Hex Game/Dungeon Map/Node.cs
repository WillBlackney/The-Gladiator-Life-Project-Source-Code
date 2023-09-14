using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WeAreGladiators.DungeonMap
{
    public class Node
    {
        public readonly List<Point> incoming = new List<Point>();
        public readonly List<Point> outgoing = new List<Point>();
        public readonly Point point;
        public Vector2 position;
        public Node(EncounterType nodeType, string blueprintName, Point point)
        {
            NodeType = nodeType;
            BlueprintName = blueprintName;
            this.point = point;
        }

        public EncounterType NodeType { get; private set; }
        public string BlueprintName { get; private set; }
        public void RerollType(EncounterType type, string blueprintName)
        {
            NodeType = type;
            BlueprintName = blueprintName;
        }

        public void AddIncoming(Point p)
        {
            if (incoming.Any(element => element.Equals(p)))
            {
                return;
            }

            incoming.Add(p);
        }

        public void AddOutgoing(Point p)
        {
            if (outgoing.Any(element => element.Equals(p)))
            {
                return;
            }

            outgoing.Add(p);
        }

        public void RemoveIncoming(Point p)
        {
            incoming.RemoveAll(element => element.Equals(p));
        }

        public void RemoveOutgoing(Point p)
        {
            outgoing.RemoveAll(element => element.Equals(p));
        }

        public bool HasNoConnections()
        {
            return incoming.Count == 0 && outgoing.Count == 0;
        }
    }
}
