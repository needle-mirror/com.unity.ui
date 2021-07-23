using System;

namespace UnityEngine.UIElements
{
    internal class DefaultEventSystem
    {
        private bool isAppFocused => Application.isFocused;

        internal static Func<bool> IsEditorRemoteConnected = () => false;

        private IInput m_Input;
        internal IInput input
        {
            get => m_Input ?? (m_Input = new Input());
            set => m_Input = value;
        }

        private bool ShouldIgnoreEventsOnAppNotFocused()
        {
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.Windows:
                case OperatingSystemFamily.Linux:
                case OperatingSystemFamily.MacOSX:
#if UNITY_EDITOR
                    if (IsEditorRemoteConnected())
                        return false;
#endif
                    return true;
                default:
                    return false;
            }
        }

        private readonly string m_HorizontalAxis = "Horizontal";
        private readonly string m_VerticalAxis = "Vertical";
        private readonly string m_SubmitButton = "Submit";
        private readonly string m_CancelButton = "Cancel";
        private readonly float m_InputActionsPerSecond = 10;
        private readonly float m_RepeatDelay = 0.5f;

        private bool m_SendingTouchEvents;

        private Event m_Event = new Event();
        private BaseRuntimePanel m_FocusedPanel;

        public BaseRuntimePanel focusedPanel
        {
            get => m_FocusedPanel;
            set
            {
                if (m_FocusedPanel != value)
                {
                    m_FocusedPanel?.Blur();
                    m_FocusedPanel = value;
                    m_FocusedPanel?.Focus();
                }
            }
        }

        public enum UpdateMode
        {
            Always,
            IgnoreIfAppNotFocused
        }

        public void Update(UpdateMode updateMode = UpdateMode.Always)
        {
            if (!isAppFocused && ShouldIgnoreEventsOnAppNotFocused() && updateMode == UpdateMode.IgnoreIfAppNotFocused)
                return;

            // touch needs to take precedence because of the mouse emulation layer
            m_SendingTouchEvents = ProcessTouchEvents();

            SendIMGUIEvents();

            SendInputEvents();
        }

        void SendIMGUIEvents()
        {
            while (Event.PopEvent(m_Event))
            {
                if (m_Event.type == EventType.Repaint)
                    continue;

                if (m_Event.type == EventType.KeyUp || m_Event.type == EventType.KeyDown)
                {
                    SendFocusBasedEvent(self => UIElementsRuntimeUtility.CreateEvent(self.m_Event), this);
                }
                else if (!m_SendingTouchEvents && input.mousePresent)
                {
                    if (m_Event.type == EventType.ScrollWheel)
                    {
                        var screenPosition = GetLocalScreenPosition(m_Event, out var targetDisplay);
                        SendPositionBasedEvent(screenPosition, m_Event.delta, PointerId.mousePointerId, targetDisplay, (panelPosition, panelDelta, self) =>
                        {
                            self.m_Event.mousePosition = panelPosition;
                            return UIElementsRuntimeUtility.CreateEvent(self.m_Event);
                        }, this);
                    }
                    else
                    {
                        var screenPosition = GetLocalScreenPosition(m_Event, out var targetDisplay);
                        SendPositionBasedEvent(screenPosition, m_Event.delta, PointerId.mousePointerId, targetDisplay, (panelPosition, panelDelta, self) =>
                        {
                            self.m_Event.mousePosition = panelPosition;
                            self.m_Event.delta = panelDelta;
                            return UIElementsRuntimeUtility.CreateEvent(self.m_Event);
                        }, this, deselectIfNoTarget: m_Event.type == EventType.MouseDown);
                    }
                }
            }
        }

        void SendInputEvents()
        {
            bool sendNavigationMove = ShouldSendMoveFromInput();

            if (sendNavigationMove)
            {
                SendFocusBasedEvent(self => NavigationMoveEvent.GetPooled(self.GetRawMoveVector()), this);
            }

            if (input.GetButtonDown(m_SubmitButton))
            {
                SendFocusBasedEvent(self => NavigationSubmitEvent.GetPooled(), this);
            }

            if (input.GetButtonDown(m_CancelButton))
            {
                SendFocusBasedEvent(self => NavigationCancelEvent.GetPooled(), this);
            }
        }

