using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    internal static class RuntimeEditorDelegateRegistration
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            UIDocument.IsEditorPlaying = IsEditorPlaying;
            UIDocument.IsEditorPlayingOrWillChangePlaymode = IsEditorPlayingOrWillChangePlaymode;
            UIDocument.UpdateGameView = UpdateGameView;
#if !UNITY_2020_2_OR_NEWER
            // This is a copy of an assignment in the Editor module used only for 2020.1 compatibility and should be
            // removed when support for 2020.1 is dropped.
            VisualTreeAssetChangeTrackerUpdater.IsEditorPlaying = IsEditorPlaying;
            AssetOperationsAccess.GetAssetPath = GetAssetPath;
            AssetOperationsAccess.GetAssetDirtyCount = GetAssetDirtyCount;
#endif
        }

        internal static string GetAssetPath(Object asset)
        {
            return AssetDatabase.GetAssetPath(asset);
        }

        internal static int GetAssetDirtyCount(Object asset)
        {
            return EditorUtility.GetDirtyCount(asset);
        }

        internal static bool IsEditorPlaying()
        {
            return EditorApplication.isPlaying;
        }

        internal static bool IsEditorPlayingOrWillChangePlaymode()
        {
            return EditorApplication.isPlayingOrWillChangePlaymode;
        }

        internal static void UpdateGameView()
        {
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }
}
