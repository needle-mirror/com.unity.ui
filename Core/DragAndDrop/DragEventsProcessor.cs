namespace UnityEngine.UIElements
{
    internal abstract class DragEventsProcessor
    {
        private bool m_CanStartDrag;
        private Vector3 m_Start;
        internal readonly VisualElement m_Target;

        private const int k_DistanceToActivation = 5;

        internal DragEventsProcessor(VisualElement target)
        {
            m_Target = target;

            m_Target.RegisterCallback<PointerDownEvent>(OnPointerDownEvent, TrickleDown.TrickleDown);
            m_Target.RegisterCallback<PointerUpEvent>(OnPointerUpEvent, TrickleDown.TrickleDown);
            m_Target.RegisterCallback<PointerLeaveEvent>(OnPointerLeaveEvent);
            m_Target.RegisterCallback<PointerMoveEvent>(OnPointerMoveEvent);

#if UNITY_EDITOR
            m_Target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            m_Target.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
            m_Target.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);
#endif
            m_Target.RegisterCallback<DetachFromPanelEvent>(UnregisterCallbacksFromTarget);
        }

        private void UnregisterCallbacksFromTarget(DetachFromPanelEvent evt)
        {
            m_Target.UnregisterCallback<PointerDownEvent>(OnPointerDownEvent);
            m_Target.UnregisterCallback<PointerUpEvent>(OnPointerUpEvent);
            m_Target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeaveEvent);
            m_Target.UnregisterCallback<PointerMoveEvent>(OnPointerMoveEvent);
#if UNITY_EDITOR
            m_Target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            m_Target.UnregisterCallback<DragPerformEvent>(OnDragPerformEvent);
            m_Target.UnregisterCallback<DragExitedEvent>(OnDragExitedEvent);
#endif

            m_Target.UnregisterCallback<DetachFromPanelEvent>(UnregisterCallbacksFromTarget);
        }

        protected abstract bool CanStartDrag(Vector3 pointerPosition);
        protected abstract StartDragArgs StartDrag(Vector3 pointerPosition);
        protected abstract DragVisualMode UpdateDrag(Vector3 pointerPosition);

        protected abstract void OnDrop(Vector3 pointerPosition);
        protected abstract void ClearDragAndDropUI();

        internal void OnPointerUp()
        {
            m_CanStartDrag = false;
        }

        private void OnPointerDownEvent(PointerDownEvent evt)
        {
            if (evt.button != (int)MouseButton.LeftMouse)
            {
                m_CanStartDrag = false;
                return;
            }

            if (CanStartDrag(evt.position))
            {
                m_CanStartDrag = true;
                m_Start = evt.position;
            }
        }

        private void OnPointerUpEvent(PointerUpEvent evt)
        {
            OnPointerUp();
        }

        private void OnPointerLeaveEvent(PointerLeaveEvent evt)
        {
            if (evt.target == m_Target)
                ClearDragAndDropUI();
        }

#if UNITY_EDITOR
        private void OnDragExitedEvent(DragExitedEvent evt)
        {
            ClearDragAndDropUI();
        }

        private void OnDragPerformEvent(DragPerformEvent evt)
        {
            m_CanStartDrag = false;
            OnDrop(evt.mousePosition);

            ClearDragAndDropUI();
            DragAndDropUtility.dragAndDrop.AcceptDrag();
        }

        private void OnDragUpdate(DragUpdatedEvent evt)
        {
            var visualMode = UpdateDrag(evt.mousePosition);
            DragAndDropUtility.dragAndDrop.SetVisualMode(visualMode);
        }

#endif
        private void OnPointerMoveEvent(PointerMoveEvent evt)
        {
            if (!m_CanStartDrag)
                return;

            if (Mathf.Abs(m_Start.x - evt.position.x) > k_DistanceToActivation ||
                Mathf.Abs(m_Start.y - evt.position.y) > k_DistanceToActivation)
            {
                // Drag can only be started by mouse events or else it will throw an error, so we leave early.
                if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.MouseDrag)
                    return;

                var args = StartDrag(evt.position);
                DragAndDropUtility.dragAndDrop.StartDrag(args);
                m_CanStartDrag = false;
            }
        }
    }
}
