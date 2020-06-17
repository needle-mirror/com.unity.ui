using System;
using UnityEngine;

namespace Unity.UIElements.Editor.Samples
{
    [Serializable]
    internal class UIElementsSnippetAsset : ScriptableObject
    {
        public string text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        [SerializeField]
        private string m_Text;
    }
}
