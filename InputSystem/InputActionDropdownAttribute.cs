#if UNITY_INPUT_SYSTEM

namespace UnityEngine.UIElements.InputSystem
{
    internal class InputActionDropdownAttribute : PropertyAttribute
    {
        public string inputActionAssetProperty { get; }

        public InputActionDropdownAttribute(string inputActionAssetProperty)
        {
            this.inputActionAssetProperty = inputActionAssetProperty;
        }
    }
}
#endif
