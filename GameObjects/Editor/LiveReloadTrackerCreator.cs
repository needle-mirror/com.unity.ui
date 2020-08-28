using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    internal static class LiveReloadTrackerCreator
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            UIDocument.CreateLiveReloadVisualTreeAssetTracker = CreateVisualTreeAssetTrackerInstance;
            PanelSettings.CreateLiveReloadStyleSheetAssetTracker = CreateStyleSheetAssetTrackerInstance;
        }

        internal static ILiveReloadAssetTracker<VisualTreeAsset> CreateVisualTreeAssetTrackerInstance(UIDocument owner)
        {
            return new UIDocumentVisualTreeAssetTracker(owner);
        }

        internal static ILiveReloadAssetTracker<StyleSheet> CreateStyleSheetAssetTrackerInstance()
        {
            return new LiveReloadStyleSheetAssetTracker();
        }
    }

    internal class UIDocumentVisualTreeAssetTracker : BaseLiveReloadVisualTreeAssetTracker
    {
        private UIDocument m_Owner;

        public UIDocumentVisualTreeAssetTracker(UIDocument owner)
        {
            m_Owner = owner;
        }

        internal override void OnVisualTreeAssetChanged(bool inMemoryChange)
        {
#if UNITY_2020_2_OR_NEWER
            if (inMemoryChange && !DefaultEditorWindowBackend.IsGameViewWindowLiveReloadOn())
            {
                return;
            }
#endif

            if (m_Owner.rootVisualElement != null)
            {
                m_Owner.HandleLiveReload();
            }
        }
    }
}
