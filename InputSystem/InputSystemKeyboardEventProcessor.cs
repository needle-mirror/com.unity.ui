#if UNITY_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements.Collections;

namespace UnityEngine.UIElements.InputSystem
{
    internal class InputSystemKeyboardEventProcessor : IKeyboardEventProcessor
    {
        private readonly HashSet<Keyboard> m_DirtyKeyboards = new HashSet<Keyboard>();

        public void OnEnable()
        {
            UnityEngine.InputSystem.InputSystem.onEvent += OnEvent;
        }

        public void OnDisable()
        {
            UnityEngine.InputSystem.InputSystem.onEvent -= OnEvent;
        }

        void OnEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (eventPtr.handled || !(device is Keyboard keyboard))
                return;
            m_DirtyKeyboards.Add(keyboard);
        }

        private int m_LastPressedKey = -1;
        private float m_LastPressedTime;
        private Keyboard m_LastPressedKeyboard;

        void ReadKeyboardEvents(InputSystemEventSystem eventSystem, Keyboard keyboard)
        {
            var keys = keyboard.allKeys;
            var count = keys.Count;
            for (var i = 0; i < count; i++)
            {
                if (!keys[i].wasPressedThisFrame && !keys[i].wasReleasedThisFrame)
                    continue;

                var inputKeyCode = keys[i].keyCode;
                if (inputKeyCode == Key.PrintScreen && ShouldSkipBuggedPrintScreenKey(keyboard, keys[i]))
                    continue;

                var eventKeyCode = GetEventKeyCodeFromInputKey(inputKeyCode);
                if (eventKeyCode == KeyCode.None)
                    continue;

                var modifiers = GetEventModifiers(keyboard);
                char c = GetCharacterFromKeyCode(eventKeyCode, ref modifiers);

                if (keys[i].wasPressedThisFrame)
                {
                    m_LastPressedKey = i;
                    m_LastPressedTime = Time.unscaledTime;
                    m_LastPressedKeyboard = keyboard;

                    SendEvent(eventSystem, keyboard,  t => KeyDownEvent.GetPooled('\0', t.eventKeyCode, t.modifiers), (eventKeyCode, modifiers));
                    if (c != '\0')
                        SendEvent(eventSystem, keyboard, t => KeyDownEvent.GetPooled(t.c, KeyCode.None, t.modifiers), (c, modifiers));
                }
                if (keys[i].wasReleasedThisFrame)
                {
                    if (m_LastPressedKey == i)
                        m_LastPressedKey = -1;

                    SendEvent(eventSystem, keyboard,  t => KeyUpEvent.GetPooled('\0', t.eventKeyCode, t.modifiers), (eventKeyCode, modifiers));
                }
            }
        }

        //Bug: it seems Key.PrintScreen is always pressed after any of these keys... (seen on Windows)
        static Key[] s_KeysWithPrintScreenBug =
        {
            Key.LeftArrow, Key.RightArrow, Key.UpArrow, Key.DownArrow,
            Key.PageUp, Key.PageDown, Key.Home, Key.End, Key.Insert, Key.Delete
        };

        bool ShouldSkipBuggedPrintScreenKey(Keyboard keyboard, KeyControl printScreenKey)
        {
            var pressed = printScreenKey.wasPressedThisFrame;
            foreach (var keyCode in s_KeysWithPrintScreenBug)
            {
                var otherKey = keyboard[keyCode];
                if (pressed ? otherKey.wasPressedThisFrame : otherKey.wasReleasedThisFrame)
                    return true;
            }
            return false;
        }

        void CheckForRepeatedEvents(InputSystemEventSystem eventSystem)
        {
            if (m_LastPressedKey >= 0 && Time.unscaledTime >= m_LastPressedTime + eventSystem.m_RepeatDelay)
            {
                m_LastPressedTime += eventSystem.m_RepeatRate;

                var keyboard = m_LastPressedKeyboard;
                var keys = keyboard.allKeys;
                var i = m_LastPressedKey;

                if (!keys[i].isPressed)
                {
                    m_LastPressedKey = -1;
                    return;
                }

                var keyCode = GetEventKeyCodeFromInputKey(keys[i].keyCode);
                if (keyCode == KeyCode.None)
                {
                    m_LastPressedKey = -1;
                    return;
                }

                var modifiers = GetEventModifiers(keyboard);
                char c = GetCharacterFromKeyCode(keyCode, ref modifiers);

                SendEvent(eventSystem, keyboard,  t => KeyDownEvent.GetPooled('\0', t.keyCode, t.modifiers), (keyCode, modifiers));
                if (c != '\0')
                    SendEvent(eventSystem, keyboard, t => KeyDownEvent.GetPooled(t.c, KeyCode.None, t.modifiers), (c, modifiers));
            }
        }

        bool SendEvent<TArg>(InputSystemEventSystem eventSystem, Keyboard keyboard, Func<TArg, EventBase> evtFactory, TArg arg, InputEventPtr inputEventPtr)
        {
            if (SendEvent(eventSystem, keyboard, evtFactory, arg))
            {
                inputEventPtr.handled = true;
                return true;
            }
            return false;
        }

