using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Abilities;
using WeAreGladiators.Items;
using WeAreGladiators.Libraries;
using WeAreGladiators.Player;
using WeAreGladiators.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.TownFeatures
{
    public class AbilityTomeShopSlot : MonoBehaviour
    {

        // Getters + Accessors
        #region

        public AbilityTomeShopData MyData { get; private set; }

        #endregion
        // Properties + Components
        #region

        [Header("Components")]
        [SerializeField]
        private TextMeshProUGUI abilityNameText;
        [SerializeField] private TextMeshProUGUI goldCostText;
        [SerializeField] private Image abilityBookImage;
        [SerializeField] private Transform scaleParent;

        // Non inspector fields

        #endregion

        // Input
        #region

        public void MouseClick()
        {
            if (AbleToBuy())
            {
                TownController.Instance.HandleBuyAbilityTomeFromLibrary(MyData);
            }
        }
        public void MouseEnter()
        {
            if (MyData != null)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(MyData.ability.keyWords);
                AbilityPopupController.Instance.OnAbilityShopTomeMousedOver(this);
            }
        }
        public void MouseExit()
        {
            if (MyData != null)
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
            {
                ret = true;
            }
            return ret;
        }
        public void BuildFromTomeShopData(AbilityTomeShopData data)
        {
            gameObject.SetActive(true);
            MyData = data;
            abilityNameText.text = data.ability.displayedName;
            abilityBookImage.sprite = SpriteLibrary.Instance.GetTalentSchoolBookSprite(data.ability.talentRequirementData.talentSchool);

            // Color cost text red if not enough gold
            string col = TextLogic.brownBodyText;
            if (PlayerDataController.Instance.CurrentGold < data.goldCost)
            {
                col = TextLogic.redText;
            }
            goldCostText.text = TextLogic.ReturnColoredText(data.goldCost.ToString(), col);
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
