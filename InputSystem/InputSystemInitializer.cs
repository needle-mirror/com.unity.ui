#if UNITY_INPUT_SYSTEM

namespace UnityEngine.InputSystem.Composites
{
    internal class InputSystemInitializer
    {
        private static bool s_Initialized;

        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        #endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Init()
        {
            if (!s_Initialized)
            {
                RegisterBindings();
                s_Initialized = true;
            }
        }

        static void RegisterBindings()
        {
            InputSystem.RegisterBindingComposite<ButtonWithSignModifier>();
        }
    }
}
#endif
