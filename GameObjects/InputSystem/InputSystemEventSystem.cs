#if UNITY_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements.Collections;

namespace UnityEngine.UIElements
{
    /// <summary>
    /// Handles input and sending events to UIElements Panel through use of Unity's Input System package.
    /// </summary>
    public partial class EventSystem : MonoBehaviour
    {
        [Tooltip("The Initial delay (in seconds) between an initial keyboard action and a repeated action.")]
        [SerializeField] internal float m_RepeatDelay = 0.5f;

        [Tooltip("The speed (in seconds) that the keyboard action repeats itself once repeating (max 1 per frame).")]
        [SerializeField] internal float m_RepeatRate = 0.05f;

        [SerializeField] private bool m_FallbackOnIMGUIKeyboardEvents = false;

        [SerializeField] private InputActionAsset m_InputActionAsset = null;
        [SerializeField, InputActionDropdown(nameof(m_InputActionAsset))] private InputActionReference m_NavigateAction;
        [SerializeField, InputActionDropdown(nameof(m_InputActionAsset))] private InputActionReference m_TabAction;
        [SerializeField, InputActionDropdown(nameof(m_InputActionAsset))] private InputActionReference m_SubmitAction;
        [SerializeField, InputActionDropdown(nameof(m_InputActionAsset))] private InputActionReference m_CancelAction;
        [SerializeField, InputActionDropdown(nameof(m_InputActionAsset))] private InputActionReference m_PointAction;
        [SerializeField, InputActionDropdown(nameof(m_InputActionAsset))] private InputActionReference m_ClickAction;
        [SerializeField, InputActionDropdown(nameof(m_InputActionAsset))] private InputActionReference m_ScrollWheelAction;
        [SerializeField, InputActionDropdown(nameof(m_InputActionAsset))] private InputActionReference m_MiddleClickAction;
        [SerializeField, InputActionDropdown(nameof(m_InputActionAsset))] private InputActionReference m_RightClickAction;

        private event Action OnDisableEvent;

        private RuntimePanel m_FocusedPanel;

        internal RuntimePanel focusedPanel
        {
            get => m_FocusedPanel;
            set
            {
                if (m_FocusedPanel != value)
                {
                    m_FocusedPanel?.focusController.BlurLastFocusedElement();
                    m_FocusedPanel = value;
                }
            }
        }

        private readonly Dictionary<int, Vector2> m_LastPointedPositions =
            new Dictionary<int, Vector2>();

        private Vector2 m_LastPointedPosition;
        private Vector2 m_LastPointedDelta;

        private IKeyboardEventProcessor m_KeyboardEventProcessor;

        void Awake()
        {
#if UNITY_EDITOR
            var playerSettings = Resources.FindObjectsOfTypeAll<UnityEditor.PlayerSettings>().FirstOrDefault();
            var enableNativePlatformBackendsForNewInputSystem = new UnityEditor.SerializedObject(playerSettings).FindProperty("enableNativePlatformBackendsForNewInputSystem");
            if (enableNativePlatformBackendsForNewInputSystem != null && !enableNativePlatformBackendsForNewInputSystem.boolValue)
            {
                Debug.LogError("EventSystem has detected new Input System package but can't currently use it. Please enable \"Input System Package (New)\" in ProjectSettings/Player/Input or uninstall Input System package.");
            }
#endif

            m_KeyboardEventProcessor = m_FallbackOnIMGUIKeyboardEvents
                ? (IKeyboardEventProcessor) new IMGUIKeyboardEventProcessor()
                : new InputSystemKeyboardEventProcessor();
        }

        void OnEnable()
        {
            RegisterCallback(m_NavigateAction, OnNavigatePerformed, OnNavigationCanceled);
            RegisterCallback(m_TabAction, OnTabPerformed, OnNavigationCanceled);
            RegisterCallback(m_SubmitAction, OnSubmitPerformed, OnNavigationCanceled);
            RegisterCallback(m_CancelAction, OnCancelPerformed, OnNavigationCanceled);
            RegisterCallback(m_PointAction, OnPointPerformed);
            RegisterCallback(m_ClickAction, OnClickPerformed);
            RegisterCallback(m_ScrollWheelAction, OnScrollWheelPerformed);
            RegisterCallback(m_MiddleClickAction, OnMiddleClickPerformed);
            RegisterCallback(m_RightClickAction, OnRightClickPerformed);

            m_KeyboardEventProcessor.OnEnable();
        }

