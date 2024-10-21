using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Utility.Descriptors;
using Utility.Interfaces;
using Utility.Interfaces.NonGeneric;
using Utility.Trees.Abstractions;
using Utility.Trees.Decisions;
using Utility.Trees.Demo.MVVM.Infrastructure;
using Utility.WPF.Factorys;
using Utility.WPF.Templates;

namespace Utility.Trees.Demo.MVVM
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
                        //new DataTemplateDecisionTree<IReadOnlyTree>(new Decision<IReadOnlyTree>(item => (item.Data as IReferenceDescriptor).Instance as Table != null),  md => MakeVoidTemplate()),
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
            //return MakeTemplate(item);
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

                return textBlock;
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
