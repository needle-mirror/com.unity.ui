using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class ObjectFieldSnippet : ElementSnippet<ObjectFieldSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the field from UXML and assign it its value.
            var uxmlField = container.Q<ObjectField>("the-uxml-field");
            uxmlField.value = new Texture2D(10, 10) { name = "new_texture" };

            // Create a new field, disable it, and give it a style class.
            var csharpField = new ObjectField("C# Field");
            csharpField.SetEnabled(false);
            csharpField.AddToClassList("some-styled-field");
            csharpField.value = uxmlField.value;
            container.Add(csharpField);

            // Mirror value of uxml field into the C# field.
            uxmlField.RegisterCallback<ChangeEvent<Object>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
            /// </sample>
        }
    }
}
