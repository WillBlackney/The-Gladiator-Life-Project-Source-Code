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
        private PerkIconData myIconData;
        private string statusName;
        private int statusStacks;        

        [Header("Component References")]
        public TextMeshProUGUI statusStacksText;
        public Image passiveImage;

        private static PerkIconView mousedOver;

        public static PerkIconView MousedOver
        {
            get { return mousedOver; }
            private set { mousedOver = value; }
        }
        #endregion

        // Getters + Accessors
        #region
        public PerkIconData MyIconData
        {
            get { return myIconData; }
        }
        public string StatusName
        {
            get { return statusName; }
        }
        public int StatusStacks
        {
            get { return statusStacks; }
        }
        #endregion

        // Input 
        #region
        public void MouseEnter()
        {
            if(myIconData != null && 
                GameController.Instance.GameState == GameState.CombatActive && 
                (MousedOver == null || MousedOver != this))            
                KeyWordLayoutController.Instance.BuildAllViewsFromPassiveTag(myIconData.perkTag);            
        }
        public void MouseExit()
        {
            KeyWordLayoutController.Instance.FadeOutMainView();
        }
        #endregion

        // Logic
        #region
        public void Build(PerkIconData iconData)
        {
            myIconData = iconData;
            passiveImage.sprite = PerkController.Instance.GetPassiveSpriteByName(iconData.passiveName);

            statusName = iconData.passiveName;
            if (iconData.showStackCount)
            {
                statusStacksText.gameObject.SetActive(true);
            }
            if (iconData.hiddenOnPassivePanel)
            {
                gameObject.SetActive(false);
            }

            statusStacksText.text = statusStacks.ToString();

        }
        public void ModifyIconViewStacks(int stacksGainedOrLost)
        {
            statusStacks += stacksGainedOrLost;
            statusStacksText.text = statusStacks.ToString();
            if (statusStacks == 0)
            {
                statusStacksText.gameObject.SetActive(false);
            }
        }
        #endregion

    }
}