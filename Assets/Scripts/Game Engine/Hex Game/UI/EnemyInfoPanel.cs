using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.Utilities;
using HexGameEngine.Characters;
using HexGameEngine.Perks;
using TMPro;
using UnityEngine.UI;
using HexGameEngine.Items;
using HexGameEngine.UCM;
using HexGameEngine.Abilities;
using HexGameEngine.Cards;
using DG.Tweening;
using CardGameEngine.UCM;

namespace HexGameEngine.UI
{
    public class EnemyInfoPanel : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] private GameObject mainVisualParent;
        [SerializeField] private Scrollbar[] scrollBarResets;
        [Space(20)]
        [Header("Left Panel Components")]
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI healthRangeText;
        [SerializeField] private TextMeshProUGUI xpRewardValueText;
        [SerializeField] private TextMeshProUGUI totalArmourText;
        [SerializeField] private List<UIAbilityIcon> abilityIcons;
        [SerializeField] private UIPerkIcon[] passiveIcons;
        [SerializeField] private UniversalCharacterModel characterPanelUcm;

        [Header("Core Attribute Components")]
        [SerializeField] private TextMeshProUGUI mightText;
        [SerializeField] private GameObject[] mightStars;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI constitutionText;
        [SerializeField] private GameObject[] constitutionStars;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private GameObject[] accuracyStars;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI dodgeText;
        [SerializeField] private GameObject[] dodgeStars;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI resolveText;
        [SerializeField] private GameObject[] resolveStars;
        [Space(10)]
        [SerializeField] private TextMeshProUGUI witsText;
        [SerializeField] private GameObject[] witsStars;
        [Space(20)]

        [Header("Secondary Attribute Text Components")]
        [SerializeField] private TextMeshProUGUI criticalChanceText;
        [SerializeField] private TextMeshProUGUI criticalModifierText;
        [SerializeField] private TextMeshProUGUI initiativeText;
        [SerializeField] private TextMeshProUGUI visionText;
        [Space(20)]
        [Header("Resistances Text Components")]
        [SerializeField] private TextMeshProUGUI physicalResistanceText;
        [SerializeField] private TextMeshProUGUI magicResistanceText;
        [SerializeField] private TextMeshProUGUI injuryResistanceText;
        [SerializeField] private TextMeshProUGUI debuffResistanceText;
        [Space(20)]

        private HexCharacterData characterCurrentlyViewing;
        #endregion
    }
}