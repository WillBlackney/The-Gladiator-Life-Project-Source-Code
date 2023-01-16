using HexGameEngine.CameraSystems;
using HexGameEngine.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HexGameEngine.UI
{
    public class CursorController : Singleton<CursorController>
    {
        [Header("Cursor Data")]
        [SerializeField] CursorData[] allCursorData;

        [Header("Components")]
        [SerializeField] Transform placementParent;
        [SerializeField] Image cursorImage;
        [SerializeField] Image cursorShadowImage;
        private CursorData currentCursor = null;
     

        private void Update()
        {
            //Vector2 mousePos = CameraController.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            placementParent.position = Input.mousePosition;
        }

        private void Start()
        {
            Cursor.visible = false;
            SetCursor(CursorType.NormalPointer);
        }

        public void SetCursor(CursorType type)
        {
            currentCursor = GetCursorData(type);
            cursorImage.sprite = currentCursor.sprite;
            cursorShadowImage.sprite = cursorShadowImage.sprite;
        }

        private CursorData GetCursorData(CursorType type)
        {
            return Array.Find(allCursorData, x => x.typeTag == type);
        }

    }
    [System.Serializable]
    class CursorData
    {
        public CursorType typeTag;
        [PreviewField(75)]
        public Sprite sprite;
    }

    public enum CursorType
    {
        NormalPointer = 0,
        HandPointer = 1,
        Inspect = 2,
        InspectClick = 3,
        MoveClick = 4,
        TargetClick = 5,
        BuyClick = 6,
        PlusClick = 7,
        CancelClick = 8,
        Enter_Door = 9,
    }
}