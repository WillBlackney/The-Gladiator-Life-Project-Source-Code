using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

namespace HexGameEngine.Cards
{
    public class CardViewModel : MonoBehaviour
    {
        // Properties + Component References
        #region
      //  [Header("General Properties")]
       // [HideInInspector] public Card card;
       // [HideInInspector] public CampCard campCard;
        [HideInInspector] public EventSetting eventSetting;

        [Header("General Components")]
        public Transform movementParent;
        public CardViewModel myPreviewCard;
        public bool isPreviewCard;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Core GUI Components")]
        public CardLocationTracker locationTracker;
        public DraggingActions draggingActions;
        public Draggable draggable;
        public HoverPreview hoverPreview;
        public CardSlotHelper mySlotHelper;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Text References")]
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;

        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Energy Components")]
        public GameObject energyIconVisualParent;
        public TextMeshProUGUI energyText;

        [Header("Cooldown Components")]
        public GameObject cooldownIconVisualParent;
        public TextMeshProUGUI cooldownText;

        [Header("Ability Type Components")]
        public GameObject abilityTypeIconVisualParent;
        public Image abilityTypeImage;

        [Header("Talent Components")]
        public GameObject talentIconVisualParent;
        public Image talentIconImage;

        [Header("Image References")]
        public Image graphicImage;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Glow Outline References")]
        public Animator glowAnimator;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Colouring References")]
        public Image[] talentRenderers;
        public Image[] rarityRenderers;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Canvas References")]
        public Canvas canvas;
        public CanvasGroup cg;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        #endregion

        private void OnEnable()
        {
            if (isPreviewCard)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 1000;
            }
        }
    }

    public enum EventSetting
    {
        None = 0,
        Camping = 1,
    }
}