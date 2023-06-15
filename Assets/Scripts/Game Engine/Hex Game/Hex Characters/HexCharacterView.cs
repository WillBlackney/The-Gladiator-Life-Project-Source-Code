﻿using HexGameEngine.UCM;
using HexGameEngine.Abilities;
using HexGameEngine.Combat;
using HexGameEngine.Perks;
using HexGameEngine.TurnLogic;
using Sirenix.OdinInspector;
using Spriter2UnityDX;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using HexGameEngine.UI;
using HexGameEngine.VisualEvents;
using HexGameEngine.Utilities;

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
        public Slider healthBarWorldUnder;
        public Image healthBarWorldUnderSliderImage;
        public Color healthBarNormalColor;
        public Color healthBarDamageColor;
        public TextMeshProUGUI healthTextWorld;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Armour Bar World References")]
        public Slider armourBarWorld;
        public Slider armourBarWorldUnder;
        public Image armourBarWorldUnderSliderImage;
        public Color armourBarNormalColor;
        public Color armourBarDamageColor;
        public TextMeshProUGUI armourTextWorld;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Stress Bar World References")]
        public GameObject stressBarVisualParent;
        public Slider stressBarWorld;
        public Image stressBarShatteredGlowWorld;
        public Image stressStateIconWorld;
        [PropertySpace(SpaceBefore = 50, SpaceAfter = 0)]    

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
        public CharacterVfxManager vfxManager;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Perk Components")]
        public PerkLayoutPanel perkIconsPanel;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        public bool mouseOverModel = false;
        public bool mouseOverWorldUI = false;

        private string currentAnimation;
        public string CurrentAnimation
        {
            get { return currentAnimation; }
            set
            {
                // Show or stop movement dust poofs
                currentAnimation = value;
                if (vfxManager != null && 
                    (currentAnimation == AnimationEventController.RUN ||
                    currentAnimation == AnimationEventController.CHARGE ||
                    currentAnimation == AnimationEventController.CHARGE_END ||
                    currentAnimation == AnimationEventController.TACKLE ||
                    currentAnimation == AnimationEventController.TACKLE_END)) vfxManager.PlayMovementDirtPoofs();               
                else if (vfxManager != null) vfxManager.StopMovementDirtPoofs();

                // Show or stop dash trail
                if (vfxManager != null &&
                   (currentAnimation == AnimationEventController.CHARGE ||
                   currentAnimation == AnimationEventController.TACKLE)) 
                {
                    vfxManager.PlayDashTrail();
                    DelayUtils.DelayedCall(2f, () =>
                    {
                        if (currentAnimation != AnimationEventController.CHARGE &&
                         currentAnimation != AnimationEventController.TACKLE)
                        {
                            vfxManager.StopDashTrail();
                        }
                    });
                }
                else if (vfxManager != null) vfxManager.StopDashTrail();

                // Show normal face on idle
                if (currentAnimation == AnimationEventController.IDLE) ucm.ShowNormalFace();
            }
        }

        #endregion

        // Getters
        #region
        public Vector3 WorldPosition
        {
            get 
            { 
                if(mainMovementParent != null) return mainMovementParent.transform.position; 
                else
                {
                    Debug.LogWarning("HexCharacterView.WorldPosition() mainMovementParent object is null!!!");
                    return Vector2.zero;
                }
            }
        }

        #endregion

        // Input
        #region
        public void OnAnyWorldUiMouseEnter()
        {
            armourTextWorld.gameObject.SetActive(true);
            healthTextWorld.gameObject.SetActive(true);
            mouseOverWorldUI = true;
            if(UIController.Instance.CharacterWorldUiState == ShowCharacterWorldUiState.OnMouseOver)
                HexCharacterController.Instance.FadeInCharacterWorldCanvas(this, null, 0.25f);
        }
        public void OnAnyWorldUiMouseExit()
        {
            armourTextWorld.gameObject.SetActive(false);
            healthTextWorld.gameObject.SetActive(false);
            mouseOverWorldUI = false;
            StartCoroutine(OnAnyWorldUiMouseExitCoroutine());

        }
        private IEnumerator OnAnyWorldUiMouseExitCoroutine()
        {
            if(UIController.Instance.CharacterWorldUiState == ShowCharacterWorldUiState.Always)
            {
                armourTextWorld.gameObject.SetActive(false);
                healthTextWorld.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(0.25f);
            if (!mouseOverWorldUI && !mouseOverModel &&
                UIController.Instance.CharacterWorldUiState == ShowCharacterWorldUiState.OnMouseOver)
            {
                armourTextWorld.gameObject.SetActive(false);
                healthTextWorld.gameObject.SetActive(false);
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(this, null, 0.25f, 0.001f);
            }        
                           
        }
        public void OnModelRightClick()
        {
            if(character != null && 
                character.controller == Controller.AI &&
                !AbilityController.Instance.AwaitingAbilityOrder() &&
                !AbilityController.Instance.HitChanceModalIsVisible)         
                EnemyInfoPanel.Instance.HandleBuildAndShowPanel(character.characterData);            
        }
        #endregion

        // Misc
        #region
        public void DoShatteredGlow()
        {
            stressBarShatteredGlowWorld.DOKill();
            stressBarShatteredGlowWorld.DOFade(0, 0);
            stressBarShatteredGlowWorld.DOFade(1, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }
        public void StopShatteredGlow()
        {
            stressBarShatteredGlowWorld.DOKill();
            stressBarShatteredGlowWorld.DOFade(0, 0.25f);
        }
        #endregion
    }
}
