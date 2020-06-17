using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

namespace Samples.Editor.General
{
    public class RootSelectorWindow : EditorWindow
    {
        [MenuItem("Window/UI Toolkit/Examples/General/Root Selector (Window)")]
        public static void OpenWindow()
        {
            var window = GetWindow<RootSelectorWindow>("Root Selector");
            EditorGUIUtility.PingObject(MonoScript.FromScriptableObject(window));
        }

        [SerializeField] private VisualTreeAsset uxmlAsset = default;
        [SerializeField] private StyleSheet ussAsset = default;

        public void OnEnable()
        {
            uxmlAsset.CloneTree(rootVisualElement);
            rootVisualElement.styleSheets.Add(ussAsset);
        }
    }
}
