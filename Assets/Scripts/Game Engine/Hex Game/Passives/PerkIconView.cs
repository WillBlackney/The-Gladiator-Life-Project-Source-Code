﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using HexGameEngine.Audio;
using HexGameEngine.UI;
using System.Linq;

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
        [SerializeField] private TextMeshProUGUI statusStacksText;
        [SerializeField] private Image passiveImage;
        [SerializeField] private Image frameImage;
        [SerializeField] private Sprite normalFrame;
        [SerializeField] private Sprite injuryFrame;

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
            {
                MainModalController.Instance.BuildAndShowModal(new ActivePerk(myIconData.perkTag, 1));
                KeyWordLayoutController.Instance.BuildAllViewsFromKeyWordModels(myIconData.keywords.ToList());
            }
               // KeyWordLayoutController.Instance.BuildAllViewsFromPassiveTag(myIconData.perkTag);            
        }
        public void MouseExit()
        {
            MainModalController.Instance.HideModal();
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
            if (iconData.showStackCount && iconData.isInjury == false)
            {
                statusStacksText.gameObject.SetActive(true);
            }
            else statusStacksText.gameObject.SetActive(false);

            if (iconData.hiddenOnPassivePanel)
            {
                gameObject.SetActive(false);
            }

            statusStacksText.text = statusStacks.ToString();

            // Build frame
            frameImage.sprite = normalFrame;
            if (iconData.isInjury) frameImage.sprite = injuryFrame;

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