# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.0-preview.12] - 2020-11-16
---

### Fixed

- Fixed cyclic dependency error with Input System package
- Fixed missing field exception in GraphView for `Vertex.idsFlags` field

## [1.0.0-preview.11] - 2020-11-13
---

### Fixed

- Fixed shader issues breaking Editor rendering in linear projects
- Revert API Change in Vertex.uv and fix related shader code

## [1.0.0-preview.10] - 2020-10-30
---

### Added

- Added an "inverted" property to Slider to allow users to reverse the display. Fixing at the same time the default direction on vertical sliders.
- Opening the UI Toolkit Debugger from the Game View now selects the first found PanelSettings Panel, if there is one.
- Added support for azimuthAngle, altitudeAngle, twist, and multiple Pen devices in EventSystem when using Input System package
- Improved renderer batching performance
- Added support for new USS text properties for TextElement
- Added Rich Text tags support in TextElement.text
- Added support for using Font Assets within UI Toolkit

### Changed

- Allow :root pseudo selector to target the element receiving the style sheet
- Updated menu item for Live Reload to be called "UI Toolkit Live Reload" to avoid user confusion.
- The DynamicAtlas system now tracks texture references and supports dynamic removals
- Enable dynamic atlassing of sprites.
- In runtime EventSystem when using the Input System package, converted multiple PointerMoveEvents and WheelEvents occuring during a single frame into single combined events, reducing impact on performance.
- UIDocuments are now sorted by a float field in the inspector ("sort order") and not automatically by GameObject hierarchy

### Fixed

- Fixed PanelSettings custom inspector applying sort order value in Edit Mode
- Fixed moving UIDocument under existing inactive UIDocument not calculating parent relationship right away
- Fixed assigning UXML to UIDocument for the first time not showing in Game View
- Fixed small artifacts and blurry text during runtime. (issue ID 1262249)
- Fixed zoom affects text wrapping. (issue ID 1279623)
- Fixed text not rendered correctly in linear. (issue ID 1215895)
- Fixed some chinese characters not shown in runtime. (issue ID 1271770)
- Changed TextField to match IMGUI behavior of select-all-on-mouse-up (issue ID 1179932)
- Changed EnumField+derivatives & BasePopupField-based classes to properly handle pointer down events (issue ID 1248669)
- Improved performance of visual element transform calculations
- Fixed tight sprite scale modes
- The UXML importer now throws a warning for special attributes with AttributeOverrides (issue ID 1280413)
- Binding operations are done asynchronouly to improve performance and avoid editor lockups (issue ID 1243897)
- Fixed runtime event system not changing focused panel when clicking on a control from another panel
- Fixed ScrollWheel event incorrectly moves the mouse for 1 frame when using runtime EventSystem, causing hovered element in ListView to flicker (issue ID 1280184)
- Fixed inconsistent ScrollWheel direction in EventSystem when using Input System package.
- Fixed arrow keys not working in the Slider/SliderInt (issue ID 1244539)
- Fixed Element not visisle after being hiden/shown in specific sequence (issue ID 1269414)
- Prevent NullRefException when providing textureless sprites as background (issue ID 1276866)
- Hover selector should not be working during DragEvents (issue ID 1180282)
- Fixed warning when trying to change the cursor (issue ID 1183813)
- Fixed dropdown menu from toolbar of windows glued to the left side of the screen on Mac (issue ID 1221212)
- Using PropertyField for Array types in the Inspector allows customized label value on the Foldout (issue ID 1214572)
- Fix mouse events in SceneView (issues ID 1210649, 1220685)
- Throw a proper argument null exception when using UQuery
- Disable the parsing of rich text tags in text fields (issue ID 1176895)
- Fixed panel color that was not being premultiplied before clearing the target texture background (issue ID 1220685)
- Fix wrong order of Attach/Detach events (issue ID 1269758)
- Fixed the ellipsis activated unpredictably with scaling factor other than 100% (issue ID 1268016)

### Removed

- Removed ability to reference drag events in the Player, since those are never sent
- Removed PanelRenderer component which was Obsolete

## [1.0.0-preview.9] - 2020-09-24
---

### Fixed

- Fixed ListView hovered item display style flickering continually when moving the mouse over it (issue ID 1274802)
- Fix compilation issue of Device Simulator package with UI Toolkit package (issue ID 1278014)

