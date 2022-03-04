using CardGameEngine.UCM;
using HexGameEngine.Abilities;
using HexGameEngine.Combat;
using HexGameEngine.Perks;
using HexGameEngine.TurnLogic;
using HexGameEngine.UCM;
using Sirenix.OdinInspector;
using Spriter2UnityDX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexGameEngine.Characters
{
    public class HexCharacterView : MonoBehaviour
    {
        // Properties + Component References
        #region
        [Header("Misc Properties")]
        [HideInInspector] public HexCharacterModel character;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("World Space Canvas References")]
        public CanvasGroup worldSpaceCG;
        public Transform worldSpaceCanvasParent;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Health Bar World References")]
        public Slider healthBarWorld;
        public TextMeshProUGUI healthTextWorld;
        public TextMeshProUGUI maxHealthTextWorld;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]       

        [Header("Stress Bar World References")]
        public Slider stressBarWorld;
        public TextMeshProUGUI stressTextWorld;
        public TextMeshProUGUI maxStressTextWorld;
        [PropertySpace(SpaceBefore = 50, SpaceAfter = 0)]

        [Header("GUI Canvas References")]
        public Canvas uiCanvas;
        public GameObject uiCanvasParent;
        public CanvasGroup uiCanvasCg;
        public UniversalCharacterModel uiPotraitUCM;
        public TextMeshProUGUI characterNameTextUI;
        public AbilityButton[] abilityButtons;
        public StressPanelView stressPanel;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Health Bar UI References")]
        public Slider healthBarUI;
        public TextMeshProUGUI healthTextUI;
        public TextMeshProUGUI maxHealthTextUI;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Energy UI References")]
        public Slider energyBar;
        public TextMeshProUGUI energyTextUI;
        public TextMeshProUGUI maxEnergyTextUI;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Stress Bar UI References")]
        public Slider stressBarUI;
        public TextMeshProUGUI stressTextUI;
        public TextMeshProUGUI maxStressTextUI;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Custom Components")]
        [HideInInspector] public TurnWindow myActivationWindow;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Movement + Anim References")]
        public GameObject mainMovementParent;
        public GameObject ucmMovementParent;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Free Strike Indicator Components")]
        public GameObject freeStrikeVisualParent;
        public Transform freeStrikeSizingParent;

        [Header("UCM References")]
        public GameObject ucmVisualParent;
        public GameObject ucmSizingParent;
        public GameObject ucmShadowParent;
        public CanvasGroup ucmShadowCg;     
        public UniversalCharacterModel ucm;
        public Animator ucmAnimator;
        public EntityRenderer entityRenderer;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Perk Components")]
        public GameObject passiveIconsVisualParent;
        [HideInInspector] public List<PerkIconView> passiveIcons;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        public bool mouseOverModel = false;
        public bool mouseOverWorldUI = false;

        #endregion

        // Getters
        #region
        public Vector3 WorldPosition
        {
            get { return mainMovementParent.transform.position; }
        }

        #endregion

        // Input
        #region
        public void OnAnyWorldUiMouseEnter()
        {
            mouseOverWorldUI = true;
            HexCharacterController.Instance.FadeInCharacterWorldCanvas(this, null, 0.25f);
        }
        public void OnAnyWorldUiMouseExit()
        {
            mouseOverWorldUI = false;
            if (!mouseOverWorldUI && !mouseOverModel)
            {
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(this, null, 0.25f, 0.25f);
            }

        }
        #endregion
    }
}
