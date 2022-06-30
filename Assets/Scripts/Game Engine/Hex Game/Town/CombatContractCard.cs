using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using HexGameEngine.Characters;
using HexGameEngine.Libraries;
using HexGameEngine.Items;
using HexGameEngine.Abilities;
using HexGameEngine.UI;
using DG.Tweening;

namespace HexGameEngine.TownFeatures
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
        [SerializeField] private GameObject glowOutline;

        [Header("Reward Components")]
        [SerializeField] private TextMeshProUGUI goldRewardText;
        [SerializeField] private GameObject itemParent;
        [SerializeField] private Image itemImage;
        [SerializeField] private Image itemRarityOverlay;
        [SerializeField] private Image abilityTomeImage;

        // Non inspector fields
        private CombatContractData myContractData;
        private static CombatContractCard selectectedCombatCard = null;
        #endregion

        // Getters + Accessors
        #region
        public static CombatContractCard SelectectedCombatCard
        {
            get { return selectectedCombatCard; }
            private set { selectectedCombatCard = value; }                
        }
        public CombatContractData MyContractData
        {
            get { return myContractData; }
        }
        public Image AbilityTomeImage
        {
            get { return abilityTomeImage; }
        }
        public Image ItemImage
        {
            get { return itemImage; }
        }
        #endregion

        // Input Events
        #region
        public void OnCardMouseOver()
        {
            if (selectectedCombatCard == this) return;
            gameObject.transform.DOKill();
            gameObject.transform.DOScale(1.1f, 0.2f);
        }
        public void OnCardMouseExit()
        {
            if (selectectedCombatCard == this) return;
            gameObject.transform.DOKill();
            gameObject.transform.DOScale(1f, 0.2f);
        }      
        public void OnCardClicked()
        {
            if (selectectedCombatCard == this)
            {
                HandleDeselect();
                return;
            }
            if(selectectedCombatCard != null)
            {
                // Deselct + shrink old selection
                selectectedCombatCard.glowOutline.SetActive(false);
                selectectedCombatCard.gameObject.transform.DOKill();
                selectectedCombatCard.gameObject.transform.DOScale(1f, 0.2f);                
            }

            // Scale up this card and flag as selected
            TownController.Instance.SetDeploymentButtonReadyState(true);
            selectectedCombatCard = this;
            selectectedCombatCard.glowOutline.SetActive(true);
            selectectedCombatCard.gameObject.transform.DOKill();
            selectectedCombatCard.gameObject.transform.DOScale(1.1f, 0f);
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
            myContractData = data;
            gameObject.SetActive(true);
            gameObject.transform.parent.gameObject.SetActive(true);
            gameObject.transform.localScale = new Vector3(1, 1, 1);

            if (data.enemyEncounterData.difficulty == CombatDifficulty.Basic) basicSkullsParent.SetActive(true);
            else if (data.enemyEncounterData.difficulty == CombatDifficulty.Elite) eliteSkullsParent.SetActive(true);
            else if (data.enemyEncounterData.difficulty == CombatDifficulty.Boss) bossSkullsParent.SetActive(true);

            for (int i = 0; i < enemyRows.Length; i++)
                enemyRows[i].HideAndReset();

            for (int i = 0; i < enemyRows.Length && i < data.enemyEncounterData.enemiesInEncounter.Count; i++)
            {
                enemyRows[i].BuildFromEnemyData(data.enemyEncounterData.enemiesInEncounter[i]);
            }

            goldRewardText.text = data.combatRewardData.goldAmount.ToString();
            abilityTomeImage.sprite = SpriteLibrary.Instance.GetTalentSchoolBookSprite(data.combatRewardData.abilityAwarded.talentRequirementData.talentSchool);
            itemParent.SetActive(false);
            
            if(data.combatRewardData.item != null)
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
            myContractData = null;
            basicSkullsParent.SetActive(false);
            eliteSkullsParent.SetActive(false);
            bossSkullsParent.SetActive(false);
            itemParent.SetActive(false);
            for (int i = 0; i < enemyRows.Length; i++)
                enemyRows[i].HideAndReset();
        }
        public static void HandleDeselect(float speed = 0.2f)
        {
            TownController.Instance.SetDeploymentButtonReadyState(false);
            if (selectectedCombatCard != null)
            {
                selectectedCombatCard.gameObject.transform.DOKill();
                selectectedCombatCard.gameObject.transform.DOScale(1f, speed);
                selectectedCombatCard.glowOutline.SetActive(false);
                selectectedCombatCard = null;
            }
        }
        #endregion
    }
}