using UnityEngine;

namespace WeAreGladiators.DungeonMap
{
    public class DottedLineRenderer : MonoBehaviour
    {
        public bool scaleInUpdate;
        private LineRenderer lR;
        private Renderer rend;

        private void Start()
        {
            ScaleMaterial();
            enabled = scaleInUpdate;
        }

        private void Update()
        {
            rend.material.mainTextureScale =
                new Vector2(Vector2.Distance(lR.GetPosition(0), lR.GetPosition(lR.positionCount - 1)) / lR.widthMultiplier,
                    1);
        }

        public void ScaleMaterial()
        {
            lR = GetComponent<LineRenderer>();
            rend = GetComponent<Renderer>();
            rend.material.mainTextureScale =
                new Vector2(Vector2.Distance(lR.GetPosition(0), lR.GetPosition(lR.positionCount - 1)) / lR.widthMultiplier,
                    1);
        }
    }
}
