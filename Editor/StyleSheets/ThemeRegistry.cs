using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements.StyleSheets
{
    static class ThemeRegistry
    {
        internal static string k_DefaultStyleSheetPath
        {
            get
            {
#if UIE_PACKAGE
                return "Packages/com.unity.ui/PackageResources/StyleSheets/Generated/Default.tss.asset";
#else
                return "StyleSheets/Generated/Default.tss.asset";
#endif
            }
        }

        public const string kThemeScheme = "unity-theme";
        public const string kUnityThemesPath = "Assets/UI Toolkit/UnityThemes";
        public const string kUnityRuntimeThemeFileName = "UnityDefaultRuntimeTheme.tss";
        public const string kUnityRuntimeThemePath = kUnityThemesPath + "/" + kUnityRuntimeThemeFileName;
        private static Dictionary<string, string> m_Themes;

        public static Dictionary<string, string> themes
        {
            get
            {
                if (m_Themes == null)
                {
                    m_Themes = new Dictionary<string, string>();

                    RegisterTheme("default", k_DefaultStyleSheetPath);
                }
                return m_Themes;
            }
        }

        public static void RegisterTheme(string themeName, string path)
        {
            themes[themeName] = path;
        }

        public static void UnregisterTheme(string themeName)
        {
            themes.Remove(themeName);
        }

#if UIE_PACKAGE
        private static bool s_IsDefaultThemeReady = false;
        internal static void RefreshDefaultThemes()
        {
            if (!s_IsDefaultThemeReady)
            {
                RegisterTheme("default", k_DefaultStyleSheetPath);
                s_IsDefaultThemeReady = UIElementsPackageUtility.IsUIEPackageLoaded;
            }
        }

#endif // UIE_PACKAGE
    }
}
