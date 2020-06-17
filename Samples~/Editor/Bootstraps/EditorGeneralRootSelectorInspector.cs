using Samples.Editor.General;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Samples.Utils
{
    public static partial class MenuItems
    {
        [MenuItem("Window/UI Toolkit/Examples/General/Root Selector (Inspector)")]
        public static void ShowGeneralRootSelectorInspector()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            var go = new GameObject("Sample Object");
            go.AddComponent<RootSelectorBehavior>();
            EditorGUIUtility.PingObject(go);
            Selection.activeGameObject = go;
        }
    }
}
