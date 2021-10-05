# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.0-preview.18] - 2021-10-05

### Fixed
- Fixed default.tss.asset failing to load while the UI Toolkit package is being imported
- Fixed asset dependencies preventing references to TSS resources from UXML inline styles (case 1357255)
- Fixed ClickEvent unpredictably not being sent on Android and iOS when the touch sequence spans over multiple update frames
- Fixed binary compatibility error when using GraphView and minimaps
- Fixed compbatibility error with Graph Tools Foundation package
- Fixed inconsistencies of underline/strikethrough text visuals
- Fixed exception thrown when repainting a panel that uses a destroyed texture
- Fixed UIDebugger element picking in the Game View on mac
- Fixed Tab key being ignored on Mac builds
- Fixed Text Shadow Color being resetted in the UI Builder after reopening a project

### Changed
- Removed default auto assignment of PanelSettings's default runtime theme
- Popupfields choices are now accessible through c# property


## [1.0.0-preview.17] - 2021-08-26
---

### Fixed

- Removed dependency on moq
- Fixed corrupted text after upgrading/removing/adding a new package. (case 1358577)
- Fixed some NullReferenceException issues related to text
- Fixed ObjectField label alignment issues (case 1320031)
- Drag and drop in ListView will not bind negative index. (case 1319543)
- Submit event on a ListView focuses in the content to allow keyboard navigation. (case 1311688)
- Fixed selection on pointer up on mobile to allow touch scrolling. (case 1312139)

## [1.0.0-preview.16] - 2021-08-12
---

### Fixed

- Fixed Editor fonts missing when using System Fonts (case 1357686)
- Removed warning on console about .meta for folder that doesn't exist (case 1357693)
- Fixed Editor fonts get messed up after saving or building player (case 1357698)

## [1.0.0-preview.15] - 2021-07-23
---

### Added

- Added new controls (RadioButton, RadioButtonGroup, DropdownField) to the UITK Samples and UI Builder Standard Library (case 1310158)
- Added visualTreeAssetSource property to VisualElement to allow identifying the VisualTreeAsset a visual tree was cloned from
- Added multi-language support
- Added Textcore package as a dependency

### Changed

- Deprecated OnKeyDown method in ListView. Use the event system instead, see SendEvent.
- Optimized some data access for Live Reload feature
- Removed additional overhead of attaching to panel for Live Reload when the option is turned off, improving performance in loading VisualTreeAssets
- Added a new RuntimeDefault theme with less overhead for runtime usage
- Improved UI Toolkit event debugger. Improvements include optimizations, adjustable UI, settings, event and callback filtering, event replay
- Buttons are now focusable
- Improved USS validation to support more complex properties

### Fixed

