using System;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements.StyleSheets;
using Button = UnityEngine.UIElements.Button;

namespace UnityEditor.UIElements
{
    static class PanelSettingsCreator
    {
        static void CreateDirectoryRecursively(string dirPath)
        {
            var paths = dirPath.Split('/');
            string currentPath = "";

            foreach (var path in paths)
            {
                currentPath += path;
                if (!Directory.Exists(currentPath))
                {
                    Directory.CreateDirectory(currentPath);
                }
                currentPath += "/";
            }
        }

        internal static string GetTssTemplateContent()
        {
            return "@import url(\"" + ThemeRegistry.kThemeScheme + "://default\");";
        }

        internal static ThemeStyleSheet GetOrCreateDefaultTheme()
        {
            // Create unity default theme if it is not there
            var defaultTssAsset = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(ThemeRegistry.kUnityRuntimeThemePath);
            if (defaultTssAsset == null)
            {
                CreateDirectoryRecursively(ThemeRegistry.kUnityThemesPath);
                File.WriteAllText(ThemeRegistry.kUnityRuntimeThemePath, GetTssTemplateContent());
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                defaultTssAsset = AssetDatabase.LoadAssetAtPath<ThemeStyleSheet>(ThemeRegistry.kUnityRuntimeThemePath);
            }

            // Reimport the asset in a delayed call because the loading of the internal tss may fail if it happens
            // during domain reload (on package update for example). This needs to be done when we're creating the asset
            // and when we know it already exists because the first may not work and the second time be called (not sure
            // why it calls twice, but it may fail the first time because the assets are not done loading).
            EditorApplication.delayCall += () =>
            {
                AssetDatabase.ImportAsset(ThemeRegistry.kUnityRuntimeThemePath);
            };

            return defaultTssAsset;
        }

        [MenuItem("Assets/Create/UI Toolkit/Panel Settings Asset", false, 701, false)]
        static void CreatePanelSettings()
        {
            var defaultTssAsset = GetOrCreateDefaultTheme();

            PanelSettings settings = ScriptableObject.CreateInstance<PanelSettings>();
            settings.themeStyleSheet = defaultTssAsset;
            ProjectWindowUtil.CreateAsset(settings, "New Panel Settings.asset");
        }
    }
}
