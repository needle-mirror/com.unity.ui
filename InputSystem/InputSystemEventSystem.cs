using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting.APIUpdating;
#if UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif
using UnityEngine.UIElements.Collections;

namespace UnityEngine.UIElements.InputSystem
{
    /// <summary>
    /// Handles input and sending events to UIElements Panel through use of Unity's Input System package.
    /// </summary>
    [AddComponentMenu("UI Toolkit/Input System Event System (UI Toolkit)")]
    public class InputSystemEventSystem : MonoBehaviour
    {
        /// <summary>
        /// Returns true if the application has the focus. Events are sent only if this flag is set to true.
        /// </summary>
        public bool isAppFocused { get; private set; } = true;

        /// <summary>
        /// Overrides the default input when NavigationEvents are sent.
        /// </summary>
        /// <remarks>
        /// Use this override to bypass the default input system with your own input system.
        /// This is useful when you want to send fake input to the event system.
        /// This property will be ignored if the New Input System is used.
        /// </remarks>
        [Obsolete("EventSystem no longer supports input override for legacy input. Install Input System package for full input binding functionality.")]
        public InputWrapper inputOverride { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected InputSystemEventSystem()
        {
        }

        void OnApplicationFocus(bool hasFocus)
        {
            isAppFocused = hasFocus;
        }

#if UNITY_INPUT_SYSTEM
        private bool ShouldIgnoreEventsOnAppNotFocused()
        {
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.Windows:
                case OperatingSystemFamily.Linux:
                case OperatingSystemFamily.MacOSX:
#if UNITY_EDITOR
                    if (UnityEditor.EditorApplication.isRemoteConnected)
                        return false;
#endif
                    return true;
                default:
                    return false;
            }
        }

        [Tooltip("The Initial delay (in seconds) between an initial keyboard action and a repeated action.")]
        [SerializeField] internal float m_RepeatDelay = 0.5f;

        [Tooltip("The speed (in seconds) that the keyboard action repeats itself once repeating (max 1 per frame).")]
        [SerializeField] internal float m_RepeatRate = 0.05f;

#pragma warning disable CS0414
        [SerializeField] private bool m_FallbackOnIMGUIKeyboardEvents = false;
#pragma warning restore CS0414

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

#pragma warning disable CS0067
        private event Action OnDisableEvent;
#pragma warning restore CS0067

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

        private IKeyboardEventProcessor m_KeyboardEventProcessor;

        void OnEnable()
        {
            // Keep #if inside OnEnable so the inspector shows an Enable/Disable option regardless of the
            // input system being active. This makes the script's internal behavior less visible to the user.
#if ENABLE_INPUT_SYSTEM
            UIElementsRuntimeUtility.RegisterEventSystem(this);

            RegisterCallback(m_NavigateAction, OnNavigatePerformed, OnNavigationCanceled);
            RegisterCallback(m_TabAction, OnTabPerformed, OnNavigationCanceled);
            RegisterCallback(m_SubmitAction, OnSubmitPerformed, OnNavigationCanceled);
            RegisterCallback(m_CancelAction, OnCancelPerformed, OnNavigationCanceled);
            RegisterCallback(m_PointAction, OnPointPerformed, OnPointPerformed);
            RegisterCallback(m_ClickAction, OnClickPerformed, OnClickPerformed);
            RegisterCallback(m_ScrollWheelAction, OnScrollWheelPerformed, OnScrollWheelPerformed);
            RegisterCallback(m_MiddleClickAction, OnMiddleClickPerformed, OnMiddleClickPerformed);
            RegisterCallback(m_RightClickAction, OnRightClickPerformed, OnRightClickPerformed);

            m_KeyboardEventProcessor = m_FallbackOnIMGUIKeyboardEvents
                ? (m_KeyboardEventProcessor as IMGUIKeyboardEventProcessor) ?? (IKeyboardEventProcessor) new IMGUIKeyboardEventProcessor()
                : (m_KeyboardEventProcessor as InputSystemKeyboardEventProcessor) ?? new InputSystemKeyboardEventProcessor();

            m_KeyboardEventProcessor.OnEnable();
#endif
        }

