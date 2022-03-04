using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexGameEngine.UCM;
using CardGameEngine.UCM;
using TMPro;

namespace HexGameEngine.RewardSystems
{
    public class CharacterCombatStatCard : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] private UniversalCharacterModel ucm;
        [SerializeField] private GameObject levelUpParent;
        [SerializeField] private CharacterCombatStatCardPerkIcon[] injuryIcons;

        [Header("Text Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI xpText;
        [SerializeField] private TextMeshProUGUI healthLostText;
        [SerializeField] private TextMeshProUGUI stressGainedText;
        [SerializeField] private TextMeshProUGUI killsText;

        #endregion

        // Getters + Accessors
        #region
        public UniversalCharacterModel Ucm
        {
            get { return ucm; }
        }

        public GameObject LevelUpParent
        {
            get { return levelUpParent; }
        }

        public CharacterCombatStatCardPerkIcon[] InjuryIcons
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