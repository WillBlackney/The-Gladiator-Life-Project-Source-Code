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
        [SerializeField] GameObject visualParent;
        [SerializeField] Transform placementParent;
        [SerializeField] Image cursorImage;
        [SerializeField] Image cursorShadowImage;
        private CursorData currentCursor = null;
        private CursorData fallbackCursor = null;

        public CursorData FallbackCursor
        {
            get { return fallbackCursor; }
        }

        private void LateUpdate()
        {
            placementParent.position = Input.mousePosition;
        }
        private void Update()
        {
            placementParent.position = Input.mousePosition;
        }
    
        protected override void Awake()
        {
            base.Awake();

            if (!Application.isMobilePlatform && !Application.isConsolePlatform) visualParent.SetActive(true);
        }

        private void Start()
        {
            Cursor.visible = false;
            SetCursor(CursorType.NormalPointer);
            SetFallbackCursor(CursorType.NormalPointer);
        }

        public void SetCursor(CursorType type)
        {
            currentCursor = GetCursorData(type);
            cursorImage.sprite = currentCursor.sprite;
            cursorShadowImage.sprite = currentCursor.sprite;
        }
        public void SetFallbackCursor(CursorType type)
        {
            fallbackCursor = GetCursorData(type);
        }

        private CursorData GetCursorData(CursorType type)
        {
            return Array.Find(allCursorData, x => x.typeTag == type);
        }

    }
    [System.Serializable]
    public class CursorData
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