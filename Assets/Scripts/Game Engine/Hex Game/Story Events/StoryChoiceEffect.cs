using HexGameEngine.Items;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HexGameEngine.StoryEvents
{
    [System.Serializable]
    public class StoryChoiceEffect
    {
        // General Fields
        [Header("General Settings")]
        public StoryChoiceEffectType effectType;
        [ShowIf("ShowTarget")]
        public ChoiceEffectTarget target;

        // Load page fields
        [ShowIf("ShowPageToLoad")]
        [Header("Page Settings")]
        public StoryEventPageSO pageToLoad;

        // Healing Fields
        [Header("Heal Settings")]
        [ShowIf("ShowHealType")]
        public HealType healType;
        [ShowIf("ShowHealPercentage")]
        [Range(1, 100)]
        public float healPercentage;
        [ShowIf("ShowHealAmount")]
        public int healAmount;

        // Max Health Mod Fields
        [Header("Max Health Modification Settings")]
        [ShowIf("ShowMaxHealthGainedOrLost")]
        public int maxHealthGainedOrLost;

        // Damage Fields
        [Header("Heal Settings")]
        [ShowIf("ShowDamageAmount")]
        public int damageAmount;

        // Item Fields
        [Header("Item Settings")]
        [ShowIf("ShowItemRewardType")]
        public ItemRewardType itemRewardType;
        [ShowIf("ShowItemGained")]
        public ItemDataSO itemGained;
        [ShowIf("ShowItemRewardType")]
        public int totalItemsGained = 1;

        // Gold Fields
        [Header("Gold Settings")]
        [ShowIf("ShowLoseAllGold")]
        public bool loseAllGold;
        [ShowIf("ShowGoldGainedOrLost")]
        public int goldGainedOrLost;

        // Card Fields
        [Header("Card Settings")]
        [ShowIf("ShowCardGained")]
        public int copiesGained = 1;
        [ShowIf("ShowRandomCard")]
        public bool randomCard;
        [ShowIf("ShowRandomCard")]
        public int randomCardAmount;

        // Combat Fields
        // [Header("Card Settings")]
        //[ShowIf("ShowEnemyWave")]
        //public EnemyWaveSO enemyWave;




        // Odin Show Ifs
        #region     
        public bool ShowLoseAllGold()
        {
            return effectType == StoryChoiceEffectType.ModifyGold;
        }
        public bool ShowGoldGainedOrLost()
        {
            if (effectType == StoryChoiceEffectType.ModifyGold && loseAllGold == false)
                return true;
            else
                return false;
        }           
        public bool ShowPageToLoad()
        {
            return effectType == StoryChoiceEffectType.LoadPage;
        }
        #endregion

    }
}