using UnityEngine.UIElements;
using UnityEngine;

namespace Samples.Runtime.General
{
    [RequireComponent(typeof(UIDocument), typeof(EventSystem))]
    public class RootSelector : MonoBehaviour
    {
        [SerializeField] private PanelSettings panelSettings = default;
        [SerializeField] private VisualTreeAsset sourceAsset = default;
        [SerializeField] private StyleSheet styleAsset = default;

        private void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            uiDocument.panelSettings = panelSettings;
            uiDocument.visualTreeAsset = sourceAsset;
        }

        private void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();
            uiDocument.rootVisualElement.styleSheets.Add(styleAsset);
        }
    }
}