        internal void SendFocusBasedEvent<TArg>(Func<TArg, EventBase> evtFactory, TArg arg)
        {
            // Send focus-based events to focused panel if there's one
            if (focusedPanel != null)
            {
                using (EventBase evt = evtFactory(arg))
                {
                    focusedPanel.visualTree.SendEvent(evt);
                    UpdateFocusedPanel(focusedPanel);
                    return;
                }
            }

            // Send Keyboard events to all panels if none is focused.
            // This is so that navigation with Tab can be started without clicking on an element.

            // Try all the panels, from closest to deepest
            var panels = UIElementsRuntimeUtility.GetSortedPlayerPanels();
            for (var i = panels.Count - 1; i >= 0; i--)
            {
                var panel = panels[i];
                if (panel is BaseRuntimePanel runtimePanel)
                {
                    using (EventBase evt = evtFactory(arg))
                    {
                        runtimePanel.visualTree.SendEvent(evt);

                        if (evt.processedByFocusController)
                        {
                            UpdateFocusedPanel(runtimePanel);
                        }

                        if (evt.isPropagationStopped)
                        {
                            break;
                        }
                    }
                }
            }
        }

        // For Unit Tests
        internal void SendPositionBasedEvent<TArg>(Vector3 mousePosition, Vector3 delta, int pointerId,
            Func<Vector3, Vector3, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false) =>
            SendPositionBasedEvent(mousePosition, delta, pointerId, null, evtFactory, arg, deselectIfNoTarget);

        void SendPositionBasedEvent<TArg>(Vector3 mousePosition, Vector3 delta, int pointerId, int? targetDisplay, Func<Vector3, Vector3, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false)
        {
            // Allow focus to be lost before processing the event
            if (focusedPanel != null)
            {
                UpdateFocusedPanel(focusedPanel);
            }

            // We first try to send the event to a runtime panel that might be capturing the pointer, regardless of targetDisplay.
            var panels = UIElementsRuntimeUtility.GetSortedPlayerPanels();
            for (var i = panels.Count - 1; i >= 0; i--)
            {
                var panel = panels[i];
                if (panel is BaseRuntimePanel runtimePanel)
                {
                    if (panel.GetCapturingElement(pointerId) is VisualElement ve && ve.panel == panel)
                    {
                        runtimePanel.ScreenToPanel(mousePosition, delta, out var panelPosition, out var panelDelta, true);

                        using (EventBase evt = evtFactory(panelPosition, panelDelta, arg))
                        {
                            runtimePanel.visualTree.SendEvent(evt);

                            if (evt.processedByFocusController)
                            {
                                UpdateFocusedPanel(runtimePanel);
                            }

                            // Only 1 panel receives the event.
                            return;
                        }
                    }
                }
            }

            // Find a candidate panel for the event
            // Try all the panels, from closest to deepest
            BaseRuntimePanel candidatePanel = null;
            Vector2 candidateMousePosition = Vector2.zero;
            Vector2 candidateDelta = Vector2.zero;
            for (var i = panels.Count - 1; i >= 0; i--)
            {
                if (panels[i] is BaseRuntimePanel runtimePanel && (targetDisplay == null || runtimePanel.targetDisplay == targetDisplay))
                {
                    if (runtimePanel.ScreenToPanel(mousePosition, delta, out candidateMousePosition, out candidateDelta) &&
                        runtimePanel.Pick(candidateMousePosition) != null)
                    {
                        candidatePanel = runtimePanel;
                        break;
                    }
                }
            }

            BaseRuntimePanel lastActivePanel = PointerDeviceState.GetPanel(pointerId, ContextType.Player) as BaseRuntimePanel;
            if (lastActivePanel != null && lastActivePanel != candidatePanel)
            {
                // Send an event to the last panel the pointer was in, so it can dispatch [Mouse|Pointer][Out|Leave] events.
                lastActivePanel.ScreenToPanel(mousePosition, delta, out var panelPosition, out var panelDelta, true);
                using (EventBase lastActivePanelEvent = evtFactory(panelPosition, panelDelta, arg))
                {
                    lastActivePanel.visualTree.SendEvent(lastActivePanelEvent);
                }
            }

            if (candidatePanel != null)
            {
                using (EventBase evt = evtFactory(candidateMousePosition, candidateDelta, arg))
                {
                    candidatePanel.visualTree.SendEvent(evt);

                    if (evt.processedByFocusController)
                    {
                        UpdateFocusedPanel(candidatePanel);
                    }
                }
            }
            else
            {
                if (lastActivePanel == null)
                {
                    // Mouse and pointer events calls PointerDeviceState.SavePointerPosition in their PreDispatch().
                    // If we did not send any event, we need to manually update the pointer position.
                    PointerDeviceState.SavePointerPosition(pointerId, mousePosition, null, ContextType.Player);
                }

                if (deselectIfNoTarget)
                {
                    focusedPanel = null;
                }
            }
        }

        private void UpdateFocusedPanel(BaseRuntimePanel runtimePanel)
        {
            if (runtimePanel.focusController.focusedElement != null)
            {
                focusedPanel = runtimePanel;
            }
            else if (focusedPanel == runtimePanel)
            {
                focusedPanel = null;
            }
        }

