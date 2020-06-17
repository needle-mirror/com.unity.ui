using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class ToggleSnippet : ElementSnippet<ToggleSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the field from UXML and assign it its value.
            var uxmlField = container.Q<Toggle>("the-uxml-field");
            uxmlField.value = true;

            // Create a new field, disable it, and give it a style class.
            var csharpField = new Toggle("C# Field");
            csharpField.value = false;
            csharpField.SetEnabled(false);
            csharpField.AddToClassList("some-styled-field");
            csharpField.value = uxmlField.value;
            container.Add(csharpField);

            // Mirror value of uxml field into the C# field.
            uxmlField.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
            /// </sample>
        }
    }
}
