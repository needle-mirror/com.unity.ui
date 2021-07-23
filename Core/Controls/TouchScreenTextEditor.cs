namespace UnityEngine.UIElements
{
    internal class TouchScreenTextEditorEventHandler : TextEditorEventHandler
    {
        private IVisualElementScheduledItem m_TouchKeyboardPoller = null;
        private VisualElement m_LastPointerDownTarget;

#if UNITY_IOS || UNITY_ANDROID
        static TouchScreenKeyboard s_KeyboardOnScreen;
#endif

        public TouchScreenTextEditorEventHandler(TextEditorEngine editorEngine, ITextInputField textInputField)
            : base(editorEngine, textInputField) {}

        void PollTouchScreenKeyboard()
        {
            if (TouchScreenKeyboard.isSupported && !TouchScreenKeyboard.isInPlaceEditingAllowed)
            {
                if (m_TouchKeyboardPoller == null)
                {
                    m_TouchKeyboardPoller = (textInputField as VisualElement)?.schedule.Execute(DoPollTouchScreenKeyboard).Every(100);
                }
                else
                {
                    m_TouchKeyboardPoller.Resume();
                }
            }
        }

        void DoPollTouchScreenKeyboard()
        {
            if (TouchScreenKeyboard.isSupported && !TouchScreenKeyboard.isInPlaceEditingAllowed)
            {
                if (textInputField.editorEngine.keyboardOnScreen != null)
                {
#if UNITY_IOS || UNITY_ANDROID
                    if (s_KeyboardOnScreen != textInputField.editorEngine.keyboardOnScreen)
                    {
                        textInputField.editorEngine.keyboardOnScreen = null;
                        m_TouchKeyboardPoller.Pause();
                        return;
                    }
#endif
                    textInputField.UpdateText(textInputField.CullString(textInputField.editorEngine.keyboardOnScreen.text));

                    if (!textInputField.isDelayed)
                    {
                        textInputField.UpdateValueFromText();
                    }

                    if (textInputField.editorEngine.keyboardOnScreen.status != TouchScreenKeyboard.Status.Visible)
                    {
                        textInputField.editorEngine.keyboardOnScreen = null;
                        m_TouchKeyboardPoller.Pause();

                        if (textInputField.isDelayed)
                        {
                            textInputField.UpdateValueFromText();
                        }
                    }
                }
            }
        }

        public override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);

            if (editorEngine.keyboardOnScreen != null)
                return;

            if (!textInputField.isReadOnly && evt.eventTypeId == PointerDownEvent.TypeId())
            {
                textInputField.CaptureMouse();
                m_LastPointerDownTarget = evt.target as VisualElement;
            }
            else if (!textInputField.isReadOnly && evt.eventTypeId == PointerUpEvent.TypeId())
            {
                textInputField.ReleaseMouse();
                if (m_LastPointerDownTarget == null || !m_LastPointerDownTarget.worldBound.Contains(((PointerUpEvent)evt).position))
                    return;

                m_LastPointerDownTarget = null;

                textInputField.SyncTextEngine();
                textInputField.UpdateText(editorEngine.text);

                editorEngine.keyboardOnScreen = TouchScreenKeyboard.Open(textInputField.text,
                    TouchScreenKeyboardType.Default,
                    true, // autocorrection
                    editorEngine.multiline,
                    textInputField.isPasswordField);

#if UNITY_IOS || UNITY_ANDROID
                s_KeyboardOnScreen = editorEngine.keyboardOnScreen;
#endif

                if (editorEngine.keyboardOnScreen != null)
                {
                    PollTouchScreenKeyboard();
                }

                // Scroll offset might need to be updated
                editorEngine.UpdateScrollOffset();
                evt.StopPropagation();
            }
        }
    }
}
