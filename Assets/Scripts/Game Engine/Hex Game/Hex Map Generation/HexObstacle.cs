using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.HexTiles
{

    public class HexObstacle : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        private SpriteRenderer[] obstacleSprites;

        public Hex MyHex { get; private set; }
        public SpriteRenderer MySR { get; private set; }

        public void SetMyHex(Hex h)
        {
            MyHex = h;
        }

        public void RandomizeObstacleSprite()
        {
            SpriteRenderer sr = obstacleSprites[RandomGenerator.NumberBetween(0, obstacleSprites.Length - 1)];
            MySR = sr;
            sr.gameObject.SetActive(true);
        }
    }
}
