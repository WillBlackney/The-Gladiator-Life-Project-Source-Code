using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    [Serializable]
    public class ModalSceneWidget : MonoBehaviour
    {
        #region Properties + Components

        [ShowIf("ShowPreset")]
        public ModalBuildPreset preset;
        public bool customData;

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

        public static ModalSceneWidget MousedOver { get; private set; }

        #endregion

        #region Events

        public void MouseEnter()
        {
            MousedOver = this;
            MainModalController.Instance.WidgetMouseEnter(this);
        }
        public void MouseExit()
        {
            if (MousedOver == this)
            {
                MousedOver = null;
                MainModalController.Instance.WidgetMouseExit(this);
            }
        }
        private void OnDisable()
        {
            if (MousedOver == this)
            {
                MousedOver = null;
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
