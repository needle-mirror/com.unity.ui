#if UIE_PACKAGE
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using UnityEngine;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("UnityEditor.UIElementsModule")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Unity Technologies")]
[assembly: AssemblyProduct("com.unity.ui")]
[assembly: AssemblyCopyright("Copyright © Unity Technologies 2020")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

//[assembly: AssemblyIsEditorAssembly]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("64c368b7-2f49-4a4f-975a-6b64cf0a41c1")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// ADD_NEW_PLATFORM_HERE
[assembly: InternalsVisibleTo("Unity.LiveNotes")]
[assembly: InternalsVisibleTo("Unity.Burst")]
[assembly: InternalsVisibleTo("Unity.Burst.Editor")]
[assembly: InternalsVisibleTo("Unity.Cloud.Collaborate.Editor")]
[assembly: InternalsVisibleTo("Unity.CollabProxy.Editor")]
[assembly: InternalsVisibleTo("Unity.CollabProxy.EditorTests")]
[assembly: InternalsVisibleTo("Unity.CollabProxy.UI")]
[assembly: InternalsVisibleTo("Unity.CollabProxy.UI.Tests")]
[assembly: InternalsVisibleTo("Unity.CollabProxy.Client")]
[assembly: InternalsVisibleTo("Unity.CollabProxy.Client.Tests")]
[assembly: InternalsVisibleTo("UnityEditor.Advertisements")]
[assembly: InternalsVisibleTo("Unity.PackageManager")]
[assembly: InternalsVisibleTo("Unity.PackageManagerStandalone")]
[assembly: InternalsVisibleTo("Unity.AndroidBuildPipeline")]
[assembly: InternalsVisibleTo("Unity.Automation")]
[assembly: InternalsVisibleTo("UnityEngine.Common")]
[assembly: InternalsVisibleTo("Unity.PureCSharpTests")]
[assembly: InternalsVisibleTo("Unity.IntegrationTests")]
[assembly: InternalsVisibleTo("Unity.DeploymentTests.Services")]
[assembly: InternalsVisibleTo("Unity.IntegrationTests.UnityAnalytics")]
[assembly: InternalsVisibleTo("Unity.Timeline.Editor")]
[assembly: InternalsVisibleTo("Unity.PackageManagerUI.Develop.Editor")]
[assembly: InternalsVisibleTo("Unity.DeviceSimulator.Editor")]

[assembly: InternalsVisibleTo("Unity.Timeline.EditorTests")]
[assembly: InternalsVisibleTo("UnityEditor.Graphs")]
[assembly: InternalsVisibleTo("UnityEditor.UWP.Extensions")]
[assembly: InternalsVisibleTo("UnityEditor.iOS.Extensions.Common")]
[assembly: InternalsVisibleTo("UnityEditor.iOS.Extensions")]
[assembly: InternalsVisibleTo("UnityEditor.AppleTV.Extensions")]
[assembly: InternalsVisibleTo("UnityEditor.Android.Extensions")]
[assembly: InternalsVisibleTo("UnityEditor.WebGL.Extensions")]
[assembly: InternalsVisibleTo("UnityEditor.LinuxStandalone.Extensions")]
[assembly: InternalsVisibleTo("UnityEditor.WindowsStandalone.Extensions")]
[assembly: InternalsVisibleTo("UnityEditor.OSXStandalone.Extensions")]
[assembly: InternalsVisibleTo("UnityEditor.Lumin.Extensions")]
[assembly: InternalsVisibleTo("UnityEditor.Stadia.Extensions")]
[assembly: InternalsVisibleTo("UnityEditor.Networking")]
[assembly: InternalsVisibleTo("UnityEngine.Networking")]
[assembly: InternalsVisibleTo("Unity.Analytics.Editor")]
[assembly: InternalsVisibleTo("UnityEditor.Analytics")]
[assembly: InternalsVisibleTo("UnityEditor.Purchasing")]
[assembly: InternalsVisibleTo("UnityEditor.Lumin")]
[assembly: InternalsVisibleTo("UnityEditor.EditorTestsRunner")]
[assembly: InternalsVisibleTo("UnityEditor.TestRunner")]
[assembly: InternalsVisibleTo("UnityEditor.TestRunner.Tests")]
[assembly: InternalsVisibleTo("Unity.Compiler.Client")]
[assembly: InternalsVisibleTo("ExternalCSharpCompiler")]
[assembly: InternalsVisibleTo("UnityEngine.TestRunner")]
[assembly: InternalsVisibleTo("UnityEditor.VR")]
[assembly: InternalsVisibleTo("Unity.RuntimeTests")]
[assembly: InternalsVisibleTo("Unity.RuntimeTests.Framework")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor-firstpass-testable")]
[assembly: InternalsVisibleTo("Assembly-CSharp-Editor-testable")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")] // for Moq
[assembly: InternalsVisibleTo("UnityEditor.InteractiveTutorialsFramework")]
[assembly: InternalsVisibleTo("UnityEditor.Networking")]
[assembly: InternalsVisibleTo("UnityEditor.UI")]
[assembly: InternalsVisibleTo("UnityEditor.AR")]
[assembly: InternalsVisibleTo("UnityEditor.SpatialTracking")]
[assembly: InternalsVisibleTo("Unity.WindowsMRAutomation")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEngineBridgeDev.003")]  // for Graph Tools Foundation
[assembly: InternalsVisibleTo("Unity.InternalAPIEngineBridge.015")]     // for Graph Tools Foundation
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.001")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.002")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.003")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.004")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.005")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.006")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.007")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.008")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.009")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.010")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.011")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.012")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.013")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.014")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.015")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.016")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.017")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.018")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.019")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.020")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.021")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.022")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.023")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridge.024")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridgeDev.001")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridgeDev.002")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridgeDev.003")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridgeDev.004")]
[assembly: InternalsVisibleTo("Unity.InternalAPIEditorBridgeDev.005")]
[assembly: InternalsVisibleTo("Unity.XR.Remoting.Editor")]
[assembly: InternalsVisibleTo("UnityEngine.Common")]
[assembly: InternalsVisibleTo("Unity.UI.Builder.Editor")]
[assembly: InternalsVisibleTo("UnityEditor.UIBuilderModule")]
[assembly: InternalsVisibleTo("Unity.UI.Builder.EditorTests")]
[assembly: InternalsVisibleTo("Unity.GraphViewTestUtilities.Editor")]
[assembly: InternalsVisibleTo("Unity.ProBuilder.Editor")]
[assembly: InternalsVisibleTo("Unity.2D.Sprite.Editor")]
[assembly: InternalsVisibleTo("Unity.2D.Sprite.EditorTests")]
[assembly: InternalsVisibleTo("Unity.2D.Tilemap.Editor")]
[assembly: InternalsVisibleTo("Unity.2D.Tilemap.EditorTests")]
[assembly: InternalsVisibleTo("Unity.PackageCleanConsoleTest.Editor")]
[assembly: InternalsVisibleTo("Unity.UIElements.Tests")]
[assembly: InternalsVisibleTo("Unity.UIElements.EditorTests")]
[assembly: InternalsVisibleTo("UnityEngine.UIElementsGameObjectsModule")]
[assembly: InternalsVisibleTo("UnityEditor.UIElementsGameObjectsModule")]
[assembly: InternalsVisibleTo("UnityEngine.UIElementsInputSystemModule")]
[assembly: InternalsVisibleTo("UnityEditor.UIElementsInputSystemModule")]
[assembly: InternalsVisibleTo("Unity.UIElements.Editor.Text")]

[assembly: InternalsVisibleTo("Unity.SceneTemplate.Editor")]
[assembly: InternalsVisibleTo("com.unity.purchasing.udp.Editor")]
#if ENABLE_LINQPAD
[assembly: InternalsVisibleTo("LINQPadQuery")] // required in order for LINQPad to hook up to this dll and call into internals
#endif

#endif
