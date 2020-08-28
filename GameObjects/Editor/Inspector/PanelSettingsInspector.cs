using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements.Inspector
{
    [CustomEditor(typeof(PanelSettings))]
    internal class PanelSettingsInspector : Editor
    {
#if UIE_PACKAGE
        const string k_DefaultStyleSheetPath = "Packages/com.unity.ui/PackageResources/StyleSheets/Inspector/PanelSettingsInspector.uss";
        const string k_InspectorVisualTreeAssetPath = "Packages/com.unity.ui/PackageResources/UXML/Inspector/PanelSettingsInspector.uxml";
#else
        const string k_DefaultStyleSheetPath = "UIPackageResources/StyleSheets/Inspector/PanelSettingsInspector.uss";
        const string k_InspectorVisualTreeAssetPath = "UIPackageResources/UXML/Inspector/PanelSettingsInspector.uxml";
#endif

        private static StyleSheet k_DefaultStyleSheet = null;

        private VisualElement m_RootVisualElement;

        private VisualTreeAsset m_InspectorUxml;

        private ObjectField m_ThemeStyleSheetField;
        private ObjectField m_TargetTextureField;

        private EnumField m_ScaleModeField;
        private EnumField m_screenMatchModeField;

        private VisualElement m_ScaleModeConstantPixelSizeGroup;
        private VisualElement m_ScaleModeScaleWithScreenSizeGroup;
        private VisualElement m_ScaleModeContantPhysicalSizeGroup;

        private VisualElement m_ScreenMatchModeMatchWidthOrHeightGroup;

        PropertyField m_ClearColorField;
        PropertyField m_ColorClearValueField;

        private void ConfigureFields()
        {
            // Using MandatoryQ instead of just Q to make sure modifications of the UXML file don't make the
            // necessary elements disappear unintentionally.
            m_ThemeStyleSheetField = m_RootVisualElement.MandatoryQ<ObjectField>("themeStyleSheet");
            m_ThemeStyleSheetField.objectType = typeof(StyleSheet);

            // We have decided to hide the setting of the style sheet until themes are available so we can
            // prevent some misunderstandings happening with people removing the style sheet or replacing
            // with a local one that does not have all the basic styles necessary defined.
            m_ThemeStyleSheetField.style.display = DisplayStyle.None;

            m_TargetTextureField = m_RootVisualElement.MandatoryQ<ObjectField>("targetTexture");
            m_TargetTextureField.objectType = typeof(RenderTexture);

            m_ScaleModeField = m_RootVisualElement.MandatoryQ<EnumField>("scaleMode");
            m_screenMatchModeField = m_RootVisualElement.MandatoryQ<EnumField>("screenMatchMode");

            m_ScaleModeConstantPixelSizeGroup = m_RootVisualElement.MandatoryQ("scaleModeConstantPixelSize");
            m_ScaleModeScaleWithScreenSizeGroup = m_RootVisualElement.MandatoryQ("scaleModeScaleWithScreenSize");
            m_ScaleModeContantPhysicalSizeGroup = m_RootVisualElement.MandatoryQ("scaleModeConstantPhysicalSize");

            m_ScreenMatchModeMatchWidthOrHeightGroup =
                m_RootVisualElement.MandatoryQ("screenMatchModeMatchWidthOrHeight");

            m_ClearColorField = m_RootVisualElement.MandatoryQ<PropertyField>("clearColor");
            m_ColorClearValueField = m_RootVisualElement.MandatoryQ<PropertyField>("colorClearValue");
        }

        private void BindFields()
        {
            m_ScaleModeField.RegisterCallback<ChangeEvent<Enum>>(evt =>
                UpdateScaleModeValues((PanelScaleModes)evt.newValue));
            m_screenMatchModeField.RegisterCallback<ChangeEvent<Enum>>(evt =>
                UpdateScreenMatchModeValues((PanelScreenMatchModes)evt.newValue));
            m_ClearColorField.RegisterCallback<ChangeEvent<bool>>(evt =>
                UpdateColorClearValue(evt.newValue));
        }

        private void UpdateScaleModeValues(PanelScaleModes scaleMode)
        {
            switch (scaleMode)
            {
                case PanelScaleModes.ConstantPixelSize:
                    m_ScaleModeConstantPixelSizeGroup.style.display = DisplayStyle.Flex;
                    m_ScaleModeScaleWithScreenSizeGroup.style.display = DisplayStyle.None;
                    m_ScaleModeContantPhysicalSizeGroup.style.display = DisplayStyle.None;
                    break;
                case PanelScaleModes.ScaleWithScreenSize:
                    m_ScaleModeConstantPixelSizeGroup.style.display = DisplayStyle.None;
                    m_ScaleModeScaleWithScreenSizeGroup.style.display = DisplayStyle.Flex;
                    m_ScaleModeContantPhysicalSizeGroup.style.display = DisplayStyle.None;
                    break;
                case PanelScaleModes.ConstantPhysicalSize:
                    m_ScaleModeConstantPixelSizeGroup.style.display = DisplayStyle.None;
                    m_ScaleModeScaleWithScreenSizeGroup.style.display = DisplayStyle.None;
                    m_ScaleModeContantPhysicalSizeGroup.style.display = DisplayStyle.Flex;
                    break;
            }
        }

        private void UpdateScreenMatchModeValues(PanelScreenMatchModes screenMatchMode)
        {
            switch (screenMatchMode)
            {
                case PanelScreenMatchModes.MatchWidthOrHeight:
                    m_ScreenMatchModeMatchWidthOrHeightGroup.style.display = DisplayStyle.Flex;
                    break;
                default:
                    m_ScreenMatchModeMatchWidthOrHeightGroup.style.display = DisplayStyle.None;
                    break;
            }
        }

        void UpdateColorClearValue(bool newClearColor)
        {
            m_ColorClearValueField.SetEnabled(newClearColor);
        }

        public override VisualElement CreateInspectorGUI()
        {
            if (m_RootVisualElement == null)
            {
                m_RootVisualElement = new VisualElement();
            }
            else
            {
                m_RootVisualElement.Clear();
            }

            if (m_InspectorUxml == null)
            {
                m_InspectorUxml = EditorGUIUtility.Load(k_InspectorVisualTreeAssetPath) as VisualTreeAsset;
            }

            if (k_DefaultStyleSheet == null)
            {
                k_DefaultStyleSheet = EditorGUIUtility.Load(k_DefaultStyleSheetPath) as StyleSheet;
            }
            m_RootVisualElement.styleSheets.Add(k_DefaultStyleSheet);

            m_InspectorUxml.CloneTree(m_RootVisualElement);
            ConfigureFields();
            BindFields();

            PanelSettings panelSettings = (PanelSettings)target;
            UpdateScaleModeValues(panelSettings.scaleMode);
            UpdateScreenMatchModeValues(panelSettings.screenMatchMode);
            UpdateColorClearValue(panelSettings.clearColor);

            return m_RootVisualElement;
        }
    }
}
