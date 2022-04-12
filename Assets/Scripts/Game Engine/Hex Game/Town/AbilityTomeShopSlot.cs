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
    public class AbilityTomeShopSlot : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Components")]
        [SerializeField] TextMeshProUGUI abilityNameText;
        [SerializeField] TextMeshProUGUI goldCostText;
        [SerializeField] Image abilityBookImage;

        // Non inspector fields
        private AbilityTomeShopData myData;
        #endregion

        // Getters + Accessors
        #region
        public AbilityTomeShopData MyData
        {
            get { return myData; }
        }
        #endregion

        // Input
        #region
        public void MouseClick()
        {
            if (AbleToBuy())            
                TownController.Instance.HandleBuyAbilityTomeFromLibrary(MyData);            
        }
        public void MouseEnter()
        {
            if (myData != null)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(myData.ability.keyWords);
                AbilityPopupController.Instance.OnAbilityShopTomeMousedOver(this);
            }
        }
        public void MouseExit()
        {
            if (myData != null)
            {
                KeyWordLayoutController.Instance.FadeOutMainView();
                AbilityPopupController.Instance.OnAbilityButtonMousedExit();
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
        public void BuildFromTomeShopData(AbilityTomeShopData data)
        {
            gameObject.SetActive(true);
            myData = data;
            abilityNameText.text = data.ability.abilityName;
            abilityBookImage.sprite = SpriteLibrary.Instance.GetTalentSchoolBookSprite(data.ability.talentRequirementData.talentSchool);

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

    public class AbilityTomeShopData
    {
        public AbilityData ability;
        public int goldCost;

        public AbilityTomeShopData (AbilityData ability, int goldCost)
        {
            this.ability = ability;
            this.goldCost = goldCost;
        }
    }
}