- Fixed regression on the styling of the ProgressWindow's progress bars, removed usage of images in progress bar (case 1297045)
- Prevented infinite recursion using UIElements with nested property drawers (case 1292133)
- Fixes performance of Line Renderer list of positions when its size is big enough to require scroll on the Inspector view (case 1296193)
- Fixed playmode tint applied twice on text (case 1283050)
- Fixed InspectorElement enabled state in EditorElement.Reinit (case 1221162)
- Fixed window creator on linux caused a null reference and the new editor window was displayed corrupted (case 1295140)
- Fix add component button is overlapping on Default Material after undoing added component (case 1279830)
- Fixed bug where multiple elements could be shown as being focused simultaneously due to delayed focus events not being well tracked when there is more than 1 of them in the queue (case 1154256)
- Fixed VisualElement contains "null" stylesheet after deleted uss file from project (case 1290271)
- Capturing mouse/pointer during a click will cancel that click (case 1283061)
- Changed EnumField+derivatives & BasePopupField-based classes to properly handle pointer down events (case 1248669)
- Fixed PropertyField now has no label displayed if an empty string is passed as the label argument. If a null value is passed, the label will still be displayed using the localized display name, like before (case 1293580)
- Elements hover and focused states are not properly reset when attaching to a new hierarchy (case 1287198)
- Fixed older GraphView still relying on old idsFlags field
- Patched graphics issue with Intel drivers causing bad stretching and clipping in the editor (case 1309555)
- Fixed Graph Inspector window sticks to the mouse cursor on changing any dropdown value in Node settings (case 1297002)
- Fixed scrollbar showing for no meaningful reason when the content of a scrollview is almost equal to the size of the scrollview (case 1297053)
- Fix undocking and redocking a shadergraph and then selecting a property on the blackboard throws UIElement errors (case 1302295)
- Fixed ArgumentException is thrown when the PropertyField is bind to the BuildTarget enum Popup/Dropdown (Enum-compatible) fields now gracefully handle unselected/invalid values (case 1304581)
- Fixed errors caused by the use of the current culture to parse UXML attributes as float/double (case 1308180)
- Fix Custom UIElements Inspector is editable when GameObject has its Flags set to NotEditable (case 1306242)
- Fixed cases where errors during static initialization of style resources would make the inspector window blank (case 1294463)
- Fixed cases where errors during static initialization of style resources would cause an infinite window layout loading error loop (case 1309276)
- Fixed issue where SerializedObject bindings use cases would lead to editor crashes (case 1305198)
- InspectorElement now correctly supports rebinding when used outside of the InspectorWindow (case 1299036)
- Fixed empty name showing up in Memory Profiler for one of the UI Toolkit internal render texture (case 1307441)
- Fixed custom editor not showing as disabled for read-only inspectors (case 1299346)
- Fix PropertyField shows that it is actively selected when it has been disabled while being actively selected (case 1306238)
- Fixed Label Element is not resized when Display Style is changed from None to Flex (case 1293761)
- Fixed an Error in the asset management of the StyleSheet that would show up in a build
- Fixed PropertyField created from UXML missing its default label (case 1309780)
- Fixed NullReferenceException on turning UI Toolkit Live Reload on/off on maximized docked Search window (case 1312367)
- GenericDropdownMenu no longer needs to be reinstanced on every use (case 1308433)
- Changing choices programmatically after setting the value will show the correct selection, in RadioButtonGroup
- Fixed exception on Text Settings coming from uninitialized Line Breaking Rules when text wrap is enabled (case 1305483)
- Fixed ellipsis showing up for no reason on UI Toolkit labels (case 1291452)
- Missing theme style sheet on PanelSettings now gets logged to console
- Fixed highlighter positioning and draw order (case 1174816)
- Fixed bug where runtime cursor should not be reset unless it was overridden (case 1292577)
- Fixed the inability to launch the editor in clamped GLES 3.0 and 3.1 mode. Also, it is now possible to use UIToolkit on GLES 3.0 and 3.1 devices that do not support float render textures (case 1311845)
- Fixed InputSystem fails to store ElementUnderPointer when a VisualElement is moving, creating flickering hover feedback (case 1306526)
- Fixes silent crash that can be caused by recursive onSelectionChange callback in ListView (case 1315473)
- KeyboardNavigationManipulator does not trigger the callback if operation is None (case 1314063)
- Fixed SVG triangle clipping issue (case 1288416)
- Fixed clipping with large rects when under a group transform (case 1296815)
- Fixed directional navigation bug where some elements could be skipped during horizontal navigation. Improved choice of next element when multiple candidates are valid (case 1298017)
- Fixed EventSystem using InputSystem package sometimes sending large amounts of PointerMoveEvents during a single frame (case 1295751)
- Fixed wrong runtime touch event coordinates on panels with scaling
- Fixed NullReferenceException with using TrackPropertyValue on BindingExtensions for ExposedReference and Generic serialized property types (case 1312147)
- Added missing styling definition for dropdown in runtime stylesheet (case 1314322)
- Fixed the hover and pressed color of buttons in the Runtime theme (case 1316380)
- Fixed an issue where changing the size of a TwoPaneSplitView would not resize its content (case 1313077)
- Fixed multiple errors appear after modifying ScriptableObject's array indirectly (case 1303188)
- Panels instantiated by PanelSettings assets now ordered deterministically when their sort order have the exact same value
- The TwoPaneSplitView's view data will now be persisted when the viewDataKey is set (case 1314083)
- Fixed the naming of Game Objects created using the "UI Toolkit > UI Document" creation menu on Hierarchy View to be incremental (case 1318889)
- Improved readability of USS import error/warnings (case 1295682)
- Fixed nested UI Document allowed changing the Panel Settings once (case 1315242)
- Fixed left-click not opening EnumField after using ContextMenu on MacOS (case 1311011)
- The label of a focused Foldout now has its color changed (case 1311200)
- Fixed a bug in the test FinalAlphaValue on device with different DPI (case 1314061)
- Scroll bars now use display instead of visibility to avoid scroll bars being visible when parent visibility is set to false (case 1297886)
- Prevented clicks from passing through runtime panels if they weren't used (case 1314140)
- Fixed custom element UXML factory not picked up in pre-compiled DLL (case 1316913)
- Match text colors of UITK label and UITK field label with IMGUI label and IMGUI prefix label respectively. It also fixes the text color of buttons (case 1310581)
- Fixed Tooltips of "Panel Settings" Asset always appear at the center of the Inspector window (case 1319166)
- Cleaned up the Theme menu (case 1318600)
- Fixed view data persistence not working inside custom inspectors that use UI Toolkit (case 1311181)
- Fixed rebuild logic on inspectors with culled elements (case 1324058)
- Fixed the focus handling so elements not displayed in the hierarchy cannot be focused (case 1324376)
- Fixed default clicking scroll amount in ScrollView (case 1306562)
- Fixed focus outline for the following controls: CurveField, GradientField, EnumField/PopupField (and derivatives), RadioButton (choice), ObjectField (when hovered) (case 1324381)
- Removed an extra step from the RadioButtonGroup focus navigation (case 1324373)
- Prevented reload of windows that will break when turning Live Reload on/off (case 1318930)
- Child's reference is not renamed in parent UXML after renaming the child UXML file (case 1319903)
- Set UIDocument's execution order to -100 to ensure root visual element is created when user's OnEnable runs
- Fixed dynamic atlas not being regenerated after downloading a texture from the cache server (case 1333693)
- Fixed performance test issue for UI Toolkit renderer repaints (case 1337832)
- Fixed TextureId leak that could occur when a Panel was disposed or when the graphics device reloads (case 1336881)
- Fixed scissor clipping on hidden elements (case 1340827)
- Fixed Image class alignment issue on non-standard DPIs (case 1330817)
- Fix incorrect pointer enter events and pointer leave events in playmode using the DefaultEventSystem (case 1313220)
- Fix element sometimes entering hover state when window is resized (case 1290545)
- Fixed clipping issue with nested scrollviews (case 1335094)
- Fixed precision errors in gamma-linear conversions (case 1317742)
- Fixed clipping of the content of a mask element whose size is zero (case 1320182)
- Fixed issue that caused non-square dynamic atlases to be recreated every frame (case 1327689)
- Fixed GroupTransform that was triggering asserts when the nested masking fallback was used (case 1328734)
- Fixed corruption of the stencil buffer caused by misplaced geometry used to pop masks (case 1332741)
- Value Change Callbacks for bound fields now happen after the value is applied to the target object (case 1321156)
- Fixed issue with inspector fields failing to get focused when clicked depending on neighboring fields (case 1335344)
- Fixed disabled state not showing properly after hierarchical changes were applied (case 1321042)
- Fixed UI Toolkit package's InputSystemEventSystem component ceasing to feed events to UI Toolkit when a script is modified or reimported while in play mode, triggering a reload of the assemblies (case case 1324337)
- Fixed Button remains in hover state after a touch (case 1326493)
- Fixed invocation of callback event handlers for Click, Middle Click, Right Click, Point and Scroll Wheel actions configured using the Button type instead of the default (PassThrough) with the new Input System (case 1308116)
- Fixed undo of a change of "Sort Order" field value for a UI Document (case 1337070)
- Fixed an issue that was causing the content of a GroupTransform element to be clipped with the incorrect clipping rect (case 1328740)
- Value Change Callbacks for bound fields now happen after the value is applied to the target object (case 1324728)
- Nested InspectorElement for a different target and Editor type are now ignored when binding their parent Editor (case case 1336093)
- An element can now receive a PointerUpEvent notification when the mouse position is outside of the game view (case 1306631)
- An element with mouse capture enabled now receives runtime mouse events even when the mouse position is outside the element (case 1328035)
- When loading a project that contains a dialog that is embedded in the editor, the dialog's CreateGUI callback is invoked after the Awake and OnEnable callbacks (case 1326173)
- Fixed read only fields mouse dragger (case 1337002)
- Fixed the missing Unicode arrow on ShaderGraph Transform Node (case case 1333774)
- Fix corrupted atlas for Inter (case 1330758)
- Fixed error when moving cursor over a reorderable ListView after clicking a context menu (case 1292065)
- Fix exception in ListView when pressing page up key after hitting navigation keys (case 1324806)
- Changed Image scale mode to the more intuitive default, being ScaleMode.ScaleToFit instead of ScaleMode.ScaleAndCrop. Changing scale mode will also trigger a repaint (case 1215470)
- Fixed the theme selector ordering when importing uss files (case 1317035)
- Fixed styling of runtime ScrollView (case 1323488)
- Fixed ScrollView horizontal scrolling via scroll wheel does not scroll (case 1328220)
- Fixed Toolbar shrinking when there is another element filling the parent container (case 1330415)
- Fixed runtime horizontal ScrollView elements alignment (case 1328206)
- Ensured Theme Style Sheet assignment for new and existing PanelSettings assets (case 1340472)
- Fixed wrong mouse position on events when a UI Toolkit element has mouse capture, the mouse is outside the element's editor window and that window doesn't have the active mouse capture from the OS (case 1342115)
- Fixed pointer events not working correctly when multiple UI Documents have different Screen Match values (case 1341135)
- Fixed samples contained within the package

