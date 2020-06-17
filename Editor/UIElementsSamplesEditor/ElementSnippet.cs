using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements.Samples;
using UnityEngine.UIElements;

namespace Unity.UIElements.Editor.Samples
{
    internal class ElementSnippet<T> where T : new()
    {
        private static readonly string s_CodeContainerClassName = "unity-snippet-code__container";
        private static readonly string s_CodeTitleClassName = "unity-snippet-code__title";
        private static readonly string s_CodeClassName = "unity-snippet-code__code";
        private static readonly string s_CodeLineNumbersClassName = "unity-snippet-code__code-line-numbers";
        private static readonly string s_CodeTextClassName = "unity-snippet-code__code-text";
        private static readonly string s_CodeInputClassName = "unity-snippet-code__input";
        private static readonly string s_CodeCodeOuterContainerClassName = "unity-snippet-code__code_outer_container";
        private static readonly string s_CodeCodeContainerClassName = "unity-snippet-code__code_container";

        private static readonly string s_DemoContainerClassName = "unity-samples-explorer__demo-container";
        private static readonly string s_SnippetsContainer = "unity-samples-explorer__snippets-container";

        private static readonly string s_SampleBeginTag = "/// <sample>";
        private static readonly string s_SampleEndTag = "/// </sample>";

        private static readonly string s_CodeAssetsPath = "Packages/com.unity.ui/Editor/UIElementsSamplesEditor/Snippets/";
        private static readonly string s_UXMLAssetsPath = "Packages/com.unity.ui/PackageResources/Snippets/UXML/";
        private static readonly string s_USSAssetsPath = "Packages/com.unity.ui/PackageResources/Snippets/StyleSheets/";

        internal virtual void Apply(VisualElement container)
        {
        }

        private static string ProcessCSharp(string text)
        {
            const string badEndLine = "\r\n";
            const string goodEndLine = "\n";

            text = text.Replace(badEndLine, goodEndLine);

            int startIndex = text.IndexOf(s_SampleBeginTag);
            if (startIndex < 0)
                return text;

            int endIndex = text.IndexOf(s_SampleEndTag);
            if (endIndex < 0)
                return text;

            var actualStartIndex = startIndex + s_SampleBeginTag.Length;

            string leadingWhiteSpace = "";
            for (int i = actualStartIndex; i < endIndex; ++i)
            {
                if (char.IsWhiteSpace(text[i]))
                    continue;

                leadingWhiteSpace = text.Substring(actualStartIndex, i - actualStartIndex);
                actualStartIndex = i;
                break;
            }

            for (int i = endIndex - 1; i > actualStartIndex; --i)
            {
                if (char.IsWhiteSpace(text[i]))
                    continue;

                endIndex = i + 1;
                break;
            }

            text = text.Substring(actualStartIndex, endIndex - actualStartIndex);
            text = text.Replace(leadingWhiteSpace, goodEndLine);

            return text;
        }

        private static VisualElement CreateSnippetCode(string title, string path)
        {
            var container = new VisualElement();
            container.AddToClassList(s_CodeContainerClassName);

            var titleLabel = new Label(title);
            titleLabel.AddToClassList(s_CodeTitleClassName);
            container.Add(titleLabel);

            var text = "";
            // Some controls don't support UXML.
            try
            {
                text = File.ReadAllText(path);
            }
            catch (FileNotFoundException)
            {
                return null;
            }

            if (path.EndsWith(".cs")) // C#
                text = ProcessCSharp(text);

            var lineCount = text.Count(x => x == '\n') + 1;
            string lineNumbersText = "";
            for (int i = 1; i <= lineCount; ++i)
            {
                if (!string.IsNullOrEmpty(lineNumbersText))
                    lineNumbersText += "\n";

                lineNumbersText += i.ToString();
            }

            var lineNumbers = new Label(lineNumbersText);
            lineNumbers.RemoveFromClassList(TextField.ussClassName);
            lineNumbers.AddToClassList(s_CodeClassName);
            lineNumbers.AddToClassList(s_CodeLineNumbersClassName);
            lineNumbers.AddToClassList(s_CodeInputClassName);

            var code = new TextField(TextField.kMaxLengthNone, true, false, char.MinValue) { value = text };
            code.isReadOnly = true;
            code.RemoveFromClassList(TextField.ussClassName);
            code.AddToClassList(s_CodeClassName);
            code.AddToClassList(s_CodeTextClassName);

            var codeInput = code.Q(className: TextField.inputUssClassName);
            codeInput.AddToClassList(s_CodeInputClassName);

            var codeOuterContainer = new VisualElement();
            codeOuterContainer.AddToClassList(s_CodeCodeOuterContainerClassName);
            container.Add(codeOuterContainer);

            var codeContainer = new VisualElement();
            codeContainer.AddToClassList(s_CodeCodeContainerClassName);
            codeOuterContainer.Add(codeContainer);

            codeContainer.Add(lineNumbers);
            codeContainer.Add(code);

            return container;
        }

        internal static VisualElement Create(UIElementsSamples.SampleTreeItem item)
        {
            var snippet = new T() as ElementSnippet<T>;
            var tname = typeof(T).Name;

            var container = new VisualElement();
            container.AddToClassList(s_DemoContainerClassName);

            var csAssetPath = s_CodeAssetsPath + tname + ".cs";
            var ussAssetPath = s_USSAssetsPath + tname + ".uss";
            var uxmlAssetPath = s_UXMLAssetsPath + tname + ".uxml";

            var csSnippet = CreateSnippetCode("C#", csAssetPath);

            var ussSnippet = CreateSnippetCode("USS", ussAssetPath);
            var styleSheet = EditorGUIUtility.Load(ussAssetPath) as StyleSheet;
            container.styleSheets.Add(styleSheet);

            var uxmlSnippet = CreateSnippetCode("UXML", uxmlAssetPath);
            if (uxmlSnippet != null)
            {
                var visualTree = EditorGUIUtility.Load(uxmlAssetPath) as VisualTreeAsset;
                visualTree.CloneTree(container);
            }

            snippet.Apply(container);

            var scrollView = new ScrollView();
            scrollView.AddToClassList(s_SnippetsContainer);
            scrollView.Add(csSnippet);
            scrollView.Add(ussSnippet);

            if (uxmlSnippet != null)
                scrollView.Add(uxmlSnippet);

            var panel = new VisualElement();
            panel.Add(container);
            panel.Add(scrollView);

            return panel;
        }
    }
}
