// NOTE: We do a lot of work in this class to avoid treating pointer events in all cases. This is done to keep
// compatibility with previous propagation behaviour of mouse/pointer events and avoid introducing breaking changes.
// We moved pointer events support in this class and removed PointerClickable.cs in a first step towards handling
// only pointer events as the default, and slowly moving away from mouse events which are not touch screen friendly.

namespace UnityEngine.UIElements
{
    /// <summary>
    /// Manipulator that tracks Mouse events on an element and callbacks when the elements is clicked.
    /// </summary>
    public class Clickable : PointerManipulator
    {
        /// <summary>
        /// Callback triggered when the target element is clicked, including event data.
        /// </summary>
        public event System.Action<EventBase> clickedWithEventInfo;
        /// <summary>
        /// Callback triggered when the target element is clicked.
        /// </summary>
        public event System.Action clicked;

        private readonly long m_Delay; // in milliseconds
        private readonly long m_Interval; // in milliseconds

        /// <summary>
        /// This property tracks the activation of the manipulator. Set it to true when the manipulator is activated.
        /// </summary>
        protected bool active { get; set; }

        /// <summary>
        /// Specifies the mouse position saved during the last mouse event on the target Element.
        /// </summary>
        public Vector2 lastMousePosition { get; private set; }

        private IVisualElementScheduledItem m_Repeater;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="delay">Determines when the event begins. Applies if delay > 0.</param>
        /// <param name="interval">Determines the time delta between event repetition. Applies if interval > 0.</param>
        public Clickable(System.Action handler, long delay, long interval) : this(handler)
        {
            m_Delay = delay;
            m_Interval = interval;
            active = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Clickable(System.Action<EventBase> handler)
        {
            clickedWithEventInfo = handler;
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        }

        // Click-once type constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        public Clickable(System.Action handler)
        {
            clicked = handler;

            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });

            active = false;
        }

        private void OnTimer(TimerState timerState)
        {
            if ((clicked != null || clickedWithEventInfo != null) && IsRepeatable())
            {
                if (target.ContainsPoint(lastMousePosition))
                {
                    Invoke(null);
                    target.pseudoStates |= PseudoStates.Active;
                }
                else
                {
                    target.pseudoStates &= ~PseudoStates.Active;
                }
            }
        }

        private bool IsRepeatable()
        {
            return (m_Delay > 0 || m_Interval > 0);
        }

