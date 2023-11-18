using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Items;
using WeAreGladiators.Libraries;
using WeAreGladiators.UI;

namespace WeAreGladiators.TownFeatures
{
    public class CombatContractCard : MonoBehaviour
    {
        // Properties + Components
        #region

        [Header("Difficulty Components")]
        [SerializeField] private GameObject basicSkullsParent;
        [SerializeField] private GameObject eliteSkullsParent;
        [SerializeField] private GameObject bossSkullsParent;

        [Header("Core Components")]
        [SerializeField] private CombatContractCardEnemyInfoRow[] enemyRows;
        [SerializeField] private Image glowOutline;
        [SerializeField] private TextMeshProUGUI deploymentLimitText;

        [Header("Reward Components")]
        [SerializeField] private TextMeshProUGUI goldRewardText;
        [SerializeField] private GameObject itemParent;
        [SerializeField] private Image itemImage;
        [SerializeField] private Image itemRarityOverlay;
        [SerializeField] private Image abilityTomeImage;

        // Non inspector fields

        #endregion

        // Getters + Accessors
        #region

        public static CombatContractCard SelectectedCombatCard { get; private set; }
        public CombatContractData MyContractData { get; private set; }
        public Image AbilityTomeImage => abilityTomeImage;
        public Image ItemImage => itemImage;
        public TextMeshProUGUI DeploymentLimitText => deploymentLimitText;

        #endregion

        // Input Events
        #region

        public void OnCardMouseOver()
        {
            if (SelectectedCombatCard == this)
            {
                return;
            }
            gameObject.transform.DOKill();
            gameObject.transform.DOScale(1.1f, 0.2f);
        }
        public void OnCardMouseExit()
        {
            if (SelectectedCombatCard == this)
            {
                return;
            }
            gameObject.transform.DOKill();
            gameObject.transform.DOScale(1f, 0.1f);
        }
        public void OnCardClicked()
        {
            if (SelectectedCombatCard == this)
            {
                HandleDeselect();
                return;
            }
            if (SelectectedCombatCard != null)
            {
                // Deselct + shrink old selection
                SelectectedCombatCard.glowOutline.DOKill();
                SelectectedCombatCard.gameObject.transform.DOKill();

                SelectectedCombatCard.glowOutline.DOFade(0f, 0.1f);
                SelectectedCombatCard.gameObject.transform.DOScale(1f, 0.1f);
            }

            // Scale up this card and flag as selected
            TownController.Instance.SetDeploymentButtonReadyState(true);
            SelectectedCombatCard = this;
            SelectectedCombatCard.glowOutline.DOKill();
            SelectectedCombatCard.glowOutline.DOFade(0.2f, 0.1f);

            SelectectedCombatCard.gameObject.transform.DOKill();
            SelectectedCombatCard.gameObject.transform.DOScale(1.1f, 0f);
        }
        public void OnItemBoxMouseOver()
        {
            ItemPopupController.Instance.OnCombatContractItemIconMousedOver(this);
        }
        public void OnItemBoxMouseExit()
        {
            ItemPopupController.Instance.HidePanel();
        }
        public void OnAbilityTomeBoxMouseOver()
        {
            AbilityPopupController.Instance.OnCombatContractAbilityIconMousedOver(this);
        }
        public void OnAbilityTomeBoxMouseExit()
        {
            AbilityPopupController.Instance.HidePanel();
        }

        #endregion

        // Logic
        #region

        public void BuildFromContractData(CombatContractData data)
        {
            ResetAndHide();
            MyContractData = data;
            gameObject.SetActive(true);
            gameObject.transform.parent.gameObject.SetActive(true);
            gameObject.transform.localScale = new Vector3(1, 1, 1);

            if (data.enemyEncounterData.difficulty == CombatDifficulty.Basic)
            {
                basicSkullsParent.SetActive(true);
            }
            else if (data.enemyEncounterData.difficulty == CombatDifficulty.Elite)
            {
                eliteSkullsParent.SetActive(true);
            }
            else if (data.enemyEncounterData.difficulty == CombatDifficulty.Boss)
            {
                bossSkullsParent.SetActive(true);
            }

            for (int i = 0; i < enemyRows.Length; i++)
            {
                enemyRows[i].HideAndReset();
            }

            for (int i = 0; i < enemyRows.Length && i < data.enemyEncounterData.enemiesInEncounter.Count; i++)
            {
                enemyRows[i].BuildFromEnemyData(data.enemyEncounterData.enemiesInEncounter[i]);
            }

            deploymentLimitText.text = data.enemyEncounterData.deploymentLimit.ToString();
            goldRewardText.text = data.combatRewardData.goldAmount.ToString();
            abilityTomeImage.sprite = SpriteLibrary.Instance.GetTalentSchoolBookSprite(data.combatRewardData.abilityAwarded.talentRequirementData.talentSchool);
            itemParent.SetActive(false);

            if (data.combatRewardData.item != null)
            {
                itemParent.SetActive(true);
                itemRarityOverlay.color = ColorLibrary.Instance.GetRarityColor(data.combatRewardData.item.rarity);
                itemImage.sprite = data.combatRewardData.item.ItemSprite;
            }

        }
        public void ResetAndHide()
        {
            gameObject.transform.parent.gameObject.SetActive(false);
            gameObject.SetActive(false);
            MyContractData = null;
            basicSkullsParent.SetActive(false);
            eliteSkullsParent.SetActive(false);
            bossSkullsParent.SetActive(false);
            itemParent.SetActive(false);
            for (int i = 0; i < enemyRows.Length; i++)
            {
                enemyRows[i].HideAndReset();
            }
        }
        public static void HandleDeselect(float speed = 0.1f)
        {
            TownController.Instance.SetDeploymentButtonReadyState(false);
            if (SelectectedCombatCard != null)
            {
                SelectectedCombatCard.gameObject.transform.DOKill();
                SelectectedCombatCard.gameObject.transform.DOScale(1f, speed);
                SelectectedCombatCard.glowOutline.DOKill();
                SelectectedCombatCard.glowOutline.DOFade(0f, 0.1f);
                SelectectedCombatCard = null;
            }
        }

        #endregion
    }
}
