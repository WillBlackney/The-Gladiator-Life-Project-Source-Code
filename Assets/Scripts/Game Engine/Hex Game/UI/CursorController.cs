using HexGameEngine.CameraSystems;
using HexGameEngine.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexGameEngine.UI
{
    public class CursorController : Singleton<CursorController>
    {
        [Header("Cursor Data")]
        [SerializeField] CursorData[] allCursorData;

        [Header("Components")]
        [SerializeField] Transform placementParent;
        [SerializeField] SpriteRenderer cursorSR;

        private CursorData currentCursor = null;
     

        private void Update()
        {
            Vector2 mousePos = CameraController.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            placementParent.position = mousePos;
        }

        protected override void Awake()
        {
            base.Awake();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            SetCursor(CursorType.NormalPointer);
        }

        public void SetCursor(CursorType type)
        {
            currentCursor = GetCursorData(type);
            cursorSR.sprite = currentCursor.sprite;
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