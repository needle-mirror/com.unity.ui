using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor
{
    static partial class UIElementsTemplate
    {
#if UIE_PACKAGE
        [UsedImplicitly, CommandHandler(nameof(CreateUSSFile))]
        private static void CreateUSSFile(CommandExecuteContext c)
        {
            CreateUSSAsset();
        }

#else

        // Add submenu after GUI Skin
        [MenuItem("Assets/Create/UIElements/USS File", false, 603, false)]
        public static void CreateUSSFile()
        {
            if (CommandService.Exists(nameof(CreateUSSFile)))
                CommandService.Execute(nameof(CreateUSSFile), CommandHint.Menu);
            else
                CreateUSSAsset();
        }

#endif

        private static void CreateUSSAsset()
        {
            var folder = GetCurrentFolder();
            var path = AssetDatabase.GenerateUniqueAssetPath(folder + "/NewUSSFile.uss");
            var contents = "VisualElement {}";
            var icon = EditorGUIUtility.IconContent<StyleSheet>().image as Texture2D;
            ProjectWindowUtil.CreateAssetWithContent(path, contents, icon);
        }
    }
}
