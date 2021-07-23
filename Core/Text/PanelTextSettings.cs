using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements
{
    /// <summary>
    /// Represents text rendering settings for a specific UI panel.
    /// <seealso cref="PanelSettings.textSettings"/>
    /// </summary>
    public class PanelTextSettings : TextSettings
    {
        private static PanelTextSettings s_DefaultPanelTextSettings;

        internal static PanelTextSettings defaultPanelTextSettings
        {
            get
            {
                if (s_DefaultPanelTextSettings == null)
                {
                    #if UNITY_EDITOR
                    s_DefaultPanelTextSettings = EditorGUIUtilityLoad(s_DefaultEditorPanelTextSettingPath) as PanelTextSettings;
                    if (s_DefaultPanelTextSettings != null)
                        UpdateLocalizationFontAsset();
                    #endif

                    if (s_DefaultPanelTextSettings == null)
                        s_DefaultPanelTextSettings = ScriptableObject.CreateInstance<PanelTextSettings>();
                }

                return s_DefaultPanelTextSettings;
            }
        }

        internal static void UpdateLocalizationFontAsset()
        {
            string platform = " - Linux";
#if UNITY_EDITOR_WIN
            platform = " - Win";
#elif UNITY_EDITOR_OSX
            platform = " - OSX";
#endif
            var localizationAssetPathPerSystemLanguage = new Dictionary<SystemLanguage, string>()
            {
                { SystemLanguage.English, Path.Combine(s_ResourcesPath, $"FontAssets/DynamicOSFontAssets/Localization/English{platform}.asset") },
                { SystemLanguage.Japanese, Path.Combine(s_ResourcesPath, $"FontAssets/DynamicOSFontAssets/Localization/Japanese{platform}.asset") },
                { SystemLanguage.ChineseSimplified, Path.Combine(s_ResourcesPath, $"FontAssets/DynamicOSFontAssets/Localization/ChineseSimplified{platform}.asset") },
                { SystemLanguage.ChineseTraditional, Path.Combine(s_ResourcesPath, $"FontAssets/DynamicOSFontAssets/Localization/ChineseTraditional{platform}.asset") },
                { SystemLanguage.Korean, Path.Combine(s_ResourcesPath, $"FontAssets/DynamicOSFontAssets/Localization/Korean{platform}.asset") }
            };

            var globalFallbackAssetPath = Path.Combine(s_ResourcesPath, $"FontAssets/DynamicOSFontAssets/GlobalFallback/GlobalFallback{platform}.asset");

            var localizationAsset = EditorGUIUtilityLoad(localizationAssetPathPerSystemLanguage[GetCurrentLanguage()]) as FontAsset;
            var globalFallbackAsset = EditorGUIUtilityLoad(globalFallbackAssetPath) as FontAsset;

            defaultPanelTextSettings.fallbackFontAssets[0] = localizationAsset;
            defaultPanelTextSettings.fallbackFontAssets[defaultPanelTextSettings.fallbackFontAssets.Count - 1] = globalFallbackAsset;
        }

        internal FontAsset GetCachedFontAsset(Font font)
        {
            return GetCachedFontAssetInternal(font);
        }

        #if UIE_PACKAGE
        internal static readonly string s_DefaultEditorPanelTextSettingPath = "Packages/com.unity.ui/PackageResources/Default Editor Text Settings.asset";
        internal static readonly string s_ResourcesPath = "Packages/com.unity.ui/PackageResources/";

        #else
        internal static readonly string s_DefaultEditorPanelTextSettingPath = "UIPackageResources/Default Editor Text Settings.asset";
        internal static readonly string s_ResourcesPath = "UIPackageResources";
        #endif


        internal static Func<string, Object> EditorGUIUtilityLoad;
        internal static Func<SystemLanguage> GetCurrentLanguage;
    }
}
