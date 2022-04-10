using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.UCM;
using CardGameEngine.UCM;
using TMPro;
using HexGameEngine.UI;

namespace HexGameEngine.RewardSystems
{
    public class CharacterCombatStatCard : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] private GameObject levelUpParent;
        [SerializeField] private UIPerkIcon[] injuryIcons;
        [SerializeField] private GameObject knockDownIndicatorParent;
        [SerializeField] private GameObject deathIndicatorParent;

        [Header("Model Components")]
        [SerializeField] private UniversalCharacterModel ucm;
        [SerializeField] private GameObject portraitDeathIcon;

        [Header("Text Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI xpText;
        [SerializeField] private TextMeshProUGUI healthLostText;
        [SerializeField] private TextMeshProUGUI stressGainedText;
        [SerializeField] private TextMeshProUGUI killsText;

        #endregion

        // Getters + Accessors
        #region
        public GameObject KnockDownIndicatorParent
        {
            get { return knockDownIndicatorParent; }
        }
        public GameObject DeathIndicatorParent
        {
            get { return deathIndicatorParent; }
        }
        public GameObject PortraitDeathIcon
        {
            get { return portraitDeathIcon; }
        }
        public UniversalCharacterModel Ucm
        {
            get { return ucm; }
        }

        public GameObject LevelUpParent
        {
            get { return levelUpParent; }
        }

        public UIPerkIcon[] InjuryIcons
        {
            get { return injuryIcons; }
        }
        public TextMeshProUGUI NameText
        {
            get { return nameText; }
        }
        public TextMeshProUGUI XpText
        {
            get { return xpText; }
        }
        public TextMeshProUGUI HealthLostText
        {
            get { return healthLostText; }
        }
        public TextMeshProUGUI StressGainedText
        {
            get { return stressGainedText; }
        }
        public TextMeshProUGUI KillsText
        {
            get { return killsText; }
        }
        #endregion
    }
}