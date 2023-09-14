using UnityEngine;

namespace Spriter2UnityDX
{
    [RequireComponent(typeof(SpriteRenderer))] [ExecuteInEditMode] [DisallowMultipleComponent] [AddComponentMenu("")]
    public class SortingOrderUpdater : MonoBehaviour
    {
        private int sor;
        private SpriteRenderer srenderer;
        private Transform trans;
        private float z_index = float.NaN;

        public int SpriteCount { get; set; }
        public int SortingOrder
        {
            get => sor;
            set
            {
                sor = value;
                UpdateSortingOrder();
            }
        }

        private void Awake()
        {
            trans = transform;
            srenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            float newZ = trans.localPosition.z;
            if (newZ != z_index)
            {
                z_index = newZ;
                UpdateSortingOrder();
            }
        }

        private void UpdateSortingOrder()
        {
            if (srenderer)
            {
                srenderer.sortingOrder = (int) (z_index * -1000) + sor - SpriteCount;
            }
        }
    }
}
