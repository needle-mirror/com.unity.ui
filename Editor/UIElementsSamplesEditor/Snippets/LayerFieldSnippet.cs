using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class LayerFieldSnippet : ElementSnippet<LayerFieldSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the field from UXML and assign it its value.
            var uxmlField = container.Q<LayerField>("the-uxml-field");
            uxmlField.value = 1;

            // Create a new field, disable it, and give it a style class.
            var csharpField = new LayerField("C# Field");
            csharpField.SetEnabled(false);
            csharpField.AddToClassList("some-styled-field");
            csharpField.value = uxmlField.value;
            container.Add(csharpField);

            // Mirror value of uxml field into the C# field.
            uxmlField.RegisterCallback<ChangeEvent<int>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
            /// </sample>
        }
    }
}
