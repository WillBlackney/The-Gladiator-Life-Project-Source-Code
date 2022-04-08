using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using HexGameEngine.Utilities;
using UnityEngine.UI;
using HexGameEngine.CameraSystems;
using TMPro;

namespace HexGameEngine.UI
{
    public class ModalSceneWidget : MonoBehaviour
    {
        public ModalBuildPreset preset;

        public void MouseEnter()
        {
            MainModalController.Instance.WidgetMouseEnter(this);
        }
        public void MouseExit()
        {
            MainModalController.Instance.WidgetMouseExit(this);
        }
        void OnDisable()
        {
            MainModalController.Instance.WidgetMouseExit(this);
        }
    }
}