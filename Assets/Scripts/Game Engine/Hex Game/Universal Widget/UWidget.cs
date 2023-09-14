using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using WeAreGladiators.UI;

namespace WeAreGladiators.UWidget
{
    public class UWidget : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
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
        [Tooltip("Events that will be triggered when the Widget is clicked down and up using the left mouse button")]
        [SerializeField]
        private WidgetEventData[] onClickEvents;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [Tooltip("Events that will be triggered when the Widget is clicked down and up using the right mouse button")]
        [SerializeField]
        private WidgetEventData[] onRightClickEvents;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [Tooltip("Events that will be triggered during the first frame in which the user left/right clicks down while their mouse " +
            "is within the volume of the Widget's Rect Transform or Collider component.")]
        [SerializeField]
        private WidgetEventData[] mouseDownEvents;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [Tooltip("Events that will be triggered during the first frame in which the user moves their mouse " +
            "within the volume of the Widget's Rect Transform or Collider component.")]
        [SerializeField]
        private WidgetEventData[] mouseEnterEvents;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [Tooltip("Events that will be triggered during the first frame in which the user moves their mouse" +
            " outside the volume of the Widget's Rect Transform or Collider component.")]
        [SerializeField]
        private WidgetEventData[] mouseExitEvents;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [Tooltip("Events that will be triggered when the UI element is disabled")]
        [SerializeField]
        private WidgetEventData[] onDisableEvents;
        [PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [Header("Misc Properties")]
        private bool hasRunSetup;

        private bool didMouseDown;

        #endregion

        //  Properties + Accessors
        #region

        public static UWidget MousedOver { get; private set; }
        [field: PropertySpace(SpaceBefore = 20, SpaceAfter = 0)]
        [field: Header("Input State")]
        public float TimeSinceLastPointerEnter { get; private set; }
        public WidgetEventData[] MouseDownEvents
        {
            get => mouseDownEvents;
            private set => mouseDownEvents = value;
        }
        public WidgetEventData[] MouseEnterEvents
        {
            get => mouseEnterEvents;
            private set => mouseEnterEvents = value;
        }
        public WidgetEventData[] OnClickEvents
        {
            get => onClickEvents;
            private set => onClickEvents = value;
        }
        public WidgetEventData[] OnRightClickEvents
        {
            get => onRightClickEvents;
            private set => onRightClickEvents = value;
        }
        public WidgetEventData[] MouseExitEvents
        {
            get => mouseExitEvents;
            private set => mouseExitEvents = value;
        }
        public WidgetEventData[] OnDisableEvents
        {
            get => onDisableEvents;
            private set => onDisableEvents = value;
        }

        #endregion

        // Input Listeners
        #region

        public void OnPointerUp(PointerEventData eventData)
        {
            if (UWidgetController.Instance != null && inputType == WidgetInputType.IPointer && didMouseDown)
            {
                if (eventData.button == PointerEventData.InputButton.Right)
                {
                    UWidgetController.Instance.HandleWidgetEvents(this, OnRightClickEvents, "Right Click");
                }
                else if (eventData.button == PointerEventData.InputButton.Left)
                {
                    UWidgetController.Instance.HandleWidgetEvents(this, OnClickEvents, "Left Click");
                }
                didMouseDown = false;
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (UWidgetController.Instance != null && inputType == WidgetInputType.IPointer)
            {
                didMouseDown = true;
                UWidgetController.Instance.HandleWidgetEvents(this, MouseDownEvents, "Mouse Down");
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (MousedOver != this && UWidgetController.Instance != null && inputType == WidgetInputType.IPointer)
            {
                didMouseDown = false;
                MousedOver = this;
                TimeSinceLastPointerEnter = Time.realtimeSinceStartup;
                UWidgetController.Instance.HandleWidgetEvents(this, MouseEnterEvents, "Mouse Enter");
            }

        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (UWidgetController.Instance != null && inputType == WidgetInputType.IPointer)
            {
                didMouseDown = false;
                if (MousedOver == this)
                {
                    CursorController.Instance.SetCursor(CursorController.Instance.FallbackCursor.typeTag);
                    MousedOver = null;
                }
                UWidgetController.Instance.HandleWidgetEvents(this, MouseExitEvents, "Mouse Exit");
            }

        }
        private void OnMouseDown()
        {
            if (UWidgetController.Instance != null && inputType == WidgetInputType.Collider)
            {
                didMouseDown = true;
                UWidgetController.Instance.HandleWidgetEvents(this, MouseDownEvents, "Mouse Down");
            }
        }

        public void OnMouseOver()
        {
            if (inputType == WidgetInputType.Collider &&
                (Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Mouse0)))
            {
                didMouseDown = true;
            }

            if (UWidgetController.Instance != null && inputType == WidgetInputType.Collider && didMouseDown)
            {
                if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    UWidgetController.Instance.HandleWidgetEvents(this, OnRightClickEvents, "Right Click");
                }
                else if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    UWidgetController.Instance.HandleWidgetEvents(this, OnClickEvents, "Left Click");
                }

                didMouseDown = false;
            }
        }
        public void OnMouseEnter()
        {
            if (MousedOver != this && UWidgetController.Instance != null && inputType == WidgetInputType.Collider)
            {
                didMouseDown = false;
                MousedOver = this;
                TimeSinceLastPointerEnter = Time.realtimeSinceStartup;
                UWidgetController.Instance.HandleWidgetEvents(this, MouseEnterEvents, "Mouse Enter");
            }
        }
        public void OnMouseExit()
        {
            if (UWidgetController.Instance != null && inputType == WidgetInputType.Collider)
            {
                didMouseDown = false;
                if (MousedOver == this)
                {
                    CursorController.Instance.SetCursor(CursorController.Instance.FallbackCursor.typeTag);
                    MousedOver = null;
                }
                UWidgetController.Instance.HandleWidgetEvents(this, MouseExitEvents, "Mouse Exit");
            }
        }
        private void OnDisable()
        {
            didMouseDown = false;
            if (UWidgetController.Instance != null)
            {
                UWidgetController.Instance.HandleWidgetEvents(this, OnDisableEvents, "On Disabled");
            }
        }

