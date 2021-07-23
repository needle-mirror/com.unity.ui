#if UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

namespace UnityEngine.InputSystem.Composites
{
    /// <summary>
    /// A button with a modifier inverting the sign of the resulting value.
    /// </summary>
    [Preserve]
    [DisplayStringFormat("{modifier}+{button}")]
    public class ButtonWithSignModifier : InputBindingComposite<float>
    {
        /// <summary>
        /// Binding for the button that acts as a modifier, e.g. <c>&lt;Keyboard/leftCtrl</c>.
        /// </summary>
        /// <value>Part index to use with <see cref="InputBindingCompositeContext.ReadValue{T}(int)"/>.</value>
        /// <remarks>
        /// This property is automatically assigned by the input system.
        /// </remarks>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once UnassignedField.Global
        [InputControl(layout = "Button")] public int modifier;

        /// <summary>
        /// Binding for the button whose sign is inverted by the modifier.
        /// </summary>
        /// <value>Part index to use with <see cref="InputBindingCompositeContext.ReadValue{T}(int)"/>.</value>
        /// <remarks>
        /// This property is automatically assigned by the input system.
        /// </remarks>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once UnassignedField.Global
        [InputControl(layout = "Button")] public int button;

        /// <summary>
        /// Return the value of the <see cref="button"/> part multiplied by 1 or -1 depending if <see cref="modifier"/> is pressed.
        /// </summary>
        /// <param name="context">Evaluation context passed in from the input system.</param>
        /// <returns>The current value of the composite.</returns>
        public override float ReadValue(ref InputBindingCompositeContext context)
        {
            return context.ReadValue<float>(button) * (context.ReadValueAsButton(modifier) ? -1 : 1);
        }

        /// <summary>
        /// Same as <see cref="ReadValue"/> in this case.
        /// </summary>
        /// <param name="context">Evaluation context passed in from the input system.</param>
        /// <returns>A >0 value if the composite is currently actuated.</returns>
        public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
        {
            return ReadValue(ref context);
        }
    }
}
#endif
