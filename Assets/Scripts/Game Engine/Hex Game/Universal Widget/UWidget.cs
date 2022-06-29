using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HexGameEngine.UWidget
{
    public class UWidget : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {

        // Variables + Component References
        #region
        [Header("Core Properties")]
        [Tooltip("Sets the type of input events this Widget should intercept and listen for.")]
        public WidgetInputType inputType;
        [Tooltip("If true, any animation events playing on this widget will stop when a new input event is triggered.")]
        public bool killPreviousTweensOnNewSequenceStart = true;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Event Data")]
        [Tooltip("Events that will be triggered when the Widget is clicked")]
        [SerializeField] WidgetEventData[] onClickEvents;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Tooltip("Events that will be triggered during the first frame in which the user moves their mouse " +
            "within the volume of the Widget's Rect Transform or Collider component.")]
        [SerializeField] WidgetEventData[] mouseEnterEvents;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Tooltip("Events that will be triggered during the first frame in which the user moves their mouse" +
            " outside the volume of the Widget's Rect Transform or Collider component.")]
        [SerializeField] WidgetEventData[] mouseExitEvents;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Tooltip("Events that will be triggered when the UI element is disabled")]
        [SerializeField] WidgetEventData[] onDisableEvents;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Input State")]
        private float timeSinceLastPointerEnter;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]

        [Header("Misc Properties")]
        private bool hasRunSetup = false;

        private static UWidget mousedOver;
        #endregion

        //  Properties + Accessors
        #region
        public static UWidget MousedOver
        {
            get { return mousedOver; }
            private set { mousedOver = value; }
        }       
        public float TimeSinceLastPointerEnter
        {
            get { return timeSinceLastPointerEnter; }
            private set { timeSinceLastPointerEnter = value; }
        }

        public WidgetEventData[] MouseEnterEvents
        {
            get { return mouseEnterEvents; }
            private set { mouseEnterEvents = value; }
        }
        public WidgetEventData[] OnClickEvents
        {
            get { return onClickEvents; }
            private set { onClickEvents = value; }
        }
        public WidgetEventData[] MouseExitEvents
        {
            get { return mouseExitEvents; }
            private set { mouseExitEvents = value; }
        }
        public WidgetEventData[] OnDisableEvents
        {
            get { return onDisableEvents; }
            private set { onDisableEvents = value; }
        }
        #endregion

        // Input Listeners
        #region
        public void OnPointerClick(PointerEventData eventData)
        {
            if (UWidgetController.Instance != null && inputType == WidgetInputType.IPointer)
            {
                UWidgetController.Instance.HandleWidgetEvents(this, OnClickEvents);
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (MousedOver != this && UWidgetController.Instance != null && inputType == WidgetInputType.IPointer)
            {
                MousedOver = this;
                TimeSinceLastPointerEnter = Time.realtimeSinceStartup;
                UWidgetController.Instance.HandleWidgetEvents(this, MouseEnterEvents);
            }

        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (UWidgetController.Instance != null && inputType == WidgetInputType.IPointer)
            {
                if (MousedOver == this) MousedOver = null;
                UWidgetController.Instance.HandleWidgetEvents(this, MouseExitEvents);
            }

        }

        public void OnMouseDown()
        {
            if (UWidgetController.Instance != null && inputType == WidgetInputType.Collider)
            {
                UWidgetController.Instance.HandleWidgetEvents(this, OnClickEvents);
            }
        }
        public void OnMouseEnter()
        {
            if (MousedOver != this && UWidgetController.Instance != null && inputType == WidgetInputType.Collider)
            {
                MousedOver = this;
                TimeSinceLastPointerEnter = Time.realtimeSinceStartup;
                UWidgetController.Instance.HandleWidgetEvents(this, MouseEnterEvents);
            }
        }
        public void OnMouseExit()
        {
            if (UWidgetController.Instance != null && inputType == WidgetInputType.Collider)
            {
                if (MousedOver == this) MousedOver = null;
                UWidgetController.Instance.HandleWidgetEvents(this, MouseExitEvents);
            }
        }
        void OnDisable()
        {
            if (UWidgetController.Instance != null)
                UWidgetController.Instance.HandleWidgetEvents(this, OnDisableEvents);
        }
        #endregion

        // Setup + Initialization
        #region
        void Start()
        {
            // Runs the setup as soon as the application is launched.
            // NOTE: 'Start' is only executed on game objects that are active, if the
            // game object is disabled when the application starts, the set up will not run.
            // To remedy this, whenever this game object is enabled, it will check if the set up
            // has already executed. If it hasn't, it will run the set up as part of the 'OnEnable' event.
            if (!hasRunSetup)
            {           
                RunSetup();
            }
        }
        void OnEnable()
        {
            // If the set up was not executed during 'Start' (because this game object was disabled)
            // then run the setup on first enable.
            if (!hasRunSetup)
            {
                RunSetup();
            }
        }
        void RunSetup()
        {
            Debug.Log("Running setup on " + gameObject.name);
            // Set and cache original scaling values of transforms for shrink/enlarge/etc events,
            // but only if the widget event manipulates a transform's scale in some way

            // Set up on click events
            for (int i = 0; i < onClickEvents.Length; i++)
            {
                if (OnClickEvents[i].transformToScale != null && !OnClickEvents[i].OriginalScaleIsSet)
                {
                    OnClickEvents[i].SetOriginalScale(OnClickEvents[i].transformToScale.localScale);
                }
                if (OnClickEvents[i].transformToWiggle != null && !OnClickEvents[i].OriginalPositionIsSet)
                {
                    OnClickEvents[i].SetOriginalPosition(OnClickEvents[i].transformToWiggle.localPosition);
                }
                if (OnClickEvents[i].transformToWiggle != null && !OnClickEvents[i].OriginalRotationIsSet)
                {
                    OnClickEvents[i].SetOriginalRotation(OnClickEvents[i].transformToWiggle.localRotation.eulerAngles);
                }
                if (OnClickEvents[i].transformToMove != null && !OnClickEvents[i].OriginalPositionIsSet)
                {
                    OnClickEvents[i].SetOriginalPosition(OnClickEvents[i].transformToMove.localPosition);
                }

            }

            // Set up on mouse enter events
            for (int i = 0; i < MouseEnterEvents.Length; i++)
            {
                if (MouseEnterEvents[i].transformToScale != null && !MouseEnterEvents[i].OriginalScaleIsSet)
                {
                    MouseEnterEvents[i].SetOriginalScale(MouseEnterEvents[i].transformToScale.localScale);
                }
                if (MouseEnterEvents[i].transformToWiggle != null && !MouseEnterEvents[i].OriginalPositionIsSet)
                {
                    MouseEnterEvents[i].SetOriginalPosition(MouseEnterEvents[i].transformToWiggle.localPosition);
                }
                if (MouseEnterEvents[i].transformToWiggle != null && !MouseEnterEvents[i].OriginalRotationIsSet)
                {
                    MouseEnterEvents[i].SetOriginalRotation(MouseEnterEvents[i].transformToWiggle.localRotation.eulerAngles);
                }
                if (MouseEnterEvents[i].transformToMove != null && !MouseEnterEvents[i].OriginalPositionIsSet)
                {
                    MouseEnterEvents[i].SetOriginalPosition(MouseEnterEvents[i].transformToMove.localPosition);
                }
            }

            // Set up on mouse exit events
            for (int i = 0; i < MouseExitEvents.Length; i++)
            {
                if (MouseExitEvents[i].transformToScale != null && !MouseExitEvents[i].OriginalScaleIsSet)
                {
                    MouseExitEvents[i].SetOriginalScale(MouseExitEvents[i].transformToScale.localScale);
                }
                if (MouseExitEvents[i].transformToWiggle != null && !MouseExitEvents[i].OriginalPositionIsSet)
                {
                    MouseExitEvents[i].SetOriginalPosition(MouseExitEvents[i].transformToWiggle.localPosition);
                }
                if (MouseExitEvents[i].transformToWiggle != null && !MouseExitEvents[i].OriginalRotationIsSet)
                {
                    MouseExitEvents[i].SetOriginalRotation(MouseExitEvents[i].transformToWiggle.localRotation.eulerAngles);
                }
                if (MouseExitEvents[i].transformToMove != null && !MouseExitEvents[i].OriginalPositionIsSet)
                {
                    MouseExitEvents[i].SetOriginalPosition(MouseExitEvents[i].transformToMove.localPosition);
                }
            }

            // Disable events
            for (int i = 0; i < OnDisableEvents.Length; i++)
            {
                if (OnDisableEvents[i].transformToScale != null && !OnDisableEvents[i].OriginalScaleIsSet)
                {
                    OnDisableEvents[i].SetOriginalScale(OnDisableEvents[i].transformToScale.localScale);
                }
                if (OnDisableEvents[i].transformToWiggle != null && !OnDisableEvents[i].OriginalPositionIsSet)
                {
                    OnDisableEvents[i].SetOriginalPosition(OnDisableEvents[i].transformToWiggle.localPosition);
                }
                if (OnDisableEvents[i].transformToWiggle != null && !OnDisableEvents[i].OriginalRotationIsSet)
                {
                    OnDisableEvents[i].SetOriginalRotation(OnDisableEvents[i].transformToWiggle.localRotation.eulerAngles);
                }
                if (OnDisableEvents[i].transformToMove != null && !OnDisableEvents[i].OriginalPositionIsSet)
                {
                    OnDisableEvents[i].SetOriginalPosition(OnDisableEvents[i].transformToMove.localPosition);
                }
            }

            hasRunSetup = true;
        }
        #endregion
    }
}
