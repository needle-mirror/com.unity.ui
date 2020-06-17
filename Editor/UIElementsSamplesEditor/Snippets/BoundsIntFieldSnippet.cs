using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class BoundsIntFieldSnippet : ElementSnippet<BoundsIntFieldSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the field from UXML and assign it its value.
            var uxmlField = container.Q<BoundsIntField>("the-uxml-field");
            uxmlField.value = new BoundsInt(new Vector3Int(1, 2, 3), new Vector3Int(2, 4, 6));

            // Create a new field, disable it, and give it a style class.
            var csharpField = new BoundsIntField("C# Field");
            csharpField.SetEnabled(false);
            csharpField.AddToClassList("some-styled-field");
            csharpField.value = uxmlField.value;
            container.Add(csharpField);

            // Mirror value of uxml field into the C# field.
            uxmlField.RegisterCallback<ChangeEvent<BoundsInt>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
            /// </sample>
        }
    }
}
