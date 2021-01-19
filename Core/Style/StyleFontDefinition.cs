using System;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements
{
    /// <summary>
    /// Style value that can be either a <see cref="FontDefinition"/> or a <see cref="StyleKeyword"/>.
    /// </summary>
    public struct StyleFontDefinition : IStyleValue<FontDefinition>, IEquatable<StyleFontDefinition>
    {
        public FontDefinition value
        {
            get { return m_Keyword == StyleKeyword.Undefined ? m_Value : new FontDefinition(); }
            set
            {
                m_Value = value;
                m_Keyword = StyleKeyword.Undefined;
            }
        }
        /// <summary>
        /// The style keyword.
        /// </summary>
        public StyleKeyword keyword
        {
            get { return m_Keyword; }
            set { m_Keyword = value; }
        }

        /// <summary>
        /// Creates from either a <see cref="FontDefinition"/> or a <see cref="StyleKeyword"/>.
        /// </summary>
        public StyleFontDefinition(FontDefinition f)
            : this(f, StyleKeyword.Undefined)
        {}

        public StyleFontDefinition(Font f)
            : this(f, StyleKeyword.Undefined)
        {}

        /// <summary>
        /// Creates from either a <see cref="FontDefinition"/> or a <see cref="StyleKeyword"/>.
        /// </summary>
        public StyleFontDefinition(StyleKeyword keyword)
            : this(new FontDefinition(), keyword)
        {}

        internal StyleFontDefinition(object obj, StyleKeyword keyword)
            : this(FontDefinition.FromObject(obj), keyword)
        {
        }

        internal StyleFontDefinition(object obj)
            : this(FontDefinition.FromObject(obj), StyleKeyword.Undefined)
        {
        }

        internal StyleFontDefinition(Font f, StyleKeyword keyword)
            : this(FontDefinition.FromFont(f), keyword)
        {}


        internal StyleFontDefinition(GCHandle gcHandle, StyleKeyword keyword)
            : this(gcHandle.IsAllocated ? FontDefinition.FromObject(gcHandle.Target) : new FontDefinition(), keyword)
        {}

        internal StyleFontDefinition(FontDefinition f, StyleKeyword keyword)
        {
            m_Keyword = keyword;
            m_Value = f;
        }

        internal StyleFontDefinition(StyleFontDefinition sfd)
        {
            m_Keyword = sfd.keyword;
            m_Value = sfd.value;
        }

        private StyleKeyword m_Keyword;
        private FontDefinition m_Value;

        public static implicit operator StyleFontDefinition(StyleKeyword keyword)
        {
            return new StyleFontDefinition(keyword);
        }

        public static implicit operator StyleFontDefinition(FontDefinition f)
        {
            return new StyleFontDefinition(f);
        }

        public bool Equals(StyleFontDefinition other)
        {
            return m_Keyword == other.m_Keyword && m_Value.Equals(other.m_Value);
        }

        public override bool Equals(object obj)
        {
            return obj is StyleFontDefinition other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)m_Keyword * 397) ^ m_Value.GetHashCode();
            }
        }

        public static bool operator==(StyleFontDefinition left, StyleFontDefinition right)
        {
            return left.Equals(right);
        }

        public static bool operator!=(StyleFontDefinition left, StyleFontDefinition right)
        {
            return !left.Equals(right);
        }
    }
}
