using System;
using System.Collections.Generic;
using Unity.Profiling;

namespace UnityEngine.UIElements
{
#if UNITY_EDITOR
    internal class VisualTreeAssetChangeTrackerUpdater : BaseVisualTreeUpdater
    {
        private static readonly string s_Description = "Update UI Assets (Editor)";
        private static readonly ProfilerMarker s_ProfilerMarker = new ProfilerMarker(s_Description);
        public override ProfilerMarker profilerMarker => s_ProfilerMarker;

        // HashSet is for faster removals when elements go away from a panel
        private HashSet<TextElement> m_TextElements = new HashSet<TextElement>();

        private bool m_HasAnyTextAssetChanged;

        public VisualTreeAssetChangeTrackerUpdater()
        {
            TextDelegates.OnTextAssetChange += OnTextAssetChange;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TextDelegates.OnTextAssetChange -= OnTextAssetChange;

                m_TextElements.Clear();
            }
        }

        void OnTextAssetChange(Object asset)
        {
            // Note: due to Rich Text Tags, it's very difficult to predict if a text asset is actually in use
            // Therefore we will invalidate ALL text objects but only for panels which have Live Reload enabled
            if (panel.enableAssetReload)
            {
                m_HasAnyTextAssetChanged = true;
                panel.RequestUpdateAfterExternalEvent(this);
            }
        }

        public override void OnVersionChanged(VisualElement ve, VersionChangeType versionChangeType)
        {
            // Nothing to be done, we only need the Update
        }

        internal static Func<bool> IsEditorPlaying;

        private int m_PreviousInMemoryAssetsVersion = 0;

        private const int kMinUpdateDelayMs = 1000; // TODO this should probably be a setting at some point
        private long m_LastUpdateTimeMs = 0;

        // These are 2 data structures that hold some duplication to allow faster access to information as needed.
        // We guarantee the keys on m_TrackerToVisualElementMap are exactly the same entries from m_LiveReloadVisualTreeAssetTrackers
        // to avoid having to get all keys from the dictionary, as we use the list of trackers for the Update() method.
        private HashSet<ILiveReloadAssetTracker<VisualTreeAsset>> m_LiveReloadVisualTreeAssetTrackers = new HashSet<ILiveReloadAssetTracker<VisualTreeAsset>>();
        private Dictionary<ILiveReloadAssetTracker<VisualTreeAsset>, HashSet<VisualElement>> m_TrackerToVisualElementMap = new Dictionary<ILiveReloadAssetTracker<VisualTreeAsset>, HashSet<VisualElement>>();

        internal ILiveReloadAssetTracker<StyleSheet> m_LiveReloadStyleSheetAssetTracker;

        internal void StartVisualTreeAssetTracking(ILiveReloadAssetTracker<VisualTreeAsset> tracker,
            VisualElement visualElementUsingAsset)
        {
            tracker.StartTrackingAsset(visualElementUsingAsset.m_VisualTreeAssetSource);

            if (m_TrackerToVisualElementMap.TryGetValue(tracker, out var visualElements))
            {
                visualElements.Add(visualElementUsingAsset);
            }
            else
            {
                visualElements = new HashSet<VisualElement>();
                m_TrackerToVisualElementMap[tracker] = visualElements;
                m_LiveReloadVisualTreeAssetTrackers.Add(tracker);
            }
        }

        internal void StopVisualTreeAssetTracking(VisualElement visualElementUsingAsset)
        {
            ILiveReloadAssetTracker<VisualTreeAsset> foundTracker = null;

            foreach (var trackerMapEntry in m_TrackerToVisualElementMap)
            {
                var tracker = trackerMapEntry.Key;

                if (!tracker.IsTrackingAsset(visualElementUsingAsset.m_VisualTreeAssetSource))
                {
                    continue;
                }

                var visualElements = trackerMapEntry.Value;

                foreach (var visualElement in visualElements)
                {
                    if (visualElement == visualElementUsingAsset)
                    {
                        foundTracker = tracker;
                        break;
                    }
                }

                if (foundTracker != null)
                {
                    visualElements.Remove(visualElementUsingAsset);
                    break;
                }
            }

            foundTracker?.StopTrackingAsset(visualElementUsingAsset.m_VisualTreeAssetSource);
        }

        internal HashSet<ILiveReloadAssetTracker<VisualTreeAsset>> GetVisualTreeAssetTrackersListCopy()
        {
            // Return a copy, we don't want it to be modified as it would break the logic in this updater.
            // This is being called by the AssetPostprocessor, so not all the time and we can take the overhead in
            // this case in exchange for safety.
            return new HashSet<ILiveReloadAssetTracker<VisualTreeAsset>>(m_LiveReloadVisualTreeAssetTrackers);
        }

        public override void Update()
        {
            UpdateTextElements();

            // Early out: no tracker found for panel.
            if (m_LiveReloadVisualTreeAssetTrackers.Count == 0 && m_LiveReloadStyleSheetAssetTracker == null)
            {
                return;
            }

            if (IsEditorPlaying.Invoke())
            {
                long currentTimeMs = Panel.TimeSinceStartupMs();

                if (currentTimeMs < m_LastUpdateTimeMs + kMinUpdateDelayMs)
                {
                    return;
                }

                m_LastUpdateTimeMs = currentTimeMs;
            }

            // There's no need to reload anything if there are no changes.
            if (m_PreviousInMemoryAssetsVersion == UIElementsUtility.m_InMemoryAssetsVersion)
            {
                return;
            }

            // This value is updated by the UI Builder whenever an asset is changed.
            // We update here to prevent unnecessary checking of asset changes.
            m_PreviousInMemoryAssetsVersion = UIElementsUtility.m_InMemoryAssetsVersion;

            foreach (var tracker in m_LiveReloadVisualTreeAssetTrackers)
            {
                tracker.CheckTrackedAssetsDirty();
            }

            if (m_LiveReloadStyleSheetAssetTracker != null && m_LiveReloadStyleSheetAssetTracker.CheckTrackedAssetsDirty())
            {
                panel.DirtyStyleSheets();
            }
        }

        public void RegisterTextElement(TextElement element)
        {
            if (panel.enableAssetReload)
            {
                m_TextElements.Add(element);
            }
        }

        public void UnregisterTextElement(TextElement element)
        {
            if (panel.enableAssetReload)
            {
                m_TextElements.Remove(element);
            }
        }

        private void UpdateTextElements()
        {
            if (!m_HasAnyTextAssetChanged)
                return;

            try
            {
                foreach (var textElement in m_TextElements)
                {
                    textElement.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
                }
            }
            finally
            {
                m_HasAnyTextAssetChanged = false;
            }
        }
    }
#endif
}
