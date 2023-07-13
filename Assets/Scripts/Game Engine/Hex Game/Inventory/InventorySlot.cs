using UnityEngine;

namespace WeAreGladiators.Items
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] InventoryItemView myItemView;

        public InventoryItemView MyItemView
        {
            get { return myItemView; }
        }
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