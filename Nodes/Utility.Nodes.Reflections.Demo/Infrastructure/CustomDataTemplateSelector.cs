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

                if (item is ITree { Data: ObjectValue { } })
                {
                    return MakeTemplate(item, "None");
                }
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
                if (item is CustomMethodsNode { Data: { } })
                {
                    return MakeTemplate(item, "None");
                }
                // method
                if (item is MethodNode { Data: PropertyData { Descriptor: { } } })
                {
                    return MakeTemplate(item);
                    
                }
                // root
                if (item is IReadOnlyTree { Data: PropertyData { Descriptor: RootDescriptor { }, } })
                {
                    return MakeHeaderTemplate(item, depth);
                }
                // default
                if (item is IReadOnlyTree { Data: ObjectValue { Descriptor: { } } })
                {
                    return MakeHeaderTemplate(item, depth);
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
                else
                {

                }
            }
            else if (index[1] == 2)
            {
                return MakeTemplate(item, "ViewModel");

            }

            else if (index[1] == 3)
            {
                if (depth == 2)
                {
                    if (index[2] == 0)
                        return MakeButtonTemplate(item, "Refresh");
                    if (index[2] == 1)
                        return MakeButtonTemplate(item, "Save");
                }
                else
                {
                    return MakeTemplate(item, "None");
                }
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
        private DataTemplate MakeHeaderTemplate(object item, int count)
        {
            return TemplateGenerator.CreateDataTemplate(() =>
            {

                var textBlock = new TextBlock
                {
                    FontWeight = FontWeight.FromOpenTypeWeight(600 - count * 10),
                    FontSize = 18 - count * 0.5
                };
                textBlock.SetBinding(TextBlock.TextProperty, Binding(item));
              
                return textBlock;
            });

            static Binding Binding(object item)
            {
                return new(nameof(PropertyData.Name))
                {
                    Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter,
                    Path = new PropertyPath($"{nameof(Node.Data)}.{nameof(PropertyData.Name)}"),
                    Source = item
                };
            }
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

        private DataTemplate MakeButtonTemplate(object item, string name)
        {
            return TemplateGenerator.CreateDataTemplate(() =>
            {
                var contentControl = new Button
                {
                    Content = name
                };
                contentControl.Click += (s, e) => replay.OnNext(name);
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
