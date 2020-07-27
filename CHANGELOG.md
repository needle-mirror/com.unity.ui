# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

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
