using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Samples.Editor.General
{
    [CustomEditor(typeof(RootSelectorBehavior))]
    public class RootSelectorBehaviorInspector : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset uxmlAsset = default;
        [SerializeField] private StyleSheet ussAsset = default;

        public override VisualElement CreateInspectorGUI()
        {
            var container = uxmlAsset.CloneTree();
            container.styleSheets.Add(ussAsset);
            return container;
        }
    }
}
