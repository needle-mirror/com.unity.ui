#if UNITY_INPUT_SYSTEM
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.UIElements.InputSystem;

namespace UnityEditor.UIElements.Inspector
{
    [CustomPropertyDrawer(typeof(InputActionDropdownAttribute))]
    public class InputActionDropdownInspector : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var assetProperty = property.serializedObject
                .FindProperty(((InputActionDropdownAttribute)attribute).inputActionAssetProperty);

            InputActionAsset asset = assetProperty.objectReferenceValue as InputActionAsset;
            var result = new VisualElement();
            UpdateChoices(property, asset, result);

            result.schedule.Execute(() =>
            {
                if (!SerializedPropertyDelegates.IsPropertyValid(property)) return;
                var newAsset = assetProperty.objectReferenceValue as InputActionAsset;
                if (asset != newAsset)
                {
                    asset = newAsset;
                    UpdateChoices(property, asset, result);
                }
            }).Every(100);

            return result;
        }

        private void UpdateChoices(SerializedProperty property, InputActionAsset inputActionAsset, VisualElement result)
        {
            result.Clear();

            if (inputActionAsset == null)
            {
                var propertyField = new ObjectField(property.displayName);
                propertyField.SetEnabled(false);
                result.Add(propertyField);
                return;
            }

            var assetReferences = AssetDatabase
                .LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(inputActionAsset))
                .OfType<InputActionReference>()
                .ToList();

            // Ugly hack: GenericMenu interprets "/" as a submenu path. But luckily, "/" is not the only slash we have in Unicode.
            var choices = inputActionAsset.Select(a => a.actionMap.name + "\uFF0F" + a.name).ToList();
            choices.Insert(0, "None");
            var references = inputActionAsset.Select(a => assetReferences.Find(r => a == r.action)).ToList();
            references.Insert(0, null);

            var field = new PopupField<string>(property.displayName, choices, 0);
            {
                var selectedAction = ((InputActionReference)property.objectReferenceValue)?.action;
                field.value = choices[Mathf.Max(0, references.FindIndex(a => (a == null ? null : a.action) == selectedAction))];
            }
            field.RegisterCallback<ChangeEvent<string>>(ev =>
            {
                property.objectReferenceValue = references[Mathf.Clamp(field.index, 0, references.Count - 1)];
                property.serializedObject.ApplyModifiedProperties();
            });
            field.schedule.Execute(() =>
            {
                if (!SerializedPropertyDelegates.IsPropertyValid(property)) return;
                var selectedAction = ((InputActionReference)property.objectReferenceValue)?.action;
                field.value = choices[Mathf.Max(0, references.FindIndex(a => (a == null ? null : a.action) == selectedAction))];
            }).Every(100);

            result.Add(field);
        }
    }
}
#endif
