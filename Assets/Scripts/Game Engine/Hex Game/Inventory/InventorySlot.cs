using UnityEngine;
using WeAreGladiators.RewardSystems;
using WeAreGladiators.TownFeatures;

namespace WeAreGladiators.Items
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private InventoryItemView myItemView;
        [SerializeField] private ItemShopSlot myArmouryItemView;
        [SerializeField] private CombatLootIcon mylootIconView;

        public InventoryItemView MyItemView => myItemView;
        public ItemShopSlot MyArmouryItemView => myArmouryItemView;
        public CombatLootIcon MylootIconView => mylootIconView;
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
