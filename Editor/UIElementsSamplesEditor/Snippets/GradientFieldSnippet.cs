using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class GradientFieldSnippet : ElementSnippet<GradientFieldSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            var initialValue = new Gradient();
            initialValue.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(Color.red, 0),
                new GradientColorKey(Color.blue, 10),
                new GradientColorKey(Color.green, 20)
            };

            // Get a reference to the field from UXML and assign it its value.
            var uxmlField = container.Q<GradientField>("the-uxml-field");
            uxmlField.value = initialValue;

            // Create a new field, disable it, and give it a style class.
            var csharpField = new GradientField("C# Field");
            csharpField.SetEnabled(false);
            csharpField.AddToClassList("some-styled-field");
            csharpField.value = uxmlField.value;
            container.Add(csharpField);

            // Mirror value of uxml field into the C# field.
            uxmlField.RegisterCallback<ChangeEvent<Gradient>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
            /// </sample>
        }
    }
}
