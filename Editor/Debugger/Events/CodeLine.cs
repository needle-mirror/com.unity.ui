using Unity.CodeEditor;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements.Debugger
{
    class CodeLine : Label
    {
        string m_FileName;
        int m_LineNumber;

        public int hashCode { get; }

        public CodeLine(string name, string fileName, int lineNumber, int hashCode) : base(name)
        {
            m_FileName = fileName;
            m_LineNumber = lineNumber;
            this.hashCode = hashCode;
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
                e.menu.AppendAction("Go to callback registration point", GotoCode, DropdownMenuAction.AlwaysEnabled);
            }
        }

        void GotoCode(DropdownMenuAction action)
        {
#if UIE_PACKAGE
            // TODO: Figure out why we can't make this work
            // CodeEditor.Editor.Current.OpenProject(m_FileName, m_LineNumber);
#else
            CodeEditor.Editor.CurrentCodeEditor.OpenProject(m_FileName, m_LineNumber);
#endif
        }
    }
}
