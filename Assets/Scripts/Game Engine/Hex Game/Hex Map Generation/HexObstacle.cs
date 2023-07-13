using WeAreGladiators.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.HexTiles
{


    public class HexObstacle : MonoBehaviour
    {
        [Header("Components")]       
        [SerializeField] SpriteRenderer[] obstacleSprites;
        SpriteRenderer mySR;

        private Hex myHex;

        public Hex MyHex
        {
            get { return myHex; }
        }
        public SpriteRenderer MySR
        {
            get { return mySR; }
        }

        public void SetMyHex(Hex h)
        {
            myHex = h;
        }

        public void RandomizeObstacleSprite()
        {
            SpriteRenderer sr = obstacleSprites[RandomGenerator.NumberBetween(0, obstacleSprites.Length - 1)];
            mySR = sr;
            sr.gameObject.SetActive(true);
        }
    }
}