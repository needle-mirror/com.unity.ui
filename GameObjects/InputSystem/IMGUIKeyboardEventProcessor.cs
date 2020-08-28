#if UNITY_INPUT_SYSTEM && ENABLE_INPUT_SYSTEM

namespace UnityEngine.UIElements
{
    internal class IMGUIKeyboardEventProcessor : IKeyboardEventProcessor
    {
        private readonly Event m_Event = new Event();

        public void OnEnable() {}

        public void OnDisable() {}

        public void ProcessKeyboardEvents(EventSystem eventSystem)
        {
            while (Event.PopEvent(m_Event))
            {
                if (m_Event.type != EventType.KeyDown && m_Event.type != EventType.KeyUp)
                    continue;

                using (EventBase evt = UIElementsRuntimeUtility.CreateEvent(m_Event))
                {
                    eventSystem.focusedPanel.visualTree.SendEvent(evt);
                }
            }
        }
    }
}
#endif
