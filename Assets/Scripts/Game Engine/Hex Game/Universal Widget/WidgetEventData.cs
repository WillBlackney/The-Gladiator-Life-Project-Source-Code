using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using HexGameEngine.Audio;
using HexGameEngine.UI;

namespace HexGameEngine.UWidget
{
    [Serializable]
    public class WidgetEventData
    {
        // Properties + References
        #region
        [Header("Core Event Properties")]
        [Tooltip("The type of event triggered.")]
        public WidgetEvent widgetEventType;

        [ShowIf("ShowScalingSequence")]
        public ScaleEventOrder scalingSequence;

        [ShowIf("ShowDelayProperties")]
        [Tooltip("When checked, this event will wait for a specified amount of time to elapse before starting.")]
        public bool enableStartDelay;

        [ShowIf("enableStartDelay", true)]
        [Tooltip("The duration in seconds that this event will wait before starting.")]
        public float startDelay;

        [ShowIf("enableStartDelay", true)]
        [Tooltip("When checked, this event will only start if the user has kept their mouse/finger over the specified object for the duration of the start delay.")]
        public bool onlyIfMouseIsStillOverMe;

        [ShowIf("widgetEventType", WidgetEvent.DisableGameObject)]
        [Tooltip("The 'Game Object' you wish to disable.")]
        public GameObject objectToDisable;

        [ShowIf("widgetEventType", WidgetEvent.EnableGameObject)]
        [Tooltip("The 'Game Object' you wish to enable.")]
        public GameObject objectToEnable;

        [ShowIf("widgetEventType", WidgetEvent.InvokeFunction)]
        [Tooltip("The function/method that will be called/invoked.")]
        public UnityEvent functionInvoked;

        [ShowIf("widgetEventType", WidgetEvent.PlaySound)]
        public Sound soundPlayed;

        [ShowIf("ShowCanvasField")]
        [Tooltip("The 'Canvas Group' component you wish to manipulate.")]
        public CanvasGroup canvasGroup;

        [ShowIf("ShowImageField")]
        [Tooltip("The 'Image' component you wish to manipulate.")]
        public Image image;

        [ShowIf("widgetEventType", WidgetEvent.TransistionTextColour)]
        [Tooltip("The 'TextMeshProUGUI' component you wish to manipulate.")]
        public TextMeshProUGUI text;

        [ShowIf("ShowEndColour")]
        [Tooltip("The colour the given element will transistion to.")]
        public Color endColour;


        // Movement Properties + Values
        [ShowIf("ShowMovementType")]
        [Tooltip("The direction of the movement")]
        public MovementType movementType;

        [ShowIf("ShowTransformToMove")]
        [Tooltip("The 'Transform' or 'Rect Transform' component that will be targetted by the move effect.")]
        public Transform transformToMove;

        [ShowIf("ShowMoveDistance")]
        [Tooltip("The distance the specified transform component will move locally during the move event.")]
        public float moveDistance;

        // Shared Values
        [ShowIf("ShowFadeSpeed")]
        [Tooltip("The duration in seconds this event will take to complete (E.g. a value of 1 will take one second to complete. A value of 0 will make the event complete instantly).")]
        public float transistionSpeed;

        // Scaling Properties + Values
        private bool originalScaleIsSet = false;
        private Vector3 originalScale;

        [Header("General Transform Settings")]
        [ShowIf("ShowTransformToScale")]
        [Tooltip("The 'Transform' or 'RectTransform' component you wish to manipulate.")]
        public Transform transformToScale;

        [Header("Enlarge Settings")]
        [ShowIf("ShowPercentageSizeIncrease")]
        [Range(1, 5f)]
        [Tooltip("The amount as perctange that the transform's scale will increase, relative to its original scale" +
            " (E.g. A value of 2 will double this transform's scale/size).")]
        public float percentageSizeIncrease = 1f;

        [ShowIf("ShowEnlargeSpeed")]
        [Tooltip("The duration in seconds the given transform will take to scale up to its specified size")]
        public float enlargeSpeed = 0.5f;

