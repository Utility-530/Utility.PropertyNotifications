using System;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.PropertyDescriptors;
using Utility.Trees;
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
                throw new Exception("f 2233");

            if (tree.MatchAncestor(a => a is MasterSlaveNode { Value: 0 }) is { } _x)
                if (sd(item, depth) is DataTemplate dataTemplate)
                {
                    return dataTemplate;
                }

            if (tree.MatchAncestor(a => a is MasterSlaveNode { Value: 1 }) is { } x)
                return MakeButtonTemplate(item);


            if (tree.MatchAncestor(a => a is SelectionNode) is { })
            {
                if (sd(item, depth) is DataTemplate dataTemplate)
                {
                    return dataTemplate;
                }

            }
            //else if (index[1] == 2)
            //{
            //    return MakeTemplate(item);
            //}

            if (tree.MatchAncestor(a => a is CustomMethodsNode) is { })
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
            //else if (index[2] > 3)
            //{
            //    return MakeTemplate(item);
            //}

            if (item is IReadOnlyTree { Data: IMemberDescriptor { IsValueOrStringProperty: { } isValueOrString } })
            {
                if (isValueOrString)
                    return MakeTemplate(item);
                else
                    return MakeTemplate(item, "OnlyHeader");
            }
            // other
            //if (item is IReadOnlyTree { Data: var data })
            //{
            return MakeTemplate(item);
            //}
        }

        public DataTemplate sd(object item, int depth)
        {
            if (item is SlaveNode { })
            {
                return MakeHeaderTemplate2(item, depth);
            }
            if (item is IReadOnlyTree { Data: IPropertiesDescriptor { } })
            {
                return MakeTemplate(item, "None");
            }
            if (item is CustomMethodsNode { Data: { } })
            {
                return MakeTemplate(item, "None");
            }

            if (item is IReadOnlyTree { Parent.Data: CollectionItemDescriptor { } _ })
            {
                return MakeTemplate(item, "None");
            }

            if (item is IReadOnlyTree { Parent.Parent.Data: CollectionItemDescriptor { Index: { } _index } _ })
            {
                if (_index == 0)
                    return MakeTemplate(item, "TopHeader");
                else if (_index > 0)
                    return MakeTemplate(item, "NoHeader");
            }
            // root
            if (item is RootNode { })
            {
                return MakeTemplate(item, "None");
            }
            // root
            if (item is IReadOnlyTree { Data: RootDescriptor { } })
            {
                //return MakeHeaderTemplate(item, depth);
                return MakeTemplate(item, "None");
            }
            // default
            if (item is IReadOnlyTree { Data: ObjectValue { Descriptor: { } } })
            {
                return MakeHeaderTemplate(item, depth);
            }
            // collection item
            if (item is ITree { Data: ICollectionItemDescriptor { } })
            {
                return MakeTemplate(item, "None");
            }
            // inner collection item descriptor

            // parameter
            //if (item is ParameterNode { Data: PropertyData { Descriptor: { } } })
            //{
            //    return MakeTemplate(item);
            //}
            // methods
            if (item is IReadOnlyTree { Data: IMethodsDescriptor { } })
            {
                return MakeTemplate(item, "None");
            }
            if (item is IReadOnlyTree { Data: CollectionDescriptor { } })
            {
                return MakeTemplate(item, "None");
            }
            if (item is IReadOnlyTree { Data: IPropertiesDescriptor { } })
            {
                return MakeHeaderTemplate(item, depth);
            }
            // method
            if (item is IReadOnlyTree { Data: IMethodDescriptor { } })
            {
                return MakeTemplate(item, "Method");
            }
            return null;
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

        private DataTemplate MakeHeaderTemplate2(object item, int count)
        {
            return TemplateGenerator.CreateDataTemplate(() =>
            {
                return new TextBlock
                {
                    FontWeight = FontWeight.FromOpenTypeWeight(600 - count * 10),
                    FontSize = 18 - count * 0.5,
                    Text = item.ToString()
                };
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
                return new(nameof(IMemberDescriptor.Name))
                {
                    Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter,
                    Path = new PropertyPath($"{nameof(Node.Data)}.{nameof(IMemberDescriptor.Name)}"),
                    Source = item
                };
            }
        }

        private DataTemplate MakeButtonTemplate(object item)
        {
            return TemplateGenerator.CreateDataTemplate(() =>
            {
                var binding = new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(Node.Data)), Source = item };
                var contentControl = new Button
                {
                    ContentTemplate = this.FindResource<DataTemplate>("OnlyHeader")
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
