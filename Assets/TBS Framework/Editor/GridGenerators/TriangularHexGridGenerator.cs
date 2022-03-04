using UnityEngine;
using System.Collections.Generic;
using TbsFramework.Cells;
using UnityEditor;

namespace TbsFramework.EditorUtils.GridGenerators
{
    /// <summary>
    /// Generates triangle shaped grid of hexagons.
    /// </summary>
    [ExecuteInEditMode()]
    public class TriangularHexGridGenerator : ICellGridGenerator
    {
        public GameObject HexagonPrefab;
        public int Side;

        public override GridInfo GenerateGrid()
        {
            List<Cell> hexagons = new List<Cell>();

            if (HexagonPrefab.GetComponent<Hexagon>() == null)
            {
                Debug.LogError("Invalid hexagon prefab provided");
                return null;
            }

            for (int i = 0; i < Side; i++)
            {
                for (int j = 0; j < Side - i; j++)
                {
                    GameObject hexagon = PrefabUtility.InstantiatePrefab(HexagonPrefab) as GameObject;
                    var hexSize = hexagon.GetComponent<Cell>().GetCellDimensions();

                    hexagon.transform.position = new Vector3((i * hexSize.x * 0.75f), (i * hexSize.y * 0.5f) + (j * hexSize.y), 0);
                    hexagon.GetComponent<Hexagon>().OffsetCoord = new Vector2(i, Side - j - 1 - (i / 2));
                    hexagon.GetComponent<Hexagon>().HexGridType = HexGridType.odd_q;
                    hexagon.GetComponent<Hexagon>().MovementCost = 1;
                    hexagons.Add(hexagon.GetComponent<Cell>());

                    hexagon.transform.parent = CellsParent;
                }
            }

            var hexDimensions = HexagonPrefab.GetComponent<Cell>().GetCellDimensions();
            var hexSide = hexDimensions.x / 2;

            GridInfo gridInfo = new GridInfo();
            gridInfo.Cells = hexagons;
            gridInfo.Dimensions = new Vector3((hexDimensions.x * (Side - 1) * Mathf.Sqrt(3) / 2), hexDimensions.y * (Side - 1), hexDimensions.z);
            gridInfo.Center = gridInfo.Dimensions / 2;

            return gridInfo;
        }
    }
}