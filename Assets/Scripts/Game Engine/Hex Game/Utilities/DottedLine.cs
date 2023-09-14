using System.Collections.Generic;
using UnityEngine;

namespace WeAreGladiators.Utilities
{
    public class DottedLine : Singleton<DottedLine>
    {
        // Inspector fields
        public GameObject ArrowPrefab;
        [Range(0.01f, 1f)]
        public float Size;
        [Range(0.1f, 2f)]
        public float Delta;

        // Utility fields
        private readonly List<GameObject> dots = new List<GameObject>();

        private void DrawDottedLine(Vector2 start, Vector2 end)
        {
            Vector2 point = start;
            Vector2 direction = (end - start).normalized;
            List<Vector2> positions = new List<Vector2>();

            while ((end - start).magnitude > (point - start).magnitude)
            {
                //if(!positions.Contains(point))
                positions.Add(point);
                point += direction * Delta;
            }

            Render(start, end, positions);
        }
        public void DrawPathAlongPoints(List<Vector2> points)
        {
            DestroyAllPaths();
            for (int i = 0; i < points.Count - 1; i++)
            {
                DrawDottedLine(points[i], points[i + 1]);
            }
        }
        public void DestroyAllPaths()
        {
            foreach (GameObject dot in dots)
            {
                Destroy(dot);
            }

            dots.Clear();
        }

        private GameObject GetOneArrow()
        {
            GameObject gameObject = Instantiate(ArrowPrefab);
            gameObject.transform.localScale = Vector3.one * Size;
            gameObject.transform.parent = transform;
            return gameObject;
        }

        private void Render(Vector2 start, Vector2 end, List<Vector2> renderPositions)
        {
            Vector2 dir = end - start;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            foreach (Vector2 position in renderPositions)
            {
                // var g = GetOneDot();
                GameObject g = GetOneArrow();
                g.transform.position = position;
                g.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                dots.Add(g);
            }

            if (dots.Count > 1)
            {
                Destroy(dots[0].gameObject);
            }

        }
    }

}
