using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Abilities;
using TMPro;
using UnityEngine.UI;
using HexGameEngine.Libraries;
using HexGameEngine.Player;
using HexGameEngine.Utilities;
using HexGameEngine.UI;
using HexGameEngine.Items;

namespace HexGameEngine.TownFeatures
{
    public class ItemShopSlot : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Components")]
        [SerializeField] TextMeshProUGUI itemNameText;
        [SerializeField] TextMeshProUGUI goldCostText;
        [SerializeField] Image itemImage;

        // Non inspector fields
        private ItemShopData myData;
        #endregion

        // Getters + Accessors
        #region
        public ItemShopData MyData
        {
            get { return myData; }
        }
        #endregion

        // Input
        #region
        public void MouseClick()
        {            
            if (AbleToBuy())
                TownController.Instance.HandleBuyItemFromLibrary(MyData);            
        }
        public void MouseEnter()
        {            
            if (myData != null)
            {
                //ItemPopupController.Instance.OnInventoryItemMousedOver(this);
            }
            
        }
        public void MouseExit()
        {
            
            if (myData != null)
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

            if (MyData.goldCost <= PlayerDataController.Instance.CurrentGold &&
                InventoryController.Instance.HasFreeInventorySpace())
                ret = true;
            return ret;
        }
        public void BuildFromItemShopData(ItemShopData data)
        {
            gameObject.SetActive(true);
            myData = data;
            itemNameText.text = data.item.itemName;
            itemImage.sprite = data.item.ItemSprite;

            // Color cost text red if not enough gold
            string col = "<color=#FFFFFF>";
            if (PlayerDataController.Instance.CurrentGold < data.goldCost) col = TextLogic.lightRed;
            goldCostText.text = TextLogic.ReturnColoredText(data.goldCost.ToString(), col);
        }
        public void Reset()
        {
            gameObject.SetActive(false);
            myData = null;
        }
        #endregion
    }

    public class ItemShopData
    {
        public ItemData item;
        public int goldCost;

        public ItemShopData(ItemData item, int goldCost)
        {
            this.item = item;
            this.goldCost = goldCost;
        }
    }
}