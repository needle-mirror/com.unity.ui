using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class TextFieldSnippet : ElementSnippet<TextFieldSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the field from UXML and append to it its value.
            var uxmlField = container.Q<TextField>("the-uxml-field");
            uxmlField.value += "..";

            // Create a new field, disable it, and give it a style class.
            var csharpField = new TextField("C# Field");
            csharpField.value = "It's snowing outside...";
            csharpField.SetEnabled(false);
            csharpField.AddToClassList("some-styled-field");
            csharpField.value = uxmlField.value;
            container.Add(csharpField);

            // Mirror value of uxml field into the C# field.
            uxmlField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
            /// </sample>
        }
    }
}