        /// <summary>
        /// Called to register mouse event callbacks on the target element.
        /// </summary>
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            target.RegisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOut);

            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerCancelEvent>(OnPointerCancel);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        /// <summary>
        /// Called to unregister event callbacks from the target element.
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            target.UnregisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOut);

            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerCancelEvent>(OnPointerCancel);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        /// <summary>
        /// This method is called when a MouseDownEvent is sent to the target element.
        /// </summary>
        /// <param name="evt">The event.</param>
        protected void OnMouseDown(MouseDownEvent evt)
        {
            if (CanStartManipulation(evt))
                ProcessDownEvent(evt, evt.localMousePosition, PointerId.mousePointerId);
        }

        /// <summary>
        /// This method is called when a MouseMoveEvent is sent to the target element.
        /// </summary>
        /// <param name="evt">The event.</param>
        protected void OnMouseMove(MouseMoveEvent evt)
        {
            if (active)
                ProcessMoveEvent(evt, evt.localMousePosition);
        }

        /// <summary>
        /// This method is called when a MouseUpEvent is sent to the target element.
        /// </summary>
        /// <param name="evt">The event.</param>
        protected void OnMouseUp(MouseUpEvent evt)
        {
            if (active && CanStopManipulation(evt))
                ProcessUpEvent(evt, evt.localMousePosition, PointerId.mousePointerId);
        }

        void OnMouseCaptureOut(MouseCaptureOutEvent evt)
        {
            if (active)
                ProcessCancelEvent(evt, PointerId.mousePointerId);
        }

        /// <summary>
        /// This method is called when a PointerDownEvent is sent to the target element.
        /// </summary>
        /// <param name="evt">The event.</param>
        void OnPointerDown(PointerDownEvent evt)
        {
            if (!CanStartManipulation(evt)) return;

            if (evt.pointerId != PointerId.mousePointerId)
            {
                ProcessDownEvent(evt, evt.localPosition, evt.pointerId);
                target.panel.PreventCompatibilityMouseEvents(evt.pointerId);
            }
            else
            {
#if  !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
                target.panel.PreventCompatibilityMouseEvents(evt.pointerId);
#endif
                evt.StopImmediatePropagation();
            }
        }

        /// <summary>
        /// This method is called when a PointerMoveEvent is sent to the target element.
        /// </summary>
        /// <param name="evt">The event.</param>
        void OnPointerMove(PointerMoveEvent evt)
        {
            if (!active) return;

            if (evt.pointerId != PointerId.mousePointerId)
            {
                ProcessMoveEvent(evt, evt.localPosition);
                target.panel.PreventCompatibilityMouseEvents(evt.pointerId);
            }
            else
            {
#if  !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
                target.panel.PreventCompatibilityMouseEvents(evt.pointerId);
#endif
                evt.StopPropagation();
            }
        }

        /// <summary>
        /// This method is called when a PointerUpEvent is sent to the target element.
        /// </summary>
        /// <param name="evt">The event.</param>
        void OnPointerUp(PointerUpEvent evt)
        {
            if (!active || !CanStopManipulation(evt)) return;

            if (evt.pointerId != PointerId.mousePointerId)
            {
                ProcessUpEvent(evt, evt.localPosition, evt.pointerId);
                target.panel.PreventCompatibilityMouseEvents(evt.pointerId);
            }
            else
            {
#if  !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
                target.panel.PreventCompatibilityMouseEvents(evt.pointerId);
#endif
                evt.StopPropagation();
            }
        }

        /// <summary>
        /// This method is called when a PointerCancelEvent is sent to the target element.
        /// </summary>
        /// <param name="evt">The event.</param>
        void OnPointerCancel(PointerCancelEvent evt)
        {
            if (!active || !CanStopManipulation(evt)) return;

            if (IsNotMouseEvent(evt.pointerId))
            {
                ProcessCancelEvent(evt, evt.pointerId);
            }
        }

        /// <summary>
        /// This method is called when a PointerCaptureOutEvent is sent to the target element.
        /// </summary>
        /// <param name="evt">The event.</param>
        void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            if (!active) return;

            if (IsNotMouseEvent(evt.pointerId))
            {
                ProcessCancelEvent(evt, evt.pointerId);
            }
        }

        static bool IsNotMouseEvent(int pointerId)
        {
            return pointerId != PointerId.mousePointerId;
        }

        /// <summary>
        /// Invokes a click action.
        /// </summary>
        protected void Invoke(EventBase evt)
        {
            clicked?.Invoke();
            clickedWithEventInfo?.Invoke(evt);
        }

        internal void SimulateSingleClick(EventBase evt, int delayMs = 100)
        {
            target.pseudoStates |= PseudoStates.Active;
            target.schedule.Execute(() => target.pseudoStates &= ~PseudoStates.Active).ExecuteLater(delayMs);
            Invoke(evt);
        }

        /// <summary>
        /// This method processes the down event sent to the target Element.
        /// </summary>
        protected virtual void ProcessDownEvent(EventBase evt, Vector2 localPosition, int pointerId)
        {
            active = true;
            target.CapturePointer(pointerId);
            if (!(evt is IPointerEvent))
                target.panel.ProcessPointerCapture(pointerId);

            lastMousePosition = localPosition;
            if (IsRepeatable())
            {
                // Repeatable button clicks are performed on the MouseDown and at timer events
                if (target.ContainsPoint(localPosition))
                {
                    Invoke(evt);
                }

                if (m_Repeater == null)
                {
                    m_Repeater = target.schedule.Execute(OnTimer).Every(m_Interval).StartingIn(m_Delay);
                }
                else
                {
                    m_Repeater.ExecuteLater(m_Delay);
                }
            }

            target.pseudoStates |= PseudoStates.Active;

            evt.StopImmediatePropagation();
        }

        /// <summary>
        /// This method processes the move event sent to the target Element.
        /// </summary>
        protected virtual void ProcessMoveEvent(EventBase evt, Vector2 localPosition)
        {
            lastMousePosition = localPosition;

            if (target.ContainsPoint(localPosition))
            {
                target.pseudoStates |= PseudoStates.Active;
            }
            else
            {
                target.pseudoStates &= ~PseudoStates.Active;
            }

            evt.StopPropagation();
        }

        /// <summary>
        /// This method processes the up event sent to the target Element.
        /// </summary>
        protected virtual void ProcessUpEvent(EventBase evt, Vector2 localPosition, int pointerId)
        {
            active = false;
            target.ReleasePointer(pointerId);
            if (!(evt is IPointerEvent))
                target.panel.ProcessPointerCapture(pointerId);

            target.pseudoStates &= ~PseudoStates.Active;

            if (IsRepeatable())
            {
                // Repeatable button clicks are performed on the MouseDown and at timer events only
                m_Repeater?.Pause();
            }
            else
            {
                // Non repeatable button clicks are performed on the MouseUp
                if (target.ContainsPoint(localPosition))
                {
                    Invoke(evt);
                }
            }

            evt.StopPropagation();
        }

        /// <summary>
        /// This method processes the up cancel sent to the target Element.
        /// </summary>
        protected virtual void ProcessCancelEvent(EventBase evt, int pointerId)
        {
            active = false;
            target.ReleasePointer(pointerId);
            if (!(evt is IPointerEvent))
                target.panel.ProcessPointerCapture(pointerId);

            target.pseudoStates &= ~PseudoStates.Active;

            if (IsRepeatable())
            {
                m_Repeater?.Pause();
            }

            evt.StopPropagation();
        }
    }
}