        void OnDisable()
        {
#if ENABLE_INPUT_SYSTEM
            UIElementsRuntimeUtility.UnregisterEventSystem(this);

            OnDisableEvent?.Invoke();
            OnDisableEvent = null;

            m_KeyboardEventProcessor.OnDisable();
#endif
        }

#if ENABLE_INPUT_SYSTEM
        void Update()
        {
            if (!isAppFocused && ShouldIgnoreEventsOnAppNotFocused())
                return;

            m_KeyboardEventProcessor.ProcessKeyboardEvents(this);
            CheckForRepeatedNavigationEvents();
            ProcessPointerMoveEvents();
            ProcessPointerScrollEvents();
            ProcessPointerClickEvents();
        }

#endif

        void Reset()
        {
#if UNITY_EDITOR
            m_InputActionAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionAsset>(
                UnityEditor.AssetDatabase.GUIDToAssetPath(
                    UnityEditor.AssetDatabase.FindAssets("UIToolkitInputActions t:InputActionAsset a:all").First()));
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
                SendFocusBasedEvent(context, m => NavigationMoveEvent.GetPooled(m), movement);
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
                SendFocusBasedEvent(context, d => NavigationTabEvent.GetPooled(d), direction);
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

        private readonly Dictionary<InputControl, PointerEvent> m_PointerStates = new Dictionary<InputControl, PointerEvent>();
        private readonly Dictionary<InputControl, Vector2> m_ShelvedPointerPositions = new Dictionary<InputControl, Vector2>();
        private readonly Dictionary<InputControl, Vector2> m_ShelvedScrollDeltas = new Dictionary<InputControl, Vector2>();
        private readonly HashSet<(ButtonControl control, int button)> m_ActivePointerPresses = new HashSet<(ButtonControl, int)>();

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            OnGenericClickPerformed(context, (int)MouseButton.LeftMouse);
        }

        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            OnGenericClickPerformed(context, (int)MouseButton.RightMouse);
        }

        private void OnMiddleClickPerformed(InputAction.CallbackContext context)
        {
            OnGenericClickPerformed(context, (int)MouseButton.MiddleMouse);
        }

        private void OnScrollWheelPerformed(InputAction.CallbackContext context)
        {
            var pointerEvent = GetPointerState(context.control.parent);
            pointerEvent.ReadDeviceState(context.control);
            ShelveScrollDelta(context, InputSystemToLegacyScreenDelta(context.ReadValue<Vector2>()));
        }

        private void OnPointPerformed(InputAction.CallbackContext context)
        {
            var pointerEvent = GetPointerState(context.control.parent);
            pointerEvent.ReadDeviceState(context.control);
            ShelvePointerPosition(context, pointerEvent);
        }

        private void OnGenericClickPerformed(InputAction.CallbackContext context, int button)
        {
            if (context.control is ButtonControl buttonControl)
            {
                m_ActivePointerPresses.Add((buttonControl, button));
            }
            else // Handle unknown click immediately.
            {
                var pointerEvent = GetPointerState(context.control.parent);
                pointerEvent.ReadDeviceState(context.control, button);
                ProcessPointerClick(pointerEvent, button, context.ReadValue<float>() > 0);
            }
        }

        private void ShelvePointerPosition(InputAction.CallbackContext context, PointerEvent pointerEvent)
        {
            if (!m_ShelvedPointerPositions.ContainsKey(context.control.parent))
            {
                m_ShelvedPointerPositions[context.control.parent] = pointerEvent.previousPosition;
            }
        }