## [1.0.0-preview.14] - 2021-01-19
---

### Added

- UQuery: Enumerator support allows for foreach iteration with no or minimal GC allocations

### Fixed

- UI Toolkit Debugger: Allowed debugger to pick elements from Game View in Edit Mode (and not only in Play Mode like before)
- UI Toolkit Debugger: Overlay, "Show Layout" now work for Game View in Edit Mode (and not only in Play Mode like before)
- UQuery: Query<VisualElement> now correctly returns all elements (case 1288531)
- Not allowing text changes on text input field when they are disabled (case 1286143)
- Fixed Images failing to load if using the url("") syntax while having multiple resolution variants (case 1293843)
- Fixed ListView item selection through PointerMoveEvent, for example when holding right-click down while clicking (case 1287031)
- Fixed bug where multiple elements could be shown as being focused at the same time due to delayed focus events not being well tracked when there is more than 1 of them in the queue (case 1154256)
- Fixed wrong addressing of dynamic transforms when new atlas slot is used (case 1293058)
- PropertyField now able to have no label displayed if an empty string is passed as the label argument. Note that if a null value is passed, the label will still be displayed using the localized display name, like before (case 1293580)
- Fixed a bug where users were able to drag slider outside its container when a text field was present
- Fixed an issue where sometimes style assets loading would fail during initialization


## [1.0.0-preview.13] - 2020-11-27
---

### Fixed

- Fixed text-shadow and strokes on button text
- Fixed InternalsVisibleTo for com.unity.ui.builder 1.0.0-preview.10


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
