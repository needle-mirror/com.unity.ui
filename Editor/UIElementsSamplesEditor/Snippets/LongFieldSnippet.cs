using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class LongFieldSnippet : ElementSnippet<LongFieldSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the field from UXML and assign it its value.
            var uxmlField = container.Q<LongField>("the-uxml-field");
            uxmlField.value = 42;

            // Create a new field, disable it, and give it a style class.
            var csharpField = new LongField("C# Field");
            csharpField.SetEnabled(false);
            csharpField.AddToClassList("some-styled-field");
            csharpField.value = uxmlField.value;
            container.Add(csharpField);

            // Mirror value of uxml field into the C# field.
            uxmlField.RegisterCallback<ChangeEvent<long>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
            /// </sample>
        }
    }
}
