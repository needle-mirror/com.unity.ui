using UnityEngine.UIElements;

namespace UnityEditor.UIElements.Inspector
{
    [CustomEditor(typeof(EventSystem))]
    internal class EventSystemInspector : Editor
    {
        private VisualElement navigationOptions;
        private VisualElement repeatOptions;

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            navigationOptions = new Foldout { text = "Navigation Events Settings", style = { left = 3 } };
            navigationOptions.Add(new PropertyField(serializedObject.FindProperty("m_HorizontalAxis")));
            navigationOptions.Add(new PropertyField(serializedObject.FindProperty("m_VerticalAxis")));
            navigationOptions.Add(new PropertyField(serializedObject.FindProperty("m_SubmitButton")));
            navigationOptions.Add(new PropertyField(serializedObject.FindProperty("m_CancelButton")));

            repeatOptions = new VisualElement();
            repeatOptions.Add(new PropertyField(serializedObject.FindProperty("m_InputActionsPerSecond")));
            repeatOptions.Add(new PropertyField(serializedObject.FindProperty("m_RepeatDelay")));

            root.Add(navigationOptions);
            root.Add(repeatOptions);

            return root;
        }
    }
}
