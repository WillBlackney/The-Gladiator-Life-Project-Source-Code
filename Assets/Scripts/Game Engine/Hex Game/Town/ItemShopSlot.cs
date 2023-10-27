using DG.Tweening;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Abilities;
using WeAreGladiators.Boons;
using WeAreGladiators.Items;
using WeAreGladiators.JourneyLogic;
using WeAreGladiators.Libraries;
using WeAreGladiators.Player;
using WeAreGladiators.UI;
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

        public ItemShopData MyItemData { get; private set; }
        #endregion

        #region Input
        private void HandleRightClickOrTap()
        {
            if (MyItemData == null) return;
            if (AbleToBuyItem())
            {
                ItemController.Instance.HandleBuyItem(MyItemData);
            }
        }
        public void OnTap()
        {
            if(!Application.isMobilePlatform) return;
            HandleRightClickOrTap();
        }
        public void MouseClick()
        {
            HandleRightClickOrTap();
        }
        public void MouseEnter()
        {
            if (MyItemData == null) return;
            if (MyItemData.Item != null)
            {
                ItemPopupController.Instance.OnShopItemMousedOver(this);
            }
            else if (MyItemData.ability != null)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(MyItemData.ability.keyWords);
                AbilityPopupController.Instance.OnAbilityShopTomeMousedOver(this);
            }

        }
        public void MouseExit()
        {
            if (MyItemData == null) return;
            if (MyItemData.Item != null)
            {
                ItemPopupController.Instance.HidePanel();
            }
            else if (MyItemData.ability != null)
            {
                KeyWordLayoutController.Instance.FadeOutMainView();
                AbilityPopupController.Instance.OnAbilityButtonMousedExit();
            }
        }

        #endregion

        #region Logic

        private bool AbleToBuyItem()
        {
            if (MyItemData == null) return false;

            bool ret = false;

            int cost = 100000;
            if (MyItemData.Item != null || MyItemData.ability != null) cost = MyItemData.GoldCost;
            else return false;

            if (cost <= RunController.Instance.CurrentGold &&
                InventoryController.Instance.HasFreeInventorySpace())
            {
                ret = true;
            }
            return ret;
        }
        public void BuildFromItemShopData(ItemShopData data)
        {
            //MyAbilityTomeData = null;
            MyItemData = data;

            gameObject.SetActive(true);   
            if(data.Item != null) itemImage.sprite = data.Item.ItemSprite;
            else if (data.ability != null) itemImage.sprite = SpriteLibrary.Instance.GetTalentSchoolBookSprite(data.ability.talentRequirementData.talentSchool);

            // Color cost text red if not enough gold, or green if selling at a discount
            string col = "<color=#FFFFFF>";
            
            if (data.Item != null && data.GoldCost < data.Item.baseGoldValue &&
                data.GoldCost <= RunController.Instance.CurrentGold)
            {
                col = TextLogic.lightGreen;
            }
            else if (RunController.Instance.CurrentGold < data.GoldCost)
            {
                col = TextLogic.lightRed;
            }
            goldCostText.text = TextLogic.ReturnColoredText(data.GoldCost.ToString(), col);
        }
        public void Reset()
        {
            gameObject.SetActive(false);
            MyItemData = null;
        }
       

        #endregion
    }

    public class ItemShopData
    {
        [OdinSerialize] private int baseGoldPrice;
        [OdinSerialize] private ItemData item;
        [OdinSerialize] public AbilityData ability;
        public ItemData Item => item;
        public int BaseGoldPrice => baseGoldPrice;
        public ItemShopData(ItemData item, int baseGoldCost)
        {
            ability = null;
            this.item = item;
            baseGoldPrice = baseGoldCost;
        }
        public ItemShopData(AbilityData ability, int baseGoldCost)
        {
            item = null;
            this.ability = ability;
            baseGoldPrice = baseGoldCost;
        }
        public int GoldCost
        {
            get
            {
                float priceMod = 1f;
                if (BoonController.Instance != null)
                {
                    if (BoonController.Instance.DoesPlayerHaveBoon(BoonTag.ArmourySurplus) &&
                        ability == null)
                    {
                        priceMod -= 0.5f;
                    }
                }

                return (int) (baseGoldPrice * priceMod);
            }
        }
        
    }

    /*
    public class AbilityTomeShopData
    {
        public AbilityData ability;
        public int goldCost;

        public AbilityTomeShopData(AbilityData ability, int goldCost)
        {
            this.ability = ability;
            this.goldCost = goldCost;
        }
    }*/
}
