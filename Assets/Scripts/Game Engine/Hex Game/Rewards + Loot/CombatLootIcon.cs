using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Abilities;
using WeAreGladiators.Items;
using WeAreGladiators.Libraries;
using WeAreGladiators.UI;

namespace WeAreGladiators.RewardSystems
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
        private int goldReward;

        #endregion

        // Getters + Accessors
        #region

        public ItemData ItemReward { get; private set; }
        public AbilityData AbilityReward { get; private set; }

        #endregion

        // Input 
        #region

        public void MouseEnter()
        {
            if (ItemReward != null)
            {
                ItemPopupController.Instance.OnCombatItemLootIconMousedOver(this);
            }
            else if (AbilityReward != null)
            {
                AbilityPopupController.Instance.OnCombatAbilityLootIconMousedOver(this);
            }

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
            AbilityReward = null;
            ItemReward = null;
            goldReward = 0;

            gameObject.SetActive(false);
            goldParent.SetActive(false);
            abilityBookParent.SetActive(false);
            itemParent.SetActive(false);

        }
        public void BuildAsItemReward(ItemData item)
        {
            gameObject.SetActive(true);
            ItemReward = item;
            itemParent.SetActive(true);
            itemImage.sprite = item.ItemSprite;
            itemRarityOverlayImage.color = ColorLibrary.Instance.GetRarityColor(item.rarity);
        }
        public void BuildAsAbilityReward(AbilityData ability)
        {
            gameObject.SetActive(true);
            AbilityReward = ability;
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
