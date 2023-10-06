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

        [Header("Components")]
        [SerializeField] private TextMeshProUGUI goldAmountText;
        [SerializeField] private GameObject goldParent;
        [SerializeField] private Image itemImage;

        // Non inspector fields
        private int goldReward;
        private bool isGoldReward = false;

        #endregion

        // Getters + Accessors
        #region

        public ItemData ItemReward { get; private set; }
        public AbilityData AbilityReward { get; private set; }
        public bool IsGoldReward => isGoldReward;

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
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(AbilityReward.keyWords);
            }
        }
        public void MouseExit()
        {
            ItemPopupController.Instance.HidePanel();
            AbilityPopupController.Instance.HidePanel();
            MainModalController.Instance.HideModal();
            KeyWordLayoutController.Instance.FadeOutMainView();
        }

        #endregion

        // Logic
        #region

        public void Reset()
        {
            AbilityReward = null;
            ItemReward = null;
            isGoldReward = false;
            goldReward = 0;

            gameObject.SetActive(false);
            goldParent.SetActive(false);
            itemImage.gameObject.SetActive(false);

        }
        public void BuildAsItemReward(ItemData item)
        {
            gameObject.SetActive(true);
            itemImage.gameObject.SetActive(true);
            goldParent.SetActive(false);

            ItemReward = item;
            itemImage.sprite = item.ItemSprite;
        }
        public void BuildAsAbilityReward(AbilityData ability)
        {
            gameObject.SetActive(true);
            itemImage.gameObject.SetActive(true);
            goldParent.SetActive(false);

            AbilityReward = ability;
            itemImage.sprite = SpriteLibrary.Instance.GetTalentSchoolBookSprite(ability.talentRequirementData.talentSchool);
        }
        public void BuildAsGoldReward(int goldAmount)
        {
            gameObject.SetActive(true);
            goldParent.SetActive(true);
            itemImage.gameObject.SetActive(false);

            goldReward = goldAmount;
            goldAmountText.text = goldReward.ToString();
        }

        #endregion
    }
}
