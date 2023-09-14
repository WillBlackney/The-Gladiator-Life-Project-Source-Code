using UnityEngine;

namespace WeAreGladiators.Items
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private InventoryItemView myItemView;

        public InventoryItemView MyItemView => myItemView;
        public void Reset()
        {
            gameObject.SetActive(false);
        }
        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
