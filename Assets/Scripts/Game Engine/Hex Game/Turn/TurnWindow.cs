using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using WeAreGladiators.Characters;
using WeAreGladiators.UCM;
using WeAreGladiators.HexTiles;
using DG.Tweening;

namespace WeAreGladiators.TurnLogic
{
    public class TurnWindow : MonoBehaviour
    {
        // Properties + Component References
        #region
        [Header("Component References")]
        [SerializeField] private GameObject visualParent;
        public TextMeshProUGUI rollText;
        public CanvasGroup myCanvasGroup;
        public UniversalCharacterModel myUCM;

        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Properties")]
        public HexCharacterModel myCharacter;
        public bool animateNumberText;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Colouring References")]
        [SerializeField] private Image frameImage;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color highlightColor;
        #endregion

        // Mouse + Pointer Events
        #region
        public void MouseEnter()
        {
            frameImage.DOKill();
            frameImage.DOColor(highlightColor, 0.2f);
            if (myCharacter != null && myCharacter.livingState == LivingState.Alive &&
                myCharacter.currentTile != null)
            {
                myCharacter.currentTile.mouseOverParent.SetActive(true);
            }
        }
        public void MouseExit()
        {
            frameImage.DOKill();
            frameImage.DOColor(normalColor, 0.2f);
            if (myCharacter != null &&
                myCharacter.currentTile != null)
            {
                myCharacter.currentTile.mouseOverParent.SetActive(false);
            }
        }
        public void Hide()
        {
            frameImage.DOKill();
            frameImage.color = normalColor;
            visualParent.SetActive(false);
        }
        public void Show()
        {
            frameImage.DOKill();
            frameImage.color = normalColor;
            visualParent.SetActive(true);
        }
        #endregion

    }
}

