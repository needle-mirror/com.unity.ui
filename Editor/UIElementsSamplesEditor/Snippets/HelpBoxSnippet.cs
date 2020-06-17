using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class HelpBoxSnippet : ElementSnippet<HelpBoxSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the help box from UXML and update its text.
            var uxmlHelpBox = container.Q<HelpBox>("the-uxml-help-box");
            uxmlHelpBox.text += " (Updated in C#)";

            // Create a new help box and give it a style class.
            var csharpHelpBox = new HelpBox("This is a help box", HelpBoxMessageType.Warning);
            csharpHelpBox.AddToClassList("some-styled-help-box");
            container.Add(csharpHelpBox);
            /// </sample>
        }
    }
}
