using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class MinMaxSliderSnippet : ElementSnippet<MinMaxSliderSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the field from UXML and assign it its value.
            var uxmlField = container.Q<MinMaxSlider>("the-uxml-field");
            uxmlField.value = new Vector2(10, 12);

            // Create a new field, disable it, and give it a style class.
            var csharpField = new MinMaxSlider("C# Field", 0, 20, -10, 40);
            csharpField.SetEnabled(false);
            csharpField.AddToClassList("some-styled-field");
            csharpField.value = uxmlField.value;
            container.Add(csharpField);

            // Mirror value of uxml field into the C# field.
            uxmlField.RegisterCallback<ChangeEvent<Vector2>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
            /// </sample>
        }
    }
}
