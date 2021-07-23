using System.Linq;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    internal class PanelTextSettingsImporter
    {
        #if UIE_PACKAGE
        internal static string k_DefaultPanelTextSettingsPath =
            "Packages/com.unity.ui/PackageResources/Text/Default Panel Text Settings.asset";
        #else
        internal static string k_DefaultPanelTextSettingsPath =
            "UIPackageResources/Text/Default Panel Text Settings.asset";
        #endif


        PanelTextSettingsImporter() {}

        internal static PanelTextSettings GetDefaultPanelTextSettings()
        {
            return EditorGUIUtility.Load(k_DefaultPanelTextSettingsPath) as PanelTextSettings;
        }
    }
}
