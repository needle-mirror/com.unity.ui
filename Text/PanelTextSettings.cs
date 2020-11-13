using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.UIElements.Text
{
    internal class PanelTextSettings : TextSettings
    {
        internal static string k_TextSettingsAssetName = "Default UITK Text Settings";
        internal static string k_TextSettingsPath = "Assets/UI Toolkit/" + k_TextSettingsAssetName;

        internal static string k_EditorTextSettingsPath =
            "Packages/com.unity.ui/PackageResources/Text/UITK Editor Text Settings.asset";

        const string k_AssetSearchByTypeTextSettings = "t:paneltextsettings";
        const string k_AssetsFolder = "Assets";
        static string[] k_AssetsFolderFilter = new[] { k_AssetsFolder };

        private static PanelTextSettings s_EditorTextSettings;

        internal static PanelTextSettings EditorTextSettings
        {
            get
            {
#if UNITY_EDITOR
                if (s_EditorTextSettings == null)
                {
                    s_EditorTextSettings = AssetDatabase.LoadAssetAtPath<PanelTextSettings>(k_EditorTextSettingsPath);
                    s_EditorTextSettings.InitializeFontReferenceLookup();
                }
#endif
                return s_EditorTextSettings;
            }
        }

        internal FontAsset GetCachedFontAsset(Font font)
        {
            return GetCachedFontAssetInternal(font);
        }

        internal static PanelTextSettings GetTextSettings()
        {
            #if UNITY_EDITOR
            var textSettingsInProject = AssetDatabase.FindAssets(k_AssetSearchByTypeTextSettings, k_AssetsFolderFilter).FirstOrDefault();
            if (!string.IsNullOrEmpty(textSettingsInProject))
                // Use the first one found.
                return AssetDatabase.LoadAssetAtPath<PanelTextSettings>(AssetDatabase.GUIDToAssetPath(textSettingsInProject));
            #endif
            return null;
        }

        internal static bool HasTextSettings
        {
            get
            {
                #if UNITY_EDITOR
                return AssetDatabase.FindAssets(k_AssetSearchByTypeTextSettings, k_AssetsFolderFilter).Length != 0;
                #else
                return false;
                #endif
            }
        }
    }
}
