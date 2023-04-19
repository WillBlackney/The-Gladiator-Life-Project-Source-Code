using CardGameEngine.UCM;
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
        public TextMeshProUGUI healthTextWorld;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Armour Bar World References")]
        public Slider armourBarWorld;
        public Slider armourBarWorldUnder;
        public TextMeshProUGUI armourTextWorld;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Stress Bar World References")]
        public GameObject stressBarVisualParent;
        public Slider stressBarWorld;
        public TextMeshProUGUI stressTextWorld;
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
        [HideInInspector] public string currentAnimation;

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
            stressTextWorld.gameObject.SetActive(true);
            mouseOverWorldUI = true;
            if(UIController.Instance.CharacterWorldUiState == ShowCharacterWorldUiState.OnMouseOver)
                HexCharacterController.Instance.FadeInCharacterWorldCanvas(this, null, 0.25f);
        }
        public void OnAnyWorldUiMouseExit()
        {
            armourTextWorld.gameObject.SetActive(false);
            healthTextWorld.gameObject.SetActive(false);
            stressTextWorld.gameObject.SetActive(false);
            mouseOverWorldUI = false;
            StartCoroutine(OnAnyWorldUiMouseExitCoroutine());

        }
        private IEnumerator OnAnyWorldUiMouseExitCoroutine()
        {
            if(UIController.Instance.CharacterWorldUiState == ShowCharacterWorldUiState.Always)
            {
                armourTextWorld.gameObject.SetActive(false);
                healthTextWorld.gameObject.SetActive(false);
                stressTextWorld.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(0.25f);
            if (!mouseOverWorldUI && !mouseOverModel &&
                UIController.Instance.CharacterWorldUiState == ShowCharacterWorldUiState.OnMouseOver)
            {
                armourTextWorld.gameObject.SetActive(false);
                healthTextWorld.gameObject.SetActive(false);
                stressTextWorld.gameObject.SetActive(false);
                HexCharacterController.Instance.FadeOutCharacterWorldCanvas(this, null, 0.25f, 0.25f);
            }        
                           
        }
        public void OnModelRightClick()
        {
            if(character != null && 
                character.controller == Controller.AI)         
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
