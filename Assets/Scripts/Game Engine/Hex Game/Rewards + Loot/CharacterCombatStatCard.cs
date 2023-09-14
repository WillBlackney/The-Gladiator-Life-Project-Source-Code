using DG.Tweening;
using TMPro;
using UnityEngine;
using WeAreGladiators.UCM;
using WeAreGladiators.UI;

namespace WeAreGladiators.RewardSystems
{
    public class CharacterCombatStatCard : MonoBehaviour
    {

        public void ShowLevelUpIndicator()
        {
            LevelUpParent.SetActive(true);
            LevelUpParent.transform.DOKill();
            LevelUpParent.transform.DOScale(new Vector3(2, 2, 2), 0f);
            LevelUpParent.transform.DOScale(new Vector3(2.25f, 2.25f, 2.25f), 1f).SetLoops(-1, LoopType.Yoyo);
            LevelUpParent.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, 360), 2f)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear)
                .SetRelative();

            levelUpParentCg.DOKill();
            levelUpParentCg.DOFade(0.65f, 0f);
            levelUpParentCg.DOFade(1f, 1f).SetLoops(-1, LoopType.Yoyo);

        }
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

        public GameObject KnockDownIndicatorParent => knockDownIndicatorParent;
        public GameObject DeathIndicatorParent => deathIndicatorParent;
        public GameObject PortraitDeathIcon => portraitDeathIcon;
        public UniversalCharacterModel Ucm => ucm;

        public GameObject LevelUpParent => levelUpParent;

        public UIPerkIcon[] InjuryIcons => injuryIcons;
        public TextMeshProUGUI CurrentLevelText => currentLevelText;
        public TextMeshProUGUI NameText => nameText;
        public TextMeshProUGUI SubNameText => subNameText;
        public TextMeshProUGUI XpText => xpText;
        public TextMeshProUGUI HealthLostText => healthLostText;
        public TextMeshProUGUI StressGainedText => stressGainedText;
        public TextMeshProUGUI KillsText => killsText;
        public TextMeshProUGUI ArmourLostText => armourLostText;
        public TextMeshProUGUI DamageDealtText => damageDealtText;

        #endregion
    }
}