        bool SendEvent<TArg>(InputSystemEventSystem eventSystem, Keyboard keyboard, Func<TArg, EventBase> evtFactory, TArg arg)
        {
            var context = new InputSystemEventSystem.InputContext(null, keyboard);
            return eventSystem.SendFocusBasedEvent(context, evtFactory, arg);
        }

        static EventModifiers GetEventModifiers(Keyboard keyboard)
        {
            return (keyboard.altKey.isPressed ? EventModifiers.Alt : 0) |
                (keyboard.ctrlKey.isPressed ? EventModifiers.Control : 0) |
                (keyboard.shiftKey.isPressed ? EventModifiers.Shift : 0) |
                (keyboard.leftCommandKey.isPressed | keyboard.rightCommandKey.isPressed ? EventModifiers.Command : 0) |
                (keyboard.numLockKey.isPressed ? EventModifiers.Numeric : 0) |
                (keyboard.capsLockKey.isPressed ? EventModifiers.CapsLock : 0);
        }

        private static readonly Dictionary<Key, KeyCode> KeyToKeyCode = BuildKeyToKeyCodeDictionary();

        static Dictionary<Key, KeyCode> BuildKeyToKeyCodeDictionary()
        {
            var keyToKeyCode = new Dictionary<Key, KeyCode>();
            var keys = Enum.GetValues(typeof(Key));
            foreach (Key key in keys)
            {
                var name = Enum.GetName(typeof(Key), key) ?? "None";
                var codeName = name
                    .Replace("Digit", "Alpha")
                    .Replace("Numpad", "Keypad")
                    .Replace("Ctrl", "Control");
                if (Enum.TryParse(codeName, ignoreCase:true, out KeyCode keyCode))
                    keyToKeyCode[key] = keyCode;
            }

            // Manually force a few KeyCodes that have a different name or two names that share the same Key
            keyToKeyCode[Key.Enter] = KeyCode.Return;
            keyToKeyCode[Key.ContextMenu] = KeyCode.Menu;
            keyToKeyCode[Key.PrintScreen] = KeyCode.Print;

            Assert.AreEqual(Key.RightAlt, Key.AltGr);
            keyToKeyCode[Key.RightAlt] = KeyCode.RightAlt; // Same Key: AltGr

            Assert.AreEqual(Key.LeftCommand, Key.LeftApple);
            Assert.AreEqual(Key.LeftCommand, Key.LeftMeta);
            Assert.AreEqual(Key.LeftCommand, Key.LeftWindows);
            keyToKeyCode[Key.LeftCommand] = KeyCode.LeftCommand; // Same Key: LeftApple, LeftMeta, LeftWindows

            Assert.AreEqual(Key.RightCommand, Key.RightApple);
            Assert.AreEqual(Key.RightCommand, Key.RightMeta);
            Assert.AreEqual(Key.RightCommand, Key.RightWindows);
            keyToKeyCode[Key.RightCommand] = KeyCode.RightCommand; // Same Key: RightApple, RightMeta, RightWindows

            // Manually assign Keys that have no KeyCode equivalent
            keyToKeyCode[Key.OEM1] = KeyCode.None;
            keyToKeyCode[Key.OEM2] = KeyCode.None;
            keyToKeyCode[Key.OEM3] = KeyCode.None;
            keyToKeyCode[Key.OEM4] = KeyCode.None;
            keyToKeyCode[Key.OEM5] = KeyCode.None;
            keyToKeyCode[Key.IMESelected] = KeyCode.None;

            foreach (Key key in keys)
            {
                // All keys should now be uniquely set. If this Assert fails, please add new exceptions in the lines above
                Assert.IsTrue(keyToKeyCode.ContainsKey(key), Enum.GetName(typeof(Key), key));
            }

            return keyToKeyCode;
        }

        static KeyCode GetEventKeyCodeFromInputKey(Key key)
        {
            return KeyToKeyCode.Get(key, KeyCode.None);
        }

        static char GetCharacterFromKeyCode(KeyCode keyCode, ref EventModifiers modifiers)
        {
            var c = (char)keyCode;
            if (c == '\r')
                c = '\n';
            if (c == '\t' || c == '\n' || (c >= 32 && c < 256 && keyCode != KeyCode.Delete))
            {
                return (modifiers & EventModifiers.Shift) != 0 ? char.ToUpper(c) : c;
            }
            modifiers |= EventModifiers.FunctionKey;
            return '\0';
        }

        public void ProcessKeyboardEvents(InputSystemEventSystem eventSystem)
        {
            foreach (var keyboard in m_DirtyKeyboards)
            {
                ReadKeyboardEvents(eventSystem, keyboard);
            }
            m_DirtyKeyboards.Clear();

            CheckForRepeatedEvents(eventSystem);
        }
    }
}
#endif
