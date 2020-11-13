using UnityEngine.UIElements.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace UnityEditor.UIElements.Text
{
    internal class TextSettingsImporter
    {
        static string k_PackageName = "UITK Essential Text Resources";

        private static string k_EssentialResourcesPackagePath =
            "Packages/com.unity.ui/PackageResources/Text/" + k_PackageName + ".unitypackage";

        TextSettingsImporter() {}

        internal static void ImportEssentialTextResources()
        {
            AssetDatabase.importPackageCompleted += ImportCallback;
            AssetDatabase.ImportPackage(k_EssentialResourcesPackagePath, false);
        }

        static void ImportCallback(string packageName)
        {
            if (packageName == k_PackageName)
            {
                if (PanelTextSettings.HasTextSettings)
                    UnityEngine.UIElements.TextDelegates.OnTextSettingsImported?.Invoke();
                AssetDatabase.importPackageCompleted -= ImportCallback;
            }
        }
    }
}
