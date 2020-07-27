using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    public class DoubleField : TextValueField<double>
    {
        // This property to alleviate the fact we have to cast all the time
        DoubleInput doubleInput => (DoubleInput)textInputBase;

        public new class UxmlFactory : UxmlFactory<DoubleField, UxmlTraits> {}
        public new class UxmlTraits : TextValueFieldTraits<double, UxmlDoubleAttributeDescription> {}

        protected override string ValueToString(double v)
        {
            return v.ToString(formatString, CultureInfo.InvariantCulture.NumberFormat);
        }

        protected override double StringToValue(string str)
        {
            double v;
            EditorGUI.StringToDouble(str, out v);
            return v;
        }

        public new static readonly string ussClassName = "unity-double-field";
        public new static readonly string labelUssClassName = ussClassName + "__label";
        public new static readonly string inputUssClassName = ussClassName + "__input";

        public DoubleField() : this((string)null) {}

        public DoubleField(int maxLength)
            : this(null, maxLength) {}

        public DoubleField(string label, int maxLength = kMaxLengthNone)
            : base(label, maxLength, new DoubleInput())
        {
            AddToClassList(ussClassName);
            labelElement.AddToClassList(labelUssClassName);
            visualInput.AddToClassList(inputUssClassName);
            AddLabelDragger<double>();
        }

        internal override bool CanTryParse(string textString) => double.TryParse(textString, out _);

        public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, double startValue)
        {
            doubleInput.ApplyInputDeviceDelta(delta, speed, startValue);
        }

        class DoubleInput : TextValueInput
        {
            DoubleField parentDoubleField => (DoubleField)parent;

            internal DoubleInput()
            {
                formatString = EditorGUI.kDoubleFieldFormatString;
            }

            protected override string allowedCharacters
            {
                get { return EditorGUI.s_AllowedCharactersForFloat; }
            }

            public override void ApplyInputDeviceDelta(Vector3 delta, DeltaSpeed speed, double startValue)
            {
                double sensitivity = NumericFieldDraggerUtility.CalculateFloatDragSensitivity(startValue);
                float acceleration = NumericFieldDraggerUtility.Acceleration(speed == DeltaSpeed.Fast, speed == DeltaSpeed.Slow);
                double v = StringToValue(text);
                v += NumericFieldDraggerUtility.NiceDelta(delta, acceleration) * sensitivity;
                v = MathUtils.RoundBasedOnMinimumDifference(v, sensitivity);
                if (parentDoubleField.isDelayed)
                {
                    text = ValueToString(v);
                }
                else
                {
                    parentDoubleField.value = v;
                }
            }

            protected override string ValueToString(double v)
            {
                return v.ToString(formatString);
            }

            protected override double StringToValue(string str)
            {
                double v;
                EditorGUI.StringToDouble(str, out v);
                return v;
            }
        }
    }
}
