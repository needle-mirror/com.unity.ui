using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace UnityEngine.UIElements
{
    internal class VisualElementFactoryRegistry
    {
        private static Dictionary<string, List<IUxmlFactory>> s_Factories;

        internal static Dictionary<string, List<IUxmlFactory>> factories
        {
            get
            {
                if (s_Factories == null)
                {
                    s_Factories = new Dictionary<string, List<IUxmlFactory>>();
                    RegisterEngineFactories();
                    RegisterUserFactories();
                }

                return s_Factories;
            }
        }

        protected static void RegisterFactory(IUxmlFactory factory)
        {
            List<IUxmlFactory> factoryList;
            if (factories.TryGetValue(factory.uxmlQualifiedName, out factoryList))
            {
                foreach (IUxmlFactory f in factoryList)
                {
                    if (f.GetType() == factory.GetType())
                    {
                        //throw new ArgumentException($"A factory for the type {factory.GetType().FullName} was already registered");
                        return;
                    }
                }
                factoryList.Add(factory);
            }
            else
            {
                factoryList = new List<IUxmlFactory>();
                factoryList.Add(factory);
                factories.Add(factory.uxmlQualifiedName, factoryList);
            }
        }

        internal static bool TryGetValue(string fullTypeName, out List<IUxmlFactory> factoryList)
        {
            factoryList = null;
            return factories.TryGetValue(fullTypeName, out factoryList);
        }

        // Core UI Toolkit elements must be registered manually for both Editor and Player use cases.
        // For performance in the Player we want to avoid scanning any builtin Unity assembly with reflection.
        // Ideally a mechanism similar to the TypeCache in the Player would exist and remove the need for this.
        static void RegisterEngineFactories()
        {
            IUxmlFactory[] factories =
            {
                // Dummy factories. Just saying that these types exist and what are their attributes.
                // Used for schema generation.
                new UxmlRootElementFactory(),
                new UxmlTemplateFactory(),
                new UxmlStyleFactory(),
                new UxmlAttributeOverridesFactory(),

                // Real object instantiating factories.
                new Button.UxmlFactory(),
                new VisualElement.UxmlFactory(),
                new IMGUIContainer.UxmlFactory(),
                new Image.UxmlFactory(),
                new Label.UxmlFactory(),
                new RepeatButton.UxmlFactory(),
                new ScrollView.UxmlFactory(),
                new Scroller.UxmlFactory(),
                new Slider.UxmlFactory(),
                new SliderInt.UxmlFactory(),
                new MinMaxSlider.UxmlFactory(),
                new GroupBox.UxmlFactory(),
                new RadioButton.UxmlFactory(),
                new RadioButtonGroup.UxmlFactory(),
                new Toggle.UxmlFactory(),
                new TextField.UxmlFactory(),
                new TemplateContainer.UxmlFactory(),
                new Box.UxmlFactory(),
                new DropdownField.UxmlFactory(),
                new HelpBox.UxmlFactory(),
                new PopupWindow.UxmlFactory(),
#if (!UIE_PACKAGE) || UNITY_2021_1_OR_NEWER
                new ProgressBar.UxmlFactory(),
#endif
                new ListView.UxmlFactory(),
                new TwoPaneSplitView.UxmlFactory(),
                new TreeView.UxmlFactory(),
                new Foldout.UxmlFactory(),
                new BindableElement.UxmlFactory(),
                new TextElement.UxmlFactory(),
            };

            foreach (var factory in factories)
            {
                RegisterFactory(factory);
            }
        }

        internal static void RegisterUserFactories()
        {
            // In the Player, we filter assemblies to only introspect types of user assemblies
            // which will exclude Unity builtin assemblies (i.e. runtime modules).
#if !UNITY_EDITOR
            HashSet<string> userAssemblies = new HashSet<string>(ScriptingRuntime.GetAllUserAssemblies());
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (!(userAssemblies.Contains(assembly.GetName().Name + ".dll"))
                    // Exclude core UIElements factories which are registered manually
                    || assembly.GetName().Name == "UnityEngine.UIElementsModule")
                    continue;

                var types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (!typeof(IUxmlFactory).IsAssignableFrom(type)
                        || type.IsInterface
                        || type.IsAbstract
                        || type.IsGenericType)
                        continue;

                    var factory = (IUxmlFactory)Activator.CreateInstance(type);
                    RegisterFactory(factory);
                }
            }
#endif
        }
    }
}
