using Unity.CodeEditor;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements.Debugger
{
    class CodeLine : Label
    {
        string m_FileName;
        int m_LineNumber;

        public int hashCode { get; private set; }

        public void Init(string textName, string fileName, int lineNumber, int lineHashCode)
        {
            text = textName;
            m_FileName = fileName;
            m_LineNumber = lineNumber;
            this.hashCode = lineHashCode;
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);

            if (elementPanel != null && elementPanel.contextualMenuManager != null)
            {
                elementPanel.contextualMenuManager.DisplayMenuIfEventMatches(evt, this);
            }

            if (evt.eventTypeId == ContextualMenuPopulateEvent.TypeId())
            {
                ContextualMenuPopulateEvent e = evt as ContextualMenuPopulateEvent;
                e.menu.AppendAction("Go to callback registration point", (e) => GotoCode(), DropdownMenuAction.AlwaysEnabled);
            }
        }

        public void GotoCode()
        {
            #if UNITY_2021_1_OR_NEWER || !UIE_PACKAGE
            CodeEditor.Editor.CurrentCodeEditor.OpenProject(m_FileName, m_LineNumber);
            #else
            CodeEditor.Editor.Current.OpenProject(m_FileName, m_LineNumber);
            #endif
        }

        public override string ToString()
        {
            return $"{m_FileName} ({m_LineNumber})";
        }
    }
}
