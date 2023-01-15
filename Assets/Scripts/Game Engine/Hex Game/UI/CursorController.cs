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
        [SerializeField] SpriteRenderer cursorSR;

        private void Update()
        {
            Vector2 mousePos = CameraController.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            placementParent.position = mousePos;
        }


    }
    [System.Serializable]
    class CursorData
    {

    }

    public enum CursorType
    {
        NormalPointer = 0,
        HandPointer = 1,
        Inspect = 2,
        InspectClick = 3,
        MoveClick = 4,
    }
}