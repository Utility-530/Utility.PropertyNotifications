using System.Collections;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using AssemblySearch;
using Autofac;
using Org.BouncyCastle.Tls;
using Utility.Interfaces;
using Utility.Interfaces.Exs;
using Utility.Interfaces.NonGeneric;
using Utility.Models;
using Utility.Nodes.Ex;
using Utility.Trees.Abstractions;
using Utility.Trees.Extensions;
using Utility.WPF.Controls.ComboBoxes;
using Utility.WPF.Demo.ComboBoxes.Infrastructure;
using Utility.WPF.Factorys;
using Utility.WPF.Helpers;
using Utility.WPF.Reactives;

namespace Utility.WPF.Demo.ComboBoxes
{
    public class ParameterSelectorBehavior : TreeSelectorBehavior
    {
        private ParameterSearchFilter filter = new ParameterSearchFilter(null, null, null, null);
        //private NodeEngine nodeEngine = new();

        public static readonly DependencyProperty AssembliesProperty = DependencyProperty.Register(nameof(Assemblies), typeof(IEnumerable), typeof(ParameterSelectorBehavior), new PropertyMetadata(assembliesChanged));
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(nameof(Type), typeof(Type), typeof(ParameterSelectorBehavior), new PropertyMetadata(typeChanged));
        public static readonly DependencyPropertyKey ParameterProperty = DependencyProperty.RegisterReadOnly(nameof(Parameter), typeof(Type), typeof(ParameterSelectorBehavior), new PropertyMetadata(parameterChanged));
        public static readonly DependencyProperty UseEntryAssemblyProperty = DependencyProperty.Register(nameof(UseEntryAssembly), typeof(bool), typeof(ParameterSelectorBehavior), new PropertyMetadata(false));
        public static readonly DependencyProperty IsStaticProperty = DependencyProperty.Register(nameof(IsStatic), typeof(bool?), typeof(ParameterSelectorBehavior), new PropertyMetadata(isStaticChanged));
        public static readonly DependencyProperty MethodNameMatchProperty = DependencyProperty.Register(nameof(MethodNameMatch), typeof(string), typeof(ParameterSelectorBehavior), new PropertyMetadata(methodNameMatchChanged));

