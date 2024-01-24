using System;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Objects;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using Utility.WPF.Factorys;
using Utility.WPF.Helpers;

namespace Utility.Nodes.Demo
{
    public class CustomDataTemplateSelector : DataTemplateSelector, IObservable<object>
    {
        readonly ReplaySubject<object> replay = new(1);

        private CustomDataTemplateSelector()
        {
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is not ITree { Depth: { } depth, Index: { } index } tree)
                throw new System.Exception("f 2233");

            if (depth == 0)
            {
                if (item is Node { Data: var data })
                {
                    return MakeTemplate(item, "None");
                }
            }
            if (index[1] == 0)
            {

                // collection item
                if (item is ITree { Data: PropertyData { Descriptor: CollectionItemDescriptor { } } })
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
                if (item is IReadOnlyTree { Parent.Data: PropertyData { Descriptor: CollectionItemDescriptor { Index: { } _index, ComponentType: { } componentType, DisplayName: { } displayName } descriptor } baseObject })
                {
                    if (_index == 0)
                        return MakeTemplate(item, "TopHeader");
                    else if (_index > 0)
                        return MakeTemplate(item, "NoHeader");
                }
                // other
                if (item is Node { Data: var data })
                {
                    return MakeTemplate(item);
                }
            }
            else if (index[1] == 1)
            {
                if (item is Node { Data: var data })
                {
                    return MakeButtonTemplate(item);
                }
            }
            else if (index[1] == 2)
            {
                return MakeTemplate(item, "ViewModel");

            }

            else if (index[1] == 3)
            {
                return MakeRefreshTemplate(item);

            }
            else if (index[1] > 3)
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


        private DataTemplate MakeButtonTemplate(object item)
        {
            return TemplateGenerator.CreateDataTemplate(() =>
            {
                var binding = new Binding { Mode = BindingMode.OneWay, /*Path = new PropertyPath(nameof(Node.Data)),*/ Source = item };
                var contentControl = new Button
                {
                    ContentTemplate = this.FindResource<DataTemplate>("Key")
                };
                contentControl.Click += ContentControl_Click;
                contentControl.SetBinding(ContentControl.ContentProperty, binding);
                return contentControl;
            });
        }

        private DataTemplate MakeRefreshTemplate(object item)
        {
            return TemplateGenerator.CreateDataTemplate(() =>
            {
        
                var contentControl = new Button
                {
                    Content = "Refresh"
                };
                contentControl.Click += (s,e) => replay.OnNext("refresh");
                return contentControl;
            });
        }

        private void ContentControl_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button { Content: { } content })
                replay.OnNext(content);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return replay.Subscribe(observer);
        }

        public static CustomDataTemplateSelector Instance { get; } = new();
    }

}
