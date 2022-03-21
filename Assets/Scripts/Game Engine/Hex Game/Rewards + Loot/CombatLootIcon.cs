using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        #endregion

        // Getters + Accessors
        #region
        #endregion

        // Input 
        #region
        #endregion

        // Logic
        #region
        #endregion
    }
}