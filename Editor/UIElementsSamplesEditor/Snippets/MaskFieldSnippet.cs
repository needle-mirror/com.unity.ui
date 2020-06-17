using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class MaskFieldSnippet : ElementSnippet<MaskFieldSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the field from UXML and assign it its value.
            var uxmlField = container.Q<MaskField>("the-uxml-field");
            uxmlField.value = 1;
            uxmlField.choices = new List<string> { "First", "Second", "Third" };

            // Create a new field, disable it, and give it a style class.
            var csharpField = new MaskField("C# Field");
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
