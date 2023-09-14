using DG.Tweening;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Boons;
using WeAreGladiators.Items;
using WeAreGladiators.Player;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.TownFeatures
{
    public class ItemShopSlot : MonoBehaviour
    {

        // Getters + Accessors
        #region

        public ItemShopData MyData { get; private set; }

        #endregion
        // Properties + Components
        #region

        [Header("Components")]
        [SerializeField]
        private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI goldCostText;
        [SerializeField] private Image itemImage;
        [SerializeField] private Transform scaleParent;

        // Non inspector fields

        #endregion

        // Input
        #region

        public void MouseClick()
        {
            if (AbleToBuy())
            {
                TownController.Instance.HandleBuyItemFromArmoury(MyData);
            }
        }
        public void MouseEnter()
        {
            if (MyData != null)
            {
                ItemPopupController.Instance.OnShopItemMousedOver(this);
            }
        }
        public void MouseExit()
        {
            if (MyData != null)
            {
                ItemPopupController.Instance.HidePanel();
            }
        }

        #endregion

        // Logic
        #region

        private bool AbleToBuy()
        {
            bool ret = false;

            if (MyData.GoldCost <= PlayerDataController.Instance.CurrentGold &&
                InventoryController.Instance.HasFreeInventorySpace())
            {
                ret = true;
            }
            return ret;
        }
        public void BuildFromItemShopData(ItemShopData data)
        {
            gameObject.SetActive(true);
            MyData = data;
            itemNameText.text = data.Item.itemName;
            itemImage.sprite = data.Item.ItemSprite;

            // Color cost text red if not enough gold, or green if selling at a discount
            string col = "<color=#FFFFFF>";
            if (PlayerDataController.Instance.CurrentGold < data.GoldCost)
            {
                col = TextLogic.lightRed;
            }
            else if (data.GoldCost < data.Item.baseGoldValue)
            {
                col = TextLogic.lightGreen;
            }
            goldCostText.text = TextLogic.ReturnColoredText(data.GoldCost.ToString(), col);
        }
        public void Reset()
        {
            gameObject.SetActive(false);
            MyData = null;
        }
        public void Enlarge()
        {
            // Calculate enlargement scale and convert it to to a vector 3
            Vector3 endScale = new Vector3(1.15f, 1.15f, 1f);

            // Scale the transform to its new size
            scaleParent.DOKill();
            scaleParent.DOScale(endScale, 0.2f);
        }
        public void Shrink(float speed = 0.2f)
        {
            // Calculate enlargement scale and convert it to to a vector 3
            Vector3 endScale = new Vector3(1f, 1f, 1f);

            // Scale the transform to its new size
            scaleParent.DOKill();
            scaleParent.DOScale(endScale, speed);
        }

        #endregion
    }

    public class ItemShopData
    {
        [OdinSerialize]
        private int baseGoldCost;
        [OdinSerialize]
        private ItemData item;

        public ItemShopData(ItemData item, int baseGoldCost)
        {
            this.item = item;
            this.baseGoldCost = baseGoldCost;
        }

        public int GoldCost
        {
            get
            {
                float priceMod = 1f;
                if (BoonController.Instance != null)
                {
                    if (BoonController.Instance.DoesPlayerHaveBoon(BoonTag.ArmourySurplus))
                    {
                        priceMod -= 0.5f;
                    }
                }

                return (int) (baseGoldCost * priceMod);
            }
        }
        public ItemData Item => item;
    }
}
