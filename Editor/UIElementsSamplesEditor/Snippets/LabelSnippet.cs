using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class LabelSnippet : ElementSnippet<LabelSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the label from UXML and update its text.
            var uxmlLabel = container.Q<Label>("the-uxml-label");
            uxmlLabel.text += " (Updated in C#)";

            // Create a new label and give it a style class.
            var csharpLabel = new Label("C# Label");
            csharpLabel.AddToClassList("some-styled-label");
            container.Add(csharpLabel);
            /// </sample>
        }
    }
}
