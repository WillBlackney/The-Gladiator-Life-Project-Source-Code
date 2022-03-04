using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using CardGameEngine.UCM;
using UnityEngine.UI;
using HexGameEngine.Characters;
using UnityEngine.EventSystems;
using DG.Tweening;
using HexGameEngine.Perks;

namespace HexGameEngine.Camping
{
    public class CampSiteCharacterBox : MonoBehaviour, IPointerClickHandler
    {
        // Components
        #region
        [Header("Core Components")]
        [SerializeField] private UniversalCharacterModel ucm;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image validTargetOutline;
        [SerializeField] private PerkIconView[] perkIcons;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Health Bar Components")]
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI healthText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Stress Bar Components")]
        [SerializeField] private Slider stressBar;
        [SerializeField] private TextMeshProUGUI stressText;

        [HideInInspector] public HexCharacterData myCharacterData;
        #endregion

        // Getters + Accessors
        #region
        public UniversalCharacterModel Ucm { get { return ucm; } }
        public TextMeshProUGUI NameText { get { return nameText; } }
        public Slider HealthBar { get { return healthBar; } }
        public TextMeshProUGUI HealthText { get { return healthText; } }
        public Slider StressBar { get { return stressBar; } }
        public TextMeshProUGUI StressText { get { return stressText; } }
        public PerkIconView[] PerkIcons { get { return perkIcons; } }

        #endregion

        // Misc
        #region
        public void PlayGlowAnimation()
        {
            Debug.Log("CampActivityButton.PlayGlowAnimation() called...");
            validTargetOutline.gameObject.SetActive(true);
            validTargetOutline.DOKill();
            validTargetOutline.DOFade(0f, 0f);
            validTargetOutline.DOFade(1f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        public void StopGlowAnimation()
        {
            Debug.Log("CampActivityButton.StopGlowAnimation() called...");
            validTargetOutline.DOKill();
            validTargetOutline.DOFade(0f, 0f);
            validTargetOutline.gameObject.SetActive(false);
        }
        #endregion

        // Input Listeners
        #region
        public void OnPointerClick(PointerEventData eventData)
        {
            CampSiteController.Instance.OnCharacterBoxClicked(this);
        }
        #endregion
    }
}