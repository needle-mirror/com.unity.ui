using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.UIElements.UIR;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace UnityEngine.UIElements
{
    /// <summary>
    /// Options that specify how elements in the panel scale when the screen size changes. See <see cref="PanelSettings.scaleMode"/>.
    /// </summary>
    public enum PanelScaleModes
    {
        /// <summary>
        /// Elements stay the same size, in pixels, regardless of screen size.
        /// </summary>
        ConstantPixelSize,
        /// <summary>
        /// Elements stay the same physical size (displayed size) regardless of screen size and resolution.
        /// </summary>
        ConstantPhysicalSize,
        /// <summary>
        /// Elements get bigger when the screen size increases, and smaller when it decreases.
        /// </summary>
        ScaleWithScreenSize
    }

    /// <summary>
    /// Options that specify how to scale the panel area when the aspect ratio of the current screen resolution
    /// does not match the reference resolution. See <see cref="PanelSettings.screenMatchMode"/>.
    /// </summary>
    public enum PanelScreenMatchModes
    {
        /// <summary>
        /// Scales the panel area using width, height, or a mix of the two as a reference.
        /// </summary>
        MatchWidthOrHeight,
        /// <summary>
        /// Crops the panel area horizontally or vertically so the panel size never exceeds
        /// the reference resolution.
        /// </summary>
        Shrink,
        /// <summary>
        /// Expand the panel area horizontally or vertically so the panel size is never
        /// smaller than the reference resolution.
        /// </summary>
        Expand
    }

    /// <summary>
    /// Defines a Panel Settings asset that instantiates a panel at runtime. The panel makes it possible for Unity to display
    /// UXML-file based UI in the Game view.
    /// </summary>
    [CreateAssetMenu(fileName = "PanelSettings", menuName = "UI Toolkit/Panel Settings Asset")]
    public class PanelSettings : ScriptableObject
    {
        private const int k_DefaultSortingOrder = 0;

        private const float k_DefaultScaleValue = 1.0f;

        internal const string k_DefaultStyleSheetPath =
            "Packages/com.unity.ui/PackageResources/StyleSheets/Generated/Default.uss.asset";

        [SerializeField]
        private StyleSheet themeUss;

        /// <summary>
        /// Specifies a style sheet that Unity applies to every UI Document attached to the panel.
        /// </summary>
        /// <remarks>
        /// By default this is the main Unity style sheet, which contains default styles for Unity-supplied
        /// elements such as buttons, sliders, and text fields.
        /// </remarks>
        public StyleSheet themeStyleSheet
        {
            get { return themeUss; }
            set
            {
                themeUss = value;
                ApplyThemeStyleSheet();
            }
        }

        [SerializeField]
        private RenderTexture m_TargetTexture;

        /// <summary>
        /// Specifies a Render Texture to render the panel's UI on.
        /// </summary>
        /// <remarks>
        /// This is useful when you want to display UI on 3D geometry in the Scene.
        /// For an example of UI displayed on 3D objects via renderTextures, see the UI Toolkit samples
        /// (menu: <b>Window > UI Toolkit > Examples > Rendering > RenderTexture (Runtime)</b>).
        /// </remarks>
        public RenderTexture targetTexture
        {
            get => m_TargetTexture;
            set
            {
                m_TargetTexture = value;
                if (m_RuntimePanel != null)
                {
                    m_RuntimePanel.targetTexture = m_TargetTexture;
                }
            }
        }

        [SerializeField]
        private PanelScaleModes m_ScaleMode = PanelScaleModes.ConstantPixelSize;

        /// <summary>
        /// Determines how elements in the panel scale when the screen size changes.
        /// </summary>
        public PanelScaleModes scaleMode
        {
            get => m_ScaleMode;
            set => m_ScaleMode = value;
        }

        [SerializeField]
        private float m_Scale = k_DefaultScaleValue;

        /// <summary>
        /// A uniform scaling factor that Unity applies to elements in the panel before
        /// the panel transform.
        /// </summary>
        /// <remarks>
        /// This value must be greater than 0.
        /// </remarks>
        public float scale
        {
            get => m_Scale;
            set => m_Scale = value;
        }

        #region Scaling parameters

        [SerializeField]
        private float m_ReferenceDpi = 96;
        [SerializeField]
        private float m_FallbackDpi = 96;

        /// <summary>
        /// The DPI that the UI is designed for.
        /// </summary>
        /// <remarks>
        /// When <see cref="scaleMode"/> is set to <c>ConstantPhysicalSize</c>, Unity compares
        /// this value to the actual screen DPI, and scales the UI accordingly in the Game view.
        ///
        /// If Unity cannot determine the screen DPI, it uses the <see cref="fallbackDpi"/> instead.
        /// </remarks>
        public float referenceDpi
        {
            get => m_ReferenceDpi;
            set => m_ReferenceDpi = value;
        }

        /// <summary>
        /// The DPI value that Unity uses when it cannot determine the screen DPI.
        /// </summary>
        public float fallbackDpi
        {
            get => m_FallbackDpi;
            set => m_FallbackDpi = value;
        }

        [SerializeField]
        private Vector2Int m_ReferenceResolution = new Vector2Int(1200, 800);

        /// <summary>
        /// The resolution the UI is designed for.
        /// <remarks>
        /// If the screen resolution is larger than the reference resolution, unity scales
        /// the UI up in the Game view. If it's smaller, Unity scales the UI down.
        /// Unity scales the UI according to the <see cref="screenMatchMode">.
        /// </summary>
        public Vector2Int referenceResolution
        {
            get => m_ReferenceResolution;
            set => m_ReferenceResolution = value;
        }

        [SerializeField]
        private PanelScreenMatchModes m_ScreenMatchMode = PanelScreenMatchModes.MatchWidthOrHeight;

        /// <summary>
        /// Specifies how to scale the panel area when the aspect ratio of the current resolution
        /// does not match the reference resolution.
        /// </summary>
        public PanelScreenMatchModes screenMatchMode
        {
            get => m_ScreenMatchMode;
            set => m_ScreenMatchMode = value;
        }

        [SerializeField]
        [Range(0f, 1f)]
        private float m_Match = 0.0f;

        /// <summary>
        /// Determines whether Unity uses width, height, or a mix of the two as a reference when it scales the panel area.
        /// </summary>
        public float match
        {
            get => m_Match;
            set => m_Match = value;
        }

        #endregion

        [SerializeField]
        private float m_SortingOrder = k_DefaultSortingOrder;

        /// <summary>
        /// When the Scene uses more than one panel, this value determines where this panel appears in the sorting
        /// order relative to other panels.
        /// </summary>
        /// <remarks>
        /// Unity renders panels with a higher sorting order value on top of panels with a lower value.
        /// </remarks>
        public float sortingOrder
        {
            get => m_SortingOrder;
            set
            {
                m_SortingOrder = value;
                if (m_RuntimePanel != null)
                {
                    m_RuntimePanel.sortingPriority = m_SortingOrder;
                }
            }
        }

        /// <summary>
        /// NEVER USE THIS DIRECTLY! Use m_Panel instead to guarantee correct initialization.
        /// </summary>
        private BaseRuntimePanel m_RuntimePanel;

        /// <summary>
        /// Internal, typed access to the Panel used to draw UI of type Player.
        /// </summary>
        private BaseRuntimePanel m_Panel
        {
            get
            {
                if (m_RuntimePanel == null)
                {
                    m_RuntimePanel = CreateRelatedRuntimePanel();
                    m_RuntimePanel.sortingPriority = m_SortingOrder;
                    var root = m_RuntimePanel.visualTree;
                    root.name = name;

                    ApplyThemeStyleSheet(root);

                    if (m_TargetTexture != null)
                    {
                        m_RuntimePanel.targetTexture = m_TargetTexture;
                    }

                    if (m_AssignedScreenToPanel != null)
                    {
                        SetScreenToPanelSpaceFunction(m_AssignedScreenToPanel);
                    }
                }

                return m_RuntimePanel;
            }
            set
            {
                // Only acceptable value to set is null, in which case we dispose the panel.
                Assert.IsNull(value);

                if (m_RuntimePanel != null)
                {
                    DisposeRelatedPanel();
                    m_RuntimePanel = null;
                }
            }
        }

        /// <summary>
        /// The panel created with the configuration held by this object.
        /// </summary>
        internal IPanel panel => m_Panel;

        /// <summary>
        /// The top level visual element.
        /// </summary>
        internal VisualElement visualTree => m_Panel.visualTree;

        // UIDocument are added to the visual tree in order of their appearance in the Hierarchy View.
        private SortedDictionary<UIDocumentHierarchicalIndex, UIDocument> m_AttachedUIDocuments = null;

        // References to shaders so they don't get stripped.
        [SerializeField]
        private Shader m_AtlasBlitShader;
        [SerializeField]
        private Shader m_RuntimeShader;
        [SerializeField]
        private Shader m_RuntimeWorldShader;

        private Rect m_TargetRect;
        private float m_ResolvedScale; // panel scaling factor (pixels <-> points)

        // Empty private constructor to avoid public constructor on API listing.
        private PanelSettings() {}

        private void Reset()
        {
#if UNITY_EDITOR
            // We assume users will want their UIDocument to look as closely as possible to what they look like in the UIBuilder.
            // This is no guarantee, but it's the best we can do at the moment.
            referenceDpi = Screen.dpi;
            scaleMode = PanelScaleModes.ConstantPhysicalSize;

            themeStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(k_DefaultStyleSheetPath);

            m_AtlasBlitShader = m_RuntimeShader = m_RuntimeWorldShader = null;
            InitializeShaders();
#endif
        }

        private void OnEnable()
        {
            InitializeShaders();
        }

        private void OnDisable()
        {
            m_Panel = null;
        }

        private void ApplyThemeStyleSheet(VisualElement root = null)
        {
            if (m_RuntimePanel == null)
            {
                return;
            }

            if (root == null)
            {
                root = visualTree;
            }

            if (themeUss != null)
            {
                themeUss.isUnityStyleSheet = true;
                root?.styleSheets.Add(themeUss);
            }
        }

        private BaseRuntimePanel CreateRelatedRuntimePanel()
        {
            var newPanel = (RuntimePanel)UIElementsRuntimeUtility.FindOrCreateRuntimePanel(this, RuntimePanel.Create);
            newPanel.visualTree.pseudoStates |= PseudoStates.Root;
#if UNITY_EDITOR
            UIElementsEditorRuntimeUtility.CreateRuntimePanelDebug(newPanel);
#endif
            return newPanel;
        }

        private void DisposeRelatedPanel()
        {
            UIElementsRuntimeUtility.DisposeRuntimePanel(this);
        }

        void InitializeShaders()
        {
            if (m_AtlasBlitShader == null)
            {
                m_AtlasBlitShader = Shader.Find(Shaders.k_AtlasBlit);
            }
            if (m_RuntimeShader == null)
            {
                m_RuntimeShader = Shader.Find(Shaders.k_Runtime);
            }
            if (m_RuntimeWorldShader == null)
            {
                m_RuntimeWorldShader = Shader.Find(Shaders.k_RuntimeWorld);
            }
            if (m_RuntimePanel != null)
            {
                m_RuntimePanel.targetTexture = m_TargetTexture;
            }
        }

        internal void ApplyPanelSettings()
        {
            Rect oldTargetRect = m_TargetRect;
            float oldResolvedScaling = m_ResolvedScale;
            m_TargetRect = GetDisplayRect(); // Expensive to evaluate, so cache
            m_ResolvedScale = ResolveScale(m_TargetRect, Screen.dpi);

            if (visualTree.style.width.value == 0 || // TODO is this check valid? This prevents having to resize the game view!
                m_ResolvedScale != oldResolvedScaling ||
                m_TargetRect.width != oldTargetRect.width ||
                m_TargetRect.height != oldTargetRect.height)
            {
                m_Panel.scale = m_ResolvedScale == 0.0f ? 0.0f : 1.0f / m_ResolvedScale;
                visualTree.style.left = 0;
                visualTree.style.top = 0;
                visualTree.style.width = m_TargetRect.width * m_ResolvedScale;
                visualTree.style.height = m_TargetRect.height * m_ResolvedScale;
            }
            m_Panel.targetTexture = targetTexture;
            m_Panel.drawToCameras = false; //we don`t support WorldSpace rendering just yet
        }

        /// <summary>
        /// Sets the function that handles the transformation from screen space to panel space. For overlay panels,
        /// this function returns the input value.
        /// </summary>
        ///
        /// <param name="screentoPanelSpaceFunction">The translation function. Set to null to revert to the default behavior.</param>
        /// <remarks>
        /// If the panel's targetTexture is applied to 3D objects, one approach is to use a function that raycasts against
        /// MeshColliders in the Scene. The function can first check whether the GameObject that the ray hits has a
        /// MeshRenderer with a shader that uses this panel's target texture. It can then return the transformed
        /// <c>RaycastHit.textureCoord</c> in the texture's pixel space.
        ///
        /// For an example of UI displayed on 3D objects via renderTextures, see the UI Toolkit samples
        /// (menu: <b>Window > UI Toolkit > Examples > Rendering > RenderTexture (Runtime)</b>).
        /// </remarks>
        public void SetScreenToPanelSpaceFunction(Func<Vector2, Vector2> screentoPanelSpaceFunction)
        {
            m_AssignedScreenToPanel = screentoPanelSpaceFunction;
            m_Panel.screenToPanelSpace = m_AssignedScreenToPanel;
        }

        private Func<Vector2, Vector2> m_AssignedScreenToPanel;

        internal float ResolveScale(Rect targetRect, float screenDpi)
        {
            // Calculate scaling
            float resolvedScale = 1.0f;
            switch (scaleMode)
            {
                case PanelScaleModes.ConstantPixelSize:
                    break;
                case PanelScaleModes.ConstantPhysicalSize:
                {
                    var dpi = screenDpi == 0.0f ? fallbackDpi : screenDpi;
                    if (dpi != 0.0f)
                        resolvedScale = referenceDpi / dpi;
                }
                break;
                case PanelScaleModes.ScaleWithScreenSize:
                    if (referenceResolution.x * referenceResolution.y != 0)
                    {
                        var refSize = (Vector2)referenceResolution;
                        var sizeRatio = new Vector2(targetRect.width / refSize.x, targetRect.height / refSize.y);

                        var denominator = 0.0f;
                        switch (screenMatchMode)
                        {
                            case PanelScreenMatchModes.Expand:
                                denominator = Mathf.Min(sizeRatio.x, sizeRatio.y);
                                break;
                            case PanelScreenMatchModes.Shrink:
                                denominator = Mathf.Max(sizeRatio.x, sizeRatio.y);
                                break;
                            default: // PanelScreenMatchModes.MatchWidthOrHeight:
                                var widthHeightRatio = Mathf.Clamp01(match);
                                denominator = Mathf.Lerp(sizeRatio.x, sizeRatio.y, widthHeightRatio);
                                break;
                        }
                        if (denominator != 0.0f)
                            resolvedScale = 1.0f / denominator;
                    }
                    break;
            }

            if (scale > 0.0f)
            {
                resolvedScale /= scale;
            }
            else
            {
                resolvedScale = 0.0f;
            }

            return resolvedScale;
        }

        internal Rect GetDisplayRect()
        {
            if (m_TargetTexture != null)
            {
                // Overlay to texture.
                return new Rect(0, 0, m_TargetTexture.width, m_TargetTexture.height); // TODO: Support sub-rects
            }

            // Overlay.

            // We may be drawing in a render texture (graphics tests do this),
            // get the active RT size in this case.
            var rt = RenderTexture.active;
            if (rt != null)
                return new Rect(0, 0, rt.width, rt.height);

            return new Rect(0, 0, Screen.width, Screen.height);
        }

        private void AttachUIDocument(UIDocument uiDocument)
        {
            if (uiDocument == null)
            {
                return;
            }

            if (m_AttachedUIDocuments == null)
            {
                m_AttachedUIDocuments = new SortedDictionary<UIDocumentHierarchicalIndex, UIDocument>(UIDocumentHierarchyUtil.indexComparer);
            }

            m_AttachedUIDocuments[uiDocument.m_HierarchicalIndex] = uiDocument;
        }

        internal void AttachAndInsertUIDocumentToVisualTree(UIDocument uiDocument)
        {
            AttachUIDocument(uiDocument);

            // Shouldn't be null but if it is there's nothing we can do about it.
            if (visualTree == null)
            {
                return;
            }

            if (uiDocument.rootVisualElement != null)
            {
                int childIndex = UIDocumentHierarchyUtil.FindHierarchicalSortedIndex(m_AttachedUIDocuments, uiDocument);
                visualTree.Insert(childIndex, uiDocument.rootVisualElement);
            }
        }

        internal void DetachUIDocument(UIDocument uiDocument)
        {
            if (m_AttachedUIDocuments == null || uiDocument == null || uiDocument.m_HierarchicalIndex.pathToParent == null)
            {
                return;
            }

            uiDocument.rootVisualElement?.RemoveFromHierarchy();

            m_AttachedUIDocuments.Remove(uiDocument.m_HierarchicalIndex);

            if (m_AttachedUIDocuments.Count == 0)
            {
                // No references to the panel, we can dispose it and it'll be recreated if it's used again.
                m_Panel = null;
            }
        }

        internal void RemoveAttachedUIDocumentFromPreviousIndex(UIDocument uiDocument, UIDocumentHierarchicalIndex previousIndex)
        {
            if (m_AttachedUIDocuments == null || previousIndex.pathToParent == null)
            {
                return;
            }

            if (m_AttachedUIDocuments.TryGetValue(previousIndex, out UIDocument previousChild) &&
                uiDocument == previousChild)
            {
                m_AttachedUIDocuments.Remove(previousIndex);
            }

            if (m_AttachedUIDocuments.Count == 0)
            {
                // No references to the panel, we can dispose it and it'll be recreated if it's used again.
                m_Panel = null;
            }
        }

        /// <summary>
        /// In Play mode, this method makes it possible to present root-level UIDocument components
        /// in the order in which they appear in the GameObject hierarchy.
        /// </summary>
        /// <remarks>
        /// When you move GameObjects in the hierarchy, their index values become outdated. This method
        /// updates the index values, and ensures that UI components are presented in the correct order.
        ///
        /// You must call this method when you move UIDocument components while in Play mode. You do not
        /// need to call it in Edit mode because Editor scripts already guarantee that components appear
        /// in the correct order.
        /// </remarks>
        public void OrderByHierarchy()
        {
            if (m_AttachedUIDocuments == null)
            {
                return;
            }

            // Asking each UIDocument to re-position themselves will alter the attached list so we need a copy
            // to safely iterate on it.
            var attachedUIDocumentsCopy = new List<UIDocument>(m_AttachedUIDocuments.Values);

            foreach (var uiDocument in attachedUIDocumentsCopy)
            {
                uiDocument.ReactToTopLevelHierarchyChanged();
            }
        }

#if UNITY_EDITOR
        private StyleSheet m_OldThemeUss;
        private RenderTexture m_OldTargetTexture;
        private float m_OldSortingOrder;
        private bool m_IsLoaded = false;

        private void OnValidate()
        {
            if (m_Scale < 0.0f || m_ScaleMode != PanelScaleModes.ConstantPixelSize)
            {
                m_Scale = k_DefaultScaleValue;
            }

            if (m_IsLoaded)
            {
                if (m_OldThemeUss != themeUss)
                {
                    var root = visualTree;
                    if (root != null)
                    {
                        if (m_OldThemeUss != null)
                        {
                            root.styleSheets.Remove(m_OldThemeUss);
                        }

                        ApplyThemeStyleSheet(root);
                    }
                }

                if (m_OldTargetTexture != m_TargetTexture)
                {
                    targetTexture = m_TargetTexture;
                }

                if (m_OldSortingOrder != m_SortingOrder)
                {
                    sortingOrder = m_SortingOrder;
                }
            }
            else
            {
                m_IsLoaded = true;
            }

            m_OldThemeUss = themeUss;
            m_OldTargetTexture = m_TargetTexture;
            m_OldSortingOrder = m_SortingOrder;
        }

#endif
    }
}
