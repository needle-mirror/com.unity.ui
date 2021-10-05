using System.IO;
#if UIE_PACKAGE && UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using System.Linq;
#endif

namespace UnityEngine.UIElements
{
    internal static class UIElementsPackageUtility
    {
#if UIE_PACKAGE
        internal static readonly string UIEPackageRootFolder = "Packages/com.unity.ui/";
#endif //UIE_PACKAGE
        internal static bool IsUIEPackageLoaded { get; private set; }
        internal static string EditorResourcesBasePath { get; private set; }

        static UIElementsPackageUtility()
        {
            Refresh();
        }

        internal static void Refresh()
        {
#if UIE_PACKAGE
            if (IsUIEPackageLoaded)
                return;
#if UNITY_EDITOR
            if (!HasPackageLoaded())
            {
                EditorResourcesBasePath = "";
                IsUIEPackageLoaded = false;
            }
            else
#endif // UNITY_EDITOR
            {
                EditorResourcesBasePath = Path.Combine(UIEPackageRootFolder , "PackageResources/");
                IsUIEPackageLoaded = true;
            }
#else // UIE_PACKAGE
            EditorResourcesBasePath = "";
            IsUIEPackageLoaded = false;
#endif // UIE_PACKAGE
        }

#if UIE_PACKAGE && UNITY_EDITOR
        static bool HasPackageLoaded()
        {
            string assemblyName = "UnityEditor.UIElementsModule";
            var editorAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);

            if (editorAssembly == null)
            {
                Debug.LogError($"Can't find {assemblyName} assembly");
                return false;
            }

            var type = editorAssembly.GetType("UnityEditor.UIElements.PackageUtilityEditorHelper");

            if (type == null)
            {
                Debug.LogError("Can't find PackageUtilityEditorHelper type");
                return false;
            }

            var hasPackageLoaded = type.GetMethod("HasPackageFullyLoaded", BindingFlags.Static | BindingFlags.Public, null, new Type[] {}, null);

            if (hasPackageLoaded == null)
            {
                Debug.LogError("Can't find PackageUtilityEditorHelper.HasPackageFullyLoaded static method");
                return false;
            }

            return (bool)hasPackageLoaded.Invoke(null, null);
        }

#endif
    }
}