## [1.0.0-preview.8] - 2020-09-01
---

### Fixed

- Fixed compilation error in EventSystem when using Input System package in Unity 2020.1


## [1.0.0-preview.7] - 2020-08-28
---

### Fixed

- Fixed `TextField` focus not selecting text (issue ID 1262318)
- UI Document component no longer needs an Event System component to be displayed properly on Unity 2020.1 (issue ID 1261447)
- Fixed GraphView shader which was incompatible with the UI Toolkit package (issue ID 1270575)
- Fixed `Label` tooltip behavior when text value can change while elided (issue ID 1266210)
- Fixed `TextField` ignoring AltGr character combinations (issue ID 1261739)
- Fixed `LongField` SerializedProperty binding
- Removed internal usage of `Random` (issue ID 1188214)
- Fixed Package Manager window stylesheet error
- Fixed UXML preview rendered outside the preview zone (issue ID 1227277)
- Fixed layout being affected by the state of `RenderTexture.active` (issue ID 1268095)
- Fixed artifacts visible when resizing windows (issue ID 1218455)
- Fixed `IResolvedStyle` missing a definition for `backgroundImage`

### Changed

- No longer possible to change the Style Sheet on the `PanelSettings` inspector as the Runtime UI will not work without the default value for now. Debug access still available.
- Changed the blending equation to allow blending of the resulting RenderTexture

### Added

- Added ability to modify the dynamic atlas settings through the Panel Settings.
- Added support for new Input System package in Event System component (if the package isn’t installed, the legacy Input system is still used)
- UI shader nows uses dynamic texture slots to reduce batch breaking
- Support for using RenderTexture in `VisualElement.backgroundImage`
- Support for Sprite assets
- Live Reload feature: modified UXML and USS files get reloaded into the UI, both in Runtime and Editor Windows.
- Added `MeasureTextSize` method for `TextField`
- Added Clear Settings to the Panel Settings asset. If you used `PanelSettings.targetTexture`, you may want to enable `PanelSettings.clearColor`.


## [1.0.0-preview.6] - 2020-07-27
---

### Fixed

- Fixed issue with custom UXML factories not working in player builds (public issue ID 1265335)

## [1.0.0-preview.5] - 2020-07-18
---

### Fixed

- Removed 4 meta files for deleted folders

## [1.0.0-preview.4] - 2020-07-17
---

### Fixed

- Fixed rendering issues on DX11
- Fixed Slider's text field displayed value not properly updating when value is set through code
- Fixed WebGL rendering glitches caused by sub-range buffer updates
- Fixed a regression making Clickable manipulator react twice to click events
- Removed cyclic dependency with Input System package
- Fixed large mesh allocations that grew indefinitely
- Fix ScrollView scrollbars not showing when children are using position:absolute
- Removed memory allocation in PointerId.hoveringPointers
- Fix inline style length getter ignoring unit (public issue ID 1258959)
- Fixed Exception thrown with Sprite Editor window is opened after transition to play mode
- Fixed TextInputBaseField<T0>.isPasswordField not updating (public issue ID 1251000)
- Reduced memory footprint of a star rule in a stylesheet
- Fixed ScrollView content not fully scrollable when the content is aligned
- Fixed ScrollView.ScrollTo() not showing the while item when possible
- Fixed GLES warning about non-square matrices
- TextInfo in TextHandle is now allocated lazily (effective in Editor only)

### Changed

- Minimum compatible versions are now 2020.2.0a18 and 2020.1.0b14
- Renamed Unity.UIElements.asmdef to UnityEngine.UIElementsGameObjectModule.asmdef
- Renamed Unity.UIElements.Editor to UnityEditor.UIElementsModule.asmdef
- PanelSettings default scale mode changed to match UIBuilder's display more closely


### Added

- UI Toolkit package now contains the equivalent editor module from Unity core
- Added live reload of UIDocument's UXML file when in Edit Mode.

## [1.0.0-preview.3] - 2020-06-17
---

### Fixed

- Fixed warnings after importing package from the registry

## [1.0.0-preview.2] - 2020-06-16
---

### Fixed

- Fixed UI freeze after installing package
- Fixed package samples to work properly after resuming playmode
- Fixed Render to texture sample with URP
- Fixed crash with bleeding edge 2020.1

## [1.0.0-preview.1] - 2020-06-12
---

Initial release
