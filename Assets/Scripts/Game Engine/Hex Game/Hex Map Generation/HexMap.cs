using System.Collections;
using System.Collections.Generic;
using TbsFramework.Cells;
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
        public List<Hex> Hexs { get; private set; }
        public HexMapSeedDataSO Seed { get; private set; }
        public HexMapShape Shape
        {
            get { return Seed.shape; }
        }
        public Hex CentreHex()
        {
            if (centreHex) return centreHex;

            Hex hRet = null;
            foreach(Hex h in Hexs)
            {
                if(h.GridPosition.x == 0 && h.GridPosition.y == 0)
                {
                    centreHex = h;
                    hRet = h;
                    break;
                }
            }
            return hRet;
        }
        public Vector2 WorldCentre
        {
            get { return CentreHex().WorldPosition; }
        }
        public HexMap(Vector3 dimensions, List<Hex> hexs, HexMapSeedDataSO seed)
        {
            Dimensions = dimensions;
            Hexs = hexs;
            Seed = seed;
        }
        #endregion
    }
}

