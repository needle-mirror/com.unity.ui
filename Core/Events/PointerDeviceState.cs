using System;

namespace UnityEngine.UIElements
{
    static class PointerDeviceState
    {
        [Flags]
        internal enum LocationFlag
        {
            None = 0,
            // The location of the pointer was outside of the panel **when the location was set**. The window may have grown since then.
            // We need to know this to avoid elements to become hovered when the window grows, due to an out of date pointer location.
            OutsidePanel = 1,
        }

        private struct PointerLocation
        {
            // Pointer position in panel space. If Panel is null, position in screen space.
            internal Vector2 Position { get; private set; }
            // The Panel that handles events for this pointer.
            internal IPanel Panel { get; private set; }
            internal LocationFlag Flags { get; private set; }

            internal void SetLocation(Vector2 position, IPanel panel)
            {
                Position = position;
                Panel = panel;
                Flags = LocationFlag.None;

                if (panel == null || !panel.visualTree.layout.Contains(position))
                {
                    Flags |= LocationFlag.OutsidePanel;
                }
            }
        }

#if UNITY_EDITOR
        private static PointerLocation[] s_EditorPointerLocations = new PointerLocation[PointerId.maxPointers];
#endif
        private static PointerLocation[] s_PlayerPointerLocations = new PointerLocation[PointerId.maxPointers];
        private static int[] s_PressedButtons = new int[PointerId.maxPointers];

        // For test usage
        internal static void Reset()
        {
            for (var i = 0; i < PointerId.maxPointers; i++)
            {
#if UNITY_EDITOR
                s_EditorPointerLocations[i].SetLocation(Vector2.zero, null);
#endif
                s_PlayerPointerLocations[i].SetLocation(Vector2.zero, null);
                s_PressedButtons[i] = 0;
            }
        }

        public static void SavePointerPosition(int pointerId, Vector2 position, IPanel panel, ContextType contextType)
        {
            switch (contextType)
            {
                case ContextType.Editor:
                default:
#if UNITY_EDITOR
                    s_EditorPointerLocations[pointerId].SetLocation(position, panel);
                    break;
#endif

                case ContextType.Player:
                    s_PlayerPointerLocations[pointerId].SetLocation(position, panel);
                    break;
            }
        }

        public static void PressButton(int pointerId, int buttonId)
        {
            Debug.Assert(buttonId >= 0);
            Debug.Assert(buttonId < 32);
            s_PressedButtons[pointerId] |= (1 << buttonId);
        }

        public static void ReleaseButton(int pointerId, int buttonId)
        {
            Debug.Assert(buttonId >= 0);
            Debug.Assert(buttonId < 32);
            s_PressedButtons[pointerId] &= ~(1 << buttonId);
        }

        public static void ReleaseAllButtons(int pointerId)
        {
            s_PressedButtons[pointerId] = 0;
        }

        public static Vector2 GetPointerPosition(int pointerId, ContextType contextType = ContextType.Editor)
        {
            switch (contextType)
            {
                case ContextType.Editor:
                default:
#if UNITY_EDITOR
                    return s_EditorPointerLocations[pointerId].Position;
#endif

                case ContextType.Player:
                    return s_PlayerPointerLocations[pointerId].Position;
            }
        }

        public static IPanel GetPanel(int pointerId, ContextType contextType)
        {
            switch (contextType)
            {
                case ContextType.Editor:
                default:
#if UNITY_EDITOR
                    return s_EditorPointerLocations[pointerId].Panel;
#endif

                case ContextType.Player:
                    return s_PlayerPointerLocations[pointerId].Panel;
            }
        }

        static bool HasFlagFast(LocationFlag flagSet, LocationFlag flag)
        {
            return (flagSet & flag) == flag;
        }

        public static bool HasLocationFlag(int pointerId, ContextType contextType, LocationFlag flag)
        {
            switch (contextType)
            {
                case ContextType.Editor:
                default:
#if UNITY_EDITOR
                    return HasFlagFast(s_EditorPointerLocations[pointerId].Flags, flag);
#endif

                case ContextType.Player:
                    return HasFlagFast(s_PlayerPointerLocations[pointerId].Flags, flag);
            }
        }

        public static int GetPressedButtons(int pointerId)
        {
            return s_PressedButtons[pointerId];
        }

        internal static bool HasAdditionalPressedButtons(int pointerId, int exceptButtonId)
        {
            return (s_PressedButtons[pointerId] & ~(1 << exceptButtonId)) != 0;
        }
    }
}
