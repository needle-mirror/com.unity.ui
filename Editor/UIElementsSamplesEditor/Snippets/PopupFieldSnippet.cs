using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class PopupFieldSnippet : ElementSnippet<PopupFieldSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Note: PopupField has no UXML support because it is a generic type.

            var choices = new List<string> { "First", "Second", "Third" };

            // Create a new field and assign it its value.
            var normalField = new PopupField<string>("Normal Field", choices, 0);
            normalField.value = "Second";
            container.Add(normalField);

            // Create a new field, disable it, and give it a style class.
            var styledField = new PopupField<string>("Styled Field", choices, 0);
            styledField.SetEnabled(false);
            styledField.AddToClassList("some-styled-field");
            styledField.value = normalField.value;
            container.Add(styledField);

            // Mirror value of uxml field into the C# field.
            normalField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                styledField.value = evt.newValue;
            });
            /// </sample>
        }
    }
}
