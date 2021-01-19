using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements.Text
{
    internal class TextUtilities
    {
        internal static Object GetTextSettings()
        {
            return PanelTextSettings.GetTextSettings();
        }

        internal static bool IsFontAsset(Object fontAsset)
        {
            return fontAsset.GetType() == typeof(FontAsset);
        }

        internal static Font GetFont(Object fontAsset)
        {
            return (fontAsset as FontAsset).sourceFontFile;
        }

        internal static FontAsset GetFontAsset(MeshGenerationContextUtils.TextParams textParam)
        {
            var textSettings = GetTextSettingsFrom(textParam);
            if (textParam.fontDefinition.fontAsset != null)
                return textParam.fontDefinition.fontAsset as FontAsset;
            if (textParam.fontDefinition.font != null)
                return textSettings.GetCachedFontAsset(textParam.fontDefinition.font);
            return textSettings.GetCachedFontAsset(textParam.font);
        }

        internal static FontAsset GetFontAsset(VisualElement ve)
        {
            var textSettings = GetTextSettingsFrom(ve);
            if (ve.computedStyle.unityFontDefinition.fontAsset != null)
                return ve.computedStyle.unityFontDefinition.fontAsset as FontAsset;
            if (ve.computedStyle.unityFontDefinition.font != null)
                return textSettings.GetCachedFontAsset(ve.computedStyle.unityFontDefinition.font);
            return textSettings.GetCachedFontAsset(ve.computedStyle.unityFont);
        }

        internal static PanelTextSettings GetTextSettingsFrom(VisualElement ve)
        {
            if (ve.panel is RuntimePanel runtimePanel)
                return runtimePanel.panelSettings.textSettings as PanelTextSettings;
            return PanelTextSettings.EditorTextSettings;
        }

        internal static PanelTextSettings GetTextSettingsFrom(MeshGenerationContextUtils.TextParams textParam)
        {
            if (textParam.panel is RuntimePanel runtimePanel)
            {
                var textsettings = runtimePanel.panelSettings.textSettings as PanelTextSettings;
                if (textsettings == null)
                    Debug.LogFormat(runtimePanel.panelSettings, "PanelSettings is invalid. Please assign a valid PanelTextSettings.");
                return textsettings;
            }
            return PanelTextSettings.EditorTextSettings;
        }

        internal static TextCoreSettings TextCoreSettingsForElement(UnityEngine.UIElements.VisualElement ve)
        {
            var fontAsset = GetFontAsset(ve);
            if (fontAsset == null)
                return new TextCoreSettings();

            var resolvedStyle = ve.resolvedStyle;
            var computedStyle = ve.computedStyle;

            // Convert the text settings pixel units to TextCore relative units
            float paddingPercent = 1.0f / fontAsset.atlasPadding;
            float pointSizeRatio = ((float)fontAsset.faceInfo.pointSize) / ve.computedStyle.fontSize.value;
            float factor = paddingPercent * pointSizeRatio;

            float outlineWidth = Mathf.Max(0.0f, resolvedStyle.unityTextOutlineWidth * factor);
            float underlaySoftness = Mathf.Max(0.0f, computedStyle.textShadow.blurRadius * factor);
            Vector2 underlayOffset = computedStyle.textShadow.offset * factor;

            var outlineColor = resolvedStyle.unityTextOutlineColor;
            if (outlineWidth < Mathf.Epsilon)
                outlineColor.a = 0.0f;

            return new TextCoreSettings() {
                outlineColor = outlineColor,
                outlineWidth = outlineWidth,
                underlayColor = computedStyle.textShadow.color,
                underlayOffset = underlayOffset,
                underlaySoftness = underlaySoftness
            };
        }
    }
}
