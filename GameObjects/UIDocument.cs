#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.UIElements
{
    /// <summary>
    /// Defines a Component that connects VisualElements to GameObjects. This makes it
    /// possible to render UI defined in UXML documents in the Game view.
    /// </summary>
    [AddComponentMenu("UI Toolkit/UI Document"), ExecuteAlways, DisallowMultipleComponent]
    public sealed class UIDocument : MonoBehaviour
    {
        internal const string k_RootStyleClassName = "unity-ui-document__root";
        internal const string k_ChildStyleClassName = "unity-ui-document__child";
        internal const string k_ContentContainerChildStyleClassName = "unity-ui-document--content-container__child";

        internal const string k_VisualElementNameSuffix = "-container";

        [SerializeField]
        private PanelSettings m_PanelSettings;

        /// <summary>
        /// Specifies the PanelSettings instance to connect this UIDocument component to.
        /// </summary>
        /// <remarks>
        /// The Panel Settings asset defines the panel that renders UI in the game view. See <see cref="PanelSettings"/>.
        ///
        /// If this UIDocument has a parent UIDocument, it uses the parent's PanelSettings automatically.
        /// </remarks>
        public PanelSettings panelSettings
        {
            get
            {
                return m_PanelSettings;
            }
            set
            {
                if (parentUI == null)
                {
                    if (m_RootVisualElement == null)
                    {
                        // If our root doesn't exist, we're not attached to the panel settings
                        // so nothing else to do but keep the new value.
                        m_PanelSettings = value;
                        return;
                    }

                    if (m_PanelSettings != null)
                    {
                        m_PanelSettings.DetachUIDocument(this);
                    }
                    m_PanelSettings = value;
                    if (m_PanelSettings != null)
                    {
                        m_PanelSettings.AttachAndInsertUIDocumentToVisualTree(this);
                    }
                }
                else
                {
                    // Children only hold the same instance as the parent, they don't attach themselves directly.
                    Assert.AreEqual(parentUI.m_PanelSettings, value);
                    m_PanelSettings = parentUI.m_PanelSettings;
                }

                if (m_ChildrenContent != null)
                {
                    // Guarantee changes to panel settings trickles down the hierarchy.
                    foreach (var child in m_ChildrenContent)
                    {
                        var childContent = child.Value;
                        childContent.panelSettings = m_PanelSettings;
                    }
                }
            }
        }

        /// <summary>
        /// If the GameObject that this UIDocument component is attached to has a parent GameObject, and
        /// that parent GameObject also has a UIDocument component attached to it, this value is set to
        /// the parent GameObject's UIDocument component automatically.
        /// </summary>
        /// <remarks>
        /// If a UIDocument has a parent, you cannot add it directly to a panel. Unity adds it to
        /// the parent's root visual element instead.
        /// </remarks>
        public UIDocument parentUI
        {
            get => m_ParentUI;
            private set => m_ParentUI = value;
        }

        [SerializeField]
        private UIDocument m_ParentUI;


        // If this UIDocument has UIDocument children (1st level only, 2nd level would be the child's
        // children), they're added to this map using the indexes of the game object hierarchy
        // as keys, which are used for sorting.
        private SortedDictionary<UIDocumentHierarchicalIndex, UIDocument> m_ChildrenContent = null;

        // Index used by either a parent or the panel renderer to order content.
        internal UIDocumentHierarchicalIndex m_HierarchicalIndex;

        [SerializeField, Tooltip("The UI Document asset that contains the UI to be shown")]
        private VisualTreeAsset sourceAsset;

        /// <summary>
        /// The <see cref="VisualTreeAsset"/> loaded into the root visual element automatically.
        /// </summary>
        /// <remarks>
        /// If you leave this empty, the root visual element is also empty.
        /// </remarks>
        public VisualTreeAsset visualTreeAsset
        {
            get { return sourceAsset; }
            set
            {
                sourceAsset = value;
                RecreateUI();
            }
        }

        private VisualElement m_RootVisualElement;

        /// <summary>
        /// The root visual element where the UI hierarchy starts.
        /// </summary>
        public VisualElement rootVisualElement
        {
            get { return m_RootVisualElement; }
        }
        private int m_FirstChildInsertIndex;

        private bool m_ContentContainerSet = false;

        // Empty private constructor to avoid public constructor on API listing.
        private UIDocument() {}

        private void Awake()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                // We're in a weird transition state that causes an error with the logic below so let's skip it.
                return;
            }
#endif
            // By default, the UI Content will try to attach itself to a parent somewhere in the hierarchy.
            // This is done to mimic the behaviour we get from UGUI's Canvas/Game Object relationship.
            SetupFromHierarchy();
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                // We're in a weird transition state that causes an error with the logic below so let's skip it.
                return;
            }
