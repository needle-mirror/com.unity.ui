using System;
using UnityEditor;

namespace UnityEngine.UIElements
{
    internal class RuntimePanel : BaseRuntimePanel
    {
        static readonly EventDispatcher s_EventDispatcher = RuntimeEventDispatcher.Create();

        internal PanelSettings m_PanelSettings;
        public static RuntimePanel Create(ScriptableObject ownerObject)
        {
            return new RuntimePanel(ownerObject);
        }

        private RuntimePanel(ScriptableObject ownerObject)
            : base(ownerObject, s_EventDispatcher)
        {
            focusController = new FocusController(new NavigateFocusRing(visualTree));
            m_PanelSettings  = ownerObject as PanelSettings;
        }

        public override void Update()
        {
            if (m_PanelSettings != null)
                m_PanelSettings.ApplyPanelSettings();

            base.Update();
        }
    }
}