        private static EventBase MakeTouchEvent(Touch touch, EventModifiers modifiers)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    return PointerDownEvent.GetPooled(touch, modifiers);
                case TouchPhase.Moved:
                    return PointerMoveEvent.GetPooled(touch, modifiers);
                case TouchPhase.Stationary:
                    return PointerStationaryEvent.GetPooled(touch, modifiers);
                case TouchPhase.Ended:
                    return PointerUpEvent.GetPooled(touch, modifiers);
                case TouchPhase.Canceled:
                    return PointerCancelEvent.GetPooled(touch, modifiers);
                default:
                    return null;
            }
        }

        private bool ProcessTouchEvents()
        {
            for (int i = 0; i < input.touchCount; ++i)
            {
                Touch touch = input.GetTouch(i);

                if (touch.type == TouchType.Indirect)
                    continue;

                // Flip Y Coordinates.
                touch.position = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(touch.position, out var targetDisplay);
                touch.rawPosition = UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(touch.rawPosition, out _);
                touch.deltaPosition = UIElementsRuntimeUtility.ScreenBottomLeftToPanelDelta(touch.deltaPosition);

                SendPositionBasedEvent(touch.position, touch.deltaPosition, PointerId.touchPointerIdBase + touch.fingerId, targetDisplay, (panelPosition, panelDelta, _touch) =>
                {
                    _touch.position = panelPosition;
                    _touch.deltaPosition = panelDelta;
                    return MakeTouchEvent(_touch, EventModifiers.None);
                }, touch);
            }

            return input.touchCount > 0;
        }

        private Vector2 GetRawMoveVector()
        {
            Vector2 move = Vector2.zero;
            move.x = input.GetAxisRaw(m_HorizontalAxis);
            move.y = input.GetAxisRaw(m_VerticalAxis);

            if (input.GetButtonDown(m_HorizontalAxis))
            {
                if (move.x < 0)
                    move.x = -1f;
                if (move.x > 0)
                    move.x = 1f;
            }

            if (input.GetButtonDown(m_VerticalAxis))
            {
                if (move.y < 0)
                    move.y = -1f;
                if (move.y > 0)
                    move.y = 1f;
            }

            return move;
        }

        private int m_ConsecutiveMoveCount;
        private Vector2 m_LastMoveVector;
        private float m_PrevActionTime;

        private bool ShouldSendMoveFromInput()
        {
            float time = Time.unscaledTime;

            Vector2 movement = GetRawMoveVector();
            if (Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f))
            {
                m_ConsecutiveMoveCount = 0;
                return false;
            }

            // If user pressed key again, always allow event
            bool allow = input.GetButtonDown(m_HorizontalAxis) || input.GetButtonDown(m_VerticalAxis);
            bool similarDir = (Vector2.Dot(movement, m_LastMoveVector) > 0);
            if (!allow)
            {
                // Otherwise, user held down key or axis.
                // If direction didn't change at least 90 degrees, wait for delay before allowing consecutive event.
                if (similarDir && m_ConsecutiveMoveCount == 1)
                    allow = (time > m_PrevActionTime + m_RepeatDelay);
                // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
                else
                    allow = (time > m_PrevActionTime + 1f / m_InputActionsPerSecond);
            }

            if (!allow)
                return false;

            // Debug.Log(m_ProcessingEvent.rawType + " axis:" + m_AllowAxisEvents + " value:" + "(" + x + "," + y + ")");
            var moveDirection = NavigationMoveEvent.DetermineMoveDirection(movement.x, movement.y);

            if (moveDirection != NavigationMoveEvent.Direction.None)
            {
                if (!similarDir)
                    m_ConsecutiveMoveCount = 0;
                m_ConsecutiveMoveCount++;
                m_PrevActionTime = time;
                m_LastMoveVector = movement;
            }
            else
            {
                m_ConsecutiveMoveCount = 0;
            }

            return moveDirection != NavigationMoveEvent.Direction.None;
        }

        static Vector2 GetLocalScreenPosition(Event evt, out int? targetDisplay)
        {
            targetDisplay = null; // TODO: find why evt.displayIndex doesn't work
            return evt.mousePosition;
        }

        internal interface IInput
        {
            bool GetButtonDown(string button);
            float GetAxisRaw(string axis);
            int touchCount { get; }
            Touch GetTouch(int index);
            bool mousePresent { get; }
        }

        internal class Input : IInput
        {
            public bool GetButtonDown(string button) => UnityEngine.Input.GetButtonDown(button);
            public float GetAxisRaw(string axis) => UnityEngine.Input.GetAxis(axis);
            public int touchCount => UnityEngine.Input.touchCount;
            public Touch GetTouch(int index) => UnityEngine.Input.GetTouch(index);
            public bool mousePresent => UnityEngine.Input.mousePresent;
        }
    }
}
