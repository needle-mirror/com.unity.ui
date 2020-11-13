using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR.Implementation;
using TextGenerationSettings = UnityEngine.TextCore.Text.TextGenerationSettings;
using TextGenerator = UnityEngine.TextCore.Text.TextGenerator;
using TextInfo = UnityEngine.TextCore.Text.TextInfo;

namespace UnityEngine.UIElements.Text
{
    internal partial struct TextCoreHandle : ITextHandle
    {
        public static ITextHandle New()
        {
            TextCoreHandle h = new TextCoreHandle();
            h.m_CurrentGenerationSettings = new UnityEngine.TextCore.Text.TextGenerationSettings();
            h.m_CurrentLayoutSettings = new UnityEngine.TextCore.Text.TextGenerationSettings();
            return h;
        }

        Vector2 m_PreferredSize;
        int m_PreviousGenerationSettingsHash;
        UnityEngine.TextCore.Text.TextGenerationSettings m_CurrentGenerationSettings;
        int m_PreviousLayoutSettingsHash;
        UnityEngine.TextCore.Text.TextGenerationSettings m_CurrentLayoutSettings;

        /// <summary>
        /// DO NOT USE m_TextInfo directly, use textInfo to guarantee lazy allocation.
        /// </summary>
        private UnityEngine.TextCore.Text.TextInfo m_TextInfo;

        /// <summary>
        /// The TextInfo instance, use from this instead of the m_TextInfo member to guarantee lazy allocation.
        /// </summary>
        internal UnityEngine.TextCore.Text.TextInfo textInfo
        {
            get
            {
                if (m_TextInfo == null)
                {
                    m_TextInfo = new UnityEngine.TextCore.Text.TextInfo();
                }

                return m_TextInfo;
            }
        }

        /// <summary>
        /// DO NOT USE m_UITKTextInfo directly, use uITKTextInfo to guarantee lazy allocation.
        /// </summary>
        private UnityEngine.UIElements.TextInfo m_UITKTextInfo;

        /// <summary>
        /// The TextInfo instance, use from this instead of the m_UITKTextInfo member to guarantee lazy allocation.
        /// </summary>
        internal UnityEngine.UIElements.TextInfo uITKTextInfo
        {
            get
            {
                if (m_UITKTextInfo == null)
                {
                    m_UITKTextInfo = new UnityEngine.UIElements.TextInfo();
                }

                return m_UITKTextInfo;
            }
        }

        internal bool IsTextInfoAllocated()
        {
            return m_TextInfo != null;
        }

        public Vector2 GetCursorPosition(CursorPositionStylePainterParameters parms, float scaling)
        {
            return UnityEngine.TextCore.Text.TextGenerator.GetCursorPosition(textInfo, parms.rect, parms.cursorIndex);
        }

        public float ComputeTextWidth(MeshGenerationContextUtils.TextParams parms, float scaling)
        {
            UpdatePreferredValues(parms);
            return m_PreferredSize.x;
        }

        public float ComputeTextHeight(MeshGenerationContextUtils.TextParams parms, float scaling)
        {
            UpdatePreferredValues(parms);
            return m_PreferredSize.y;
        }

        public float GetLineHeight(int characterIndex, MeshGenerationContextUtils.TextParams textParams, float textScaling, float pixelPerPoint)
        {
            Update(textParams, pixelPerPoint);
            var character = m_TextInfo.textElementInfo[m_TextInfo.characterCount - 1];
            var line = m_TextInfo.lineInfo[character.lineNumber];
            return line.lineHeight;
        }

        public int VerticesCount(MeshGenerationContextUtils.TextParams parms, float pixelPerPoint)
        {
            Update(parms, pixelPerPoint);
            var verticesCount = 0;
            foreach (var meshInfo in textInfo.meshInfo)
                verticesCount += meshInfo.vertexCount;
            return verticesCount;
        }

        ITextHandle ITextHandle.New()
        {
            return New();
        }

        public void DrawText(UIRStylePainter painter, MeshGenerationContextUtils.TextParams textParams, float pixelsPerPoint)
        {
            painter.DrawTextCore(textParams, this, pixelsPerPoint);
        }

