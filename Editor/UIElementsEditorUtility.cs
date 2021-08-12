using System;
using System.IO;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    // This is the required interface to UIElementsEditorUtility for Runtime game components.
    internal static class UIElementsEditorRuntimeUtility
    {
        public static void CreateRuntimePanelDebug(IPanel panel)
        {
            var panelDebug = new PanelDebug(panel);
            (panel as Panel).panelDebug = panelDebug;
        }
    }

    internal static class UIElementsEditorUtility
    {
        internal static readonly string s_DefaultCommonDarkStyleSheetPath =
            Path.Combine(UIElementsPackageUtility.EditorResourcesBasePath, "StyleSheets/Generated/DefaultCommonDark.uss.asset");
        internal static readonly string s_DefaultCommonLightStyleSheetPath =
            Path.Combine(UIElementsPackageUtility.EditorResourcesBasePath, "StyleSheets/Generated/DefaultCommonLight.uss.asset");

        private static StyleSheet s_DefaultCommonDarkStyleSheet;
        private static StyleSheet s_DefaultCommonLightStyleSheet;

        internal static string GetStyleSheetPathForFont(string sheetPath, string fontName)
        {
            return sheetPath.Replace(".uss", "_" + fontName.ToLowerInvariant() + ".uss");
        }

        internal static string GetStyleSheetPathForCurrentFont(string sheetPath)
        {
            return GetStyleSheetPathForFont(sheetPath, EditorResources.currentFontName);
        }

        internal static StyleSheet LoadSKinnedStyleSheetForFont(int skin, string fontName)
        {
            return EditorGUIUtility.Load(GetStyleSheetPathForFont(skin == EditorResources.darkSkinIndex ? s_DefaultCommonDarkStyleSheetPath : s_DefaultCommonLightStyleSheetPath, fontName)) as StyleSheet;
        }

        internal static bool IsCommonDarkStyleSheetLoaded()
        {
            return s_DefaultCommonDarkStyleSheet != null;
        }

        internal static StyleSheet GetCommonDarkStyleSheet()
        {
            if (s_DefaultCommonDarkStyleSheet == null)
            {
                s_DefaultCommonDarkStyleSheet = LoadSKinnedStyleSheetForFont(EditorResources.darkSkinIndex, EditorResources.currentFontName);
                if (s_DefaultCommonDarkStyleSheet != null)
                    s_DefaultCommonDarkStyleSheet.isDefaultStyleSheet = true;
            }

            return s_DefaultCommonDarkStyleSheet;
        }

        internal static bool IsCommonLightStyleSheetLoaded()
        {
            return s_DefaultCommonLightStyleSheet != null;
        }

        internal static StyleSheet GetCommonLightStyleSheet()
        {
            if (s_DefaultCommonLightStyleSheet == null)
            {
                s_DefaultCommonLightStyleSheet = LoadSKinnedStyleSheetForFont(EditorResources.normalSkinIndex, EditorResources.currentFontName);
                if (s_DefaultCommonLightStyleSheet != null)
                    s_DefaultCommonLightStyleSheet.isDefaultStyleSheet = true;
            }

            return s_DefaultCommonLightStyleSheet;
        }

        static UIElementsEditorUtility()
        {
        }

        internal static int GetCursorId(StyleSheet sheet, StyleValueHandle handle)
        {
            var value = sheet.ReadEnum(handle);
            if (string.Equals(value, "arrow", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.Arrow;
            else if (string.Equals(value, "text", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.Text;
            else if (string.Equals(value, "resize-vertical", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.ResizeVertical;
            else if (string.Equals(value, "resize-horizontal", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.ResizeHorizontal;
            else if (string.Equals(value, "link", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.Link;
            else if (string.Equals(value, "slide-arrow", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.SlideArrow;
            else if (string.Equals(value, "resize-up-right", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.ResizeUpRight;
            else if (string.Equals(value, "resize-up-left", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.ResizeUpLeft;
            else if (string.Equals(value, "move-arrow", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.MoveArrow;
            else if (string.Equals(value, "rotate-arrow", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.RotateArrow;
            else if (string.Equals(value, "scale-arrow", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.ScaleArrow;
            else if (string.Equals(value, "arrow-plus", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.ArrowPlus;
            else if (string.Equals(value, "arrow-minus", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.ArrowMinus;
            else if (string.Equals(value, "pan", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.Pan;
            else if (string.Equals(value, "orbit", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.Orbit;
            else if (string.Equals(value, "zoom", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.Zoom;
            else if (string.Equals(value, "fps", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.FPS;
            else if (string.Equals(value, "split-resize-up-down", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.SplitResizeUpDown;
            else if (string.Equals(value, "split-resize-left-right", StringComparison.OrdinalIgnoreCase))
                return (int)MouseCursor.SplitResizeLeftRight;

            return (int)MouseCursor.Arrow;
        }

        private static readonly string k_DefaultStylesAppliedPropertyName = "DefaultStylesApplied";
        internal static void AddDefaultEditorStyleSheets(VisualElement ve)
        {
            if (ve.styleSheets.count == 0 || ve.GetProperty(k_DefaultStylesAppliedPropertyName) == null)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    ve.styleSheets.Add(GetCommonDarkStyleSheet());
                }
                else
                {
                    ve.styleSheets.Add(GetCommonLightStyleSheet());
                }

                ve.SetProperty(k_DefaultStylesAppliedPropertyName, true);
            }
        }

        internal static void ForceDarkStyleSheet(VisualElement ele)
        {
            if (!EditorGUIUtility.isProSkin)
            {
                var lightStyle = GetCommonLightStyleSheet();
                var darkStyle = GetCommonDarkStyleSheet();
                var e = ele;
                while (e != null)
                {
                    if (e.styleSheets.Contains(lightStyle))
                    {
                        e.styleSheets.Swap(lightStyle, darkStyle);
                        break;
                    }
                    e = e.parent;
                }
            }
        }

        #if UIE_PACKAGE
        public static string[] s_Path =
        {
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/GlobalFallback - Linux.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/GlobalFallback - OSX.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/GlobalFallback - Win.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/Linux/DejavuSans SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/Linux/DroidSans SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/Linux/FreeSans SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/Linux/LiberationSans SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/OSX/Arial SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/OSX/Arial Unicode SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/OSX/Helvetica SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/OSX/LastResort SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/Windows/Gulim SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/Windows/MS Gothic SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/GlobalFallback/Windows/Microsoft YaHei SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/ChineseSimplified - Linux.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/ChineseSimplified - OSX.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/ChineseSimplified - Win.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/ChineseTraditional - Linux.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/ChineseTraditional - OSX.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/ChineseTraditional - Win.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/English - Linux.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/English - OSX.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/English - Win.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Japanese - Linux.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Japanese - OSX.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Japanese - Win.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Korean - Linux.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Korean - OSX.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Korean - Win.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Linux/NotoSansCJKjp-Bold SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Linux/NotoSansCJKjp-Regular SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Linux/NotoSansCJKkr-Bold SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Linux/NotoSansCJKkr-Regular SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Linux/NotoSansCJKsc-Bold SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Linux/NotoSansCJKsc-Regular SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Linux/NotoSansCJKtc-Bold SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Linux/NotoSansCJKtc-Regular SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/OSX/AppleSDGothicNeo Bold SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/OSX/AppleSDGothicNeo SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/OSX/Hiragino Sans W3 SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/OSX/Hiragino Sans W6 SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/OSX/Microsoft Sans Serif SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/OSX/PingFang SC Medium SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/OSX/PingFang SC SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/OSX/PingFang TC Medium SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/OSX/PingFang TC SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Windows/Malgun Gothic Bold SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Windows/Malgun Gothic SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Windows/Microsoft JhengHei Bold SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Windows/Microsoft JhengHei SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Windows/Microsoft YaHei UI Bold SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Windows/Microsoft YaHei UI SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Windows/Yu Gothic UI Bold SDF.asset",
            "Packages/com.unity.ui/PackageResources/FontAssets/DynamicOSFontAssets/Localization/Windows/Yu Gothic UI SDF.asset",
            "Packages/com.unity.ui/PackageResources/Fonts/Lucida Grande Bold SDF.asset",
            "Packages/com.unity.ui/PackageResources/Fonts/Lucida Grande SDF.asset",
            "Packages/com.unity.ui/PackageResources/Fonts/Inter/Inter-Italic SDF.asset",
            "Packages/com.unity.ui/PackageResources/Fonts/Inter/Inter-Regular SDF.asset",
            "Packages/com.unity.ui/PackageResources/Fonts/Inter/Inter-SemiBold SDF.asset",
            "Packages/com.unity.ui/PackageResources/Fonts/Inter/Inter-SemiBoldItalic SDF.asset",
            "Packages/com.unity.ui/PackageResources/Fonts/RobotoMono/RobotoMono-Bold SDF.asset",
            "Packages/com.unity.ui/PackageResources/Fonts/RobotoMono/RobotoMono-BoldItalic SDF.asset",
            "Packages/com.unity.ui/PackageResources/Fonts/RobotoMono/RobotoMono-Italic SDF.asset",
            "Packages/com.unity.ui/PackageResources/Fonts/RobotoMono/RobotoMono-Regular SDF.asset"
        };

        [InitializeOnLoadMethod]
        public static void AllowUITKResourcesToBeModified()
        {
            UnityEditor.PackageManager.ImmutableAssets.SetAssetsAllowedToBeModified(s_Path);
        }

        #endif
    }
}
