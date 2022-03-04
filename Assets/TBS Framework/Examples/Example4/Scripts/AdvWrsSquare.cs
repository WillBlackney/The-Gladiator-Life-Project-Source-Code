using TbsFramework.Cells;
using UnityEngine;

namespace TbsFramework.Example4
{
    public class AdvWrsSquare : Square
    {
        public string TileType;
        public int DefenceBoost;

        Vector3 dimensions = new Vector3(1.6f, 1.6f, 0f);

        public override Vector3 GetCellDimensions()
        {
            return dimensions;
        }

        public override void MarkAsReachable()
        {
            SetColor(new Color(0.4f, 0.7f, 1f, 0.8f));
        }
        public override void MarkAsPath()
        {
            SetColor(new Color(0, 1, 0, 0.5f));
        }
        public override void MarkAsHighlighted()
        {
            SetColor(new Color(0.8f, 0.8f, 0.8f, 0.5f));
        }
        public override void UnMark()
        {
            SetColor(new Color(1, 1, 1, 0));
        }

        private void SetColor(Color color)
        {
            var highlighter = transform.Find("marker");
            var spriteRenderer = highlighter.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }
    }
}