        private static void methodNameMatchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ParameterSelectorBehavior { } selector && e.NewValue is string str)
            {
                selector.Filter = selector.Filter with { MethodNameMatch = str };
            }
        }

        private static void isStaticChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ParameterSelectorBehavior { } selector && e.NewValue is bool b)
            {
                selector.Filter = selector.Filter with { IsStatic = b };
            }
        }

        private static void parameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ParameterSelectorBehavior { AssociatedObject.ItemsSource: IReadOnlyTree tree } selector)
            {
                if (e.NewValue is ParameterInfo param)
                    selector.changeParam(tree, param);
            }
        }

        private static void typeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ParameterSelectorBehavior { } selector && e.NewValue is Type type)
            {
                selector.Filter = selector.Filter with { ParameterType = type };
            }
        }

        private static void assembliesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ParameterSelectorBehavior selector && e.NewValue is IEnumerable<Assembly> enumerable)
            {
                selector.Filter = selector.Filter with { Assemblies = enumerable };
            }
        }

        protected override void OnAttached()
        {
            this.AssociatedObject.ValueCoercing += (s, e) =>
            {
                if (e.NewValue is not IGetValue { Value: Type { } type })
                {
                    e.Cancel = true;
                }
            };

            this.AssociatedObject
                .Observe(a => a.SelectedNode)
                .OfType<IGetValue>()
                .Select(a => a.Value)
                .Subscribe(a =>
                {
                    if (a is ParameterInfo param)
                    {
                        SetValue(ParameterProperty, param);
                    }
                    else if (a is IParameterInfo { Parameter: { } _param })
                    {
                        SetValue(ParameterProperty, _param);
                    }
                });

            if (UseEntryAssembly)
            {
                Assemblies = new List<Assembly>([Assembly.GetEntryAssembly()]);
            }
            if (Assemblies != null)
                Filter = Filter with { Assemblies = Assemblies.Cast<Assembly>() };

            AssociatedObject.OnLoaded(a =>
            {
                if (Parameter is ParameterInfo param && AssociatedObject.TreeView != null && this.AssociatedObject.ItemsSource is IReadOnlyTree tree)
                {
                    changeParam(tree, param);
                }
            });

            AssociatedObject.Filter = new Model(() =>
            [
                new Model() { Name = "search", DataTemplate = "SearchTemplate" },
                new Model() { Name = "assembly", DataTemplate = "AssembliesTemplate" },
                new Model() { Name = "type", DataTemplate = "TypeTemplate" },
                new Model(()=>[
                         new Model() { Name = "isStatic", DataTemplate = "BooleanTemplate" },
                    ]) { Name = "method", DataTemplate = "MethodTemplate" },
            ]);

            base.OnAttached();
        }

        private void changeParam(IReadOnlyTree tree, ParameterInfo param)
        {
            if (tree.Descendant(a => (a.tree.Value() is ParameterInfo _param && _param == param) || (a.tree.Value() is IParameterInfo iparam && iparam.Parameter == param)) is IReadOnlyTree { } innerTree)
            {
                AssociatedObject.IsError = false;
                AssociatedObject.UpdateSelectedItems(innerTree);
                if (AssociatedObject.TreeView?.ItemContainerGenerator.ContainerFromItem(AssociatedObject.TreeView.SelectedItem) is TreeViewItem item)
                    item.IsSelected = true;
                AssociatedObject.SelectedNode = innerTree;
            }
            else
            {
                AssociatedObject.IsError = true;
            }
        }

        public INodeRoot NodeRoot { get; set; }

        public ParameterSearchFilter Filter
        {
            get => filter;
            set
            {
                filter = value;
                var assemblyTree = AssemblyParameterSearcher.Search(value).ToArray();
                NodeRoot.Create(assemblyTree).Subscribe();      
                AssociatedObject.ItemsSource = assemblyTree;
            }
        }

        public string MethodNameMatch
        {
            get { return (string)GetValue(MethodNameMatchProperty); }
            set { SetValue(MethodNameMatchProperty, value); }
        }

        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public ParameterInfo Parameter
        {
            get { return (ParameterInfo)GetValue(ParameterProperty.DependencyProperty); }
            //set { SetValue(ParameterProperty, value); }
        }

        public IEnumerable Assemblies
        {
            get { return (IEnumerable)GetValue(AssembliesProperty); }
            set { SetValue(AssembliesProperty, value); }
        }

        public bool UseEntryAssembly
        {
            get { return (bool)GetValue(UseEntryAssemblyProperty); }
            set { SetValue(UseEntryAssemblyProperty, value); }
        }

        public bool? IsStatic
        {
            get { return (bool?)GetValue(IsStaticProperty); }
            set { SetValue(IsStaticProperty, value); }
        }

        public override DataTemplateSelector SelectedTemplateSelector() =>
            DataTemplateHelper.CreateSelector((item, b) =>
            {
                if (item is IGetData { Data: var data } tree)
                {
                    if (data is ParameterInfo)
                        return TemplateGenerator.CreateDataTemplate(() =>
                        {
                            var textBlock = new TextBlock { };
                            Binding binding = new() { Path = new PropertyPath(nameof(System.Reflection.ParameterInfo.Name)) };
                            Binding binding2 = new() { Path = new PropertyPath(nameof(IGetData.Data)) };
                            textBlock.SetBinding(TextBlock.TextProperty, binding);
                            textBlock.SetBinding(TextBlock.DataContextProperty, binding2);
                            return textBlock;
                        });
                    if (data is IParameterInfo)
                        return TemplateGenerator.CreateDataTemplate(() =>
                        {
                            var textBlock = new TextBlock { };
                            Binding binding = new() { Path = new PropertyPath(nameof(System.Reflection.ParameterInfo.Name)) };
                            Binding binding2 = new() { Path = new PropertyPath(nameof(IGetData.Data) +"."+ nameof(IParameterInfo.Parameter)) };
                            textBlock.SetBinding(TextBlock.TextProperty, binding);
                            textBlock.SetBinding(TextBlock.DataContextProperty, binding2);
                            return textBlock;
                        });
                    else if (data is Assembly assembly)
                        return TemplateGenerator.CreateDataTemplate(() =>
                        {
                            var textBlock = new TextBlock { Text = assembly.GetName().Name };
                            return textBlock;
                        });
                    return TemplateGenerator.CreateDataTemplate(() => new Ellipse { Fill = Brushes.Black, Height = 2, Width = 2, VerticalAlignment = VerticalAlignment.Bottom, ToolTip = new ContentControl { Content = data }, Margin = new Thickness(4, 0, 4, 0) });
                }
                throw new Exception("d ss!$sd");
            });
    }
}