        [Header("Shrink Settings")]
        [ShowIf("ShowPercentageSizeDecrease")]
        [Range(0, 1f)]
        [Tooltip("The amount as perctange that the transform's scale will decrease, relative to its original scale" +
            " (E.g. A value of 0.3 will reduce this transform's scale/size to 30% of its original size).")]
        public float percentageSizeDecrease = 1f;

        [ShowIf("ShowShrinkSpeed")]
        [Tooltip("The duration in seconds the given transform will take to scale down to its specified size")]
        public float shrinkSpeed;

        // Wiggle Properties
        private bool originalPositionIsSet = false;
        private Vector3 originalPosition;

        private bool originalRotationIsSet = false;
        private Vector3 originalRotation;

        [Header("Wiggle Settings")]
        [ShowIf("ShowWiggleType")]
        [Tooltip("The type of wiggle event to be triggered.")]
        public WiggleType wiggleType;

        [ShowIf("ShowWiggleType")]
        [Tooltip("The 'Transform' or 'Rect Transform' component that will be targetted by the wiggle effect.")]
        public Transform transformToWiggle;

        [ShowIf("ShowWiggleDistance")]
        [Tooltip("The distance the specified transform component will move locally during the wiggle event.")]
        public float wiggleDistance;

        [ShowIf("ShowWiggleType")]
        [Tooltip("The duration in seconds the event will take to complete a single wiggle loop.")]
        public float wiggleSpeed;

        [ShowIf("ShowWiggleType")]
        [Tooltip("When checked, the wiggle event will loop infintely while the user has their mouse/finger over the element.")]
        public bool wiggleInfinetly = false;

        [ShowIf("ShowNumberOfWiggles")]
        [Tooltip("The amount of times the wiggle event will repeat itself (NOTE: if this value is 0, the wiggle will only occur once).")]
        public int wiggleLoops = 0;

        [ShowIf("ShowRotationDegrees")]
        [Tooltip("The amount in degrees by which the transform's rotation Z value will change, relative to its original rotation" +
            " (E.g. a value of +90 will rotate the transform clockwise to a 90 degree angle. A value of -180 will rotate the transform anti clockwise by 180 degrees, essentially flipping it upside down).")]
        public float rotationDegrees;

        [ShowIf("ShowCursorType")]
        public CursorType cursorType;

        #endregion

        // Accessors + Getters
        #region
        public bool OriginalScaleIsSet
        {
            get { return originalScaleIsSet; }
            private set { originalScaleIsSet = value; }
        }
        public Vector3 OriginalScale
        {
            get { return originalScale; }
            private set { originalScale = value; }
        }
        public bool OriginalPositionIsSet
        {
            get { return originalPositionIsSet; }
            private set { originalPositionIsSet = value; }
        }
        public Vector3 OriginalPosition
        {
            get { return originalPosition; }
            private set { originalPosition = value; }
        }
        public Vector3 OriginalRotation
        {
            get { return originalRotation; }
            private set { originalRotation = value; }
        }
        public bool OriginalRotationIsSet
        {
            get { return originalRotationIsSet; }
            private set { originalRotationIsSet = value; }
        }
        #endregion

        // Misc Logic
        #region
        public void SetOriginalScale(Vector3 originalScale)
        {
            OriginalScale = originalScale;
            OriginalScaleIsSet = true;
        }
        public void SetOriginalPosition(Vector3 originalPosition)
        {
            OriginalPosition = originalPosition;
            OriginalPositionIsSet = true;
        }
        public void SetOriginalRotation(Vector3 originalRotation)
        {
            OriginalRotation = originalRotation;
            OriginalRotationIsSet = true;
        }
        #endregion

