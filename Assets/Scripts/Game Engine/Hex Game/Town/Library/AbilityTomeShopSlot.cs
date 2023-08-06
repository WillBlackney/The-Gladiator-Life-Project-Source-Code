using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Abilities;
using TMPro;
using UnityEngine.UI;
using WeAreGladiators.Libraries;
using WeAreGladiators.Player;
using WeAreGladiators.Utilities;
using WeAreGladiators.UI;
using WeAreGladiators.Items;
using DG.Tweening;

namespace WeAreGladiators.TownFeatures
{
    public class AbilityTomeShopSlot : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Components")]
        [SerializeField] TextMeshProUGUI abilityNameText;
        [SerializeField] TextMeshProUGUI goldCostText;
        [SerializeField] Image abilityBookImage;
        [SerializeField] Transform scaleParent;

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
            abilityNameText.text = data.ability.displayedName;
            abilityBookImage.sprite = SpriteLibrary.Instance.GetTalentSchoolBookSprite(data.ability.talentRequirementData.talentSchool);

            // Color cost text red if not enough gold
            string col = TextLogic.brownBodyText;
            if (PlayerDataController.Instance.CurrentGold < data.goldCost) col = TextLogic.redText;
            goldCostText.text = TextLogic.ReturnColoredText(data.goldCost.ToString(), col);
        }
        public void Reset()
        {
            gameObject.SetActive(false);
            myData = null;
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