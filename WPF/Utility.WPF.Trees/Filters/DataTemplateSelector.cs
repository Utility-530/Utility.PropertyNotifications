using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Splat;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.Generic;
using Utility.Interfaces.NonGeneric;
using Utility.Nodes.WPF;
using Utility.PropertyDescriptors;
using Utility.Trees.Abstractions;
using Utility.Trees.Decisions;
using Utility.WPF.Controls;
using Utility.WPF.Factorys;

namespace Utility.WPF.Trees.Filters
{
    public class DataTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        private bool isInitialised;
        private ResourceDictionary? res;
        private DependencyObject container;

        public static DataTemplateSelector Instance { get; } = new();

        public DecisionTree Predicate { get; set; }

        private DataTemplateSelector()
        {
            Predicate = new DataTemplateDecisionTree(new Decision(item => (item as IReadOnlyTree) != null) { })
                {
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item as IHeaderDescriptor!=null), md=> ()=>MakeHeaderTemplate(md, 1)),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item as IMethodDescriptor!=null){  },md=>()=>MakeButtonTemplate(md)),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item as IPropertiesDescriptor!=null){  },md=>()=>MakeEmptyTemplate(md)),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item as ICollectionDescriptor!=null){  },md=>()=>MakeEmptyTemplate(md)),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item as ICollectionHeadersDescriptor !=null){  },md=>()=>MakeEmptyTemplate(md)),
                    new DataTemplateDecisionTree<IReadOnlyTree>(new Decision<IReadOnlyTree>(item => (item as IReferenceDescriptor) != null),  md =>()=> MakeHeaderTemplate(md, md.Depth)),
                    //{
                    //    new DataTemplateDecisionTree<IReadOnlyTree>(new Decision<IReadOnlyTree>(item => true), )
                    //},
                    //new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionItemDescriptor!=null){  }, md=>MakeLineTemplate()),
                    new DataTemplateDecisionTree(new Decision<IGetParent<IReadOnlyTree>>(item => item.Parent!=null){  })
                    {
                        new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => true){  }, md=>() => MakeTemplate(md)),
                        //new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => ((item.Parent ).Parent!=null)){  })
                        //{
                        //    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => (((item.Parent ).Parent).Data as ICollectionItemDescriptor!=null)){  }, md=> MakeTemplate(md)),
                        //},
                    },
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item as IValueDescriptor != null){  }, md=> ()=>MakeTemplate(md)),
                    new DataTemplateDecisionTree<IReadOnlyTree>(new Decision<IReadOnlyTree>(item => (item as IPropertyDescriptor) != null), md =>()=> MakePropertyTemplate(md, md.Depth)),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => true), md=> () => MakeVoidTemplate()),
                };
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            //return base.SelectTemplate(item, container);
            this.container = container;
            Predicate.Reset();
            Predicate.Input = item;
            Predicate.Evaluate();
            return (DataTemplate)Predicate.Backput;
        }

        private DataTemplate MakeTemplate(object item)
        {
            double? width = null;

            var template = TemplateGenerator.CreateHierarcialDataTemplate(() =>
            {
                var binding = new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath("."), Source = item };
                var contentControl = new ContentPresenter
                {
                    ContentTemplateSelector = Locator.Current.GetService<System.Windows.Controls.DataTemplateSelector>()
                    //ContentTemplateSelector =// CustomDataTemplateSelector.Instance
                };
                if (width.HasValue)
                    contentControl.Width = width.Value;

                contentControl.SetBinding(ContentPresenter.ContentProperty, binding);
                return contentControl;
            }, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Children)), Source = item });

            return template;
        }

        private DataTemplate MakeEmptyTemplate(object item)
        {
            return TemplateGenerator.CreateHierarcialDataTemplate(() =>
            {
                return new System.Windows.Shapes.Rectangle { Fill = Brushes.Red, Height = 20, Width = 20 };
            }, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Children)), Source = item });
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
            //if (isInitialised == false)
            //{
            //    var uri = new Uri(@$"{typeof(StyleSelector).Assembly.GetName()};component/Themes/ContentStyles.xaml", UriKind.RelativeOrAbsolute);
            //    //var res = new ResourceDictionary() { Source = uri };
            //    res = Application.LoadComponent(uri) as ResourceDictionary;
            //    Application.Current.Resources.Add(uri, res);
            //    //var _style = App.Current.Resources["GenericTreeViewItem"] as System.Windows.Style;
            //    //_style.Setters.Add(new Setter { Property = TreeViewItem.HeaderProperty, Value = new Binding { Path = new PropertyPath(".") } });
            //    isInitialised = true;
            //}

            return TemplateGenerator.CreateHierarcialDataTemplate(() =>
            {
                return new ContentControl
                {
                    //Style = res["LabelValueContent"] as Style,
                    //HeaderTemplate = TemplateGenerator.CreateDataTemplate(() =>
                    //{
                    //    var textBlock = new TextBlock
                    //    {
                    //        FontWeight = FontWeight.FromOpenTypeWeight(600 - count * 10),
                    //        FontSize = 18 - count * 0.5
                    //    };
                    //    textBlock.SetBinding(TextBlock.TextProperty, Binding(item));
                    //    return textBlock;
                    //}),
                    //Header = item,
                    Content = item,
                    ContentTemplate = TemplateGenerator.CreateDataTemplate(() =>
                    {
                        var textBlock = new TextBlock
                        {
                            FontWeight = FontWeight.FromOpenTypeWeight(600 - count * 10),
                            FontSize = 18 - count * 0.5
                        };
                        textBlock.SetBinding(TextBlock.TextProperty, Binding(item));
                        return textBlock;
                    }),
                };
            }, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Children)), Source = item });

            static Binding Binding(object item)
            {
                return new()
                {
                    Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter,
                    Path = new PropertyPath($"{nameof(IDescriptor.Name)}"),
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

                if (item is IDescriptor { } descriptor)
                    if (item is IGetValue { Value: { } value } _value)
                    {
                        toggle.IsChecked = true;
                    }
                toggle.IsCheckedChanged += Toggle_Click;
                return toggle;

                void Toggle_Click(object sender, RoutedEventArgs e)
                {
                    if (item is IDescriptor { } descriptor)
                        if (item is ISetValue _setValue)
                            if (item is IValue _value)
                                if (item is IValueChanges _values)
                                {
                                    _values.Subscribe(a =>
                                    {
                                        if (a.Equals(default))
                                            toggle.IsChecked = false;
                                        else
                                            toggle.IsChecked = true;
                                    });
                                    if (toggle.IsChecked == true && (_value as IGetValue).Value == null)
                                    {
                                        _setValue.Value = Activator.CreateInstance(descriptor.Type);
                                    }
                                    else if (toggle.IsChecked != true && (_value as IGetValue).Value != null)
                                    {
                                        _setValue.Value = null;
                                    }
                                }
                }
            }, new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Children)), Source = item });

            static Binding Binding(object item)
            {
                return new()
                {
                    Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter,
                    Path = new PropertyPath($"{nameof(IDescriptor.Name)}"),
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
                //button.SetBinding(Button.CommandProperty, Binding2(item));
                button.Click += (s, e) => (item as IMethodDescriptor ?? throw new Exception("F 333"))?.Invoke();
                return button;
            });

            static Binding Binding(object item)
            {
                return new()
                {
                    Path = new PropertyPath($"{nameof(IMethodDescriptor.Name)}"),
                    Converter = Utility.WPF.Converters.LambdaConverter.HumanizerConverter,
                    Source = item
                };
            }
        }
    }
}