using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using WeAreGladiators.Utilities;

namespace WeAreGladiators.UI
{
    public class CursorController : Singleton<CursorController>
    {
        [Header("Cursor Data")]
        [SerializeField]
        private CursorData[] allCursorData;

        [Header("Components")]
        [SerializeField]
        private GameObject visualParent;
        [SerializeField] private Transform placementParent;
        [SerializeField] private Image cursorImage;
        [SerializeField] private Image cursorShadowImage;
        private CursorData currentCursor;

        public CursorData FallbackCursor { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isMobilePlatform && !Application.isConsolePlatform)
            {
                visualParent.SetActive(true);
            }
        }

        private void Start()
        {
            Cursor.visible = false;
            SetCursor(CursorType.NormalPointer);
            SetFallbackCursor(CursorType.NormalPointer);
        }
        private void Update()
        {
            placementParent.position = Input.mousePosition;
        }

        private void LateUpdate()
        {
            placementParent.position = Input.mousePosition;
        }

        public void SetCursor(CursorType type)
        {
            currentCursor = GetCursorData(type);
            cursorImage.sprite = currentCursor.sprite;
            cursorShadowImage.sprite = currentCursor.sprite;
        }
        public void SetFallbackCursor(CursorType type)
        {
            FallbackCursor = GetCursorData(type);
        }

        private CursorData GetCursorData(CursorType type)
        {
            return Array.Find(allCursorData, x => x.typeTag == type);
        }
    }
    [Serializable]
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
        Enter_Door = 9
    }
}
