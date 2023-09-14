using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.HexTiles
{
    public class HexMap
    {
        // Properties
        private Hex centreHex;

        // Getters + Accesors
        #region

        public Vector3 Dimensions { get; private set; }
        public List<Hex> Hexs { get; }
        public HexMapSeedDataSO Seed { get; }
        public HexMapShape Shape => Seed.shape;
        public Hex CentreHex()
        {
            if (centreHex)
            {
                return centreHex;
            }

            Hex hRet = null;
            foreach (Hex h in Hexs)
            {
                if (h.GridPosition.x == 0 && h.GridPosition.y == 0)
                {
                    centreHex = h;
                    hRet = h;
                    break;
                }
            }
            return hRet;
        }
        public Vector2 WorldCentre => CentreHex().WorldPosition;
        public HexMap(Vector3 dimensions, List<Hex> hexs, HexMapSeedDataSO seed)
        {
            Dimensions = dimensions;
            Hexs = hexs;
            Seed = seed;
        }

        #endregion
    }
}