        void OnDisable()
        {
            OnDisableEvent?.Invoke();
            OnDisableEvent = null;

            m_KeyboardEventProcessor.OnDisable();
        }

        void Update()
        {
            if (!isAppFocused && ShouldIgnoreEventsOnAppNotFocused())
                return;

            m_KeyboardEventProcessor.ProcessKeyboardEvents(this);
            CheckForRepeatedNavigationEvents();
        }

        void Reset()
        {
#if UNITY_EDITOR
            m_InputActionAsset = Resources.FindObjectsOfTypeAll<InputActionAsset>().FirstOrDefault();
#endif
            SetActionsToDefault();
        }

        [ContextMenu("Set Actions to Default")]
        private void _setActionsToDefault() => SetActionsToDefault();

        internal void SetActionsToDefault(bool forced = false)
        {
            InitAction(ref m_NavigateAction, "UI/Navigate", forced);
            InitAction(ref m_TabAction, "UI/Tab", forced);
            InitAction(ref m_SubmitAction, "UI/Submit", forced);
            InitAction(ref m_CancelAction, "UI/Cancel", forced);
            InitAction(ref m_PointAction, "UI/Point", forced);
            InitAction(ref m_ClickAction, "UI/Click", forced);
            InitAction(ref m_ScrollWheelAction, "UI/ScrollWheel", forced);
            InitAction(ref m_MiddleClickAction, "UI/MiddleClick", forced);
            InitAction(ref m_RightClickAction, "UI/RightClick", forced);
        }

        private void InitAction(ref InputActionReference action, string path, bool forced)
        {
            if (m_InputActionAsset == null)
            {
                action = null;
            }
            else if (action == null || action.asset != m_InputActionAsset || forced)
            {
#if UNITY_EDITOR
                action = (InputActionReference)UnityEditor.AssetDatabase
                    .LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(m_InputActionAsset))
                    .FirstOrDefault(o => o is InputActionReference r && r.name == path);
#else
                action = InputActionReference.Create(m_InputActionAsset.FindAction(path));
#endif
            }
        }

        private long m_LastNavigationEventId = -1;
        private float m_LastNavigationEventTime;
        private InputAction m_LastNavigationInputAction;
        private InputControl m_LastNavigationInputControl;
        private Vector2 m_LastNavigationMovement;
        private int m_LastTabDirection;

        private void OnNavigatePerformed(InputAction.CallbackContext context)
        {
            var movement = context.ReadValue<Vector2>();

            if (!Mathf.Approximately(movement.x, 0f) || !Mathf.Approximately(movement.y, 0f))
            {
                SendFocusBasedEvent(context, movement => NavigationMoveEvent.GetPooled(movement), movement);
                OnNavigationStarted(context, NavigationMoveEvent.TypeId());
                m_LastNavigationMovement = movement;
            }
        }

        private void OnTabPerformed(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<float>();

            var direction = Mathf.RoundToInt(value);
            if (direction != 0)
            {
                SendFocusBasedEvent(context, direction => NavigationTabEvent.GetPooled(direction), direction);
                OnNavigationStarted(context, NavigationTabEvent.TypeId());
                m_LastTabDirection = direction;
            }
        }