        // Conditional Inspector View Logic
        #region
        public bool ShowCursorType()
        {
            return widgetEventType == WidgetEvent.CursorSpriteChange;
        }
        public bool ShowScalingSequence()
        {
            return widgetEventType == WidgetEvent.Breathe;
        }
        public bool ShowWiggleDistance()
        {
            return widgetEventType == WidgetEvent.Wiggle &&
                (wiggleType == WiggleType.SideToSide || wiggleType == WiggleType.UpAndDown);
        }
        public bool ShowWiggleType()
        {
            return widgetEventType == WidgetEvent.Wiggle;
        }
        public bool ShowNumberOfWiggles()
        {
            return widgetEventType == WidgetEvent.Wiggle &&
                wiggleInfinetly == false;
        }
        public bool ShowRotationDegrees()
        {
            return widgetEventType == WidgetEvent.Wiggle &&
                wiggleType == WiggleType.RotateOnTheSpot;
        }
        public bool ShowEnlargeSpeed()
        {
            return widgetEventType == WidgetEvent.Enlarge ||
                widgetEventType == WidgetEvent.Breathe;
        }
        public bool ShowShrinkSpeed()
        {
            return widgetEventType == WidgetEvent.Shrink ||
                widgetEventType == WidgetEvent.Breathe;
        }
        public bool ShowTransformToScale()
        {
            return widgetEventType == WidgetEvent.Enlarge ||
                widgetEventType == WidgetEvent.Shrink ||
                widgetEventType == WidgetEvent.Breathe;
        }
        public bool ShowPercentageSizeIncrease()
        {
            return widgetEventType == WidgetEvent.Enlarge || widgetEventType == WidgetEvent.Breathe;
        }
        public bool ShowPercentageSizeDecrease()
        {
            return widgetEventType == WidgetEvent.Shrink || widgetEventType == WidgetEvent.Breathe;
        }
        public bool ShowDelayProperties()
        {
            return widgetEventType == WidgetEvent.EnableGameObject ||
                widgetEventType == WidgetEvent.DisableGameObject ||
                widgetEventType == WidgetEvent.FadeInCanvasGroup ||
                widgetEventType == WidgetEvent.FadeOutCanvasGroup ||
                widgetEventType == WidgetEvent.FadeInImage;
        }
        public bool ShowEndColour()
        {
            return widgetEventType == WidgetEvent.TransistionTextColour || widgetEventType == WidgetEvent.TransisitionImageColour;
        }
        public bool ShowCanvasField()
        {
            return widgetEventType == WidgetEvent.FadeInCanvasGroup || widgetEventType == WidgetEvent.FadeOutCanvasGroup;
        }
        public bool ShowImageField()
        {
            return widgetEventType == WidgetEvent.FadeInImage ||
                widgetEventType == WidgetEvent.FadeOutImage ||
                widgetEventType == WidgetEvent.TransisitionImageColour;
        }
        public bool ShowFadeSpeed()
        {
            return widgetEventType == WidgetEvent.FadeInCanvasGroup ||
                   widgetEventType == WidgetEvent.FadeOutCanvasGroup ||
                   widgetEventType == WidgetEvent.FadeInImage ||
                   widgetEventType == WidgetEvent.FadeOutImage ||
                   widgetEventType == WidgetEvent.TransisitionImageColour ||
                   widgetEventType == WidgetEvent.TransistionTextColour ||
                   widgetEventType == WidgetEvent.Move;
        }
        public bool ShowMovementType()
        {
            return widgetEventType == WidgetEvent.Move;
        }
        public bool ShowMoveDistance()
        {
            return widgetEventType == WidgetEvent.Move && movementType != MovementType.ReturnToOriginalPosition;
        }
        public bool ShowTransformToMove()
        {
            return widgetEventType == WidgetEvent.Move;
        }
        #endregion
    }
    public enum WidgetEvent
    {
        None = 0,
        Breathe = 13,
        CursorSpriteChange = 16,
        EnableGameObject = 1,
        Enlarge = 11,
        DisableGameObject = 2,
        FadeInCanvasGroup = 5,
        FadeOutCanvasGroup = 6,
        FadeInImage = 7,
        FadeOutImage = 8,
        InvokeFunction = 3,
        Move = 15,
        PlaySound = 4,
        Shrink = 12,
        TransisitionImageColour = 9,
        TransistionTextColour = 10,             
        Wiggle = 14,
    }
    public enum WidgetInputType
    {
        IPointer = 0,
        Collider = 1,
    }
    public enum WiggleType
    {
        RotateOnTheSpot = 0,
        SideToSide = 1,
        UpAndDown = 2,
    }
    public enum ScaleEventOrder
    {
        EnlargeThenShrink = 0,
        ShrinkThenEnlarge = 1,
    }
    public enum MovementType
    {
        Left = 0,
        Right = 1,
        Up = 2,
        Down = 3,
        ReturnToOriginalPosition = 4,
    }
}