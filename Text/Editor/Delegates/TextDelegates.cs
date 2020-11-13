using UnityEngine.UIElements.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.TextCore.Text;

namespace UnityEditor.UIElements.Text
{
    internal class TextDelegates
    {
        [InitializeOnLoadMethod]
        static void InitializeDelegates()
        {
            UnityEngine.UIElements.TextDelegates.ImportDefaultTextSettings +=
                TextSettingsImporter.ImportEssentialTextResources;
            UnityEngine.UIElements.TextDelegates.HasTextSettings += () =>
                PanelTextSettings.HasTextSettings;
#if UNITY_2020_2_OR_NEWER
            UnityEditor.UIElements.TextEditorDelegates.GetObjectFieldOfTypeFontAsset += () =>
            {
                var o = new ObjectField();
                o.objectType = typeof(FontAsset);
                return o;
            };
#elif UNITY_2020_1
            Unity.UIElements.Editor.Debugger.StyleField.GetObjectFieldOfTypeFontAsset += () =>
            {
                var o = new ObjectField();
                o.objectType = typeof(FontAsset);
                return o;
            };
#endif
        }
    }
}
