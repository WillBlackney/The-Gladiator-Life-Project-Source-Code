using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HexGameEngine.Utilities;
using UnityEngine.UI;
using HexGameEngine.CameraSystems;
using TMPro;
using Sirenix.OdinInspector;

namespace HexGameEngine.UI
{
    public class ModalSceneWidget : MonoBehaviour
    {
        public ModalBuildPreset preset;

        private static ModalSceneWidget mousedOver;
        public static ModalSceneWidget MousedOver
        {
            get { return mousedOver; }
        }


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

    }
}