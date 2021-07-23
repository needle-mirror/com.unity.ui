using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
    /// <summary>
    /// A control that allows single selection out of a logical group of <see cref="RadioButton"/> elements. Selecting one will deselect the others.
    /// </summary>
    public class RadioButtonGroup : BaseField<int>, IGroupBox
    {
        /// <summary>
        /// Instantiates a <see cref="RadioButtonGroup"/> using data from a UXML file.
        /// </summary>
        public new class UxmlFactory : UxmlFactory<RadioButtonGroup, UxmlTraits> {}

        /// <summary>
        /// Defines <see cref="UxmlTraits"/> for the <see cref="RadioButtonGroup"/>.
        /// </summary>
        public new class UxmlTraits : BaseFieldTraits<int, UxmlIntAttributeDescription>
        {
            UxmlStringAttributeDescription m_Choices = new UxmlStringAttributeDescription { name = "choices" };

            /// <summary>
            /// Initializes <see cref="RadioButtonGroup"/> properties using values from the attribute bag.
            /// </summary>
            /// <param name="ve">The object to initialize.</param>
            /// <param name="bag">The attribute bag.</param>
            /// <param name="cc">The creation context; unused.</param>
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                var f = (RadioButtonGroup)ve;
                f.choices = ParseChoiceList(m_Choices.GetValueFromBag(bag, cc));
            }
        }

        /// <summary>
        /// USS class name for RadioButtonGroup elements.
        /// </summary>
        /// <remarks>
        /// Unity adds this USS class to every instance of the RadioButtonGroup element. Any styling applied to
        /// this class affects every RadioButtonGroup located beside, or below the stylesheet in the visual tree.
        /// </remarks>
        public new static readonly string ussClassName = "unity-radio-button-group";

        IEnumerable<string> m_Choices;
        List<RadioButton> m_RadioButtons = new List<RadioButton>();
        EventCallback<ChangeEvent<bool>> m_RadioButtonValueChangedCallback;

        /// <summary>
        /// The list of available choices in the group.
        /// </summary>
        /// <remarks>
        /// Writing to this property removes existing <see cref="RadioButton"/> elements and
        /// re-creates them to display the new list.
        /// </remarks>
        public IEnumerable<string> choices
        {
            get => m_Choices;
            set
            {
                m_Choices = value;

                foreach (var radioButton in m_RadioButtons)
                {
                    radioButton.UnregisterValueChangedCallback(m_RadioButtonValueChangedCallback);
                    radioButton.RemoveFromHierarchy();
                }

                m_RadioButtons.Clear();

                if (m_Choices != null)
                {
                    foreach (var choice in m_Choices)
                    {
                        var radioButton = new RadioButton() { text = choice };
                        radioButton.RegisterValueChangedCallback(m_RadioButtonValueChangedCallback);
                        m_RadioButtons.Add(radioButton);
                        visualInput.Add(radioButton);
                    }

                    UpdateRadioButtons();
                }
            }
        }

        /// <summary>
        /// Initializes and returns an instance of RadioButtonGroup.
        /// </summary>
        public RadioButtonGroup()
            : this(null) {}

        /// <summary>
        /// Initializes and returns an instance of RadioButtonGroup.
        /// </summary>
        /// <param name="label">The label for this group</param>
        /// <param name="radioButtonChoices">The choices to display in this group</param>
        public RadioButtonGroup(string label, List<string> radioButtonChoices = null)
            : base(label, null)
        {
            AddToClassList(ussClassName);

            m_RadioButtonValueChangedCallback = RadioButtonValueChangedCallback;
            choices = radioButtonChoices;
            value = -1;
            visualInput.focusable = false;
            delegatesFocus = true;
        }

        void RadioButtonValueChangedCallback(ChangeEvent<bool> evt)
        {
            if (evt.newValue)
            {
                value = m_RadioButtons.IndexOf(evt.target as RadioButton);
                evt.StopPropagation();
            }
        }

        public override void SetValueWithoutNotify(int newValue)
        {
            base.SetValueWithoutNotify(newValue);
            UpdateRadioButtons();
        }

        void UpdateRadioButtons()
        {
            if (value >= 0 && value < m_RadioButtons.Count)
            {
                m_RadioButtons[value].value = true;
            }
            else
            {
                foreach (var radioButton in m_RadioButtons)
                {
                    radioButton.value = false;
                }
            }
        }
    }
}
