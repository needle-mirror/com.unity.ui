using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    internal static class EditorDelegateRegistration
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            VisualTreeAssetChangeTrackerUpdater.IsEditorPlaying = IsEditorPlaying;
            DefaultEventSystem.IsEditorRemoteConnected = IsEditorRemoteConnected;
            VisualTreeAssetChangeTrackerUpdater.UpdateGameView = EditorApplication.QueuePlayerLoopUpdate;
            AssetOperationsAccess.GetAssetPath = GetAssetPath;
            AssetOperationsAccess.GetAssetDirtyCount = GetAssetDirtyCount;
            PanelTextSettings.EditorGUIUtilityLoad = EditorGUIUtilityLoad;
            PanelTextSettings.GetCurrentLanguage = GetCurrentLanguage;
            DropdownUtility.MakeDropdownFunc = CreateGenericOSMenu;
        }

        internal static SystemLanguage GetCurrentLanguage()
        {
            return LocalizationDatabase.currentEditorLanguage;
        }

        internal static bool IsEditorPlaying()
        {
            return EditorApplication.isPlaying;
        }

        internal static bool IsEditorRemoteConnected()
        {
            return EditorApplication.isRemoteConnected;
        }

        internal static string GetAssetPath(Object asset)
        {
            return AssetDatabase.GetAssetPath(asset);
        }

        internal static int GetAssetDirtyCount(Object asset)
        {
            return EditorUtility.GetDirtyCount(asset);
        }

        internal static Object EditorGUIUtilityLoad(string path)
        {
            return EditorGUIUtility.Load(path);
        }

        private static GenericOSMenu CreateGenericOSMenu()
        {
            return new GenericOSMenu();
        }
    }
}
