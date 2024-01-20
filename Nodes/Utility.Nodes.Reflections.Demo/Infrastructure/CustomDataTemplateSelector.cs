using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Objects;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using Utility.ViewModels;
using Utility.WPF.Factorys;
using Utility.WPF.Helpers;

namespace Utility.Nodes.Demo
{
    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            // collection item
            if (item is IReadOnlyTree { Data: PropertyData { Descriptor: CollectionItemDescriptor { } } })
            {
                return MakeTemplate(item, "None");
            }
            // parameter
            if (item is ParameterNode { Data: PropertyData { Descriptor: { } } })
            {
                return MakeTemplate(item);
            }
            // methods
            if (item is MethodsNode { Data: { } })
            {
                return MakeTemplate(item, "None");
            }
            // method
            if (item is MethodNode { Data: PropertyData { Descriptor: { } } })
            {
                return MakeTemplate(item);
            }
            // root
            if (item is IReadOnlyTree { Data: PropertyData { Descriptor: RootDescriptor { } } })
            {
                return MakeTemplate(item, "OnlyHeader");
            }
            // default
            if (item is IReadOnlyTree { Data: ObjectValue { Descriptor: { } } })
            {
                return MakeTemplate(item, "OnlyHeader");
            }
            // inner collection item descriptor
            if (item is IReadOnlyTree { Parent.Data: PropertyData { Descriptor: CollectionItemDescriptor { Index: { } index, ComponentType: { } componentType, DisplayName: { } displayName } descriptor } baseObject })
            {
                if (index == 0)
                    return MakeTemplate(item, "TopHeader");
                else if (index > 0)
                    return MakeTemplate(item, "NoHeader");
            }
            // other
            if (item is Node { Data: var data })
            {
                return MakeTemplate(item);
            }

            throw new System.Exception("743£");
        }

        private DataTemplate MakeTemplate(object item, string? name = null)
        {
            return TemplateGenerator.CreateDataTemplate(() =>
            {
                var binding = new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(Node.Data)), Source = item };
                var contentControl = new ContentControl { };
                if (name is string _name)
                    contentControl.ContentTemplate = this.FindResource<DataTemplate>(_name);
                contentControl.SetBinding(ContentControl.ContentProperty, binding);
                return contentControl;
            });
        }
    }
}
