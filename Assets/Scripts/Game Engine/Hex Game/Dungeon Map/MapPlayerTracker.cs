﻿using System;
using System.Linq;
using DG.Tweening;
using WeAreGladiators.Utilities;
using UnityEngine;

namespace WeAreGladiators.DungeonMap
{
    public class MapPlayerTracker : Singleton<MapPlayerTracker>
    {
        // Components + Properties
        #region
        [Header("Component References")]
        public MapManager mapManager;
        public MapView view;

        [Header("Properties")]
        public float enterNodeDelay = 1f;  
        private bool locked = true;
        #endregion

        // Getters + Accessors
        #region
        public bool Locked 
        {
            get { return locked; }
            private set { locked = value; } 
        }
        #endregion

        public void LockMap()
        {
            Locked = true;
        }
        public void UnlockMap()
        {
            Locked = false;
        }
        public void SelectNode(MapNode mapNode)
        {
            if (Locked) return;
            if (mapManager.CurrentMap.path.Count == 0)
            {
                // player has not selected the node yet, he can select any of the nodes with y = 0
                if (mapNode.Node.point.y == 0)
                    SendPlayerToNode(mapNode);
            }
            else
            {
                var currentPoint = mapManager.CurrentMap.path[mapManager.CurrentMap.path.Count - 1];
                var currentNode = mapManager.CurrentMap.GetNode(currentPoint);

                if (currentNode != null && currentNode.outgoing.Any(point => point.Equals(mapNode.Node.point)))
                    SendPlayerToNode(mapNode);
            }
        }

        private void SendPlayerToNode(MapNode mapNode)
        {
            LockMap();

            mapManager.CurrentMap.path.Add(mapNode.Node.point);
            view.SetAttainableNodes();
            view.SetLineColors();

            DOTween.Sequence().AppendInterval(enterNodeDelay).OnComplete(() => HandleEnterNode(mapNode));
        }

        private static void HandleEnterNode(MapNode mapNode)
        {
            Debug.Log("Entering node: " + mapNode.Node.BlueprintName + " of type: " + mapNode.Node.NodeType);

            //GameController.Instance.HandleLoadNextEncounter(mapNode);

        }

    }
}