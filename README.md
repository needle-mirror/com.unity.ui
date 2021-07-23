# UI Toolkit (Package)

UI Toolkit is a set of features, resources and tools for developing user interfaces (UI). Use it to develop custom UI and extensions for the Unity Editor, runtime debugging tools, and runtime UI for games and applications.

UI Toolkit includes:

- A retained-mode UI system, based on recognized web technologies, that supports stylesheets, and dynamic and contextual event handling.
- UXML and USS asset types for building and styling user interfaces. These are inspired by standard web technologies such as XML, HTML, and CSS.
- Tools and resources such as a built-in UI debugger, support for the UI Builder visual UI authoring tool&#42;, and examples available in the editor.

&#42;Available via the com.unity.ui.builder package

## About this package

UI Toolkit is built into Unity as a core part of the Editor. It is also available in this package, com.unity.ui, which is currently in preview. The built-in version for each major Unity release is based on a specific version of this package, but the two are not completely identical.

## Builtin UI Toolkit vs. the UI Toolkit package

The built-in version of UI Toolkit includes the features required to make user interfaces for Unity Editor extensions. The package version contains the exact same features, as well as the features required to make runtime user interfaces for games and applications. It may also contain other new features that are currently in preview.

Both versions of UI Toolkit work exactly the same way, and use the same namespaces: `UnityEditor.UIElements` and `UnityEngine.UIElements`. When you install the package, you will not see many difference other than having access to the runtime features, and other preview features. You do not need to do any additional configuration.

# Using package-only UI Toolkit features

In a project, code that needs UI Toolkit features that are only present in the package MUST reference the assemblies explicitly. Otherwise, the compilation order is not guaranteed, and you might see errors in the Editor or at build time related to missing APIs. In a assembly definition file (.asmdef), you must add an explicit reference to `"UnityEngine.UIElementsModule"` to access new runtime APIs or `"UnityEditor.UIElementsModule"` for editor features.

Here is a sample of an assembly definition file adding these references:

```
{
  "name": "AssemblyConsumingNewAPI",
  "references": [
     "UnityEngine.UIElementsModule",
     "UnityEditor.UIElementsModule"
     ],
  "includePlatforms": [],
  "excludePlatforms": [],
  "allowUnsafeCode": true,
  "overrideReferences": false,
  "precompiledReferences": [],
  "autoReferenced": false,
  "defineConstraints": [],
  "versionDefines": [],
  "noEngineReferences": false
}
```
