using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEngine.UIElements
{
    internal static class UXMLRuntimeFactories
    {
        private static readonly bool k_Registered;

        #if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        internal static void RegisterUserFactories()
        {
            HashSet<string> userAssemblies = new HashSet<string>(GetAllUserAssemblies());
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (!(userAssemblies.Contains(assembly.GetName().Name + ".dll"))
                    || assembly.GetName().Name == "UnityEngine.UIElementsModule")
                    continue;

                var types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (!typeof(IUxmlFactory).IsAssignableFrom(type)
                        || type.IsInterface
                        || type.IsAbstract
                        || type.IsGenericType)
                        continue;

                    var factory = (IUxmlFactory)Activator.CreateInstance(type);
                    RegisterFactory(factory);
                }
            }
        }

        static void RegisterFactory(IUxmlFactory factory)
        {
            VisualElementFactoryRegistry.RegisterFactory(factory);
        }

        static string[] GetAllUserAssemblies()
        {
            return ScriptingRuntime.GetAllUserAssemblies();
        }

        #endif
    }
}
