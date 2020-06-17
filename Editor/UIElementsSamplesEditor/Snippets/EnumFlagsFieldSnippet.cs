using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class EnumFlagsFieldSnippet : ElementSnippet<EnumFlagsFieldSnippet>
    {
        [Flags]
        enum EnumFlags
        {
            First = 1,
            Second = 2,
            Third = 4
        }

        internal override void Apply(VisualElement container)
        {
            /// <sample>
            // Get a reference to the field from UXML,
            // initialize it with an Enum type,
            // and assign it its value.
            var uxmlField = container.Q<EnumFlagsField>("the-uxml-field");
            uxmlField.Init(EnumFlags.First);
            uxmlField.value = EnumFlags.Second;

            // Create a new field, disable it, and give it a style class.
            var csharpField = new EnumFlagsField("C# Field", uxmlField.value);
            csharpField.SetEnabled(false);
            csharpField.AddToClassList("some-styled-field");
            csharpField.value = uxmlField.value;
            container.Add(csharpField);

            // Mirror value of uxml field into the C# field.
            uxmlField.RegisterCallback<ChangeEvent<Enum>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
            /// </sample>
        }
    }
}