        private void OnSubmitPerformed(InputAction.CallbackContext context)
        {
            SendFocusBasedEvent(context, _ => NavigationSubmitEvent.GetPooled(), 0);
            OnNavigationStarted(context, NavigationSubmitEvent.TypeId());
        }

        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            SendFocusBasedEvent(context, _ => NavigationCancelEvent.GetPooled(), 0);
            OnNavigationStarted(context, NavigationCancelEvent.TypeId());
        }

        private void OnNavigationStarted(InputAction.CallbackContext context, long eventTypeId)
        {
            m_LastNavigationEventId = eventTypeId;
            m_LastNavigationEventTime = Time.unscaledTime;
            m_LastNavigationInputAction = context.action;
            m_LastNavigationInputControl = context.control;
        }

        private void OnNavigationCanceled(InputAction.CallbackContext context)
        {
            m_LastNavigationEventId = -1;
        }

        private void CheckForRepeatedNavigationEvents()
        {
            if (m_LastNavigationEventId >= 0 && Time.unscaledTime >= m_LastNavigationEventTime + m_RepeatDelay)
            {
                m_LastNavigationEventTime += m_RepeatRate;

                var context = new InputContext(m_LastNavigationInputAction, m_LastNavigationInputControl);
                if (m_LastNavigationEventId == NavigationMoveEvent.TypeId())
                {
                    SendFocusBasedEvent(context, t => NavigationMoveEvent.GetPooled(t.m_LastNavigationMovement), this);
                }
                if (m_LastNavigationEventId == NavigationTabEvent.TypeId())
                {
                    SendFocusBasedEvent(context, t => NavigationTabEvent.GetPooled(t.m_LastTabDirection), this);
                }
                else if (m_LastNavigationEventId == NavigationSubmitEvent.TypeId())
                {
                    SendFocusBasedEvent(context, t => NavigationSubmitEvent.GetPooled(), this);
                }
                else if (m_LastNavigationEventId == NavigationCancelEvent.TypeId())
                {
                    SendFocusBasedEvent(context, t => NavigationCancelEvent.GetPooled(), this);
                }
            }
        }

        private void OnScrollWheelPerformed(InputAction.CallbackContext context)
        {
            m_PointerEvent.ReadDeviceState(context);
            var mousePosition = m_LastPointedPositions.Get(m_PointerEvent.pointerId, m_LastPointedPosition);
            var delta = context.ReadValue<Vector2>();
            delta *= Time.deltaTime;
            SendFocusBasedEvent(context, t => WheelEvent.GetPooled(t.delta, t.mousePosition),
                (delta, mousePosition));
        }

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            var pressed = context.ReadValue<float>();
            OnGenericClickPerformed(context, (int)MouseButton.LeftMouse, pressed);
        }

        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            var pressed = context.ReadValue<float>();
            OnGenericClickPerformed(context, (int)MouseButton.RightMouse, pressed);
        }

        private void OnMiddleClickPerformed(InputAction.CallbackContext context)
        {
            var pressed = context.ReadValue<float>();
            OnGenericClickPerformed(context, (int)MouseButton.MiddleMouse, pressed);
        }

        private void OnPointPerformed(InputAction.CallbackContext context)
        {
            var position = context.ReadValue<Vector2>();
            position.y = Screen.height - position.y;
            OnGenericPointPerformed(context, position);
        }

        private readonly PointerEvent m_PointerEvent = new PointerEvent();
        private void OnGenericClickPerformed(InputAction.CallbackContext context, int button, float pressed)
        {
            m_PointerEvent.ReadDeviceState(context, button);
            var pointerId = m_PointerEvent.pointerId;

            bool SendPointerPressed(Vector2 mousePosition, Vector2 delta)
            {
                //TODO: remove MouseDownEvent and MouseUpEvent, find why they are used in TextField focus pipeline
                return pointerId == PointerId.mousePointerId && SendPositionBasedEvent(context, mousePosition, delta,
                    (panelPosition, panelDelta, data) =>
                    {
                        using (var e = PointerEventWrapper.GetPooled(data, panelPosition, panelDelta))
                            return MouseDownEvent.GetPooled(e);
                    }, m_PointerEvent) ||
                    SendPositionBasedEvent(context, mousePosition, delta,
                    (panelPosition, panelDelta, data) =>
                    {
                        using (var e = PointerEventWrapper.GetPooled(data, panelPosition, panelDelta))
                            return PointerDownEvent.GetPooled((IPointerEvent)e);
                    }, m_PointerEvent);
            }

            bool SendPointerReleased(Vector2 mousePosition, Vector2 delta)
            {
                return pointerId == PointerId.mousePointerId && SendPositionBasedEvent(context, mousePosition, delta,
                    (panelPosition, panelDelta, data) =>
                    {
                        using (var e = PointerEventWrapper.GetPooled(data, panelPosition, panelDelta))
                            return MouseUpEvent.GetPooled(e);
                    }, m_PointerEvent) ||
                    SendPositionBasedEvent(context, mousePosition, delta,
                    (panelPosition, panelDelta, data) =>
                    {
                        using (var e = PointerEventWrapper.GetPooled(data, panelPosition, panelDelta))
                            return PointerUpEvent.GetPooled((IPointerEvent)e);
                    }, m_PointerEvent);
            }

            var position = m_LastPointedPositions.Get(pointerId, m_LastPointedPosition);
            if (pressed > 0)
            {
                SendPointerPressed(position, m_LastPointedDelta);
            }
            else
            {
                SendPointerReleased(position, m_LastPointedDelta);
            }
        }

        private void OnGenericPointPerformed(InputAction.CallbackContext context, Vector2 position)
        {
            m_PointerEvent.ReadDeviceState(context);
            var pointerId = m_PointerEvent.pointerId;

            m_LastPointedDelta = position - m_LastPointedPosition;
            m_LastPointedPosition = position;
            m_LastPointedPositions[pointerId] = position;

            bool SendPointerMoved(Vector2 mousePosition, Vector2 delta)
            {
                return pointerId == PointerId.mousePointerId && SendPositionBasedEvent(context, mousePosition, delta,
                    (panelPosition, panelDelta, data) =>
                    {
                        using (var e = PointerEventWrapper.GetPooled(data, panelPosition, panelDelta))
                            return MouseMoveEvent.GetPooled(e);
                    }, m_PointerEvent) ||
                    SendPositionBasedEvent(context, mousePosition, delta,
                    (panelPosition, panelDelta, data) =>
                    {
                        using (var e = PointerEventWrapper.GetPooled(data, panelPosition, panelDelta))
                            return PointerMoveEvent.GetPooled((IPointerEvent)e);
                    }, m_PointerEvent);
            }

            SendPointerMoved(position, m_LastPointedDelta);
        }

        private class PointerEventWrapper : PointerEventBase<PointerEventWrapper>
        {
            public static PointerEventWrapper GetPooled(IPointerEvent other, Vector3 positionOverride,
                Vector3 deltaOverride)
            {
                var e = GetPooled(other);
                e.localPosition = e.position = positionOverride;
                e.deltaPosition = deltaOverride;
                return e;
            }
        }

        private class PointerEvent : IPointerEvent
        {
            public int pointerId { get; private set; }
            public string pointerType { get; private set; }
            public bool isPrimary { get; private set; }
            public int button { get; private set; }
            public int pressedButtons { get; private set; }
            public Vector3 position { get; private set; }
            public Vector3 localPosition { get; private set; }
            public Vector3 deltaPosition { get; private set; }
            public float deltaTime { get; private set; }
            public int clickCount { get; private set; }
            public float pressure { get; private set; }
            public float tangentialPressure { get; private set; }
            public float altitudeAngle { get; private set; }
            public float azimuthAngle { get; private set; }
            public float twist { get; private set; }
            public Vector2 radius { get; private set; }
            public Vector2 radiusVariance { get; private set; }
            public EventModifiers modifiers { get; private set; }
            public bool shiftKey { get; private set; }
            public bool ctrlKey { get; private set; }
            public bool commandKey { get; private set; }
            public bool altKey { get; private set; }
            public bool actionKey { get; private set; }

            private void ReadDeviceState()
            {
                shiftKey = false;
                ctrlKey = false;
                commandKey = false;
                altKey = false;
                actionKey = false;

                foreach (var device in InputSystem.InputSystem.devices)
                {
                    if (device is Keyboard keyboard)
                    {
                        shiftKey = keyboard.shiftKey.isPressed;
                        ctrlKey = keyboard.ctrlKey.isPressed;
                        commandKey = keyboard.leftCommandKey.isPressed | keyboard.rightCommandKey.isPressed;
                        altKey = keyboard.altKey.isPressed;
                        actionKey = keyboard.leftAppleKey.isPressed | keyboard.rightAppleKey.isPressed | keyboard.leftWindowsKey.isPressed | keyboard.rightWindowsKey.isPressed;
                    }
                }

                //Bug: on Windows, Keyboard.current[Key.NumLock].isPressed doesn't return the persistent state (same for Key.CapsLock)
                var systemEvent = Event.current;
                var numeric = systemEvent?.numeric ?? false;
                var capsLock = systemEvent?.capsLock ?? false;
                var functionKey = systemEvent?.functionKey ?? false;
                modifiers = (shiftKey ? EventModifiers.Shift : 0) |
                    (ctrlKey ? EventModifiers.Control : 0) |
                    (commandKey ? EventModifiers.Command : 0) |
                    (altKey ? EventModifiers.Alt : 0) |
                    (numeric ? EventModifiers.Numeric : 0) |
                    (capsLock ? EventModifiers.CapsLock : 0) |
                    (functionKey ? EventModifiers.FunctionKey : 0);

                pointerId = 0;
                pointerType = PointerType.unknown;
                isPrimary = true;
                deltaTime = 0;
                pressure = 0;
                tangentialPressure = 0;
                altitudeAngle = 0;
                azimuthAngle = 0;
                twist = 0;
                radius = Vector2.zero;
                radiusVariance = Vector2.zero;

                position = systemEvent?.mousePosition ?? default;
                localPosition = systemEvent?.mousePosition ?? default;
                deltaPosition = systemEvent?.delta ?? default;
                clickCount = Mathf.Max(1, systemEvent?.clickCount ?? 0);
                button = systemEvent?.button ?? -1;
                pressedButtons = PointerDeviceState.GetPressedButtons(PointerId.mousePointerId);
            }

            public void ReadDeviceState(InputAction.CallbackContext context, int? buttonOverride = null)
            {
                ReadDeviceState();

                if (context.control.parent is Pen pen)
                {
                    pressure = pen.pressure.EvaluateMagnitude();
                    clickCount = pen.tip.isPressed ? 1 : 0;
                    pointerType = PointerType.pen;
                    pointerId = PointerId.penPointerIdBase; //TODO: find pen index if there are many
                }
                else if (context.control.parent is TouchControl touchControl)
                {
                    pressure = touchControl.pressure.EvaluateMagnitude();
                    radius = touchControl.radius.ReadValue();
                    clickCount = touchControl.tapCount.ReadValue();
                    isPrimary = touchControl == ((Touchscreen)touchControl.device).primaryTouch;
                    pointerType = PointerType.touch;
                    pointerId = GetTouchPointerId(touchControl);
                }
                else
                {
                    if (context.control.parent is Mouse mouse)
                        clickCount = mouse.clickCount.ReadValue();
                    pointerType = PointerType.mouse;
                    pointerId = PointerId.mousePointerId;
                }

                // Move events have button = -1 and clickCount = 0
                if (buttonOverride.HasValue)
                {
                    button = buttonOverride.Value;
                }
                else
                {
                    button = -1;
                    clickCount = 0;
                }
            }

            private static int GetTouchPointerId(TouchControl touchControl)
            {
                var touches = ((Touchscreen)touchControl.device).touches;
                var count = touches.Count;
                for (int i = 0; i < count; i++)
                    if (touchControl == touches[i])
                        return PointerId.touchPointerIdBase + i;
                return PointerId.touchPointerIdBase; // Default to 0-th touch if nothing else is found
            }
        }

        private void SendEvent(IPanel panel, EventBase ev, InputContext context)
        {
            panel.visualTree.SendEvent(ev);
        }

        private void RegisterCallback(InputAction action, Action<InputAction.CallbackContext> performedCallback,
            Action<InputAction.CallbackContext> canceledCallback = null)
        {
            if (action == null)
                return;

            action.Enable();

            if (performedCallback != null)
            {
                action.performed += performedCallback;
                OnDisableEvent += () => action.performed -= performedCallback; //TODO: use signal or IDisposable pooled wrapper to avoid garbage
            }

            if (canceledCallback != null)
            {
                action.canceled += canceledCallback;
                OnDisableEvent += () => action.canceled -= canceledCallback;
            }
        }

        internal bool SendFocusBasedEvent<TArg>(InputContext context, Func<TArg, EventBase> evtFactory, TArg arg)
        {
            // Send focus-based events to focused panel if there's one
            if (focusedPanel != null)
            {
                using (EventBase evt = evtFactory(arg))
                {
                    SendEvent(focusedPanel, evt, context);
                    UpdateFocusedPanel(focusedPanel);
                    return evt.isPropagationStopped;
                }
            }
            // Otherwise try all the panels, from closest to deepest
            else
            {
                var panels = UIElementsRuntimeUtility.GetSortedPlayerPanels();
                for (var i = panels.Count - 1; i >= 0; i--)
                {
                    var panel = panels[i];
                    if (panel is RuntimePanel runtimePanel)
                    {
                        using (EventBase evt = evtFactory(arg))
                        {
                            SendEvent(runtimePanel, evt, context);

                            if (evt.processedByFocusController)
                            {
                                UpdateFocusedPanel(runtimePanel);
                            }

                            if (evt.isPropagationStopped)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        internal bool SendPositionBasedEvent<TArg>(InputAction.CallbackContext context, Vector2 position, Vector2 delta, Func<Vector2, Vector2, TArg, EventBase> evtFactory, TArg arg)
        {
            // Allow focus to be lost before processing the event
            if (focusedPanel != null)
            {
                UpdateFocusedPanel(focusedPanel);
            }

            // Try all the panels, from closest to deepest
            var panels = UIElementsRuntimeUtility.GetSortedPlayerPanels();
            for (var i = panels.Count - 1; i >= 0; i--)
            {
                var panel = panels[i];
                if (panel is RuntimePanel runtimePanel)
                {
                    if (ScreenToPanel(runtimePanel, position, delta, out var panelPosition, out var panelDelta))
                    {
                        using (EventBase evt = evtFactory(panelPosition, panelDelta, arg))
                        {
                            SendEvent(runtimePanel, evt, context);

                            if (evt.processedByFocusController)
                            {
                                UpdateFocusedPanel(runtimePanel);
                            }

                            if (evt.isPropagationStopped)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void UpdateFocusedPanel(RuntimePanel runtimePanel)
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

        static bool ScreenToPanel(BaseRuntimePanel panel, Vector2 screenPosition, Vector2 screenDelta,
            out Vector2 panelPosition, out Vector2 panelDelta)
        {
            panelPosition = Vector2.zero;
            panelDelta = screenDelta;

            panelPosition = panel.ScreenToPanel(screenPosition);

            var panelRect = panel.visualTree.layout;
            if (!panelRect.Contains(panelPosition))
            {
                panelDelta = screenDelta;
                return false;
            }

            var panelPrevPosition = panel.ScreenToPanel(screenPosition - screenDelta);
            if (!panelRect.Contains(panelPrevPosition))
            {
                panelDelta = screenDelta;
                return true;
            }

            panelDelta = panelPosition - panelPrevPosition;
            return true;
        }

        internal struct InputContext
        {
            public InputAction action { get; }
            public InputControl control { get; }

            public InputContext(InputAction action, InputControl control)
            {
                this.action = action;
                this.control = control;
            }

            public static implicit operator InputContext(InputAction.CallbackContext context) =>
                new InputContext(context.action, context.control);
        }
    }

    internal interface IKeyboardEventProcessor
    {
        void OnEnable();
        void OnDisable();
        void ProcessKeyboardEvents(EventSystem eventSystem);
    }
}
#endif
