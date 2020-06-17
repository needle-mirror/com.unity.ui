using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class Vector3IntFieldSnippet : ElementSnippet<Vector3IntFieldSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the field from UXML and assign it its value.
            var uxmlField = container.Q<Vector3IntField>("the-uxml-field");
            uxmlField.value = new Vector3Int(23, 12, 88);

            // Create a new field, disable it, and give it a style class.
            var csharpField = new Vector3IntField("C# Field");
            csharpField.SetEnabled(false);
            csharpField.AddToClassList("some-styled-field");
            csharpField.value = uxmlField.value;
            container.Add(csharpField);

            // Mirror value of uxml field into the C# field.
            uxmlField.RegisterCallback<ChangeEvent<Vector3Int>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
            /// </sample>
        }
    }
}