        private void ShelveScrollDelta(InputAction.CallbackContext context, Vector2 scrollDelta)
        {
            m_ShelvedScrollDeltas[context.control.parent] = m_ShelvedScrollDeltas.Get(context.control.parent) + scrollDelta;
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
            public int pressedButtons { get; set; }
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

            public int? targetDisplay;
            public Vector3 previousPosition => position - deltaPosition;

            public void ReadDeviceState(InputControl control, int? buttonOverride = null)
            {
                shiftKey = false;
                ctrlKey = false;
                commandKey = false;
                altKey = false;
                actionKey = false;

                foreach (var device in UnityEngine.InputSystem.InputSystem.devices)
                {
                    if (device is Keyboard keyboard)
                    {
                        shiftKey |= keyboard.shiftKey.isPressed;
                        ctrlKey |= keyboard.ctrlKey.isPressed;
                        commandKey |= keyboard.leftCommandKey.isPressed | keyboard.rightCommandKey.isPressed;
                        altKey |= keyboard.altKey.isPressed;
                        actionKey |= keyboard.leftAppleKey.isPressed | keyboard.rightAppleKey.isPressed | keyboard.leftWindowsKey.isPressed | keyboard.rightWindowsKey.isPressed;
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

                if (control.parent is Pen pen)
                {
                    pressure = pen.pressure.EvaluateMagnitude();
                    clickCount = pen.tip.isPressed ? 1 : 0;
                    azimuthAngle = (pen.tilt.ReadValue().x + 1) * Mathf.PI / 2;
                    altitudeAngle = (pen.tilt.ReadValue().y + 1) * Mathf.PI / 2;
                    twist = pen.twist.ReadValue() * Mathf.PI * 2;
                    pointerType = PointerType.pen;
                    pointerId = GetPenPointerId(pen);
                    localPosition = position = InputSystemToLegacyScreenPosition(pen.position.ReadValue(), out targetDisplay);
                    deltaPosition = InputSystemToLegacyScreenDelta(pen.delta.ReadValue());

                    // Note that the pressedButtons PointerEvent property for the pen doesn't reflect the states of the
                    // Pen.eraser, Pen.firstBarrelButton, Pen.secondBarrelButton, Pen.thirdBarrelButton and
                    // Pen.fourthBarrelButton device properties, but rather they reflect whatever PointerDownEvents
                    // with various Left/Right/Middle click flavours were invoked by the EventSystem. If users want
                    // the state of the Pen device, they can use Pen.current.eraser.isPressed, etc.
                }
                else if (control.parent is TouchControl touchControl)
                {
                    pressure = touchControl.pressure.EvaluateMagnitude();
                    radius = touchControl.radius.ReadValue();
                    clickCount = touchControl.tapCount.ReadValue();
                    pointerType = PointerType.touch;
                    pointerId = GetTouchPointerId(touchControl);
                    isPrimary = pointerId == PointerId.touchPointerIdBase;
                    localPosition = position = InputSystemToLegacyScreenPosition(touchControl.position.ReadValue(), out targetDisplay);
                    deltaPosition = InputSystemToLegacyScreenDelta(touchControl.delta.ReadValue());
                }
                else if (control.parent is Pointer pointer)
                {
                    if (pointer is Mouse mouse)
                    {
                        clickCount = mouse.clickCount.ReadValue();
                    }
                    pointerType = PointerType.mouse;
                    pointerId = PointerId.mousePointerId;
                    localPosition = position = InputSystemToLegacyScreenPosition(pointer.position.ReadValue(), out targetDisplay);
                    deltaPosition = InputSystemToLegacyScreenDelta(pointer.delta.ReadValue());
                }

                // Move events have button = -1 and clickCount = 0
                if (buttonOverride.HasValue)
                {
                    button = buttonOverride.Value;
                    clickCount = Mathf.Max(1, clickCount);
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

            private static int GetPenPointerId(Pen pen)
            {
                var n = 0;
                foreach (var device in UnityEngine.InputSystem.InputSystem.devices)
                    if (device is Pen otherPen)
                    {
                        if (pen == otherPen)
                            return n;
                        n++;
                    }
                return PointerId.penPointerIdBase; // Default to 0-th pen if nothing else is found
            }
        }

        private PointerEvent GetPointerState(InputControl control)
        {
            if (!m_PointerStates.TryGetValue(control, out var pointerEvent))
            {
                pointerEvent = m_PointerStates[control] = new PointerEvent();
            }
            return pointerEvent;
        }

        private void ProcessPointerMove(InputControl control, Vector2 previousPosition)
        {
            var pointerEvent = m_PointerStates[control];
            Vector2 position = pointerEvent.position;
            Vector2 delta = position - previousPosition;

            if (delta == Vector2.zero)
                return;

            SendPositionBasedEvent(position, delta, pointerEvent.pointerId, pointerEvent.targetDisplay,
                (panelPosition, panelDelta, data) =>
                {
                    using (var e1 = PointerEventWrapper.GetPooled(data, panelPosition, panelDelta))
                        return PointerMoveEvent.GetPooled((IPointerEvent)e1);
                }, pointerEvent);
        }

        private void ProcessPointerMoveEvents()
        {
            foreach (var kv in m_ShelvedPointerPositions)
            {
                ProcessPointerMove(kv.Key, kv.Value);
            }
            m_ShelvedPointerPositions.Clear();
        }

        internal const float kPixelPerLine = 20;
        private void ProcessPointerScroll(InputControl control, Vector2 scrollDelta)
        {
            var pointerEvent = m_PointerStates[control];
            var mousePosition = pointerEvent.position;

            // The old input system reported scroll deltas in lines. The Input package uses pixels.
            // Need to scale as the UI system expects lines.
            var delta = scrollDelta * (1 / kPixelPerLine);

            if (delta == Vector2.zero)
                return;

            SendPositionBasedEvent(mousePosition, Vector2.zero, pointerEvent.pointerId, pointerEvent.targetDisplay,
                (panelPosition, _, d) => WheelEvent.GetPooled(d, panelPosition), delta);
        }

        private void ProcessPointerScrollEvents()
        {
            foreach (var kv in m_ShelvedScrollDeltas)
            {
                ProcessPointerScroll(kv.Key, kv.Value);
            }
            m_ShelvedScrollDeltas.Clear();
        }

        private void ProcessPointerClick(ButtonControl buttonControl, int button)
        {
            var control = buttonControl.parent;
            var pointerEvent = GetPointerState(control);
            pointerEvent.ReadDeviceState(buttonControl, button);

            var wasPressed = (pointerEvent.pressedButtons & (1 << button)) != 0;
            var pressed = buttonControl.isPressed;

            if (pressed == wasPressed)
                return;

            ProcessPointerClick(pointerEvent, button, pressed);
        }

        private void ProcessPointerClick(PointerEvent pointerEvent, int button, bool pressed)
        {
            if (pressed)
            {
                PointerDeviceState.PressButton(pointerEvent.pointerId, button);
                pointerEvent.pressedButtons |= 1 << button;
                SendPositionBasedEvent(pointerEvent.position, pointerEvent.deltaPosition, pointerEvent.pointerId, pointerEvent.targetDisplay,
                    (panelPosition, panelDelta, data) =>
                    {
                        using (var e = PointerEventWrapper.GetPooled(data, panelPosition, panelDelta))
                        {
                            return PointerDownEvent.GetPooled((IPointerEvent)e);
                        }
                    }, pointerEvent, deselectIfNoTarget: true);
            }
            else
            {
                PointerDeviceState.ReleaseButton(pointerEvent.pointerId, button);
                pointerEvent.pressedButtons &= ~(1 << button);
                SendPositionBasedEvent(pointerEvent.position, pointerEvent.deltaPosition, pointerEvent.pointerId, pointerEvent.targetDisplay,
                    (panelPosition, panelDelta, data) =>
                    {
                        using (var e = PointerEventWrapper.GetPooled(data, panelPosition, panelDelta))
                        {
                            return PointerUpEvent.GetPooled((IPointerEvent)e);
                        }
                    }, pointerEvent);
            }
        }

        private void ProcessPointerClickEvents()
        {
            foreach (var press in m_ActivePointerPresses)
            {
                ProcessPointerClick(press.control, press.button);
            }
            m_ActivePointerPresses.Clear();
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

        internal bool SendFocusBasedEvent<TArg>(Func<TArg, EventBase> evtFactory, TArg arg) =>
            SendFocusBasedEvent(default, evtFactory, arg);

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

        internal bool SendPositionBasedEvent<TArg>(Vector2 position, Vector2 delta, int pointerId, Func<Vector2, Vector2, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false) =>
            SendPositionBasedEvent(default, position, delta, pointerId, null, evtFactory, arg, deselectIfNoTarget);

        internal bool SendPositionBasedEvent<TArg>(Vector2 position, Vector2 delta, int pointerId, int? targetDisplay, Func<Vector2, Vector2, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false) =>
            SendPositionBasedEvent(default, position, delta, pointerId, targetDisplay, evtFactory, arg, deselectIfNoTarget);

        internal bool SendPositionBasedEvent<TArg>(InputAction.CallbackContext context, Vector2 position, Vector2 delta, int pointerId, int? targetDisplay, Func<Vector2, Vector2, TArg, EventBase> evtFactory, TArg arg, bool deselectIfNoTarget = false)
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
                if (panel is RuntimePanel runtimePanel)
                {
                    if (panel.GetCapturingElement(pointerId) is VisualElement ve && ve.panel == panel)
                    {
                        ScreenToPanel(runtimePanel, position, delta, out var panelPosition, out var panelDelta, true);

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

                            // Only 1 panel receives the event.
                            return false;
                        }
                    }
                }
            }

            // Find a candidate panel for the event
            // Try all the panels, from closest to deepest
            RuntimePanel candidatePanel = null;
            Vector2 candidatePosition = Vector2.zero;
            Vector2 candidateDelta = Vector2.zero;
            for (var i = panels.Count - 1; i >= 0; i--)
            {
                if (panels[i] is RuntimePanel runtimePanel && (targetDisplay == null || runtimePanel.targetDisplay == targetDisplay))
                {
                    if (runtimePanel.ScreenToPanel(position, delta, out candidatePosition, out candidateDelta) &&
                        runtimePanel.Pick(candidatePosition) != null)
                    {
                        candidatePanel = runtimePanel;
                        break;
                    }
                }
            }

            var returnValue = false;

            RuntimePanel lastActivePanel = PointerDeviceState.GetPanel(pointerId, ContextType.Player) as RuntimePanel;
            if (lastActivePanel != null && lastActivePanel != candidatePanel)
            {
                // Send an event to the last panel the pointer was in, so it can dispatch [Mouse|Pointer][Out|Leave] events.
                lastActivePanel.ScreenToPanel(position, delta, out var panelPosition, out var panelDelta, true);
                using (EventBase lastActivePanelEvent = evtFactory(panelPosition, panelDelta, arg))
                {
                    SendEvent(lastActivePanel, lastActivePanelEvent, context);

                    if (lastActivePanelEvent.isPropagationStopped)
                    {
                        returnValue |= true;
                    }
                }
            }

            if (candidatePanel != null)
            {
                using (EventBase evt = evtFactory(candidatePosition, candidateDelta, arg))
                {
                    SendEvent(candidatePanel, evt, context);

                    if (evt.processedByFocusController)
                    {
                        UpdateFocusedPanel(candidatePanel);
                    }

                    if (evt.isPropagationStopped)
                    {
                        returnValue |= true;
                    }
                }
            }
            else
            {
                if (lastActivePanel == null)
                {
                    // Mouse and pointer events calls PointerDeviceState.SavePointerPosition in their PreDispatch().
                    // If we did not send any event, we need to manually update the pointer position.
                    PointerDeviceState.SavePointerPosition(pointerId, position, null, ContextType.Player);
                }

                if (deselectIfNoTarget)
                {
                    focusedPanel = null;
                }
            }

            return returnValue;
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
            out Vector2 panelPosition, out Vector2 panelDelta, bool allowOutside = false)
        {
            panelPosition = Vector2.zero;
            panelDelta = screenDelta;

            panelPosition = panel.ScreenToPanel(screenPosition);

            Vector2 panelPrevPosition;

            if (!allowOutside)
            {
                var panelRect = panel.visualTree.layout;
                if (!panelRect.Contains(panelPosition))
                {
                    panelDelta = screenDelta;
                    return false;
                }

                panelPrevPosition = panel.ScreenToPanel(screenPosition - screenDelta);
                if (!panelRect.Contains(panelPrevPosition))
                {
                    panelDelta = screenDelta;
                    return true;
                }
            }
            else
            {
                panelPrevPosition = panel.ScreenToPanel(screenPosition - screenDelta);
            }

            panelDelta = panelPosition - panelPrevPosition;
            return true;
        }

        static Vector2 InputSystemToLegacyScreenPosition(Vector2 position, out int? targetDisplay)
        {
            return UIElementsRuntimeUtility.MultiDisplayBottomLeftToPanelPosition(position, out targetDisplay);
        }

        static Vector2 InputSystemToLegacyScreenDelta(Vector2 delta)
        {
            return UIElementsRuntimeUtility.ScreenBottomLeftToPanelDelta(delta);
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
#endif
    }

    internal interface IKeyboardEventProcessor
    {
        void OnEnable();
        void OnDisable();
        void ProcessKeyboardEvents(InputSystemEventSystem eventSystem);
    }
}
