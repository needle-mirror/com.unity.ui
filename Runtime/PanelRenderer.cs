using UnityEngine.Profiling;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.UIElements
{
    /// <summary>
    /// Component to render a UXML file and stylesheets in the game view.
    /// </summary>
    [AddComponentMenu("UI Toolkit/Panel Renderer"), ExecuteInEditMode, Obsolete("PanelRenderer will be removed in a future release. Use UIDocument with PanelSettings instead.")]
    public class PanelRenderer : MonoBehaviour
    {
        /// <summary>
        /// The actual panel holder.
        /// </summary>
        private PanelSettings m_PanelSettings;

        /// <summary>
        /// The UXML file to render
        /// </summary>
        public VisualTreeAsset uxml;

#if UNITY_EDITOR
        private string m_UxmlAssetPath;
#endif

        /// <summary>
        /// The main style sheet file to give styles to Unity provided elements
        /// </summary>
        [FormerlySerializedAs("unityStyleSheet"), SerializeField]
        private StyleSheet m_ThemeStyleSheet;

        public StyleSheet themeStyleSheet
        {
            get => m_ThemeStyleSheet;
            set
            {
                m_ThemeStyleSheet = value;
                m_PanelSettings.themeStyleSheet = m_ThemeStyleSheet;
            }
        }

        /// <summary>
        /// The top level element.
        /// </summary>
        public VisualElement visualTree { get; private set; }

        internal BaseRuntimePanel m_Panel;

        /// <summary>
        /// The panel holding the visual tree instantiated from the UXML file.
        /// </summary>
        public IPanel panel => m_PanelSettings.panel;

        /// <summary>
        /// An optional texture onto which the panel should be rendered.
        /// </summary>
        [FormerlySerializedAs("targetTexture"), SerializeField]
        private RenderTexture m_TargetTexture;

        public RenderTexture targetTexture
        {
            get => m_TargetTexture;
            set
            {
                m_TargetTexture = value;
                m_PanelSettings.targetTexture = m_TargetTexture;
            }
        }

        /// <summary>
        /// Determines how elements in the Panel are scaled.
        /// </summary>
        [FormerlySerializedAs("scaleMode"), SerializeField]
        private PanelScaleModes m_ScaleMode = PanelScaleModes.ConstantPixelSize;

        public PanelScaleModes scaleMode
        {
            get => m_ScaleMode;
            set
            {
                m_ScaleMode = value;
                m_PanelSettings.scaleMode = m_ScaleMode;
            }
        }

        /// <summary>
        /// A uniform scale that prepends the panel transform
        /// </summary>
        [FormerlySerializedAs("scale"), SerializeField]
        private float m_Scale = 1.0f;

        public float scale
        {
            get => m_Scale;
            set
            {
                m_Scale = value;
                m_PanelSettings.scale = m_Scale;
            }
        }

        #region Scaling parameters


        [FormerlySerializedAs("referenceDpi"), SerializeField]
        private float m_ReferenceDpi = 96;

        public float referenceDpi
        {
            get => m_ReferenceDpi;
            set
            {
                m_ReferenceDpi = value;
                m_PanelSettings.referenceDpi = referenceDpi;
            }
        }

        [FormerlySerializedAs("fallbackDpi"), SerializeField]
        private float m_FallbackDpi = 96;

        public float fallbackDpi
        {
            get => m_FallbackDpi;
            set
            {
                m_FallbackDpi = value;
                m_PanelSettings.fallbackDpi = m_FallbackDpi;
            }
        }

        /// <summary>
        /// The resolution the UI is designed for. If the screen resolution is larger, the UI will be scaled up,
        /// and if it�s smaller, the UI will be scaled down.
        /// </summary>
        [FormerlySerializedAs("referenceResolution"), SerializeField]
        private Vector2Int m_ReferenceResolution = new Vector2Int(1200, 800);

        public Vector2Int referenceResolution
        {
            get => m_ReferenceResolution;
            set
            {
                m_ReferenceResolution = value;
                m_PanelSettings.referenceResolution = m_ReferenceResolution;
            }
        }

        /// <summary>
        /// A mode used to scale the Panel area if the aspect ratio of the current resolution
        /// does not fit the reference resolution.
        /// </summary>
        [FormerlySerializedAs("screenMatchMode"), SerializeField]
        private PanelScreenMatchModes m_ScreenMatchMode = PanelScreenMatchModes.MatchWidthOrHeight;

        public PanelScreenMatchModes screenMatchMode
        {
            get => m_ScreenMatchMode;
            set
            {
                m_ScreenMatchMode = value;
                m_PanelSettings.screenMatchMode = m_ScreenMatchMode;
            }
        }

        /// <summary>
        /// Determines if the scaling is using the width or height as reference, or a mix in between.
        /// </summary>
        [FormerlySerializedAs("match"), SerializeField]
        private float m_Match = 0.0f;

        public float match
        {
            get => m_Match;
            set
            {
                m_Match = value;
                m_PanelSettings.match = m_Match;
            }
        }

        #endregion

        /// <summary>
        /// Functions called after UXML document has been loaded.
        /// </summary>
        public Func<IEnumerable<Object>> postUxmlReload { get; set; }

        CustomSampler m_InitSampler;
        CustomSampler initSampler
        {
            get
            {
                if (m_InitSampler == null)
                    m_InitSampler = CustomSampler.Create("UIElements." + gameObject.name + ".Initialize");

                return m_InitSampler;
            }
        }

        CustomSampler m_UpdateSampler;
        CustomSampler updateSampler
        {
            get
            {
                if (m_UpdateSampler == null)
                    m_UpdateSampler = CustomSampler.Create("UIElements." + gameObject.name + ".Update");

                return m_UpdateSampler;
            }
        }


#if UNITY_EDITOR
        private StyleSheet m_OldThemeStyleSheet;
        private bool m_IsLoaded = false;

        /// <summary>
        /// Implementation of OnValidate().
        /// </summary>
        protected void OnValidate()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            Validate();

            if (m_PanelSettings == null)
            {
                return;
            }

            if (m_IsLoaded)
            {
                // Guarantee all values are updated on the PanelSettings instance.
                m_PanelSettings.fallbackDpi = fallbackDpi;
                m_PanelSettings.match = match;
                m_PanelSettings.referenceDpi = referenceDpi;
                m_PanelSettings.referenceResolution = referenceResolution;
                m_PanelSettings.scale = scale;
                m_PanelSettings.scaleMode = scaleMode;
                m_PanelSettings.screenMatchMode = screenMatchMode;
                m_PanelSettings.targetTexture = targetTexture;

                // This operation is a bit heavier so we avoid doing it if we don't have to.
                if (m_OldThemeStyleSheet != themeStyleSheet)
                {
                    m_PanelSettings.themeStyleSheet = themeStyleSheet;
                }
            }

            m_IsLoaded = true;
            m_OldThemeStyleSheet = themeStyleSheet;
        }

#endif
        protected virtual void Validate()
        {
#if UNITY_EDITOR
            m_UxmlAssetPath = uxml ? AssetDatabase.GetAssetPath(uxml) : null;
#endif
        }

        /// <summary>
        /// Implementation of Reset().
        /// </summary>
        protected void Reset()
        {
#if UNITY_EDITOR
            themeStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PanelSettings.k_DefaultStyleSheetPath);
#endif
        }

        void Initialize()
        {
            if (m_PanelSettings == null)
            {
                initSampler.Begin();

                m_PanelSettings = ScriptableObject.CreateInstance<PanelSettings>();
                m_PanelSettings.themeStyleSheet = m_ThemeStyleSheet;
                m_PanelSettings.targetTexture = targetTexture;
                m_PanelSettings.scaleMode = scaleMode;
                m_PanelSettings.scale = m_Scale;
                m_PanelSettings.referenceDpi = m_ReferenceDpi;
                m_PanelSettings.fallbackDpi = m_FallbackDpi;
                m_PanelSettings.referenceResolution = m_ReferenceResolution;
                m_PanelSettings.screenMatchMode = m_ScreenMatchMode;
                m_PanelSettings.match = m_Match;

                var root = m_PanelSettings.visualTree;
                root.name = gameObject.name;

                visualTree = new TemplateContainer { name = "runtime-panel-container" };
                visualTree.style.overflow = Overflow.Hidden;
                visualTree.StretchToParentSize();

                root.Add(visualTree);

                initSampler.End();
            }

            Validate();

            RecreateUIFromUxml();
        }

        void Cleanup()
        {
            if (m_PanelSettings != null)
            {
                DestroyImmediate(m_PanelSettings);
            }

            m_PanelSettings = null;
        }

        /// <summary>
        /// Implementation of OnEnable()
        /// </summary>
        protected void OnEnable()
        {
            // Sometimes Awake is not called. Ensure we have called Initialize().
            Initialize();
        }

        internal float ResolveScale(Rect targetRect, float screenDpi)
        {
            // TODO This is used by tests which need to be updated!
            return m_PanelSettings.ResolveScale(targetRect, screenDpi);
        }

        /// <summary>
        /// Implementation of OnDisable().
        /// </summary>
        protected void OnDisable()
        {
            // We need to Cleanup() here otherwise panels leak when entering playmode.
            Cleanup();
        }

        /// <summary>
        /// Implementation of OnDestroy().
        /// </summary>
        protected void OnDestroy()
        {
            Cleanup();
        }

        /// <summary>
        /// Force rebuild the UI from UXML (if one is attached).
        /// </summary>
        public void RecreateUIFromUxml()
        {
            if (uxml == null || visualTree == null)
                return;

            visualTree.Clear();
            visualTree.styleSheets.Clear();

            uxml.CloneTree(visualTree);

            postUxmlReload?.Invoke();
        }

#if UNITY_EDITOR
        private void ForceGameViewRepaint()
        {
            EditorApplication.QueuePlayerLoopUpdate();
        }

#endif
    }
}
