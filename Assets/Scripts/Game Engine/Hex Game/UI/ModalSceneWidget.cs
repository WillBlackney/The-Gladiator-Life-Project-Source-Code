using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators.Utilities;
using Sirenix.OdinInspector;
using System;

namespace WeAreGladiators.UI
{
    [Serializable]
    public class ModalSceneWidget : MonoBehaviour
    {
        #region Properties + Components
        [ShowIf("ShowPreset")]
        public ModalBuildPreset preset;
        public bool customData = false;

        [ShowIf("ShowCustomDataFields")]
        public string headerMessage;
        [ShowIf("ShowCustomDataFields")]
        public List<CustomString> descriptionMessage;
        [ShowIf("ShowCustomDataFields")]
        public Sprite headerSprite;
        [ShowIf("ShowCustomDataFields")]
        public bool frameSprite;
        #endregion

        #region Getters + Accessors
        private static ModalSceneWidget mousedOver;
        public static ModalSceneWidget MousedOver
        {
            get { return mousedOver; }
        }
        #endregion

        #region Events
        public void MouseEnter()
        {
            mousedOver = this;
            MainModalController.Instance.WidgetMouseEnter(this);
        }
        public void MouseExit()
        {
            if(mousedOver == this)
            {
                mousedOver = null;
                MainModalController.Instance.WidgetMouseExit(this);
            }
        }
        void OnDisable()
        {
            if (mousedOver == this)
            {
                mousedOver = null;
                MainModalController.Instance.WidgetMouseExit(this);
            }
        }
        #endregion

        #region Odin Showifs
        public bool ShowPreset()
        {
            return customData == false;
        }
        public bool ShowCustomDataFields()
        {
            return customData;
        }

        #endregion
    }
}