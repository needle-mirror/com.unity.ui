# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

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
