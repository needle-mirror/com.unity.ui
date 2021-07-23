#if UIE_PACKAGE
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    static class PackageUtilityEditorHelper
    {
        // We try to load this specific asset to see if the package has finished importing.
        // This is not ideal, but at least we have to test to make sure this code gets updated
        // in case this path changes.
        internal static readonly string DetectionAssetPath =
            "Packages/com.unity.ui/PackageResources/StyleSheets/Generated/DefaultCommonDark.uss.asset";

        // Called through reflection by UIElementsPackageUtility
        public static bool HasPackageFullyLoaded()
        {
            if (AssetDatabase.LoadAssetAtPath<StyleSheet>(DetectionAssetPath) != null)
            {
                return true;
            }
            else
            {
                // Our package wasn't imported yet. We must not attempt to load package resources.
                // We'll monitor the status of the asset and ask for a script reload next frame.
                EditorApplication.delayCall += DoScriptReload;
                return false;
            }
        }

        static void DoScriptReload()
        {
            if (AssetDatabase.LoadAssetAtPath<StyleSheet>(DetectionAssetPath) != null)
            {
                // Now that our asset is imported, let's reload the world to use package resources.
                EditorUtility.RequestScriptReload();
            }
            else
            {
                Debug.LogError("UI Toolkit package failed to initialize. Please restart the Unity Editor and report a bug.");
            }
        }
    }
}
#endif
