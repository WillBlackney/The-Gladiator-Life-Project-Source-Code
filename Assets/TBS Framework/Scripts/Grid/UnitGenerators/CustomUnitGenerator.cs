using System;
using System.Collections.Generic;
using System.Linq;
using TbsFramework.Cells;
using TbsFramework.Units;
using UnityEngine;

namespace TbsFramework.Grid.UnitGenerators
{
    public class CustomUnitGenerator : MonoBehaviour, IUnitGenerator
    {
        public Transform UnitsParent;
        public Transform CellsParent;

        /// <summary>
        /// Returns units that are already children of UnitsParent object.
        /// </summary>
        public List<Unit> SpawnUnits(List<Cell> cells)
        {
            List<Unit> ret = new List<Unit>();
            for (int i = 0; i < UnitsParent.childCount; i++)
            {
                var unit = UnitsParent.GetChild(i).GetComponent<Unit>();
                if (unit != null)
                {
                    var cell = cells.OrderBy(h => Math.Abs((h.transform.position - unit.transform.position).magnitude)).First();
                    {
                        cell.IsTaken = true;
                        unit.Cell = cell;
                        cell.CurrentUnit = unit;
                        unit.transform.position = cell.transform.position;
                        unit.Initialize();
                        ret.Add(unit);
                    }//Unit gets snapped to the nearest cell
                }
                else
                {
                    Debug.LogError("Invalid object in Units Parent game object");
                }

            }
            return ret;
        }

        /// <summary>
        /// Snaps unit objects to the nearest cell.
        /// </summary>
        public void SnapToGrid()
        {
            List<Transform> cells = new List<Transform>();

            foreach (Transform cell in CellsParent)
            {
                cells.Add(cell);
            }

            foreach (Transform unit in UnitsParent)
            {
                var closestCell = cells.OrderBy(h => Math.Abs((h.transform.position - unit.transform.position).magnitude)).First();
                if (!closestCell.GetComponent<Cell>().IsTaken)
                {
                    Vector3 offset = new Vector3(0, closestCell.GetComponent<Cell>().GetCellDimensions().y, 0);
                    unit.localPosition = closestCell.transform.localPosition + offset;
                }//Unit gets snapped to the nearest cell
            }
        }
    }
}