        public UnityEngine.UIElements.TextInfo Update(MeshGenerationContextUtils.TextParams parms, float pixelsPerPoint)
        {
            // The screenRect in TextCore is not properly implemented with regards to the offset part, so zero it out for now and we will add it ourselves later
            parms.rect = new Rect(Vector2.zero, parms.rect.size);
            int paramsHash = parms.GetHashCode();
            if (m_PreviousGenerationSettingsHash == paramsHash)
                return uITKTextInfo;

            UpdateGenerationSettingsCommon(parms, m_CurrentGenerationSettings);

            m_CurrentGenerationSettings.color = parms.fontColor;
            m_CurrentGenerationSettings.inverseYAxis = true;
            m_CurrentGenerationSettings.scale = pixelsPerPoint;

            textInfo.isDirty = true;
            UnityEngine.TextCore.Text.TextGenerator.GenerateText(m_CurrentGenerationSettings, textInfo);
            m_PreviousGenerationSettingsHash = paramsHash;
            return ConvertTo(textInfo);
        }

        private TextMeshInfo ConvertTo(MeshInfo meshInfo)
        {
            TextMeshInfo result;
            result.vertexCount = meshInfo.vertexCount;
            result.vertices = meshInfo.vertices;
            result.uvs0 = meshInfo.uvs0;
            result.uvs2 = meshInfo.uvs2;
            result.colors32 = meshInfo.colors32;
            result.triangles = meshInfo.triangles;
            result.material = meshInfo.material;

            return result;
        }

        private void ConvertTo(MeshInfo[] meshInfos, List<TextMeshInfo> result)
        {
            result.Clear();
            for (int i = 0; i < meshInfos.Length; i++)
                result.Add(ConvertTo(meshInfos[i]));
        }

        private UnityEngine.UIElements.TextInfo ConvertTo(UnityEngine.TextCore.Text.TextInfo textInfo)
        {
            uITKTextInfo.materialCount = textInfo.materialCount;
            ConvertTo(textInfo.meshInfo, uITKTextInfo.meshInfos);

            return uITKTextInfo;
        }

        void UpdatePreferredValues(MeshGenerationContextUtils.TextParams parms)
        {
            // The screenRect in TextCore is not properly implemented with regards to the offset part, so zero it out for now and we will add it ourselves later
            parms.rect = new Rect(Vector2.zero, parms.rect.size);
            int paramsHash = parms.GetHashCode();
            if (m_PreviousLayoutSettingsHash == paramsHash)
                return;

            UpdateGenerationSettingsCommon(parms, m_CurrentLayoutSettings);
            m_PreferredSize = UnityEngine.TextCore.Text.TextGenerator.GetPreferredValues(m_CurrentLayoutSettings, textInfo);
            m_PreviousLayoutSettingsHash = paramsHash;
        }

        private static TextOverflowMode GetTextOverflowMode(MeshGenerationContextUtils.TextParams textParams)
        {
            if (textParams.textOverflow == TextOverflow.Clip)
                return TextOverflowMode.Masking;

            if (textParams.textOverflow != TextOverflow.Ellipsis)
                return TextOverflowMode.Overflow;

            if (!textParams.wordWrap && textParams.overflow == OverflowInternal.Hidden)
                return TextOverflowMode.Ellipsis;

            return TextOverflowMode.Overflow;
        }

        static void UpdateGenerationSettingsCommon(MeshGenerationContextUtils.TextParams painterParams,
            UnityEngine.TextCore.Text.TextGenerationSettings settings)
        {
            if (settings.textSettings == null)
            {
                settings.textSettings = TextUtilities.GetTextSettingsFrom(painterParams);
                if (settings.textSettings == null)
                    return;
            }

            settings.fontAsset = TextUtilities.GetFontAsset(painterParams);
            settings.material = settings.fontAsset.material;

            // in case rect is not properly set (ex: style has not been resolved), make sure its width at least matches wordWrapWidth
            var screenRect = painterParams.rect;
            if (float.IsNaN(screenRect.width))
                screenRect.width = painterParams.wordWrapWidth;

            settings.screenRect = screenRect;
            settings.text = string.IsNullOrEmpty(painterParams.text) ? " " : painterParams.text;
            settings.fontSize = painterParams.fontSize > 0
                ? painterParams.fontSize
                : settings.fontAsset.faceInfo.pointSize;
            settings.fontStyle = TextGeneratorUtilities.LegacyStyleToNewStyle(painterParams.fontStyle);
            settings.textAlignment = TextGeneratorUtilities.LegacyAlignmentToNewAlignment(painterParams.anchor);
            settings.wordWrap = painterParams.wordWrap;
            settings.wordWrappingRatio = 0.4f;
            settings.richText = painterParams.richText;
            settings.overflowMode = GetTextOverflowMode(painterParams);
            settings.characterSpacing = painterParams.letterSpacing.value;
            settings.wordSpacing = painterParams.wordSpacing.value;
            settings.paragraphSpacing = painterParams.paragraphSpacing.value;
        }
    }
}
