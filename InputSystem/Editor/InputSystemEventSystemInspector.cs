#if UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.UIElements.InputSystem;

namespace UnityEditor.UIElements.Inspector
{
    [CustomEditor(typeof(InputSystemEventSystem))]
    public class InputSystemEventSystemInspector : Editor
    {
#if UIE_PACKAGE
        const string k_DefaultStyleSheetPath = "Packages/com.unity.ui/PackageResources/StyleSheets/Inspector/InputSystemEventSystemInspector.uss";
        const string k_InspectorVisualTreeAssetPath = "Packages/com.unity.ui/PackageResources/UXML/Inspector/InputSystemEventSystemInspector.uxml";
#else
        const string k_DefaultStyleSheetPath = "UIPackageResources/StyleSheets/Inspector/InputSystemEventSystemInspector.uss";
        const string k_InspectorVisualTreeAssetPath = "UIPackageResources/UXML/Inspector/InputSystemEventSystemInspector.uxml";
#endif
        private const string k_StyleClassWarningHidden = "unity-input-system-event-system-inspector--warning--hidden";

        private static StyleSheet k_DefaultStyleSheet = null;
        private VisualTreeAsset m_InspectorUxml;

        private HelpBox m_NoAssetWarning;
        private ObjectField m_AssetField;

        public override VisualElement CreateInspectorGUI()
        {
            if (m_InspectorUxml == null)
            {
                m_InspectorUxml = (VisualTreeAsset)EditorGUIUtility.Load(k_InspectorVisualTreeAssetPath);
            }

            if (k_DefaultStyleSheet == null)
            {
                k_DefaultStyleSheet = (StyleSheet)EditorGUIUtility.Load(k_DefaultStyleSheetPath);
            }

            var root = new VisualElement();
            root.styleSheets.Add(k_DefaultStyleSheet);
            m_InspectorUxml.CloneTree(root);

            var inputSystemDisabledInfo = root.Q<HelpBox>("inputSystemDisabledInfo");

#if ENABLE_INPUT_SYSTEM
            inputSystemDisabledInfo.style.display = DisplayStyle.None;
#endif

            m_NoAssetWarning = root.Q<HelpBox>("noAssetWarning");
            m_AssetField = root.Q<ObjectField>("inputActionAssetField");
            m_AssetField.objectType = typeof(InputActionAsset);
            m_AssetField.RegisterValueChangedCallback(ev => UpdateValues());

            // Need to wait 1 frame for m_AssetField.value to be properly bound.
            m_AssetField.schedule.Execute(UpdateValues);

            return root;
        }

        private void UpdateValues()
        {
            var asset = m_AssetField.value as InputActionAsset;
            m_NoAssetWarning.EnableInClassList(k_StyleClassWarningHidden, asset != null);
        }
    }
}

#endif
