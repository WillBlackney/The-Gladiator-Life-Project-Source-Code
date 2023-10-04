using DG.Tweening;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Boons;
using WeAreGladiators.Items;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Player;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.TownFeatures
{
    public class ItemShopSlot : MonoBehaviour
    {
        #region Properties + Components

        [Header("Components")]
        [SerializeField] private TextMeshProUGUI goldCostText;
        [SerializeField] private Image itemImage;


        #endregion

        #region Getters + Accessors

        public ItemShopData MyData { get; private set; }

        #endregion

        #region Input

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

        #region Logic

        private bool AbleToBuy()
        {
            bool ret = false;

            if (MyData.GoldCost <= RunController.Instance.CurrentGold &&
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
            itemImage.sprite = data.Item.ItemSprite;

            // Color cost text red if not enough gold, or green if selling at a discount
            string col = "<color=#FFFFFF>";
            if (RunController.Instance.CurrentGold < data.GoldCost)
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
       

        #endregion
    }

    public class ItemShopData
    {
        [OdinSerialize] private int baseGoldCost;
        [OdinSerialize] private ItemData item;

        public ItemData Item => item;
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
        
    }
}
