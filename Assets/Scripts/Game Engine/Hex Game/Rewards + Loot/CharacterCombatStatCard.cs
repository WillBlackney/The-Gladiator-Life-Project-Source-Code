using UnityEngine;
using HexGameEngine.UCM;
using TMPro;
using HexGameEngine.UI;
using DG.Tweening;

namespace HexGameEngine.RewardSystems
{
    public class CharacterCombatStatCard : MonoBehaviour
    {
        // Properties + Components
        #region
        [Header("Core Components")]
        [SerializeField] private GameObject levelUpParent;
        [SerializeField] private CanvasGroup levelUpParentCg;
        [SerializeField] private UIPerkIcon[] injuryIcons;
        [SerializeField] private GameObject knockDownIndicatorParent;
        [SerializeField] private GameObject deathIndicatorParent;

        [Header("Model Components")]
        [SerializeField] private UniversalCharacterModel ucm;
        [SerializeField] private GameObject portraitDeathIcon;

        [Header("Text Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI subNameText;
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private TextMeshProUGUI xpText;
        [SerializeField] private TextMeshProUGUI healthLostText;
        [SerializeField] private TextMeshProUGUI stressGainedText;
        [SerializeField] private TextMeshProUGUI killsText;
        [SerializeField] private TextMeshProUGUI armourLostText;
        [SerializeField] private TextMeshProUGUI damageDealtText;
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
        public TextMeshProUGUI CurrentLevelText
        {
            get { return currentLevelText; }
        }
        public TextMeshProUGUI NameText
        {
            get { return nameText; }
        }
        public TextMeshProUGUI SubNameText
        {
            get { return subNameText; }
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
        public TextMeshProUGUI ArmourLostText
        {
            get { return armourLostText; }
        }
        public TextMeshProUGUI DamageDealtText
        {
            get { return damageDealtText; }
        }
        #endregion

        public void ShowLevelUpIndicator()
        {
            LevelUpParent.SetActive(true);
            LevelUpParent.transform.DOKill();
            LevelUpParent.transform.DOScale(new Vector3(2, 2, 2), 0f);
            LevelUpParent.transform.DOScale(new Vector3(2.5f, 2.5f, 2.5f), 0.5f).SetLoops(-1, LoopType.Yoyo);
            LevelUpParent.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, 360), 1.0f)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear)
                .SetRelative();

            levelUpParentCg.DOKill();
            levelUpParentCg.DOFade(0.35f, 0f);
            levelUpParentCg.DOFade(0.75f, 0.5f).SetLoops(-1, LoopType.Yoyo);

        }
    }
}