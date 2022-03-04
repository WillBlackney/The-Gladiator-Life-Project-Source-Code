using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using HexGameEngine.Audio;
using HexGameEngine.UI;

namespace HexGameEngine.Perks
{
    public class PerkIconView : MonoBehaviour
    {
        // Properties + Component References
        #region
        [Header("Properties")]
        [HideInInspector] public PerkIconData myIconData;
        [HideInInspector] public string statusName;
        [HideInInspector] public int statusStacks;
        [SerializeField] private Image glowOutline;

        [Header("Component References")]
        public TextMeshProUGUI statusStacksText;
        public Image passiveImage;
        #endregion

        // Input 
        #region
        public void OnMouseEnter()
        {
            if(GameController.Instance.GameState == GameState.CombatActive ||
                GameController.Instance.GameState != GameState.CombatActive)
            {
                KeyWordLayoutController.Instance.BuildAllViewsFromPassiveTag(myIconData.perkTag);
                AudioManager.Instance.PlaySoundPooled(Sound.GUI_Button_Mouse_Over);
            }
        }
        public void OnMouseExit()
        {
            KeyWordLayoutController.Instance.FadeOutMainView();
        }
        #endregion

    }
}