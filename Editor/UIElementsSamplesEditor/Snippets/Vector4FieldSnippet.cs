using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class Vector4FieldSnippet : ElementSnippet<Vector4FieldSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the field from UXML and assign it its value.
            var uxmlField = container.Q<Vector4Field>("the-uxml-field");
            uxmlField.value = new Vector4(23.8f, 12.6f, 88.3f, 92.1f);

            // Create a new field, disable it, and give it a style class.
            var csharpField = new Vector4Field("C# Field");
            csharpField.SetEnabled(false);
            csharpField.AddToClassList("some-styled-field");
            csharpField.value = uxmlField.value;
            container.Add(csharpField);

            // Mirror value of uxml field into the C# field.
            uxmlField.RegisterCallback<ChangeEvent<Vector4>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
            /// </sample>
        }
    }
}
