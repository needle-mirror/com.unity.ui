# UI Toolkit

Unity's UI Toolkit is a collection of features, functionality, resources and tools for developing user interfaces (UI). You can use UI Toolkit to develop custom UI and extensions for the Unity Editor, runtime debugging tools, and runtime UI for games and applications.

UI Toolkit is based on, and inspired by, standard web technologies. If you have experience developing web pages or applications, much of your knowledge might be transferable, and many of the core concepts might be familiar.

For more information, see the [UI Toolkit Overview](#ui-toolkit-overview) below.

> [!NOTE]
> Although Unity recommends using UI Toolkit for some new UI development projects, it is still missing  features found in Unity UI (uGUI) and IMGUI. These older systems are more appropriate for certain use cases, and are required to support legacy projects. For information about when it is appropriate to choose an older system instead of the UI Toolkit, see the [Comparison of UI systems in Unity](https://docs.unity3d.com/2020.1/Documentation/Manual/UI-system-compare.html).

## About this package
UI Toolkit is built into Unity as a core part of the Editor. It is also available in this package, com.unity.ui, which is currently in preview. The built-in version for each major Unity release is based on a specific version of this package, but the two are not completely identical.

- The built-in version of UI Toolkit includes the features required to make user interfaces for Unity Editor extensions.
- This package contains the exact same features, as well as the features required to make runtime user interfaces for games and applications. It may also contain other new features that are currently in preview.

For more information, see the [UI Toolkit Package page in the Unity manual](https://docs.unity3d.com/2020.1/Documentation/Manual/UITK-package.html).

## Getting Documentation

- For UI Toolkit user guide, see the [UI Toolkit section of the Unity Manual](https://docs.unity3d.com/2020.1/Documentation/Manual/UIElements.html)

- For the UI Toolkit scripting reference, see [the API section of this documentation](../api/index.html).

## UI Toolkit Overview

This section provides a short description of the major UI Toolkit features, functionality, resources and tools, including:

- The [**UI system**](#ui-system) that contains the core features and functionality required to create user interfaces.
- [**UI Assets**](#ui-assets): asset types inspired by standard web formats. Use them to structure and style UI.
- [**Tools and resources**](#ui-tools-and-resources) for creating and debugging your interfaces, and learning to use UI toolkit.

### UI system

The core of UI Toolkit is a retained-mode UI system based on recognized web technologies. It supports stylesheets, and dynamic and contextual event handling.

The UI system includes the following features:

- **[Visual tree](https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-VisualTree.html):** Defines every interface you build with the UI Toolkit. A visual tree is an object graph, made of lightweight nodes, that holds all the  elements in a window or panel.
- **[Controls](https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Controls.html):** A library of standard UI controls such as buttons, popups, list views, and color pickers. You can use them as-is, customize them, and create your own custom controls.
- **[Data binding system](https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Binding.html):** Links properties to the controls that modify their values.
- **[Layout Engine](https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-LayoutEngine.html):** A layout system based on the CSS Flexbox model. It positions elements based on layout and styling properties.
- **[Event System](https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-Events.html):** Communicates user interactions to elements; for example, input, touch and pointer interactions, drag and drop operations, and other event types. The system includes a dispatcher, a handler, a synthesizer, and a library of event types.
- **UI Renderer:** A rendering system built directly on top of Unity’s graphics device layer.
- **Runtime Support features:** Assets and components required to create runtime UI for games and applications.

### UI Assets

The UI Toolkit provides the following Asset types that you can use to build user interfaces in a way that's similar to how you develop web applications:

- **[UXML documents](https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-UXML.html):** Unity eXtensible Markup Language (UXML) is an HTML and XML inspired markup language that you use to define the structure of user interfaces and reusable UI templates. Although you can build interfaces directly in C# files, Unity recommends using UXML documents in most cases.
- **[Unity Style Sheets (USS)](https://docs.unity3d.com/2020.1/Documentation/Manual/UIE-USS.html):** Stylesheets allow you to apply visual styles and behaviors to user interfaces. They are similar to Cascading Style Sheets (CSS) used on the web, and support a subset of standard CSS properties. Although you can apply styles directly in C# files, Unity recommends using USS files in most cases.

### UI Tools and resources

The UI toolkit also includes the following tools and resources to help you create UI:

- **UI Debugger:** The UI debugger (menu: **Window &gt; UI Toolkit &gt; Debugger**) a diagnostic tool similar to a web browser's debugging view. Use it to explore a hierarchy of elements and get information about its underlying UXML structure and USS styles.
- **[UI Builder (package)](https://docs.unity3d.com/2020.1/Documentation/Manual/com.unity.ui.builder.html):** The UI Builder lets you visually create and edit UI Toolkit assets such as UXML and USS files. The UI Builder package is currently in preview. You can install it from the Package Manager window in the Unity Editor (menu: **Window > Package Manager**).
- **UI Samples:** The UI Toolkit includes a library of code samples for UI controls that you can view in the Editor (menu: **Window &gt; UI Toolkit &gt; Samples**).