        #endregion

        // Setup + Initialization
        #region

        private void Start()
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
        private void OnEnable()
        {
            // If the set up was not executed during 'Start' (because this game object was disabled)
            // then run the setup on first enable.
            if (!hasRunSetup)
            {
                RunSetup();
            }
        }
        private void RunSetup()
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

            // Set up on right click events
            for (int i = 0; i < onRightClickEvents.Length; i++)
            {
                if (OnRightClickEvents[i].transformToScale != null && !OnRightClickEvents[i].OriginalScaleIsSet)
                {
                    OnRightClickEvents[i].SetOriginalScale(OnRightClickEvents[i].transformToScale.localScale);
                }
                if (OnRightClickEvents[i].transformToWiggle != null && !OnRightClickEvents[i].OriginalPositionIsSet)
                {
                    OnRightClickEvents[i].SetOriginalPosition(OnRightClickEvents[i].transformToWiggle.localPosition);
                }
                if (OnRightClickEvents[i].transformToWiggle != null && !OnRightClickEvents[i].OriginalRotationIsSet)
                {
                    OnRightClickEvents[i].SetOriginalRotation(OnRightClickEvents[i].transformToWiggle.localRotation.eulerAngles);
                }
                if (OnRightClickEvents[i].transformToMove != null && !OnRightClickEvents[i].OriginalPositionIsSet)
                {
                    OnRightClickEvents[i].SetOriginalPosition(OnRightClickEvents[i].transformToMove.localPosition);
                }

            }

            // Set up on mouse down events
            for (int i = 0; i < MouseDownEvents.Length; i++)
            {
                if (MouseDownEvents[i].transformToScale != null && !MouseDownEvents[i].OriginalScaleIsSet)
                {
                    MouseDownEvents[i].SetOriginalScale(MouseDownEvents[i].transformToScale.localScale);
                }
                if (MouseDownEvents[i].transformToWiggle != null && !MouseDownEvents[i].OriginalPositionIsSet)
                {
                    MouseDownEvents[i].SetOriginalPosition(MouseDownEvents[i].transformToWiggle.localPosition);
                }
                if (MouseDownEvents[i].transformToWiggle != null && !MouseDownEvents[i].OriginalRotationIsSet)
                {
                    MouseDownEvents[i].SetOriginalRotation(MouseDownEvents[i].transformToWiggle.localRotation.eulerAngles);
                }
                if (MouseDownEvents[i].transformToMove != null && !MouseDownEvents[i].OriginalPositionIsSet)
                {
                    MouseDownEvents[i].SetOriginalPosition(MouseDownEvents[i].transformToMove.localPosition);
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
