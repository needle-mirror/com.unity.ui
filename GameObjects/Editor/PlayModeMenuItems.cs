using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements.GameObjects
{
    internal static class PlayModeMenuItems
    {
        private const string k_UILayerName = "UI";
        private const string k_AssetSearchByTypePanelSettings = "t:panelsettings";
        private const string k_AssetsFolder = "Assets";
        private const string k_PanelSettingsAssetPath = k_AssetsFolder + "/PanelSettings.asset";
        private static string[] k_AssetsFolderFilter = new[] { k_AssetsFolder };

        [MenuItem("GameObject/UI Toolkit/UI Document", false, 9)]
        public static void AddUIDocument(MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            Type type = typeof(UIDocument);
            var root = ObjectFactory.CreateGameObject(type.Name, type);
            UIDocument uiDocument = root.GetComponent<UIDocument>();

            // Works for all stages.
            StageUtility.PlaceGameObjectInCurrentStage(root);
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                Undo.SetTransformParent(root.transform, prefabStage.prefabContentsRoot.transform, "");
            }

            Undo.SetCurrentGroupName("Create " + root.name);

            if (parent != null)
            {
                SetParentAndAlign(root, parent);
                uiDocument.ReactToHierarchyChanged();
            }
            else
            {
                root.layer = LayerMask.NameToLayer(k_UILayerName);
            }

            Selection.activeGameObject = root;


            // Look for our event system, if there's none create one.
            var eventSystems = UnityEngine.Object.FindObjectOfType(typeof(EventSystem));
            if (eventSystems == null)
            {
                root.AddComponent<EventSystem>();
            }

            // Set a PanelSettings instance so that the UI appears immediately on selecting the UXML.
            // If the UIDocument was created as a child of another UIDocument, this step is not necessary.
            if (uiDocument.parentUI == null)
            {
                var panelSettingsInProject = AssetDatabase.FindAssets(k_AssetSearchByTypePanelSettings, k_AssetsFolderFilter);
                if (panelSettingsInProject != null && panelSettingsInProject.Length > 0)
                {
                    // Use the first one found.
                    PanelSettings panelSettings =
                        AssetDatabase.LoadAssetAtPath<PanelSettings>(
                            AssetDatabase.GUIDToAssetPath(panelSettingsInProject[0]));
                    uiDocument.panelSettings = panelSettings;
                }
                else
                {
                    // Create one.
                    PanelSettings panelSettings = ScriptableObject.CreateInstance<PanelSettings>();

                    AssetDatabase.CreateAsset(panelSettings, k_PanelSettingsAssetPath);
                    panelSettingsInProject = AssetDatabase.FindAssets(k_AssetSearchByTypePanelSettings, k_AssetsFolderFilter);

                    // We just created the asset, it MUST exist so if it doesn't there's something wrong.
                    Debug.Assert(panelSettingsInProject != null && panelSettingsInProject.Length > 0,
                        "PanelSettings asset not found for assigning to created UIDocument");

                    panelSettings =
                        AssetDatabase.LoadAssetAtPath<PanelSettings>(
                            AssetDatabase.GUIDToAssetPath(panelSettingsInProject[0]));
                    uiDocument.panelSettings = panelSettings;
                }
            }
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            Undo.SetTransformParent(child.transform, parent.transform, "");
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;

            child.layer = parent.layer;
        }
    }
}
