using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Utility.Descriptors;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using Utility.Trees.Decisions;
using Utility.WPF.Controls;
using Utility.WPF.Factorys;
using Utility.WPF.Templates;

namespace Utility.Nodes.WPF
{
    public class DataTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        private bool isInitialised;
        private ResourceDictionary? res;

        public static DataTemplateSelector Instance { get; } = new();

        public DecisionTree Predicate { get; set; }

        private DataTemplateSelector()
        {
            Predicate = new DataTemplateDecisionTree(new Decision(item => (item as IReadOnlyTree) != null) { })
                {
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IHeaderDescriptor!=null), md=> ()=>MakeHeaderTemplate(md, 1)),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IMethodDescriptor!=null){  },md=>()=>MakeButtonTemplate(md)),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IPropertiesDescriptor!=null){  },md=>()=>MakeEmptyTemplate(md)),
                    new DataTemplateDecisionTree<IReadOnlyTree>(new Decision<IReadOnlyTree>(item => (item.Data as IReferenceDescriptor) != null),  md =>()=> MakeHeaderTemplate(md, md.Depth)),
                    //{
                    //    new DataTemplateDecisionTree<IReadOnlyTree>(new Decision<IReadOnlyTree>(item => true), )
                    //},
                    //new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionItemDescriptor!=null){  }, md=>MakeLineTemplate()),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item.Parent!=null){  })
                    {
                        new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => (item.Parent .Data as ICollectionItemDescriptor!=null)){  }, md=>() => MakeTemplate(md)),
                        //new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => ((item.Parent ).Parent!=null)){  })
                        //{
                        //    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => (((item.Parent ).Parent).Data as ICollectionItemDescriptor!=null)){  }, md=> MakeTemplate(md)),
                        //},
                    },
                    new DataTemplateDecisionTree<IReadOnlyTree>(new Decision<IReadOnlyTree>(item => (item.Data as IPropertyDescriptor) != null), md =>()=> MakePropertyTemplate(md, md.Depth)),
                    //new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IValueDescriptor != null){  }, md=> ()=>MakeTemplate(md)),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => true), md=> () => MakeVoidTemplate()),
                };
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Predicate.Reset();
            Predicate.Input = item;
            Predicate.Evaluate();
            return (DataTemplate)Predicate.Backput;
        }

        private DataTemplate MakeTemplate(object item)
        {
            var template = TemplateGenerator.CreateHierarcialDataTemplate(() =>
            {
                var binding = new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Data)), Source = item };
                var contentControl = new ContentPresenter
                {
                    ContentTemplateSelector = CustomDataTemplateSelector.Instance
                };
                contentControl.SetBinding(ContentPresenter.ContentProperty, binding);
                return contentControl;
            }, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Items)), Source = item });

            return template;
        }

        private DataTemplate MakeEmptyTemplate(object item)
        {
            return TemplateGenerator.CreateHierarcialDataTemplate(() =>
            {
                return new Control();
            }, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Items)), Source = item });
        }


        private DataTemplate MakeVoidTemplate()
        {
            return TemplateGenerator.CreateDataTemplate(() =>
            {
                var contentControl = new TextBlock { Text = "---", FontWeight = FontWeights.Bold };
                return contentControl;
            });
        }

        private DataTemplate MakePropertyTemplate(object item, int count)
        {
            if (isInitialised == false)
            {
                var uri = new Uri(@$"{typeof(StyleSelector).Assembly.GetName()};component/Themes/ContentStyles.xaml", UriKind.RelativeOrAbsolute);
                //var res = new ResourceDictionary() { Source = uri };
                res = Application.LoadComponent(uri) as ResourceDictionary;
                Application.Current.Resources.Add(uri, res);
                //var _style = App.Current.Resources["GenericTreeViewItem"] as System.Windows.Style;
                //_style.Setters.Add(new Setter { Property = TreeViewItem.HeaderProperty, Value = new Binding { Path = new PropertyPath(".") } });
                isInitialised = true;
            }


            return TemplateGenerator.CreateHierarcialDataTemplate(() =>
            {
                return new HeaderedContentControl
                {
                    Style = res["LabelValueContent"] as Style,
                    HeaderTemplate = TemplateGenerator.CreateDataTemplate(() =>
                    {
                        var textBlock = new TextBlock
                        {
                            FontWeight = FontWeight.FromOpenTypeWeight(600 - count * 10),
                            FontSize = 18 - count * 0.5
                        };
                        textBlock.SetBinding(TextBlock.TextProperty, Binding(item));
                        return textBlock;
                    }),
                    Header = item,
                    Content = item,
                    ContentTemplate = MakeTemplate(item)
                };
            }, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Items)), Source = item });

            static Binding Binding(object item)
            {
                return new()
                {
                    Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter,
                    Path = new PropertyPath($"{nameof(IReadOnlyTree.Data)}.{nameof(IDescriptor.Name)}"),
                    Source = item
                };
            }
        }


        private DataTemplate MakeHeaderTemplate(object item, int count)
        {
            return TemplateGenerator.CreateHierarcialDataTemplate(() =>
            {
                var textBlock = new TextBlock
                {
                    FontWeight = FontWeight.FromOpenTypeWeight(600 - count * 10),
                    FontSize = 18 - count * 0.5
                };
                textBlock.SetBinding(TextBlock.TextProperty, Binding(item));

                var toggle = new DoubleClickCheckBox() { Content = textBlock/*, Style = App.Current.Resources["NullToggleButton"] as Style*/ };

                if (item is IReadOnlyTree { Data: IDescriptor { } descriptor } data)
                    if (data is IValue { Value: { } value } _value)
                    {
                        toggle.IsChecked = true;
                    }
                toggle.IsCheckedChanged += Toggle_Click;
                return toggle;

                void Toggle_Click(object sender, RoutedEventArgs e)
                {
                    if (item is IReadOnlyTree { Data: IDescriptor { } descriptor } data)
                        if (data is ISetValue _setValue)
                            if (data is IValue _value)
                                if (data is IValueChanges _values)
                                {
                                    _values.Subscribe(a =>
                                    {
                                        if (a.Equals(default))
                                            toggle.IsChecked = false;
                                        else
                                            toggle.IsChecked = true;
                                    });
                                    if (toggle.IsChecked == true && _value.Value == null)
                                    {
                                        _setValue.Value = Activator.CreateInstance(descriptor.Type);
                                    }
                                    else if (toggle.IsChecked != true && _value.Value != null)
                                    {
                                        _setValue.Value = null;
                                    }
                                }
                }
            }, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Items)), Source = item });

            static Binding Binding(object item)
            {
                return new()
                {
                    Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter,
                    Path = new PropertyPath($"{nameof(IReadOnlyTree.Data)}.{nameof(IDescriptor.Name)}"),
                    Source = item
                };
            }
        }

        private DataTemplate MakeButtonTemplate(object item)
        {
            return TemplateGenerator.CreateDataTemplate(() =>
            {

                var button = new Button
                {

                };
                button.SetBinding(Button.ContentProperty, Binding(item));
                button.SetBinding(Button.CommandProperty, Binding2(item));

                return button;
            });

            static Binding Binding(object item)
            {
                return new()
                {
                    Path = new PropertyPath($"{nameof(IReadOnlyTree.Data)}.{nameof(IMethodDescriptor.Name)}"),
                    Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter,
                    Source = item
                };
            }
            static Binding Binding2(object item)
            {
                return new()
                {

                    Path = new PropertyPath($"{nameof(IReadOnlyTree.Data)}.{nameof(IMethodDescriptor.Command)}"),
                    Source = item
                };
            }
        }
    }
}
