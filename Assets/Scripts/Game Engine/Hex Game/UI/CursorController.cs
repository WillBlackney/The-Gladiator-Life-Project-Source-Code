using HexGameEngine.CameraSystems;
using HexGameEngine.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.UI
{
    public class CursorController : Singleton<CursorController>
    {
        [SerializeField] Transform placementParent;
        private void Update()
        {
            Vector2 mousePos = CameraController.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            placementParent.position = mousePos;
        }
    }
}