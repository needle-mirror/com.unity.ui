using System;
using System.Linq;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements.Debugger;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    class DefaultEditorWindowBackend : DefaultWindowBackend, IEditorWindowBackend
    {
        private const string k_LiveReloadMenuText = "Live Reload";
        private const string k_LiveReloadPreferenceKeySuffix = ".LiveReloadOn";
        private static string k_GameViewLiveReloadPreferenceKey = null;

        private IMGUIContainer m_NotificationContainer;

        // Cached version of the static color for the actual object instance...
        Color m_PlayModeDarkenColor;

        protected IEditorWindowModel editorWindowModel => m_Model as IEditorWindowModel;

        private class EditorWindowVisualTreeAssetTracker : BaseLiveReloadVisualTreeAssetTracker
        {
            private DefaultEditorWindowBackend m_Owner;

            public EditorWindowVisualTreeAssetTracker(DefaultEditorWindowBackend owner)
            {
                m_Owner = owner;
            }

            internal override void OnVisualTreeAssetChanged(bool inMemoryChange)
            {
                if ((inMemoryChange && !m_Owner.m_LiveReloadEnabled) || !m_Owner.m_WindowRegistered)
                    return;

                var rootVisualElement = m_Owner.editorWindowModel.window.rootVisualElement;

                if (rootVisualElement.panel is BaseVisualElementPanel panel)
                {
                    var view = panel.ownerObject as HostView;
                    if (view != null && view.actualView != null)
                    {
                        view.Reload(view.actualView);
                    }
                }
            }
        }

        private EditorWindowVisualTreeAssetTracker m_LiveReloadVisualTreeAssetTracker = null;

        private bool m_LiveReloadEnabled = false;
        private string m_LiveReloadPreferenceKey;

        public override void OnCreate(IWindowModel model)
        {
            try
            {
                base.OnCreate(model);

                m_PlayModeDarkenColor = UIElementsUtility.editorPlayModeTintColor =
                    EditorApplication.isPlayingOrWillChangePlaymode ? editorWindowModel.playModeTintColor : Color.white;

                EditorApplication.playModeStateChanged += PlayModeStateChangedCallback;
                AnimationMode.onAnimationRecordingStart += RefreshStylesAfterExternalEvent;
                AnimationMode.onAnimationRecordingStop += RefreshStylesAfterExternalEvent;

                m_NotificationContainer = new IMGUIContainer();
                m_NotificationContainer.StretchToParentSize();
                m_NotificationContainer.pickingMode = PickingMode.Ignore;

                RegisterImguiContainerGUICallbacks();

                // Window is non-null when set by deserialization; it's usually null when OnCreate is called.
                if (editorWindowModel.window != null)
                {
                    RegisterWindow();
                }
            }
            catch (Exception e)
            {
                // Log error to easily diagnose issues with panel initialization and then rethrow it.
                Debug.LogException(e);
                throw;
            }
        }

        void IEditorWindowBackend.ViewMarginsChanged()
        {
            UpdateStyleMargins();
        }

        void UpdateStyleMargins()
        {
            RectOffset margins = editorWindowModel.viewMargins;
            IStyle style = editorWindowModel.window.rootVisualElement.style;
            style.top = margins.top;
            style.bottom = margins.bottom;
            style.left = margins.left;
            style.right = margins.right;
            style.position = Position.Absolute;
        }

        void IEditorWindowBackend.OnRegisterWindow()
        {
            RegisterWindow();
        }

        void IEditorWindowBackend.OnUnregisterWindow()
        {
            UnregisterWindow();
        }

        private bool m_WindowRegistered;
        void RegisterWindow()
        {
            if (m_WindowRegistered)
                return;

            EditorWindow window = editorWindowModel.window;
            if (m_LiveReloadVisualTreeAssetTracker == null)
            {
                m_LiveReloadVisualTreeAssetTracker = new EditorWindowVisualTreeAssetTracker(this);
                m_Panel.m_LiveReloadStyleSheetAssetTracker = new LiveReloadStyleSheetAssetTracker();

                // Live Reload is off by default for all Editor Windows, except for the Game View
                m_LiveReloadPreferenceKey = GetWindowLiveReloadPreferenceKey(editorWindowModel.window.GetType());
                m_LiveReloadEnabled = EditorPrefs.GetBool(m_LiveReloadPreferenceKey, editorWindowModel.window is GameView);
            }

            var root = window.rootVisualElement;
            root.visualTreeAssetTracker = m_LiveReloadVisualTreeAssetTracker;
            if (root.hierarchy.parent != m_Panel.visualTree)
            {
                AddRootElement(root);
            }

            m_Panel.getViewDataDictionary = window.GetViewDataDictionary;
            m_Panel.saveViewData = window.SaveViewData;
            m_Panel.name = window.GetType().Name;
            m_NotificationContainer.onGUIHandler = window.DrawNotification;

            UpdateStyleMargins();
            m_WindowRegistered = true;
        }

        void UnregisterWindow()
        {
            if (!m_WindowRegistered)
                return;

            var root = editorWindowModel.window.rootVisualElement;
            if (root.hierarchy.parent == m_Panel.visualTree)
            {
                RemoveRootElement(root);
                m_Panel.getViewDataDictionary = null;
                m_Panel.saveViewData = null;
            }

            m_NotificationContainer.onGUIHandler = null;
            m_WindowRegistered = false;
        }

        const TrickleDown k_TricklePhase = TrickleDown.TrickleDown;

        private VisualElement m_RegisteredRoot;

        private void AddRootElement(VisualElement root)
        {
            m_Panel.visualTree.Add(root);
        }

        private void RemoveRootElement(VisualElement root)
        {
            root.RemoveFromHierarchy();
        }

        private void RegisterImguiContainerGUICallbacks()
        {
            var root = m_Panel.visualTree;
            root.RegisterCallback<MouseDownEvent>(SendEventToSplitterGUI, k_TricklePhase);
            root.RegisterCallback<MouseUpEvent>(SendEventToSplitterGUI, k_TricklePhase);
            root.RegisterCallback<MouseMoveEvent>(SendEventToSplitterGUI, k_TricklePhase);
            m_RegisteredRoot = root;
        }

        private void UnregisterImguiContainerGUICallbacks()
        {
            var root = m_Panel.visualTree;
            if (root == m_RegisteredRoot)
            {
                m_RegisteredRoot = null;
                root.UnregisterCallback<MouseDownEvent>(SendEventToSplitterGUI, k_TricklePhase);
                root.UnregisterCallback<MouseUpEvent>(SendEventToSplitterGUI, k_TricklePhase);
                root.UnregisterCallback<MouseMoveEvent>(SendEventToSplitterGUI, k_TricklePhase);
            }
        }

        private void SendEventToSplitterGUI(EventBase ev)
        {
            if (ev.imguiEvent == null || ev.imguiEvent.rawType == EventType.Used || ev.target == imguiContainer)
                return;

            // This will only be called after OnCreate and before OnDestroy, so
            // we assume imguiContainer != null && editorWindowModel != null

            imguiContainer.HandleIMGUIEvent(ev.imguiEvent, editorWindowModel.onSplitterGUIHandler, false);

            if (ev.imguiEvent.rawType == EventType.Used)
                ev.StopPropagation();
        }

        void IEditorWindowBackend.PlayModeTintColorChanged()
        {
            UpdatePlayModeColor(EditorApplication.isPlayingOrWillChangePlaymode
                ? editorWindowModel.playModeTintColor
                : Color.white);
        }

        public override void OnDestroy(IWindowModel model)
        {
            EditorApplication.playModeStateChanged -= PlayModeStateChangedCallback;
            AnimationMode.onAnimationRecordingStart -= RefreshStylesAfterExternalEvent;
            AnimationMode.onAnimationRecordingStop -= RefreshStylesAfterExternalEvent;

            m_NotificationContainer.onGUIHandler = null;
            m_NotificationContainer.RemoveFromHierarchy();

            UnregisterImguiContainerGUICallbacks();
            UnregisterWindow();

            base.OnDestroy(model);
        }

        void IEditorWindowBackend.NotificationVisibilityChanged()
        {
            if (editorWindowModel.notificationVisible)
            {
                if (m_NotificationContainer.parent == null)
                {
                    m_Panel.visualTree.Add(m_NotificationContainer);
                    m_NotificationContainer.StretchToParentSize();
                }
            }
            else
            {
                m_NotificationContainer.RemoveFromHierarchy();
            }
        }

        private void PlayModeStateChangedCallback(PlayModeStateChange state)
        {
            Color newColorToUse = Color.white;
            if ((state == PlayModeStateChange.ExitingEditMode) ||
                (state == PlayModeStateChange.EnteredPlayMode))
            {
                newColorToUse = editorWindowModel.playModeTintColor;
            }
            else if ((state == PlayModeStateChange.ExitingPlayMode) || (state == PlayModeStateChange.EnteredEditMode))
            {
                newColorToUse = Color.white;
            }
            UpdatePlayModeColor(newColorToUse);
        }

        void UpdatePlayModeColor(Color newColorToUse)
        {
            // Check the cached color to dirty only if needed !
            if (m_PlayModeDarkenColor != newColorToUse)
            {
                m_PlayModeDarkenColor = newColorToUse;
                UIElementsUtility.editorPlayModeTintColor = newColorToUse;

                // Make sure to dirty the right imgui container in this HostView (and all its children / parents)
                // The MarkDirtyRepaint() function is dirtying the element itself and its parent, but not the children explicitly.
                // ... and in the repaint function, it check for the current rendered element, not the parent.
                // Since the HostView "hosts" an IMGUIContainer or any VisualElement, we have to make sure to dirty everything here.
                PropagateDirtyRepaint(m_Panel.visualTree);
            }
        }

        static void PropagateDirtyRepaint(VisualElement ve)
        {
            ve.MarkDirtyRepaint();
            var count = ve.hierarchy.childCount;
            for (var i = 0; i < count; i++)
            {
                var child = ve.hierarchy[i];
                PropagateDirtyRepaint(child);
            }
        }

        void IEditorWindowBackend.Focused()
        {
            m_Panel.Focus();
        }

        void IEditorWindowBackend.Blurred()
        {
            m_Panel.Blur();
        }

        void IEditorWindowBackend.OnDisplayWindowMenu(GenericMenu menu)
        {
            AddLiveReloadOptionToMenu(menu);
            AddUIElementsDebuggerToMenu(menu);
        }

        private void AddLiveReloadOptionToMenu(GenericMenu menu)
        {
            // Live Reload is off by default for all Editor Windows, except for the Game View
            m_LiveReloadEnabled = EditorPrefs.GetBool(m_LiveReloadPreferenceKey, editorWindowModel.window is GameView);
            menu.AddItem(EditorGUIUtility.TextContent(k_LiveReloadMenuText), m_LiveReloadEnabled, ToggleLiveReloadForWindowType, editorWindowModel.window);
        }

        private void ToggleLiveReloadForWindowType(object userData)
        {
            m_LiveReloadEnabled = !m_LiveReloadEnabled;
            EditorPrefs.SetBool(m_LiveReloadPreferenceKey, m_LiveReloadEnabled);
        }

        private void AddUIElementsDebuggerToMenu(GenericMenu menu)
        {
            var itemContent = UIElementsDebugger.WindowName;
            var shortcut = ShortcutIntegration.instance.directory.FindShortcutEntry(UIElementsDebugger.k_WindowPath);
            if (shortcut != null && shortcut.combinations.Any())
                itemContent += $" {KeyCombination.SequenceToMenuString(shortcut.combinations)}";

            menu.AddItem(EditorGUIUtility.TextContent(itemContent), false, DebugWindow, editorWindowModel.window);
        }

        private void DebugWindow(object userData)
        {
            EditorWindow window = userData as EditorWindow;
            if (window == null)
                return;

            if (CommandService.Exists(UIElementsDebugger.OpenWindowCommand))
                CommandService.Execute(UIElementsDebugger.OpenWindowCommand, CommandHint.Menu, window);
            else
            {
                UIElementsDebugger.OpenAndInspectWindow(window);
            }
        }

        private void RefreshStylesAfterExternalEvent()
        {
            var panel = m_Panel.visualTree.elementPanel;
            if (panel == null)
                return;

            var updater = panel.GetUpdater(VisualTreeUpdatePhase.Bindings) as VisualTreeBindingsUpdater;
            if (updater == null)
                return;

            updater.PollElementsWithBindings((e, b) => BindingExtensions.HandleStyleUpdate(e));
        }

        private static string GetWindowLiveReloadPreferenceKey(Type windowType)
        {
            return windowType + k_LiveReloadPreferenceKeySuffix;
        }

        internal static bool IsGameViewWindowLiveReloadOn()
        {
            if (k_GameViewLiveReloadPreferenceKey == null)
            {
                k_GameViewLiveReloadPreferenceKey = GetWindowLiveReloadPreferenceKey(typeof(GameView));
            }

            return EditorPrefs.GetBool(k_GameViewLiveReloadPreferenceKey, true);
        }
    }
}
