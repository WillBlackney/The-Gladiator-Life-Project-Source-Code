using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TbsFramework.Grid;
using TbsFramework.Cells;

namespace TbsFramework.Example1
{
    public class RandomObstacleGenerator : MonoBehaviour
    {
        private System.Random _rnd = new System.Random();

        public int Amount;
        public Transform ObstaclesParent;
        public GameObject ObstaclePrefab;

        public CellGrid CellGrid;

        public void Start()
        {
            StartCoroutine(SpawnObstacles());
        }

        public IEnumerator SpawnObstacles()
        {
            while (CellGrid.Cells == null)
            {
                yield return 0;
            }

            var cells = CellGrid.Cells;

            List<GameObject> ret = new List<GameObject>();

            if (ObstaclesParent.childCount != 0)
            {
                for (int i = 0; i < ObstaclesParent.childCount; i++)
                {
                    var obstacle = ObstaclesParent.GetChild(i);
                    var bounds = getBounds(obstacle);

                    var cell = cells.OrderBy(h => Math.Abs((h.transform.position - obstacle.transform.position).magnitude)).First();
                    if (!cell.IsTaken)
                    {
                        cell.IsTaken = true;
                        obstacle.localPosition = cell.transform.localPosition + new Vector3(0, bounds.y, 0);
                    }
                    else
                    {
                        Destroy(obstacle.gameObject);
                    }
                }
            }

            List<Cell> freeCells = cells.FindAll(h => h.GetComponent<Cell>().IsTaken == false);
            freeCells = freeCells.OrderBy(h => _rnd.Next()).ToList();

            for (int i = 0; i < Mathf.Clamp(Amount, Amount, freeCells.Count); i++)
            {
                var cell = freeCells.ElementAt(i);
                cell.GetComponent<Cell>().IsTaken = true;

                var obstacle = Instantiate(ObstaclePrefab);
                obstacle.transform.parent = ObstaclesParent.transform;
                obstacle.transform.rotation = cell.transform.rotation;
                obstacle.transform.localPosition = cell.transform.localPosition + new Vector3(0, cell.GetCellDimensions().y, 0);
                ret.Add(obstacle);
            }
        }

        private Vector3 getBounds(Transform transform)
        {
            var renderer = transform.GetComponent<Renderer>();
            var combinedBounds = renderer.bounds;
            var renderers = transform.GetComponentsInChildren<Renderer>();
            foreach (var childRenderer in renderers)
            {
                if (childRenderer != renderer) combinedBounds.Encapsulate(childRenderer.bounds);
            }

            return combinedBounds.size;
        }
    }
}
