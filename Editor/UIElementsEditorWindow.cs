using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    internal class UIElementsEditorWindow : UnityEditor.EditorWindow
    {
        public void OnEnable()
        {
            var root = this.rootVisualElement;
            InitializeVisualTree(root);
        }

        public virtual void InitializeVisualTree(VisualElement root)
        {
        }
    }
}