#endif
            if (parentUI != null && m_PanelSettings == null)
            {
                // Ensures we have the same PanelSettings set as our parent, as the
                // initialization of the parent may have happened after ours.
                m_PanelSettings = parentUI.m_PanelSettings;
            }

            if (m_RootVisualElement == null)
            {
                RecreateUI();
            }
            else
            {
                AddRootVisualElementToTree();
            }
        }

        /// <summary>
        /// Initializes the full UI after the UXML file loads.
        /// </summary>
        /// <remarks>
        /// This method is called every time the UI is recreated. For example, on initialization.
        /// In the Editor, this method is called when you modify UI assets and Unity must recreate
        /// the UI.
        /// </remarks>
        private void OnCreateUI()
        {
            // Default empty implementation, users extend the class to add UI.
        }

        /// <summary>
        /// An override that provides additional cleanup that might be required to avoid problems such as memory leaks.
        /// This method is called when the UI is about to be recreated.
        /// </summary>
        private void OnAboutToRecreateUI()
        {
            // Default empty implementation.
        }

        /// <summary>
        /// Orders UIDocument components based on the way their GameObjects are ordered in the Hierarchy View.
        /// </summary>
        private void SetupFromHierarchy()
        {
            if (parentUI != null)
            {
                parentUI.RemoveChild(this);
            }
            parentUI = null;
            m_HierarchicalIndex.pathToParent = null;

            // Go up looking for a parent UIDocument, which we'd add ourselves too.
            // If that fails, we'll just add ourselves to the runtime panel through the PanelSettings
            // (assuming one is set, otherwise nothing gets drawn so it's pointless to not be
            // parented by another UIDocument OR have a PanelSettings set).
            Transform t = transform;
            Transform parentTransform = t.parent;
            if (parentTransform != null)
            {
                parentUI = parentTransform.GetComponentInParent<UIDocument>();

                if (parentUI != null)
                {
                    UIDocumentHierarchyUtil.SetHierarchicalIndex(t, parentTransform, parentUI.transform, out m_HierarchicalIndex);
                }
            }

            if (m_HierarchicalIndex.pathToParent == null)
            {
                // Value of parentTransform may be null but that's handled internally and a valid value to pass on.
                UIDocumentHierarchyUtil.SetGlobalIndex(t, parentTransform, out m_HierarchicalIndex);
            }
        }

        private void Reset()
        {
            SetupFromHierarchy();

            if (parentUI != null)
            {
                m_PanelSettings = parentUI.m_PanelSettings;
            }
#if UNITY_EDITOR
            OnValidate();
#endif
        }

        private void AddChild(UIDocument child)
        {
            if (m_ChildrenContent == null)
            {
                m_ChildrenContent = new SortedDictionary<UIDocumentHierarchicalIndex, UIDocument>(UIDocumentHierarchyUtil.indexComparer);
            }

            m_ChildrenContent[child.m_HierarchicalIndex] = child;
        }

        private void AddChildAndInsertContentToVisualTree(UIDocument child)
        {
            AddChild(child);

            if (m_RootVisualElement == null)
            {
                // Parent not yet initialized, when it initializes it'll
                // take care of the children at the same time.
                return;
            }

            int startIndex = UIDocumentHierarchyUtil.FindHierarchicalSortedIndex(m_ChildrenContent, child);

            m_RootVisualElement.Insert(m_FirstChildInsertIndex + startIndex, child.m_RootVisualElement);
        }

        private void RemoveChild(UIDocument child)
        {
            if (m_ChildrenContent == null || child.m_HierarchicalIndex.pathToParent == null)
            {
                return;
            }

            child.m_RootVisualElement?.RemoveFromHierarchy();

            if (m_ChildrenContent.TryGetValue(child.m_HierarchicalIndex, out UIDocument childForIndex) &&
                child == childForIndex)
            {
                m_ChildrenContent.Remove(child.m_HierarchicalIndex);
            }
        }

        private void RemoveChildFromPreviousIndex(UIDocument child, UIDocumentHierarchicalIndex previousHierarchicalIndex)
        {
            if (m_ChildrenContent == null || previousHierarchicalIndex.pathToParent == null)
            {
                return;
            }

            if (m_ChildrenContent.TryGetValue(previousHierarchicalIndex, out UIDocument previousChild) &&
                child == previousChild)
            {
                m_ChildrenContent.Remove(previousHierarchicalIndex);
            }
            m_ChildrenContent[child.m_HierarchicalIndex] = child;
        }

        /// <summary>
        /// Force rebuild the UI from UXML (if one is attached) and of all children (if any).
        /// </summary>
        internal void RecreateUI()
        {
            if (m_RootVisualElement != null)
            {
                m_RootVisualElement.RemoveFromHierarchy();
                m_RootVisualElement = null;
            }

            // Even though the root element is of type VisualElement, we use a TemplateContainer internally
            // because we still want to use it as a TemplateContainer.
            if (sourceAsset != null)
            {
                m_RootVisualElement = sourceAsset.Instantiate();

                // This shouldn't happen but if it does we don't fail silently.
                if (m_RootVisualElement == null)
                {
                    Debug.LogError("The UXML file set for the UIDocument could not be cloned.");
                }
            }

            if (m_RootVisualElement == null)
            {
                // Empty container if no UXML is set or if there was an error with cloning the set UXML.
                m_RootVisualElement = new TemplateContainer() { name = gameObject.name + k_VisualElementNameSuffix };
            }
            else
            {
                m_RootVisualElement.name = gameObject.name + k_VisualElementNameSuffix;
            }
            m_RootVisualElement.pickingMode = PickingMode.Ignore;

            if (isActiveAndEnabled)
            {
                AddRootVisualElementToTree();
            }

            m_ContentContainerSet = m_RootVisualElement.contentContainer != m_RootVisualElement;

            // Save the last VisualElement before we start adding children so we can guarantee
            // the order from the game object hierarchy.
            m_FirstChildInsertIndex = m_RootVisualElement.childCount;

            // Finally, we re-add our known children's element.
            // This makes sure the hierarchy of game objects reflects on the order of VisualElements.
            if (m_ChildrenContent != null)
            {
                foreach (var child in m_ChildrenContent.Values)
                {
                    if (child.isActiveAndEnabled)
                    {
                        if (child.m_RootVisualElement == null)
                        {
                            child.RecreateUI();
                        }
                        else
                        {
                            // Child already ran RecreateUI(), we need to make sure the class list is correct
                            child.AddToCorrectClassList();
                        }

                        m_RootVisualElement.Add(child.m_RootVisualElement);
                    }
                }
            }

            AddToCorrectClassList();
        }

        private void AddToCorrectClassList()
        {
            if (m_RootVisualElement == null)
            {
                return;
            }

            if (parentUI == null)
            {
                // We're not a child of any other UIDocument so stretch to take the full screen.
                m_RootVisualElement.EnableInClassList(k_RootStyleClassName, true);
                m_RootVisualElement.EnableInClassList(k_ChildStyleClassName, false);
                m_RootVisualElement.EnableInClassList(k_ContentContainerChildStyleClassName, false);
            }
            else if (parentUI.m_ContentContainerSet)
            {
                // We're a child of a UIDocument with content container set, we'll show up within it and not take the
                // full screen. The difference of having a content container set or not is that we allow users to
                // establish further styling, but we don't do anything different ourselves.
                m_RootVisualElement.EnableInClassList(k_RootStyleClassName, false);
                m_RootVisualElement.EnableInClassList(k_ChildStyleClassName, false);
                m_RootVisualElement.EnableInClassList(k_ContentContainerChildStyleClassName, true);
            }
            else
            {
                // We're a child of a UIDocument without content container set, we'll show up within it and not take the
                // full screen. The difference of having a content container set or not is that we allow users to
                // establish further styling, but we don't do anything different ourselves.
                m_RootVisualElement.EnableInClassList(k_RootStyleClassName, false);
                m_RootVisualElement.EnableInClassList(k_ChildStyleClassName, true);
                m_RootVisualElement.EnableInClassList(k_ContentContainerChildStyleClassName, false);
            }
        }

        private void AddRootVisualElementToTree()
        {
            // If we do have a parent, it will add us.
            if (parentUI != null)
            {
                parentUI.AddChildAndInsertContentToVisualTree(this);
            }
            else if (m_PanelSettings != null)
            {
                m_PanelSettings.AttachAndInsertUIDocumentToVisualTree(this);
            }
        }

        private void OnDisable()
        {
            m_RootVisualElement?.RemoveFromHierarchy();
        }

        private void OnDestroy()
        {
            if (parentUI != null)
            {
                parentUI.RemoveChild(this);
            }
            else if (m_PanelSettings != null)
            {
                m_PanelSettings.DetachUIDocument(this);
            }
        }

        private void OnTransformChildrenChanged()
        {
#if UNITY_EDITOR
            // In Editor, when not playing, we let a watcher listen for EditorApplication.hierarchyChanged events.
            if (EditorApplication.isPlaying == false)
            {
                return;
            }
#endif
            if (m_ChildrenContent != null)
            {
                // The list may change inside the call to ReactToHierarchyChanged so we need a copy.
                var childrenCopy = new List<UIDocument>(m_ChildrenContent.Values);
                foreach (var child in childrenCopy)
                {
                    child.ReactToHierarchyChanged();
                }
            }
        }

        private void OnTransformParentChanged()
        {
#if UNITY_EDITOR
            // In Editor, when not playing, we let a watcher listen for EditorApplication.hierarchyChanged events.
            if (EditorApplication.isPlaying == false)
            {
                return;
            }
#endif

            ReactToHierarchyChanged();
        }

        internal void ReactToHierarchyChanged()
        {
            if (m_RootVisualElement == null)
            {
                return;
            }

            Transform t = transform;
            Transform parentTransform = t.parent;
            var previousHierarchicalIndex = m_HierarchicalIndex;
            var previousParentContent = parentUI;
            if (parentTransform != null)
            {
                var newParentContent = parentTransform.GetComponentInParent<UIDocument>();
                if (newParentContent != null && newParentContent == parentUI)
                {
                    // If we still have the same parent, but our position may have changed within it, we just need
                    // to re-calculate our child index.
                    UIDocumentHierarchyUtil.SetHierarchicalIndex(t, parentTransform, newParentContent.transform,
                        out m_HierarchicalIndex);

                    if (previousHierarchicalIndex.CompareTo(m_HierarchicalIndex) != 0)
                    {
                        parentUI.RemoveChildFromPreviousIndex(this, previousHierarchicalIndex);

                        if (isActiveAndEnabled)
                        {
                            m_RootVisualElement.RemoveFromHierarchy();
                            AddRootVisualElementToTree();
                        }
                    }

                    return;
                }
            }

            // If we got here, either our parent changed or we're attached to the PanelSetting directly so it's easier
            // to just setup completely again. The path to parent on the index will be updated in that process.

            // First, check if we were attached to a PanelSettings directly before, as that may leave the previous
            // PanelSettings empty (in which case it should be destroyed so we need to detach properly).
            bool wasPreviouslyAttachedToPanelSettings = (parentUI == null && m_PanelSettings != null);

            SetupFromHierarchy();

            if (previousParentContent != parentUI || previousHierarchicalIndex.CompareTo(m_HierarchicalIndex) != 0)
            {
                if (wasPreviouslyAttachedToPanelSettings || (parentUI == null && m_PanelSettings != null))
                {
                    m_PanelSettings.RemoveAttachedUIDocumentFromPreviousIndex(this, previousHierarchicalIndex);
                }

                if (parentUI != null)
                {
                    // Using the property guarantees the change trickles down the hierarchy (if there is one).
                    panelSettings = parentUI.m_PanelSettings;
                }

                if (isActiveAndEnabled)
                {
                    m_RootVisualElement?.RemoveFromHierarchy();
                    AddRootVisualElementToTree();
                }

                AddToCorrectClassList();
            }
        }

        internal void ReactToTopLevelHierarchyChanged()
        {
            // This is meant for UIDocument attached to the PanelSettings only, so if it's not the case don't even bother.
            if (m_PanelSettings == null)
            {
                return;
            }

            var previousHierarchicalIndex = m_HierarchicalIndex;
            SetupFromHierarchy();

            if (previousHierarchicalIndex.CompareTo(m_HierarchicalIndex) != 0)
            {
                m_PanelSettings.RemoveAttachedUIDocumentFromPreviousIndex(this, previousHierarchicalIndex);

                if (parentUI != null)
                {
                    // Using the property guarantees the change trickles down the hierarchy (if there is one).
                    panelSettings = parentUI.m_PanelSettings;
                }

                if (isActiveAndEnabled)
                {
                    m_RootVisualElement?.RemoveFromHierarchy();
                    m_PanelSettings.AttachAndInsertUIDocumentToVisualTree(this);
                }
            }
        }

#if UNITY_EDITOR
        private VisualTreeAsset m_OldUxml = null;
        private PanelSettings m_OldPanelSettings = null;

        private void OnValidate()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            bool shouldRepaint = false;

            if (m_OldUxml != sourceAsset)
            {
                visualTreeAsset = sourceAsset;
                shouldRepaint = true;
            }

            if (m_OldPanelSettings != m_PanelSettings)
            {
                // We'll use the setter as it guarantees the right behavior.
                // It's necessary for the setter that the old value is still in place.
                var tempPanelSettings = m_PanelSettings;
                m_PanelSettings = m_OldPanelSettings;
                panelSettings = tempPanelSettings;

                shouldRepaint = true;
            }

            if (shouldRepaint)
            {
                EditorApplication.QueuePlayerLoopUpdate();
            }

            m_OldUxml = sourceAsset;
            m_OldPanelSettings = m_PanelSettings;
        }

#endif
    }
}
