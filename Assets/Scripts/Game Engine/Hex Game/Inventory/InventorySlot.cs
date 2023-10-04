using UnityEngine;
using WeAreGladiators.TownFeatures;

namespace WeAreGladiators.Items
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private InventoryItemView myItemView;
        [SerializeField] private ItemShopSlot myArmouryItemView;

        public InventoryItemView MyItemView => myItemView;
        public ItemShopSlot MyArmouryItemView => myArmouryItemView;
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
