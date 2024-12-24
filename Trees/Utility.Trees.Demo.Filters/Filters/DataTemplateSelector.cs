using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Descriptors;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using Utility.Trees.Decisions;
using Utility.Trees.Demo.Filters;
using Utility.Trees.Demo.Filters.Infrastructure;
using Utility.WPF.Controls;
using Utility.WPF.Factorys;
using Utility.WPF.Templates;

namespace Utility.Trees.Demo.Filters
{
    public class DataTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public static DataTemplateSelector Instance { get; } = new();

        public DecisionTree Predicate { get; set; }

        private DataTemplateSelector()
        {
            Predicate = new DataTemplateDecisionTree(new Decision(item => (item as IReadOnlyTree) != null) { })
                {
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IHeaderDescriptor!=null), md=> MakeHeaderTemplate(md, 1)),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IMethodDescriptor!=null){  },md=>MakeButtonTemplate(md)),
                    new DataTemplateDecisionTree<IReadOnlyTree>(new Decision<IReadOnlyTree>(item => (item.Data as IReferenceDescriptor) != null))
                    {
                        new DataTemplateDecisionTree<IReadOnlyTree>(new Decision<IReadOnlyTree>(item => true),  md => MakeHeaderTemplate(md, md.Depth))
                    },
                    //new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as ICollectionItemDescriptor!=null){  }, md=>MakeLineTemplate()),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item.Parent!=null){  })
                    {
                        new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => (item.Parent .Data as ICollectionItemDescriptor!=null)){  }, md=>MakeTemplate(md)),
                        //new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => ((item.Parent ).Parent!=null)){  })
                        //{
                        //    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => (((item.Parent ).Parent).Data as ICollectionItemDescriptor!=null)){  }, md=> MakeTemplate(md)),
                        //},
                    },
                    new DataTemplateDecisionTree<IReadOnlyTree>(new Decision<IReadOnlyTree>(item => (item.Data as PropertyDescriptor) != null), md => MakeHeaderTemplate(md, md.Depth)),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => item.Data as IValueDescriptor != null){  }, md=>MakeTemplate(md)),
                    new DataTemplateDecisionTree(new Decision<IReadOnlyTree>(item => true), md=> MakeVoidTemplate()),
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
            return TemplateGenerator.CreateDataTemplate(() =>
            {
                var binding = new Binding { Mode = BindingMode.OneWay, Path = new PropertyPath(nameof(IReadOnlyTree.Data)), Source = item };
                var contentControl = new ContentPresenter
                {
                    ContentTemplateSelector = CustomDataTemplateSelector.Instance
                };
                //if (name is string _name)
                //    contentControl.ContentTemplate = ResourceHelper.FindResource<DataTemplate>(_name);
                contentControl.SetBinding(ContentPresenter.ContentProperty, binding);

                return contentControl;
            });
        }


        private DataTemplate MakeVoidTemplate()
        {
            return TemplateGenerator.CreateDataTemplate(() =>
            {
                var contentControl = new TextBlock { Text = "---", FontWeight = FontWeights.Bold };
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

                var toggle = new DoubleClickCheckBox () { Content = textBlock, Style = App.Current.Resources["NullToggleButton"] as Style };

                if (item is IReadOnlyTree { Data: IDescriptor { } descriptor } data)
                    if (data is IValue { Value:{ } value } _value)
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
                                        if (a != null)
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
            });

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
