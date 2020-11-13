using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements.Text
{
    partial struct TextCoreHandle
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        #endif
        static void InjectTextCoreHandle()
        {
            TextHandleFactory.CreateEditorHandle += New;
            TextHandleFactory.CreateRuntimeHandle += New;
        }
    }

    internal class TextDelegates
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void InitializeDelegates()
        {
            UnityEngine.UIElements.TextDelegates.GetFont += TextUtilities.GetFont;
            UnityEngine.UIElements.TextDelegates.GetTextCoreSettingsForElement += TextUtilities.TextCoreSettingsForElement;
            UnityEngine.UIElements.TextDelegates.GetIDGradientScale += () => TextShaderUtilities.ID_GradientScale;
            UnityEngine.UIElements.TextDelegates.IsFontAsset += TextUtilities.IsFontAsset;
            UnityEngine.UIElements.TextDelegates.GetTextSettings += TextUtilities.GetTextSettings;
        }

        #if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void EditorInitialization()
        {
            InitializeDelegates();
            InitializeFontAssetInspectorEvents();
        }

        static void InitializeFontAssetInspectorEvents()
        {
            // Note: the "changed" bool parameter for these two is always true (?)
            TextEventManager.FONT_PROPERTY_EVENT.Add(((b, o) => UnityEngine.UIElements.TextDelegates.RaiseTextAssetChange(o)));
            TextEventManager.SPRITE_ASSET_PROPERTY_EVENT.Add(((b, o) => UnityEngine.UIElements.TextDelegates.RaiseTextAssetChange(o)));
            TextEventManager.COLOR_GRADIENT_PROPERTY_EVENT.Add(UnityEngine.UIElements.TextDelegates.RaiseTextAssetChange);
        }

        #endif
    }
}
