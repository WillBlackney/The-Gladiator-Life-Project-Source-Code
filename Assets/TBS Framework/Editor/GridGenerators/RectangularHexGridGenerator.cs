using System.Collections.Generic;
using TbsFramework.Cells;
using UnityEditor;
using UnityEngine;

namespace TbsFramework.EditorUtils.GridGenerators
{
    /// <summary>
    /// Generates rectangular shaped grid of hexagons.
    /// </summary>
    [ExecuteInEditMode()]
    class RectangularHexGridGenerator : ICellGridGenerator
    {
        #pragma warning disable 0649
        public GameObject HexagonPrefab;
        public int Width;
        public int Height;
        #pragma warning restore 0649

        public override GridInfo GenerateGrid()
        {
            HexGridType hexGridType = Width % 2 == 0 ? HexGridType.even_q : HexGridType.odd_q;
            List<Cell> hexagons = new List<Cell>();

            if (HexagonPrefab.GetComponent<Hexagon>() == null)
            {
                Debug.LogError("Invalid hexagon prefab provided");
                return null;
            }

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    GameObject hexagon = PrefabUtility.InstantiatePrefab(HexagonPrefab) as GameObject;
                    var hexSize = hexagon.GetComponent<Cell>().GetCellDimensions();

                    hexagon.transform.position = new Vector3((j * hexSize.x * 0.75f), (i * hexSize.y) + (j % 2 == 0 ? 0 : hexSize.y * 0.5f), 0);
                    hexagon.GetComponent<Hexagon>().OffsetCoord = new Vector2(Width - j - 1, Height - i - 1);
                    hexagon.GetComponent<Hexagon>().HexGridType = hexGridType;
                    hexagon.GetComponent<Hexagon>().MovementCost = 1;
                    hexagons.Add(hexagon.GetComponent<Cell>());

                    hexagon.transform.parent = CellsParent;
                }
            }
            var hexDimensions = HexagonPrefab.GetComponent<Cell>().GetCellDimensions();
            var hexSide = hexDimensions.x / 2;

            GridInfo gridInfo = new GridInfo();
            gridInfo.Cells = hexagons;

            gridInfo.Dimensions = new Vector3(hexSide * (Width - 1) * 1.5f, hexDimensions.y * (Height - 0.5f), hexDimensions.z);
            gridInfo.Center = gridInfo.Dimensions / 2;

            return gridInfo;
        }
    }
}