#if UNITY_INPUT_SYSTEM

namespace UnityEngine.UIElements
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
