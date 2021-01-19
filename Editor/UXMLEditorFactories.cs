using System;
using System.Collections.Generic;
using UnityEditor.UIElements.Debugger;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements
{
    [InitializeOnLoad]
    internal class UXMLEditorFactories
    {
        private static readonly bool k_Registered;

        static UXMLEditorFactories()
        {
            if (k_Registered)
                return;

            k_Registered = true;

            IUxmlFactory[] factories =
            {
                // Primitives
                new TextElement.UxmlFactory(),

                // Compounds
                new PropertyControl<int>.UxmlFactory(),
                new PropertyControl<long>.UxmlFactory(),
                new PropertyControl<float>.UxmlFactory(),
                new PropertyControl<double>.UxmlFactory(),
                new PropertyControl<string>.UxmlFactory(),

                new VisualSplitter.UxmlFactory(),

                // Toolbar
                new Toolbar.UxmlFactory(),
                new ToolbarButton.UxmlFactory(),
                new ToolbarToggle.UxmlFactory(),
                new ToolbarSpacer.UxmlFactory(),
                new ToolbarMenu.UxmlFactory(),
                new ToolbarSearchField.UxmlFactory(),
                new ToolbarPopupSearchField.UxmlFactory(),
                new ToolbarBreadcrumbs.UxmlFactory(),
                // Bound
                new PropertyField.UxmlFactory(),
                new InspectorElement.UxmlFactory(),

                // Fields
                new FloatField.UxmlFactory(),
                new DoubleField.UxmlFactory(),
                new IntegerField.UxmlFactory(),
                new LongField.UxmlFactory(),
                new CurveField.UxmlFactory(),
                new ObjectField.UxmlFactory(),
                new ColorField.UxmlFactory(),
                new EnumField.UxmlFactory(),
                new MaskField.UxmlFactory(),
                new LayerMaskField.UxmlFactory(),
                new LayerField.UxmlFactory(),
                new TagField.UxmlFactory(),
                new GradientField.UxmlFactory(),
                new EnumFlagsField.UxmlFactory(),
                new Hash128Field.UxmlFactory(),

#if !UNITY_2021_1_OR_NEWER
                new ProgressBar.UxmlFactory(),
#endif
                // Compounds
                new RectField.UxmlFactory(),
                new Vector2Field.UxmlFactory(),
                new Vector3Field.UxmlFactory(),
                new Vector4Field.UxmlFactory(),
                new BoundsField.UxmlFactory(),


                new RectIntField.UxmlFactory(),
                new Vector2IntField.UxmlFactory(),
                new Vector3IntField.UxmlFactory(),
                new BoundsIntField.UxmlFactory(),

                new EventTypeSelectField.UxmlFactory()
            };

            foreach (IUxmlFactory factory in factories)
            {
                VisualElementFactoryRegistry.RegisterFactory(factory);
            }

            // Discover packages and user factories.
            HashSet<string> userAssemblies = new HashSet<string>(ScriptingRuntime.GetAllUserAssemblies());
            var types = TypeCache.GetTypesDerivedFrom<IUxmlFactory>();
            foreach (var type in types)
            {
                if (type.IsAbstract
                    || !userAssemblies.Contains(type.Assembly.GetName().Name + ".dll")
                    || !typeof(IUxmlFactory).IsAssignableFrom(type)
                    || type.IsInterface
                    || type.IsGenericType
                    || type.Assembly.GetName().Name == "UnityEngine.UIElementsModule"
                    || type.Assembly.GetName().Name == "UnityEditor.UIElementsModule")
                    continue;

                var factory = (IUxmlFactory)Activator.CreateInstance(type);
                VisualElementFactoryRegistry.RegisterFactory(factory);
            }
        }
    }
}
