using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HexGameEngine.Abilities;
using HexGameEngine.Items;
using HexGameEngine.Libraries;
using HexGameEngine.UI;

namespace HexGameEngine.RewardSystems
{
    public class CombatLootIcon : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Gold Components")]
        [SerializeField] private TextMeshProUGUI goldAmountText;
        [SerializeField] private GameObject goldParent;

        [Header("Ability Book Components")]
        [SerializeField] private GameObject abilityBookParent;
        [SerializeField] private Image abilityBookImage;

        [Header("Item Components")]
        [SerializeField] private GameObject itemParent;
        [SerializeField] private Image itemImage;
        [SerializeField] private Image itemRarityOverlayImage;

        // Non inspector fields
        private AbilityData abilityReward;
        private ItemData itemReward;
        private int goldReward;
        #endregion

        // Getters + Accessors
        #region
        public ItemData ItemReward
        {
            get { return itemReward; }
        }
        public AbilityData AbilityReward
        {
            get { return abilityReward; }
        }
        #endregion

        // Input 
        #region
        public void MouseEnter()
        {
            if(itemReward != null)
                ItemPopupController.Instance.OnCombatItemLootIconMousedOver(this);
            else if (abilityReward != null)
                AbilityPopupController.Instance.OnCombatAbilityLootIconMousedOver(this);

            // to do in future: gold info pop up
        }
        public void MouseExit()
        {
            ItemPopupController.Instance.HidePanel();
            AbilityPopupController.Instance.HidePanel();
        }
        #endregion

        // Logic
        #region
        public void Reset()
        {
            abilityReward = null;
            itemReward = null;
            goldReward = 0;

            gameObject.SetActive(false);
            goldParent.SetActive(false);
            abilityBookParent.SetActive(false);
            itemParent.SetActive(false);
            
        }
        public void BuildAsItemReward(ItemData item)
        {
            gameObject.SetActive(true);
            itemReward = item;
            itemParent.SetActive(true);
            itemImage.sprite = item.ItemSprite;
            itemRarityOverlayImage.color = ColorLibrary.Instance.GetRarityColor(item.rarity);
        }
        public void BuildAsAbilityReward(AbilityData ability)
        {
            gameObject.SetActive(true);
            abilityReward = ability;
            abilityBookParent.SetActive(true);
            abilityBookImage.sprite = SpriteLibrary.Instance.GetTalentSchoolBookSprite(ability.talentRequirementData.talentSchool);
        }
        public void BuildAsGoldReward(int goldAmount)
        {
            gameObject.SetActive(true);
            goldReward = goldAmount;
            goldAmountText.text = goldReward.ToString();
            goldParent.SetActive(true);
        }
        #endregion
    }
}