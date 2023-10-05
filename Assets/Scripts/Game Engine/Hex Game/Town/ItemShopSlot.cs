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
        public AbilityTomeShopData MyAbilityTomeData { get; private set; }
        #endregion

        #region Input

        public void MouseClick()
        {
            if (AbleToBuyItem())
            {
                if(MyItemData != null)
                {
                    TownController.Instance.HandleBuyItemFromArmoury(MyItemData);
                }
                else if (MyAbilityTomeData != null)
                {
                    TownController.Instance.HandleBuyAbilityTomeFromLibrary(MyAbilityTomeData);
                }
               
            }
        }
        public void MouseEnter()
        {
            if (MyItemData != null)
            {
                ItemPopupController.Instance.OnShopItemMousedOver(this);
            }
            else if (MyAbilityTomeData != null)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(MyAbilityTomeData.ability.keyWords);
                AbilityPopupController.Instance.OnAbilityShopTomeMousedOver(this);
            }

        }
        public void MouseExit()
        {
            if (MyItemData != null)
            {
                ItemPopupController.Instance.HidePanel();
            }
            else if (MyAbilityTomeData != null)
            {
                KeyWordLayoutController.Instance.FadeOutMainView();
                AbilityPopupController.Instance.OnAbilityButtonMousedExit();
            }
        }

        #endregion

        #region Logic

        private bool AbleToBuyItem()
        {
            bool ret = false;

            int cost = 100000;
            if (MyItemData != null) cost = MyItemData.GoldCost;
            else if (MyAbilityTomeData != null) cost = MyAbilityTomeData.goldCost;
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
            MyAbilityTomeData = null;
            MyItemData = data;

            gameObject.SetActive(true);            
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
        public void BuildFromTomeShopData(AbilityTomeShopData data)
        {
            MyAbilityTomeData = data;
            MyItemData = null;

            gameObject.SetActive(true);
            itemImage.sprite = SpriteLibrary.Instance.GetTalentSchoolBookSprite(data.ability.talentRequirementData.talentSchool);

            // Color cost text red if not enough gold, or green if selling at a discount
            string col = "<color=#FFFFFF>";
            if (RunController.Instance.CurrentGold < data.goldCost)
            {
                col = TextLogic.lightRed;
            }
            goldCostText.text = TextLogic.ReturnColoredText(data.goldCost.ToString(), col);

        }
        public void Reset()
        {
            gameObject.SetActive(false);
            MyItemData = null;
            MyAbilityTomeData = null;
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

    public class AbilityTomeShopData
    {
        public AbilityData ability;
        public int goldCost;

        public AbilityTomeShopData(AbilityData ability, int goldCost)
        {
            this.ability = ability;
            this.goldCost = goldCost;
        }
    }
}
